using Kursovaya.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Kursovaya.Data.Repositories
{
    public class EquipmentRepository : Repository<Equipment>, IEquipmentRepository
    {
        private readonly ApplicationDbContext _context;

        public EquipmentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Equipment> GetByNode(int nodeId)
        {
            return _context.Equipment
                .Include(e => e.Model)
                .Include(e => e.Owner)
                .Include(e => e.Node)
                .Where(e => e.NodeId == nodeId)
                .ToList();
        }

        public IEnumerable<Equipment> GetByModel(int modelId)
        {
            return _context.Equipment
                .Include(e => e.Model)
                .Include(e => e.Owner)
                .Include(e => e.Node)
                .Where(e => e.ModelId == modelId)
                .ToList();
        }

        public IEnumerable<Equipment> GetByOwner(int ownerId)
        {
            return _context.Equipment
                .Include(e => e.Model)
                .Include(e => e.Owner)
                .Include(e => e.Node)
                .Where(e => e.OwnerId == ownerId)
                .ToList();
        }

        public bool IsMacUnique(string mac, int? excludeId = null)
        {
            return !_context.Equipment
                .Any(e => e.Mac == mac && (!excludeId.HasValue || e.Id != excludeId.Value));
        }

        public bool IsIpUnique(string ip, int? excludeId = null)
        {
            return !_context.Equipment
                .Any(e => e.Ip == ip && (!excludeId.HasValue || e.Id != excludeId.Value));
        }

        public Equipment GetByMac(string mac)
        {
            return _context.Equipment
                .Include(e => e.Model)
                .Include(e => e.Owner)
                .Include(e => e.Node)
                .FirstOrDefault(e => e.Mac == mac);
        }

        public Equipment GetByIp(string ip)
        {
            return _context.Equipment
                .Include(e => e.Model)
                .Include(e => e.Owner)
                .Include(e => e.Node)
                .FirstOrDefault(e => e.Ip == ip);
        }
    }
}
