using DatosSorrySliders;
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
using System.Windows.Forms;

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
            try {

                foreach (OperationContext operationContextJugador in _jugadoresEnLinea[uid])
                {
                    operationContextJugador.GetCallbackChannel<ILobbyCallback>().JugadorEntroPartida();
                }
            }
            catch (CommunicationObjectAbortedException ex) 
            {
                Console.WriteLine("Ha ocurrido un error en el callback \n"+ex.StackTrace);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);                
            }

        }

        public void SalirPartida(string uid)
        {
            if(_jugadoresEnLinea.ContainsKey(uid))
            {

                if (ExisteOperationContextEnLista(OperationContext.Current, _jugadoresEnLinea[uid]))
                {
                    try
                    {
                        EliminarContextDeLista(OperationContext.Current, _jugadoresEnLinea[uid]);
                        Console.WriteLine(_jugadoresEnLinea[uid].Count);
                        if (_jugadoresEnLinea[uid].Count > 0)
                        {
                            foreach (OperationContext operationContextJugador in _jugadoresEnLinea[uid])
                            {
                                operationContextJugador.GetCallbackChannel<ILobbyCallback>().JugadorEntroPartida();
                            }
                        }
                        else
                        {
                            EliminarPartida(uid);
                        }
                        
                    }
                    catch (CommunicationObjectAbortedException ex) 
                    {
                        Console.WriteLine("Ha ocurrido un error en el callback \n"+ex.StackTrace);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);

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
                    return;
                }
            }
        }

        private void EliminarPartida(string uid)
        {
            try
            {
                using (var context = new BaseDeDatosSorrySlidersEntities())
                {
                    var filasAfectadas = context.Database.ExecuteSqlCommand(
                        "DELETE FROM PartidaSet WHERE CodigoPartida = @codigo",
                        new SqlParameter("@codigo", uid));

                    if (filasAfectadas > 0)
                    {
                        Console.WriteLine("Partida Eliminada " + uid);
                    }
                    else
                    {
                        Console.WriteLine("Error, ninguna partida se eliminó");
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            catch (EntityException ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

    }
}
