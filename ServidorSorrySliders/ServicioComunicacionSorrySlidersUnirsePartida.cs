﻿using DatosSorrySliders;
using InterfacesServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorSorrySliders
{
    public partial class ServicioComunicacionSorrySliders: IUnirsePartida
    {
        public (Constantes, List<CuentaSet>) RecuperarJugadoresLobby(string uid)
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
                        "Order By RelacionPartidaCuentaSet.IdPartidaCuenta;", new SqlParameter("@uid", uid)).ToList();

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
                            Avatar = cuentasPartida[i].Avatar
                        });
                    }

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

        public (Constantes, PartidaSet) RecuperarPartida(string codigoPartida)
        {
            try
            {
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    var partidaRecuperada = contexto.Database.SqlQuery<PartidaSet>
                        ("SELECT PartidaSet.CodigoPartida, PartidaSet.CorreoElectronico, PartidaSet.CantidadJugadores from PartidaSet " +
                        "INNER JOIN RelacionPartidaCuentaSet ON RelacionPartidaCuentaSet.CodigoPartida = PartidaSet.CodigoPartida " +
                        "Where PartidaSet.CodigoPartida = @codigoPartida;", new SqlParameter("@codigoPartida", codigoPartida)).FirstOrDefault();
                    if (partidaRecuperada == null)
                    {
                        return (Constantes.OPERACION_EXITOSA_VACIA, null);
                    }
                    else 
                    {
                        PartidaSet partida = new PartidaSet
                        {
                            CantidadJugadores = partidaRecuperada.CantidadJugadores,
                            CodigoPartida = partidaRecuperada.CodigoPartida,
                            CorreoElectronico = partidaRecuperada.CorreoElectronico
                        };
                        return (Constantes.OPERACION_EXITOSA, partida);
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString());
                return (Constantes.ERROR_CONSULTA, null);
            }
            catch (EntityException ex)
            {
                Console.WriteLine(ex.ToString());
                return (Constantes.ERROR_CONEXION_BD, null);
            }
        }

        public Constantes SalirDelLobby(string correoJugador, string codigoPartida)
        {
            try
            {
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    contexto.Database.ExecuteSqlCommand("DELETE from RelacionPartidaCuentaSet where codigoPartida=@codigoPartida AND " +
                        "CorreoElectronico=@correoElectronico;", new SqlParameter("@codigoPartida", codigoPartida),
                        new SqlParameter("@correoElectronico", correoJugador));
                   return (Constantes.OPERACION_EXITOSA);
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString()); 
                return Constantes.ERROR_CONSULTA;
            }
            catch (EntityException ex)
            {
                Console.WriteLine(ex.ToString());
                return Constantes.ERROR_CONEXION_BD;
            }
        }

        public (Constantes, int) UnirseAlLobby(string uid, string correoJugadorNuevo)
        {
            int numeroMaximoJugadores = -1;
            try
            {
                using (var contexto = new BaseDeDatosSorrySlidersEntities())
                {
                    var cuentasPartida = contexto.Database.SqlQuery<CuentaSet>
                        ("Select CuentaSet.CorreoElectronico, Avatar, Nickname, Contraseña, IdUsuario from CuentaSet " +
                        "Inner Join RelacionPartidaCuentaSet ON RelacionPartidaCuentaSet.CorreoElectronico = CuentaSet.CorreoElectronico " +
                        "Where RelacionPartidaCuentaSet.CodigoPartida = @uid " +
                        "Order By RelacionPartidaCuentaSet.IdPartidaCuenta;", new SqlParameter("@uid", uid)).ToList();

                    for (int i = 0; i < cuentasPartida.Count; i++)
                    {
                        if (cuentasPartida[i].CorreoElectronico.Equals(correoJugadorNuevo))
                        {
                            return (Constantes.OPERACION_EXITOSA_VACIA, -1);
                        }
                    }

                    if (cuentasPartida == null)
                    {
                        return (Constantes.OPERACION_EXITOSA_VACIA, 0);
                    }

                    numeroMaximoJugadores = contexto.Database.SqlQuery<int>
                        ("Select CantidadJugadores from PartidaSet Where PartidaSet.CodigoPartida = @uid", new SqlParameter("@uid", uid)).
                        FirstOrDefault();

                    if (numeroMaximoJugadores <= cuentasPartida.Count)
                    {
                        return (Constantes.OPERACION_EXITOSA_VACIA, numeroMaximoJugadores);
                    }

                    contexto.Database.ExecuteSqlCommand("Insert into RelacionPartidaCuentaSet(Posicion,CodigoPartida, CorreoElectronico) " +
                        "VALUES (0, @uid, @correo);", new SqlParameter("@uid", uid), new SqlParameter("@correo", correoJugadorNuevo));

                    return (Constantes.OPERACION_EXITOSA, numeroMaximoJugadores);
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex);
                return (Constantes.ERROR_CONSULTA, numeroMaximoJugadores);
            }
            catch (EntityException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex);
                return (Constantes.ERROR_CONEXION_BD, numeroMaximoJugadores);
            }
        }
    }
}
