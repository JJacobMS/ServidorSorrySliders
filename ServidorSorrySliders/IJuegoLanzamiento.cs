using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace InterfacesServidorSorrySliders
{
    [ServiceContract]
    public interface IJuegoLanzamiento
    {
        [OperationContract(IsOneWay = true)]
        void AgregarJugadorJuegoLanzamiento();
        [OperationContract(IsOneWay = true)]
        void EliminarJugadorJuegoLanzamiento();
    }
}
