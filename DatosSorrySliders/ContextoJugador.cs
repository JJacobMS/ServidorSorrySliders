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

        public override int GetHashCode()
        {
            int hashCode = 1790901596;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CorreoJugador);
            hashCode = hashCode * -1521134295 + EqualityComparer<OperationContext>.Default.GetHashCode(ContextoJugadorCallBack);
            hashCode = hashCode * -1521134295 + ListoParaTurnoSiguiente.GetHashCode();
            return hashCode;
        }
    }
}
