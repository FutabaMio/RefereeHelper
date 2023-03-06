using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefereeHelper.Models.Base;

namespace RefereeHelper.Models
{
    public class Group : BaseEntity
    {
        public virtual Distance Distance { get; set; }
        public bool Gender { get; set; }
        public string Name { get; set; }
        public decimal StartAge { get; set; }
        public decimal EndAge { get; set; }
        public /*sk*/virtual IEnumerable<Member> Members { get; set; }
    }
}
