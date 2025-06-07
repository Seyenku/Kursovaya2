using Kursovaya.Data;
using Kursovaya.Data.Repositories;
using Kursovaya.Services;
using System.Web.Http;
using System.Web.Mvc;
using Unity;
using Unity.Lifetime;

namespace Kursovaya.App_Start
{
    public static class DependencyConfig
    {
        public static IUnityContainer Container { get; private set; }

        public static void RegisterDependencies()
        {
            var container = new UnityContainer();

            // DbContext и UnitOfWork
            container.RegisterType<ApplicationDbContext>(new HierarchicalLifetimeManager());
            container.RegisterType<IUnitOfWork, UnitOfWork>(new HierarchicalLifetimeManager());

            // Репозитории и сервисы
            RegisterRepositories(container);
            RegisterServices(container);

            // MVC
            DependencyResolver.SetResolver(new Unity.AspNet.Mvc.UnityDependencyResolver(container));

            // Web API
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);

            // сохранить ссылку
            Container = container;
        }

        private static void RegisterRepositories(IUnityContainer container)
        {
            container.RegisterType(typeof(IRepository<>), typeof(Repository<>));
        }

        private static void RegisterServices(IUnityContainer container)
        {
            container.RegisterType<INodeService, NodeService>();
            container.RegisterType<IBuildingService, BuildingService>();
            container.RegisterType<INodeTypeService, NodeTypeService>();
            container.RegisterType<IReportService, ReportService>();
        }
    }
}
