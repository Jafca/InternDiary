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

        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, dynamic>> orderBy)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = query.OrderBy(orderBy);

            return query;
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

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}