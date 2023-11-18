using DatosSorrySliders;
using ServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace InterfacesServidorSorrySliders
{
    [ServiceContract]
    public interface IPuntuacion
    {
        [OperationContract]
        (Constantes, List<Puntuacion>) RecuperarPuntuaciones();
    }
}
