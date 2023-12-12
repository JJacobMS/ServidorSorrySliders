using DatosSorrySliders;
using ServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PruebasSorrySliders
{
    public class CreacionRegistrosErrorConsultaPruebas
    {
        //IDetallesCuenta
        [Fact]
        public void VerificarInsertarCuentaUsuarioIncorrectoPrueba()
        {
            Constantes respuestaEsperado = Constantes.ERROR_CONSULTA;
            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            UsuarioSet usuario = new UsuarioSet();

            CuentaSet cuenta = new CuentaSet();
            Constantes resultadoObtenidos = servicioComunicacion.AgregarUsuario(usuario, cuenta);

            Assert.Equal(respuestaEsperado, resultadoObtenidos);

        }
        //IUnirsePartida
        [Fact]
        public void VerificarIntentarEntrarPartidaDatosIncompletosPrueba()
        {
            Constantes respuestaEsperado = Constantes.ERROR_CONSULTA;
            int jugadoresMaximos = -1;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            string correoExistente = null;
            string uidExistente = null;

            (Constantes resultadoObtenidos, int cantidadJugadoresPrevios) = servicioComunicacion.UnirseAlLobby(uidExistente, correoExistente);

            Assert.Equal(respuestaEsperado, resultadoObtenidos);
            Assert.Equal(jugadoresMaximos, cantidadJugadoresPrevios);
        }
        [Fact]
        public void VerificarInsertarCuentaProvisionalDatosIncompletosPrueba()
        {
            Constantes respuestaEsperado = Constantes.ERROR_CONSULTA;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            CuentaSet cuentaProvisional = new CuentaSet();

            Constantes resultadoObtenidos = servicioComunicacion.CrearCuentaProvisionalInvitado(cuentaProvisional);

            Assert.Equal(respuestaEsperado, resultadoObtenidos);
        }


        //ICrearLobby
        [Fact]
        public void VerificarCrearPartidaDatosIncompletosPrueba()
        {
            Constantes respuestaEsperado = Constantes.ERROR_CONSULTA;
            Constantes resultadoObtenido;
            string codigo;
            int cantidadJugadores = 4;
            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            CuentaSet cuenta = new CuentaSet();
            (resultadoObtenido, codigo) = servicioComunicacion.CrearPartida(cuenta.CorreoElectronico, cantidadJugadores);
            Assert.Equal(respuestaEsperado, resultadoObtenido);

        }
        //IListaAmigos
        [Fact]
        public void VerificarCrearNotificacionDatosIncompletosPrueba()
        {
            Constantes respuestaEsperado = Constantes.ERROR_CONSULTA;
            Constantes resultadoObtenido;
            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            NotificacionSet notificacionNueva = new NotificacionSet();
            resultadoObtenido = servicioComunicacion.GuardarNotificacion(notificacionNueva);
            Assert.Equal(respuestaEsperado, resultadoObtenido);
        }

        [Fact]
        public void VerificarCrearAmistadDatosIncompletosPrueba()
        {
            Constantes respuestaEsperado = Constantes.ERROR_CONSULTA;
            Constantes resultadoObtenido;
            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            string CorreoElectronicoDestinatario = null;
            string CorreoElectronicoRemitente = null;
            resultadoObtenido = servicioComunicacion.GuardarAmistad(CorreoElectronicoDestinatario, CorreoElectronicoRemitente);
            Assert.Equal(respuestaEsperado, resultadoObtenido);
        }

        [Fact]
        public void VerificarCrearBaneoDatosIncompletosPrueba()
        {
            Constantes respuestaEsperado = Constantes.ERROR_CONSULTA;
            Constantes resultadoObtenido;
            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            string CorreoElectronicoDestinatario = null;
            string CorreoElectronicoRemitente = null;
            resultadoObtenido = servicioComunicacion.BanearJugador(CorreoElectronicoDestinatario, CorreoElectronicoRemitente);
            Assert.Equal(respuestaEsperado, resultadoObtenido);
        }
    }
}
