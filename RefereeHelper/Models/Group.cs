using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.Models
{
    public class Group
    {
        public readonly Group _group;

        

        public int Id { get; set; }
        public string Name { get; set; }
        public int startAge { get; set; }
        public int endAge { get; set; }

        public int distance { get; set; }

        public Group(Group group)
        {
            _group=group;
        }

        public void AddGroupManually(string name, int startAge, int endAge, int distance)
        {
            _group.Name = name;
            _group.startAge = startAge;
            _group.endAge = endAge;
            _group.distance = distance;
        }

    }
}
