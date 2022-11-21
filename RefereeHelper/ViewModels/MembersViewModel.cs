using RefereeHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.ViewModels
{
    public class MembersViewModel : ViewModelBase
    {
        private List<Member> _members;

        public List<Member> Members
        {
            get => _members;
            set 
            { 
                _members = value;
                OnPropertyChanged(nameof(Members));
            }
        }
    }
}
