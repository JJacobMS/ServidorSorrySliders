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
        public ConfiguracionBaseDatosCrearRegistrosExistosos() 
        {
            try
            {
                using (var context = new BaseDeDatosSorrySlidersEntities())
                {
                    UsuarioSet usuario = new UsuarioSet { Nombre = "nombrePrueba", Apellido = "apellidoPrueba", };
                    context.UsuarioSet.Add(usuario);

                    UsuarioSet usuarioAmigoUno = new UsuarioSet { Nombre = "nombrePruebaUno", Apellido = "apellidoPruebaUno", };
                    context.UsuarioSet.Add(usuarioAmigoUno);

                    UsuarioSet usuarioAmigoDos = new UsuarioSet { Nombre = "nombrePruebaDos", Apellido = "apellidoPruebaDos", };
                    context.UsuarioSet.Add(usuarioAmigoDos);
                    context.SaveChanges();

                    context.Database.ExecuteSqlCommand("INSERT INTO CuentaSet(CorreoElectronico, Avatar, Contraseña, Nickname, IdUsuario) " +
                        "VALUES('correoPrueba@gmail.com', @avatar, HASHBYTES('SHA2_512', N'1234567890'), 'nicknamePrueba', @idUsuario)",
                        new SqlParameter("@avatar", BitConverter.GetBytes(0102030405)), new SqlParameter("@idUsuario", usuario.IdUsuario));

                    context.Database.ExecuteSqlCommand("INSERT INTO CuentaSet(CorreoElectronico, Avatar, Contraseña, Nickname, IdUsuario) " +
                        "VALUES('correoAmigoPruebaUno@gmail.com', @avatar, HASHBYTES('SHA2_512', N'1234567890'), 'amigoPruebasUno', @idUsuario)",
                        new SqlParameter("@avatar", BitConverter.GetBytes(0102030405)), new SqlParameter("@idUsuario", usuarioAmigoUno.IdUsuario));

                    context.Database.ExecuteSqlCommand("INSERT INTO CuentaSet(CorreoElectronico, Avatar, Contraseña, Nickname, IdUsuario) " +
                        "VALUES('correoAmigoPruebaDos@gmail.com', @avatar, HASHBYTES('SHA2_512', N'1234567890'), 'amigoPruebasDos', @idUsuario)",
                        new SqlParameter("@avatar", BitConverter.GetBytes(0102030405)), new SqlParameter("@idUsuario", usuarioAmigoDos.IdUsuario));

                    context.PartidaSet.Add(new PartidaSet
                    {
                        CodigoPartida = new Guid("00000000-0000-0000-0000-000000000000"),
                        CantidadJugadores = 4,
                        CorreoElectronico = "correoPrueba@gmail.com"
                    });

                    context.RelacionPartidaCuentaSet.Add(new RelacionPartidaCuentaSet
                    {
                        Posicion = 0,
                        CodigoPartida = new Guid("00000000-0000-0000-0000-000000000000"),
                        CorreoElectronico = "correoPrueba@gmail.com"
                    });

                    context.RelacionPartidaCuentaSet.Add(new RelacionPartidaCuentaSet
                    {
                        Posicion = 0,
                        CodigoPartida = new Guid("00000000-0000-0000-0000-000000000000"),
                        CorreoElectronico = "correoAmigoPruebaUno@gmail.com"
                    });

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

                    contexto.Database.ExecuteSqlCommand("DELETE FROM RelacionPartidaCuentaSet WHERE CodigoPartida = '00000000-0000-0000-0000-000000000000' " +
                        "OR CorreoElectronico = 'correoPrueba@gmail.com'");

                    contexto.Database.ExecuteSqlCommand("DELETE FROM PartidaSet WHERE CorreoElectronico = 'correoPrueba@gmail.com' OR " +
                        "CodigoPartida = '00000000-0000-0000-0000-000000000000'");

                    contexto.Database.ExecuteSqlCommand("DELETE FROM CuentaSet WHERE CorreoElectronico = 'correoPrueba@gmail.com' OR CorreoElectronico = 'correoParaPruebas@gmail.com'" +
                        "OR CorreoElectronico = 'correoAmigoPruebaUno@gmail.com' OR CorreoElectronico = 'correoAmigoPruebaDos@gmail.com' OR " +
                        "CorreoElectronico = '00000000-0000-0000-0000-000000000001'");

                    contexto.Database.ExecuteSqlCommand("DELETE FROM UsuarioSet WHERE Nombre = 'nombrePrueba' AND Apellido = 'apellidoPrueba'");

                    contexto.Database.ExecuteSqlCommand("DELETE FROM UsuarioSet WHERE Nombre = 'nombrePruebaUno' AND Apellido = 'apellidoPruebaUno'");

                    contexto.Database.ExecuteSqlCommand("DELETE FROM UsuarioSet WHERE Nombre = 'nombrePruebaDos' AND Apellido = 'apellidoPruebaDos'");

                    contexto.Database.ExecuteSqlCommand("DELETE FROM CuentaSet WHERE CorreoElectronico = @correo",
                        new SqlParameter("@correo", "correoParaPruebas@gmail.com"));

                    contexto.Database.ExecuteSqlCommand("DELETE FROM UsuarioSet WHERE Nombre = @nombre AND Apellido = @apellido",
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

        //IRegistroUsuario

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

        //IUnirsePartida
        [Fact]
        public void VerificarInsertarCuentaProvisionalInvitadoPrueba()
        {
            Constantes respuestaEsperado = Constantes.OPERACION_EXITOSA;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            CuentaSet cuentaProvisional = new CuentaSet
            {
                CorreoElectronico = "00000000-0000-0000-0000-000000000001",
                Avatar = BitConverter.GetBytes(0102030405),
                Nickname = "hola"
            };

            Constantes resultadoObtenidos = servicioComunicacion.CrearCuentaProvisionalInvitado(cuentaProvisional);

            Assert.Equal(respuestaEsperado, resultadoObtenidos);
        }

        //ICrearLobby
        [Fact]
        public void VerificarCrearPartidaExitosamentePrueba()
        {
            Constantes respuestaEsperado = Constantes.OPERACION_EXITOSA;
            Constantes resultadoObtenido;
            string codigo;
            int cantidadJugadores = 4;
            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            CuentaSet cuenta = new CuentaSet
            {
                CorreoElectronico = "correoPrueba@gmail.com"
            };
            (resultadoObtenido, codigo) = servicioComunicacion.CrearPartida(cuenta.CorreoElectronico, cantidadJugadores);
            Assert.Equal(respuestaEsperado, resultadoObtenido);

        }
    }
}
