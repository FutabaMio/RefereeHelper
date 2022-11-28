using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RefereeHelper.EntityFramework
{
    public class RefereeHelperDbContextFactory : IDesignTimeDbContextFactory<RefereeHelperDbContext>
    {
        public RefereeHelperDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<RefereeHelperDbContext>();
            options.
        }
    }
}
