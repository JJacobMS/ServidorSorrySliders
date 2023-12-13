﻿using DatosSorrySliders;
using InterfacesServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServidorSorrySliders
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class ServicioComunicacionSorrySliders : ILobby, IChat
    {
        private Dictionary<string, List<ContextoJugador>> _jugadoresEnLineaLobby = new Dictionary<string, List<ContextoJugador>>();
        public void EntrarPartida(string uid, string jugadorCorreo)
        {
            CambiarSingle();
            Logger log = new Logger(this.GetType(), "ILobby");
            ContextoJugador jugadorNuevo = new ContextoJugador { CorreoJugador = jugadorCorreo, ContextoJugadorCallBack = OperationContext.Current};
            lock (_jugadoresEnLineaLobby)
            {
                ManejarOperationContext.AgregarOReemplazarJugadorContextoLista(_jugadoresEnLineaLobby, jugadorNuevo, uid);
                List<ContextoJugador> jugadoresDesconectados = new List<ContextoJugador>();
                foreach (ContextoJugador jugadorOperation in _jugadoresEnLineaLobby[uid])
                {
                    try
                    {
                        jugadorOperation.ContextoJugadorCallBack.GetCallbackChannel<ILobbyCallback>().JugadorEntroPartida();
                    }
                    catch (CommunicationException ex)
                    {
                        jugadoresDesconectados.Add(jugadorOperation);
                        log.LogWarn("Error comunicación con el cliente", ex);
                    }
                    catch (TimeoutException ex)
                    {
                        jugadoresDesconectados.Add(jugadorOperation);
                        log.LogWarn("Se agoto el tiempo de espera del cliente", ex);
                    }
                }
                EliminarLobbySistema(jugadoresDesconectados, uid);
            }
            CambiarMultiple();
        }

        public void SalirPartida(string uid)
        {
            CambiarSingle();
            Logger log = new Logger(this.GetType(), "ILobby");
            lock (_jugadoresEnLineaLobby)
            {
                string jugadorEliminado = ManejarOperationContext.EliminarJugadorDiccionario(_jugadoresEnLineaLobby, uid, OperationContext.Current);
                if (jugadorEliminado.Length > 0 && _jugadoresEnLineaLobby.ContainsKey(uid))
                {
                    List<ContextoJugador> jugadoresDesconectados = new List<ContextoJugador>();
                    foreach (ContextoJugador jugador in _jugadoresEnLineaLobby[uid])
                    {
                        try
                        {
                            jugador.ContextoJugadorCallBack.GetCallbackChannel<ILobbyCallback>().JugadorSalioPartida();
                        }
                        catch (CommunicationException ex)
                        {
                            jugadoresDesconectados.Add(jugador);
                            log.LogWarn("Error comunicación con el cliente", ex);
                        }
                        catch (TimeoutException ex)
                        {
                            jugadoresDesconectados.Add(jugador);
                            log.LogWarn("Se agoto el tiempo de espera del cliente", ex);
                        }
                    }
                    EliminarLobbySistema(jugadoresDesconectados, uid);
                }
            }
            CambiarMultiple();
        }


        public void IniciarPartida(string uid)
        {
            Console.WriteLine("S");
            Logger log = new Logger(this.GetType(), "ILobby");
            lock (_jugadoresEnLineaLobby)
            {
                if (!_jugadoresEnLineaLobby.ContainsKey(uid))
                {
                    return;
                }
                foreach (ContextoJugador contexto in _jugadoresEnLineaLobby[uid])
                {
                    try
                    {
                        contexto.ContextoJugadorCallBack.GetCallbackChannel<ILobbyCallback>().HostInicioPartida();
                    }
                    catch (CommunicationException ex)
                    {
                        log.LogWarn("Error comunicación con el cliente", ex);
                    }
                    catch (TimeoutException ex)
                    {
                        log.LogWarn("Se agoto el tiempo de espera del cliente", ex);
                    }
                }
            }
        }
        private bool CodigoPartidaExiste(string codigoPartida)
        {
            if (codigoPartida == null)
            {
                return false;
            }
            return _jugadoresEnLineaLobby.ContainsKey(codigoPartida);
        }

        public void ComprobarJugadoresExistentes(string uid)
        {
            List<string> cuentasSistema = ObtenerCuentasLobby(uid);
            if (cuentasSistema.Count <= 0)
            {
                return;
            }
            Logger log = new Logger(this.GetType(), "IInicioSesion");
            CambiarSingle();
            lock (_listaContextoJugadores)
            {
                List<ContextoJugador> jugadoresDesconectados = new List<ContextoJugador>();
                foreach (string correo in cuentasSistema)
                {
                    foreach (ContextoJugador jugador in _listaContextoJugadores.Where(jugadorLista => jugadorLista.CorreoJugador.Equals(correo)))
                    {
                        try
                        {
                            jugador.ContextoJugadorCallBack.GetCallbackChannel<IUsuarioEnLineaCallback>().ComprobarJugador();
                        }
                        catch (CommunicationException ex)
                        {
                            log.LogWarn("Error comunicación con el cliente", ex);
                            jugadoresDesconectados.Add(jugador);
                        }
                        catch (TimeoutException ex)
                        {
                            log.LogWarn("Se agoto el tiempo de espera del cliente", ex);
                            jugadoresDesconectados.Add(jugador);
                        }
                    }
                }
                EliminarLobbySistema(jugadoresDesconectados, uid);
            }
        }

        private List<string> ObtenerCuentasLobby(string uid)
        {
            lock (_jugadoresEnLineaLobby)
            {
                if (!_jugadoresEnLineaLobby.ContainsKey(uid))
                {
                    return new List<string>();
                }
                List<string> lista = new List<string>();
                foreach (ContextoJugador jugador in _jugadoresEnLineaLobby[uid])
                {
                    lista.Add(jugador.CorreoJugador);
                }
                return lista;
            }
        }

        private void EliminarLobbySistema(List<ContextoJugador> jugadores, string codigoPartida)
        {
            foreach (string jugadorCorreo in jugadores.Select(jugadorAEliminar => jugadorAEliminar.CorreoJugador)) 
            {
                SalirDelLobby(jugadorCorreo, codigoPartida);
                if (!jugadorCorreo.Contains("@"))
                {
                    EliminarCuentaProvisional(jugadorCorreo);
                }
                lock (_jugadoresEnLineaLobby)
                {
                    SalirPartida(codigoPartida);
                }
                lock (_listaContextoJugadores)
                {
                    SalirDelSistema(jugadorCorreo);
                }
            }            
        }
    }
}
