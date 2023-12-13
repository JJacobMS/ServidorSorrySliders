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
    public class RecuperarRegistrosErrorConexionBaseDatosPruebas
    {
        //IInicioSesion
        [Fact]
        public void VerificarCorreoSinConexionBDPrueba()
        {
            Constantes respuestaEsperada = Constantes.ERROR_CONEXION_BD;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            string correo = "correoPrueba@gmail.com";

            Constantes respuestaActual = servicioComunicacion.VerificarExistenciaCorreoCuenta(correo);

            Assert.Equal(respuestaEsperada, respuestaActual);
        }

        [Fact]
        public void VerificarContrasenaSinConexionBDPrueba()
        {
            Constantes respuestaEsperada = Constantes.ERROR_CONEXION_BD;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            CuentaSet cuenta = new CuentaSet 
            { 
                CorreoElectronico = "correoPrueba@gmail.com", 
                Contraseña = "1234567890" 
            };

            Constantes respuestaActual = servicioComunicacion.VerificarContrasenaDeCuenta(cuenta);

            Assert.Equal(respuestaEsperada, respuestaActual);

        }
        //IDetallesCuentaUsuario
        [Fact]
        public void VerificarRecuperarUsuarioSinConexionBDPrueba()
        {
            Constantes respuestaEsperada = Constantes.ERROR_CONEXION_BD;
            UsuarioSet usuarioEsperado = null;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            string correo = "correoPrueba@gmail.com";

            (Constantes respuestaActual, UsuarioSet usuarioActual) = servicioComunicacion.RecuperarDatosUsuarioDeCuenta(correo);

            Assert.Equal(respuestaEsperada, respuestaActual);
            Assert.Equal(usuarioEsperado, usuarioActual);
        }
        [Fact]
        public void VerificarCuentaYContrasenaSinConexionBDPrueba()
        {
            Constantes respuestaEsperada = Constantes.ERROR_CONEXION_BD;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            CuentaSet cuentaNoExistente = new CuentaSet();

            Constantes respuestaActual = servicioComunicacion.VerificarContrasenaActual(cuentaNoExistente);

            Assert.Equal(respuestaEsperada, respuestaActual);
        }
        //IMenuPrincipal
        [Fact]
        public void VerificarRecuperarDatosUsuarioSinConexionBDPrueba()
        {
            Constantes respuestaEsperada = Constantes.ERROR_CONEXION_BD;
            string correoElectronicoExistente = "correoPrueba@gmail.com";
            string nicknameEsperado = null;
            byte[] avatarEsperado = null;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            (Constantes respuestaActual, string nickname, byte[] avatar) = servicioComunicacion.RecuperarDatosUsuario(correoElectronicoExistente);

            Assert.Equal(respuestaEsperada, respuestaActual);
            Assert.Equal(nicknameEsperado, nickname);
            Assert.Equal(avatarEsperado, avatar);
        }
        //Pruebas Interfaces IListaAmigos
        [Fact]
        public void VerificarRecuperarAmigosCuentaSinConexionBDPrueba()
        {
            Constantes respuestaEsperada = Constantes.ERROR_CONEXION_BD;
            string correoElectronicoPrueba = null;

            List<CuentaSet> amigosEsperados = null;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            (Constantes respuestaActual, List<CuentaSet> amigosActuales) = servicioComunicacion.RecuperarAmigosCuenta(correoElectronicoPrueba);

            Assert.Equal(respuestaEsperada, respuestaActual);
            Assert.Equal(amigosEsperados, amigosActuales);
        }
        [Fact]
        public void VerificarRecuperarCuentasSinConexionPrueba()
        {
            Constantes respuestaEsperada = Constantes.ERROR_CONEXION_BD;
            string informacionJugador = null;
            string correoBuscar = null;

            List<CuentaSet> cuentasEsperadas = null;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            (Constantes respuestaActual, List<CuentaSet> cuentasActuales) = servicioComunicacion.RecuperarJugadoresCuenta(informacionJugador, correoBuscar);

            Assert.Equal(respuestaEsperada, respuestaActual);
            Assert.Equal(cuentasEsperadas, cuentasActuales);
        }
        //IUnirsePartida
        [Fact]
        public void VerificarRecuperarJugadoresLobbySinConexionBDPrueba()
        {
            Constantes respuestaEsperada = Constantes.ERROR_CONEXION_BD;
            string uidLobby = null;

            List<CuentaSet> cuentasEsperadas = null;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            (Constantes respuestaActual, List<CuentaSet> cuentasLobbyActuales) = servicioComunicacion.RecuperarJugadoresLobby(uidLobby);

            Assert.Equal(respuestaEsperada, respuestaActual);
            Assert.Equal(cuentasEsperadas, cuentasLobbyActuales);
        }
    }
}
