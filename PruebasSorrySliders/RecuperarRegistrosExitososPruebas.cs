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
    public class ConfiguracionBaseDatosRecuperarRegistrosExistosos: IDisposable
    {
        public ConfiguracionBaseDatosRecuperarRegistrosExistosos()
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

                    context.RelaciónAmigosSet.Add(
                        new RelaciónAmigosSet { CorreoElectronicoJugadorPrincipal = "correoPrueba@gmail.com", CorreoElectronicoJugadorAmigo = "correoAmigoPruebaUno@gmail.com" });

                    context.RelaciónAmigosSet.Add(
                        new RelaciónAmigosSet { CorreoElectronicoJugadorPrincipal = "correoPrueba@gmail.com", CorreoElectronicoJugadorAmigo = "correoAmigoPruebaDos@gmail.com" });

                    context.PartidaSet.Add(new PartidaSet { 
                        CodigoPartida = new Guid("00000000-0000-0000-0000-000000000000"), CantidadJugadores = 4, 
                        CorreoElectronico = "correoPrueba@gmail.com"
                    });

                    context.RelacionPartidaCuentaSet.Add(new RelacionPartidaCuentaSet {
                        Posicion = 0, CodigoPartida = new Guid("00000000-0000-0000-0000-000000000000"),
                        CorreoElectronico = "correoPrueba@gmail.com"
                    });

                    context.RelacionPartidaCuentaSet.Add(new RelacionPartidaCuentaSet { 
                        Posicion = 0, CodigoPartida = new Guid("00000000-0000-0000-0000-000000000000"),
                        CorreoElectronico = "correoAmigoPruebaUno@gmail.com"
                    });

                    context.RelacionPartidaCuentaSet.Add(new RelacionPartidaCuentaSet
                    {
                        Posicion = 0,
                        CodigoPartida = new Guid("00000000-0000-0000-0000-000000000000"),
                        CorreoElectronico = "correoAmigoPruebaDos@gmail.com"
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
                    contexto.Database.ExecuteSqlCommand("DELETE FROM RelacionPartidaCuentaSet WHERE CodigoPartida = '00000000-0000-0000-0000-000000000000'");

                    contexto.Database.ExecuteSqlCommand("DELETE FROM PartidaSet WHERE CodigoPartida = '00000000-0000-0000-0000-000000000000'");

                    contexto.Database.ExecuteSqlCommand("DELETE FROM RelaciónAmigosSet WHERE CorreoElectronicoJugadorPrincipal = 'correoPrueba@gmail.com'");

                    contexto.Database.ExecuteSqlCommand("DELETE FROM CuentaSet WHERE CorreoElectronico = 'correoPrueba@gmail.com' " +
                        "OR CorreoElectronico = 'correoAmigoPruebaUno@gmail.com' OR CorreoElectronico = 'correoAmigoPruebaDos@gmail.com'");

                    contexto.Database.ExecuteSqlCommand("DELETE FROM UsuarioSet WHERE Nombre = 'nombrePrueba' AND Apellido = 'apellidoPrueba'");

                    contexto.Database.ExecuteSqlCommand("DELETE FROM UsuarioSet WHERE Nombre = 'nombrePruebaUno' AND Apellido = 'apellidoPruebaUno'");

                    contexto.Database.ExecuteSqlCommand("DELETE FROM UsuarioSet WHERE Nombre = 'nombrePruebaDos' AND Apellido = 'apellidoPruebaDos'");
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
    public class RecuperarRegistrosExitososPruebas: IClassFixture<ConfiguracionBaseDatosRecuperarRegistrosExistosos>
    {
        ConfiguracionBaseDatosRecuperarRegistrosExistosos _configuracion;
        public RecuperarRegistrosExitososPruebas(ConfiguracionBaseDatosRecuperarRegistrosExistosos configuracion)
        {
            _configuracion = configuracion;
        }
        //Pruebas Interfaces IInicioSesion
        [Fact]
        public void VerificarExistenciaCorreoCorrectoPrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            string correoCorrectoExistente = "correoPrueba@gmail.com";

            Constantes respuestaActual = servicioComunicacion.VerificarExistenciaCorreoCuenta(correoCorrectoExistente);

            Assert.Equal(respuestaEsperada, respuestaActual);
        }

        [Fact]
        public void VerificarContrasenaCuentaExitosamentePrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            CuentaSet cuentaExistente = new CuentaSet { CorreoElectronico = "correoPrueba@gmail.com", Contraseña = "1234567890" };

            Constantes respuestaActual = servicioComunicacion.VerificarContrasenaDeCuenta(cuentaExistente);

            Assert.Equal(respuestaEsperada, respuestaActual);

        }
        //Pruebas Interfaces IDetallesCuentaUsuario
        [Fact]
        public void VerificarRecuperarDetallesUsuarioExistentePrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA;
            UsuarioSet usuarioEsperado = new UsuarioSet
            {
                Nombre = "nombrePrueba",
                Apellido = "apellidoPrueba"
            };

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            string correoCorrectoExistente = "correoPrueba@gmail.com";

            (Constantes respuestaActual, UsuarioSet usuarioActual) = servicioComunicacion.RecuperarDatosUsuarioDeCuenta(correoCorrectoExistente);

            Assert.Equal(respuestaEsperada, respuestaActual);
            Assert.Equal(usuarioEsperado, usuarioActual);
        }

        [Fact]
        public void VerificarExistenciaContrasenaExistentePrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            CuentaSet cuentaExistente = new CuentaSet
            {
                CorreoElectronico = "correoPrueba@gmail.com",
                Contraseña = "1234567890"
            };

            Constantes respuestaActual = servicioComunicacion.VerificarContrasenaActual(cuentaExistente);

            Assert.Equal(respuestaEsperada, respuestaActual);
        }
        //Pruebas Interfaces IListaAmigos
        [Fact]
        public void VerificarRecuperarAmigosCuentaExitosamentePrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA;
            string correoElectronicoPrueba = "correoPrueba@gmail.com";

            List<CuentaSet> amigosEsperados = new List<CuentaSet>
            {
                new CuentaSet{ Nickname = "amigoPruebasUno", CorreoElectronico = "correoAmigoPruebaUno@gmail.com"},
                new CuentaSet{ Nickname = "amigoPruebasDos", CorreoElectronico = "correoAmigoPruebaDos@gmail.com"}
            };

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            
            (Constantes respuestaActual, List<CuentaSet> amigosActuales) = servicioComunicacion.RecuperarAmigosCuenta(correoElectronicoPrueba);

            Assert.Equal(respuestaEsperada, respuestaActual);
            Assert.Equal(amigosEsperados, amigosActuales);
        }

        [Fact]
        public void VerificarRecuperarCuentasExitosamentePrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA;
            string informacionJugador = "amigo";
            string correoBuscar = "correoPruebas@gmail.com";

            List<CuentaSet> cuentasEsperadas = new List<CuentaSet>
            {
                new CuentaSet{ Nickname = "amigoPruebasUno", CorreoElectronico = "correoAmigoPruebaUno@gmail.com"},
                new CuentaSet{ Nickname = "amigoPruebasDos", CorreoElectronico = "correoAmigoPruebaDos@gmail.com"}
            };

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            (Constantes respuestaActual, List<CuentaSet> cuentasActuales) = servicioComunicacion.RecuperarJugadoresCuenta(informacionJugador, correoBuscar);

            Assert.Equal(respuestaEsperada, respuestaActual);
            Assert.Equal(cuentasEsperadas, cuentasActuales);
        }
        //Pruebas Interfaces IUnirsePartida
        [Fact]
        public void VerificarRecuperarJugadoresLobbyExitosamentePrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA;
            string uidLobby = "00000000-0000-0000-0000-000000000000";

            List<CuentaSet> cuentasEsperadas = new List<CuentaSet>
            {
                new CuentaSet{ Nickname = "nicknamePrueba", CorreoElectronico = "correoPrueba@gmail.com"},
                new CuentaSet{ Nickname = "amigoPruebasUno", CorreoElectronico = "correoAmigoPruebaUno@gmail.com"},
                new CuentaSet{ Nickname = "amigoPruebasDos", CorreoElectronico = "correoAmigoPruebaDos@gmail.com"},
            };

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            (Constantes respuestaActual, List<CuentaSet> cuentasLobbyActuales) = servicioComunicacion.RecuperarJugadoresLobby(uidLobby);

            Assert.Equal(respuestaEsperada, respuestaActual);
            Assert.Equal(cuentasEsperadas, cuentasLobbyActuales);
        }

        [Fact]
        public void VerificarRecuperarPartidaExitosamentePrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA;
            string uidLobby = "00000000-0000-0000-0000-000000000000";

            PartidaSet partidaEsperada = new PartidaSet
            {
                CantidadJugadores = 4,
                CodigoPartida = new Guid("00000000-0000-0000-0000-000000000000"),
                CorreoElectronico = "correoPrueba@gmail.com"
            };

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            (Constantes respuestaActual, PartidaSet partidaRecuperada) = servicioComunicacion.RecuperarPartida(uidLobby);

            Assert.Equal(respuestaEsperada, respuestaActual);
            Assert.Equal(partidaEsperada, partidaRecuperada);
        }
        //IMenuPrincipal
        [Fact]
        public void VerificarRecuperarDatosUsuarioExitosamentePrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA;
            string correoElectronicoExistente = "correoPrueba@gmail.com";
            string nicknameEsperado = "nicknamePrueba";
            byte[] avatarEsperado = BitConverter.GetBytes(0102030405);

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            (Constantes respuestaActual, string nickname, byte[] avatar) = servicioComunicacion.RecuperarDatosUsuario(correoElectronicoExistente);

            Assert.Equal(respuestaEsperada, respuestaActual);
            Assert.Equal(nicknameEsperado, nickname);
            Assert.Equal(avatarEsperado, avatar);
        }
    }
}
