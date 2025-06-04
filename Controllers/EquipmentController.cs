using Kursovaya.Models;
using Kursovaya.Services;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Kursovaya.Controllers
{
    public class EquipmentController : Controller
    {
        private readonly IEquipmentService _equipmentService;
        private readonly IModelService _modelService;
        private readonly INodeService _nodeService;
        private readonly IOwnerService _ownerService;

        public EquipmentController(
            IEquipmentService equipmentService,
            IModelService modelService,
            INodeService nodeService,
            IOwnerService ownerService)
        {
            _equipmentService = equipmentService;
            _modelService = modelService;
            _nodeService = nodeService;
            _ownerService = ownerService;
        }

        // GET: Equipment
        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            var equipment = _equipmentService.GetEquipmentPaged(page, pageSize);
            var totalCount = _equipmentService.GetTotalCount();

            var viewModel = new EquipmentListViewModel
            {
                Equipment = equipment.Select(MapToViewModel).ToList(),
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return View(viewModel);
        }

        // GET: Equipment/Details/5
        public ActionResult Details(int id)
        {
            var equipment = _equipmentService.GetEquipmentById(id);
            if (equipment == null)
            {
                return HttpNotFound();
            }

            var viewModel = MapToViewModel(equipment);
            return View(viewModel);
        }

        // GET: Equipment/Create
        public ActionResult Create()
        {
            var viewModel = new EquipmentViewModel();
            PopulateDropDownLists(viewModel);
            return View(viewModel);
        }

        // POST: Equipment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EquipmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var equipment = new Equipment
                    {
                        ModelId = model.ModelId,
                        NodeId = model.NodeId,
                        OwnerId = model.OwnerId,
                        SerialNumber = model.SerialNumber,
                        InventoryNumber = model.InventoryNumber,
                        Mac = model.Mac,
                        Ip = model.Ip
                    };

                    _equipmentService.CreateEquipment(equipment);
                    TempData["SuccessMessage"] = "Оборудование успешно создано";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Произошла ошибка при создании оборудования: " + ex.Message);
                }
            }

            PopulateDropDownLists(model);
            return View(model);
        }

        // GET: Equipment/Edit/5
        public ActionResult Edit(int id)
        {
            var equipment = _equipmentService.GetEquipmentById(id);
            if (equipment == null)
            {
                return HttpNotFound();
            }

            var viewModel = MapToViewModel(equipment);
            PopulateDropDownLists(viewModel);
            return View(viewModel);
        }

        // POST: Equipment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EquipmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var equipment = _equipmentService.GetEquipmentById(model.Id);
                    if (equipment == null)
                    {
                        return HttpNotFound();
                    }

                    equipment.ModelId = model.ModelId;
                    equipment.NodeId = model.NodeId;
                    equipment.OwnerId = model.OwnerId;
                    equipment.SerialNumber = model.SerialNumber;
                    equipment.InventoryNumber = model.InventoryNumber;
                    equipment.Mac = model.Mac;
                    equipment.Ip = model.Ip;

                    _equipmentService.UpdateEquipment(equipment);
                    TempData["SuccessMessage"] = "Оборудование успешно обновлено";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Произошла ошибка при обновлении оборудования: " + ex.Message);
                }
            }

            PopulateDropDownLists(model);
            return View(model);
        }

        // GET: Equipment/Delete/5
        public ActionResult Delete(int id)
        {
            var equipment = _equipmentService.GetEquipmentById(id);
            if (equipment == null)
            {
                return HttpNotFound();
            }

            var viewModel = MapToViewModel(equipment);
            return View(viewModel);
        }

        // POST: Equipment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _equipmentService.DeleteEquipment(id);
                TempData["SuccessMessage"] = "Оборудование успешно удалено";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Произошла ошибка при удалении оборудования: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // AJAX: Получение оборудования по узлу
        [HttpGet]
        public JsonResult GetEquipmentByNode(int nodeId)
        {
            var equipment = _equipmentService.GetEquipmentByNode(nodeId)
                .Select(e => new
                {
                    Id = e.Id,
                    ModelName = e.Model?.Name,
                    SerialNumber = e.SerialNumber,
                    InventoryNumber = e.InventoryNumber,
                    Mac = e.Mac,
                    Ip = e.Ip
                });

            return Json(equipment, JsonRequestBehavior.AllowGet);
        }

        // AJAX: Проверка уникальности MAC-адреса
        [HttpGet]
        public JsonResult CheckMacUnique(string mac, int? equipmentId = null)
        {
            var isUnique = _equipmentService.IsMacUnique(mac, equipmentId);
            return Json(new { isUnique }, JsonRequestBehavior.AllowGet);
        }

        // AJAX: Проверка уникальности IP-адреса
        [HttpGet]
        public JsonResult CheckIpUnique(string ip, int? equipmentId = null)
        {
            var isUnique = _equipmentService.IsIpUnique(ip, equipmentId);
            return Json(new { isUnique }, JsonRequestBehavior.AllowGet);
        }

        private EquipmentViewModel MapToViewModel(Equipment equipment)
        {
            return new EquipmentViewModel
            {
                Id = equipment.Id,
                ModelId = equipment.ModelId,
                NodeId = equipment.NodeId,
                OwnerId = equipment.OwnerId,
                SerialNumber = equipment.SerialNumber,
                InventoryNumber = equipment.InventoryNumber,
                Mac = equipment.Mac,
                Ip = equipment.Ip,
                ModelName = equipment.Model?.Name,
                NodeName = equipment.Node?.Name,
                OwnerName = equipment.Owner?.Name
            };
        }

        private void PopulateDropDownLists(EquipmentViewModel model)
        {
            model.Models = _modelService.GetAllModels()
                .Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.Name,
                    Selected = m.Id == model.ModelId
                });

            model.Nodes = _nodeService.GetAllNodes()
                .Select(n => new SelectListItem
                {
                    Value = n.Id.ToString(),
                    Text = n.Name,
                    Selected = n.Id == model.NodeId
                });

            model.Owners = _ownerService.GetAllOwners()
                .Select(o => new SelectListItem
                {
                    Value = o.Id.ToString(),
                    Text = o.Name,
                    Selected = o.Id == model.OwnerId
                });
        }
    }
}