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
                    int filasAfectadas = contexto.SaveChanges();
                    if (filasAfectadas != 0)
                    {
                        return Constantes.OPERACION_EXITOSA;
                    }
                    else 
                    {
                        return Constantes.OPERACION_EXITOSA_VACIA;
                    }
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

        public void NotificarUsuario(string correoElectronico)
        {
            LlamarCallback(correoElectronico);
        }

        public void LlamarCallback(string correoElectronico)
        {
            Logger log = new Logger(this.GetType(), "INotificarJugadores");
            try
            {
                if (_jugadoresEnLineaListaAmigos.ContainsKey(correoElectronico))
                {
                    _jugadoresEnLineaListaAmigos[correoElectronico].GetCallbackChannel<INotificarJugadoresCallback>().RecuperarNotificacion();
                }
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

        public Constantes AgregarProxy(string correoElectronico)
        {
            Logger log = new Logger(this.GetType(), "INotificar");
            try
            {
                if (_jugadoresEnLineaListaAmigos.ContainsKey(correoElectronico))
                {
                    _jugadoresEnLineaListaAmigos.Remove(correoElectronico);
                    _jugadoresEnLineaListaAmigos.Add(correoElectronico, OperationContext.Current);
                    Console.WriteLine("Remover key con contexto y guardar key con contexto");
                }
                else 
                {
                    _jugadoresEnLineaListaAmigos.Add(correoElectronico, OperationContext.Current);
                    Console.WriteLine("Guardar nuevo Key y contexto");
                }
                foreach (var correo in _jugadoresEnLineaListaAmigos)
                {
                    Console.WriteLine("Correo Guardado: "+correo);
                }
                return Constantes.OPERACION_EXITOSA;
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
            return Constantes.ERROR_CONEXION_SERVIDOR;
        }

        public void EliminarProxy(string correoElectronico)
        {
            if (_jugadoresEnLineaListaAmigos.ContainsKey(correoElectronico))
            {
                _jugadoresEnLineaListaAmigos.Remove(correoElectronico);
            }
        }

        public (Constantes, List<NotificacionSet>) RecuperarNotificaciones(string correoElectronico)
        {
            Logger log = new Logger(this.GetType(), "IListaAmigos");
            try
            {
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    var notificacionesJugador = contexto.Database.SqlQuery<NotificacionSet>("SELECT idNotificacion, " +
                        "CorreoElectronicoRemitente, CorreoElectronicoDestinatario, IdTipoNotificacion, Mensaje FROM " +
                        "NotificacionSet where CorreoElectronicoDestinatario=@correo", new SqlParameter("@correo", correoElectronico)).ToList();

                    List<NotificacionSet> listaNotificaciones = new List<NotificacionSet>(){ };

                    foreach (var notificacion in notificacionesJugador) 
                    {
                        NotificacionSet notificacionNueva = new NotificacionSet()
                        {
                            IdNotificacion = notificacion.IdNotificacion,
                            CorreoElectronicoRemitente = notificacion.CorreoElectronicoRemitente,
                            CorreoElectronicoDestinatario = notificacion.CorreoElectronicoDestinatario,
                            IdTipoNotificacion = notificacion.IdTipoNotificacion,
                            Mensaje = notificacion.Mensaje
                            
                        };
                        listaNotificaciones.Add(notificacionNueva);
                    }

                    if (listaNotificaciones == null || listaNotificaciones.Count <= 0)
                    {
                        return (Constantes.OPERACION_EXITOSA_VACIA, null);
                    }
                    else
                    {
                        return (Constantes.OPERACION_EXITOSA, listaNotificaciones);
                    }
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

        public Constantes EliminarNotificacionJugador(string correoElectronico, int idNotificacion)
        {
            Logger log = new Logger(GetType(), "IListaAmigos");
            try
            {
                using (var context = new BaseDeDatosSorrySlidersEntities()) {
                    var filasAfectadas = context.Database.ExecuteSqlCommand("DELETE FROM NotificacionSet where IdNotificacion=@idNotificacion AND " +
                        "CorreoElectronicoDestinatario=@correoElectronico;", new SqlParameter("@idNotificacion", idNotificacion), 
                        new SqlParameter("@correoElectronico", correoElectronico));
                    if (filasAfectadas > 0)
                    {
                        return Constantes.OPERACION_EXITOSA;
                    }
                    else
                    {
                        return Constantes.OPERACION_EXITOSA_VACIA;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.StackTrace);
                log.LogError("Error al ejecutar consulta SQL", ex);
                return Constantes.ERROR_CONSULTA;
            }
            catch (EntityException ex) 
            {
                Console.WriteLine(ex.StackTrace);
                log.LogError("Error de conexión a la base de datos", ex);
                return Constantes.ERROR_CONEXION_BD;
            }
        }

        public Constantes GuardarAmistad(string correoElectronicoDestinatario, string correoElectronicoRemitente)
        {
            Logger log = new Logger(this.GetType(),"IListaAmgos");
            using (var contexto = new BaseDeDatosSorrySlidersEntities()) 
            {
                try
                {
                    var filasAfectadas = contexto.Database.ExecuteSqlCommand("INSERT INTO RelaciónAmigosSet (CorreoElectronicoJugadorPrincipal, CorreoElectronicoJugadorAmigo) " +
                        "VALUES (@correoDestinatario,@correoRemitente), (@correoRemitente, @correoDestinatario);", 
                        new SqlParameter("@correoDestinatario",correoElectronicoDestinatario), new SqlParameter("@correoRemitente", correoElectronicoRemitente));
                    if (filasAfectadas > 0)
                    {
                        return Constantes.OPERACION_EXITOSA;
                    }
                    else 
                    {
                        return Constantes.OPERACION_EXITOSA_VACIA;
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    log.LogError("Error al ejecutar consulta SQL", ex);
                    return Constantes.ERROR_CONSULTA;
                }
                catch (EntityException ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    log.LogError("Error de conexión a la base de datos", ex);
                    return Constantes.ERROR_CONEXION_BD;
                }
            }
        }

        public (Constantes, List<CuentaSet>) RecuperarAmigos(string correoElectronico)
        {
            Logger log = new Logger(this.GetType(), "IListaAmigos");
            try
            {
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    var amigosJugador = contexto.Database.SqlQuery<CuentaSet>("SELECT CuentaSet.CorreoElectronico, CuentaSet.Nickname, CuentaSet.Avatar, CuentaSet.Contraseña, CuentaSet.IdUsuario " +
                        "FROM CuentaSet INNER JOIN RelaciónAmigosSet on CuentaSet.CorreoElectronico = RelaciónAmigosSet.CorreoElectronicoJugadorAmigo AND " +
                        "RelaciónAmigosSet.CorreoElectronicoJugadorPrincipal=@correo;", new SqlParameter("@correo", correoElectronico)).ToList();

                    List<CuentaSet> listaAmigos = new List<CuentaSet>();

                    foreach (var amigo in amigosJugador)
                    {
                        CuentaSet amigoNuevo = new CuentaSet()
                        {
                            CorreoElectronico = amigo.CorreoElectronico,
                            Nickname = amigo.Nickname
                        };
                        listaAmigos.Add(amigoNuevo);
                    }

                    if (listaAmigos == null || listaAmigos.Count <= 0)
                    {
                        return (Constantes.OPERACION_EXITOSA_VACIA, null);
                    }
                    else
                    {
                        return (Constantes.OPERACION_EXITOSA, listaAmigos);
                    }
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

        public Constantes EliminarAmistad(string correoElectronicoPrincipal, string correoElectronicoAmigo)
        {
            Logger log = new Logger(this.GetType(), "IListaAmigos");
            try
            {
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    var filasAfectadas = contexto.Database.ExecuteSqlCommand("DELETE FROM RelaciónAmigosSet where " +
                        "(CorreoElectronicoJugadorPrincipal = @correoPrincipal AND CorreoElectronicoJugadorAmigo = @correoAmigo) OR " +
                        "(CorreoElectronicoJugadorPrincipal = @correoAmigo AND CorreoElectronicoJugadorAmigo = @correoPrincipal);", 
                        new SqlParameter("@correoPrincipal", correoElectronicoPrincipal), new SqlParameter("@correoAmigo",correoElectronicoAmigo));

                    if (filasAfectadas <= 0)
                    {
                        return Constantes.OPERACION_EXITOSA_VACIA;
                    }
                    else
                    {
                        return Constantes.OPERACION_EXITOSA;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.StackTrace);
                log.LogError("Error al ejecutar consulta SQL", ex);
                return Constantes.ERROR_CONSULTA;
            }
            catch (EntityException ex)
            {
                Console.WriteLine(ex.StackTrace);
                log.LogError("Error de conexión a la base de datos", ex);
                return Constantes.ERROR_CONEXION_BD;
            }
        }
    }
}
