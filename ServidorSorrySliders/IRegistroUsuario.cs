using DatosSorrySliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace InterfacesServidorSorrySliders
{
    [ServiceContract]
    public interface IRegistroUsuario
    {
        [OperationContract]
        Constantes AgregarUsuario(UsuarioSet usuarioPorGuardar, CuentaSet cuentaPorGuardar);
    }
}
