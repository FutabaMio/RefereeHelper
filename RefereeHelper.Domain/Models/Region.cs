using RefereeHelper.Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.Models
{
    public class Region : BaseEntity
    {

       

        public string Name { get; set; }
        public int codeNumber { get; set; }

      
    }
}
