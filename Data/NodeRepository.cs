using Kursovaya.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Kursovaya.Data.Repositories
{
    public class NodeRepository : Repository<Node>, INodeRepository
    {
        private readonly ApplicationDbContext _context;

        public NodeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Node> GetByBuilding(int buildingId)
        {
            return _context.Nodes
                .Include(n => n.BuildingInfo)
                .Include(n => n.NodeType)
                .Where(n => n.Building == buildingId)
                .ToList();
        }

        public IEnumerable<Node> GetByType(int typeId)
        {
            return _context.Nodes
                .Include(n => n.BuildingInfo)
                .Include(n => n.NodeType)
                .Where(n => n.Type == typeId)
                .ToList();
        }

        public IEnumerable<Node> GetWithEquipmentCount()
        {
            return _context.Nodes
                .Include(n => n.Equipment)
                .ToList();
        }

        public IEnumerable<Node> GetFiltered(NodeFilterViewModel filter, int page, int pageSize)
        {
            var query = _context.Nodes
                .Include(n => n.BuildingInfo)
                .Include(n => n.NodeType)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.NodeNameFilter))
                query = query.Where(n => n.Name.Contains(filter.NodeNameFilter));

            if (filter.BuildingFilter.HasValue)
                query = query.Where(n => n.Building == filter.BuildingFilter.Value);

            if (filter.TypeFilter.HasValue)
                query = query.Where(n => n.Type == filter.TypeFilter.Value);

            return query
                .OrderBy(n => n.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public int GetFilteredCount(NodeFilterViewModel filter)
        {
            var query = _context.Nodes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.NodeNameFilter))
                query = query.Where(n => n.Name.Contains(filter.NodeNameFilter));

            if (filter.BuildingFilter.HasValue)
                query = query.Where(n => n.Building == filter.BuildingFilter.Value);

            if (filter.TypeFilter.HasValue)
                query = query.Where(n => n.Type == filter.TypeFilter.Value);

            return query.Count();
        }
    }
}
