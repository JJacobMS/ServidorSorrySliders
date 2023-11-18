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

            ManejarOperationContext.AgregarJugadorContextoLista(_jugadoresEnLineaJuegoLanzamiento, jugadorNuevo, codigoPartida);
            Console.WriteLine("Se agregó " + correoElectronico + " al juego");
            CambiarMultiple();
        }

        public void EliminarJugadorJuegoLanzamiento(string codigoPartida)
        {
            CambiarSingle();
            Logger log = new Logger(this.GetType(), "IJuegoLanzamiento");
            if (_jugadoresEnLineaJuegoLanzamiento.ContainsKey(codigoPartida))
            {
                if (ManejarOperationContext.ExisteJugadorEnLista(OperationContext.Current, _jugadoresEnLineaJuegoLanzamiento[codigoPartida]))
                {
                    string jugadorAEliminar = ManejarOperationContext.DevolverCorreoJugador(_jugadoresEnLineaJuegoLanzamiento[codigoPartida], OperationContext.Current);
                    ManejarOperationContext.EliminarJugadorLista(OperationContext.Current, _jugadoresEnLineaJuegoLanzamiento[codigoPartida]);
                    Console.WriteLine("Se eliminó " + jugadorAEliminar + " del juego");
                    if (_jugadoresEnLineaJuegoLanzamiento[codigoPartida].Count > 0)
                    {
                        foreach (ContextoJugador jugador in _jugadoresEnLineaJuegoLanzamiento[codigoPartida])
                        {
                            try
                            {
                                jugador.ContextoJugadorCallBack.GetCallbackChannel<IJuegoLanzamientoCallback>().JugadorSalioJuegoLanzamiento(jugadorAEliminar);

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
                    }
                }
            }
            CambiarMultiple();
        }

        public void NotificarFinalizarLanzamiento(string codigoPartida, string correo)
        {
            CambiarSingle();
            Logger log = new Logger(this.GetType(), "IJuegoLanzamiento");
            if (_jugadoresEnLineaJuegoLanzamiento.ContainsKey(codigoPartida))
            {
                if (_jugadoresEnLineaJuegoLanzamiento[codigoPartida].Count > 0)
                {
                    int posicionJugador = ManejarOperationContext.DevolverPosicionCorreoJugador(_jugadoresEnLineaJuegoLanzamiento[codigoPartida], correo);
                    if (posicionJugador != -1)
                    {
                        _jugadoresEnLineaJuegoLanzamiento[codigoPartida][posicionJugador].ListoParaTurnoSiguiente = true;
                    }
                    if (NoHayJugadoresPendientes(codigoPartida))
                    {
                        foreach (ContextoJugador jugador in _jugadoresEnLineaJuegoLanzamiento[codigoPartida])
                        {
                            try
                            {
                                jugador.ContextoJugadorCallBack.GetCallbackChannel<IJuegoLanzamientoCallback>().JugadoresListosParaSiguienteTurno();
                                jugador.ListoParaTurnoSiguiente = false;
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
                    }
                }

            }
            CambiarMultiple();
        }

        public void NotificarLanzamientoDado(string codigoPartida, string correo, int numeroDado)
        {
            Logger log = new Logger(this.GetType(), "IJuegoLanzamiento");
            if (_jugadoresEnLineaJuegoLanzamiento.ContainsKey(codigoPartida))
            {
                if (_jugadoresEnLineaJuegoLanzamiento[codigoPartida].Count > 0)
                {
                    foreach (ContextoJugador jugador in _jugadoresEnLineaJuegoLanzamiento[codigoPartida])
                    {
                        if (jugador.CorreoJugador.Equals(correo))
                        {
                            continue;
                        }
                        try
                        {
                            jugador.ContextoJugadorCallBack.GetCallbackChannel<IJuegoLanzamientoCallback>().JugadorTiroDado(numeroDado);
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
                }

            }
        }

        public void NotificarLanzamientoLinea(string codigoPartida, string correo, double posicionX, double posicionY)
        {
            Logger log = new Logger(this.GetType(), "IJuegoLanzamiento");
            if (_jugadoresEnLineaJuegoLanzamiento.ContainsKey(codigoPartida))
            {
                if (_jugadoresEnLineaJuegoLanzamiento[codigoPartida].Count > 0)
                {
                    foreach (ContextoJugador jugador in _jugadoresEnLineaJuegoLanzamiento[codigoPartida])
                    {
                        if (jugador.CorreoJugador.Equals(correo))
                        {
                            continue;
                        }
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
                }

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
