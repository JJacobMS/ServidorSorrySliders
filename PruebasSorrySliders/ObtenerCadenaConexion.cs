using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebasSorrySliders
{
    public static class ObtenerCadenaConexion
    {
        public static void ObtenerCadenaConexionBaseDatos()
        {

            string connectionString = Environment.GetEnvironmentVariable("VARIABLEENTORNO");
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var connectionStringSection = config.ConnectionStrings.ConnectionStrings["BaseDeDatosSorrySlidersEntities"];

            if (connectionStringSection != null)
            {
                Console.WriteLine($"La cadena de conexión  es: {connectionString}");

                connectionStringSection.ConnectionString = connectionString;

                config.Save(ConfigurationSaveMode.Modified);

                ConfigurationManager.RefreshSection("connectionStrings");

                string updatedConnectionString = ConfigurationManager.ConnectionStrings["BaseDeDatosSorrySlidersEntities"].ConnectionString;

                Console.WriteLine($"La cadena de conexión actualizada es: {updatedConnectionString}");

            }


        }
    }
}
