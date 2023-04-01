using RefereeHelper.State.Navigators;
using RefereeHelper.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RefereeHelper.Commands
{
    public class UpdateCurrentViewModelCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private iNavigator _navigator;

        public UpdateCurrentViewModelCommand(iNavigator navigator)
        {
            _navigator=navigator;
        }

        public bool CanExecute(object parameter)
        {
           return true;
        }

        public void Execute(object parameter)
        {
            if(parameter is ViewType)
            {
                ViewType viewType = (ViewType)parameter;
                switch (viewType)
                {
                    case ViewType.Members:
                        _navigator.CurrentViewModel = new MembersViewModel();
                        break;
                    case ViewType.Groups:
                        _navigator.CurrentViewModel = new GroupsViewModel();
                        break;
                    case ViewType.Clubs:
                        _navigator.CurrentViewModel = new ClubsViewModel();
                        break;
                    case ViewType.Competitions:
                            _navigator.CurrentViewModel = new CompetitionsViewModel();
                        break;
                    case ViewType.Timings:
                        _navigator.CurrentViewModel = new TimingViewModel();
                        break;
                    case ViewType.Distances:
                        _navigator.CurrentViewModel = new DistancesViewModel();
                        break;
                    case ViewType.Comands:
                        _navigator.CurrentViewModel = new ComandViewModel();
                        break;
                    case ViewType.Start:
                        _navigator.CurrentViewModel = new StartViewModel();
                        break;
                    case ViewType.Participation:
                        _navigator.CurrentViewModel = new ParticipationViewModel();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
