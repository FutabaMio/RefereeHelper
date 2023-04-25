using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefereeHelper.Models.Base;

namespace RefereeHelper.Models
{
    public class Competition : BaseEntity
    {
        /// <summary>
        /// Название мероприятия
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Дата проведения мероприятия
        /// </summary>
        public DateTime? Date { get; set; }
        /// <summary>
        /// Место проведения мероприятия
        /// </summary>
        public string? Place { get; set; }
        /// <summary>
        /// Имя организатора
        /// </summary>
        public string? Organizer { get; set; }
        /// <summary>
        /// Имя судьи
        /// </summary>
        public string? Judge { get; set; }
        /// <summary>
        /// Секретарь
        /// </summary>
        public string? Secretary { get; set; }
        /// <summary>
        /// true - отсчитывается с дня рождения, false - с нового года
        /// </summary>
        public bool TypeAge { get; set; }
        public virtual IEnumerable<Partisipation> Partisipations { get; set; }
    }
}
