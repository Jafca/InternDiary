using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;

namespace InternDiary.Data
{
    public class EFRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private DbContext _db;
        private DbSet<TEntity> _dbSet;

        public EFRepository(DbContext context)
        {
            _db = context;
            _dbSet = context.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Expression<Func<TEntity, dynamic>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (!string.IsNullOrEmpty(includeProperties))
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(includeProperty);

            if (orderBy != null)
                query = query.OrderBy(orderBy);

            return query;
        }

        public virtual IEnumerable<TEntity> GetAll(
            Expression<Func<TEntity, dynamic>> orderBy = null,
            string includeProperties = "")
        {
            return Get(null, orderBy, includeProperties);
        }

        public virtual void Add(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public virtual void Alter(TEntity entityToUpdate)
        {
            _dbSet.AddOrUpdate(entityToUpdate);
        }

        public virtual void Remove(TEntity entityToDelete)
        {
            if (_db.Entry(entityToDelete).State == EntityState.Detached)
                _dbSet.Attach(entityToDelete);
            _dbSet.Remove(entityToDelete);
        }
    }
}