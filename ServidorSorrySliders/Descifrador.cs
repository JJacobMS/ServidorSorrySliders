using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServidorSorrySliders
{
    public static class Descifrador
    {
        public static string Descrifrar(string textoCifrado)
        {
            byte[] datosEncriptados = Convert.FromBase64String(textoCifrado);
            byte[] datosDescifrados = ProtectedData.Unprotect(datosEncriptados, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(datosDescifrados);
        }
    }
}
