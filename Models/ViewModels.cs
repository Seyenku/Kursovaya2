using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Kursovaya.Models
{

    public class NodeFilterViewModel
    {
        [Display(Name = "Здание")]
        public int? BuildingFilter { get; set; }

        [Display(Name = "Тип узла")]
        public int? TypeFilter { get; set; }

        [Display(Name = "Название узла")]
        public string NodeNameFilter { get; set; }

        [Display(Name = "Количество устройств")]
        public int? DeviceCount { get; set; }

        [Display(Name = "Периоды")]
        public IList<string> Periods { get; set; } = new List<string>();

        public IEnumerable<SelectListItem> Buildings { get; set; }
        public IEnumerable<SelectListItem> NodeTypes { get; set; }
    }

    public class EquipmentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Выберите модель")]
        [Display(Name = "Модель")]
        public int ModelId { get; set; }

        [Required(ErrorMessage = "Выберите узел")]
        [Display(Name = "Узел")]
        public int NodeId { get; set; }

        [Required(ErrorMessage = "Выберите владельца")]
        [Display(Name = "Владелец")]
        public int OwnerId { get; set; }

        [Display(Name = "Серийный номер")]
        public string SerialNumber { get; set; }

        [Display(Name = "Инвентарный номер")]
        public string InventoryNumber { get; set; }

        [Display(Name = "MAC-адрес")]
        public string Mac { get; set; }

        [Display(Name = "IP-адрес")]
        public string Ip { get; set; }

        [Display(Name = "Модель")]
        public string ModelName { get; set; }

        [Display(Name = "Узел")]
        public string NodeName { get; set; }

        [Display(Name = "Владелец")]
        public string OwnerName { get; set; }

        public IEnumerable<SelectListItem> Models { get; set; }
        public IEnumerable<SelectListItem> Nodes { get; set; }
        public IEnumerable<SelectListItem> Owners { get; set; }
    }

    public class EquipmentListViewModel
    {
        public List<EquipmentViewModel> Equipment { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
    }

    public class ModelViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название модели обязательно")]
        [Display(Name = "Название модели")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Выберите тип оборудования")]
        [Display(Name = "Тип оборудования")]
        public int TypeId { get; set; }

        [Required(ErrorMessage = "Выберите тип установки")]
        [Display(Name = "Тип установки")]
        public int InstallTypeId { get; set; }

        [Required(ErrorMessage = "Выберите производителя")]
        [Display(Name = "Производитель")]
        public int ManufacturerId { get; set; }

        [Display(Name = "Управляемое")]
        public int Managed { get; set; }

        [Display(Name = "Консольный порт")]
        public int Console { get; set; }

        [Display(Name = "Поддержка PoE")]
        public int Poe { get; set; }

        [Display(Name = "Тип оборудования")]
        public string TypeName { get; set; }

        [Display(Name = "Тип установки")]
        public string InstallTypeName { get; set; }

        [Display(Name = "Производитель")]
        public string ManufacturerName { get; set; }

        public List<ModelPortViewModel> Ports { get; set; }

        public IEnumerable<SelectListItem> Types { get; set; }
        public IEnumerable<SelectListItem> InstallTypes { get; set; }
        public IEnumerable<SelectListItem> Manufacturers { get; set; }
    }

    public class ModelPortViewModel
    {
        public int Id { get; set; }
        public int ModelId { get; set; }
        public int PortTypeId { get; set; }
        public int Quantity { get; set; }
        public string ConnectorName { get; set; }
        public string SpeedName { get; set; }
        public string Description { get; set; }
    }

    public class ModelsListViewModel
    {
        public List<ModelViewModel> Models { get; set; }
        public ModelFilterViewModel Filter { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
    }

    public class ModelFilterViewModel
    {
        [Display(Name = "Производитель")]
        public string ManufacturerFilter { get; set; }

        [Display(Name = "Название модели")]
        public string NameFilter { get; set; }

        [Display(Name = "Тип оборудования")]
        public string EquipmentTypeFilter { get; set; }

        [Display(Name = "Тип установки")]
        public string InstallationTypeFilter { get; set; }

        [Display(Name = "Управляемое")]
        public int? ManagedFilter { get; set; }

        [Display(Name = "Консольный порт")]
        public int? ConsolePortFilter { get; set; }

        [Display(Name = "Поддержка PoE")]
        public int? PoeSupportFilter { get; set; }

        [Display(Name = "Порты")]
        public string PortFilter { get; set; }

        public IEnumerable<SelectListItem> Manufacturers { get; set; }
        public IEnumerable<SelectListItem> EquipmentTypes { get; set; }
        public IEnumerable<SelectListItem> InstallationTypes { get; set; }
    }
}