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
    public class ConfiguracionBaseDatosCrearRegistrosFallidos : IDisposable
    {
        public ConfiguracionBaseDatosCrearRegistrosFallidos()
        {
            try
            {
                using (var context = new BaseDeDatosSorrySlidersEntities())
                {
                    UsuarioSet usuario = new UsuarioSet { Nombre = "nombrePrueba", Apellido = "apellidoPrueba", };
                    context.UsuarioSet.Add(usuario);

                    UsuarioSet usuarioAmigoUno = new UsuarioSet { Nombre = "nombrePruebaUno", Apellido = "apellidoPruebaUno", };
                    context.UsuarioSet.Add(usuarioAmigoUno);

                    context.SaveChanges();

                    context.Database.ExecuteSqlCommand("INSERT INTO CuentaSet(CorreoElectronico, Avatar, Contraseña, Nickname, IdUsuario) " +
                        "VALUES('correoPrueba@gmail.com', @avatar, HASHBYTES('SHA2_512', N'1234567890'), 'nicknamePrueba', @idUsuario)",
                        new SqlParameter("@avatar", BitConverter.GetBytes(0102030405)), new SqlParameter("@idUsuario", usuario.IdUsuario));

                    context.Database.ExecuteSqlCommand("INSERT INTO CuentaSet(CorreoElectronico, Avatar, Contraseña, Nickname, IdUsuario) " +
                        "VALUES('correoBaneadoPruebaUno@gmail.com', @avatar, HASHBYTES('SHA2_512', N'1234567890'), 'amigoPruebasUno', @idUsuario)",
                        new SqlParameter("@avatar", BitConverter.GetBytes(0102030405)), new SqlParameter("@idUsuario", usuarioAmigoUno.IdUsuario));

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

                    context.RelacionBaneadosSet.Add(new RelacionBaneadosSet
                    {
                        CorreoElectronicoJugadorPrincipal = "correoPrueba@gmail.com",
                        CorreoElectronicoJugadorBaneado = "correoBaneadoPruebaUno@gmail.com"
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
                    contexto.Database.ExecuteSqlCommand("DELETE FROM RelacionBaneadosSet where CorreoElectronicoJugadorPrincipal = 'correoPrueba@gmail.com'");

                    contexto.Database.ExecuteSqlCommand("DELETE FROM RelacionPartidaCuentaSet WHERE CodigoPartida = '00000000-0000-0000-0000-000000000000' " +
                        "OR CorreoElectronico = 'correoPrueba@gmail.com'");

                    contexto.Database.ExecuteSqlCommand("DELETE FROM PartidaSet WHERE CorreoElectronico = 'correoPrueba@gmail.com' OR " +
                        "CodigoPartida = '00000000-0000-0000-0000-000000000000'");

                    contexto.Database.ExecuteSqlCommand("DELETE FROM CuentaSet WHERE CorreoElectronico = 'correoPrueba@gmail.com' " +
                        "OR CorreoElectronico = 'correoBaneadoPruebaUno@gmail.com' ");

                    contexto.Database.ExecuteSqlCommand("DELETE FROM UsuarioSet WHERE Nombre = 'nombrePrueba' AND Apellido = 'apellidoPrueba'");

                    contexto.Database.ExecuteSqlCommand("DELETE FROM UsuarioSet WHERE Nombre = 'nombrePruebaUno' AND Apellido = 'apellidoPruebaUno'");


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
    public class CreacionRegistrosErrorPruebas : IClassFixture<ConfiguracionBaseDatosCrearRegistrosFallidos>
    {
        ConfiguracionBaseDatosCrearRegistrosFallidos _configuracion;

        public CreacionRegistrosErrorPruebas(ConfiguracionBaseDatosCrearRegistrosFallidos configuracion)
        {
            _configuracion = configuracion;
        }

        /*[Fact]
        public void VerificarCrearPartidaSinExitoPrueba()
        {
            Constantes respuestaEsperado = Constantes.ERROR_CONSULTA;
            string codigoEsperado = null;
            int cantidadJugadores = 4;
            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            CuentaSet cuenta = new CuentaSet();
            string codigo;
            Constantes resultadoObtenido;
            (resultadoObtenido, codigo) = servicioComunicacion.CrearPartida(cuenta.CorreoElectronico, cantidadJugadores);
            Assert.Equal(respuestaEsperado, resultadoObtenido);
            Assert.Equal(codigoEsperado, codigo);

        }*/
        [Fact]
        public void VerificarJugadorBaneadoInsertarPartidaPrueba()
        {
            Constantes respuestaEsperado = Constantes.OPERACION_EXITOSA_VACIA;
            int jugadoresMaximos = -2;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            string correoExistente = "correoBaneadoPruebaUno@gmail.com";
            string uidExistente = "00000000-0000-0000-0000-000000000000";

            (Constantes resultadoObtenidos, int cantidadJugadoresPrevios) = servicioComunicacion.UnirseAlLobby(uidExistente, correoExistente);

            Assert.Equal(respuestaEsperado, resultadoObtenidos);
            Assert.Equal(jugadoresMaximos, cantidadJugadoresPrevios);
        }
        [Fact]
        public void VerificarPartidaNoExistenteIntentarEntrarPrueba()
        {
            Constantes respuestaEsperado = Constantes.OPERACION_EXITOSA_VACIA;
            int jugadoresMaximos = 0;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            string correoExistente = "correo@gmail.com";
            string uidExistente = "00000000-0000-0000-0000-000000000002";

            (Constantes resultadoObtenidos, int cantidadJugadoresPrevios) = servicioComunicacion.UnirseAlLobby(uidExistente, correoExistente);

            Assert.Equal(respuestaEsperado, resultadoObtenidos);
            Assert.Equal(jugadoresMaximos, cantidadJugadoresPrevios);
        }

        [Fact]
        public void VerificarIntentarEntrarMismaCuentaPrueba()
        {
            Constantes respuestaEsperado = Constantes.OPERACION_EXITOSA_VACIA;
            int jugadoresMaximos = -1;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            string correoExistente = "correoPrueba@gmail.com";
            string uidExistente = "00000000-0000-0000-0000-000000000000";

            (Constantes resultadoObtenidos, int cantidadJugadoresPrevios) = servicioComunicacion.UnirseAlLobby(uidExistente, correoExistente);

            Assert.Equal(respuestaEsperado, resultadoObtenidos);
            Assert.Equal(jugadoresMaximos, cantidadJugadoresPrevios);
        }
    }
}
