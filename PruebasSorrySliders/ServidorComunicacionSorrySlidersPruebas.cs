﻿using DatosSorrySliders;
using ServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PruebasSorrySliders
{
    public class ServidorComunicacionSorrySlidersPruebas
    {
        [Fact]
        public void VerificarExistenciaCorreoCorrectoPrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            string correoCorrectoExistente = "sulem477@gmail.com";

            Constantes respuestaActual = servicioComunicacion.VerificarExistenciaCorreoCuenta(correoCorrectoExistente);
            
            Assert.Equal(respuestaEsperada, respuestaActual);

        }

        [Fact]
        public void VerificarExistenciaCorreoIncorrectoPrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA_VACIA;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            string correoCorrectoExistente = "jacob@gmail.com";

            Constantes respuestaActual = servicioComunicacion.VerificarExistenciaCorreoCuenta(correoCorrectoExistente);

            Assert.Equal(respuestaEsperada, respuestaActual);

        }

        [Fact]
        public void VerificarContrasenaCuentaExitosamente()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            CuentaSet cuentaExistente = new CuentaSet { CorreoElectronico = "sulem477@gmail.com", Contraseña = "123"};

            Constantes respuestaActual = servicioComunicacion.VerificarContrasenaDeCuenta(cuentaExistente);

            Assert.Equal(respuestaEsperada, respuestaActual);

        }

        [Fact]
        public void VerificarContrasenaIncorrectaCuentaExistente()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA_VACIA;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            CuentaSet cuentaExistente = new CuentaSet { CorreoElectronico = "sulem477@gmail.com", Contraseña = "1234" };

            Constantes respuestaActual = servicioComunicacion.VerificarContrasenaDeCuenta(cuentaExistente);

            Assert.Equal(respuestaEsperada, respuestaActual);
        }

        [Fact]
        public void VerificarInsertarCuentaUsuarioExitosamentePrueba()
        {
            Constantes respuestaEsperado = Constantes.OPERACION_EXITOSA;
            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            UsuarioSet usuario = new UsuarioSet
            {
                Nombre = "Jacob",
                Apellido = "Montiel",
            };

            CuentaSet cuenta = new CuentaSet
            {
                CorreoElectronico = "correoEjemplo3@gmail.com",
                Contraseña = "asdfghj8",
                Nickname = "nickname",
                Avatar = BitConverter.GetBytes(0102030405)
            };
            Constantes resultadoObtenidos = servicioComunicacion.AgregarUsuario(usuario,cuenta);

            Assert.Equal(respuestaEsperado, resultadoObtenidos);

        }

        [Fact]
        public void VerificarInsertarCuentaUsuarioIncorrectoPrueba()
        {
            Constantes respuestaEsperado = Constantes.ERROR_CONSULTA;
            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            UsuarioSet usuario = new UsuarioSet
            {
                Nombre = "Sulem",
                Apellido = "Martinez",
            };

            CuentaSet cuenta = new CuentaSet
            {
                CorreoElectronico = "correoEjemplo@gmail.com",
                Contraseña = "password12345",
                Nickname = "melus",
                Avatar = BitConverter.GetBytes(0102030405)
            };
            Constantes resultadoObtenidos = servicioComunicacion.AgregarUsuario(usuario, cuenta);

            Assert.Equal(respuestaEsperado, resultadoObtenidos);

        }

        [Fact]
        public void VerificarCrearPartidaExitosamentePrueba()
        {
            Constantes respuestaEsperado = Constantes.OPERACION_EXITOSA;
            Constantes resultadoObtenido;
            string codigo;
            int cantidadJugadores = 4;
            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            
            CuentaSet cuenta = new CuentaSet
            {
                CorreoElectronico = "sao@gmail.com"
            };
            (resultadoObtenido, codigo) = servicioComunicacion.CrearPartida(cuenta.CorreoElectronico, cantidadJugadores);
            Assert.Equal(respuestaEsperado, resultadoObtenido);

        }

        [Fact]
        public void VerificarRecuperarPartidaExitosamentePrueba()
        {
            Constantes respuestaEsperado = Constantes.OPERACION_EXITOSA;
            Constantes resultadoObtenido;
            string codigoPartida= "B7C18916-DFA0-4F39-8561-5BF44B1C0076";
            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            PartidaSet partidaRecuperada = new PartidaSet{};
            (resultadoObtenido, partidaRecuperada) = servicioComunicacion.RecuperarPartida(codigoPartida);
            Assert.Equal(respuestaEsperado, resultadoObtenido);

        }
    }
}
