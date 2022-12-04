using RefereeHelper.Domain.Models;
using RefereeHelper.Models;
using RefereeHelper.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.ViewModels
{
    public class DistancesViewModel : ViewModelBase
    {
        private List<Distance> _distances;

        public List<Distance> Distances
        {
            get => _distances;
            set
            {
                _distances = value;
                OnPropertyChanged(nameof(Distances));
            }
        }
    }
}
