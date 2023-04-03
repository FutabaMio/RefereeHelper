using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefereeHelper.Models.Base;


namespace RefereeHelper.Models
{
    public class Partisipation : BaseEntity
    {
        /// <summary>
        /// Участник
        /// </summary>
        public virtual Member? Member { get; set; }
        /// <summary>
        /// Соревнование
        /// </summary>
        public virtual competition Competition { get; set; }
        /// <summary>
        /// Группа
        /// </summary>
        public virtual Group? Group { get; set; }
        public /*sk*/virtual IEnumerable<Start> Starts { get; set; }
    }
}