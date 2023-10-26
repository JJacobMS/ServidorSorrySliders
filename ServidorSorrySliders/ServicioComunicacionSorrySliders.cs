﻿using DatosSorrySliders;
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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServidorSorrySliders
{
    public partial class ServicioComunicacionSorrySliders : IInicioSesion
    {

        public Constantes VerificarContrasenaDeCuenta(CuentaSet cuentaPorVerificar)
        {
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
                Debug.WriteLine(ex.ToString());
                return Constantes.ERROR_CONSULTA;
            }
            catch (EntityException ex)
            {
                Console.WriteLine(ex.ToString());
                return Constantes.ERROR_CONEXION_BD;
            }
        }

        public Constantes VerificarExistenciaCorreoCuenta(string correoElectronico)
        {
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
            catch (EntityException excepcion)
            {
                Debug.WriteLine(excepcion.ToString());
                return Constantes.ERROR_CONEXION_BD;
            }
        }
    }

    public partial class ServicioComunicacionSorrySliders : IMenuPrincipal
    {
        public (Constantes, string, byte[], string) RecuperarDatosUsuario(string correoElectronico)
        {
            try
            {
                using (var context = new BaseDeDatosSorrySlidersEntities())
                {
                    var cuenta = context.CuentaSet.FirstOrDefault(registro => registro.CorreoElectronico == correoElectronico);
                    if (cuenta != null)
                    {
                        return (Constantes.OPERACION_EXITOSA, cuenta.Nickname, cuenta.Avatar, cuenta.Contraseña);
                    }
                    else
                    {
                        return (Constantes.OPERACION_EXITOSA_VACIA, null, null, null);
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex);
                return (Constantes.ERROR_CONSULTA, null, null, null);
            }
            catch (EntityException ex)
            {
                Console.WriteLine(ex);
                return (Constantes.ERROR_CONEXION_BD, null, null, null);
            }
        }

        public (Constantes, List<Puntuacion>) RecuperarPuntuaciones()
        {
            try
            {
                using (var context = new BaseDeDatosSorrySlidersEntities())
                {
                    var puntuaciones = context.Database.SqlQuery<Puntuacion>("SELECT CuentaSet.Nickname, Count(Posicion) AS NumeroPartidasGanadas FROM RelacionPartidaCuentaSET " +
                    "Inner Join CuentaSet on CuentaSet.CorreoElectronico = RelacionPartidaCuentaSET.CorreoElectronico " +
                    "where RelacionPartidaCuentaSET.CorreoElectronico = RelacionPartidaCuentaSET.CorreoElectronico AND Posicion = 0 GROUP BY CuentaSet.Nickname ORDER BY NumeroPartidasGanadas DESC;").ToList();
                    if (puntuaciones == null)
                    {
                        return (Constantes.OPERACION_EXITOSA_VACIA, null);
                    }
                    else
                    {
                        return (Constantes.OPERACION_EXITOSA, puntuaciones);

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
    }



}
