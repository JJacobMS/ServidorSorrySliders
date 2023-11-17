using ServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace InterfacesServidorSorrySliders
{
    [ServiceContract(CallbackContract = typeof(IJuegoLanzamientoCallback))]
    public interface IJuegoLanzamiento
    {
        [OperationContract(IsOneWay = true)]
        void AgregarJugadorJuegoLanzamiento(string codigoPartida, string correoElectronico);
        [OperationContract(IsOneWay = true)]
        void EliminarJugadorJuegoLanzamiento(string codigoPartida);
        [OperationContract(IsOneWay = true)]
        void NotificarLanzamientoDado(string codigoPartida, string correo, int numeroDado);
        [OperationContract(IsOneWay = true)]
        void NotificarLanzamientoLinea(string codigoPartida, string correo, double posicionX, double posicionY);
        [OperationContract(IsOneWay = true)]
        void NotificarFinalizarLanzamiento(string codigoPartida, string correo);
    }

    [ServiceContract]
    public interface IJuegoLanzamientoCallback
    {
        [OperationContract(IsOneWay = true)]
        void JugadorTiroDado(int numeroDado);
        [OperationContract(IsOneWay = true)]
        void JugadorDetuvoLinea(double posicionX, double posicionY);
        [OperationContract(IsOneWay = true)]
        void JugadorSalioJuegoLanzamiento(string correoElectronicoSalido);
        [OperationContract(IsOneWay = true)]
        void JugadoresListosParaSiguienteTurno();
    }
}
