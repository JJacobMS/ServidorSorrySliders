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
    public interface IMenuPrincipal
    {
        [OperationContract]
        (Constantes, string, byte[]) RecuperarDatosUsuario(string correoElectronico);

    }
}
