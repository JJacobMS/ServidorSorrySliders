using DatosSorrySliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServidorSorrySliders
{
    public class ManejarOperationContext
    {
        public static bool AgregarJugadorContextoLista(Dictionary<string, List<ContextoJugador>> jugadoresEnLinea, ContextoJugador jugadorNuevo, string uid)
        {
            if (!jugadoresEnLinea.ContainsKey(uid))
            {
                jugadoresEnLinea.Add(uid, new List<ContextoJugador>());
            }

            if (!ExisteJugadorEnLista(jugadorNuevo.ContextoJugadorCallBack, jugadoresEnLinea[uid]))
            {
                jugadoresEnLinea[uid].Add(jugadorNuevo);
                return true;
            }
            return false;
        }
        public static bool ExisteJugadorEnLista(OperationContext contextoNuevo, List<ContextoJugador> jugadoresActuales)
        {
            for (int i = 0; i < jugadoresActuales.Count; i++)
            {
                if (contextoNuevo.SessionId.Equals(jugadoresActuales[i].ContextoJugadorCallBack.SessionId))
                {
                    return true;
                }
            }
            return false;
        }
        public static void EliminarJugadorLista(OperationContext contextoAEliminar, List<ContextoJugador> jugadoresExistentes)
        {
            for (int i = jugadoresExistentes.Count - 1; i >= 0; i--)
            {
                OperationContext operationContextJugador = jugadoresExistentes[i].ContextoJugadorCallBack;
                if (contextoAEliminar.SessionId.Equals(operationContextJugador.SessionId))
                {
                    jugadoresExistentes.RemoveAt(i);
                    //¿Si se eliminan todos entonces se debería eliminar el registro del diccionario?
                    return;
                }
            }
        }

        public static string DevolverCorreoJugador(List<ContextoJugador> jugadoresExistentes, OperationContext jugadorActual)
        {
            for (int i = 0; i < jugadoresExistentes.Count; i++)
            {
                if (jugadorActual.SessionId.Equals(jugadoresExistentes[i].ContextoJugadorCallBack.SessionId))
                {
                    return jugadoresExistentes[i].CorreoJugador;
                }
            }
            return null;
        }
        public static int DevolverPosicionCorreoJugador(List<ContextoJugador> jugadoresExistentes, string correo)
        {
            for (int i = 0; i < jugadoresExistentes.Count; i++)
            {
                if (correo.Equals(jugadoresExistentes[i].CorreoJugador))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
