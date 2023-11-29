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
        void NotificarJugadores(string uid, string nombrePeon, int movimientoCanvas);
    }

    [ServiceContract]
    public interface IJuegoNotificacionCallback
    {
        [OperationContract]
        void NotificarJugadorMovimiento(string uid, string nombrePeon, int movimientoCanvas);
    }

}
