using RefereeHelper.Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.Models
{
    public class Distance : BaseEntity
    {
        public string Name { get; set; }
        public decimal Length { get; set; }
        public decimal Height { get; set; }
        public TimeOnly startTime { get; set; }
    }
}
