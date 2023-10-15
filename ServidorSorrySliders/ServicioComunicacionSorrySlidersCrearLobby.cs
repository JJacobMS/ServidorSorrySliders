using DatosSorrySliders;
using InterfacesServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServidorSorrySliders
{
    public partial class ServicioComunicacionSorrySliders : ICrearLobby
    {
        public (Constantes, string) CrearPartida(string correoHost, int numeroJugadores)
        {
            try
            {
                using (var context = new BaseDeDatosSorrySlidersEntities())
                {
                    PartidaSet partida = new PartidaSet
                    {
                        CodigoPartida = Guid.NewGuid(),
                        CorreoElectronico = correoHost,
                        CantidadJugadores = numeroJugadores
                    };
                    context.PartidaSet.Add(partida);

                    context.SaveChanges();

                    RelacionPartidaCuentaSet relacionPartidaCuenta = new RelacionPartidaCuentaSet
                    {
                        Posicion = 0,
                        CorreoElectronico = correoHost,
                        CodigoPartida=partida.CodigoPartida
                    };
                    context.RelacionPartidaCuentaSet.Add(relacionPartidaCuenta);
                    context.SaveChanges();

                    var partidaCreada = context.RelacionPartidaCuentaSet.OrderByDescending(registro => registro.IdPartidaCuenta).FirstOrDefault();

                    if (partidaCreada != null)
                    {
                        return (Constantes.OPERACION_EXITOSA, partidaCreada.CodigoPartida.ToString());
                    }
                    else
                    {
                        return (Constantes.OPERACION_EXITOSA_VACIA, null);
                    }

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
