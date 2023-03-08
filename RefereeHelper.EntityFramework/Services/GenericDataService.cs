using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RefereeHelper.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.EntityFramework.Services
{
    public class GenericDataService<T> : IDataService<T> 
        where T : BaseEntity
    {
        private readonly RefereeHelperDbContextFactory _dbContextFactory;
        public GenericDataService(RefereeHelperDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        public async Task<IEnumerable<T>> GetAll()
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                IEnumerable<T> values= await dbContext.Set<T>().ToListAsync();
                return values;
            }
        }

        public async Task<T> Get(int id)
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                T value = await dbContext.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
                if (value != null)
                {
                    return value;
                }
                else
                {
                    throw new Exception("Object not found");
                }
            }
        }

        public async Task<T> Create(T entity)
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                EntityEntry<T> createdEntity = await dbContext.AddAsync(entity);
                await dbContext.SaveChangesAsync();
                return createdEntity.Entity;
            }
        }

        public async Task<T> Update(int id, T entity)
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                entity.Id = id;
                dbContext.Set<T>().Update(entity);
                await dbContext.SaveChangesAsync();
                return entity;
            }
        }

        public async Task<bool> Delete(int id)
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                T entity = await dbContext.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
                dbContext.Set<T>().Remove(entity);
                await dbContext.SaveChangesAsync();
                return true;
            }
        }
    }
}
