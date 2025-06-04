using Kursovaya.Models;
using System.Collections.Generic;

namespace Kursovaya.Services
{
    // Сервис для работы с узлами
    public interface INodeService
    {
        IEnumerable<Node> GetAllNodes();
        Node GetNodeById(int id);
        IEnumerable<Node> GetNodesByBuilding(int buildingId);
        IEnumerable<Node> GetFilteredNodes(NodeFilterViewModel filter, int page, int pageSize);
        int GetFilteredNodesCount(NodeFilterViewModel filter);
        int CreateNode(Node node);
        void UpdateNode(Node node);
        void DeleteNode(int id);
    }

    // Сервис для работы с оборудованием
    public interface IEquipmentService
    {
        IEnumerable<Equipment> GetAllEquipment();
        Equipment GetEquipmentById(int id);
        IEnumerable<Equipment> GetEquipmentByNode(int nodeId);
        IEnumerable<Equipment> GetEquipmentPaged(int page, int pageSize);
        int GetTotalCount();
        int CreateEquipment(Equipment equipment);
        void UpdateEquipment(Equipment equipment);
        void DeleteEquipment(int id);
        bool IsMacUnique(string mac, int? equipmentId = null);
        bool IsIpUnique(string ip, int? equipmentId = null);
    }

    // Сервис для работы с моделями оборудования
    public interface IModelService
    {
        IEnumerable<EquipmentModel> GetAllModels();
        EquipmentModel GetModelById(int id);
        IEnumerable<EquipmentModel> GetModelsByType(int typeId);
        IEnumerable<EquipmentModel> GetFilteredModels(ModelFilterViewModel filter, int page, int pageSize);
        int GetFilteredModelsCount(ModelFilterViewModel filter);
        IEnumerable<ModelPort> GetModelPorts(int modelId);
        int CreateModel(EquipmentModel model);
        void UpdateModel(EquipmentModel model);
        void DeleteModel(int id);
        void AddPortToModel(ModelPort modelPort);
        void RemoveAllPortsFromModel(int modelId);
    }

    // Сервис для работы со зданиями
    public interface IBuildingService
    {
        IEnumerable<Building> GetAllBuildings();
        Building GetBuildingById(int id);
        int CreateBuilding(Building building);
        void UpdateBuilding(Building building);
        void DeleteBuilding(int id);
    }

    // Сервис для работы с типами узлов
    public interface INodeTypeService
    {
        IEnumerable<NodeType> GetAllNodeTypes();
        NodeType GetNodeTypeById(int id);
        int CreateNodeType(NodeType nodeType);
        void UpdateNodeType(NodeType nodeType);
        void DeleteNodeType(int id);
    }

    // Сервис для работы с типами оборудования
    public interface IEquipmentTypeService
    {
        IEnumerable<EquipmentType> GetAllTypes();
        EquipmentType GetTypeById(int id);
        int CreateType(EquipmentType type);
        void UpdateType(EquipmentType type);
        void DeleteType(int id);
    }

    // Сервис для работы с типами установки
    public interface IInstallationTypeService
    {
        IEnumerable<InstallationType> GetAllTypes();
        InstallationType GetTypeById(int id);
        int CreateType(InstallationType type);
        void UpdateType(InstallationType type);
        void DeleteType(int id);
    }

    // Сервис для работы с производителями
    public interface IManufacturerService
    {
        IEnumerable<Manufacturer> GetAllManufacturers();
        Manufacturer GetManufacturerById(int id);
        int CreateManufacturer(Manufacturer manufacturer);
        void UpdateManufacturer(Manufacturer manufacturer);
        void DeleteManufacturer(int id);
    }

    // Сервис для работы с владельцами
    public interface IOwnerService
    {
        IEnumerable<Owner> GetAllOwners();
        Owner GetOwnerById(int id);
        int CreateOwner(Owner owner);
        void UpdateOwner(Owner owner);
        void DeleteOwner(int id);
    }

    // Сервис для работы с типами портов
    public interface IPortTypeService
    {
        IEnumerable<PortType> GetAllPortTypes();
        PortType GetPortTypeById(int id);
        IEnumerable<ConnectorType> GetAllConnectorTypes();
        IEnumerable<PortSpeed> GetAllPortSpeeds();
        int CreatePortType(PortType portType);
        void UpdatePortType(PortType portType);
        void DeletePortType(int id);
    }
}