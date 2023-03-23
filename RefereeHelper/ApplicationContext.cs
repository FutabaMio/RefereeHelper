using Microsoft.EntityFrameworkCore;
using RefereeHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Member> Members { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Distance> Distances { get; set; }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<competition> Competitions { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Timing> Timings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Zachet.db");
        }
    }
}
