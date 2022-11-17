using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.Models
{
    public class Distance
    {
        public int Id { get; }
        public string Name { get; }
        public decimal length { get; }
        public decimal height { get; }
        public TimeOnly startTime { get; set; }

    }
}
