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
            ContextoJugador jugadorNuevo = new ContextoJugador 
            { 
                CorreoJugador = jugadorCorreo, 
                ContextoJugadorCallBack = OperationContext.Current 
            };
            CambiarSingle();
            lock (_listaContextoJugadores)
            {
                int posicionJugador = RecuperarPosicionJugadorEnLinea(jugadorCorreo);
                if (posicionJugador == -1)
                {
                    _listaContextoJugadores.Add(jugadorNuevo);
                }
                else
                {
                    _listaContextoJugadores[posicionJugador] = jugadorNuevo;
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

        public void SalirJuegoCompleto(string uid, string correo)
        {
            lock (_diccionarioPuntuacion)
            {
                ManejarOperationContext.EliminarJugadorDiccionarioPorCorreo(_diccionarioPuntuacion, uid, correo);
            }
            lock (_jugadoresEnLineaJuegoLanzamiento)
            {
                ManejarOperationContext.EliminarJugadorDiccionarioPorCorreo(_jugadoresEnLineaJuegoLanzamiento, uid, correo);
            }
            lock (_jugadoresEnLineaChat)
            {
                ManejarOperationContext.EliminarJugadorDiccionarioPorCorreo(_jugadoresEnLineaChat, uid, correo);
            }
        }

        public bool ComprobarMismoJugadorConectado(string jugadorCorreo)
        {
            lock (_listaContextoJugadores)
            {
                if(_listaContextoJugadores.Exists(jugador => jugador.ContextoJugadorCallBack.SessionId.Equals(OperationContext.Current) && jugador.CorreoJugador.Equals(jugadorCorreo)))
                {
                    return true;
                }
                return false;
            }
        }
    }
}
