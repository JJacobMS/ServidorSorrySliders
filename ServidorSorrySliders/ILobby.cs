using DatosSorrySliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServidorSorrySliders
{
    [ServiceContract]
    public interface ILobby
    {
        [OperationContract(IsOneWay = true)]
        void EntrarPartida(string uid, string correoJugadorNuevo);
        [OperationContract]
        (Constantes, List<CuentaSet>) UnirseAlLobby(string uid, string correoJugadorNuevo);
    }

    [ServiceContract]
    public interface ILobbyCallback
    {
        [OperationContract(IsOneWay = true)]
        void JugadorEntroPartida(List<CuentaSet> listaJugadores);
    }
}


// Crear un uid cuando crea 
// EntrarPartida Cliente -> Servidor (One way)
// JugadorEntroPartida Servidor -> Cliente (One way) Callback
