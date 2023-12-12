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
    public class RecuperarRegistrosVaciosPruebas
    {
        //IInicioSesion
        [Fact]
        public void VerificarCorreoNoExistentePrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA_VACIA;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            string correoCorrectoNoExistente = "correoPrueba@gmail.com";

            Constantes respuestaActual = servicioComunicacion.VerificarExistenciaCorreoCuenta(correoCorrectoNoExistente);

            Assert.Equal(respuestaEsperada, respuestaActual);
        }

        [Fact]
        public void VerificarContrasenaCuentaNoExistentePrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA_VACIA;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            CuentaSet cuentaNoExistente = new CuentaSet { CorreoElectronico = "correoPrueba@gmail.com", Contraseña = "1234567890" };

            Constantes respuestaActual = servicioComunicacion.VerificarContrasenaDeCuenta(cuentaNoExistente);

            Assert.Equal(respuestaEsperada, respuestaActual);

        }
        
        //Pruebas Interfaces IDetallesCuentaUsuario
        [Fact]
        public void VerificarRecuperarUsuarioNoExistentePrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA_VACIA;
            UsuarioSet usuarioEsperado = null;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            string correoNoExistente = "correoPrueba@gmail.com";

            (Constantes respuestaActual, UsuarioSet usuarioActual) = servicioComunicacion.RecuperarDatosUsuarioDeCuenta(correoNoExistente);

            Assert.Equal(respuestaEsperada, respuestaActual);
            Assert.Equal(usuarioEsperado, usuarioActual);
        }

        [Fact]
        public void VerificarCuentaNoExistenteContrasenaNoExistentePrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA_VACIA;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            CuentaSet cuentaNoExistente = new CuentaSet
            {
                CorreoElectronico = "correoPrueba@gmail.com",
                Contraseña = "1234567890"
            };

            Constantes respuestaActual = servicioComunicacion.VerificarContrasenaActual(cuentaNoExistente);

            Assert.Equal(respuestaEsperada, respuestaActual);
        }
        //Pruebas Interfaces IListaAmigos
        [Fact]
        public void VerificarRecuperarAmigosCuentaNoExistentePrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA_VACIA;
            string correoElectronicoPrueba = "correoPrueba@gmail.com";

            List<CuentaSet> amigosEsperados = null;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            (Constantes respuestaActual, List<CuentaSet> amigosActuales) = servicioComunicacion.RecuperarAmigosCuenta(correoElectronicoPrueba);

            Assert.Equal(respuestaEsperada, respuestaActual);
            Assert.Equal(amigosEsperados, amigosActuales);
        }

        [Fact]
        public void VerificarRecuperarCuentasNoExistentesPrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA_VACIA;
            string informacionJugador = "amigo";
            string correoElectronicoPrueba = "correoPrueba@gmail.com";

            List<CuentaSet> cuentasEsperadas = null;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            (Constantes respuestaActual, List<CuentaSet> cuentasActuales) = servicioComunicacion.RecuperarJugadoresCuenta(informacionJugador, correoElectronicoPrueba);

            Assert.Equal(respuestaEsperada, respuestaActual);
            Assert.Equal(cuentasEsperadas, cuentasActuales);
        }
        //Pruebas Interfaces IUnirsePartida
        [Fact]
        public void VerificarRecuperarJugadoresLobbyNoExistentesPrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA_VACIA;
            string uidLobby = "00000000-0000-0000-0000-000000000000";

            List<CuentaSet> cuentasEsperadas = null;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            (Constantes respuestaActual, List<CuentaSet> cuentasLobbyActuales) = servicioComunicacion.RecuperarJugadoresLobby(uidLobby);

            Assert.Equal(respuestaEsperada, respuestaActual);
            Assert.Equal(cuentasEsperadas, cuentasLobbyActuales);
        }
        //IMenuPrincipal
        [Fact]
        public void VerificarRecuperarDatosUsuarioNoExistentePrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA_VACIA;
            string correoElectronicoExistente = "correoPrueba@gmail.com";
            string nicknameEsperado = null;
            byte[] avatarEsperado = null;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            (Constantes respuestaActual, string nickname, byte[] avatar) = servicioComunicacion.RecuperarDatosUsuario(correoElectronicoExistente);

            Assert.Equal(respuestaEsperada, respuestaActual);
            Assert.Equal(nicknameEsperado, nickname);
            Assert.Equal(avatarEsperado, avatar);
        }
        //IPuntuaciones
        
    }
}

