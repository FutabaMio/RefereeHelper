using Microsoft.EntityFrameworkCore;
using RefereeHelper.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.EntityFramework
{
    public class RefereeHelperDbContext : DbContext
    {


        public DbSet<Members> Members { get; set; }
        public DbSet<Groups> Groups { get; set; }
        public DbSet<Distances> Distances { get; set; }

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
