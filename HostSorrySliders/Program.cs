using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ServidorSorrySliders;

namespace HostSorrySliders
{
    public static class Program
    {
        static void Main(string[] args)
        {
            ObtenerCadenaConexionBaseDatos();
            Logger log = new Logger(typeof(Program));
            try
            {
                using (ServiceHost host = new ServiceHost(typeof(ServidorSorrySliders.ServicioComunicacionSorrySliders)))
                {
                    host.Open();
                    Console.WriteLine("Servidor en línea");
                    Console.ReadLine();
                }
            }
            catch (AddressAccessDeniedException ex) 
            {
                log.LogError("No se cuentan con los permisos necesarios en el servidor", ex);
            }
            catch (Exception ex)
            {
                log.LogFatal("Ha ocurrido un error inesperado", ex);
                Console.ReadLine();
            }
        }

        public static void ObtenerCadenaConexionBaseDatos() 
        {

            string connectionString = Environment.GetEnvironmentVariable("VARIABLEENTORNO");
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var connectionStringSection = config.ConnectionStrings.ConnectionStrings["BaseDeDatosSorrySlidersEntities"];

            if (connectionStringSection != null)
            {

                connectionStringSection.ConnectionString = connectionString;

                config.Save(ConfigurationSaveMode.Modified);

                ConfigurationManager.RefreshSection("connectionStrings");

            }


        }
    }
}
