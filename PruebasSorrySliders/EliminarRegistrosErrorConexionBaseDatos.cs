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
    public class EliminarRegistrosErrorConexionBaseDatos
    {
        /// <seealso cref="InterfacesServidorSorrySliders.IListaAmigos"/>
        [Fact]
        public void VerificarEliminarNotificacionJugadorSinConexionBDPrueba()
        {
            Constantes respuestaEsperada = Constantes.ERROR_CONEXION_BD;
            Constantes respuestaActual;
            string correoElectronicoDestinatario = null;
            int idNotificacion = 0;
            ServicioComunicacionSorrySliders servicio = new ServicioComunicacionSorrySliders();
            respuestaActual = servicio.EliminarNotificacionJugador(correoElectronicoDestinatario, idNotificacion);
            Assert.Equal(respuestaActual, respuestaEsperada);
        }
        [Fact]
        public void VerificarEliminarAmistadSinConexionBDPrueba()
        {
            Constantes respuestaEsperada = Constantes.ERROR_CONEXION_BD;
            Constantes respuestaActual;
            string correoElectronicoPrincipal = null;
            string correoElectronicoAmigo = null;
            ServicioComunicacionSorrySliders servicio = new ServicioComunicacionSorrySliders();
            respuestaActual = servicio.EliminarAmistad(correoElectronicoPrincipal, correoElectronicoAmigo);
            Assert.Equal(respuestaActual, respuestaEsperada);
        }

        [Fact]
        public void VerificarEliminarBaneoSinConexionBDPrueba()
        {
            Constantes respuestaEsperada = Constantes.ERROR_CONEXION_BD;
            Constantes respuestaActual;
            string correoElectronicoPrincipal = null;
            string correoElectronicoAmigo = null;
            ServicioComunicacionSorrySliders servicio = new ServicioComunicacionSorrySliders();
            respuestaActual = servicio.EliminarBaneo(correoElectronicoPrincipal, correoElectronicoAmigo);
            Assert.Equal(respuestaActual, respuestaEsperada);
        }

    }
}
