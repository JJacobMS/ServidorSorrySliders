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
        void EntrarPartida(string uid, string jugadorCorreo);
        [OperationContract(IsOneWay = true)]
        void SalirPartida(string uid);
        [OperationContract(IsOneWay =true)]
        void IniciarPartida(string uid);
        [OperationContract]
        void ComprobarJugadoresExistentes(string uid);

    }

    [ServiceContract]
    public interface ILobbyCallback
    {
        [OperationContract(IsOneWay = true)]
        void JugadorEntroPartida();
        [OperationContract(IsOneWay = true)]
        void JugadorSalioPartida();
        [OperationContract(IsOneWay = true)]
        void HostInicioPartida();
        [OperationContract(IsOneWay = true)]
        void ComprobarJugadorLobby();
    }
}

