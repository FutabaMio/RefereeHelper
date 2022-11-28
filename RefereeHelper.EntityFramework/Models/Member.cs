using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateOnly bornDate { get; set; }
        public bool gender { get;  set; } //0-девочка (потому что дырка), 1-мальчик(потому что палка)
                                          //public string chipNumber { get; set;}


        public DbSet<Members> members { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=SyclicSheck");
        }


    }
}
