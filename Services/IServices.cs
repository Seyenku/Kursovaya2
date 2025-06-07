using Kursovaya.Models;
using Kursovaya.Models.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using static NodesApiController;

namespace Kursovaya.Services
{
    // Сервис для работы с узлами
    public interface INodeService
    {
        // Async методы для Core
        Task<PagedResult<Node>> GetNodesAsync(NodeFilterViewModel filter, int page, int pageSize);
        Task<IReadOnlyList<Node>> GetAllAsync();
        Task<Node> GetByIdAsync(int id);
        Task<int> CreateAsync(Node node);
        Task UpdateAsync(int id, NodeUpdateDto dto);
        Task DeleteAsync(int id);
    }

    // Сервис для работы со зданиями
    public interface IBuildingService
    {
        Task<IEnumerable<Building>> GetAllBuildingsAsync();
    }

    // Сервис для работы с типами узлов
    public interface INodeTypeService
    {
        Task<IEnumerable<NodeType>> GetAllNodeTypesAsync();
    }

    // Сервис для работы с отчетом
    public interface IReportService
    {
        ReportConfigViewModel GetReportConfig();
        ReportViewModel GenerateReport(ReportRequest request);
        byte[] ExportToExcel(ReportRequest request);
    }
}