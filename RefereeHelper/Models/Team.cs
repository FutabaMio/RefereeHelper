using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefereeHelper.Models.Base;


namespace RefereeHelper.Models
{
    public class Team : BaseEntity
    {
        public string Name { get; set; }
        public /*sk*/virtual IEnumerable<Start> Starts { get; set; }
    }
}
