using Kursovaya.App_Start;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Kursovaya.Core.Preparation
{
    // Адаптеры для подготовки к переносу на .NET Core

    // 1. Асинхронные версии интерфейсов сервисов
    public interface IAsyncNodeService
    {
        Task<IEnumerable<Models.Node>> GetAllNodesAsync();
        Task<Models.Node> GetNodeByIdAsync(int id);
        Task<IEnumerable<Models.Node>> GetNodesByBuildingAsync(int buildingId);
        Task<PagedResult<Models.Node>> GetFilteredNodesAsync(Models.NodeFilterViewModel filter, int page, int pageSize);
        Task<int> CreateNodeAsync(Models.Node node);
        Task UpdateNodeAsync(Models.Node node);
        Task DeleteNodeAsync(int id);
    }

    public interface IAsyncEquipmentService
    {
        Task<IEnumerable<Models.Equipment>> GetAllEquipmentAsync();
        Task<Models.Equipment> GetEquipmentByIdAsync(int id);
        Task<IEnumerable<Models.Equipment>> GetEquipmentByNodeAsync(int nodeId);
        Task<PagedResult<Models.Equipment>> GetEquipmentPagedAsync(int page, int pageSize);
        Task<int> CreateEquipmentAsync(Models.Equipment equipment);
        Task UpdateEquipmentAsync(Models.Equipment equipment);
        Task DeleteEquipmentAsync(int id);
        Task<bool> IsMacUniqueAsync(string mac, int? equipmentId = null);
        Task<bool> IsIpUniqueAsync(string ip, int? equipmentId = null);
    }

    // 2. Результат пагинации для .NET Core
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;
    }

    // 3. Конфигурация для .NET Core
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; }
        public int CommandTimeout { get; set; } = 30;
        public bool EnableRetryOnFailure { get; set; } = true;
        public int MaxRetryCount { get; set; } = 3;
        public TimeSpan MaxRetryDelay { get; set; } = TimeSpan.FromSeconds(30);
        public bool EnableSensitiveDataLogging { get; set; } = false;
        public bool EnableDetailedErrors { get; set; } = false;
    }

    public class AppSettings
    {
        public DatabaseSettings Database { get; set; }
        public PaginationSettings Pagination { get; set; }
        public ExportSettings Export { get; set; }
        public LoggingSettings Logging { get; set; }
    }

    public class PaginationSettings
    {
        public int DefaultPageSize { get; set; } = 10;
        public int MaxPageSize { get; set; } = 100;
    }

    public class ExportSettings
    {
        public string[] AllowedFormats { get; set; } = { "xlsx", "csv", "json" };
        public int MaxRecordsPerExport { get; set; } = 10000;
        public string TempPath { get; set; } = "~/App_Data/Temp";
    }

    public class LoggingSettings
    {
        public string LogLevel { get; set; } = "Information";
        public string LogPath { get; set; } = "~/App_Data/Logs";
        public bool EnableFileLogging { get; set; } = true;
        public bool EnableConsoleLogging { get; set; } = true;
        public int MaxLogFileSizeMB { get; set; } = 50;
        public int MaxLogFiles { get; set; } = 10;
    }

    // 4. Middleware для подготовки к .NET Core
    public interface IMiddleware
    {
        Task InvokeAsync(System.Web.HttpContext context, Func<Task> next);
    }

    public class ErrorHandlingMiddleware : IMiddleware
    {
        private readonly ILoggingService _logger;

        public ErrorHandlingMiddleware(ILoggingService logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(System.Web.HttpContext context, Func<Task> next)
        {
            try
            {
                await next();
            }
            catch (Exception ex)
            {
                _logger.LogError("Unhandled exception occurred", ex);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(System.Web.HttpContext context, Exception exception)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var response = new
            {
                error = "Internal Server Error",
                message = exception.Message,
                timestamp = DateTime.UtcNow
            };

            context.Response.Write(JsonConvert.SerializeObject(response));
        }
    }

    // 5. DTOs для API подготовки
    public class NodeDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Building { get; set; }
        [Required]
        public int Type { get; set; }
        public string Other { get; set; }
        public DateTime? VerificationDate { get; set; }
        public string BuildingName { get; set; }
        public string TypeName { get; set; }
        public int DeviceCount { get; set; }
    }

    public class EquipmentDto
    {
        public int Id { get; set; }
        [Required]
        public int ModelId { get; set; }
        [Required]
        public int NodeId { get; set; }
        [Required]
        public int OwnerId { get; set; }
        public string SerialNumber { get; set; }
        public string InventoryNumber { get; set; }
        public string Mac { get; set; }
        public string Ip { get; set; }
        public string ModelName { get; set; }
        public string NodeName { get; set; }
        public string OwnerName { get; set; }
    }

    public class ModelDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int TypeId { get; set; }
        [Required]
        public int InstallTypeId { get; set; }
        [Required]
        public int ManufacturerId { get; set; }
        public bool Managed { get; set; }
        public bool Console { get; set; }
        public bool Poe { get; set; }
        public string TypeName { get; set; }
        public string InstallTypeName { get; set; }
        public string ManufacturerName { get; set; }
        public IEnumerable<ModelPortDto> Ports { get; set; }
    }

    public class ModelPortDto
    {
        public int Id { get; set; }
        public int ModelId { get; set; }
        public int PortTypeId { get; set; }
        public int Quantity { get; set; }
        public string ConnectorName { get; set; }
        public string SpeedName { get; set; }
        public string Description { get; set; }
    }

    // 6. Фильтры для API
    public class NodeFilterDto
    {
        public int? BuildingFilter { get; set; }
        public int? NodeTypeFilter { get; set; }
        public string NameContains { get; set; }
    }

    public class EquipmentFilterDto
    {
        public int? NodeId { get; set; }
        public int? ModelId { get; set; }
        public int? OwnerId { get; set; }
        public string Mac { get; set; }
        public string Ip { get; set; }
    }

    public class ModelFilterDto
    {
        public int? TypeId { get; set; }
        public int? ManufacturerId { get; set; }
        public string NameContains { get; set; }
    }

    public interface IAsyncModelService
    {
        Task<IEnumerable<Models.EquipmentModel>> GetAllModelsAsync();
        Task<Models.EquipmentModel> GetModelByIdAsync(int id);
        Task<IEnumerable<Models.EquipmentModel>> GetFilteredModelsAsync(ModelFilterDto filter, int page, int pageSize);
        Task<int> GetFilteredModelsCountAsync(ModelFilterDto filter);
        Task<IEnumerable<Models.ModelPort>> GetModelPortsAsync(int modelId);
        Task<int> CreateModelAsync(Models.EquipmentModel model);
        Task UpdateModelAsync(Models.EquipmentModel model);
        Task DeleteModelAsync(int id);
        Task AddPortToModelAsync(Models.ModelPort modelPort);
        Task RemoveAllPortsFromModelAsync(int modelId);
    }
}