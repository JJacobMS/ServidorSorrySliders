using DatosSorrySliders;
using ServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PruebasSorrySliders
{
    public class ConfiguracionBaseDatosCrearRegistrosExistosos : IDisposable
    {
        public ConfiguracionBaseDatosCrearRegistrosExistosos() { }

        public void Dispose()
        {
            try
            {
                using (var context = new BaseDeDatosSorrySlidersEntities())
                {
                    context.Database.ExecuteSqlCommand("DELETE FROM CuentaSet WHERE CorreoElectronico = @correo",
                        new SqlParameter("@correo", "correoParaPruebas@gmail.com"));

                    context.Database.ExecuteSqlCommand("DELETE FROM UsuarioSet WHERE Nombre = @nombre AND Apellido = @apellido",
                        new SqlParameter("@nombre", "nombrePrueba"), new SqlParameter("@apellido", "apellidoPrueba"));
                    
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex);
            }
            catch (EntityException ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    public class CreacionRegistrosExitososPruebas : IClassFixture<ConfiguracionBaseDatosCrearRegistrosExistosos>
    {
        ConfiguracionBaseDatosCrearRegistrosExistosos _configuracion;

        public CreacionRegistrosExitososPruebas(ConfiguracionBaseDatosCrearRegistrosExistosos configuracion)
        {
            _configuracion = configuracion;
        }

        [Fact]
        public void VerificarInsertarCuentaUsuarioExitosamentePrueba()
        {
            Constantes respuestaEsperado = Constantes.OPERACION_EXITOSA;
            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            UsuarioSet usuario = new UsuarioSet
            {
                Nombre = "nombrePrueba",
                Apellido = "apellidoPrueba",
            };

            CuentaSet cuenta = new CuentaSet
            {
                CorreoElectronico = "correoParaPruebas@gmail.com",
                Contraseña = "asdfghj8",
                Nickname = "nicknamePrueba",
                Avatar = BitConverter.GetBytes(0102030405)
            };
            Constantes resultadoObtenidos = servicioComunicacion.AgregarUsuario(usuario, cuenta);

            Assert.Equal(respuestaEsperado, resultadoObtenidos);
        }
    }
}
