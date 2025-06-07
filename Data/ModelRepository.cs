using Kursovaya.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Kursovaya.Data.Repositories
{
    public class ModelRepository : Repository<EquipmentModel>, IModelRepository
    {
        private readonly ApplicationDbContext _context;

        public ModelRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<EquipmentModel> GetByType(int typeId)
        {
            return _context.EquipmentModels
                .Include(m => m.Type)
                .Include(m => m.Manufacturer)
                .Include(m => m.InstallationType)
                .Where(m => m.TypeId == typeId)
                .ToList();
        }

        public IEnumerable<EquipmentModel> GetByManufacturer(int manufacturerId)
        {
            return _context.EquipmentModels
                .Include(m => m.Type)
                .Include(m => m.Manufacturer)
                .Include(m => m.InstallationType)
                .Where(m => m.ManufacturerId == manufacturerId)
                .ToList();
        }

        public IEnumerable<EquipmentModel> GetFiltered(ModelFilterViewModel filter, int page, int pageSize)
        {
            var query = _context.EquipmentModels
                .Include(m => m.Type)
                .Include(m => m.Manufacturer)
                .Include(m => m.InstallationType)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.NameFilter))
                query = query.Where(m => m.Name.Contains(filter.NameFilter));

            if (!string.IsNullOrEmpty(filter.ManufacturerFilter))
                query = query.Where(m => m.Manufacturer.Name.Contains(filter.ManufacturerFilter));

            if (!string.IsNullOrEmpty(filter.EquipmentTypeFilter))
                query = query.Where(m => m.Type.Name.Contains(filter.EquipmentTypeFilter));

            if (!string.IsNullOrEmpty(filter.InstallationTypeFilter))
                query = query.Where(m => m.InstallationType.Name.Contains(filter.InstallationTypeFilter));

            if (filter.ManagedFilter.HasValue)
                query = query.Where(m => m.Managed == filter.ManagedFilter);

            if (filter.ConsolePortFilter.HasValue)
                query = query.Where(m => m.Console == filter.ConsolePortFilter);

            if (filter.PoeSupportFilter.HasValue)
                query = query.Where(m => m.Poe == filter.PoeSupportFilter);

            return query
                .OrderBy(m => m.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public int GetFilteredCount(ModelFilterViewModel filter)
        {
            var query = _context.EquipmentModels.AsQueryable();

            if (!string.IsNullOrEmpty(filter.NameFilter))
                query = query.Where(m => m.Name.Contains(filter.NameFilter));

            if (!string.IsNullOrEmpty(filter.ManufacturerFilter))
                query = query.Where(m => m.Manufacturer.Name.Contains(filter.ManufacturerFilter));

            if (!string.IsNullOrEmpty(filter.EquipmentTypeFilter))
                query = query.Where(m => m.Type.Name.Contains(filter.EquipmentTypeFilter));

            if (!string.IsNullOrEmpty(filter.InstallationTypeFilter))
                query = query.Where(m => m.InstallationType.Name.Contains(filter.InstallationTypeFilter));

            if (filter.ManagedFilter.HasValue)
                query = query.Where(m => m.Managed == filter.ManagedFilter);

            if (filter.ConsolePortFilter.HasValue)
                query = query.Where(m => m.Console == filter.ConsolePortFilter);

            if (filter.PoeSupportFilter.HasValue)
                query = query.Where(m => m.Poe == filter.PoeSupportFilter);

            return query.Count();
        }

        public IEnumerable<ModelPort> GetModelPorts(int modelId)
        {
            return _context.ModelPorts
                .Include(p => p.PortType)
                .Include(p => p.PortType.ConnectorType)
                .Include(p => p.PortType.Speed)
                .Where(p => p.ModelId == modelId)
                .ToList();
        }

        public void RemoveAllPorts(int modelId)
        {
            var ports = _context.ModelPorts.Where(p => p.ModelId == modelId).ToList();
            _context.ModelPorts.RemoveRange(ports);
        }

        public void AddPort(ModelPort port)
        {
            _context.ModelPorts.Add(port);
        }
    }
}
