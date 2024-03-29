﻿using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefereeHelper.Models.Base;

namespace RefereeHelper.Models
{
    public class Club : BaseEntity
    {
        public virtual Region Region { get; set; }
        public string Couch { get; set; }
        public string Name { get; set; }
        //public List<Member> members { get; set; } <- Узнать, надо ли список участников хранить в клубе (в бд SyclicSheck их нет)
        public /*sk*/virtual IEnumerable<Member>? members { get; set; }



    }
}
