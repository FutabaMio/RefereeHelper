using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.Models
{
    class Start
    {
        public int Id { get; set; }
        public int PartisipationId { get; set; }
        public int TeamId { get; set; }
        public int Number { get; set; }
        public string Chip { get; set; }
    }
}
