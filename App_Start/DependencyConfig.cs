using Kursovaya.Data;
using Kursovaya.Data.Repositories;
using Kursovaya.Services;
using System.Collections.Generic;
using System.Web.Mvc;
using Unity;

namespace Kursovaya.App_Start
{
    public static class DependencyConfig
    {
        public static IUnityContainer Container { get; private set; }

        public static void RegisterDependencies()
        {
            var container = new UnityContainer();

            // Регистрация DbContext
            container.RegisterType<ApplicationDbContext>(
                new Unity.Lifetime.HierarchicalLifetimeManager());

            // Регистрация Unit of Work
            container.RegisterType<IUnitOfWork, UnitOfWork>(
                new Unity.Lifetime.HierarchicalLifetimeManager());

            // Регистрация репозиториев
            RegisterRepositories(container);

            // Регистрация сервисов
            RegisterServices(container);

            // Установка резолвера зависимостей для MVC
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        private static void RegisterRepositories(IUnityContainer container)
        {
            // Базовые репозитории
            container.RegisterType(typeof(IRepository<>), typeof(Repository<>));

            // Специализированные репозитории
            container.RegisterType<INodeRepository, NodeRepository>();
            container.RegisterType<IEquipmentRepository, EquipmentRepository>();
            container.RegisterType<IModelRepository, ModelRepository>();
            container.RegisterType<IBuildingRepository, BuildingRepository>();
            container.RegisterType<IPortTypeRepository, PortTypeRepository>();
        }

        private static void RegisterServices(IUnityContainer container)
        {
            // Основные сервисы
            container.RegisterType<INodeService, NodeService>();
            container.RegisterType<IEquipmentService, EquipmentService>();
            container.RegisterType<IModelService, ModelService>();
            container.RegisterType<IBuildingService, BuildingService>();
            container.RegisterType<INodeTypeService, NodeTypeService>();

            // Справочные сервисы
            container.RegisterType<IEquipmentTypeService, EquipmentTypeService>();
            container.RegisterType<IInstallationTypeService, InstallationTypeService>();
            container.RegisterType<IManufacturerService, ManufacturerService>();
            container.RegisterType<IOwnerService, OwnerService>();
            container.RegisterType<IPortTypeService, PortTypeService>();

            // Дополнительные сервисы для подготовки к .NET Core
            container.RegisterType<IValidationService, ValidationService>();
            container.RegisterType<IExportService, ExportService>();
            container.RegisterType<IImportService, ImportService>();
            container.RegisterType<ILoggingService, LoggingService>();
        }
    }

    // Дополнительные сервисы для подготовки к переносу на .NET Core
    public interface IValidationService
    {
        ValidationResult ValidateNode(Models.NodeViewModel model);
        ValidationResult ValidateEquipment(Models.EquipmentViewModel model);
        ValidationResult ValidateModel(Models.ModelViewModel model);
    }

    public interface IExportService
    {
        byte[] ExportNodesToExcel();
        byte[] ExportEquipmentToExcel();
        byte[] ExportModelsToExcel();
        string ExportNodesToJson();
        string ExportEquipmentToJson();
    }

    public interface IImportService
    {
        ImportResult ImportNodesFromExcel(byte[] fileData);
        ImportResult ImportEquipmentFromExcel(byte[] fileData);
        ImportResult ImportFromJson(string jsonData);
    }

    public interface ILoggingService
    {
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message, System.Exception exception = null);
        void LogDebug(string message);
    }

    // Результаты операций
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
    }

    public class ImportResult
    {
        public bool Success { get; set; }
        public int ProcessedRecords { get; set; }
        public int SuccessfulRecords { get; set; }
        public int FailedRecords { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
    }
}