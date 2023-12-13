using DatosSorrySliders;
using ServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PruebasSorrySliders
{
    public class ActualizarRegistrosErrorSqlPruebas
    {
        public ActualizarRegistrosErrorSqlPruebas()
        {
            ObtenerCadenaConexion.ObtenerCadenaConexionBaseDatos();
        }
        /// <seealso cref="InterfacesServidorSorrySliders.IDetallesCuentaUsuario"/>
        [Fact]
        public void VerificarCambioContrasenaCuentaErrorSqlPrueba()
        {
            Constantes respuestaEsperada = Constantes.ERROR_CONSULTA;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            CuentaSet cuentaExistente = new CuentaSet();

            Constantes respuestaActual = servicioComunicacion.CambiarContrasena(cuentaExistente);

            Assert.Equal(respuestaEsperada, respuestaActual);
        }

        /// <seealso cref="InterfacesServidorSorrySliders.IRegistroUsuario"/>
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

        /// <seealso cref="InterfacesServidorSorrySliders.IJuegoPuntuacion"/>
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
