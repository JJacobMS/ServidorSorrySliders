using DatosSorrySliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServidorSorrySliders
{
    public partial class ServicioComunicacionSorrySliders : IChat
    {
        private Dictionary<string, List<ContextoJugador>> _jugadoresEnLineaChat = new Dictionary<string, List<ContextoJugador>>();
        public void ChatJuego(string uid, string nickname, string mensaje)
        {
            Logger log = new Logger(this.GetType(), "IChat");
            lock (_jugadoresEnLineaChat)
            {
                foreach (ContextoJugador contextoJugador in _jugadoresEnLineaChat[uid])
                {
                    try
                    {
                        contextoJugador.ContextoJugadorCallBack.GetCallbackChannel<IChatCallback>().DevolverMensaje(nickname, mensaje);
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
            }
        }

        public void IngresarAlChat(string uid, string correo)
        {
            CambiarSingle();
            ContextoJugador jugadorNuevo = new ContextoJugador { CorreoJugador = correo, ContextoJugadorCallBack = OperationContext.Current };

            lock (_jugadoresEnLineaChat)
            {
                ManejarOperationContext.AgregarOReemplazarJugadorContextoLista(_jugadoresEnLineaChat, jugadorNuevo, uid);
            }
            Console.WriteLine( "Agregar Jugador Chat " + correo);
            CambiarMultiple();
        }
        public void ExpulsarJugadorPartida(string uid, string correo)
        {
            Logger log = new Logger(this.GetType(), "IChat");
            lock (_jugadoresEnLineaChat)
            {
                foreach (ContextoJugador contextoJugador in _jugadoresEnLineaChat[uid])
                {
                    try
                    {
                        contextoJugador.ContextoJugadorCallBack.GetCallbackChannel<IChatCallback>().ExpulsadoDeJugador(correo);
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
            }
        }

        public void SalirChatListaJugadores(string uid, string correo)
        {
            CambiarSingle();
            lock (_jugadoresEnLineaChat)
            {
                if (_jugadoresEnLineaChat.ContainsKey(uid))
                {
                    if (ManejarOperationContext.ExisteJugadorEnLista(OperationContext.Current, _jugadoresEnLineaChat[uid]))
                    {
                        ManejarOperationContext.EliminarJugadorLista(OperationContext.Current, _jugadoresEnLineaChat[uid]);

                        Console.WriteLine("Jugador eliminado del chat y lista jugadores");
                        if (_jugadoresEnLineaChat[uid].Count > 0)
                        {
                            NotificarEliminarJugadorChat(uid, correo);
                        }
                    }
                }
            }
            CambiarMultiple();
        }

        private void NotificarEliminarJugadorChat(string uid, string correo)
        {
            Logger log = new Logger(this.GetType(), "IChat");
            foreach (ContextoJugador jugador in _jugadoresEnLineaChat[uid])
            {
                try
                {
                    jugador.ContextoJugadorCallBack.GetCallbackChannel<IChatCallback>().JugadorSalioListaJugadores(correo);
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
        }

    }
}
