using DatosSorrySliders;
using ServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PruebasSorrySliders
{
    public class ConfiguracionBaseDatosActualizarRegistrosExistosos : IDisposable
    {
        public ConfiguracionBaseDatosActualizarRegistrosExistosos() 
        {
            try
            {
                using (var context = new BaseDeDatosSorrySlidersEntities())
                {
                    UsuarioSet usuario = new UsuarioSet 
                    { 
                        Nombre = "nombrePrueba", 
                        Apellido = "apellidoPrueba", 
                    };
                    context.UsuarioSet.Add(usuario);
                    UsuarioSet usuario2 = new UsuarioSet 
                    { 
                        Nombre = "UsuarioDePrueba",
                        Apellido = "ApellidoDePrueba", 
                    };
                    context.UsuarioSet.Add(usuario2);
                    UsuarioSet usuario3 = new UsuarioSet 
                    { 
                        Nombre = "UsuarioDePrueba2", 
                        Apellido = "ApellidoDePrueba2", 
                    };
                    context.UsuarioSet.Add(usuario3);
                    context.SaveChanges();

                    context.Database.ExecuteSqlCommand("INSERT INTO CuentaSet(CorreoElectronico, Avatar, Contraseña, Nickname, IdUsuario) " +
                        "VALUES('correoPrueba@gmail.com', @avatar, HASHBYTES('SHA2_512', N'1234567890'), 'nicknamePrueba', @idUsuario)",
                        new SqlParameter("@avatar", BitConverter.GetBytes(0102030405)), new SqlParameter("@idUsuario", usuario.IdUsuario));

                    context.Database.ExecuteSqlCommand("INSERT INTO CuentaSet(CorreoElectronico, Avatar, Contraseña, Nickname, IdUsuario) " +
                        "VALUES('correoPrueba2@gmail.com', @avatar, HASHBYTES('SHA2_512', N'1234567890'), 'nicknamePrueba', @idUsuario)",
                        new SqlParameter("@avatar", BitConverter.GetBytes(0102030405)), new SqlParameter("@idUsuario", usuario2.IdUsuario));

                    context.Database.ExecuteSqlCommand("INSERT INTO CuentaSet(CorreoElectronico, Avatar, Contraseña, Nickname, IdUsuario) " +
                        "VALUES('correoPrueba3@gmail.com', @avatar, HASHBYTES('SHA2_512', N'1234567890'), 'nicknamePrueba', @idUsuario)",
                        new SqlParameter("@avatar", BitConverter.GetBytes(0102030405)), new SqlParameter("@idUsuario", usuario3.IdUsuario));

                    context.Database.ExecuteSqlCommand("INSERT INTO PartidaSet (CodigoPartida, CantidadJugadores, CorreoElectronico) VALUES ('00000000-0000-0000-0000-000000000000',4,'correoPrueba3@gmail.com');");


                    context.Database.ExecuteSqlCommand("INSERT INTO RelacionPartidaCuentaSet (CodigoPartida, CorreoElectronico, Posicion) VALUES ('00000000-0000-0000-0000-000000000000','correoPrueba3@gmail.com',0);");
                    
                    context.SaveChanges();

                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex);
            }
            catch (EntityException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public void Dispose()
        {
            try
            {
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    contexto.Database.ExecuteSqlCommand("DELETE FROM RelacionPartidaCuentaSet where CodigoPartida='00000000-0000-0000-0000-000000000000';");
                    
                    contexto.Database.ExecuteSqlCommand("DELETE FROM PartidaSet where CodigoPartida='00000000-0000-0000-0000-000000000000'; ");

                    contexto.Database.ExecuteSqlCommand("DELETE FROM CuentaSet WHERE CorreoElectronico = 'correoPrueba@gmail.com'");

                    contexto.Database.ExecuteSqlCommand("DELETE FROM CuentaSet WHERE CorreoElectronico = 'correoPrueba3@gmail.com'");

                    contexto.Database.ExecuteSqlCommand("DELETE FROM UsuarioSet WHERE Nombre = 'nombrePrueba' AND Apellido = 'apellidoPrueba'");

                    contexto.Database.ExecuteSqlCommand("DELETE FROM CuentaSet WHERE CorreoElectronico = 'correoPrueba2@gmail.com'");

                    contexto.Database.ExecuteSqlCommand("DELETE FROM UsuarioSet WHERE Nombre = 'UsuarioDePrueba' AND Apellido = 'ApellidoDePrueba'");

                    contexto.Database.ExecuteSqlCommand("DELETE FROM UsuarioSet WHERE Nombre = 'UsuarioDePrueba2' AND Apellido = 'ApellidoDePrueba2'");


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
    public class ActualizarRegistrosExitosamentePruebas : IClassFixture<ConfiguracionBaseDatosActualizarRegistrosExistosos>
    {
        ConfiguracionBaseDatosActualizarRegistrosExistosos _configuracion;

        public ActualizarRegistrosExitosamentePruebas(ConfiguracionBaseDatosActualizarRegistrosExistosos configuracion)
        {
            _configuracion = configuracion;
        }

        //IDetallesCuenta

        [Fact]
        public void VerificarCambioContrasenaCuentaExistentePrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            CuentaSet cuentaExistente = new CuentaSet
            {
                CorreoElectronico = "correoPrueba@gmail.com",
                Contraseña = "1234567890"
            };

            Constantes respuestaActual = servicioComunicacion.CambiarContrasena(cuentaExistente);

            Assert.Equal(respuestaEsperada, respuestaActual);
        }

        //IRegistroUsuario
        [Fact]
        public void ActualizarUsuarioExitosoPrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA;
            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            var nuevaActualizada = new CuentaSet
            {
                CorreoElectronico = "correoPrueba2@gmail.com",
                Avatar = BitConverter.GetBytes(0102030405),
                Nickname = "NicknameDePrueba",
                Contraseña = "ContraseñaDePrueba1234567890"
            };
            var usuarioActualizado = new UsuarioSet
            {
                Nombre = "UsuarioDePrueba",
                Apellido = "ApellidoDePrueba"
            };
            Constantes respuestaActual = servicioComunicacion.ActualizarUsuario(usuarioActualizado, nuevaActualizada);
            Assert.Equal(respuestaActual, respuestaEsperada);
        }
        //IJuegoPuntuacion
        [Fact]
        public void ActualizarGanadorExitosoPrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA;
            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            string uid = "00000000-0000-0000-0000-000000000000";
            string correoElectronico = "correoPrueba3@gmail.com";
            int posicion = 1;
            Constantes respuestaActual = servicioComunicacion.ActualizarGanador(uid, correoElectronico, posicion);
            Assert.Equal(respuestaActual, respuestaEsperada);
        }

    }
}
