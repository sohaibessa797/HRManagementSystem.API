using Domain.SoftDelet;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Infrastructure.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context; 
            _dbSet = _context.Set<T>();
        }
        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public T FirstOrDefault(Expression<Func<T, bool>>? predicate = null, params Expression<Func<T, object>>[]? includes)
        {
            IQueryable<T> query = _dbSet;

            if (typeof(ISoftDeletable).IsAssignableFrom(typeof(T)))
            {
                query = query.Where(e => !((ISoftDeletable)e).IsDeleted);
            }

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? predicate = null, params Expression<Func<T, object>>[]? includes)
        {
            IQueryable<T> query = _dbSet;

            if (typeof(ISoftDeletable).IsAssignableFrom(typeof(T)))
            {
                //query = query.Where(e => !((ISoftDeletable)e).IsDeleted);
                var parameter = Expression.Parameter(typeof(T), "e");
                var property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
                var condition = Expression.Equal(property, Expression.Constant(false));
                var lambda = Expression.Lambda<Func<T, bool>>(condition, parameter);

                query = query.Where(lambda);
            }

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            return query.ToList();
        }

        public bool Any(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Any(predicate);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
        public void SoftDelete(T entity)
        {
            if (entity is ISoftDeletable deletableEntity)
            {
                deletableEntity.IsDeleted = true;
                _dbSet.Update(entity);
            }
            else
            {
                throw new InvalidOperationException($"{typeof(T).Name} does not support soft delete.");
            }
        }

        public void SoftDeleteRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                if (entity is ISoftDeletable deletableEntity)
                {
                    deletableEntity.IsDeleted = true;
                }
                else
                {
                    throw new InvalidOperationException($"{typeof(T).Name} does not support soft delete.");
                }
            }

            _dbSet.UpdateRange(entities);
        }
    }
}
