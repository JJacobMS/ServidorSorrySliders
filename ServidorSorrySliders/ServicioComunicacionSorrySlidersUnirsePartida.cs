using DatosSorrySliders;
using InterfacesServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace ServidorSorrySliders
{
    public partial class ServicioComunicacionSorrySliders: IUnirsePartida
    {
        
        public (Constantes, List<CuentaSet>) RecuperarJugadoresLobby(string uid)
        {
            Logger log = new Logger(this.GetType(), "IUnirsePartida");
            List<CuentaSet> cuentasPartidaLobby = new List<CuentaSet>();
            try
            {
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    var cuentasPartida = contexto.Database.SqlQuery<CuentaSet>
                        ("Select CuentaSet.CorreoElectronico, Avatar, Nickname, Contraseña, IdUsuario from CuentaSet " +
                        "Inner Join RelacionPartidaCuentaSet ON RelacionPartidaCuentaSet.CorreoElectronico = CuentaSet.CorreoElectronico " +
                        "Where RelacionPartidaCuentaSet.CodigoPartida = @uid " +
                        "Order By RelacionPartidaCuentaSet.IdPartidaCuenta;", new SqlParameter("@uid", uid)).ToList();

                    if (cuentasPartida == null || cuentasPartida.Count <= 0)
                    {
                        return (Constantes.OPERACION_EXITOSA_VACIA, null);
                    }

                    for (int i = 0; i < cuentasPartida.Count; i++)
                    {
                        cuentasPartidaLobby.Add(new CuentaSet
                        {
                            Nickname = cuentasPartida[i].Nickname,
                            CorreoElectronico = cuentasPartida[i].CorreoElectronico,
                            Avatar = cuentasPartida[i].Avatar
                        });
                    }

                    return (Constantes.OPERACION_EXITOSA, cuentasPartidaLobby);
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
            catch (DataException ex)
            {
                log.LogError("Hubo un error con alguno de los componentes de ADO.NET", ex);
                return (Constantes.ERROR_CONSULTA, null);
            }
        }

        public (Constantes, PartidaSet) RecuperarPartida(string codigoPartida)
        {
            Logger log = new Logger(this.GetType(), "IUnirsePartida");
            try
            {
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    var partidaRecuperada = contexto.Database.SqlQuery<PartidaSet>
                        ("SELECT PartidaSet.CodigoPartida, PartidaSet.CorreoElectronico, PartidaSet.CantidadJugadores from PartidaSet " +
                        "INNER JOIN RelacionPartidaCuentaSet ON RelacionPartidaCuentaSet.CodigoPartida = PartidaSet.CodigoPartida " +
                        "Where PartidaSet.CodigoPartida = @codigoPartida;", new SqlParameter("@codigoPartida", codigoPartida)).FirstOrDefault();
                    if (partidaRecuperada == null)
                    {
                        return (Constantes.OPERACION_EXITOSA_VACIA, null);
                    }
                    else 
                    {
                        PartidaSet partida = new PartidaSet
                        {
                            CantidadJugadores = partidaRecuperada.CantidadJugadores,
                            CodigoPartida = partidaRecuperada.CodigoPartida,
                            CorreoElectronico = partidaRecuperada.CorreoElectronico
                        };
                        return (Constantes.OPERACION_EXITOSA, partida);
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
            catch (DataException ex)
            {
                log.LogError("Hubo un error con alguno de los componentes de ADO.NET", ex);
                return (Constantes.ERROR_CONSULTA, null);
            }
        }

        public void SalirDelLobby(string correoJugador, string codigoPartida)
        {
            Logger log = new Logger(this.GetType(), "IUnirsePartida");
            try
            {
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    contexto.Database.ExecuteSqlCommand("DELETE from RelacionPartidaCuentaSet where codigoPartida=@codigoPartida AND " +
                        "CorreoElectronico=@correoElectronico;", new SqlParameter("@codigoPartida", codigoPartida),
                        new SqlParameter("@correoElectronico", correoJugador));

                    int numeroJugadoresRestantes = contexto.Database.SqlQuery<int>
                        ("SELECT COUNT(CorreoElectronico) FROM RelacionPartidaCuentaSet " +
                        "WHERE codigoPartida = @codigoPartida", 
                        new SqlParameter("@codigoPartida", codigoPartida)).FirstOrDefault();

                    if (numeroJugadoresRestantes <= 0)
                    {
                        contexto.Database.ExecuteSqlCommand("DELETE FROM PartidaSet WHERE CodigoPartida = @codigo",
                            new SqlParameter("@codigo", codigoPartida));
                    }
                }
            }
            catch (SqlException ex)
            {
                log.LogError("Error al ejecutar consulta SQL", ex);
            }
            catch (EntityException ex)
            {
                log.LogError("Error de conexión a la base de datos", ex);
            }
            catch (DataException ex)
            {
                log.LogError("Hubo un error con alguno de los componentes de ADO.NET", ex);
            }
        }

        public (Constantes, int) UnirseAlLobby(string uid, string correoJugadorNuevo)
        {
            Logger log = new Logger(this.GetType(), "IUnirsePartida");
            int numeroMaximoJugadores = -1;
            try
            {
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    var estaBaneado = contexto.Database.SqlQuery<string>("select DISTINCT CorreoElectronicoJugadorBaneado from RelacionBaneadosSet " +
                        "INNER JOIN RelacionPartidaCuentaSet On CorreoElectronico = CorreoElectronicoJugadorPrincipal " +
                        "WHERE CodigoPartida = @uid AND CorreoElectronicoJugadorBaneado = @correo",
                        new SqlParameter("@uid", uid), new SqlParameter("@correo", correoJugadorNuevo)).Any();

                    int partidaTerminada = -3;
                    if (!CodigoPartidaExiste(uid))
                    {
                        return (Constantes.OPERACION_EXITOSA_VACIA, partidaTerminada);
                    }

                    int jugadorBaneado = -2;
                    if (estaBaneado)
                    {
                        return (Constantes.OPERACION_EXITOSA_VACIA,jugadorBaneado);
                    }

                    var cuentasPartida = contexto.Database.SqlQuery<CuentaSet>
                        ("Select CuentaSet.CorreoElectronico, Avatar, Nickname, Contraseña, IdUsuario from CuentaSet " +
                        "Inner Join RelacionPartidaCuentaSet ON RelacionPartidaCuentaSet.CorreoElectronico = CuentaSet.CorreoElectronico " +
                        "Where RelacionPartidaCuentaSet.CodigoPartida = @uid " +
                        "Order By RelacionPartidaCuentaSet.IdPartidaCuenta;", new SqlParameter("@uid", uid)).ToList();

                    int partidaInexistente = 0;
                    if (cuentasPartida == null)
                    {
                        return (Constantes.OPERACION_EXITOSA_VACIA, partidaInexistente);
                    }

                    numeroMaximoJugadores = contexto.Database.SqlQuery<int>
                        ("Select CantidadJugadores from PartidaSet Where PartidaSet.CodigoPartida = @uid", new SqlParameter("@uid", uid)).
                        FirstOrDefault();

                    if (numeroMaximoJugadores <= cuentasPartida.Count)
                    {
                        return (Constantes.OPERACION_EXITOSA_VACIA, numeroMaximoJugadores);
                    }

                    int jugadorEnPartida = -1;
                    for (int i = 0; i < cuentasPartida.Count; i++)
                    {
                        if (cuentasPartida[i].CorreoElectronico.Equals(correoJugadorNuevo))
                        {
                            return (Constantes.OPERACION_EXITOSA_VACIA, jugadorEnPartida);
                        }
                    }

                    contexto.Database.ExecuteSqlCommand("Insert into RelacionPartidaCuentaSet(Posicion,CodigoPartida, CorreoElectronico) " +
                        "VALUES (0, @uid, @correo);", new SqlParameter("@uid", uid), new SqlParameter("@correo", correoJugadorNuevo));

                    return (Constantes.OPERACION_EXITOSA, numeroMaximoJugadores);
                }
            }
            catch (SqlException ex)
            {
                log.LogError("Error al ejecutar consulta SQL", ex);
                return (Constantes.ERROR_CONSULTA, numeroMaximoJugadores);
            }
            catch (EntityException ex)
            {
                log.LogError("Error de conexión a la base de datos", ex);
                return (Constantes.ERROR_CONEXION_BD, numeroMaximoJugadores);
            }
        }

        public Constantes CrearCuentaProvisionalInvitado(CuentaSet cuentaProvisionalInvitado)
        {
            Logger log = new Logger(this.GetType(), "IUnirsePartida");
            try
            {
                using (var context = new BaseDeDatosSorrySlidersEntities())
                {
                    (Constantes respuesta, int idUsuarioInvitado) = RecuperarUsuarioInvitado();
                    if (respuesta != Constantes.OPERACION_EXITOSA)
                    {
                        return respuesta;
                    }

                    if (idUsuarioInvitado != -1)
                    {
                        context.Database.ExecuteSqlCommand("INSERT INTO CuentaSet(CorreoElectronico, Avatar, Contraseña, Nickname, IdUsuario) " +
                        "VALUES(@correo, @avatar,'', @nickname, @idUsuario)",
                        new SqlParameter("@correo", cuentaProvisionalInvitado.CorreoElectronico),
                        new SqlParameter("@avatar", cuentaProvisionalInvitado.Avatar),
                        new SqlParameter("@nickname", cuentaProvisionalInvitado.Nickname),
                        new SqlParameter("@idUsuario", idUsuarioInvitado));
                    }
                    return Constantes.OPERACION_EXITOSA;
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

        public void EliminarCuentaProvisional(string correoElectronico)
        {
            Logger log = new Logger(this.GetType(), "IUnirsePartida");
            try
            {
                using (var context = new BaseDeDatosSorrySlidersEntities())
                {
                    context.Database.ExecuteSqlCommand("DELETE FROM CuentaSet " +
                        "WHERE CorreoElectronico = @correo",
                        new SqlParameter("@correo", correoElectronico));
                }
            }
            catch (SqlException ex)
            {
                log.LogError("Error al ejecutar consulta SQL", ex);
            }
            catch (EntityException ex)
            {
                log.LogError("Error de conexión a la base de datos", ex);
            }
        }

        private (Constantes, int) RecuperarUsuarioInvitado()
        {
            Constantes respuesta;
            int idUsuario = -1;
            Logger log = new Logger(this.GetType(), "IUnirsePartida");
            try
            {
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    var usuarioInvitado = contexto.Database.SqlQuery<UsuarioSet>
                        ("SELECT * FROM UsuarioSet WHERE Apellido = '' AND Nombre = ''").FirstOrDefault();

                    if (usuarioInvitado == null)
                    {
                        idUsuario = contexto.Database.SqlQuery<int>
                            ("INSERT INTO UsuarioSet(Nombre, Apellido) VALUES('', '') SELECT CAST(SCOPE_IDENTITY() AS int)").Single();
                    }
                    else
                    {
                        idUsuario = usuarioInvitado.IdUsuario;
                    }
                    respuesta = Constantes.OPERACION_EXITOSA;

                }
            }
            catch (SqlException ex)
            {
                respuesta = Constantes.ERROR_CONSULTA;
                log.LogError("Error al ejecutar consulta SQL", ex);
            }
            catch (EntityException ex)
            {
                respuesta = Constantes.ERROR_CONEXION_BD;
                log.LogError("Error de conexión a la base de datos", ex);
            }

            return (respuesta, idUsuario);
        }
        public void SalirJuegoCompleto(string uid, string correo)
        {
            lock (_diccionarioPuntuacion)
            {
                ManejarOperationContext.EliminarJugadorDiccionarioPorCorreo(_diccionarioPuntuacion, uid, correo);
            }
            lock (_jugadoresEnLineaJuegoLanzamiento)
            {
                ManejarOperationContext.EliminarJugadorDiccionarioPorCorreo(_jugadoresEnLineaJuegoLanzamiento, uid, correo);
            }
            lock (_jugadoresEnLineaChat)
            {
                ManejarOperationContext.EliminarJugadorDiccionarioPorCorreo(_jugadoresEnLineaChat, uid, correo);
            }
        }
    }
}
