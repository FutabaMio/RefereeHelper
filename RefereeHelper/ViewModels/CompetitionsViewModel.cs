using RefereeHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.ViewModels
{
    public class CompetitionsViewModel : ViewModelBase
    {
        private List<Competition> _competitions;

        public List<Competition> Competitions
        {
            get => _competitions;
            set
            {
                _competitions = value;
                OnPropertyChanged(nameof(Competitions));
            }
        }
    }
}
