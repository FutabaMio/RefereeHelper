using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.Models
{
    public class Start
    {
        public int Id { get; set; }
        public virtual Partisipation Partisipation { get; set; }
        public virtual Team? Team { get; set; }
        public int Number { get; set; }
        public string? Chip { get; set; }
        public /*sk*/virtual IEnumerable<Timing> Timings { get; set; }

    }
}
