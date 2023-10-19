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
        void EntrarPartida(string uid, string correoJugadorNuevo);

        [OperationContract(IsOneWay = true)]
        void SalirPartida(string uid);
        
    }

    [ServiceContract]
    public interface ILobbyCallback
    {
        [OperationContract(IsOneWay = true)]
        void JugadorEntroPartida(List<CuentaSet> listaJugadores);

        [OperationContract(IsOneWay = true)]
        void JugadorSalioPartida();
    }
}

