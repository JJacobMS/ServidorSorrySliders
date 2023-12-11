using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServidorSorrySliders
{
    [DataContract]
    public class JugadorGanador
    {
        [DataMember]
        private string _correoElectronico;
        [DataMember]
        private string _nickname;
        [DataMember]
        private int _posicion;
        [DataMember]
        public string CorreoElectronico { get => _correoElectronico; set => _correoElectronico = value; }
        [DataMember]
        public string Nickname { get => _nickname; set => _nickname = value; }
        [DataMember]
        public int Posicion { get => _posicion; set => _posicion = value; }
    }
}
