using PruebasSorrySliders.ServidorComunicacionSorrySlidersPrueba;
using ServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PruebasSorrySliders
{
    public class LlamadaCallBackUsuarioEnLineaPruebas : IDisposable
    {
        private static UsuariosEnLineaClient _proxyUsuarioEnLinea;
        private static ImplementacionPruebaCallbackUsuarioEnLinea _implementacionCallbackUsuario;

        public LlamadaCallBackUsuarioEnLineaPruebas()
        {
            _implementacionCallbackUsuario = new ImplementacionPruebaCallbackUsuarioEnLinea();
            _proxyUsuarioEnLinea = new UsuariosEnLineaClient(new InstanceContext(_implementacionCallbackUsuario));
        }

        public void Dispose()
        {
            _proxyUsuarioEnLinea = null;
            _implementacionCallbackUsuario = null;
        }

        public void LlamadaCallBackEntrarSistemaExitosaPrueba()
        {

        }
    }

    public class ImplementacionPruebaCallbackUsuarioEnLinea : IUsuariosEnLineaCallback
    {
        public void ComprobarJugador()
        {
            throw new NotImplementedException();
        }
    }
}
