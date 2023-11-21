using DatosSorrySliders;
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
        public void EntrarPartida(string uid, string cuentaCorreo)
        {
            CambiarSingle();
            Logger log = new Logger(this.GetType(), "ILobby");
            ContextoJugador jugadorNuevo = new ContextoJugador { CorreoJugador = cuentaCorreo, ContextoJugadorCallBack = OperationContext.Current};
            lock (_jugadoresEnLineaLobby)
            {
                ManejarOperationContext.AgregarJugadorContextoLista(_jugadoresEnLineaLobby, jugadorNuevo, uid);

                foreach (ContextoJugador jugadorOperation in _jugadoresEnLineaLobby[uid])
                {
                    bool huboError = false;
                    try
                    {
                        jugadorOperation.ContextoJugadorCallBack.GetCallbackChannel<ILobbyCallback>().JugadorEntroPartida();
                    }
                    catch (CommunicationObjectAbortedException ex)
                    {
                        huboError = true;
                        Console.WriteLine("Ha ocurrido un error en el callback \n" + ex.StackTrace);
                        log.LogWarn("La conexión del usuario se ha perdido", ex);
                    }
                    catch (InvalidCastException ex)
                    {
                        huboError = true;
                        Console.WriteLine(ex.StackTrace);
                        Console.WriteLine("El metodo del callback no pertenece a dicho contexto \n" + ex.StackTrace);
                        log.LogWarn("el callback no pertenece a dicho contexto ", ex);
                    }
                    catch (Exception ex)
                    {
                        huboError = true;
                        Console.WriteLine(ex.StackTrace);
                        log.LogFatal("Ha ocurrido un error inesperado", ex);
                    }
                    if (huboError)
                    {
                        //COMPROBAR JUGADORES
                        /*ManejarOperationContext.EliminarJugadorLista(jugadorOperation.ContextoJugadorCallBack, _jugadoresEnLineaLobby[uid]);
                        EliminarRelacionPartidaJugadorDesconectado(jugadorOperation.CorreoJugador, uid);*/
                    }
                }
            }
            CambiarMultiple();
        }

        public void SalirPartida(string uid)
        {
            CambiarSingle();
            Logger log = new Logger(this.GetType(), "ILobby");
            lock (_jugadoresEnLineaLobby)
            {
                if (_jugadoresEnLineaLobby.ContainsKey(uid))
                {
                    if (ManejarOperationContext.ExisteJugadorEnLista(OperationContext.Current, _jugadoresEnLineaLobby[uid]))
                    {

                        ManejarOperationContext.EliminarJugadorLista(OperationContext.Current, _jugadoresEnLineaLobby[uid]);

                        Console.WriteLine("Jugador eliminado del lobby");
                        if (_jugadoresEnLineaLobby[uid].Count > 0)
                        {
                            foreach (ContextoJugador jugador in _jugadoresEnLineaLobby[uid])
                            {
                                try
                                {
                                    jugador.ContextoJugadorCallBack.GetCallbackChannel<ILobbyCallback>().JugadorSalioPartida();
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
            }
            CambiarMultiple();
        }

        public void IniciarPartida(string uid)
        {
            Logger log = new Logger(this.GetType(), "ILobby");
            try
            {
                lock (_jugadoresEnLineaLobby)
                {
                    foreach (ContextoJugador contexto in _jugadoresEnLineaLobby[uid])
                    {
                        contexto.ContextoJugadorCallBack.GetCallbackChannel<ILobbyCallback>().HostInicioPartida();
                    }
                }
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
