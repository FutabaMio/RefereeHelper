using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.Models
{
    public class Competition
    {
        public  Competition _competition;

      

        public int Id { get; set; }
        public string Name { get; set; }
        public DateOnly Date { get; set; }
        public string Place { get; set; }

         public Competition(Competition competition)
        {
            _competition=competition;
        }

        public void AddCompetitionManually(string name, string place,DateOnly date)
        {
            _competition.Name = name;
            _competition.Date = date;
            _competition.Place = place;
        }
    }
}
