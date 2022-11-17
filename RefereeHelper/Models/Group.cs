using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.Models
{
    public class Group
    {
        public int Id { get; }
        public string Name { get; }
        public int startAge { get; }
        public int endAge { get; }

        public int distance { get; }
    }
}
