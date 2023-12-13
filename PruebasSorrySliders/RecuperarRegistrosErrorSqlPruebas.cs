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
    public class RecuperarRegistrosErrorSqlPruebas
    {
        /// <seealso cref="InterfacesServidorSorrySliders.IInicioSesion"/>
        [Fact]
        public void VerificarContrasenaCuentaErrorSqlPrueba()
        {
            Constantes respuestaEsperada = Constantes.ERROR_CONSULTA;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            CuentaSet cuentaNoExistente = new CuentaSet 
            { 
                CorreoElectronico = "correoPrueba@gmail.com" 
            };

            Constantes respuestaActual = servicioComunicacion.VerificarContrasenaDeCuenta(cuentaNoExistente);

            Assert.Equal(respuestaEsperada, respuestaActual);

        }

        /// <seealso cref="InterfacesServidorSorrySliders.IDetallesCuentaUsuario"/>
        [Fact]
        public void VerificarRecuperarUsuarioIncompletoErrorSqlPrueba()
        {
            Constantes respuestaEsperada = Constantes.ERROR_CONSULTA;
            UsuarioSet usuarioEsperado = null;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            string correoNoExistente = null;

            (Constantes respuestaActual, UsuarioSet usuarioActual) = servicioComunicacion.RecuperarDatosUsuarioDeCuenta(correoNoExistente);

            Assert.Equal(respuestaEsperada, respuestaActual);
            Assert.Equal(usuarioEsperado, usuarioActual);
        }
        [Fact]
        public void VerificarCuentaYContrasenaIncompletaErrorSqlPrueba()
        {
            Constantes respuestaEsperada = Constantes.ERROR_CONSULTA;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            CuentaSet cuentaNoExistente = new CuentaSet();

            Constantes respuestaActual = servicioComunicacion.VerificarContrasenaActual(cuentaNoExistente);

            Assert.Equal(respuestaEsperada, respuestaActual);
        }

        /// <seealso cref="InterfacesServidorSorrySliders.IListaAmigos"/>
        [Fact]
        public void VerificarRecuperarAmigosCuentaIncompletosErrorSqlPrueba()
        {
            Constantes respuestaEsperada = Constantes.ERROR_CONSULTA;
            string correoElectronicoPrueba = null;

            List<CuentaSet> amigosEsperados = null;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            (Constantes respuestaActual, List<CuentaSet> amigosActuales) = servicioComunicacion.RecuperarAmigosCuenta(correoElectronicoPrueba);

            Assert.Equal(respuestaEsperada, respuestaActual);
            Assert.Equal(amigosEsperados, amigosActuales);
        }
        [Fact]
        public void VerificarRecuperarCuentasIncompletasErrorSqlPrueba()
        {
            Constantes respuestaEsperada = Constantes.ERROR_CONSULTA;
            string informacionJugador = null;
            string correoBuscar = null;

            List<CuentaSet> cuentasEsperadas = null;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            (Constantes respuestaActual, List<CuentaSet> cuentasActuales) = servicioComunicacion.RecuperarJugadoresCuenta(informacionJugador, correoBuscar);

            Assert.Equal(respuestaEsperada, respuestaActual);
            Assert.Equal(cuentasEsperadas, cuentasActuales);
        }

        /// <seealso cref="InterfacesServidorSorrySliders.IUnirsePartida"/>
        [Fact]
        public void VerificarRecuperarJugadoresLobbyDatosIncompletosErrorSqlPrueba()
        {
            Constantes respuestaEsperada = Constantes.ERROR_CONSULTA;
            string uidLobby = null;

            List<CuentaSet> cuentasEsperadas = null;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            (Constantes respuestaActual, List<CuentaSet> cuentasLobbyActuales) = servicioComunicacion.RecuperarJugadoresLobby(uidLobby);

            Assert.Equal(respuestaEsperada, respuestaActual);
            Assert.Equal(cuentasEsperadas, cuentasLobbyActuales);
        }
    }
}
