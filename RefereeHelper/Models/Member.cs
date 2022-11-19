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

       

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateOnly bornDate { get; set; }
        public bool gender { get;  set; } //0-девочка, 1-мальчик
        //public string chipNumber { get; set;}


        public Member(Member member)
        {
            _member=member;
        }



        public void AddMemberManually(string name, string surname, DateOnly bornDate, bool gender)
        {
            _member.Name = name;
            _member.Surname = surname;
            _member.bornDate = bornDate;
            _member.gender = gender;
            //_member.chip = вызов функции считывания чипа или добавление через текстбокс?
        }

      /*  public void DeleteMember()
        {
            Можно заменить на выбор пользователя и delete, чтобы удалить, или перенести на viewmodel
        } */

       // public void AddMember из БД

        // public void AddMemberFromTable из таблицы EXCEL
    }
}
