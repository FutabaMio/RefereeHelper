using RefereeHelper.Models;
using RefereeHelper.State.Navigators;
using RefereeHelper.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public iNavigator Navigator { get; set; } = new Navigator(new MembersViewModel());
        
    }
}
