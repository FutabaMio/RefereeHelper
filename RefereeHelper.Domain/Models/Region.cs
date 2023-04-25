using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefereeHelper.Models.Base;


namespace RefereeHelper.Models
{
    public class Region : BaseEntity
    {
        /// <summary>
        /// Название региона
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Кодовый номер региона
        /// </summary>
        public int СodeNumber { get; set; }
        public /*sk*/virtual IEnumerable<Club> Clubs { get; set; }

    }
}
