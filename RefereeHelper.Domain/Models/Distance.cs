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
        public decimal Length { get; set; }
        /// <summary>
        /// Перепад высот дистанции
        /// </summary>
        public decimal Height { get; set; }
        /// <summary>
        /// Кол-во кругов дистанции
        /// </summary>
        public decimal Circles { get; set; }
        /// <summary>
        /// Стартовое время дистанции
        /// </summary>
        public DateTime startTime { get; set; }
        public /*sk*/virtual IEnumerable<Group> Groups { get; set; }

    }
}
