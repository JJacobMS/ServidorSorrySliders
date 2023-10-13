using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatosSorrySliders
{
    public class Utilidades
    {
        public static string ConvertirArrayByteString(byte[] bytes)
        {
            var stringBuilder = new StringBuilder("0x");
            foreach (var byteArreglo in bytes)
            {
                stringBuilder.Append(byteArreglo);
            }
            return stringBuilder.ToString();
            //string cadena = Encoding.UTF8.GetString(bytes);

        }
    }
}
