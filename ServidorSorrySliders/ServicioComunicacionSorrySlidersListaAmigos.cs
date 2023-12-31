﻿using DatosSorrySliders;
using InterfacesServidorSorrySliders;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
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
                log.LogError("Error al ejecutar consulta SQL", ex);
                return (Constantes.ERROR_CONSULTA, null);
            }
            catch (EntityException ex)
            {
                log.LogError("Error de conexión a la base de datos", ex);
                return (Constantes.ERROR_CONEXION_BD, null);
            }
        }

        public (Constantes, List<CuentaSet>) RecuperarJugadoresCuenta(string informacionJugador, string correoJugador)
        {
            Logger log = new Logger(this.GetType(), "IListaAmigos");
            try
            {
                List<CuentaSet> jugadoresEncontrados = new List<CuentaSet>();
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    var jugadores = contexto.Database.SqlQuery<CuentaSet>
                        ("Select * From CuentaSet Where (Nickname Like '%' + @nickname +'%') " +
                        "AND CorreoElectronico Like '%@%' AND CorreoElectronico != @correoJugadorOriginal " +
                        "ORDER BY CorreoElectronico DESC ",
                        new SqlParameter("@nickname", informacionJugador),
                        new SqlParameter("@correoJugadorOriginal", correoJugador)).ToList();
                    
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
                log.LogError("Error al ejecutar consulta SQL", ex);
                return (Constantes.ERROR_CONSULTA, null);
            }
            catch (EntityException ex)
            {
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

                    if (notificacionesRecuperadas == null || notificacionesRecuperadas.Count <= 0)
                    {
                        return (Constantes.OPERACION_EXITOSA_VACIA, null);
                    }

                    for (int i = 0; i < notificacionesRecuperadas.Count; i++)
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
                log.LogError("Error al ejecutar consulta SQL", ex);
                return (Constantes.ERROR_CONSULTA, null);
            }
            catch (EntityException ex)
            {
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
                log.LogError("Error al ejecutar consulta SQL", ex);
                return (Constantes.ERROR_CONSULTA);
            }
            catch (EntityException ex)
            {
                log.LogError("Error de conexión a la base de datos", ex);
                return (Constantes.ERROR_CONEXION_BD);
            }
            catch (DataException ex)
            {
                log.LogError("Hubo un error con alguno de los componentes de ADO.NET", ex);
                return (Constantes.ERROR_CONSULTA);
            }
        }

        public void NotificarUsuario(string correoElectronico)
        {
            LlamarCallback(correoElectronico);
        }

        public void LlamarCallback(string correoElectronico)
        {
            Logger log = new Logger(this.GetType(), "INotificarJugadores");
            lock (_jugadoresEnLineaListaAmigos)
            {
                try
                {
                    if (_jugadoresEnLineaListaAmigos.ContainsKey(correoElectronico))
                    {
                        _jugadoresEnLineaListaAmigos[correoElectronico].GetCallbackChannel<INotificarJugadoresCallback>().RecuperarNotificacion();
                    }
                }
                catch (CommunicationException ex)
                {
                    EliminarProxy(correoElectronico);
                    log.LogWarn("Error comunicación con el cliente", ex);
                }
                catch (TimeoutException ex)
                {
                    EliminarProxy(correoElectronico);
                    log.LogWarn("Se agoto el tiempo de espera del cliente", ex);
                }
            }
        }

        public Constantes AgregarProxy(string correoElectronico)
        {
            Logger log = new Logger(this.GetType(), "INotificar");
            lock (_jugadoresEnLineaListaAmigos)
            {
                try
                {
                    if (_jugadoresEnLineaListaAmigos.ContainsKey(correoElectronico))
                    {
                        _jugadoresEnLineaListaAmigos.Remove(correoElectronico);
                        _jugadoresEnLineaListaAmigos.Add(correoElectronico, OperationContext.Current);
                    }
                    else
                    {
                        _jugadoresEnLineaListaAmigos.Add(correoElectronico, OperationContext.Current);
                    }
                    return Constantes.OPERACION_EXITOSA;
                }
                catch (CommunicationException ex)
                {
                    EliminarProxy(correoElectronico);
                    log.LogWarn("Error comunicación con el cliente", ex);
                }
                catch (TimeoutException ex)
                {
                    EliminarProxy(correoElectronico);
                    log.LogWarn("Se agoto el tiempo de espera del cliente", ex);
                }
                return Constantes.ERROR_CONEXION_SERVIDOR;
            }
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

                    if (listaNotificaciones.Count <= 0 ) 
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
                log.LogError("Error al ejecutar consulta SQL", ex);
                return (Constantes.ERROR_CONSULTA, null);
            }
            catch (EntityException ex)
            {
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
                log.LogError("Error al ejecutar consulta SQL", ex);
                return Constantes.ERROR_CONSULTA;
            }
            catch (EntityException ex) 
            {
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
                    int registrosAmistad = VerificarExistenciaAmistad(correoElectronicoDestinatario, correoElectronicoRemitente);
                    if (registrosAmistad <= 0)
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
                    else
                    {
                        return Constantes.OPERACION_EXITOSA_VACIA;
                    }
                }
                catch (SqlException ex)
                {
                    log.LogError("Error al ejecutar consulta SQL", ex);
                    return Constantes.ERROR_CONSULTA;
                }
                catch (EntityException ex)
                {
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

                    if (listaAmigos.Count <= 0) 
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
                log.LogError("Error al ejecutar consulta SQL", ex);
                return (Constantes.ERROR_CONSULTA, null);
            }
            catch (EntityException ex)
            {
                log.LogError("Error de conexión a la base de datos", ex);
                return (Constantes.ERROR_CONEXION_BD, null);
            }
        }

        public int VerificarExistenciaAmistad(string correoElectronicoPrincipal, string correoElectronicoAmigo)
        {
            int respuesta = 1;
            Logger log = new Logger(this.GetType(), "IListaAmigos");
            try
            {
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    var filasAfectadas = contexto.Database.SqlQuery<int>("SELECT COUNT(*) FROM RelaciónAmigosSet where CorreoElectronicoJugadorPrincipal=@correoPrincipal " +
                        "AND CorreoElectronicoJugadorAmigo=@correoAmigo;", new SqlParameter("@correoPrincipal", correoElectronicoPrincipal), 
                        new SqlParameter("@correoAmigo", correoElectronicoAmigo)).ToArray();
                    return filasAfectadas[0];
                }
            }
            catch (SqlException ex)
            {
                log.LogError("Error al ejecutar consulta SQL", ex);
                return respuesta;
            }
            catch (EntityException ex)
            {
                log.LogError("Error de conexión a la base de datos", ex);
                return respuesta;
            }
        }

        public Constantes EliminarAmistad(string correoElectronicoPrincipal, string correoElectronicoAmigo)
        {
            CambiarSingle();
            Logger log = new Logger(this.GetType(), "IListaAmigos");
            try
            {
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    var filasAfectadas = contexto.Database.ExecuteSqlCommand("DELETE FROM RelaciónAmigosSet where " +
                        "(CorreoElectronicoJugadorPrincipal = @correoPrincipal AND CorreoElectronicoJugadorAmigo = @correoAmigo) OR " +
                        "(CorreoElectronicoJugadorPrincipal = @correoAmigo AND CorreoElectronicoJugadorAmigo = @correoPrincipal);",
                        new SqlParameter("@correoPrincipal", correoElectronicoPrincipal), new SqlParameter("@correoAmigo", correoElectronicoAmigo));
                    if (filasAfectadas <= 0)
                    {
                        CambiarMultiple();
                        return Constantes.OPERACION_EXITOSA_VACIA;
                    }
                    else
                    {
                        CambiarMultiple();
                        return Constantes.OPERACION_EXITOSA;
                    }
                }
            }
            catch (SqlException ex)
            {
                log.LogError("Error al ejecutar consulta SQL", ex);
                CambiarMultiple();
                return Constantes.ERROR_CONSULTA;
            }
            catch (EntityException ex)
            {
                log.LogError("Error de conexión a la base de datos", ex);
                CambiarMultiple();
                return Constantes.ERROR_CONEXION_BD;
            }
        }

        public (Constantes, List<CuentaSet>) RecuperarBaneados(string correoElectronico)
        {
            Logger log = new Logger(this.GetType(), "IListaAmigos");
            try
            {
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    var baneadosJugador = contexto.Database.SqlQuery<CuentaSet>("SELECT CuentaSet.CorreoElectronico, CuentaSet.Nickname, CuentaSet.Avatar, CuentaSet.Contraseña, CuentaSet.IdUsuario " +
                        "FROM CuentaSet INNER JOIN RelacionBaneadosSet on CuentaSet.CorreoElectronico = RelacionBaneadosSet.CorreoElectronicoJugadorBaneado AND " +
                        "RelacionBaneadosSet.CorreoElectronicoJugadorPrincipal=@correo;", new SqlParameter("@correo", correoElectronico)).ToList();
                    List<CuentaSet> listaBaneados = new List<CuentaSet>();
                    foreach (var amigo in baneadosJugador)
                    {
                        CuentaSet baneado = new CuentaSet()
                        {
                            CorreoElectronico = amigo.CorreoElectronico,
                            Nickname = amigo.Nickname
                        };
                        listaBaneados.Add(baneado);
                    }
                    if (listaBaneados.Count <= 0)
                    {
                        return (Constantes.OPERACION_EXITOSA_VACIA, null);
                    }
                    else
                    {
                        return (Constantes.OPERACION_EXITOSA, listaBaneados);
                    }
                }
            }
            catch (SqlException ex)
            {
                log.LogError("Error al ejecutar consulta SQL", ex);
                return (Constantes.ERROR_CONSULTA, null);
            }
            catch (EntityException ex)
            {
                log.LogError("Error de conexión a la base de datos", ex);
                return (Constantes.ERROR_CONEXION_BD, null);
            }
        }

        public (Constantes, List<CuentaSet>) RecuperarSolicitudesAmistad(string correoElectronico)
        {
            Logger log = new Logger(this.GetType(), "IListaAmigos");
            try
            {
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    var jugadoresSolicitudAmigos = contexto.Database.SqlQuery<CuentaSet>("SELECT CuentaSet.CorreoElectronico, CuentaSet.Nickname, CuentaSet.Avatar, CuentaSet.Contraseña, CuentaSet.IdUsuario " +
                        "FROM CuentaSet INNER JOIN NotificacionSet ON " +
                        "((CuentaSet.CorreoElectronico = NotificacionSet.CorreoElectronicoDestinatario OR CuentaSet.CorreoElectronico = NotificacionSet.CorreoElectronicoRemitente) " +
                        "AND NotificacionSet.IdTipoNotificacion = 2) AND (CorreoElectronicoRemitente=@correo OR CorreoElectronicoDestinatario=@correo) " +
                        "AND CuentaSet.CorreoElectronico!=@correo;", new SqlParameter("@correo", correoElectronico)).ToList();
                    List<CuentaSet> listaSolicitudes = new List<CuentaSet>();
                    foreach (var jugador in jugadoresSolicitudAmigos)
                    {
                        CuentaSet jugadorSolicitado = new CuentaSet()
                        {
                            CorreoElectronico = jugador.CorreoElectronico,
                            Nickname = jugador.Nickname
                        };
                        listaSolicitudes.Add(jugadorSolicitado);
                    }
                    if (listaSolicitudes.Count <= 0) 
                    {
                        return (Constantes.OPERACION_EXITOSA_VACIA, null);
                    }
                    else
                    {
                        return (Constantes.OPERACION_EXITOSA, listaSolicitudes);
                    }
                }
            }
            catch (SqlException ex)
            {
                log.LogError("Error al ejecutar consulta SQL", ex);
                return (Constantes.ERROR_CONSULTA, null);
            }
            catch (EntityException ex)
            {
                log.LogError("Error de conexión a la base de datos", ex);
                return (Constantes.ERROR_CONEXION_BD, null);
            }
        }

        public Constantes BanearJugador(string correoElectronicoPrincipal, string correoElectronicoBaneado)
        {
            Logger log = new Logger(this.GetType(), "IListaAmigos");
            try
            {
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    RelacionBaneadosSet relacionBaneo = new RelacionBaneadosSet()
                    {
                        CorreoElectronicoJugadorPrincipal = correoElectronicoPrincipal,
                        CorreoElectronicoJugadorBaneado = correoElectronicoBaneado
                    };
                    contexto.RelacionBaneadosSet.Add(relacionBaneo);
                    int filasAfectadas = contexto.SaveChanges();
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
                log.LogError("Error al ejecutar consulta SQL", ex);
                return (Constantes.ERROR_CONSULTA);
            }
            catch (EntityException ex)
            {
                log.LogError("Error de conexión a la base de datos", ex);
                return (Constantes.ERROR_CONEXION_BD);
            }
            catch (DataException ex)
            {
                log.LogError("Hubo un error con alguno de los componentes de ADO.NET", ex);
                return (Constantes.ERROR_CONSULTA);
            }
        }

        public Constantes EliminarBaneo(string correoElectronicoPrincipal, string correoElectronicoBaneado)
        {
            Logger log = new Logger(this.GetType(), "IListaAmigos");
            try
            {
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    var filasAfectadas = contexto.Database.ExecuteSqlCommand("DELETE FROM RelacionBaneadosSet where " +
                        "CorreoElectronicoJugadorPrincipal=@correoPrincipal AND CorreoElectronicoJugadorBaneado=@correoBaneado;",
                        new SqlParameter("@correoPrincipal", correoElectronicoPrincipal), new SqlParameter("@correoBaneado", correoElectronicoBaneado));

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
                log.LogError("Error al ejecutar consulta SQL", ex);
                return Constantes.ERROR_CONSULTA;
            }
            catch (EntityException ex)
            {
                log.LogError("Error de conexión a la base de datos", ex);
                return Constantes.ERROR_CONEXION_BD;
            }
        }

        public Constantes EnviarCorreo(string correoElectronicoDestinatario, string asuntoCorreo, string cuerpoCorreo)
        {
            Logger log = new Logger(this.GetType(), "IListaAmigos");

            IConfiguration configuracion;
            try
            {
                configuracion = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "recursos"))
                .AddJsonFile("ConfiguracionesAplicacion.json").Build();
            }
            catch (IOException ex)
            {
                log.LogWarn("No se ha pudo recuperar el archivo de credenciales", ex);
                return Constantes.ERROR_CONSULTA;
            }

            try
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    MailMessage correo = new MailMessage();
                    string correoJuego = configuracion["ConfiguracionesCorreo:CorreoJuego"];
                    string contraseñaAplicacion = Descifrador.Descrifrar(configuracion["ConfiguracionesCorreo:ContrasenaJuego"]);
                    correo.From = new MailAddress(correoJuego);
                    correo.To.Add(correoElectronicoDestinatario);
                    correo.Subject = asuntoCorreo;
                    correo.Body = cuerpoCorreo;
                    correo.IsBodyHtml = true;
                    SmtpClient clienteSmtp = new SmtpClient(configuracion["ConfiguracionesCorreo:ClienteSmtp"]);

                    string puerto = configuracion.GetSection("ConfiguracionesCorreo")["PuertoSmtp"];
                    clienteSmtp.Port = int.Parse(puerto);
                    clienteSmtp.Credentials = new NetworkCredential(correoJuego, contraseñaAplicacion);
                    clienteSmtp.EnableSsl = true;
                    clienteSmtp.Send(correo);
                    return Constantes.OPERACION_EXITOSA;
                }
                else
                {
                    return Constantes.OPERACION_EXITOSA_VACIA;
                }
            }
            catch (FormatException ex)
            {
                log.LogError("Ha ocurrido un error inesperado", ex);
                return Constantes.ERROR_CONSULTA;
            }
            catch (SmtpFailedRecipientException ex)
            {
                log.LogError("Ha al enviar el correo electronico al destinatario", ex);
                return Constantes.ERROR_CONSULTA;
            }
            catch (SmtpException ex)
            {
                log.LogError("Ha ocurrido un error de autenticación", ex);
                return Constantes.ERROR_CONSULTA;
            }
            catch (CryptographicException ex)
            {
                log.LogError("Ha ocurrido un error de cifrado", ex);
                return Constantes.ERROR_CONSULTA;
            }
        }

    }
}
