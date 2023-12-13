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
    public static class ManejarOperationContext
    {
        public static void AgregarOReemplazarJugadorContextoLista(Dictionary<string, List<ContextoJugador>> jugadoresEnLinea, ContextoJugador jugadorNuevo, string uid)
        {
            if (!jugadoresEnLinea.ContainsKey(uid))
            {
                jugadoresEnLinea.Add(uid, new List<ContextoJugador>());
            }
            int posicionJugador = DevolverPosicionCorreoJugador(jugadoresEnLinea[uid], jugadorNuevo.CorreoJugador);
            if (posicionJugador == -1)
            {
                jugadoresEnLinea[uid].Add(jugadorNuevo);
            }
            else
            {
                jugadoresEnLinea[uid][posicionJugador] = jugadorNuevo;
            }
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
            return "";
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

        /// <summary>
        /// Verifica que exista la llave en el diccionario, que el jugador exista y de ahí lo elimina. Al final, checa si el código ya no tiene jugadores, y si es así, lo elimina del diccionario
        /// </summary>
        /// <param name="diccionario"></param>
        /// <param name="codigoPartida"></param>
        /// <param name="contextoActual"></param>
        /// <returns></returns>
        public static string EliminarJugadorDiccionario(Dictionary<string, List<ContextoJugador>> diccionario, string codigoPartida, OperationContext contextoActual)
        {
            if (diccionario.ContainsKey(codigoPartida))
            {
                string jugadorAEliminar = DevolverCorreoJugador(diccionario[codigoPartida], contextoActual);
                EliminarJugadorLista(contextoActual, diccionario[codigoPartida]);

                if (diccionario[codigoPartida].Count == 0)
                {
                    diccionario.Remove(codigoPartida);
                }
                return jugadorAEliminar;
            }
            return "";
        }

        public static void EliminarKeyDiccionario(Dictionary<string, List<ContextoJugador>> diccionario, string uid) 
        {
            if (diccionario.ContainsKey(uid)) 
            {
                diccionario.Remove(uid);
            }
        }
        public static void EliminarJugadorDiccionarioPorCorreo(Dictionary<string, List<ContextoJugador>> diccionario, string codigoPartida, string correo)
        {
            if (diccionario.ContainsKey(codigoPartida))
            {
                int posicion = DevolverPosicionCorreoJugador(diccionario[codigoPartida], correo);
                if (posicion != -1)
                {
                    EliminarJugadorDiccionario(diccionario, codigoPartida, diccionario[codigoPartida][posicion].ContextoJugadorCallBack);
                }
            }
        }
    }
}
