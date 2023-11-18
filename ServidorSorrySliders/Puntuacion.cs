using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServidorSorrySliders
{
    [DataContract]
    
    public class Puntuacion
    {
        [DataMember]
        private string _nickname;
        [DataMember]
        private int _numeroPartidasGanadas;
        [DataMember]
        public string Nickname { get => _nickname; set => _nickname = value; }
        [DataMember]
        public int NumeroPartidasGanadas { get => _numeroPartidasGanadas; set => _numeroPartidasGanadas = value; }
    }
}
