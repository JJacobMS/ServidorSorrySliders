using DatosSorrySliders;
using InterfacesServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServidorSorrySliders
{
    public partial class ServicioComunicacionSorrySliders : IListaAmigos, INotificarJugadores
    {
        private Dictionary<string, OperationContext> _jugadoresEnLineaListaAmigos = new Dictionary<string, OperationContext>();
        public (Constantes, List<CuentaSet>) RecuperarAmigosCuenta(string correoElectronico)
        {
            Logger log = new Logger(this.GetType(), "IListaAmigos");
            try
            {
                List<CuentaSet> amigosJugador = new List<CuentaSet>();
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    var listaAmigos = contexto.Database.SqlQuery<CuentaSet>
                        ("Select Nickname, CorreoElectronico, Avatar, Contraseña, IdUsuario from RelaciónAmigosSet " +
                        "Inner Join CuentaSet On CorreoElectronico = CorreoElectronicoJugadorAmigo Where CorreoElectronicoJugadorPrincipal = @correo",
                        new SqlParameter("@correo", correoElectronico)).ToList();

                    if (listaAmigos == null || listaAmigos.Count <= 0)
                    {
                        return (Constantes.OPERACION_EXITOSA_VACIA, null);
                    }

                    for (int i = 0; i < listaAmigos.Count; i++) 
                    {
                        CuentaSet amigo = new CuentaSet
                        {
                            Nickname = listaAmigos[i].Nickname, CorreoElectronico = listaAmigos[i].CorreoElectronico
                        };
                        amigosJugador.Add(amigo);
                    }

                    return (Constantes.OPERACION_EXITOSA, amigosJugador);

                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.StackTrace);
                log.LogError("Error al ejecutar consulta SQL", ex);
                return (Constantes.ERROR_CONSULTA, null);
            }
            catch (EntityException ex)
            {
                Console.WriteLine(ex.StackTrace);
                log.LogError("Error de conexión a la base de datos", ex);
                return (Constantes.ERROR_CONEXION_BD, null);
            }
        }

        public (Constantes, List<CuentaSet>) RecuperarJugadoresCuenta(string informacionJugador)
        {
            Logger log = new Logger(this.GetType(), "IListaAmigos");
            try
            {
                List<CuentaSet> jugadoresEncontrados = new List<CuentaSet>();
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    var jugadores = contexto.Database.SqlQuery<CuentaSet>
                        ("Select * From CuentaSet Where Nickname Like '%' + @nickname + '%' Or CorreoElectronico Like '%' + @correo + '%' " +
                        "AND CorreoElectronico Like '%@%' ORDER BY CorreoElectronico DESC",
                        new SqlParameter("@nickname", informacionJugador), new SqlParameter("@correo", informacionJugador)).ToList();

                    if (jugadores == null || jugadores.Count <= 0)
                    {
                        return (Constantes.OPERACION_EXITOSA_VACIA, null);
                    }

                    for (int i = 0; i < jugadores.Count; i++)
                    {
                        CuentaSet jugador = new CuentaSet
                        {
                            Nickname = jugadores[i].Nickname,
                            CorreoElectronico = jugadores[i].CorreoElectronico
                        };
                        jugadoresEncontrados.Add(jugador);
                    }

                    return (Constantes.OPERACION_EXITOSA, jugadoresEncontrados);

                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.StackTrace);
                log.LogError("Error al ejecutar consulta SQL", ex);
                return (Constantes.ERROR_CONSULTA, null);
            }
            catch (EntityException ex)
            {
                Console.WriteLine(ex.StackTrace);
                log.LogError("Error de conexión a la base de datos", ex);
                return (Constantes.ERROR_CONEXION_BD, null);
            }
        }

        public (Constantes, List<TipoNotificacion>) RecuperarTipoNotificacion()
        {
            Logger log = new Logger(this.GetType(), "IListaAmigos");
            try
            {
                List<TipoNotificacion> tiposNotificacion = new List<TipoNotificacion>();
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    var notificacionesRecuperadas = contexto.Database.SqlQuery<TipoNotificacion>
                        ("SELECT IdTipoNotificacion, NombreNotificacion from TipoNotificacion").ToList();

                    if (notificacionesRecuperadas == null || notificacionesRecuperadas.Count() <= 0)
                    {
                        return (Constantes.OPERACION_EXITOSA_VACIA, null);
                    }

                    for (int i = 0; i < notificacionesRecuperadas.Count(); i++)
                    {
                        TipoNotificacion notificacion = new TipoNotificacion
                        {
                            IdTipoNotificacion = notificacionesRecuperadas[i].IdTipoNotificacion,
                            NombreNotificacion = notificacionesRecuperadas[i].NombreNotificacion
                        };
                        tiposNotificacion.Add(notificacion);
                    }

                    return (Constantes.OPERACION_EXITOSA, tiposNotificacion);

                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.StackTrace);
                log.LogError("Error al ejecutar consulta SQL", ex);
                return (Constantes.ERROR_CONSULTA, null);
            }
            catch (EntityException ex)
            {
                Console.WriteLine(ex.StackTrace);
                log.LogError("Error de conexión a la base de datos", ex);
                return (Constantes.ERROR_CONEXION_BD, null);
            }
        }

        public Constantes GuardarNotificacion(NotificacionSet notificacion)
        {
            Logger log = new Logger(this.GetType(), "IListaAmigos");
            try
            {
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    NotificacionSet notificacionNueva = new NotificacionSet 
                    {
                        CorreoElectronicoRemitente = notificacion.CorreoElectronicoRemitente,
                        CorreoElectronicoDestinatario = notificacion.CorreoElectronicoDestinatario,
                        IdTipoNotificacion = notificacion.IdTipoNotificacion,
                        Mensaje = notificacion.Mensaje
                    };
                    contexto.NotificacionSet.Add(notificacionNueva);
                    contexto.SaveChanges();
                    return Constantes.OPERACION_EXITOSA;
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString());
                log.LogError("Error al ejecutar consulta SQL", ex);
                return (Constantes.ERROR_CONSULTA);
            }
            catch (EntityException ex)
            {
                Console.WriteLine(ex.ToString());
                log.LogError("Error de conexión a la base de datos", ex);
                return (Constantes.ERROR_CONEXION_BD);
            }
        }

        public void NotificarJugadorInvitado(string correoElectronico)
        {
            Logger log = new Logger(this.GetType(), "INotificarJugadores");

            try
            {
                _jugadoresEnLineaListaAmigos[correoElectronico].GetCallbackChannel<ICallbackNotificarJugadores>().RecuperarNotificacion();

            }
            catch (CommunicationObjectAbortedException ex)
            {
                Console.WriteLine("Ha ocurrido un error en el callback \n" + ex.StackTrace);
                log.LogWarn("La conexión del usuario se ha perdido", ex);

            }
            catch (InvalidCastException ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("El metodo del callback no pertenece a dicho contexto \n" + ex.StackTrace);
                log.LogWarn("el callback no pertenece a dicho contexto ", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
        }
    }
}
