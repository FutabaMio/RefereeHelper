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
        public string Name { get; set; }
        public int StartAge { get; set; }
        public int EndAge { get; set; }
        public /*sk*/virtual IEnumerable<Member> Members { get; set; }

        //public List<Member> members { get; set; } <- уточнить
        //public int distance { get; set; } <- уточнить

    }
}
