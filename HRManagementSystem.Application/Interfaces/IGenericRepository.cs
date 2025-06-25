using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? predicate = null, params Expression<Func<T, object>>[]? includes);
        T FirstOrDefault(Expression<Func<T, bool>>? predicate = null, params Expression<Func<T, object>>[]? includes);

        void Add(T entity);

        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

        void SoftDelete(T entity);
        void SoftDeleteRange(IEnumerable<T> entities);

    }
}
