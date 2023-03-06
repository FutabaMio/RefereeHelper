using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefereeHelper.Models.Base;

namespace RefereeHelper.Models
{
    public class Distance : BaseEntity
    {
        public string Name { get; set; }
        public decimal Length { get; set; }
        public decimal Height { get; set; }
        public decimal Circles { get; set; }
        public DateTime startTime { get; set; }
        public /*sk*/virtual IEnumerable<Group> Groups { get; set; }

    }
}
