using RefereeHelper.Models;
using RefereeHelper.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.ViewModels
{
    public class CompetitionsViewModel : ViewModelBase
    {
        private List<competition> _competitions;
        private List<Group> _group;

        public List<competition> Competitions
        {
            get => _competitions;
            set
            {
                _competitions = value;
                OnPropertyChanged(nameof(Competitions));
            }
        }

        public List<Group> Group
        {
            get => _group;
            set
            {
                _group = value;
                OnPropertyChanged(nameof(Group));
            }
        }
    }
}
