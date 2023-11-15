using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServidorSorrySliders
{
    [ServiceContract(CallbackContract = typeof(IChatCallback))]
    public interface IChat
    {
        [OperationContract(IsOneWay = true)]
        void ChatJuego(string uid, string nickname, string mensaje);
        [OperationContract(IsOneWay = true)]
        void IngresarAlChat(string uid);
    }

    [ServiceContract]
    public interface IChatCallback
    {
        [OperationContract]
        void DevolverMensaje(string nickname, string mensaje);
    }
}
