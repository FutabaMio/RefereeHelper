using RefereeHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.Commands
{
    public class AddMemberManuallyComamnd : CommandBase
    {
        private readonly Member _member;

        public AddMemberManuallyComamnd(Member member)
        {
            _member=member;
        }

        public override void Execute(object? parameter)
        {
           
        }
    }
}
