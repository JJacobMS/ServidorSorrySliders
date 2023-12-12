
using DatosSorrySliders;
using PruebasSorrySliders.ServidorComunicacionSorrySlidersPrueba;
using ServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PruebasSorrySliders
{
    //Para estas pruebas debería estar el servidor
    public class LlamadaCallBackJuegoLanzamientoPruebas : IDisposable
    {
        private static JuegoLanzamientoClient _proxyJuegoLanzamiento;
        private static ImplementacionPruebaCallbackJuegoLanzamiento _implementacionCallbackJuegoLanzamiento;
        private static ImplementacionPruebaCallbackJuegoLanzamiento _nuevaImplementacionCallbackJuegoLanzamiento;
        private static JuegoLanzamientoClient _proxyNuevoJuegoLanzamiento;
        public LlamadaCallBackJuegoLanzamientoPruebas()
        {
            _implementacionCallbackJuegoLanzamiento = new ImplementacionPruebaCallbackJuegoLanzamiento();
            _proxyJuegoLanzamiento = new JuegoLanzamientoClient(new InstanceContext(_implementacionCallbackJuegoLanzamiento));
            _proxyJuegoLanzamiento.AgregarJugadorJuegoLanzamiento("3F2504E0-4F89-11D3-9A0C-0305E82C3301", "jazmin@gmail.com");

            _nuevaImplementacionCallbackJuegoLanzamiento = new ImplementacionPruebaCallbackJuegoLanzamiento();
            _proxyNuevoJuegoLanzamiento = new JuegoLanzamientoClient(new InstanceContext(_nuevaImplementacionCallbackJuegoLanzamiento));
            _proxyNuevoJuegoLanzamiento.AgregarJugadorJuegoLanzamiento("3F2504E0-4F89-11D3-9A0C-0305E82C3301", "sulem477@gmail.com");
        }

        public void Dispose()
        {
            _proxyJuegoLanzamiento.EliminarJugadorJuegoLanzamiento("3F2504E0-4F89-11D3-9A0C-0305E82C3301");
            _proxyNuevoJuegoLanzamiento.EliminarJugadorJuegoLanzamiento("3F2504E0-4F89-11D3-9A0C-0305E82C3301");
        }

        [Fact]
        public async void EliminarJugadorLlamadaCallBackJugadorPrueba()
        {
            string codigoPartida = "3F2504E0-4F89-11D3-9A0C-0305E82C3301";
            string correoJugadorSalido = "jazmin@gmail.com";

            _proxyJuegoLanzamiento.EliminarJugadorJuegoLanzamiento(codigoPartida);

            await Task.Delay(2000);
            Assert.True(_nuevaImplementacionCallbackJuegoLanzamiento.HaActivadoEventoEliminarJugador);
            Assert.Equal(_nuevaImplementacionCallbackJuegoLanzamiento.CorreoElectronicoSalido, correoJugadorSalido);

        }
        [Fact]
        public async void EliminarJugadorPartidaNoExistentePrueba()
        {
            string codigoPartida = "3F2504E0-4F89-11D3-9A0C-0305E82C3302";

            _proxyJuegoLanzamiento.EliminarJugadorJuegoLanzamiento(codigoPartida);

            string correoEsperado = "";

            await Task.Delay(2000);
            Assert.False(_nuevaImplementacionCallbackJuegoLanzamiento.HaActivadoEventoEliminarJugador);
            Assert.Equal(_nuevaImplementacionCallbackJuegoLanzamiento.CorreoElectronicoSalido, correoEsperado);

        }

        [Fact]
        public async void NotificarFinalizarLanzamientoJugadorLlamadaCallBackJugadorPrueba()
        {
            string codigoPartida = "3F2504E0-4F89-11D3-9A0C-0305E82C3301";
            string correoJugador = "jazmin@gmail.com";
            string correoJugadorNuevo = "sulem477@gmail.com";

            _proxyJuegoLanzamiento.NotificarFinalizarLanzamiento(codigoPartida, correoJugador);
            _proxyNuevoJuegoLanzamiento.NotificarFinalizarLanzamiento(codigoPartida, correoJugadorNuevo);

            await Task.Delay(2000);
            Assert.True(_implementacionCallbackJuegoLanzamiento.HaActivadoEventoNotificarSiguienteTurno);
            Assert.True(_nuevaImplementacionCallbackJuegoLanzamiento.HaActivadoEventoNotificarSiguienteTurno);
        }
        [Fact]
        public async void NotificarFinalizarLanzamientoPartidaNoExistentePrueba()
        {
            string codigoPartida = "3F2504E0-4F89-11D3-9A0C-0305E82C3302";
            string correoJugador = "jazmin@gmail.com";
            string correoJugadorNuevo = "sulem477@gmail.com";

            _proxyJuegoLanzamiento.NotificarFinalizarLanzamiento(codigoPartida, correoJugador);
            _proxyNuevoJuegoLanzamiento.NotificarFinalizarLanzamiento(codigoPartida, correoJugadorNuevo);

            await Task.Delay(2000);
            Assert.False(_implementacionCallbackJuegoLanzamiento.HaActivadoEventoNotificarSiguienteTurno);
            Assert.False(_nuevaImplementacionCallbackJuegoLanzamiento.HaActivadoEventoNotificarSiguienteTurno);
        }
        [Fact]
        public async void NotificarLanzamientoDadoLlamadaCallBackJugadorPrueba()
        {
            string codigoPartida = "3F2504E0-4F89-11D3-9A0C-0305E82C3301";
            string correoJugador = "jazmin@gmail.com";
            int numeroDado = 1;

            _proxyJuegoLanzamiento.NotificarLanzamientoDado(codigoPartida, correoJugador, 1);

            await Task.Delay(2000);
            Assert.True(_nuevaImplementacionCallbackJuegoLanzamiento.HaActivadoEventoJugadorTirarDado);
            Assert.Equal(_nuevaImplementacionCallbackJuegoLanzamiento.NumeroDadoNotificado, numeroDado);
        }
        [Fact]
        public async void NotificarLanzamientoDadoPartidaNoExistentePrueba()
        {
            string codigoPartida = "3F2504E0-4F89-11D3-9A0C-0305E82C3302";
            string correoJugador = "jazmin@gmail.com";
            int numeroDadoEsperado = -1;

            _proxyJuegoLanzamiento.NotificarLanzamientoDado(codigoPartida, correoJugador, 1);

            await Task.Delay(2000);
            Assert.False(_nuevaImplementacionCallbackJuegoLanzamiento.HaActivadoEventoJugadorTirarDado);
            Assert.Equal(_nuevaImplementacionCallbackJuegoLanzamiento.NumeroDadoNotificado, numeroDadoEsperado);
        }
        [Fact]
        public async void NotificarLanzamientoPeonLlamadaCallBackJugadorPrueba()
        {
            string codigoPartida = "3F2504E0-4F89-11D3-9A0C-0305E82C3301";
            string correoJugador = "jazmin@gmail.com";
            int posicionX = 12;
            int posicionY = 14;

            _proxyJuegoLanzamiento.NotificarLanzamientoLinea(codigoPartida, correoJugador, posicionX, posicionY);

            await Task.Delay(2000);
            Assert.True(_nuevaImplementacionCallbackJuegoLanzamiento.HaActivadoEventoJugadorLanzoPeon);
            Assert.Equal(_nuevaImplementacionCallbackJuegoLanzamiento.PosicionX, posicionX);
            Assert.Equal(_nuevaImplementacionCallbackJuegoLanzamiento.PosicionY, posicionY);
        }
        [Fact]
        public async void NotificarLanzamientoPartidaNoExistentePrueba()
        {
            string codigoPartida = "3F2504E0-4F89-11D3-9A0C-0305E82C3302";
            string correoJugador = "jazmin@gmail.com";
            int posicionX = 12;
            int posicionY = 14;

            _proxyJuegoLanzamiento.NotificarLanzamientoLinea(codigoPartida, correoJugador, posicionX, posicionY);

            int posicionXEsperada = -1;
            int posicionYEsperada = -1;
             

            await Task.Delay(2000);
            Assert.False(_nuevaImplementacionCallbackJuegoLanzamiento.HaActivadoEventoJugadorLanzoPeon);
            Assert.Equal(_nuevaImplementacionCallbackJuegoLanzamiento.PosicionX, posicionXEsperada);
            Assert.Equal(_nuevaImplementacionCallbackJuegoLanzamiento.PosicionY, posicionYEsperada);
        }
        [Fact]
        public async void NotificarCambiarPosicionPeonesTableroYContinuarPrueba()
        {
            string codigoPartida = "3F2504E0-4F89-11D3-9A0C-0305E82C3301";
            string correoJugador = "jazmin@gmail.com";
            PeonesTablero peones = new PeonesTablero();

            _proxyJuegoLanzamiento.NotificarPosicionFichasFinales(codigoPartida, correoJugador, peones);

            await Task.Delay(2000);
            Assert.True(_nuevaImplementacionCallbackJuegoLanzamiento.HaActivadoEventoCambiarPosiciones);
        }
        [Fact]
        public async void NotificarCambiarPosicionPartidaNoExistentePrueba()
        {
            string codigoPartida = "3F2504E0-4F89-11D3-9A0C-0305E82C3303";
            string correoJugador = "jazmin@gmail.com";
            PeonesTablero peones = new PeonesTablero();

            _proxyJuegoLanzamiento.NotificarPosicionFichasFinales(codigoPartida, correoJugador, peones);

            await Task.Delay(2000);
            Assert.False(_nuevaImplementacionCallbackJuegoLanzamiento.HaActivadoEventoCambiarPosiciones);
        }

    }

    public class ImplementacionPruebaCallbackJuegoLanzamiento : IJuegoLanzamientoCallback
    {
        public bool HaActivadoEventoEliminarJugador { get; set; }
        public bool HaActivadoEventoNotificarSiguienteTurno { get; set; }
        public string CorreoElectronicoSalido { get; set; }
        public bool HaActivadoEventoJugadorTirarDado { get; set; }
        public int NumeroDadoNotificado { get; set; }
        public double PosicionX { get; set; }
        public double PosicionY { get; set; }
        public bool HaActivadoEventoJugadorLanzoPeon { get; set; }
        public bool HaActivadoEventoCambiarPosiciones { get; set; }

        public ImplementacionPruebaCallbackJuegoLanzamiento()
        {
            HaActivadoEventoEliminarJugador = false;
            HaActivadoEventoNotificarSiguienteTurno = false;
            HaActivadoEventoJugadorTirarDado = false;
            HaActivadoEventoJugadorLanzoPeon = false;
            HaActivadoEventoCambiarPosiciones = false;

            CorreoElectronicoSalido = "";
            NumeroDadoNotificado = -1;
            PosicionX = -1;
            PosicionY = -1;
        }
        public void JugadorDetuvoLinea(double posicionX, double posicionY)
        {
            this.PosicionX = posicionX;
            PosicionY = posicionY;
            HaActivadoEventoJugadorLanzoPeon = true;
        }

        public void JugadoresListosParaSiguienteTurno()
        {
            HaActivadoEventoNotificarSiguienteTurno = true;
        }

        public void JugadorSalioJuegoLanzamiento(string correoElectronicoSalido)
        {
            HaActivadoEventoEliminarJugador = true;
            CorreoElectronicoSalido = correoElectronicoSalido;
        }

        public void JugadorTiroDado(int numeroDado)
        {
            HaActivadoEventoJugadorTirarDado = true;
            NumeroDadoNotificado = numeroDado;
        }

        public void CambiarPosicionPeonesTableroYContinuar(PeonesTablero peones)
        {
            HaActivadoEventoCambiarPosiciones = true;
        }
    }
}