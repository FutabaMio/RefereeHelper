using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefereeHelper.Models.Base;

namespace RefereeHelper.Models
{
    public class Member : BaseEntity
    {
        /// <summary>
        /// Разряд
        /// </summary>
        public virtual Discharge? Discharge { get; set; }
        /// <summary>
        /// Клуб
        /// </summary>
        public virtual Club? Club { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Фамилия
        /// </summary>
        public string FamilyName { get; set; }
        /// <summary>
        /// Отчество
        /// </summary>
        public string? SecondName { get; set; }
        /// <summary>
        /// Номер телефона
        /// </summary>
        public string? Phone { get; set; }
        /// <summary>
        /// Город
        /// </summary>
        public string? City { get; set; }
        /// <summary>
        /// Дата рождения
        /// </summary>
        public DateTime BornDate { get; set; }
        /// <summary>
        /// True - male, false - female
        /// </summary>
        public bool Gender { get; set; }
        
        //public string chipNumber { get; set;}
        public IEnumerable<Partisipation> Partisipations { get; set; }



    }
}
