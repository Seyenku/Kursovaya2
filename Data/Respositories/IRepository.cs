using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kursovaya.Data.Repositories
{
    // Базовый интерфейс репозитория для подготовки к переносу на .NET Core
    public interface IRepository<T> where T : class
    {
        // Основные CRUD операции
        T GetById(int id);
        T GetById(object id);
        IEnumerable<T> GetAll();
        IQueryable<T> Query();

        // Поиск с условиями
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        T FirstOrDefault(Expression<Func<T, bool>> predicate);

        // Пагинация
        IEnumerable<T> GetPaged(int page, int pageSize);
        IEnumerable<T> GetPaged(int page, int pageSize, Expression<Func<T, bool>> predicate);

        // Подсчет
        int Count();
        int Count(Expression<Func<T, bool>> predicate);
        bool Any(Expression<Func<T, bool>> predicate);

        // Операции изменения
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

        // Сохранение изменений
        void SaveChanges();
    }

    // Интерфейс Unit of Work для управления транзакциями
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> Repository<T>() where T : class;
        void SaveChanges();
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
    }

    // Специализированные репозитории
    public interface INodeRepository : IRepository<Models.Node>
    {
        IEnumerable<Models.Node> GetByBuilding(int buildingId);
        IEnumerable<Models.Node> GetByType(int typeId);
        IEnumerable<Models.Node> GetWithEquipmentCount();
        IEnumerable<Models.Node> GetFiltered(Models.NodeFilterViewModel filter, int page, int pageSize);
        int GetFilteredCount(Models.NodeFilterViewModel filter);
    }

    public interface IEquipmentRepository : IRepository<Models.Equipment>
    {
        IEnumerable<Models.Equipment> GetByNode(int nodeId);
        IEnumerable<Models.Equipment> GetByModel(int modelId);
        IEnumerable<Models.Equipment> GetByOwner(int ownerId);
        bool IsMacUnique(string mac, int? excludeId = null);
        bool IsIpUnique(string ip, int? excludeId = null);
        Models.Equipment GetByMac(string mac);
        Models.Equipment GetByIp(string ip);
    }

    public interface IModelRepository : IRepository<Models.EquipmentModel>
    {
        IEnumerable<Models.EquipmentModel> GetByType(int typeId);
        IEnumerable<Models.EquipmentModel> GetByManufacturer(int manufacturerId);
        IEnumerable<Models.EquipmentModel> GetFiltered(Models.ModelFilterViewModel filter, int page, int pageSize);
        int GetFilteredCount(Models.ModelFilterViewModel filter);
        IEnumerable<Models.ModelPort> GetModelPorts(int modelId);
        void RemoveAllPorts(int modelId);
    }

    public interface IBuildingRepository : IRepository<Models.Building>
    {
        IEnumerable<Models.Building> GetWithNodeCount();
    }

    public interface IPortTypeRepository : IRepository<Models.PortType>
    {
        IEnumerable<Models.PortType> GetByConnectorType(int connectorTypeId);
        IEnumerable<Models.PortType> GetBySpeed(int speedId);
    }
}