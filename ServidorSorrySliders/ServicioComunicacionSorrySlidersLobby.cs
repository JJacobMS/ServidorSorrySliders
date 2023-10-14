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
        
        public (Constantes, List<CuentaSet>) UnirseAlLobby(string uid, string correoJugadorNuevo)
        {
            List<CuentaSet> cuentasPartidaLobby = new List<CuentaSet>();
            try
            {
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    var cuentasPartida = contexto.Database.SqlQuery<CuentaSet>
                        ("Select CuentaSet.CorreoElectronico, Avatar, Nickname, Contraseña, IdUsuario from CuentaSet " +
                        "Inner Join RelacionPartidaCuentaSet ON RelacionPartidaCuentaSet.CorreoElectronico = CuentaSet.CorreoElectronico " +
                        "Where RelacionPartidaCuentaSet.CodigoPartida = @uid " +
                        "Order By RelacionPartidaCuentaSet.Posicion;", new SqlParameter("@uid", uid)).ToList();

                    if (cuentasPartida == null)
                    {
                        return (Constantes.OPERACION_EXITOSA_VACIA, null);
                    }

                    for (int i = 0; i < cuentasPartida.Count; i++)
                    {
                        cuentasPartidaLobby.Add(new CuentaSet
                        {
                            Nickname = cuentasPartida[i].Nickname,
                            CorreoElectronico = cuentasPartida[i].CorreoElectronico,
                            //Avatar = cuentasPartida[i].Avatar
                        });
                    }

                    var cantidadMaximaJugadores = contexto.Database.SqlQuery<int>
                        ("Select CantidadJugadores from PartidaSet Where PartidaSet.CodigoPartida = @uid", new SqlParameter("@uid", uid)).
                        FirstOrDefault();

                    if (cantidadMaximaJugadores <= cuentasPartida.Count)
                    {
                        return (Constantes.OPERACION_EXITOSA_VACIA, cuentasPartidaLobby);
                    }

                    /*contexto.Database.ExecuteSqlCommand("Insert into RelacionPartidaCuentaSet(Posicion,CodigoPartida, CorreoElectronico) " +
                        "VALUES (@posicion, @uid, @correo);", new SqlParameter("@posicion", cuentasPartida.Count),
                        new SqlParameter("@uid", uid), new SqlParameter("@correo", correoJugadorNuevo));*/

                    return (Constantes.OPERACION_EXITOSA, cuentasPartidaLobby);
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex);
                return (Constantes.ERROR_CONSULTA, null);
            }
            catch (EntityException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex);
                return (Constantes.ERROR_CONEXION_BD, null);
            }
        }

        public void EntrarPartida(string uid, string correoJugadorNuevo)
        {
            
        }


    }
}
