using Microsoft.EntityFrameworkCore;
using RefereeHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.EntityFramework.Services
{
    internal class TimingDataService : GenericDataService<Timing>
    {
        public TimingDataService(RefereeHelperDbContextFactory dbContextFactory) : base(dbContextFactory)
        {
        }
        public override async Task<IEnumerable<Timing>> GetAll()
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                IEnumerable<Timing> values = await dbContext.Set<Timing>().Include(x => x.Start).ThenInclude(y => y.Partisipation).ThenInclude(z => z.Member).ToListAsync();
                return values;
            }
        }
    }
}
