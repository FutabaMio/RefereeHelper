using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.Models
{
    public class Competition
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Place { get; set; }
        public string Organizer { get; set; }
        public string Judge { get; set; }
        public string Secretary { get; set; }
        public /*sk*/virtual IEnumerable<Partisipation> Partisipations { get; set; }
    }
}
