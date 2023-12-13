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
                if (_jugadoresEnLineaChat.ContainsKey(uid))
                {
                    foreach (ContextoJugador contextoJugador in _jugadoresEnLineaChat[uid])
                    {
                        try
                        {
                            contextoJugador.ContextoJugadorCallBack.GetCallbackChannel<IChatCallback>().DevolverMensaje(nickname, mensaje);
                        }
                        catch (CommunicationException ex)
                        {
                            log.LogWarn("Hubo un error de comunicación con el cliente", ex);
                        }
                        catch (TimeoutException ex)
                        {
                            log.LogWarn("Ha ocurrido una excepción de tiempo de respuesta", ex);
                        }

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
            CambiarMultiple();
        }
        public void ExpulsarJugadorPartida(string uid, string correo)
        {
            CambiarSingle();
            Logger log = new Logger(this.GetType(), "IChat");
            lock (_jugadoresEnLineaChat)
            {
                if (_jugadoresEnLineaChat.ContainsKey(uid))
                {
                    foreach (ContextoJugador contextoJugador in _jugadoresEnLineaChat[uid])
                    {
                        try
                        {
                            contextoJugador.ContextoJugadorCallBack.GetCallbackChannel<IChatCallback>().ExpulsadoDeJugador(correo);
                        }
                        catch (CommunicationException ex)
                        {
                            log.LogWarn("Hubo un error de comunicación con el cliente", ex);
                        }
                        catch (TimeoutException ex)
                        {
                            log.LogWarn("Ha ocurrido una excepción de tiempo de respuesta", ex);
                        }
                    }
                }
            }
            CambiarMultiple();
            EliminarJugadorDiccionarioJuegoPorExpulsion(uid, correo);
        }

        private void EliminarJugadorDiccionarioJuegoPorExpulsion(string codigoPartida, string correoJugador)
        {
            CambiarSingle();
            SalirDelLobby(correoJugador, codigoPartida);
            if (!correoJugador.Contains("@"))
            {
                EliminarCuentaProvisional(correoJugador);
            }
            lock (_diccionarioPuntuacion)
            {
                EliminarJugador(codigoPartida, correoJugador);
            }
            lock (_jugadoresEnLineaJuegoLanzamiento)
            {
                ManejarOperationContext.EliminarJugadorDiccionarioPorCorreo(_jugadoresEnLineaJuegoLanzamiento, codigoPartida, correoJugador);
                NotificarJugadorSalioPartidaLanzamiento(correoJugador, codigoPartida);
            }
            lock (_jugadoresEnLineaChat)
            {
                ManejarOperationContext.EliminarJugadorDiccionarioPorCorreo(_jugadoresEnLineaChat, codigoPartida, correoJugador);
            }
            CambiarMultiple();
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
            lock (_jugadoresEnLineaChat)
            {
                if (!_jugadoresEnLineaChat.ContainsKey(uid))
                {
                    return;
                }
                foreach (ContextoJugador jugador in _jugadoresEnLineaChat[uid])
                {
                    try
                    {
                        jugador.ContextoJugadorCallBack.GetCallbackChannel<IChatCallback>().JugadorSalioListaJugadores(correo);
                    }
                    catch (CommunicationException ex)
                    {
                        log.LogWarn("Hubo un error de comunicación con el cliente", ex);
                    }
                    catch (TimeoutException ex)
                    {
                        log.LogWarn("Ha ocurrido una excepción de tiempo de respuesta", ex);
                    }
                }
            }
        }

    }
}
