using Kursovaya.Data;
using Kursovaya.Data.Repositories;
using Kursovaya.Models;
using Kursovaya.Models.Common;
using Kursovaya.Models.Reporting;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using static NodesApiController;


namespace Kursovaya.Services
{
    public sealed class NodeService : INodeService
    {
        private readonly IRepository<Node> _repo;

        public NodeService(IRepository<Node> repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public async Task<PagedResult<Node>> GetNodesAsync(NodeFilterViewModel f, int page, int size)
        {
            var q = _repo.Query().Include(n => n.Equipment);

            if (f != null)
            {
                if (f.BuildingFilter.HasValue)
                    q = q.Where(n => n.Building == f.BuildingFilter.Value);

                if (f.TypeFilter.HasValue)
                    q = q.Where(n => n.Type == f.TypeFilter.Value);

                if (!string.IsNullOrWhiteSpace(f.NodeNameFilter))
                    q = q.Where(n => n.Name.Contains(f.NodeNameFilter));

                if (f.DeviceCountFilter.HasValue)
                    q = q.Where(n => n.Equipment.Count == f.DeviceCountFilter);
            }

            var total = await q.CountAsync().ConfigureAwait(false);
            var items = await q.OrderBy(n => n.Id)
                               .Skip((page - 1) * size)
                               .Take(size)
                               .ToListAsync()
                               .ConfigureAwait(false);

            return new PagedResult<Node>(items, total);
        }

        public async Task<IReadOnlyList<Node>> GetAllAsync()
            => (await _repo.GetAllAsync().ConfigureAwait(false)).AsReadOnly();

        public Task<Node> GetByIdAsync(int id) 
            => _repo.GetByIdAsync(id);

        // переписать без Task
        public async Task<int> CreateAsync(Node node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            _repo.Add(node);
            await _repo.SaveChangesAsync().ConfigureAwait(false);
            return node.Id;
        }

        public async Task UpdateAsync(int id, NodeUpdateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var node = await _repo.GetByIdAsync(id).ConfigureAwait(false);
            if (node == null)
                throw new InvalidOperationException($"Узел с id={id} не найден.");

            node.Building = dto.BuildingId;
            node.Name = dto.Name;
            node.Type = dto.TypeId;
            node.Other = dto.Other;
            node.VerificationDate = dto.VerificationDate;

            _repo.Update(node);
            await _repo.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteAsync(int id)
        {
            var node = await _repo.GetByIdAsync(id).ConfigureAwait(false);
            if (node == null)
                return;

            _repo.Remove(node);
            await _repo.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    public sealed class BuildingService : IBuildingService
    {
        private readonly IRepository<Building> _repo;

        public BuildingService(IRepository<Building> repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public Task<IEnumerable<Building>> GetAllBuildingsAsync()
        {
            return _repo.GetAllAsync().ContinueWith(t => (IEnumerable<Building>)t.Result);
        }
    }

    public sealed class NodeTypeService : INodeTypeService
    {
        private readonly IRepository<NodeType> _repo;

        public NodeTypeService(IRepository<NodeType> repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public Task<IEnumerable<NodeType>> GetAllNodeTypesAsync()
        {
            return _repo.GetAllAsync().ContinueWith(t => (IEnumerable<NodeType>)t.Result);
        }
    }

    public sealed class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public ReportConfigViewModel GetReportConfig()
        {
            var config = new ReportConfigViewModel
            {
                AvailableSources = FieldRegistry.Map.Keys.ToList(),
                AvailableFields = new Dictionary<string, List<string>>()
            };

            foreach (var source in config.AvailableSources)
            {
                var fieldList = new List<string>();
                foreach (var obj in FieldRegistry.Map[source])
                {
                    var type = obj.GetType();
                    var nameProp = type.GetProperty("Name");
                    var name = (string)nameProp.GetValue(obj);
                    fieldList.Add(name);
                }

                config.AvailableFields[source] = fieldList;
            }

            return config;
        }

        public ReportViewModel GenerateReport(ReportRequest request)
        {
            var result = new ReportViewModel
            {
                SelectedFields = request.SelectedFields,
                SourceTable = request.SourceTable
            };

            if (!FieldRegistry.Map.ContainsKey(request.SourceTable))
                return result;

            switch (request.SourceTable)
            {
                case "Nodes":
                    foreach (var node in _context.Nodes.Include(n => n.BuildingInfo).Include(n => n.NodeType).AsNoTracking())
                    {
                        var row = new ReportRow();
                        foreach (FieldDescriptor<Node> f in FieldRegistry.Map["Nodes"])
                        {
                            if (request.SelectedFields.Contains(f.Name))
                            {
                                row.Fields.Add(new ReportField
                                {
                                    FieldName = f.Name,
                                    DisplayName = f.DisplayName,
                                    Value = f.Value(node)
                                });
                            }
                        }
                        result.Rows.Add(row);
                    }
                    break;

                case "EquipmentModels":
                    foreach (var model in _context.EquipmentModels.Include(m => m.Manufacturer).Include(m => m.Type).AsNoTracking())
                    {
                        var row = new ReportRow();
                        foreach (FieldDescriptor<EquipmentModel> f in FieldRegistry.Map["EquipmentModels"])
                        {
                            if (request.SelectedFields.Contains(f.Name))
                            {
                                row.Fields.Add(new ReportField
                                {
                                    FieldName = f.Name,
                                    DisplayName = f.DisplayName,
                                    Value = f.Value(model)
                                });
                            }
                        }
                        result.Rows.Add(row);
                    }
                    break;

                case "Equipment":
                    foreach (var eq in _context.Equipment
                                               .Include(e => e.Model)
                                               .Include(e => e.Owner)
                                               .Include(e => e.Node)
                                               .AsNoTracking())
                    {
                        var row = new ReportRow();
                        foreach (FieldDescriptor<Equipment> f in FieldRegistry.Map["Equipment"])
                        {
                            if (request.SelectedFields.Contains(f.Name))
                            {
                                row.Fields.Add(new ReportField
                                {
                                    FieldName = f.Name,
                                    DisplayName = f.DisplayName,
                                    Value = f.Value(eq)
                                });
                            }
                        }
                        result.Rows.Add(row);
                    }
                    break;
            }

            return result;
        }

        public byte[] ExportToExcel(ReportRequest request)
        {
            var rpt = GenerateReport(request);
            if (rpt.Rows.Count == 0) return new byte[0];

            var entityType = GetEntityType(request.SourceTable);

            using (var p = new ExcelPackage())
            {
                var ws = p.Workbook.Worksheets.Add("Report");

                for (int c = 0; c < rpt.SelectedFields.Count; c++)
                    ws.Cells[1, c + 1].Value = GetDisplayName(rpt.SelectedFields[c], entityType);

                for (int r = 0; r < rpt.Rows.Count; r++)
                    for (int c = 0; c < rpt.SelectedFields.Count; c++)
                        ws.Cells[r + 2, c + 1].Value =
                            rpt.Rows[r].Fields.FirstOrDefault(f => f.FieldName == rpt.SelectedFields[c])?.Value ?? "";

                return p.GetAsByteArray();
            }
        }

        private Type GetEntityType(string table)
        {
            var prop = _context.GetType().GetProperty(table);
            return prop?.PropertyType.GenericTypeArguments.FirstOrDefault();
        }

        private string GetDisplayName(string propName, Type type)
        {
            var prop = type.GetProperty(propName);
            if (prop == null) return propName;

            var attr = prop.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
            return attr?.Name ?? propName;
        }
    }
}