
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

    }

    public class ImplementacionPruebaCallbackJuegoLanzamiento : IJuegoLanzamientoCallback
    {
        private bool _haActivadoEventoEliminarJugador;
        private bool _haActivadoEventoNotificarSiguienteTurno;
        private string _correoElectronicoSalido;
        private bool _haActivadoEventoJugadorTirarDado;
        private int _numeroDadoNotificado;
        private double _posicionX;
        private double _posicionY;
        private bool _haActivadoEventoJugadorLanzoPeon;
        public bool HaActivadoEventoEliminarJugador 
        { 
            get => _haActivadoEventoEliminarJugador; 
        }
        public bool HaActivadoEventoNotificarSiguienteTurno 
        { 
            get => _haActivadoEventoNotificarSiguienteTurno;  
        }
        public string CorreoElectronicoSalido 
        { 
            get => _correoElectronicoSalido;    
        }
        public bool HaActivadoEventoJugadorTirarDado 
        { 
            get => _haActivadoEventoJugadorTirarDado; 
        }
        public int NumeroDadoNotificado 
        { 
            get => _numeroDadoNotificado;
        }
        public double PosicionX 
        { 
            get => _posicionX; 
        }
        public double PosicionY 
        { 
            get => _posicionY; 
        }
        public bool HaActivadoEventoJugadorLanzoPeon 
        { 
            get => _haActivadoEventoJugadorLanzoPeon; 
        }

        public ImplementacionPruebaCallbackJuegoLanzamiento()
        {
            _haActivadoEventoEliminarJugador = false;
            _haActivadoEventoNotificarSiguienteTurno = false;
            _haActivadoEventoJugadorTirarDado = false;
            _haActivadoEventoJugadorLanzoPeon = false;

            _correoElectronicoSalido = "";
            _numeroDadoNotificado = -1;
            _posicionX = -1;
            _posicionY = -1;
        }
        public void JugadorDetuvoLinea(double posicionX, double posicionY)
        {
            _posicionX = posicionX;
            _posicionY = posicionY;
            _haActivadoEventoJugadorLanzoPeon = true;
        }

        public void JugadoresListosParaSiguienteTurno()
        {
            _haActivadoEventoNotificarSiguienteTurno = true;
        }

        public void JugadorSalioJuegoLanzamiento(string correoElectronicoSalido)
        {
            _haActivadoEventoEliminarJugador = true;
            _correoElectronicoSalido = correoElectronicoSalido;
        }

        public void JugadorTiroDado(int numeroDado)
        {
            _haActivadoEventoJugadorTirarDado = true;
            _numeroDadoNotificado = numeroDado;
        }
    }
}