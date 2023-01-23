using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.Models
{
    class Partisipation
    {
        public int Id { get; set; }
        public int CompetitionId { get; set; }
        public int MemberId { get; set; }
        public int GroupId { get; set; }
    }
}
