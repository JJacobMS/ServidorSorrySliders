using DatosSorrySliders;
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
            ContextoJugador jugadorNuevo = new ContextoJugador { CorreoJugador = correoElectronico, ContextoJugadorCallBack = OperationContext.Current };
            lock (_jugadoresEnLineaJuegoLanzamiento)
            {
                ManejarOperationContext.AgregarOReemplazarJugadorContextoLista(_jugadoresEnLineaJuegoLanzamiento, jugadorNuevo, codigoPartida);
            }
            Console.WriteLine("Se agregó " + correoElectronico + " al juego");
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
        /// <summary>
        /// Notifica a todos los jugadores de esa partida que el jugadorEliminado se ha salido, en el caso de que no encuentre a uno lo saca
        /// </summary>
        /// <param name="jugadorEliminado"></param>
        /// <param name="codigoPartida"></param>
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
                        Console.WriteLine("Notificó jugador " + jugador.CorreoJugador);
                    }
                    catch (CommunicationObjectAbortedException ex)
                    {
                        log.LogWarn("La conexión del usuario se ha perdido", ex);
                        jugadoresSinConexion.Add(jugador);
                    }
                    catch (InvalidCastException ex)
                    {
                        log.LogWarn("el callback no pertenece a dicho contexto ", ex);
                    }
                    catch (Exception ex)
                    {
                        log.LogFatal("Ha ocurrido un error inesperado", ex);
                    }
                }
                if (jugadoresSinConexion.Count > 0)
                {
                    EliminarJugadoresSinConexionJugadoresLanzamiento(jugadoresSinConexion, codigoPartida);
                }
            }
        }
        private void EliminarJugadoresSinConexionJugadoresLanzamiento(List<ContextoJugador> jugadoresSinConexion, string codigo)
        {
            foreach (ContextoJugador jugador in jugadoresSinConexion)
            {
                ManejarOperationContext.EliminarJugadorDiccionario(_jugadoresEnLineaJuegoLanzamiento, codigo, jugador.ContextoJugadorCallBack);
                NotificarJugadorSalioPartidaLanzamiento(jugador.CorreoJugador, codigo);
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
            if (_jugadoresEnLineaJuegoLanzamiento.ContainsKey(codigoPartida))
            {
                if (_jugadoresEnLineaJuegoLanzamiento[codigoPartida].Count > 0)
                {
                    return ManejarOperationContext.DevolverPosicionCorreoJugador(_jugadoresEnLineaJuegoLanzamiento[codigoPartida], correoElectronico);
                }
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
                catch (CommunicationObjectAbortedException ex)
                {
                    jugadoresSinConexion.Add(jugador);
                    log.LogWarn("La conexión del usuario se ha perdido", ex);
                }
                catch (InvalidCastException ex)
                {
                    log.LogWarn("el callback no pertenece a dicho contexto ", ex);
                }
                catch (Exception ex)
                {
                    log.LogFatal("Ha ocurrido un error inesperado", ex);
                }
            }
            if (jugadoresSinConexion.Count > 0)
            {
                EliminarJugadoresSinConexionJugadoresLanzamiento(jugadoresSinConexion, codigoPartida);
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
                catch (CommunicationObjectAbortedException ex)
                {
                    log.LogWarn("La conexión del usuario se ha perdido", ex);
                }
                catch (InvalidCastException ex)
                {
                    log.LogWarn("el callback no pertenece a dicho contexto ", ex);
                }
                catch (Exception ex)
                {
                    log.LogFatal("Ha ocurrido un error inesperado", ex);
                }
            }
            if (jugadoresSinConexion.Count > 0)
            {
                EliminarJugadoresSinConexionJugadoresLanzamiento(jugadoresSinConexion, codigoPartida);
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
                catch (CommunicationObjectAbortedException ex)
                {
                    Console.WriteLine("Ha ocurrido un error en el callback \n" + ex.StackTrace);
                    log.LogWarn("La conexión del usuario se ha perdido", ex);
                }
                catch (InvalidCastException ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine("El metodo del callback no pertenece a dicho contexto \n" + ex.StackTrace);
                    log.LogWarn("el callback no pertenece a dicho contexto ", ex);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    log.LogFatal("Ha ocurrido un error inesperado", ex);
                }
            }
            if (jugadoresSinConexion.Count > 0)
            {
                EliminarJugadoresSinConexionJugadoresLanzamiento(jugadoresSinConexion, codigoPartida);
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

    }
}
