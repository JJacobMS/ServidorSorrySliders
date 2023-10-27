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
    public class ServidorComunicacionSorrySlidersDetallesCuentaUsuarioPruebas
    {
        [Fact]
        public void VerificarRecuperarDetallesUsuarioExistente()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA;
            UsuarioSet usuarioEsperado = new UsuarioSet
            {
                Nombre = "Sulem", Apellido = "Martinez"
            };

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            string correoCorrectoExistente = "sulem477@gmail.com";

            (Constantes respuestaActual, UsuarioSet usuarioActual) = servicioComunicacion.RecuperarDatosUsuarioDeCuenta(correoCorrectoExistente);

            Assert.Equal(respuestaEsperada, respuestaActual);
            Assert.Equal(usuarioEsperado, usuarioActual);
        }
        [Fact]
        public void VerificarNoRecuperarDetallesUsuarioNoExistente()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA_VACIA;
            UsuarioSet usuarioEsperado = null;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            string correoCorrectoNoExistente = "correoNoExistente";

            (Constantes respuestaActual, UsuarioSet usuarioActual) = servicioComunicacion.RecuperarDatosUsuarioDeCuenta(correoCorrectoNoExistente);

            Assert.Equal(respuestaEsperada, respuestaActual);
            Assert.Equal(usuarioEsperado, usuarioActual);
        }
        [Fact]
        public void VerificarExistenciaContrasenaExistente()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            CuentaSet cuentaExistente = new CuentaSet
            {
                CorreoElectronico = "sulem477@gmail.com", Contraseña = "1234567890"
            };

            Constantes respuestaActual = servicioComunicacion.VerificarContrasenaActual(cuentaExistente);

            Assert.Equal(respuestaEsperada, respuestaActual);
        }
        [Fact]
        public void VerificarNoExistenciaContrasenaErronea()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA_VACIA;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            CuentaSet cuentaExistente = new CuentaSet
            {
                CorreoElectronico = "sulem477@gmail.com",
                Contraseña = "1"
            };

            Constantes respuestaActual = servicioComunicacion.VerificarContrasenaActual(cuentaExistente);

            Assert.Equal(respuestaEsperada, respuestaActual);
        }
        [Fact]
        public void VerificarCambioContrasenaCuentaExistente()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            CuentaSet cuentaExistente = new CuentaSet
            {
                CorreoElectronico = "sulem477@gmail.com",
                Contraseña = "01234567890"
            };

            Constantes respuestaActual = servicioComunicacion.CambiarContrasena(cuentaExistente);

            Assert.Equal(respuestaEsperada, respuestaActual);
        }
        [Fact]
        public void VerificarCambioContrasenaCuentaNoExistente()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA_VACIA;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            CuentaSet cuentaExistente = new CuentaSet
            {
                CorreoElectronico = "correoNoExistente",
                Contraseña = "01234567890"
            };

            Constantes respuestaActual = servicioComunicacion.CambiarContrasena(cuentaExistente);

            Assert.Equal(respuestaEsperada, respuestaActual);
        }
    }
}
