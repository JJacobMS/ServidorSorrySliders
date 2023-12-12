using DatosSorrySliders;
using ServidorSorrySliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PruebasSorrySliders
{
    //Estas pruebas se deben ejecutar sin conexión a la base de datos
    public class CreacionRegistroErrorConexionBaseDatosPruebas
    {
        //IUnirsePartida
        [Fact]
        public void VerificarEntrarPartidaSinConexionBDPrueba()
        {
            Constantes respuestaEsperado = Constantes.ERROR_CONEXION_BD;
            int jugadoresMaximos = -1;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            string correoExistente = null;
            string uidExistente = null;

            (Constantes resultadoObtenidos, int cantidadJugadoresPrevios) = servicioComunicacion.UnirseAlLobby(uidExistente, correoExistente);

            Assert.Equal(respuestaEsperado, resultadoObtenidos);
            Assert.Equal(jugadoresMaximos, cantidadJugadoresPrevios);
        }
        [Fact]
        public void VerificarInsertarCuentaProvisionalSinConexionBDPrueba()
        {
            Constantes respuestaEsperado = Constantes.ERROR_CONEXION_BD;

            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            CuentaSet cuentaProvisional = new CuentaSet();

            Constantes resultadoObtenidos = servicioComunicacion.CrearCuentaProvisionalInvitado(cuentaProvisional);

            Assert.Equal(respuestaEsperado, resultadoObtenidos);
        }
        //IRegistroUsuario

        [Fact]
        public void VerificarInsertarCuentaUsuarioSinConexionBDPrueba()
        {
            Constantes respuestaEsperado = Constantes.ERROR_CONEXION_BD;
            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            UsuarioSet usuario = new UsuarioSet
            {
                Nombre = "nombrePrueba",
                Apellido = "apellidoPrueba",
            };

            CuentaSet cuenta = new CuentaSet
            {
                CorreoElectronico = "correoParaPruebas@gmail.com",
                Contraseña = "asdfghj8",
                Nickname = "nicknamePrueba",
                Avatar = BitConverter.GetBytes(0102030405)
            };
            Constantes resultadoObtenidos = servicioComunicacion.AgregarUsuario(usuario, cuenta);

            Assert.Equal(respuestaEsperado, resultadoObtenidos);
        }
        //ICrearLobby
        [Fact]
        public void VerificarCrearPartidaSinConexionBDPrueba()
        {
            Constantes respuestaEsperado = Constantes.ERROR_CONEXION_BD;
            Constantes resultadoObtenido;
            string codigo;
            int cantidadJugadores = 4;
            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();

            CuentaSet cuenta = new CuentaSet
            {
                CorreoElectronico = "correoPrueba@gmail.com"
            };
            (resultadoObtenido, codigo) = servicioComunicacion.CrearPartida(cuenta.CorreoElectronico, cantidadJugadores);
            Assert.Equal(respuestaEsperado, resultadoObtenido);

        }
        //IListaAmigos
        [Fact]
        public void VerificarCrearNotificacionSinConexionBDPrueba()
        {
            Constantes respuestaEsperado = Constantes.ERROR_CONEXION_BD;
            Constantes resultadoObtenido;
            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            NotificacionSet notificacionNueva = new NotificacionSet
            {
                CorreoElectronicoDestinatario = "correoCuatroDestinatario@gmail.com",
                CorreoElectronicoRemitente = "correoTresRemitente@gmail.com",
                Mensaje = "MensajeNotificacion",
                IdTipoNotificacion = 1
            };

            resultadoObtenido = servicioComunicacion.GuardarNotificacion(notificacionNueva);
            Assert.Equal(respuestaEsperado, resultadoObtenido);
        }

        [Fact]
        public void VerificarCrearAmistadSinConexionBDPrueba()
        {
            Constantes respuestaEsperado = Constantes.ERROR_CONEXION_BD;
            Constantes resultadoObtenido;
            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            string CorreoElectronicoDestinatario = "correoTresRemitente@gmail.com";
            string CorreoElectronicoRemitente = "correoCuatroDestinatario@gmail.com";
            resultadoObtenido = servicioComunicacion.GuardarAmistad(CorreoElectronicoDestinatario, CorreoElectronicoRemitente);
            Assert.Equal(respuestaEsperado, resultadoObtenido);
        }

        [Fact]
        public void VerificarCrearBaneoSinConexionBDPrueba()
        {
            Constantes respuestaEsperado = Constantes.ERROR_CONEXION_BD;
            Constantes resultadoObtenido;
            ServicioComunicacionSorrySliders servicioComunicacion = new ServicioComunicacionSorrySliders();
            string CorreoElectronicoDestinatario = "correoTresRemitente@gmail.com";
            string CorreoElectronicoRemitente = "correoCuatroDestinatario@gmail.com";
            resultadoObtenido = servicioComunicacion.BanearJugador(CorreoElectronicoDestinatario, CorreoElectronicoRemitente);
            Assert.Equal(respuestaEsperado, resultadoObtenido);
        }
    }
}
