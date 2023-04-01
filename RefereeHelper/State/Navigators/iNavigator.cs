using RefereeHelper.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RefereeHelper.State.Navigators
{
    public enum ViewType
    {
        Members,
        Groups,
        Clubs,
        Competitions,
        Distances,
        Regions,
        Timings,
        Comands,
        Start,
        Participation
    }

    public interface iNavigator
    {
        ViewModelBase CurrentViewModel { get; set; }
        ICommand UpdateCurrentViewModelCommand { get; }
    }
}
