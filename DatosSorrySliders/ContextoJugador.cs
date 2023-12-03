using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DatosSorrySliders
{
    public class ContextoJugador
    {
        public string CorreoJugador { get; set; }
        public OperationContext ContextoJugadorCallBack { get; set; }
        public bool ListoParaTurnoSiguiente { get; set; }

        public ContextoJugador()
        {
            ListoParaTurnoSiguiente = false;
        }

        public override bool Equals(object obj)
        {
            if (obj is ContextoJugador jugador)
            {
                if (jugador.CorreoJugador == null || !jugador.CorreoJugador.Equals(CorreoJugador))
                {
                    return false;
                }

                return true;
            }
            return false;
        }
    }
}
