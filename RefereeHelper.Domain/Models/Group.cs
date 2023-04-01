using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefereeHelper.Models.Base;

namespace RefereeHelper.Models
{
    public class Group : BaseEntity
    {
        /// <summary>
        /// Дистанция
        /// </summary>
        public virtual Distance Distance { get; set; }
        /// <summary>
        /// Гендер группы: true - мужской, false - женский
        /// </summary>
        public bool Gender { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Нижний порог возраста
        /// </summary>
        public decimal StartAge { get; set; }
        /// <summary>
        /// Верхний порог возраста
        /// </summary>
        public decimal EndAge { get; set; }
        public /*sk*/virtual IEnumerable<Member> Members { get; set; }
    }
}
