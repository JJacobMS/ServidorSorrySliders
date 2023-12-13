using DatosSorrySliders;
using InterfacesServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServidorSorrySliders
{
    public partial class ServicioComunicacionSorrySliders : IInicioSesion
    {
        public Constantes JugadorEstaEnLinea(string jugadorCorreo)
        {
            Logger log = new Logger(this.GetType(), "IInicioSesion");
            CambiarSingle();
            lock (_listaContextoJugadores)
            {
                for (int i = 0; i < _listaContextoJugadores.Count; i++)
                {
                    if (_listaContextoJugadores[i].CorreoJugador.Equals(jugadorCorreo))
                    {
                        try
                        {
                            _listaContextoJugadores[i].ContextoJugadorCallBack.GetCallbackChannel<IUsuarioEnLineaCallback>().ComprobarJugador();
                        }
                        catch (CommunicationException ex)
                        {
                            log.LogWarn("La conexión del usuario se ha perdido", ex);
                            SalirDelSistema(jugadorCorreo);
                            CambiarMultiple();
                            return Constantes.OPERACION_EXITOSA_VACIA;
                        }
                        catch (TimeoutException ex)
                        {
                            log.LogWarn("La conexión del usuario se ha perdido", ex);
                            SalirDelSistema(jugadorCorreo);
                            CambiarMultiple();
                            return Constantes.OPERACION_EXITOSA_VACIA;
                        }
                        CambiarMultiple();
                        return Constantes.OPERACION_EXITOSA;
                    }
                }
                CambiarMultiple();
                return Constantes.OPERACION_EXITOSA_VACIA;
            }
        }

        public Constantes VerificarContrasenaDeCuenta(CuentaSet cuentaPorVerificar)
        {
            Logger log = new Logger(this.GetType(), "IInicioSesion");
            try
            {
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    var cuentaVerificada = contexto.Database.SqlQuery<string>
                        ("SELECT CorreoElectronico From CuentaSet WHERE HASHBYTES('SHA2_512', @contrasena) = Contraseña AND CorreoElectronico = @correo",
                        new SqlParameter("@contrasena", cuentaPorVerificar.Contraseña), new SqlParameter("@correo", cuentaPorVerificar.CorreoElectronico)).
                    FirstOrDefault();

                    if (cuentaVerificada == null)
                    {
                        return Constantes.OPERACION_EXITOSA_VACIA;
                    }

                    return Constantes.OPERACION_EXITOSA;
                }
            }
            catch (SqlException ex)
            {
                log.LogError("Error al ejecutar consulta SQL", ex);
                return Constantes.ERROR_CONSULTA;
            }
            catch (EntityException ex)
            {
                log.LogError("Error de conexión a la base de datos", ex);
                return Constantes.ERROR_CONEXION_BD;
            }
        }

        public Constantes VerificarExistenciaCorreoCuenta(string correoElectronico)
        {
            Logger log = new Logger(this.GetType(), "IInicioSesion");
            try
            {
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    CuentaSet cuentaVerificada = contexto.CuentaSet.
                    Where(cuenta => cuenta.CorreoElectronico == correoElectronico).FirstOrDefault();

                    if (cuentaVerificada == null)
                    {
                        return Constantes.OPERACION_EXITOSA_VACIA;
                    }

                    return Constantes.OPERACION_EXITOSA;
                }
            }
            catch (SqlException ex)
            {
                log.LogError("Error al ejecutar consulta SQL", ex);
                return Constantes.ERROR_CONSULTA;
            }
            catch (EntityException ex)
            {
                log.LogError("Error de conexión a la base de datos", ex);
                return Constantes.ERROR_CONEXION_BD;
            }
        }
    }

    public partial class ServicioComunicacionSorrySliders : IMenuPrincipal
    {
        public (Constantes, string, byte[]) RecuperarDatosUsuario(string correoElectronico)
        {
            Logger log = new Logger(this.GetType(), "IInicioSesion");
            try
            {
                using (var context = new BaseDeDatosSorrySlidersEntities())
                {
                    var cuenta = context.CuentaSet.FirstOrDefault(registro => registro.CorreoElectronico == correoElectronico);
                    if (cuenta != null)
                    {
                        return (Constantes.OPERACION_EXITOSA, cuenta.Nickname, cuenta.Avatar);
                    }
                    else
                    {
                        return (Constantes.OPERACION_EXITOSA_VACIA, null, null);
                    }
                }
            }
            catch (SqlException ex)
            {
                log.LogError("Error al ejecutar consulta SQL", ex);
                return (Constantes.ERROR_CONSULTA, null, null);
            }
            catch (EntityException ex)
            {
                log.LogError("Error de conexión a la base de datos", ex);
                return (Constantes.ERROR_CONEXION_BD, null, null);
            }
        }
        
    }



}
