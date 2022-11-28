﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RefereeHelper.EntityFramework
{
    internal class RefereeHelperDbContextFactory : IDesignTimeDbContextFactory<RefereeHelperDbContext>
    {
        public RefereeHelperDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<RefereeHelperDbContext>();
            options.UseSqlite("Data Context=SyclicSheck.db");
            return new RefereeHelperDbContext(options.Options);
        }
    }
 }
