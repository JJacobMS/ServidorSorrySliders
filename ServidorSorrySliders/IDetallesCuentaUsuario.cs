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
    public interface IDetallesCuentaUsuario
    {
        [OperationContract]
        (Constantes, UsuarioSet) RecuperarDatosUsuarioDeCuenta(string correoElectronico);

        [OperationContract]
        Constantes VerificarContrasenaActual(CuentaSet cuenta);

        [OperationContract]
        Constantes CambiarContrasena(CuentaSet cuenta);
    }
}
