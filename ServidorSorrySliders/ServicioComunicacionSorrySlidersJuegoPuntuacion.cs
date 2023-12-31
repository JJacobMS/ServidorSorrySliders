﻿using DatosSorrySliders;
using InterfacesServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServidorSorrySliders
{
    public partial class ServicioComunicacionSorrySliders : IJuegoPuntuacion
    {
        private Dictionary<string, List<ContextoJugador>> _diccionarioPuntuacion = new Dictionary<string, List<ContextoJugador>>();  
        public void AgregarJugador(string uid, string correoElectronico)
        {
            CambiarSingle();
            ContextoJugador contextoJugador = new ContextoJugador()
            {
                CorreoJugador = correoElectronico,
                ContextoJugadorCallBack = OperationContext.Current
            };
            lock (_diccionarioPuntuacion) 
            {
                ManejarOperationContext.AgregarOReemplazarJugadorContextoLista(_diccionarioPuntuacion, contextoJugador, uid);
            }
            CambiarMultiple();
        }

        public void EliminarJugador(string uid, string correoElectronico)
        {
            CambiarSingle();
            lock (_diccionarioPuntuacion)
            {
                if (_diccionarioPuntuacion.ContainsKey(uid)) 
                {
                    int posicionJugador = ManejarOperationContext.DevolverPosicionCorreoJugador(_diccionarioPuntuacion[uid], correoElectronico);
                    if (posicionJugador >= 0) 
                    {
                        _diccionarioPuntuacion[uid].RemoveAt(posicionJugador);
                        NotificarEliminarJugador(uid, correoElectronico);
                    }
                }
            }
            CambiarMultiple();
        }
        private void NotificarEliminarJugador(string uid, string correoElectronico) 
        {
            Logger log = new Logger(this.GetType(), "IJuegoPuntuacion");
            CambiarSingle();
            lock (_diccionarioPuntuacion)
            {
                if (!_diccionarioPuntuacion.ContainsKey(uid))
                {
                    return;
                }
                var contextosJugadoresDiccionario = _diccionarioPuntuacion[uid].ToList();
                foreach (ContextoJugador contextoJugador in contextosJugadoresDiccionario)
                {
                    try
                    {
                        contextoJugador.ContextoJugadorCallBack.GetCallbackChannel<IJuegoNotificacionCallback>().EliminarTurnoJugador(correoElectronico);
                    }
                    catch (CommunicationObjectAbortedException ex)
                    {
                        log.LogWarn("Error comunicación con el cliente", ex);
                        EliminarJugador(uid, contextoJugador.CorreoJugador);
                    }
                    catch (TimeoutException ex)
                    {
                        log.LogWarn("Se agoto el tiempo de espera del cliente", ex);
                        EliminarJugador(uid, contextoJugador.CorreoJugador);
                    }
                }
            }
            CambiarMultiple();
        }

        public void NotificarCambioTurno(string uid)
        {
            Logger log = new Logger(this.GetType(), "IJuegoPuntuacion");
            CambiarSingle();
            lock (_diccionarioPuntuacion)
            {
                if (!_diccionarioPuntuacion.ContainsKey(uid))
                {
                    return;
                }
                var contextosJugadoresDiccionario = _diccionarioPuntuacion[uid].ToList();
                foreach (ContextoJugador contextoJugador in contextosJugadoresDiccionario)
                {
                    try
                    {
                        contextoJugador.ContextoJugadorCallBack.GetCallbackChannel<IJuegoNotificacionCallback>().CambiarTurno();
                    }
                    catch (CommunicationObjectAbortedException ex)
                    {
                        log.LogWarn("Error comunicación con el cliente", ex);
                        EliminarJugador(uid, contextoJugador.CorreoJugador);
                    }
                    catch (TimeoutException ex)
                    {
                        log.LogWarn("Se agoto el tiempo de espera del cliente", ex);
                        EliminarJugador(uid, contextoJugador.CorreoJugador);
                    }
                }
            }
            CambiarMultiple();
        }

        public void NotificarJugadores(string uid, string correoJugador, string nombrePeon, int puntosObtenidos)
        {
            Logger log = new Logger(this.GetType(), "IJuegoPuntuacion");
            CambiarSingle();
            lock (_diccionarioPuntuacion)
            {
                if (!_diccionarioPuntuacion.ContainsKey(uid))
                {
                    return;
                }
                var contextosJugadoresDiccionario = _diccionarioPuntuacion[uid].ToList();

                foreach (ContextoJugador contextoJugador in contextosJugadoresDiccionario)
                {
                    if (contextoJugador.CorreoJugador != correoJugador)
                    {
                        try
                        {
                            contextoJugador.ContextoJugadorCallBack.GetCallbackChannel<IJuegoNotificacionCallback>().NotificarJugadorMovimiento(nombrePeon, puntosObtenidos);   
                        }
                        catch (CommunicationObjectAbortedException ex)
                        {
                            log.LogWarn("Error comunicación con el cliente", ex);
                            EliminarJugador(uid, contextoJugador.CorreoJugador);
                        }
                        catch (TimeoutException ex)
                        {
                            log.LogWarn("Se agoto el tiempo de espera del cliente", ex);
                            EliminarJugador(uid, contextoJugador.CorreoJugador);
                        }
                    }
                }
            }
            CambiarMultiple();
        }

        public Constantes ActualizarGanador(string uid, string correoElectronico, int posicion)
        {
            Logger log = new Logger(this.GetType(), "IJuegoPuntuacion");
            try
            {
                int filasAfectadas = 0;
                using (var context = new BaseDeDatosSorrySlidersEntities())
                {
                    filasAfectadas = filasAfectadas + context.Database.ExecuteSqlCommand("UPDATE RelacionPartidaCuentaSet SET Posicion = @posicion where CodigoPartida = @codigoPartida AND CorreoElectronico=@correoElectronico;",
                    new SqlParameter("@posicion", posicion),
                    new SqlParameter("@codigoPartida", uid), 
                    new SqlParameter("@correoElectronico", correoElectronico));
                    if (filasAfectadas > 0)
                    {
                        return Constantes.OPERACION_EXITOSA;
                    }
                    else
                    {
                        return Constantes.OPERACION_EXITOSA_VACIA;
                    }

                }
            }
            catch (SqlException ex)
            {
                log.LogError("Error al ejecutar consulta SQL", ex);
                return Constantes.ERROR_CONSULTA;
            }
            catch (EntityException ex)
            {
                log.LogError("Error con la base de datos", ex);
                return Constantes.ERROR_CONEXION_BD;
            }
        }

       

        public void EliminarDiccionariosJuego(string uid)
        {
            CambiarSingle();
            lock (_diccionarioPuntuacion)
            {
                ManejarOperationContext.EliminarKeyDiccionario(_diccionarioPuntuacion, uid);
            }
            lock (_jugadoresEnLineaChat)
            {
                ManejarOperationContext.EliminarKeyDiccionario(_jugadoresEnLineaChat, uid);
            }
            lock (_jugadoresEnLineaJuegoLanzamiento)
            {
                ManejarOperationContext.EliminarKeyDiccionario(_jugadoresEnLineaJuegoLanzamiento, uid);
            }
            CambiarMultiple();
        }

        public void NotificarCambiarPagina(string uid, int[] arrayPosiciones, string[] arrayNickname)
        {
            Logger log = new Logger(this.GetType(), "IJuegoPuntuacion");
            
            CambiarSingle();
            lock (_diccionarioPuntuacion)
            {
                if (!_diccionarioPuntuacion.ContainsKey(uid))
                {
                    return;
                }
                foreach (ContextoJugador contextoJugador in _diccionarioPuntuacion[uid])
                {
                    try
                    {
                        contextoJugador.ContextoJugadorCallBack.GetCallbackChannel<IJuegoNotificacionCallback>().CambiarPagina(arrayPosiciones, arrayNickname);
                    }
                    catch (CommunicationObjectAbortedException ex)
                    {
                        log.LogWarn("Error comunicación con el cliente", ex);
                    }
                    catch (TimeoutException ex)
                    {
                        log.LogWarn("Se agoto el tiempo de espera del cliente", ex);
                    }
                }
            }
            CambiarMultiple();
        }
    }
}
