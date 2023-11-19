using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DatosSorrySliders
{
    [DataContract]
    public enum Constantes
    {
        [EnumMember]
        ERROR_CONEXION_BD,
        [EnumMember]
        ERROR_CONSULTA,
        [EnumMember]
        ERROR_CONEXION_SERVIDOR,
        [EnumMember]
        OPERACION_EXITOSA,
        [EnumMember]
        OPERACION_EXITOSA_VACIA,
        [EnumMember]
        ERROR_TIEMPO_ESPERA_SERVIDOR
    }
}
