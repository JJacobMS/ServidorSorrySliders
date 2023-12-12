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
    public class EliminarRegistrosNoExistentesPruebas
    {
        //IListaAmigos
        [Fact]
        public void VerificarEliminarNotificacionJugadorNoExistentePrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA_VACIA;
            Constantes respuestaActual;
            string correoElectronicoDestinatario = "";
            int idNotificacion = 0;
            ServicioComunicacionSorrySliders servicio = new ServicioComunicacionSorrySliders();
            respuestaActual = servicio.EliminarNotificacionJugador(correoElectronicoDestinatario, idNotificacion);
            Assert.Equal(respuestaActual, respuestaEsperada);
        }
        [Fact]
        public void VerificarEliminarAmistadNoExistentePrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA_VACIA;
            Constantes respuestaActual;
            string correoElectronicoPrincipal = "";
            string correoElectronicoAmigo = "";
            ServicioComunicacionSorrySliders servicio = new ServicioComunicacionSorrySliders();
            respuestaActual = servicio.EliminarAmistad(correoElectronicoPrincipal, correoElectronicoAmigo);
            Assert.Equal(respuestaActual, respuestaEsperada);
        }

        [Fact]
        public void VerificarEliminarBaneoNoExistentePrueba()
        {
            Constantes respuestaEsperada = Constantes.OPERACION_EXITOSA_VACIA;
            Constantes respuestaActual;
            string correoElectronicoPrincipal = "";
            string correoElectronicoAmigo = "";
            ServicioComunicacionSorrySliders servicio = new ServicioComunicacionSorrySliders();
            respuestaActual = servicio.EliminarBaneo(correoElectronicoPrincipal, correoElectronicoAmigo);
            Assert.Equal(respuestaActual, respuestaEsperada);
        }
    }
}
