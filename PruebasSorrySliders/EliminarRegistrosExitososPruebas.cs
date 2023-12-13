using DatosSorrySliders;
using ServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PruebasSorrySliders
{
    public class ConfiguracionBaseDatosEliminarRegistrosExistosos : IDisposable
    {
        public int IdNotificacion { get; set; }

        public ConfiguracionBaseDatosEliminarRegistrosExistosos()
        {
            try
            {
                using (var context = new BaseDeDatosSorrySlidersEntities())
                {
                    UsuarioSet usuario = new UsuarioSet
                    {
                        Nombre = "nombrePruebaEliminar1",
                        Apellido = "apellidoPruebaEliminar1",
                    };
                    context.UsuarioSet.Add(usuario);

                    UsuarioSet usuario2 = new UsuarioSet
                    {
                        Nombre = "nombrePruebaEliminar2",
                        Apellido = "apellidoPruebaEliminar2",
                    };
                    context.UsuarioSet.Add(usuario2);
                    context.SaveChanges();

                    context.Database.ExecuteSqlCommand("INSERT INTO CuentaSet(CorreoElectronico, Avatar, Contraseña, Nickname, IdUsuario) " +
                        "VALUES('correoPruebaEliminar1@gmail.com', @avatar, HASHBYTES('SHA2_512', N'1234567890'), 'nicknamePrueba', @idUsuario)",
                        new SqlParameter("@avatar", BitConverter.GetBytes(0102030405)), new SqlParameter("@idUsuario", usuario.IdUsuario));

                    context.Database.ExecuteSqlCommand("INSERT INTO CuentaSet(CorreoElectronico, Avatar, Contraseña, Nickname, IdUsuario) " +
                        "VALUES('correoPruebaEliminar2@gmail.com', @avatar, HASHBYTES('SHA2_512', N'1234567890'), 'nicknamePrueba', @idUsuario)",
                        new SqlParameter("@avatar", BitConverter.GetBytes(0102030405)), new SqlParameter("@idUsuario", usuario2.IdUsuario));

                    NotificacionSet notificacionNueva = new NotificacionSet
                    {
                        CorreoElectronicoRemitente = "correoPruebaEliminar1@gmail.com",
                        CorreoElectronicoDestinatario = "correoPruebaEliminar2@gmail.com",
                        IdTipoNotificacion = 1,
                        Mensaje = "",
                    };
                    context.NotificacionSet.Add(notificacionNueva);
                    context.Database.ExecuteSqlCommand("INSERT INTO RelaciónAmigosSet(CorreoElectronicoJugadorPrincipal, CorreoElectronicoJugadorAmigo) values ('correoPruebaEliminar1@gmail.com','correoPruebaEliminar2@gmail.com');");
                    context.Database.ExecuteSqlCommand("INSERT INTO RelaciónAmigosSet(CorreoElectronicoJugadorPrincipal, CorreoElectronicoJugadorAmigo) values ('correoPruebaEliminar2@gmail.com','correoPruebaEliminar1@gmail.com');");
                    context.Database.ExecuteSqlCommand("INSERT INTO RelacionBaneadosSet(CorreoElectronicoJugadorPrincipal, CorreoElectronicoJugadorBaneado) values ('correoPruebaEliminar1@gmail.com','correoPruebaEliminar2@gmail.com');");
                    context.Database.ExecuteSqlCommand("INSERT INTO RelacionBaneadosSet(CorreoElectronicoJugadorPrincipal, CorreoElectronicoJugadorBaneado) values ('correoPruebaEliminar2@gmail.com','correoPruebaEliminar1@gmail.com');");

                    context.SaveChanges();
                    IdNotificacion = notificacionNueva.IdNotificacion;

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
                using (var context = new BaseDeDatosSorrySlidersEntities())
                {
                    context.Database.ExecuteSqlCommand("DELETE FROM RelaciónAmigosSet where CorreoElectronicoJugadorPrincipal='correoPruebaEliminar2@gmail.com' OR CorreoElectronicoJugadorPrincipal='correoPruebaEliminar1@gmail.com';");
                    
                    context.Database.ExecuteSqlCommand("DELETE FROM RelacionBaneadosSet where CorreoElectronicoJugadorPrincipal='correoPruebaEliminar2@gmail.com' OR CorreoElectronicoJugadorPrincipal='correoPruebaEliminar1@gmail.com';");

                    context.Database.ExecuteSqlCommand("DELETE FROM NotificacionSet where CorreoElectronicoRemitente='correoPruebaEliminar2@gmail.com' OR CorreoElectronicoDestinatario='correoPruebaEliminar2@gmail.com';");

                    context.Database.ExecuteSqlCommand("DELETE FROM CuentaSet where CorreoElectronico='correoPruebaEliminar2@gmail.com';");

                    context.Database.ExecuteSqlCommand("DELETE FROM CuentaSet where CorreoElectronico='correoPruebaEliminar1@gmail.com';");

                    context.Database.ExecuteSqlCommand("DELETE FROM UsuarioSet where Nombre='nombrePruebaEliminar1';");

                    context.Database.ExecuteSqlCommand("DELETE FROM UsuarioSet where Nombre='nombrePruebaEliminar2';");

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
    }
    public class EliminarRegistrosExitososPruebas : IClassFixture<ConfiguracionBaseDatosEliminarRegistrosExistosos>
    {
        ConfiguracionBaseDatosEliminarRegistrosExistosos _configuracion;
        public EliminarRegistrosExitososPruebas(ConfiguracionBaseDatosEliminarRegistrosExistosos configuracion) 
        {
            _configuracion = configuracion;
        }

        //IListaAmigos
        [Fact]
        public void VerificarEliminarNotificacionJugadorPrueba() 
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA;
            Constantes respuestaActual;
            string correoElectronicoDestinatario = "correoPruebaEliminar2@gmail.com";
            int idNotificacion = _configuracion.IdNotificacion;
            ServicioComunicacionSorrySliders servicio = new ServicioComunicacionSorrySliders();
            respuestaActual = servicio.EliminarNotificacionJugador(correoElectronicoDestinatario, idNotificacion);
            Assert.Equal(respuestaActual, respuestaEsperada);
        }

        [Fact]
        public void VerificarEliminarAmistadExitosoPrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA;
            Constantes respuestaActual;
            string correoElectronicoPrincipal = "correoPruebaEliminar1@gmail.com";
            string correoElectronicoAmigo = "correoPruebaEliminar2@gmail.com";
            ServicioComunicacionSorrySliders servicio = new ServicioComunicacionSorrySliders();
            respuestaActual = servicio.EliminarAmistad(correoElectronicoPrincipal, correoElectronicoAmigo);
            Assert.Equal(respuestaActual, respuestaEsperada);
        }

        [Fact]
        public void VerificarEliminarBaneoExitosoPrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA;
            Constantes respuestaActual;
            string correoElectronicoPrincipal = "correoPruebaEliminar1@gmail.com";
            string correoElectronicoAmigo = "correoPruebaEliminar2@gmail.com";
            ServicioComunicacionSorrySliders servicio = new ServicioComunicacionSorrySliders();
            respuestaActual = servicio.EliminarBaneo(correoElectronicoPrincipal, correoElectronicoAmigo);
            Assert.Equal(respuestaActual, respuestaEsperada);
        }
    }
}
