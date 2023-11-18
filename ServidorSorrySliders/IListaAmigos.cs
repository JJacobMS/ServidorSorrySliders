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
        [OperationContract]
        (Constantes, List<NotificacionSet>) RecuperarNotificaciones(string correoElectronico);
        [OperationContract]
        (Constantes, List<CuentaSet>) RecuperarAmigos(string correoElectronico);
        [OperationContract]
        Constantes EliminarNotificacionJugador(string correoElectronico, int idNotificacion);
        [OperationContract]
        void NotificarInvitado(string correoElectronico);
        [OperationContract]
        Constantes GuardarAmistad(string correoElectronicoDestinatario, string correoElectronicoRemitente);
       

    }

    [ServiceContract(CallbackContract = typeof(INotificarJugadoresCallback))]
    public interface INotificarJugadores
    {
        [OperationContract]
        Constantes AgregarProxy(string correoElectronico);
        [OperationContract]
        void EliminarProxy(string correoElectronico);

        [OperationContract(IsOneWay = true)]
        void LlamarCallback(string correoElectronico);
        /*private void NotificarInvitado(correo)
         buscar en el diccionario para ver si lo encuentra y si lo encuentra
            llamar al callback
         */
    }

    [ServiceContract]
    public interface INotificarJugadoresCallback
    {
        [OperationContract(IsOneWay = true)]
        void RecuperarNotificacion();
    }
}
