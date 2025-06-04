using System;
using Kursovaya.Data.Repositories;

namespace Kursovaya.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        // Пример доступа к репозиториям
        public INodeRepository Nodes => new NodeRepository(_context);
        public IEquipmentRepository Equipment => new EquipmentRepository(_context);
    }

    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        INodeRepository Nodes { get; }
        IEquipmentRepository Equipment { get; }
    }
}
