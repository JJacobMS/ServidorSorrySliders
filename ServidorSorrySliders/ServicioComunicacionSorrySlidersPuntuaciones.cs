using DatosSorrySliders;
using InterfacesServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorSorrySliders
{
    public partial class ServicioComunicacionSorrySliders : IPuntuacion
    {
        public (Constantes, List<Puntuacion>) RecuperarPuntuaciones()
        {
            Logger log = new Logger(this.GetType(), "IInicioSesion");
            try
            {
                using (var context = new BaseDeDatosSorrySlidersEntities())
                {
                    var puntuaciones = context.Database.SqlQuery<Puntuacion>("SELECT TOP(5) CuentaSet.Nickname, Count(Posicion) AS NumeroPartidasGanadas FROM RelacionPartidaCuentaSET " +
                    "Inner Join CuentaSet on CuentaSet.CorreoElectronico = RelacionPartidaCuentaSET.CorreoElectronico " +
                    "where RelacionPartidaCuentaSET.CorreoElectronico = RelacionPartidaCuentaSET.CorreoElectronico AND Posicion = 0 GROUP BY CuentaSet.Nickname ORDER BY NumeroPartidasGanadas DESC;").ToList();
                    if (puntuaciones == null)
                    {
                        return (Constantes.OPERACION_EXITOSA_VACIA, null);
                    }
                    else
                    {
                        return (Constantes.OPERACION_EXITOSA, puntuaciones);

                    }
                }
            }
            catch (SqlException ex)
            {
                log.LogError("Error al ejecutar consulta SQL", ex);
                Console.WriteLine(ex.ToString());
                return (Constantes.ERROR_CONSULTA, null);
            }
            catch (EntityException ex)
            {
                log.LogError("Error de conexión a la base de datos", ex);
                Console.WriteLine(ex.ToString());
                return (Constantes.ERROR_CONEXION_BD, null);
            }
        }
    }
}
