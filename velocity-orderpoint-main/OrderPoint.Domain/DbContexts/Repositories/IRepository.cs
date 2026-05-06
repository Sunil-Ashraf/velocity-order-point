using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using OrderPoint.Domain.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.DbContexts.Repositories
{
    public class IRepository<T> where T : class
    {
        protected DbSet<T> DbSet;
        AppDbContext db;
        public IRepository(AppDbContext _db)
        {
            db = _db;
            DbSet = db.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return db.Set<T>();
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> expression)
        {
            return db.Set<T>().Where(expression);
        }

        public T GetBy(Expression<Func<T, bool>> expression)
        {
            return db.Set<T>().FirstOrDefault(expression);
        }
        public async  Task<T> GetByAsync(Expression<Func<T, bool>> expression)
        {
            return await db.Set<T>().FirstOrDefaultAsync(expression);
        }

        public T GetByAsNoTracking(Expression<Func<T, bool>> expression)
        {
            return db.Set<T>().AsNoTracking().FirstOrDefault(expression);
        }

        // ✅ Add an async version
        public async Task AddAsync(T entity)
        {
            await db.Set<T>().AddAsync(entity);
            await db.SaveChangesAsync();
        }
        // ✅ Add an async version
        public async Task AddRangeAsync(IEnumerable<T> entitylist)
        {
            await db.Set<T>().AddRangeAsync(entitylist);
            await db.SaveChangesAsync();
        }
        public void Add(T model)
        {
            db.Set<T>().Add(model);
            db.SaveChanges();
        }
        public   void AddRange(IEnumerable<T> entitylist)
        {
              db.Set<T>().AddRange(entitylist);
              db.SaveChanges();
        }

        // ✅ Async Delete
        public async Task DeleteAsync(T entity)
        {
            db.Set<T>().Remove(entity);
            await db.SaveChangesAsync();
        }
        public void Delete(T model)
        {
            db.Set<T>().Remove(model);
            db.SaveChanges();
        }
        public void DeleteRange(IEnumerable<T> model)
        {
            db.Set<T>().RemoveRange(model);
            db.SaveChanges();
        }

        // ✅ Async Update
        public async Task UpdateAsync(T entity)
        {
            db.Set<T>().Update(entity);
            await db.SaveChangesAsync();
        }
        public void Update(T model)
        {
            db.Set<T>().Update(model);
            db.SaveChanges();
        }

        public void UpdateList(IEnumerable<T> model)
        {
            db.Set<T>().UpdateRange(model);
            db.SaveChanges();
        }

        public IQueryable<TResult> GetIncludewithFirstOrDefault<TResult>(Expression<Func<T, TResult>> selector,
          Expression<Func<T, bool>> predicate = null,
          Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
          Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
          bool disableTracking = true)
        {
            IQueryable<T> query = DbSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (orderBy != null)
            {
                return orderBy(query).Select(selector);
            }
            else
            {
                return query.Select(selector);
            }
        }
    }

}
