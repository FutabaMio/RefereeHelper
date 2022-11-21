using RefereeHelper.Commands;
using RefereeHelper.Models;
using RefereeHelper.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RefereeHelper.State.Navigators
{
    public class Navigator : ObservableObject,  iNavigator
    {
        private ViewModelBase _currentViewModel;

        public Navigator(ViewModelBase currentViewModel)
        {
            _currentViewModel=currentViewModel;
        }

        public ViewModelBase CurrentViewModel
        {
            get
            {
                return _currentViewModel;
            }
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }

        public ICommand UpdateCurrentViewModelCommand => new UpdateCurrentViewModelCommand(this);

        //public event PropertyChangedEventHandler? PropertyChanged;

 
    }
}
