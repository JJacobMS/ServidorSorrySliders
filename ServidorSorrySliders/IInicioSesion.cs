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
    public interface IInicioSesion
    {
        [OperationContract]
        Constantes VerificarExistenciaCorreoCuenta(string correoElectronico);

        [OperationContract]
        Constantes VerificarContrasenaDeCuenta(CuentaSet cuentaPorVerificar);
        [OperationContract]
        Constantes JugadorEstaEnLinea(string jugadorCorreo);
    }
}
