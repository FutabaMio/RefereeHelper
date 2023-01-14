using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string FamilyName { get; set; }
        public DateTime bornDate { get; set; }
        public int gender { get; set; }

        //public string chipNumber { get; set;}



    }
}
