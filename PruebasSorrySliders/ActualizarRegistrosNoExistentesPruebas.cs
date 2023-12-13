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
    public class ActualizarRegistrosNoExistentesPruebas
    {
        /// <seealso cref="InterfacesServidorSorrySliders.IDetallesCuentaUsuario"/>
        [Fact]
        public void VerificarCambioContrasenaCuentaNoExistentePrueba()
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

        /// <seealso cref="InterfacesServidorSorrySliders.IRegistroUsuario"/>
        [Fact]
        public void ActualizarUsuarioNoExistentePrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA_VACIA;
            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            var nuevaActualizada = new CuentaSet
            {
                CorreoElectronico = "",
                Nickname = "",
                Avatar= BitConverter.GetBytes(0102030405)
            };
            var usuarioActualizado = new UsuarioSet 
            {
                Nombre="",
                Apellido="",
            };
            Constantes respuestaActual = servicioComunicacion.ActualizarUsuario(usuarioActualizado, nuevaActualizada);
            Assert.Equal(respuestaActual, respuestaEsperada);
        }

        /// <seealso cref="InterfacesServidorSorrySliders.IJuegoPuntuacion"/>
        [Fact]
        public void ActualizarGanadorNoExistentePrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA_VACIA;
            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            string uid = "00000000-0000-0000-0000-000000000000";
            string correoElectronico = "";
            int posicion = 0;
            Constantes respuestaActual = servicioComunicacion.ActualizarGanador(uid, correoElectronico, posicion);
            Assert.Equal(respuestaActual, respuestaEsperada);
        }
    }
}
