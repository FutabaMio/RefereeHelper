using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefereeHelper.Models.Base;

namespace RefereeHelper.Models
{
    public class Member : BaseEntity
    {
        public virtual Discharge? Discharge { get; set; }
        public virtual Club? Club { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string FamilyName { get; set; }
        public DateTime bornDate { get; set; }
        public int gender { get; set; }

        //public string chipNumber { get; set;}
        public IEnumerable<Partisipation> Partisipations { get; set; }



    }
}
