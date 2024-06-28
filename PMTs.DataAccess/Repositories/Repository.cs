using Microsoft.EntityFrameworkCore;
using PMTs.DataAccess.InterfaceRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PMTs.DataAccess.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {

        protected readonly DbContext Context;

        public Repository(DbContext context)
        {
            Context = context;
        }

        public TEntity GetById(int id)
        {
            return Context.Set<TEntity>().Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Context.Set<TEntity>().ToList();
        }

        public IEnumerable<TEntity> GetAllByFactory(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
            {
                return Context.Set<TEntity>().AsNoTracking().ToList();
            }

            return Context.Set<TEntity>().Where(predicate).AsNoTracking().ToList();
        }

        public IEnumerable<TEntity> GetAllByFactoryTake100(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
            {
                return Context.Set<TEntity>().AsNoTracking().ToList();
            }

            return Context.Set<TEntity>().Where(predicate).Take(100).AsNoTracking().ToList();
        }

        public TEntity GetByFactory(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Where(predicate).FirstOrDefault();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Where(predicate);
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().SingleOrDefault(predicate);
        }

        public void Add(TEntity entity)
        {
            Context.Set<TEntity>().Add(entity);
            Context.SaveChanges();
        }
        public void AddRange(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().AddRange(entities);
            Context.SaveChanges();
        }

        public void Update(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            Context.SaveChanges();
        }

        public void Remove(TEntity entity)
        {
            Context.Set<TEntity>().Remove(entity);
            Context.SaveChanges();
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().RemoveRange(entities);
            Context.SaveChanges();
        }

        public int UpdateList(IEnumerable<TEntity> models)
        {
            using (var dbContextTransaction = Context.Database.BeginTransaction())
            {

                try
                {
                    int respone = 0;
                    foreach (var model in models)
                    {
                        Context.Set<TEntity>().Update(model);
                    }
                    respone = Context.SaveChanges();
                    dbContextTransaction.Commit();
                    return respone;
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }

        }

        public int AddList(IEnumerable<TEntity> models)
        {
            using (var dbContextTransaction = Context.Database.BeginTransaction())
            {

                try
                {
                    int respone = 0;
                    foreach (var model in models)
                    {
                        Context.Set<TEntity>().Add(model);
                    }
                    respone = Context.SaveChanges();
                    dbContextTransaction.Commit();
                    return respone;
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }

        }
        public int RemoveandAddList(IEnumerable<TEntity> entities, IEnumerable<TEntity> models)
        {
            using (var dbContextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    Context.Set<TEntity>().RemoveRange(entities);

                    int respone = 0;
                    foreach (var model in models)
                    {
                        Context.Set<TEntity>().Add(model);
                    }
                    respone = Context.SaveChanges();
                    dbContextTransaction.Commit();
                    return respone;
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }

        }

        public IEnumerable<TEntity> GetAllByPredicate(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Where(predicate).ToList();
        }

        public TEntity GetByPredicate(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().FirstOrDefault(predicate);
        }
    }
}