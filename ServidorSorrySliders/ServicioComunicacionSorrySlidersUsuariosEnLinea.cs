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
                int posicionJugador = RecuperarPosicionJugadorEnLinea(jugadorCorreo);
                if (posicionJugador == -1)
                {
                    _listaContextoJugadores.Add(jugadorNuevo);
                    Console.WriteLine("Entró al sistema " + jugadorCorreo);
                }
                else
                {
                    _listaContextoJugadores[posicionJugador] = jugadorNuevo;
                    Console.WriteLine("Cambio de contexto " + jugadorCorreo);
                }
                
                
            }
            CambiarMultiple();
        }


        public void SalirDelSistema(string jugadorCorreo)
        {
            CambiarSingle();
            lock (_listaContextoJugadores)
            {
                int posicionJugador = RecuperarPosicionJugadorEnLinea(jugadorCorreo);
                if (posicionJugador != -1) 
                { 
                    _listaContextoJugadores.RemoveAt(posicionJugador);
                    Console.WriteLine("Salio del sistema " + jugadorCorreo);
                }
            }
            CambiarMultiple();
        }

        private int RecuperarPosicionJugadorEnLinea(string jugadorCorreo)
        {
            for (int i = 0; i < _listaContextoJugadores.Count; i++)
            {
                if (_listaContextoJugadores[i].CorreoJugador.Equals(jugadorCorreo))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
