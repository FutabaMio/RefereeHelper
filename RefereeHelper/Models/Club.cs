﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.Models
{
    public class Club
    {
        public Club _club;
        public int Id { get; set; }
        public string name { get; set; }
        public List<Member> members { get; set; }


        
    }
}
