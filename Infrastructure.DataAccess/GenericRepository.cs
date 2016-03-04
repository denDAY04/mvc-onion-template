using Core.DomainServices;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.DataAccess
{
    public class GenericRepository<T> : IGenericRepository<T>
        where T : class
    {
        protected readonly ApplicationContext Context;
        protected readonly DbSet<T> DbSet;

        public GenericRepository(ApplicationContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        } 

        public IEnumerable<T> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "",
            int? page = null,
            int? pageSize = null)
        {
            var query = FilterLogic(filter, orderBy, includeProperties, page, pageSize);
            return query.ToList();
        }

        public async Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "",
            int? page = null,
            int? pageSize = null)
        {
            var query = FilterLogic(filter, orderBy, includeProperties, page, pageSize);
            return await query.ToListAsync();
        }

        private IQueryable<T> FilterLogic(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, string includeProperties, int? page, int? pageSize)
        {
            IQueryable<T> query = DbSet;

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            if (page.HasValue && pageSize.HasValue)
                query = query
                    .Skip((page.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);
            return query;
        }

        // Last resort!
        // It's best practice to not rely on your ORM to implement IQueryable.
        public IQueryable<T> AsQueryable()
        {
            return DbSet.AsQueryable();
        }

        public T Create()
        {
            var entity = DbSet.Create<T>();
            return entity;
        }

        public T GetByKey(params object[] key)
        {
            return DbSet.Find(key);
        }

        public async Task<T> GetByKeyAsync(params object[] key)
        {
            return await DbSet.FindAsync(key);
        }

        public T Insert(T entity)
        {
            return DbSet.Add(entity);
        }

        public void DeleteByKey(params object[] key)
        {
            var entityToDelete = DbSet.Find(key);

            if (Context.Entry(entityToDelete).State == EntityState.Detached)
                DbSet.Attach(entityToDelete);

            DbSet.Remove(entityToDelete);
        }

        public void Update(T entity)
        {
            DbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }

        public int Count(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = DbSet;

            if (filter != null)
                query = query.Where(filter);

            return query.Count();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = DbSet;

            if (filter != null)
                query = query.Where(filter);

            return await query.CountAsync();
        }
    }
}
