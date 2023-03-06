using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefereeHelper.Models.Base;


namespace RefereeHelper.Models
{
    public class Partisipation : BaseEntity
    {
        public virtual Member? Member { get; set; }
        public virtual Competition Competition { get; set; }
        public virtual Group? Group { get; set; }
        public /*sk*/virtual IEnumerable<Start> Starts { get; set; }
    }
}