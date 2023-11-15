using DatosSorrySliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace InterfacesServidorSorrySliders
{
    [ServiceContract(CallbackContract = typeof(ICallbackListaAmigos))]
    public interface IListaAmigos
    {
        [OperationContract]
        (Constantes, List<CuentaSet>) RecuperarAmigosCuenta(string correoElectronico);

        [OperationContract]
        (Constantes, List<CuentaSet>) RecuperarJugadoresCuenta(string informacionJugador);
        [OperationContract]
        (Constantes, List<TipoNotificacion>) RecuperarTipoNotificacion();
        [OperationContract]
        Constantes GuardarNotificacion(string correoRemitente, string correoDestinatario);
    }
    [ServiceContract]
    public interface ICallbackListaAmigos 
    {
        [OperationContract(IsOneWay = true)]
        void InvitarAmigo(string correo, string uid);
    }
}
