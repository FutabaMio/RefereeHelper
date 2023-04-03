using Microsoft.EntityFrameworkCore;
using RefereeHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.EntityFramework
{
    public class RefereeHelperDbContext : DbContext
    {
        public RefereeHelperDbContext(DbContextOptions options) : base(options)
        { 
            Database.EnsureCreated();   
        }

        public DbSet<Member> Members { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Distance> Distances { get; set; }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<competition> Competitions { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Timing> Timings { get; set; }
        public DbSet<Discharge> Discharges { get; set; }
        public DbSet<Partisipation> Partisipations { get; set; }
        public DbSet<Start> Starts { get; set; }
        public DbSet<Team> Teams { get; set; }

    }
}
