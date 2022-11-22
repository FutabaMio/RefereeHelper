using RefereeHelper.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.ViewModels
{
    public class DistancesViewModel : ViewModelBase
    {
        private List<Distances> _distances;

        public List<Distances> Distances
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
