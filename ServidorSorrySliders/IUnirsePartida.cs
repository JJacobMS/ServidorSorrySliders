using DatosSorrySliders;
using ServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace InterfacesServidorSorrySliders
{
    [ServiceContract]
    public interface IUnirsePartida
    {
        [OperationContract]
        (Constantes, int) UnirseAlLobby(string uid, string correoJugadorNuevo);

        [OperationContract]
        (Constantes, List<CuentaSet>) RecuperarJugadoresLobby(string uid);

        [OperationContract]
        (Constantes, PartidaSet) RecuperarPartida(string codigoPartida);
    }
}
