using RefereeHelper.Models;
using RefereeHelper.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.ViewModels
{
    public class TimingViewModel : ViewModelBase
    {
        private List<Timing> _timing;

        public List<Timing> Timing
        {
            get => _timing;
            set
            {
                _timing = value;
                OnPropertyChanged(nameof(Timing));
            }
        }
    }
}
