﻿using DatosSorrySliders;
using InterfacesServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServidorSorrySliders
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public partial class ServicioComunicacionSorrySliders : ILobby
    {
        Dictionary<string, List<OperationContext>> _jugadoresEnLinea = new Dictionary<string, List<OperationContext>>();
        public void EntrarPartida(string uid)
        {
            if (!_jugadoresEnLinea.ContainsKey(uid))
            {
                _jugadoresEnLinea.Add(uid, new List<OperationContext>());
            }

            if (!ExisteOperationContextEnLista(OperationContext.Current, _jugadoresEnLinea[uid]))
            {
                _jugadoresEnLinea[uid].Add(OperationContext.Current);
            }

            List<OperationContext> contextosExistentes = _jugadoresEnLinea[uid];
            foreach (OperationContext operationContextJugador in _jugadoresEnLinea[uid])
            {
                operationContextJugador.GetCallbackChannel<ILobbyCallback>().JugadorEntroPartida();
            }
        }

        public void SalirPartida(string uid)
        {
            if(_jugadoresEnLinea.ContainsKey(uid))
            {
                List<OperationContext> contextosExistentes = _jugadoresEnLinea[uid];
                if (ExisteOperationContextEnLista(OperationContext.Current, _jugadoresEnLinea[uid]))
                {
                    EliminarContextDeLista(OperationContext.Current, _jugadoresEnLinea[uid]);
                    foreach (OperationContext operationContextJugador in _jugadoresEnLinea[uid])
                    {
                        operationContextJugador.GetCallbackChannel<ILobbyCallback>().JugadorEntroPartida();
                    }
                }
            }
        }

        private bool ExisteOperationContextEnLista(OperationContext contextoNuevo, List<OperationContext> contextosExistentes)
        {
            for (int i = 0; i < contextosExistentes.Count; i++)
            {
                if (contextoNuevo.SessionId.Equals(contextosExistentes[i].SessionId))
                {
                    return true;
                }
            }
            return false;
        }

        private void EliminarContextDeLista(OperationContext contextoAEliminar, List<OperationContext> contextosExistentes)
        {
            for (int i = contextosExistentes.Count - 1; i >= 0; i--)
            {
                OperationContext operationContextJugador = contextosExistentes[i];
                if (contextoAEliminar.SessionId.Equals(operationContextJugador.SessionId))
                {
                    contextosExistentes.RemoveAt(i);
                }
            }
        }

    }
}
