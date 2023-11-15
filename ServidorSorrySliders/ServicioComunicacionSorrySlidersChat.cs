using DatosSorrySliders;
using System;
using System.Collections.Generic;
using System.Linq;
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
            foreach (ContextoJugador contextoJugador in _jugadoresEnLineaChat[uid])
            {
                try
                {
                    contextoJugador.ContextoJugadorCallBack.GetCallbackChannel<IChatCallback>().DevolverMensaje(nickname, mensaje);
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

        public void IngresarAlChat(string uid, string correo)
        {
            ContextoJugador jugadorNuevo = new ContextoJugador { CorreoJugador = correo, ContextoJugadorCallBack = OperationContext.Current};
            Console.WriteLine( "Agregar Jugador Chat" + ManejarOperationContext.AgregarJugadorContextoLista(_jugadoresEnLineaChat, jugadorNuevo, uid));
        }
    }
}
