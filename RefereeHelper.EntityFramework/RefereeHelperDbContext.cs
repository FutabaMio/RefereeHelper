using Microsoft.EntityFrameworkCore;
using RefereeHelper.Models;

namespace RefereeHelper.EntityFramework
{
    public class RefereeHelperDbContext : DbContext
    {
        public RefereeHelperDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Member> Members { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Distance> Distances { get; set; }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Competition> Competitions { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Timing> Timings { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Groups>().OwnsOne(a=> a.Members)

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("");

            base.OnConfiguring(optionsBuilder);
        }

    }
}
