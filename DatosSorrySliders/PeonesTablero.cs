using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DatosSorrySliders
{
    [DataContract]
    public class PeonesTablero
    {
        [DataMember]
        public Dictionary<int, List<(double, double)>> PeonesActualmenteTablero { get; set; }
    }
}
