using RefereeHelper.Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.Models
{
    public class Group : BaseEntity
    {
        public string Name { get; set; }
        public int startAge { get; set; }
        public int endAge { get; set; }
        public List<Member> members { get; set; }
        public int distance { get; set; }

    }
}
