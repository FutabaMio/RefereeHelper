﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefereeHelper.Domain.Models;
using RefereeHelper.Models.Base;


namespace RefereeHelper.Models
{
    public class Timing : BaseEntity
    {
        public int? StartId { get; set; }
        /// <summary>
        /// Старт
        /// </summary>
        public virtual Start? Start { get; set; }
        /// <summary>
        /// Текущее время (пересечения финиша/старта/начала нового круга)
        /// </summary>
        public TimeOnly? TimeNow { get; set; }
        /// <summary>
        /// Время, прошедшее со старта
        /// </summary>
        public TimeOnly? TimeFromStart { get; set; }            // может быть и DateTime
        /// <summary>
        /// Время круга
        /// </summary>
        public TimeOnly? CircleTime { get; set; }
        /// <summary>
        /// Количество прошедших кругов
        /// </summary>
        public int? Circle { get; set; }
        /// <summary>
        /// Позиция по группе
        /// </summary>
        public int? Place { get; set; }
        /// <summary>
        /// Абсолютная позиция
        /// </summary>
        public int? PlaceAbsolute { get; set; }
        /// <summary>
        /// Финишировал ли
        /// </summary>
        public bool? IsFinish { get; set; }

        public static implicit operator Timing(TimingDataItem v)
        {
            throw new NotImplementedException();
        }

    }
}

