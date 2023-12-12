using DatosSorrySliders;
using PruebasSorrySliders.ServidorComunicacionSorrySlidersPrueba;
using ServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PruebasSorrySliders
{
    //Para realizar estas pruebas se requiere que el servidor y la base de datos estén funcionando
    public class ConfiguracionBaseDatosCrearRegistrosFallidos : IDisposable
    {
        private static LobbyClient _proxyJuegoLanzamiento;
        private static LobbyClient _proxyNuevoJuegoLanzamiento;
        private static LobbyClient _proxyNuevoJuegoLanzamientoLleno;
        public ConfiguracionBaseDatosCrearRegistrosFallidos()
        {
            try
            {
                ImplementacionLobbyPruebas implementacionCallbackLobby = new ImplementacionLobbyPruebas();
                _proxyJuegoLanzamiento = new LobbyClient(new InstanceContext(implementacionCallbackLobby));
                _proxyJuegoLanzamiento.EntrarPartida("00000000-0000-0000-0000-000000000000", "correoPrueba@gmail.com");

                ImplementacionLobbyPruebas implementacionCallbackLobbyNueva = new ImplementacionLobbyPruebas();
                _proxyNuevoJuegoLanzamiento = new LobbyClient(new InstanceContext(implementacionCallbackLobbyNueva));
                _proxyNuevoJuegoLanzamiento.EntrarPartida("00000000-0000-0000-0000-000000000001", "correoPrueba@gmail.com");

                ImplementacionLobbyPruebas implementacionCallbackLobbyLleno = new ImplementacionLobbyPruebas();
                _proxyNuevoJuegoLanzamientoLleno = new LobbyClient(new InstanceContext(implementacionCallbackLobbyLleno));
                _proxyNuevoJuegoLanzamientoLleno.EntrarPartida("00000000-0000-0000-0000-000000000002", "correoPrueba@gmail.com");

                using (var context = new BaseDeDatosSorrySlidersEntities())
                {
                    UsuarioSet usuario = new UsuarioSet { Nombre = "nombrePrueba", Apellido = "apellidoPrueba", };
                    context.UsuarioSet.Add(usuario);

                    UsuarioSet usuarioAmigoUno = new UsuarioSet { Nombre = "nombrePruebaUno", Apellido = "apellidoPruebaUno", };
                    context.UsuarioSet.Add(usuarioAmigoUno);

                    UsuarioSet usuarioAmigoDos = new UsuarioSet { Nombre = "nombrePruebaUno", Apellido = "apellidoPruebaUno", };
                    context.UsuarioSet.Add(usuarioAmigoDos);

                    context.SaveChanges();

                    context.Database.ExecuteSqlCommand("INSERT INTO CuentaSet(CorreoElectronico, Avatar, Contraseña, Nickname, IdUsuario) " +
                        "VALUES('correoPrueba@gmail.com', @avatar, HASHBYTES('SHA2_512', N'1234567890'), 'nicknamePrueba', @idUsuario)",
                        new SqlParameter("@avatar", BitConverter.GetBytes(0102030405)), new SqlParameter("@idUsuario", usuario.IdUsuario));

                    context.Database.ExecuteSqlCommand("INSERT INTO CuentaSet(CorreoElectronico, Avatar, Contraseña, Nickname, IdUsuario) " +
                        "VALUES('correoBaneadoPruebaUno@gmail.com', @avatar, HASHBYTES('SHA2_512', N'1234567890'), 'amigoPruebasUno', @idUsuario)",
                        new SqlParameter("@avatar", BitConverter.GetBytes(0102030405)), new SqlParameter("@idUsuario", usuarioAmigoUno.IdUsuario));

                    context.Database.ExecuteSqlCommand("INSERT INTO CuentaSet(CorreoElectronico, Avatar, Contraseña, Nickname, IdUsuario) " +
                        "VALUES('correoDos@gmail.com', @avatar, HASHBYTES('SHA2_512', N'1234567890'), 'amigoPruebasUno', @idUsuario)",
                        new SqlParameter("@avatar", BitConverter.GetBytes(0102030405)), new SqlParameter("@idUsuario", usuarioAmigoDos.IdUsuario));

                    context.PartidaSet.Add(new PartidaSet
                    {
                        CodigoPartida = new Guid("00000000-0000-0000-0000-000000000000"),
                        CantidadJugadores = 4,
                        CorreoElectronico = "correoPrueba@gmail.com"
                    });

                    context.PartidaSet.Add(new PartidaSet
                    {
                        CodigoPartida = new Guid("00000000-0000-0000-0000-000000000002"),
                        CantidadJugadores = 2,
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
                        CodigoPartida = new Guid("00000000-0000-0000-0000-000000000002"),
                        CorreoElectronico = "correoPrueba@gmail.com"
                    });

                    context.RelacionPartidaCuentaSet.Add(new RelacionPartidaCuentaSet
                    {
                        Posicion = 0,
                        CodigoPartida = new Guid("00000000-0000-0000-0000-000000000002"),
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
                _proxyJuegoLanzamiento.SalirPartida("00000000-0000-0000-0000-000000000000");
                _proxyNuevoJuegoLanzamiento.SalirPartida("00000000-0000-0000-0000-000000000001");
                _proxyNuevoJuegoLanzamientoLleno.SalirPartida("00000000-0000-0000-0000-000000000002");
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    contexto.Database.ExecuteSqlCommand("DELETE FROM RelacionBaneadosSet where CorreoElectronicoJugadorPrincipal = 'correoPrueba@gmail.com'");

                    contexto.Database.ExecuteSqlCommand("DELETE FROM RelacionPartidaCuentaSet WHERE CodigoPartida = '00000000-0000-0000-0000-000000000000' " +
                        "OR CorreoElectronico = 'correoPrueba@gmail.com' OR CodigoPartida = '00000000-0000-0000-0000-000000000002'");

                    contexto.Database.ExecuteSqlCommand("DELETE FROM PartidaSet WHERE CorreoElectronico = 'correoPrueba@gmail.com' OR " +
                        "CodigoPartida = '00000000-0000-0000-0000-000000000000'");

                    contexto.Database.ExecuteSqlCommand("DELETE FROM CuentaSet WHERE CorreoElectronico = 'correoPrueba@gmail.com' " +
                        "OR CorreoElectronico = 'correoBaneadoPruebaUno@gmail.com' OR CorreoElectronico ='correoDos@gmail.com' ");

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
    public class CreacionRegistrosErrorExitosoServidorPruebas : IClassFixture<ConfiguracionBaseDatosCrearRegistrosFallidos>
    {
        ConfiguracionBaseDatosCrearRegistrosFallidos _configuracion;

        public CreacionRegistrosErrorExitosoServidorPruebas(ConfiguracionBaseDatosCrearRegistrosFallidos configuracion)
        {
            _configuracion = configuracion;
        }

        [Fact]
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

        }
        [Fact]
        public void VerificarEntrarPartidaInexistente()
        {
            Constantes respuestaEsperado = Constantes.OPERACION_EXITOSA_VACIA;
            int numeroError = -3;
            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            string correoExistente = "correo@gmail.com";
            string uidExistente = "00000000-0000-0000-0000-000000000000";

            Constantes resultadoObtenido;
            int cantidadJugadores;
            (resultadoObtenido, cantidadJugadores) = servicioComunicacion.UnirseAlLobby(uidExistente, correoExistente);
            Assert.Equal(respuestaEsperado, resultadoObtenido);
            Assert.Equal(numeroError, cantidadJugadores);

        }
        [Fact]
        public void VerificarJugadorBaneadoInsertarPartidaPrueba()
        {
            Constantes respuestaEsperado = Constantes.OPERACION_EXITOSA_VACIA;
            int jugadoresMaximos = -2;

            UnirsePartidaClient proxyClienteUnirse = new UnirsePartidaClient();

            string correoExistente = "correoBaneadoPruebaUno@gmail.com";
            string uidExistente = "00000000-0000-0000-0000-000000000000";

            (Constantes resultadoObtenidos, int cantidadJugadoresPrevios) = proxyClienteUnirse.UnirseAlLobby(uidExistente, correoExistente);

            Assert.Equal(respuestaEsperado, resultadoObtenidos);
            Assert.Equal(jugadoresMaximos, cantidadJugadoresPrevios);
        }
        [Fact]
        public void VerificarPartidaNoExistenteIntentarEntrarPrueba()
        {
            Constantes respuestaEsperado = Constantes.OPERACION_EXITOSA_VACIA;
            int jugadoresMaximos = 0;

            UnirsePartidaClient proxyClienteUnirse = new UnirsePartidaClient();

            string correoExistente = "correo@gmail.com";
            string uidExistente = "00000000-0000-0000-0000-000000000001";

            (Constantes resultadoObtenidos, int cantidadJugadoresPrevios) = proxyClienteUnirse.UnirseAlLobby(uidExistente, correoExistente);

            Assert.Equal(respuestaEsperado, resultadoObtenidos);
            Assert.Equal(jugadoresMaximos, cantidadJugadoresPrevios);
        }

        [Fact]
        public void VerificarIntentarEntrarMismaCuentaPrueba()
        {
            Constantes respuestaEsperado = Constantes.OPERACION_EXITOSA_VACIA;
            int jugadoresMaximos = -1;

            UnirsePartidaClient proxyClienteUnirse = new UnirsePartidaClient();

            string correoExistente = "correoPrueba@gmail.com";
            string uidExistente = "00000000-0000-0000-0000-000000000000";

            (Constantes resultadoObtenidos, int cantidadJugadoresPrevios) = proxyClienteUnirse.UnirseAlLobby(uidExistente, correoExistente);

            Assert.Equal(respuestaEsperado, resultadoObtenidos);
            Assert.Equal(jugadoresMaximos, cantidadJugadoresPrevios);
        }
        [Fact]
        public void VerificarIntentarEntrarLobbyLlenoPrueba()
        {
            Constantes respuestaEsperado = Constantes.OPERACION_EXITOSA_VACIA;
            int jugadoresMaximos = 2;

            UnirsePartidaClient proxyClienteUnirse = new UnirsePartidaClient();

            string correoExistente = "correoPrueba1@gmail.com";
            string uidExistente = "00000000-0000-0000-0000-000000000002";

            (Constantes resultadoObtenidos, int cantidadJugadoresPrevios) = proxyClienteUnirse.UnirseAlLobby(uidExistente, correoExistente);

            Assert.Equal(respuestaEsperado, resultadoObtenidos);
            Assert.Equal(jugadoresMaximos, cantidadJugadoresPrevios);
        }
        [Fact]
        public void VerificarInsertarJugadorPartidaExitosamentePrueba()
        {
            Constantes respuestaEsperado = Constantes.OPERACION_EXITOSA;
            int jugadoresMaximos = 4;

            UnirsePartidaClient proxyClienteUnirse = new UnirsePartidaClient();

            string correoExistente = "correoDos@gmail.com";
            string uidExistente = "00000000-0000-0000-0000-000000000000";

            (Constantes resultadoObtenidos, int cantidadJugadoresPrevios) = proxyClienteUnirse.UnirseAlLobby(uidExistente, correoExistente);

            Assert.Equal(respuestaEsperado, resultadoObtenidos);
            Assert.Equal(jugadoresMaximos, cantidadJugadoresPrevios);
        }
    }

    public class ImplementacionLobbyPruebas : ServidorComunicacionSorrySlidersPrueba.ILobbyCallback
    {
        public void HostInicioPartida()
        {
            Logger log = new Logger(this.GetType());
            log.LogInfo("Host Inicio Partida");
        }

        public void JugadorEntroPartida()
        {
            Logger log = new Logger(this.GetType());
            log.LogInfo("Jugador entró Partida");
        }

        public void JugadorSalioPartida()
        {
            Logger log = new Logger(this.GetType());
            log.LogInfo("Jugador salió Partida");
        }
    }
}
