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
    }
}
