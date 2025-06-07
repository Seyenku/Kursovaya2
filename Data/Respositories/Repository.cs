// Repository.cs
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Kursovaya.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _ctx;
        protected readonly DbSet<T> _set;

        public Repository(ApplicationDbContext ctx)
        {
            _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
            _set = ctx.Set<T>();
        }

        // ------------------- Чтение ----------------------------------------
        public IQueryable<T> Query() => _set;                 // IQueryable остаётся синхронным

        public Task<T> GetByIdAsync(int id) => _set.FindAsync(id);

        public Task<List<T>> GetAllAsync() => _set.ToListAsync();

        public Task<List<T>> FindAsync(Expression<Func<T, bool>> p)
            => _set.Where(p).ToListAsync();

        public Task<int> CountAsync(Expression<Func<T, bool>> p = null)
            => (p == null ? _set.CountAsync() : _set.CountAsync(p));

        public Task<List<T>> GetPagedAsync(int page, int size,
                                           Expression<Func<T, bool>> p = null)
        {
            if (page < 1) throw new ArgumentOutOfRangeException(nameof(page));
            if (size < 1) throw new ArgumentOutOfRangeException(nameof(size));

            var q = p == null ? _set : _set.Where(p);
            return q.Skip((page - 1) * size).Take(size).ToListAsync();
        }

        // ------------------- Изменение -------------------------------------
        public void Add(T e) => _set.Add(e);
        public void AddRange(IEnumerable<T> col) => _set.AddRange(col);
        public void Update(T e) => _ctx.Entry(e).State = EntityState.Modified;
        public void Remove(T e) => _set.Remove(e);
        public void RemoveRange(IEnumerable<T> col) => _set.RemoveRange(col);

        // ------------------- Сохранение ------------------------------------
        public Task<int> SaveChangesAsync() => _ctx.SaveChangesAsync();
    }

}