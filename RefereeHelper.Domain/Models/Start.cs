using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefereeHelper.Models.Base;

namespace RefereeHelper.Models
{
    public class Start : BaseEntity
    {
        public int PartisipationId { get; set; }
        /// <summary>
        /// Участие
        /// </summary>
        public virtual Partisipation Partisipation { get; set; }
        /// <summary>
        /// Ключ Команды
        /// </summary>
        public int? TeamId { get; set; }
        
        /// <summary>
        /// Команда
        /// </summary>
        public virtual Team? Team { get; set; }
        /// <summary>
        /// Стартовый номер
        /// </summary>
        public int? Number { get; set; }
        /// <summary>
        /// Статус спортсмена
        /// 0 - всё нормально
        /// 1 - Финишировал
        /// 2 - Do Not Finish
        /// 3 - Do Not Start
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Позиция по группе
        /// </summary>
        public int? Place { get; set; }
        /// <summary>
        /// Абсолютная позиция
        /// </summary>
        public int? PlaceAbsolute { get; set; }
        /// <summary>
        /// Чип
        /// </summary>
        public string? Chip { get; set; }
        /// <summary>
        /// Стартовое время
        /// </summary>
        public DateTime? StartTime { get; set; }
        public /*sk*/virtual IEnumerable<Timing> Timings { get; set; }

        public override string ToString()
        {
            string fin= this.Number.ToString();
            return fin;
        }

    }
}