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
        public string Name { get; set; }        //имя 
        public string FamilyName { get; set; }     //фамилия
        public string? SecondName { get; set; } //отчество
        public string? Phone { get; set; }
        public string? City { get; set; }
        public DateTime BornDate { get; set; }
        public bool Gender { get; set; }
        
        //public string chipNumber { get; set;}
        public IEnumerable<Partisipation> Partisipations { get; set; }



    }
}
