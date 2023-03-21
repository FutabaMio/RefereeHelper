using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.EntityFramework.Services
{
    public interface IDataService<T>
    {
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> predicate);
        Task<T> Get(int id);
        Task<T> Create (T entity);
        Task<T> Update (int id, T entity);
        Task<bool> Delete (int id);
    }
}
