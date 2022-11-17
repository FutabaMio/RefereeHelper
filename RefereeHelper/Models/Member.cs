using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.Models
{
    public class Member
    {
        public Member _member;

       

        public int Id { get; }
        public string Name { get; }
        public string Surname { get; }
        public DateOnly bornDate { get; }
        public bool gender { get; } //0-девочка (дырка), 1-мальчик (палка)

        public Member(Member member)
        {
            _member=member;
        }

       // public void AddMember //из базы

        // public void AddMemberFromTable из таблицы
    }
}
