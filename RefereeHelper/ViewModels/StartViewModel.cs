using RefereeHelper.Models;
using RefereeHelper.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.ViewModels
{
    public class StartViewModel : ViewModelBase
    {
        private List<Start> _start;
        public List<Start> Start
        {
            get => _start;
            set
            {
                _start= value;
                OnPropertyChanged(nameof(Start));
            }
        }
    }
}
