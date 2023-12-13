using PruebasSorrySliders.ServidorComunicacionSorrySlidersPrueba;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PruebasSorrySliders
{

    public class LlamadaCallChatPruebas: IDisposable
    {
        /// <seealso cref="ServidorSorrySliders.IChat"/>
        private static ChatClient _proxyChat;
        private static ChatClient _proxyChatNuevo;
        private static ChatClient _proxyChatNuevoDos;
        private static ImplementacionCallBackChat _implementacionCallBackChat;
        private static ImplementacionCallBackChat _implementacionCallBackChatNueva;
        private static ImplementacionCallBackChat _implementacionCallBackChatNuevaDos;

        public LlamadaCallChatPruebas()
        {
            _implementacionCallBackChat = new ImplementacionCallBackChat();
            _proxyChat = new ChatClient(new InstanceContext(_implementacionCallBackChat));
            _proxyChat.IngresarAlChat("3F2504E0-4F89-11D3-9A0C-0305E82C3301", "jacob@gmail.com");

            _implementacionCallBackChatNueva = new ImplementacionCallBackChat();
            _proxyChatNuevo = new ChatClient(new InstanceContext(_implementacionCallBackChatNueva));
            _proxyChatNuevo.IngresarAlChat("3F2504E0-4F89-11D3-9A0C-0305E82C3301", "sulem@gmail.com");

            _implementacionCallBackChatNuevaDos = new ImplementacionCallBackChat();
            _proxyChatNuevoDos = new ChatClient(new InstanceContext(_implementacionCallBackChatNuevaDos));
            _proxyChatNuevoDos.IngresarAlChat("3F2504E0-4F89-11D3-9A0C-0305E82C3301", "correo@gmail.com");

        }
        public void Dispose()
        {
            _proxyChat.SalirChatListaJugadores("3F2504E0-4F89-11D3-9A0C-0305E82C3301", "jacob@gmail.com");
            _proxyChatNuevo.SalirChatListaJugadores("3F2504E0-4F89-11D3-9A0C-0305E82C3301", "sulem@gmail.com");
        }

        [Fact]
        public async void EnviarMensajeChatExitosamentePrueba()
        {
            string codigoPartida = "3F2504E0-4F89-11D3-9A0C-0305E82C3301";
            string correoEnChat = "jacob@gmail.com";
            string mensaje = "hola";

            _proxyChat.ChatJuego(codigoPartida, correoEnChat, mensaje);

            await Task.Delay(2000);
            Assert.True(_implementacionCallBackChatNueva.HaDevueltoElMensaje);
            Assert.Equal(_implementacionCallBackChatNueva.MensajeRecibido, mensaje);
            Assert.Equal(_implementacionCallBackChatNueva.CorreoRecibido, correoEnChat);
        }
        [Fact]
        public async void ExpulsarJugadorNotificarExitosamentePrueba()
        {
            string codigoPartida = "3F2504E0-4F89-11D3-9A0C-0305E82C3301";
            string correoExpulsado = "sulem@gmail.com";

            _proxyChat.ExpulsarJugadorPartida(codigoPartida, correoExpulsado);

            await Task.Delay(2000);
            Assert.True(_implementacionCallBackChatNuevaDos.HaExpulsadoJugador);
            Assert.Equal(_implementacionCallBackChatNuevaDos.CorreoRecibido, correoExpulsado);
        }
        [Fact]
        public async void SalirChatNotificarExitosamentePrueba()
        {
            string codigoPartida = "3F2504E0-4F89-11D3-9A0C-0305E82C3301";
            string correoExpulsado = "jacob@gmail.com";

            _proxyChat.ExpulsarJugadorPartida(codigoPartida, correoExpulsado);

            await Task.Delay(2000);
            Assert.True(_implementacionCallBackChatNuevaDos.HaExpulsadoJugador);
            Assert.Equal(_implementacionCallBackChatNuevaDos.CorreoRecibido, correoExpulsado);
        }
        [Fact]
        public async void EnviarMensajeJugadorPartidaNoExistentePrueba()
        {
            string codigoPartida = "3F2504E0-4F89-11D3-9A0C-0305E82C3302";
            string correoEnChat = "jacob@gmail.com";
            string mensaje = "hola";

            _proxyChat.ChatJuego(codigoPartida, correoEnChat, mensaje);

            string correoEnChatEsperado = null;
            string mensajeEsperado = null;

            await Task.Delay(2000);
            Assert.False(_implementacionCallBackChatNueva.HaDevueltoElMensaje);
            Assert.Equal(_implementacionCallBackChatNueva.MensajeRecibido, mensajeEsperado);
            Assert.Equal(_implementacionCallBackChatNueva.CorreoRecibido, correoEnChatEsperado);
        }
        [Fact]
        public async void ExpulsarJugadorQueNoExistentePrueba()
        {
            string codigoPartida = "3F2504E0-4F89-11D3-9A0C-0305E82C3302";
            string correoExpulsado = "sulem@gmail.com";

            _proxyChat.ExpulsarJugadorPartida(codigoPartida, correoExpulsado);

            string correoEsperado = null;

            await Task.Delay(2000);
            Assert.False(_implementacionCallBackChatNuevaDos.HaExpulsadoJugador);
            Assert.Equal(_implementacionCallBackChatNuevaDos.CorreoRecibido, correoEsperado);
        }
        [Fact]
        public async void SalirChatNotificarJugadorPrueba()
        {
            string codigoPartida = "3F2504E0-4F89-11D3-9A0C-0305E82C3302";
            string correoExpulsado = "jacob@gmail.com";

            _proxyChat.ExpulsarJugadorPartida(codigoPartida, correoExpulsado);

            string correoEsperado = null;

            await Task.Delay(2000);
            Assert.False(_implementacionCallBackChatNuevaDos.HaExpulsadoJugador);
            Assert.Equal(_implementacionCallBackChatNuevaDos.CorreoRecibido, correoEsperado);
        }
    }
    public class ImplementacionCallBackChat : IChatCallback
    {
        public string CorreoRecibido { get; set; }
        public string MensajeRecibido { get; set; }
        public bool HaDevueltoElMensaje { get; set; }
        public bool HaExpulsadoJugador { get; set; }
        public bool HaSalidoChat { get; set; }
        public ImplementacionCallBackChat()
        {
            HaDevueltoElMensaje = false;
            HaExpulsadoJugador = false;
            CorreoRecibido = null;
            MensajeRecibido = null;
        }
        public void DevolverMensaje(string nickname, string mensaje)
        {
            HaDevueltoElMensaje = true;
            CorreoRecibido = nickname;
            MensajeRecibido = mensaje;
        }

        public void ExpulsadoDeJugador(string correoElectronico)
        {
            HaExpulsadoJugador = true;
            CorreoRecibido = correoElectronico;
        }

        public void JugadorSalioListaJugadores(string correoElectronico)
        {
            HaSalidoChat = true;
            CorreoRecibido = correoElectronico;
        }
    }
}
