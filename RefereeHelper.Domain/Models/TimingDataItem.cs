using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.Domain.Models
{
    public class TimingDataItem
    {
        public string Id { get; set; }
        public string FamilyName { get; set; }
        public string MemberName { get; set; }
        public string Team { get; set; }
        public string Startnumber { get; set; }
        public string Chip { get; set; }
        public string StartTime { get; set; }
        public string TimeNow { get; set; }
        public string TimeFromStart { get; set; }
        public string Circle { get; set; }
        public string CircleTime { get; set; }
        public string Place { get; set; }
        public string PlaceAbsolute { get; set; }
        public string IsFinish { get; set; }
    }
}
