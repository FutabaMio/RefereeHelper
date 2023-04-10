using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefereeHelper.Models.Base;

namespace RefereeHelper.Models
{
    public class Distance : BaseEntity
    {
        /// <summary>
        /// Название дистанции
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Длина дистанции
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// Перепад высот дистанции
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Кол-во кругов дистанции
        /// </summary>
        public int Circles { get; set; }
        /// <summary>
        /// Стартовое время дистанции
        /// </summary>
        public DateTime StartTime { get; set; }
        public /*sk*/virtual IEnumerable<Group> Groups { get; set; }

    }
}
