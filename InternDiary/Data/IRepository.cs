using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace InternDiary.Data
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, dynamic>> orderBy);
        void Add(TEntity entity);
        void Alter(TEntity entity);
        void Remove(TEntity entity);
        void Save();
    }
}