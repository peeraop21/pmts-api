using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity GetById(int id);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> GetAllByFactory(Expression<Func<TEntity, bool>> predicate);
        IEnumerable<TEntity> GetAllByPredicate(Expression<Func<TEntity, bool>> predicate);
        IEnumerable<TEntity> GetAllByFactoryTake100(Expression<Func<TEntity, bool>> predicate);
        TEntity GetByPredicate(Expression<Func<TEntity, bool>> predicate);
        TEntity GetByFactory(Expression<Func<TEntity, bool>> predicate);
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);

        void Update(TEntity entity);

        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        int UpdateList(IEnumerable<TEntity> models);
        int AddList(IEnumerable<TEntity> models);
        int RemoveandAddList(IEnumerable<TEntity> entities, IEnumerable<TEntity> models);
    }
}