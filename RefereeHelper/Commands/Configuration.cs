using RefereeHelper.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.Commands
{
    internal class Configuration
    {
        public static ObservableCollection<Member> Members { get; set; }
        public static ObservableCollection<Group> Groups { get; set; }
        public static ObservableCollection<Distance> Distances { get; set; }
        public static ObservableCollection<Club> Clubs { get; set; }
        public static ObservableCollection<Competition> Competitions { get; set; }
        public static ObservableCollection<Region> Regions { get; set; }
        public static ObservableCollection<Timing> Timing { get; set; }
    }
}
