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
    public interface IListaAmigos
    {
        [OperationContract]
        (Constantes, List<CuentaSet>) RecuperarAmigosCuenta(string correoElectronico);

        [OperationContract]
        (Constantes, List<CuentaSet>) RecuperarJugadoresCuenta(string informacionJugador);
        [OperationContract]
        (Constantes, List<TipoNotificacion>) RecuperarTipoNotificacion();
        [OperationContract]
        Constantes GuardarNotificacion(NotificacionSet notificacion);
    }

    [ServiceContract(CallbackContract = typeof(ICallbackNotificarJugadores))]
    public interface INotificarJugadores
    {
        [OperationContract(IsOneWay = true)]
        void NotificarJugadorInvitado(string correoElectronico);
    }

    [ServiceContract]
    public interface ICallbackNotificarJugadores
    {
        [OperationContract(IsOneWay = true)]
        void RecuperarNotificacion();
    }
}
