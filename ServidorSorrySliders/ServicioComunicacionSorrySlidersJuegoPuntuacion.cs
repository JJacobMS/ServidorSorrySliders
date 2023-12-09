using DatosSorrySliders;
using InterfacesServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServidorSorrySliders
{
    public partial class ServicioComunicacionSorrySliders : IJuegoPuntuacion
    {
        private Dictionary<string, List<ContextoJugador>> _diccionarioPuntuacion = new Dictionary<string, List<ContextoJugador>>();  
        public void AgregarJugador(string uid, string correoElectronico)
        {
            CambiarSingle();
            ContextoJugador contextoJugador = new ContextoJugador()
            {
                CorreoJugador = correoElectronico,
                ContextoJugadorCallBack = OperationContext.Current
            };
            lock (_diccionarioPuntuacion) 
            {
                ManejarOperationContext.AgregarOReemplazarJugadorContextoLista(_diccionarioPuntuacion, contextoJugador, uid);
            }
            Console.WriteLine("Jugador agregado "+contextoJugador.CorreoJugador);
            CambiarMultiple();
        }

        public void EliminarJugador(string uid, string correoElectronico)
        {
            CambiarSingle();
            lock (_diccionarioPuntuacion)
            {
                Console.WriteLine("EliminarJugador " + correoElectronico);
                if (_diccionarioPuntuacion.ContainsKey(uid)) 
                {
                    int posicionJugador = ManejarOperationContext.DevolverPosicionCorreoJugador(_diccionarioPuntuacion[uid], correoElectronico);
                    _diccionarioPuntuacion[uid].RemoveAt(posicionJugador);
                }
            }
            CambiarMultiple();
            NotificarEliminarJugador(uid, correoElectronico);
        }
        private void NotificarEliminarJugador(string uid, string correoElectronico) 
        {
            Console.WriteLine("Notificar eliminacion");
            foreach (ContextoJugador contextoJugador in _diccionarioPuntuacion[uid])
            {
                contextoJugador.ContextoJugadorCallBack.GetCallbackChannel<IJuegoNotificacionCallback>().EliminarTurnoJugador(correoElectronico);
            }
        }

        public void NotificarCambioTurno(string uid)
        {
            foreach (ContextoJugador contextoJugador in _diccionarioPuntuacion[uid])
            {
                Logger log = new Logger(this.GetType(), "IJuegoPuntuacion");
                try
                {
                    contextoJugador.ContextoJugadorCallBack.GetCallbackChannel<IJuegoNotificacionCallback>().CambiarTurno();
                }
                catch (CommunicationObjectAbortedException ex)
                {
                    log.LogWarn("La conexión del usuario se ha perdido", ex);
                    EliminarJugador(uid, contextoJugador.CorreoJugador);
                }
                catch (TimeoutException ex)
                {
                    log.LogWarn("La conexión del usuario se ha perdido", ex);
                    EliminarJugador(uid, contextoJugador.CorreoJugador);
                }
                catch (InvalidCastException ex)
                {
                    log.LogWarn("el callback no pertenece a dicho contexto ", ex);
                }
            }
        }

        public void NotificarJugadores(string uid, string correoJugador, string nombrePeon, int puntosObtenidos)
        {
            Logger log = new Logger(this.GetType(), "IJuegoPuntuacion");
            lock (_diccionarioPuntuacion)
            {
                foreach (ContextoJugador contextoJugador in _diccionarioPuntuacion[uid])
                {
                    if (contextoJugador.CorreoJugador != correoJugador)
                    {
                        try
                        {
                            Console.WriteLine("Notificar a "+contextoJugador.CorreoJugador);
                            contextoJugador.ContextoJugadorCallBack.GetCallbackChannel<IJuegoNotificacionCallback>().NotificarJugadorMovimiento(nombrePeon, puntosObtenidos);   
                        }
                        catch (CommunicationObjectAbortedException ex)
                        {
                            log.LogWarn("La conexión del usuario se ha perdido", ex);
                            Console.WriteLine("ESTA DESCONECTADO " + contextoJugador.CorreoJugador);
                            EliminarJugador(uid, contextoJugador.CorreoJugador);
                        }
                        catch (TimeoutException ex)
                        {
                            log.LogWarn("La conexión del usuario se ha perdido", ex);
                            Console.WriteLine("ESTA DESCONECTADO " + contextoJugador.CorreoJugador);
                            EliminarJugador(uid, contextoJugador.CorreoJugador);
                        }
                        catch (InvalidCastException ex)
                        {
                            log.LogWarn("el callback no pertenece a dicho contexto ", ex);
                        }
                    }
                }
            }
        }

        public Constantes ActualizarGanador(string uid, string correoElectronico, int posicion)
        {
            Logger log = new Logger(this.GetType(), "IJuegoPuntuacion");
            try
            {
                int filasAfectadas = 0;
                using (var context = new BaseDeDatosSorrySlidersEntities())
                {
                    filasAfectadas = filasAfectadas + context.Database.ExecuteSqlCommand("UPDATE RelacionPartidaCuentaSet SET Posicion = @posicion where CodigoPartida = @codigoPartida AND CorreoElectronico=@correoElectronico;",
                    new SqlParameter("@posicion", posicion),
                    new SqlParameter("@codigoPartida", uid), 
                    new SqlParameter("@correoElectronico", correoElectronico));
                    if (filasAfectadas > 0)
                    {
                        return Constantes.OPERACION_EXITOSA;
                    }
                    else
                    {
                        return Constantes.OPERACION_EXITOSA_VACIA;
                    }

                }
            }
            catch (SqlException ex)
            {
                log.LogError("Error al ejecutar consulta SQL", ex);
                Console.WriteLine(ex.StackTrace);
                return Constantes.ERROR_CONSULTA;
            }
            catch (EntityException ex)
            {
                Console.WriteLine(ex.StackTrace);
                log.LogError("Error de conexión a la base de datos", ex);
                return Constantes.ERROR_CONEXION_BD;
            }
        }

        public void NotificarCambiarPagina(string uid)
        {
            Logger log = new Logger(this.GetType(), "IJuegoPuntuacion");
            Console.WriteLine("Diccionario" + uid);
            foreach (ContextoJugador contextoJugador in _diccionarioPuntuacion[uid])
            {
                try
                {
                    Console.WriteLine("CALLBACK" + contextoJugador.CorreoJugador);
                    contextoJugador.ContextoJugadorCallBack.GetCallbackChannel<IJuegoNotificacionCallback>().CambiarPagina();

                }
                catch (CommunicationObjectAbortedException ex)
                {
                    log.LogWarn("La conexión del usuario se ha perdido", ex);
                    Console.WriteLine("catch");
                }
                catch (TimeoutException ex)
                {
                    Console.WriteLine("catch");
                    log.LogWarn("La conexión del usuario se ha perdido", ex);
                }
                catch (InvalidCastException ex)
                {
                    Console.WriteLine("catch");
                    log.LogWarn("el callback no pertenece a dicho contexto ", ex);
                }
            }
        }

        public void EliminarDiccionariosJuego(string uid)
        {
            ManejarOperationContext.EliminarKeyDiccionario(_diccionarioPuntuacion, uid);
            ManejarOperationContext.EliminarKeyDiccionario(_jugadoresEnLineaChat, uid);
            ManejarOperationContext.EliminarKeyDiccionario(_jugadoresEnLineaJuegoLanzamiento, uid);

        }
    }
}
