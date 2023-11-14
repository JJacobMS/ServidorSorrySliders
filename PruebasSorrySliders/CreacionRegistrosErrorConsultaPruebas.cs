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
        [Fact]
        public void VerificarInsertarCuentaUsuarioIncorrectoPrueba()
        {
            Constantes respuestaEsperado = Constantes.ERROR_CONSULTA;
            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            UsuarioSet usuario = new UsuarioSet
            {
                Nombre = "Sulem",
                Apellido = "Martinez",
            };

            CuentaSet cuenta = new CuentaSet
            {
                CorreoElectronico = "correoEjemplo@gmail.com",
                Contraseña = "password12345"
            };
            Constantes resultadoObtenidos = servicioComunicacion.AgregarUsuario(usuario, cuenta);

            Assert.Equal(respuestaEsperado, resultadoObtenidos);

        }
    }
}
