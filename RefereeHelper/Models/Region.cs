using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.Models
{
    public class Region
    {
        public Region _region;

       

        public int Id { get; set; }
        public string Name { get; set; }
        public int codeNumber { get; set; }

        public Region(Region region)
        {
            _region=region;
        }

        public void AddRegionManually(string name, int number)
        {
            _region.Name = name;
            _region.codeNumber = number;
        }
    }
}
