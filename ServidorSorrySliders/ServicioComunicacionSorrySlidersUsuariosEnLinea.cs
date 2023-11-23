using DatosSorrySliders;
using InterfacesServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServidorSorrySliders
{
    public partial class ServicioComunicacionSorrySliders : IUsuariosEnLinea
    {
        private List<ContextoJugador> _listaContextoJugadores = new List<ContextoJugador>();
        public void EntrarConCuenta(string jugadorCorreo)
        {
            ContextoJugador jugadorNuevo = new ContextoJugador { CorreoJugador = jugadorCorreo, ContextoJugadorCallBack = OperationContext.Current };
            
            CambiarSingle();
            lock (_listaContextoJugadores)
            {
                _listaContextoJugadores.Add(jugadorNuevo);
                Console.WriteLine("Entró al sistema " + jugadorCorreo);
            }
            CambiarMultiple();
        }

        public void SalirDelSistema(string jugadorCorreo)
        {
            CambiarSingle();
            lock (_listaContextoJugadores)
            {
                for(int i= 0; i < _listaContextoJugadores.Count; i++)
                {
                    if (_listaContextoJugadores[i].ContextoJugadorCallBack.SessionId.Equals(OperationContext.Current.SessionId))
                    {
                        _listaContextoJugadores.RemoveAt(i);
                        Console.WriteLine("Salio del sistema " + jugadorCorreo);
                        break;
                    }
                }
            }
            CambiarMultiple();
        }
    }
}
