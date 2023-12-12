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
    public class ActualizarRegistrosErrorSqlPruebas
    {
        [Fact]
        public void VerificarCambioContrasenaCuentaErrorSqlPrueba()
        {
            Constantes respuestaEsperada = Constantes.ERROR_CONSULTA;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            CuentaSet cuentaExistente = new CuentaSet();

            Constantes respuestaActual = servicioComunicacion.CambiarContrasena(cuentaExistente);

            Assert.Equal(respuestaEsperada, respuestaActual);
        }

        //IRegistroUsuario
        [Fact]
        public void ActualizarUsuarioErrorSqlPrueba()
        {
            Constantes respuestaEsperada = Constantes.ERROR_CONSULTA;
            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            var nuevaActualizada = new CuentaSet();
            var usuarioActualizado = new UsuarioSet();
            Constantes respuestaActual = servicioComunicacion.ActualizarUsuario(usuarioActualizado, nuevaActualizada);
            Assert.Equal(respuestaActual, respuestaEsperada);
        }
        //IJuegoPuntuacion
        [Fact]
        public void ActualizarGanadorErrorSqlPrueba()
        {
            Constantes respuestaEsperada = Constantes.ERROR_CONSULTA;
            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            string uid = null;
            string correoElectronico = null;
            int posicion = 0;
            Constantes respuestaActual = servicioComunicacion.ActualizarGanador(uid, correoElectronico, posicion);
            Assert.Equal(respuestaActual, respuestaEsperada);
        }
    }
}
