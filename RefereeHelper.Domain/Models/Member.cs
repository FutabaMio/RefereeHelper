using RefereeHelper.Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.Models
{
    public class Member : BaseEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateOnly bornDate { get; set; }
        public bool gender { get;  set; } //0-девочка (потому что дырка), 1-мальчик(потому что палка)
        //public string chipNumber { get; set;}


       
    }
}
