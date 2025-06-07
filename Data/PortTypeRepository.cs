using Kursovaya.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Kursovaya.Data.Repositories
{
    public class PortTypeRepository : Repository<PortType>, IPortTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public PortTypeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<PortType> GetByConnectorType(int connectorTypeId)
        {
            return _context.PortTypes
                .Include(p => p.ConnectorType)
                .Include(p => p.Speed)
                .Where(p => p.ConnectorTypeId == connectorTypeId)
                .ToList();
        }

        public IEnumerable<PortType> GetBySpeed(int speedId)
        {
            return _context.PortTypes
                .Include(p => p.ConnectorType)
                .Include(p => p.Speed)
                .Where(p => p.SpeedId == speedId)
                .ToList();
        }
    }
}
