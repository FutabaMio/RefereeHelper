using RefereeHelper.Models;
using RefereeHelper.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.ViewModels
{
    public class ParticipationViewModel : ViewModelBase
    {
        private List<Partisipation> _participation;

        public List<Partisipation> Participation
        {
            get => _participation;
            set
            {
                _participation = value;
                OnPropertyChanged(nameof(Participation));
            }
        }
    }
}
