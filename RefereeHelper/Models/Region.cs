using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.Models
{
    public class Region
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int codeNumber { get; set; }
        public /*sk*/virtual IEnumerable<Club> Clubs { get; set; }

    }
}
