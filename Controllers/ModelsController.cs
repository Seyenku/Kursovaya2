using Kursovaya.Models;
using Kursovaya.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Kursovaya.Controllers
{
    public class ModelsController : Controller
    {
        private readonly IModelService _modelService;
        private readonly IEquipmentTypeService _equipmentTypeService;
        private readonly IInstallationTypeService _installationTypeService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPortTypeService _portTypeService;

        public ModelsController(
            IModelService modelService,
            IEquipmentTypeService equipmentTypeService,
            IInstallationTypeService installationTypeService,
            IManufacturerService manufacturerService,
            IPortTypeService portTypeService)
        {
            _modelService = modelService;
            _equipmentTypeService = equipmentTypeService;
            _installationTypeService = installationTypeService;
            _manufacturerService = manufacturerService;
            _portTypeService = portTypeService;
        }

        // GET: Models
        public ActionResult Index(ModelFilterViewModel filter, int page = 1, int pageSize = 10)
        {
            var models = _modelService.GetFilteredModels(filter, page, pageSize);
            var totalCount = _modelService.GetFilteredModelsCount(filter);

            var viewModel = new ModelsListViewModel
            {
                Models = models.Select(MapToViewModel).ToList(),
                Filter = filter ?? new ModelFilterViewModel(),
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            // Заполняем выпадающие списки для фильтров
            PopulateFilterDropDownLists(viewModel.Filter);

            return View(viewModel);
        }

        // GET: Models/Details/5
        public ActionResult Details(int id)
        {
            var model = _modelService.GetModelById(id);
            if (model == null)
            {
                return HttpNotFound();
            }

            var viewModel = MapToViewModel(model);
            return View(viewModel);
        }

        // GET: Models/Create
        public ActionResult Create()
        {
            var viewModel = new ModelViewModel
            {
                Ports = new List<ModelPortViewModel>()
            };
            PopulateDropDownLists(viewModel);
            return View(viewModel);
        }

        // POST: Models/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ModelViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var model = new EquipmentModel
                    {
                        Name = viewModel.Name,
                        TypeId = viewModel.TypeId,
                        InstallTypeId = viewModel.InstallTypeId,
                        ManufacturerId = viewModel.ManufacturerId,
                        Managed = viewModel.Managed,
                        Console = viewModel.Console,
                        Poe = viewModel.Poe
                    };

                    var modelId = _modelService.CreateModel(model);

                    // Добавляем порты
                    if (viewModel.Ports != null && viewModel.Ports.Any())
                    {
                        foreach (var portViewModel in viewModel.Ports.Where(p => p.Quantity > 0))
                        {
                            var modelPort = new ModelPort
                            {
                                ModelId = modelId,
                                PortTypeId = portViewModel.PortTypeId,
                                Quantity = portViewModel.Quantity
                            };
                            _modelService.AddPortToModel(modelPort);
                        }
                    }

                    TempData["SuccessMessage"] = "Модель успешно создана";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Произошла ошибка при создании модели: " + ex.Message);
                }
            }

            PopulateDropDownLists(viewModel);
            return View(viewModel);
        }

        // GET: Models/Edit/5
        public ActionResult Edit(int id)
        {
            var model = _modelService.GetModelById(id);
            if (model == null)
            {
                return HttpNotFound();
            }

            var viewModel = MapToViewModel(model);
            PopulateDropDownLists(viewModel);
            return View(viewModel);
        }

        // POST: Models/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ModelViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var model = _modelService.GetModelById(viewModel.Id);
                    if (model == null)
                    {
                        return HttpNotFound();
                    }

                    model.Name = viewModel.Name;
                    model.TypeId = viewModel.TypeId;
                    model.InstallTypeId = viewModel.InstallTypeId;
                    model.ManufacturerId = viewModel.ManufacturerId;
                    model.Managed = viewModel.Managed;
                    model.Console = viewModel.Console;
                    model.Poe = viewModel.Poe;

                    _modelService.UpdateModel(model);

                    // Обновляем порты
                    _modelService.RemoveAllPortsFromModel(model.Id);
                    if (viewModel.Ports != null && viewModel.Ports.Any())
                    {
                        foreach (var portViewModel in viewModel.Ports.Where(p => p.Quantity > 0))
                        {
                            var modelPort = new ModelPort
                            {
                                ModelId = model.Id,
                                PortTypeId = portViewModel.PortTypeId,
                                Quantity = portViewModel.Quantity
                            };
                            _modelService.AddPortToModel(modelPort);
                        }
                    }

                    TempData["SuccessMessage"] = "Модель успешно обновлена";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Произошла ошибка при обновлении модели: " + ex.Message);
                }
            }

            PopulateDropDownLists(viewModel);
            return View(viewModel);
        }

        // GET: Models/Delete/5
        public ActionResult Delete(int id)
        {
            var model = _modelService.GetModelById(id);
            if (model == null)
            {
                return HttpNotFound();
            }

            var viewModel = MapToViewModel(model);
            return View(viewModel);
        }

        // POST: Models/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _modelService.DeleteModel(id);
                TempData["SuccessMessage"] = "Модель успешно удалена";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Произошла ошибка при удалении модели: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // AJAX: Получение моделей по типу оборудования
        [HttpGet]
        public JsonResult GetModelsByType(int typeId)
        {
            var models = _modelService.GetModelsByType(typeId)
                .Select(m => new { Value = m.Id, Text = m.Name });

            return Json(models, JsonRequestBehavior.AllowGet);
        }

        // AJAX: Получение портов модели
        [HttpGet]
        public JsonResult GetModelPorts(int modelId)
        {
            var ports = _modelService.GetModelPorts(modelId)
                .Select(p => new
                {
                    Id = p.Id,
                    PortTypeId = p.PortTypeId,
                    Quantity = p.Quantity,
                    ConnectorName = p.PortType?.ConnectorType?.Name,
                    SpeedName = p.PortType?.Speed?.Name
                });

            return Json(ports, JsonRequestBehavior.AllowGet);
        }

        private ModelViewModel MapToViewModel(EquipmentModel model)
        {
            return new ModelViewModel
            {
                Id = model.Id,
                Name = model.Name,
                TypeId = model.TypeId,
                InstallTypeId = model.InstallTypeId,
                ManufacturerId = model.ManufacturerId,
                Managed = model.Managed,
                Console = model.Console,
                Poe = model.Poe,
                TypeName = model.Type?.Name,
                InstallTypeName = model.InstallationType?.Name,
                ManufacturerName = model.Manufacturer?.Name,
                Ports = model.Ports?.Select(p => new ModelPortViewModel
                {
                    Id = p.Id,
                    ModelId = p.ModelId,
                    PortTypeId = p.PortTypeId,
                    Quantity = p.Quantity,
                    ConnectorName = p.PortType?.ConnectorType?.Name,
                    SpeedName = p.PortType?.Speed?.Name,
                    Description = $"{p.PortType?.ConnectorType?.Name} {p.PortType?.Speed?.Name}"
                }).ToList() ?? new List<ModelPortViewModel>()
            };
        }

        private void PopulateDropDownLists(ModelViewModel model)
        {
            model.Types = _equipmentTypeService.GetAllTypes()
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Name,
                    Selected = t.Id == model.TypeId
                });

            model.InstallTypes = _installationTypeService.GetAllTypes()
                .Select(it => new SelectListItem
                {
                    Value = it.Id.ToString(),
                    Text = it.Name,
                    Selected = it.Id == model.InstallTypeId
                });

            model.Manufacturers = _manufacturerService.GetAllManufacturers()
                .Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.Name,
                    Selected = m.Id == model.ManufacturerId
                });
        }

        private void PopulateFilterDropDownLists(ModelFilterViewModel filter)
        {
            filter.Manufacturers = _manufacturerService.GetAllManufacturers()
                .Select(m => new SelectListItem { Value = m.Name, Text = m.Name });

            filter.EquipmentTypes = _equipmentTypeService.GetAllTypes()
                .Select(t => new SelectListItem { Value = t.Name, Text = t.Name });

            filter.InstallationTypes = _installationTypeService.GetAllTypes()
                .Select(it => new SelectListItem { Value = it.Name, Text = it.Name });
        }
    }
}