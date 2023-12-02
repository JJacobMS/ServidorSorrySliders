using DatosSorrySliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace InterfacesServidorSorrySliders
{
    [ServiceContract(CallbackContract = typeof(IJuegoNotificacionCallback))]
    public interface IJuegoPuntuacion
    {
        [OperationContract(IsOneWay = true)]
        void AgregarJugador(string uid, string correoElectronico);

        [OperationContract(IsOneWay = true)]
        void EliminarJugador(string uid, string correoElectronico);

        [OperationContract(IsOneWay = true)]
        void NotificarJugadores(string uid, string correoJugador, string nombrePeon, int puntosObtenidos);
        [OperationContract(IsOneWay = true)]
        void NotificarCambioTurno(string uid);
        [OperationContract]
        Constantes ActualizarGanador(string uid, string correoElectronico, int posicion);
        [OperationContract(IsOneWay = true)]
        void NotificarCambiarPagina(string uid);
    }

    [ServiceContract]
    public interface IJuegoNotificacionCallback
    {
        [OperationContract]
        void NotificarJugadorMovimiento(string nombrePeon, int puntosObtenidos);
        [OperationContract]
        void CambiarTurno();
        [OperationContract]
        void EliminarTurnoJugador(string correoElectronico);
        [OperationContract]
        void CambiarPagina();
    }
}
