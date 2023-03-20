using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefereeHelper.Models.Base;


namespace RefereeHelper.Models
{
    public class Timing : BaseEntity
    {
        public virtual Start? Start { get; set; }
        public TimeOnly? TimeNow { get; set; }
        public TimeOnly? TimeFromStart { get; set; }            // может быть и DateTime
        public TimeOnly? CircleTime { get; set; }
        public int? Circle { get; set; }
        public int? Place { get; set; }
        public int? PlaceAbsolute { get; set; }
        //public bool autoMode { get; set; } = false;            //почитать об инициализации переменной после указания её свойств
        public TimeOnly? currentTime { get; set; }              //надо распарсить только время, поискать как (TimeOnly currentTime = DateOnly.FromDateTime(timer))
     }
}

