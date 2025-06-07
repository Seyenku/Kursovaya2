using Kursovaya.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Kursovaya.Data.Repositories
{
    public class BuildingRepository : Repository<Building>, IBuildingRepository
    {
        private readonly ApplicationDbContext _context;

        public BuildingRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Building> GetWithNodeCount()
        {
            return _context.Buildings
                .Include(b => b.Nodes)
                .ToList();
        }
    }
}
