using DatosSorrySliders;
using InterfacesServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServidorSorrySliders
{
    public partial class ServicioComunicacionSorrySliders : IListaAmigos
    {
        public (Constantes, List<CuentaSet>) RecuperarAmigosCuenta(string correoElectronico)
        {
            try
            {
                List<CuentaSet> amigosJugador = new List<CuentaSet>();
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    var listaAmigos = contexto.Database.SqlQuery<CuentaSet>
                        ("Select Nickname, CorreoElectronico, Avatar, Contraseña, IdUsuario from RelaciónAmigosSet " +
                        "Inner Join CuentaSet On CorreoElectronico = CorreoElectronicoJugadorAmigo Where CorreoElectronicoJugadorPrincipal = @correo",
                        new SqlParameter("@correo", correoElectronico)).ToList();

                    if (listaAmigos == null || listaAmigos.Count <= 0)
                    {
                        return (Constantes.OPERACION_EXITOSA_VACIA, null);
                    }

                    for (int i = 0; i < listaAmigos.Count; i++) 
                    {
                        CuentaSet amigo = new CuentaSet
                        {
                            Nickname = listaAmigos[i].Nickname, CorreoElectronico = listaAmigos[i].CorreoElectronico
                        };
                        amigosJugador.Add(amigo);
                    }

                    return (Constantes.OPERACION_EXITOSA, amigosJugador);

                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Error1", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (Constantes.ERROR_CONSULTA, null);
            }
            catch (EntityException ex)
            {
                MessageBox.Show(ex.Message, "Error2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (Constantes.ERROR_CONEXION_BD, null);
            }
        }

        public (Constantes, List<CuentaSet>) RecuperarJugadoresCuenta(string informacionJugador)
        {
            try
            {
                List<CuentaSet> jugadoresEncontrados = new List<CuentaSet>();
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    var jugadores = contexto.Database.SqlQuery<CuentaSet>
                        ("Select * From CuentaSet Where Nickname Like '%' + @nickname + '%' Or CorreoElectronico Like '%' + @correo + '%' " +
                        "AND CorreoElectronico Like '%@%' ORDER BY CorreoElectronico DESC",
                        new SqlParameter("@nickname", informacionJugador), new SqlParameter("@correo", informacionJugador)).ToList();

                    if (jugadores == null || jugadores.Count <= 0)
                    {
                        return (Constantes.OPERACION_EXITOSA_VACIA, null);
                    }

                    for (int i = 0; i < jugadores.Count; i++)
                    {
                        CuentaSet jugador = new CuentaSet
                        {
                            Nickname = jugadores[i].Nickname,
                            CorreoElectronico = jugadores[i].CorreoElectronico
                        };
                        jugadoresEncontrados.Add(jugador);
                    }

                    return (Constantes.OPERACION_EXITOSA, jugadoresEncontrados);

                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Error1", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (Constantes.ERROR_CONSULTA, null);
            }
            catch (EntityException ex)
            {
                MessageBox.Show(ex.Message, "Error2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (Constantes.ERROR_CONEXION_BD, null);
            }
        }
    }
}
