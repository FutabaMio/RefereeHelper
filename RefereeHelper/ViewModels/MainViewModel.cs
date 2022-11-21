using RefereeHelper.State.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public iNavigator Navigator { get; set; } = new Navigator(new MembersViewModel());
        
    }
}
