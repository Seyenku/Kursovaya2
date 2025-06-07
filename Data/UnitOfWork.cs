using Kursovaya.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Kursovaya.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> Repository<T>() where T : class;

        Task<int> SaveChangesAsync();

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private DbContextTransaction _transaction;
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);
            if (_repositories.ContainsKey(type))
                return (IRepository<T>)_repositories[type];

            var repo = new Repository<T>(_context);
            _repositories.Add(type, repo);
            return repo;
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public Task BeginTransactionAsync()
        {
            _transaction = _context.Database.BeginTransaction();
            return Task.CompletedTask;
        }

        public Task CommitTransactionAsync()
        {
            try
            {
                _context.SaveChanges();
                _transaction?.Commit();
            }
            catch
            {
                _transaction?.Rollback();
                throw;
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }

            return Task.CompletedTask;
        }

        public Task RollbackTransactionAsync()
        {
            _transaction?.Rollback();
            _transaction?.Dispose();
            _transaction = null;

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
