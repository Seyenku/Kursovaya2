using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Kursovaya.Data.Repositories
{
    public interface IRepository<T> where T : class
    {
        // --- Чтение ---------------------------------------------------------
        IQueryable<T> Query();                                            // ленивый IQueryable
        Task<T> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);
        Task<List<T>> GetPagedAsync(int page, int pageSize,
                                         Expression<Func<T, bool>> predicate = null);

        // --- Изменение ------------------------------------------------------
        void Add(T entity);
        void Update(T entity);
        void Remove(T entity);
        void AddRange(IEnumerable<T> entities);
        void RemoveRange(IEnumerable<T> entities);

        // --- Сохранение -----------------------------------------------------
        Task<int> SaveChangesAsync();
    }
}