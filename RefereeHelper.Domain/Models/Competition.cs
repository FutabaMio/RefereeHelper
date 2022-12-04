using RefereeHelper.Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.Models
{
    public class Competition : BaseEntity
    {

        public string Name { get; set; }
        public DateOnly Date { get; set; }
        public string Place { get; set; }

    }
}
