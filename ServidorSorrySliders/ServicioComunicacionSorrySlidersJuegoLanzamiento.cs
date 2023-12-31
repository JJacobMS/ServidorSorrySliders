﻿using DatosSorrySliders;
using InterfacesServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServidorSorrySliders
{
    public partial class ServicioComunicacionSorrySliders : IJuegoLanzamiento
    {
        private Dictionary<string, List<ContextoJugador>> _jugadoresEnLineaJuegoLanzamiento = new Dictionary<string, List<ContextoJugador>>();
        public void AgregarJugadorJuegoLanzamiento(string codigoPartida, string correoElectronico)
        {
            CambiarSingle();
            ContextoJugador jugadorNuevo = new ContextoJugador 
            { 
                CorreoJugador = correoElectronico, 
                ContextoJugadorCallBack = OperationContext.Current 
            };
            lock (_jugadoresEnLineaJuegoLanzamiento)
            {
                ManejarOperationContext.AgregarOReemplazarJugadorContextoLista(_jugadoresEnLineaJuegoLanzamiento, jugadorNuevo, codigoPartida);
            }
            CambiarMultiple();
        }

        public void EliminarJugadorJuegoLanzamiento(string codigoPartida)
        {
            CambiarSingle();
            lock (_jugadoresEnLineaJuegoLanzamiento)
            {
                string jugadorEliminado = ManejarOperationContext.EliminarJugadorDiccionario(_jugadoresEnLineaJuegoLanzamiento, codigoPartida, OperationContext.Current);
                NotificarJugadorSalioPartidaLanzamiento(jugadorEliminado, codigoPartida);
            }
            CambiarMultiple();
        }

        private void NotificarJugadorSalioPartidaLanzamiento(string jugadorEliminado, string codigoPartida)
        {
            Logger log = new Logger(this.GetType(), "IJuegoLanzamiento");
            if (jugadorEliminado.Length > 0 && _jugadoresEnLineaJuegoLanzamiento.ContainsKey(codigoPartida))
            {
                List<ContextoJugador> jugadoresSinConexion = new List<ContextoJugador>();
                foreach (ContextoJugador jugador in _jugadoresEnLineaJuegoLanzamiento[codigoPartida])
                {
                    try
                    {
                        jugador.ContextoJugadorCallBack.GetCallbackChannel<IJuegoLanzamientoCallback>().JugadorSalioJuegoLanzamiento(jugadorEliminado);
                    }
                    catch (CommunicationException ex)
                    {
                        jugadoresSinConexion.Add(jugador);
                        log.LogWarn("Error comunicación con el cliente", ex);
                    }
                    catch (TimeoutException ex)
                    {
                        jugadoresSinConexion.Add(jugador);
                        log.LogWarn("Se agoto el tiempo de espera del cliente", ex);
                    }
                }
                if (jugadoresSinConexion.Count > 0)
                {
                    EliminarJugadoresSinConexionMientrasJugabaLanzamiento(jugadoresSinConexion, codigoPartida);
                }
            }
        }

        public void NotificarFinalizarLanzamiento(string codigoPartida, string correo)
        {
            CambiarSingle();
            lock (_jugadoresEnLineaJuegoLanzamiento)
            {
                int posicionJugador = PartidaYJugadorExisten(codigoPartida, correo);
                if (posicionJugador != -1)
                {
                    _jugadoresEnLineaJuegoLanzamiento[codigoPartida][posicionJugador].ListoParaTurnoSiguiente = true;

                    if (NoHayJugadoresPendientes(codigoPartida))
                    {
                        NotificarJugadoresPartidaTurnoSiguiente(codigoPartida);
                    }
                }
            }
            CambiarMultiple();
        }

        private int PartidaYJugadorExisten(string codigoPartida, string correoElectronico)
        {
            if (_jugadoresEnLineaJuegoLanzamiento.ContainsKey(codigoPartida) && _jugadoresEnLineaJuegoLanzamiento[codigoPartida].Count > 0)
            {
                return ManejarOperationContext.DevolverPosicionCorreoJugador(_jugadoresEnLineaJuegoLanzamiento[codigoPartida], correoElectronico);
            }
            return -1;
        }

        private void NotificarJugadoresPartidaTurnoSiguiente(string codigoPartida)
        {
            Logger log = new Logger(this.GetType(), "IJuegoLanzamiento");

            List<ContextoJugador> jugadoresSinConexion = new List<ContextoJugador>();
            foreach (ContextoJugador jugador in _jugadoresEnLineaJuegoLanzamiento[codigoPartida])
            {
                try
                {
                    jugador.ContextoJugadorCallBack.GetCallbackChannel<IJuegoLanzamientoCallback>().JugadoresListosParaSiguienteTurno();
                    jugador.ListoParaTurnoSiguiente = false;
                }
                catch (CommunicationException ex)
                {
                    jugadoresSinConexion.Add(jugador);
                    log.LogWarn("Error comunicación con el cliente", ex);
                }
                catch (TimeoutException ex)
                {
                    jugadoresSinConexion.Add(jugador);
                    log.LogWarn("Se agoto el tiempo de espera del cliente", ex);
                }
            }
            if (jugadoresSinConexion.Count > 0)
            {
                EliminarJugadoresSinConexionMientrasJugabaLanzamiento(jugadoresSinConexion, codigoPartida);
            }
        }

        public void NotificarLanzamientoDado(string codigoPartida, string correo, int numeroDado)
        {
            lock (_jugadoresEnLineaJuegoLanzamiento)
            {
                if (_jugadoresEnLineaJuegoLanzamiento.ContainsKey(codigoPartida))
                {
                    NotificarJugadoresPartidaLanzamientoDado(codigoPartida, correo, numeroDado);
                }
            }
        }

        private void NotificarJugadoresPartidaLanzamientoDado(string codigoPartida, string correo, int numeroDado)
        {
            Logger log = new Logger(this.GetType(), "IJuegoLanzamiento");
            List<ContextoJugador> jugadoresSinConexion = new List<ContextoJugador>();
            foreach (ContextoJugador jugador in _jugadoresEnLineaJuegoLanzamiento[codigoPartida].Where(jugador => !jugador.CorreoJugador.Equals(correo)))
            {
                try
                {
                    jugador.ContextoJugadorCallBack.GetCallbackChannel<IJuegoLanzamientoCallback>().JugadorTiroDado(numeroDado);
                }
                catch (CommunicationException ex)
                {
                    jugadoresSinConexion.Add(jugador);
                    log.LogWarn("Error comunicación con el cliente", ex);
                }
                catch (TimeoutException ex)
                {
                    jugadoresSinConexion.Add(jugador);
                    log.LogWarn("Se agoto el tiempo de espera del cliente", ex);
                }
            }
            if (jugadoresSinConexion.Count > 0)
            {
                EliminarJugadoresSinConexionMientrasJugabaLanzamiento(jugadoresSinConexion, codigoPartida);
            }
        }

        public void NotificarLanzamientoLinea(string codigoPartida, string correo, double posicionX, double posicionY)
        {
            lock (_jugadoresEnLineaJuegoLanzamiento)
            {
                if (_jugadoresEnLineaJuegoLanzamiento.ContainsKey(codigoPartida) && _jugadoresEnLineaJuegoLanzamiento[codigoPartida].Count > 0)
                {
                    NotificarTodosJugadoresPartidaLanzamientoLinea(codigoPartida, correo, posicionX, posicionY);
                }
            }
        }

        private void NotificarTodosJugadoresPartidaLanzamientoLinea(string codigoPartida, string correo, double posicionX, double posicionY)
        {
            Logger log = new Logger(this.GetType(), "IJuegoLanzamiento");
            List<ContextoJugador> jugadoresSinConexion = new List<ContextoJugador>();
            foreach (ContextoJugador jugador in _jugadoresEnLineaJuegoLanzamiento[codigoPartida].Where(jugador => !jugador.CorreoJugador.Equals(correo)))
            {
                try
                {
                    jugador.ContextoJugadorCallBack.GetCallbackChannel<IJuegoLanzamientoCallback>().JugadorDetuvoLinea(posicionX, posicionY);
                }
                catch (CommunicationException ex)
                {
                    jugadoresSinConexion.Add(jugador);
                    log.LogWarn("Error comunicación con el cliente", ex);
                }
                catch (TimeoutException ex)
                {
                    jugadoresSinConexion.Add(jugador);
                    log.LogWarn("Se agoto el tiempo de espera del cliente", ex);
                }
            }
            if (jugadoresSinConexion.Count > 0)
            {
                EliminarJugadoresSinConexionMientrasJugabaLanzamiento(jugadoresSinConexion, codigoPartida);
            }
        }

        private bool NoHayJugadoresPendientes(string codigo)
        {
            foreach (ContextoJugador jugador in _jugadoresEnLineaJuegoLanzamiento[codigo])
            {
                if (!jugador.ListoParaTurnoSiguiente)
                {
                    return false;
                }
            }
            return true;
        }

        private void CambiarSingle()
        {
            var hostServicio = (ServiceHost)OperationContext.Current.Host;
            var comportamiento = hostServicio.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            comportamiento.ConcurrencyMode = ConcurrencyMode.Single;
        }

        private void CambiarMultiple()
        {
            var hostServicio = (ServiceHost)OperationContext.Current.Host;
            var comportamiento = hostServicio.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            comportamiento.ConcurrencyMode = ConcurrencyMode.Multiple;
        }

        private void EliminarJugadoresSinConexionMientrasJugabaLanzamiento(List<ContextoJugador> jugadoresSinConexion, string codigo)
        {
            foreach (ContextoJugador jugador in jugadoresSinConexion)
            {
                ManejarOperationContext.EliminarJugadorDiccionario(_jugadoresEnLineaJuegoLanzamiento, codigo, jugador.ContextoJugadorCallBack);
                NotificarJugadorSalioPartidaLanzamiento(jugador.CorreoJugador, codigo);

                lock (_jugadoresEnLineaChat)
                {
                    if (_jugadoresEnLineaChat.ContainsKey(codigo))
                    {
                        int posicionChat = ManejarOperationContext.DevolverPosicionCorreoJugador(_jugadoresEnLineaChat[codigo], jugador.CorreoJugador);
                        if (posicionChat != -1)
                        {
                            ManejarOperationContext.EliminarJugadorDiccionario(_jugadoresEnLineaChat, codigo, _jugadoresEnLineaChat[codigo][posicionChat].ContextoJugadorCallBack);
                            NotificarEliminarJugadorChat(codigo, jugador.CorreoJugador);
                        }
                    }                    
                }

                lock (_diccionarioPuntuacion)
                {
                    if (_diccionarioPuntuacion.ContainsKey(codigo))
                    {
                        int posicionChat = ManejarOperationContext.DevolverPosicionCorreoJugador(_diccionarioPuntuacion[codigo], jugador.CorreoJugador);
                        if (posicionChat != -1)
                        {
                            ManejarOperationContext.EliminarJugadorDiccionario(_diccionarioPuntuacion, codigo, _diccionarioPuntuacion[codigo][posicionChat].ContextoJugadorCallBack);
                            NotificarEliminarJugador(codigo, jugador.CorreoJugador);
                        }
                    }
                }

            }
        }

        public void NotificarPosicionFichasFinales(string codigoPartida, string correo, PeonesTablero peones)
        {
            lock (_jugadoresEnLineaJuegoLanzamiento)
            {
                if (_jugadoresEnLineaJuegoLanzamiento.ContainsKey(codigoPartida) && _jugadoresEnLineaJuegoLanzamiento[codigoPartida].Count > 0)
                {
                    NotificarCambiarFichas(codigoPartida, correo, peones);
                }
            }
        }

        private void NotificarCambiarFichas(string codigoPartida, string correo, PeonesTablero peones)
        {
            Logger log = new Logger(this.GetType(), "IJuegoLanzamiento");
            List<ContextoJugador> jugadoresSinConexion = new List<ContextoJugador>();
            foreach (ContextoJugador jugador in _jugadoresEnLineaJuegoLanzamiento[codigoPartida])
            {
                try
                {
                    jugador.ContextoJugadorCallBack.GetCallbackChannel<IJuegoLanzamientoCallback>().CambiarPosicionPeonesTableroYContinuar(peones);
                }
                catch (CommunicationException ex)
                {
                    jugadoresSinConexion.Add(jugador);
                    log.LogWarn("Error comunicación con el cliente " + correo, ex);
                }
                catch (TimeoutException ex)
                {
                    jugadoresSinConexion.Add(jugador);
                    log.LogWarn("Se agoto el tiempo de espera del cliente " + correo, ex);
                }
            }
            if (jugadoresSinConexion.Count > 0)
            {
                EliminarJugadoresSinConexionMientrasJugabaLanzamiento(jugadoresSinConexion, codigoPartida);
            }
        }
    }
}
