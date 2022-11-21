using System.Collections.Generic;
using RefereeHelper.Models;

namespace RefereeHelper.ViewModels
{
    public class GroupsViewModel : ViewModelBase
    {
        private List<Group> _groups;

        public List<Group> Groups
        {
            get => _groups;
            set
            {
                _groups = value;
                OnPropertyChanged(nameof(Groups));
            }
        }
    }
}
