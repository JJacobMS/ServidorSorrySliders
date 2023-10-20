using DatosSorrySliders;
using InterfacesServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
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
                Debug.WriteLine(ex.ToString());
                return (Constantes.ERROR_CONSULTA, null);
            }
            catch (EntityException ex)
            {
                Console.WriteLine(ex.ToString());
                return (Constantes.ERROR_CONEXION_BD, null);
            }
        }

    }
}
