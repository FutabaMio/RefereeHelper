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
        public int TeamId { get; set; }
        
        /// <summary>
        /// Команда
        /// </summary>
        public virtual Team? Team { get; set; }
        /// <summary>
        /// Стартовый номер
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// Чип
        /// </summary>
        public string? Chip { get; set; }
        /// <summary>
        /// Стартовое время
        /// </summary>
        public TimeOnly? StartTime { get; set; }
        public /*sk*/virtual IEnumerable<Timing> Timings { get; set; }

    }
}
