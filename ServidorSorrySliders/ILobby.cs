using DatosSorrySliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServidorSorrySliders
{
    [ServiceContract(CallbackContract = typeof(ILobbyCallback))]
    public interface ILobby
    {
        [OperationContract(IsOneWay = true)]
        void EntrarPartida(string uid);
        [OperationContract(IsOneWay = true)]
        void SalirPartida(string uid);
        [OperationContract(IsOneWay = true)]
        void ChatJuego(string uid, string nickname, string mensaje);

    }

    [ServiceContract]
    public interface ILobbyCallback
    {
        [OperationContract(IsOneWay = true)]
        void JugadorEntroPartida();
        [OperationContract(IsOneWay = true)]
        void JugadorSalioPartida();
        [OperationContract]
        void DevolverMensaje(string nickname, string mensaje);
    }
}

