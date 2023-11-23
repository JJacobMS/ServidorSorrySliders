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
    //Para esta prueba no se debería tener una conexión con la base de datos
    public class ActualizarRegistroErrorConexionBaseDatosPruebas
    {
        [Fact]
        public void VerificarCambioContrasenaCuentaNoExistentePrueba()
        {
            Constantes respuestaEsperada = Constantes.ERROR_CONEXION_BD;

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
