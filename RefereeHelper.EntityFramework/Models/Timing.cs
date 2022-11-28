using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.Models
{
    public class Timing
    {
        public int Id { get; set; }
        public bool autoMode { get; set; } = false;            //почитать об инициализации переменной после указания её свойств
        public DateTime currentTime { get; set; }              //надо распарсить только время, поискать как
     }
}

