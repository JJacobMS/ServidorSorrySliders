using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ServidorSorrySliders;

namespace HostSorrySliders
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (ServiceHost host = new ServiceHost(typeof(ServidorSorrySliders.ServicioComunicacionSorrySliders)))
                {
                    host.Open();
                    Console.WriteLine("Server is running");
                    Console.ReadLine();
                }
            }
            catch (AddressAccessDeniedException ex) 
            {
                Console.WriteLine("No se cuentan con los permisos necesarios en el servidor \n"+ex.StackTrace);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ha ocurrido un error con el servidor \n"+ex.StackTrace);
            }
        }
    }
}
