﻿using System;
using System.Collections.Generic;
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
                Console.WriteLine("No se cuentan con los permisos necesarios en el servidor \n"+ex.StackTrace);
            }
            catch (Exception ex)
            {
                log.LogFatal("Ha ocurrido un error inesperado", ex);
                Console.WriteLine("Ha ocurrido un error con el servidor \n"+ex.StackTrace);
                Console.ReadLine();
            }
        }
    }
}
