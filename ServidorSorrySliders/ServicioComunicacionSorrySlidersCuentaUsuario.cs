﻿using DatosSorrySliders;
using InterfacesServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorSorrySliders
{
    public partial class ServicioComunicacionSorrySliders : IDetallesCuentaUsuario
    {
        
        public (Constantes, UsuarioSet) RecuperarDatosUsuarioDeCuenta (string correoElectronico)
        {
            Logger log = new Logger(this.GetType(), "IDetallesCuentaUsuario");
            try
            {
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    var usuarioConsulta = contexto.Database.SqlQuery<UsuarioSet>
                        ("select UsuarioSet.IdUsuario, Nombre, Apellido from UsuarioSet " +
                        "Inner Join CuentaSet On CuentaSet.IdUsuario = UsuarioSet.IdUsuario " +
                        "Where CuentaSet.CorreoElectronico = @correo",
                        new SqlParameter("@correo", correoElectronico)).
                    FirstOrDefault();

                    if (usuarioConsulta == null)
                    {
                        return (Constantes.OPERACION_EXITOSA_VACIA, null);
                    }

                    UsuarioSet usuario = new UsuarioSet
                    {
                        Nombre = usuarioConsulta.Nombre, Apellido = usuarioConsulta.Apellido
                    };

                    return (Constantes.OPERACION_EXITOSA, usuario);
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

        public Constantes VerificarContrasenaActual(CuentaSet cuenta)
        {
            Logger log = new Logger(this.GetType(), "IDetallesCuentaUsuario");
            try
            {
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    var cuentaVerificada = contexto.Database.SqlQuery<string>
                        ("Select CorreoElectronico From cuentaSet " +
                        "WHERE HASHBYTES('SHA2_512', @contrasena) = Contraseña And CorreoElectronico = @correo", 
                        new SqlParameter("@contrasena", cuenta.Contraseña),
                        new SqlParameter("@correo", cuenta.CorreoElectronico)).
                    FirstOrDefault();

                    if (cuentaVerificada == null)
                    {
                        return Constantes.OPERACION_EXITOSA_VACIA;
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

        public Constantes CambiarContrasena(CuentaSet cuenta)
        {
            Logger log = new Logger(this.GetType(), "IDetallesCuentaUsuario");
            try
            {
                using (var context = new BaseDeDatosSorrySlidersEntities())
                {
                    var filasAfectadas = context.Database.ExecuteSqlCommand(
                        "UPDATE CuentaSet SET Contraseña = HASHBYTES('SHA2_512', @contrasenaNueva) " +
                        "WHERE CorreoElectronico = @correo",
                        new SqlParameter("@contrasenaNueva", cuenta.Contraseña),
                        new SqlParameter("@correo", cuenta.CorreoElectronico));

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
    }

    public partial class ServicioComunicacionSorrySliders : IRegistroUsuario
    {
        public Constantes ActualizarUsuario(UsuarioSet usuarioPorActualizar, CuentaSet cuentaPorActualizar)
        {
            Logger log = new Logger(this.GetType(), "IRegistroUsuario");
            try
            {
                using (var context = new BaseDeDatosSorrySlidersEntities()) 
                {
                    var filasAfectadasUsuario = context.Database.ExecuteSqlCommand("UPDATE UsuarioSet SET Nombre = @nombre, " +
                        "Apellido= @apellido where IdUsuario = (SELECT IdUsuario FROM CuentaSet WHERE CorreoElectronico = @correo)",
                        new SqlParameter("@correo", cuentaPorActualizar.CorreoElectronico),
                        new SqlParameter("@apellido", usuarioPorActualizar.Apellido), 
                        new SqlParameter("@nombre", usuarioPorActualizar.Nombre));

                    var filasAfectadasCuenta = context.Database.ExecuteSqlCommand("UPDATE CuentaSet SET Nickname = @nickname, " +
                        "Avatar = @avatar where CorreoElectronico = @correo; ",
                        new SqlParameter("@nickname", cuentaPorActualizar.Nickname),
                        new SqlParameter("@correo", cuentaPorActualizar.CorreoElectronico),
                        new SqlParameter("@avatar", (cuentaPorActualizar.Avatar)));


                    if (filasAfectadasUsuario > 0 && filasAfectadasCuenta > 0)
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

        public Constantes AgregarUsuario(UsuarioSet usuarioPorGuardar, CuentaSet cuentaPorGuardar)
        {
            Logger log = new Logger(this.GetType(), "IRegistroUsuario");
            try
            {
                using (var context = new BaseDeDatosSorrySlidersEntities())
                {
                    context.UsuarioSet.Add(usuarioPorGuardar);
                    context.SaveChanges();

                    context.Database.ExecuteSqlCommand("INSERT INTO CuentaSet(CorreoElectronico, Avatar, Contraseña, Nickname, IdUsuario) " +
                        "VALUES(@correo, @avatar, HASHBYTES('SHA2_512', @contrasena), @nickname, @idUsuario)",
                        new SqlParameter("@correo", cuentaPorGuardar.CorreoElectronico),
                        new SqlParameter("@avatar", (cuentaPorGuardar.Avatar)),
                        new SqlParameter("@contrasena", cuentaPorGuardar.Contraseña), 
                        new SqlParameter("@nickname", cuentaPorGuardar.Nickname),
                        new SqlParameter("@idUsuario", usuarioPorGuardar.IdUsuario));
                }
                return Constantes.OPERACION_EXITOSA;
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
            catch (DataException ex)
            {
                log.LogError("Error con los datos", ex);
                return Constantes.ERROR_CONSULTA;
            }

        }
    }
}
