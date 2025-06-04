using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Kursovaya.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext Context;
        protected readonly DbSet<T> DbSet;

        public Repository(ApplicationDbContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

        public T GetById(int id)
        {
            return DbSet.Find(id);
        }

        public T GetById(object id)
        {
            return DbSet.Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return DbSet.ToList();
        }

        public IQueryable<T> Query()
        {
            return DbSet.AsQueryable();
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Where(predicate).ToList();
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return DbSet.FirstOrDefault(predicate);
        }

        public IEnumerable<T> GetPaged(int page, int pageSize)
        {
            return DbSet.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public IEnumerable<T> GetPaged(int page, int pageSize, Expression<Func<T, bool>> predicate)
        {
            return DbSet.Where(predicate).Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public int Count()
        {
            return DbSet.Count();
        }

        public int Count(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Count(predicate);
        }

        public bool Any(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Any(predicate);
        }

        public void Add(T entity)
        {
            DbSet.Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            DbSet.AddRange(entities);
        }

        public void Update(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(T entity)
        {
            DbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            DbSet.RemoveRange(entities);
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }
    }
}
