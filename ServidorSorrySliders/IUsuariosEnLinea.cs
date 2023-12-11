using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace InterfacesServidorSorrySliders
{
    [ServiceContract(CallbackContract = typeof(IUsuarioEnLineaCallback))]
    public interface IUsuariosEnLinea
    {
        [OperationContract(IsOneWay = true)]
        void EntrarConCuenta(string jugadorCorreo);
        [OperationContract(IsOneWay = true)]
        void SalirDelSistema(string jugadorCorreo);
        [OperationContract(IsOneWay = true)]
        void SalirJuegoCompleto(string uid, string correo);
    }

    [ServiceContract]
    public interface IUsuarioEnLineaCallback
    {
        [OperationContract(IsOneWay = true)]
        void ComprobarJugador();
    }
}
