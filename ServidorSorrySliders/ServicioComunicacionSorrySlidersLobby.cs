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
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class ServicioComunicacionSorrySliders : ILobby
    {
        private Dictionary<string, List<OperationContext>> _jugadoresEnLinea = new Dictionary<string, List<OperationContext>>();
        public void EntrarPartida(string uid)
        {
            Logger log = new Logger(this.GetType(), "ILobby");
            if (!_jugadoresEnLinea.ContainsKey(uid))
            {
                _jugadoresEnLinea.Add(uid, new List<OperationContext>());
            }

            if (!ExisteOperationContextEnLista(OperationContext.Current, _jugadoresEnLinea[uid]))
            {
                _jugadoresEnLinea[uid].Add(OperationContext.Current);
            }
            foreach (OperationContext operationContextJugador in _jugadoresEnLinea[uid])
            {
                try 
                {
                    operationContextJugador.GetCallbackChannel<ILobbyCallback>().JugadorEntroPartida();
                }
                catch (CommunicationObjectAbortedException ex) 
                {
                    Console.WriteLine("Ha ocurrido un error en el callback \n"+ex.StackTrace);
                    log.LogWarn("La conexión del usuario se ha perdido", ex);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    log.LogFatal("Ha ocurrido un error inesperado", ex);
                }
            }
        }

        public void SalirPartida(string uid)
        {
            Logger log = new Logger(this.GetType(), "ILobby");
            if (_jugadoresEnLinea.ContainsKey(uid))
            {

                if (ExisteOperationContextEnLista(OperationContext.Current, _jugadoresEnLinea[uid]))
                {
                    EliminarContextDeLista(OperationContext.Current, _jugadoresEnLinea[uid]);


                    if (_jugadoresEnLinea[uid].Count > 0)
                    {

                        foreach (OperationContext operationContextJugador in _jugadoresEnLinea[uid])
                        {
                            try
                            {
                                operationContextJugador.GetCallbackChannel<ILobbyCallback>().JugadorEntroPartida();

                            }
                            catch (CommunicationObjectAbortedException ex)
                            {
                                Console.WriteLine("Ha ocurrido un error en el callback \n" + ex.StackTrace);
                                log.LogWarn("La conexión del usuario se ha perdido", ex);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace);
                                log.LogFatal("Ha ocurrido un error inesperado", ex);
                            }
                        }
                    }
                    else
                    {
                        EliminarPartida(uid);
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
            Logger log = new Logger(this.GetType(), "ILobby");
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
                log.LogError("Error al ejecutar consulta SQL", ex);
            }
            catch (EntityException ex)
            {
                log.LogError("Error de conexión a la base de datos", ex);
                Console.WriteLine(ex.StackTrace);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                log.LogFatal("Ha ocurrido un error inesperado", ex);
            }
        }

        public void ChatJuego(string uid, string nickname, string mensaje)
        {
            Logger log = new Logger(this.GetType(), "IJuego");
            Console.WriteLine("Mensaje original en servidor" +" nickname "+nickname+"  men "+mensaje);
            

            foreach (OperationContext contexto in _jugadoresEnLinea[uid])
            {
                try
                {
                    contexto.GetCallbackChannel<ILobbyCallback>().DevolverMensaje(nickname, mensaje);
                }
                catch (CommunicationObjectAbortedException ex)
                {
                    Console.WriteLine("Ha ocurrido un error en el callback \n" + ex.StackTrace);
                    log.LogWarn("La conexión del usuario se ha perdido", ex);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    log.LogFatal("Ha ocurrido un error inesperado", ex);
                }
            }
        }
    }
}
