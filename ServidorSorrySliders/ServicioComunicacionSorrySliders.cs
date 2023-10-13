using DatosSorrySliders;
using InterfacesServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServidorSorrySliders
{
    public partial class ServicioComunicacionSorrySliders : IInicioSesion
    {

        public Constantes VerificarContrasenaDeCuenta(CuentaSet cuentaPorVerificar)
        {
            try
            {
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    var cuentaVerificada = contexto.Database.SqlQuery<string>
                        ("SELECT CorreoElectronico From CuentaSet WHERE HASHBYTES('SHA2_512', @contrasena) = Contraseña AND CorreoElectronico = @correo",
                        new SqlParameter("@contrasena", cuentaPorVerificar.Contraseña), new SqlParameter("@correo", cuentaPorVerificar.CorreoElectronico)).
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
                Debug.WriteLine(ex.ToString());
                return Constantes.ERROR_CONSULTA;
            }
            catch (EntityException ex)
            {
                Console.WriteLine(ex.ToString());
                return Constantes.ERROR_CONEXION_BD;
            }
        }

        public Constantes VerificarExistenciaCorreoCuenta(string correoElectronico)
        {
            try
            {
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    CuentaSet cuentaVerificada = contexto.CuentaSet.
                    Where(cuenta => cuenta.CorreoElectronico == correoElectronico).FirstOrDefault();

                    if (cuentaVerificada == null)
                    {
                        return Constantes.OPERACION_EXITOSA_VACIA;
                    }

                    return Constantes.OPERACION_EXITOSA;
                }
            }
            catch (EntityException excepcion)
            {
                Debug.WriteLine(excepcion.ToString());
                return Constantes.ERROR_CONEXION_BD;
            }
        }
    }

    public partial class ServicioComunicacionSorrySliders : IRegistroUsuario
    {
        public Constantes AgregarUsuario(UsuarioSet usuarioPorGuardar, CuentaSet cuentaPorGuardar)
        {
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
                        new SqlParameter("@contrasena", cuentaPorGuardar.Contraseña), new SqlParameter("@nickname", cuentaPorGuardar.Nickname),
                        new SqlParameter("@idUsuario", usuarioPorGuardar.IdUsuario));

                    /*string query = $"INSERT INTO CuentaSet(CorreoElectronico, Avatar, Contraseña, Nickname, IdUsuario) VALUES('{cuentaPorGuardar.CorreoElectronico}', {Utilidades.ConvertirArrayByteString(cuentaPorGuardar.Avatar)}, HASHBYTES('SHA2_512', N'{cuentaPorGuardar.Contraseña}'), '{cuentaPorGuardar.Nickname}', {usuarioPorGuardar.IdUsuario})";
                    context.Database.ExecuteSqlCommand(query);*/
                    Console.WriteLine("Inserción exitosa");
                }
                return Constantes.OPERACION_EXITOSA;
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Error1", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return Constantes.ERROR_CONSULTA;
            }
            catch (EntityException ex)
            {
                MessageBox.Show(ex.Message, "Error2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return Constantes.ERROR_CONEXION_BD;
            }

        }
    }

    public partial class ServicioComunicacionSorrySliders : IMenuPrincipal
    {
        public (Constantes, string, byte[]) RecuperarDatosUsuario(string correoElectronico)
        {

            try
            {
                using (var context = new BaseDeDatosSorrySlidersEntities())
                {
                    var usuario = context.CuentaSet.FirstOrDefault(registro => registro.CorreoElectronico == correoElectronico);
                    if (usuario != null)
                    {
                        return (Constantes.OPERACION_EXITOSA, usuario.Nickname, usuario.Avatar);
                    }
                    else
                    {
                        return (Constantes.OPERACION_EXITOSA_VACIA, null, null);
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Error1", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (Constantes.ERROR_CONSULTA, null, null);
            }
            catch (EntityException ex)
            {
                MessageBox.Show(ex.Message, "Error2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (Constantes.ERROR_CONEXION_BD, null, null);
            }
        }
    }
}
