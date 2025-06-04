using Kursovaya.Models;
using Kursovaya.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Kursovaya.Controllers
{
    public class NodesController : Controller
    {
        private readonly INodeService _nodeService;
        private readonly IBuildingService _buildingService;
        private readonly INodeTypeService _nodeTypeService;

        public NodesController(INodeService nodeService, IBuildingService buildingService, INodeTypeService nodeTypeService)
        {
            _nodeService = nodeService;
            _buildingService = buildingService;
            _nodeTypeService = nodeTypeService;
        }

        // GET: Nodes
        public ActionResult Index(NodeFilterViewModel filter, int page = 1, int pageSize = 10)
        {
            var nodes = _nodeService.GetFilteredNodes(filter, page, pageSize);
            var totalCount = _nodeService.GetFilteredNodesCount(filter);

            var viewModel = new NodesListViewModel
            {
                Nodes = nodes.Select(MapToViewModel).ToList(),
                Filter = filter ?? new NodeFilterViewModel(),
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                TreeDataJson = GenerateTreeData()
            };

            // Заполняем выпадающие списки для фильтров
            viewModel.Filter.Buildings = _buildingService.GetAllBuildings()
                .Select(b => new SelectListItem { Value = b.BuildId.ToString(), Text = b.BuildName });

            viewModel.Filter.NodeTypes = _nodeTypeService.GetAllNodeTypes()
                .Select(nt => new SelectListItem { Value = nt.Id.ToString(), Text = nt.Name });

            return View(viewModel);
        }

        // GET: Nodes/Details/5
        public ActionResult Details(int id)
        {
            var node = _nodeService.GetNodeById(id);
            if (node == null)
            {
                return HttpNotFound();
            }

            var viewModel = MapToViewModel(node);
            return View(viewModel);
        }

        // GET: Nodes/Create
        public ActionResult Create()
        {
            var viewModel = new NodeViewModel();
            PopulateDropDownLists(viewModel);
            return View(viewModel);
        }

        // POST: Nodes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NodeViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var node = new Node
                    {
                        Name = model.Name,
                        Building = model.Building,
                        Type = model.Type,
                        Other = model.Other,
                        VerificationDate = model.VerificationDate
                    };

                    _nodeService.CreateNode(node);
                    TempData["SuccessMessage"] = "Узел успешно создан";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Произошла ошибка при создании узла: " + ex.Message);
                }
            }

            PopulateDropDownLists(model);
            return View(model);
        }

        // GET: Nodes/Edit/5
        public ActionResult Edit(int id)
        {
            var node = _nodeService.GetNodeById(id);
            if (node == null)
            {
                return HttpNotFound();
            }

            var viewModel = MapToViewModel(node);
            PopulateDropDownLists(viewModel);
            return View(viewModel);
        }

        // POST: Nodes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NodeViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var node = _nodeService.GetNodeById(model.Id);
                    if (node == null)
                    {
                        return HttpNotFound();
                    }

                    node.Name = model.Name;
                    node.Building = model.Building;
                    node.Type = model.Type;
                    node.Other = model.Other;
                    node.VerificationDate = model.VerificationDate;

                    _nodeService.UpdateNode(node);
                    TempData["SuccessMessage"] = "Узел успешно обновлен";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Произошла ошибка при обновлении узла: " + ex.Message);
                }
            }

            PopulateDropDownLists(model);
            return View(model);
        }

        // GET: Nodes/Delete/5
        public ActionResult Delete(int id)
        {
            var node = _nodeService.GetNodeById(id);
            if (node == null)
            {
                return HttpNotFound();
            }

            var viewModel = MapToViewModel(node);
            return View(viewModel);
        }

        // POST: Nodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _nodeService.DeleteNode(id);
                TempData["SuccessMessage"] = "Узел успешно удален";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Произошла ошибка при удалении узла: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // AJAX: Получение узлов для выпадающего списка
        [HttpGet]
        public JsonResult GetNodesByBuilding(int buildingId)
        {
            var nodes = _nodeService.GetNodesByBuilding(buildingId)
                .Select(n => new { Value = n.Id, Text = n.Name });

            return Json(nodes, JsonRequestBehavior.AllowGet);
        }

        // AJAX: Получение данных для дерева
        [HttpGet]
        public JsonResult GetTreeData()
        {
            var treeData = GenerateTreeData();
            return Json(treeData, JsonRequestBehavior.AllowGet);
        }

        private NodeViewModel MapToViewModel(Node node)
        {
            return new NodeViewModel
            {
                Id = node.Id,
                Name = node.Name,
                Building = node.Building,
                Type = node.Type,
                Other = node.Other,
                VerificationDate = node.VerificationDate,
                BuildingName = node.BuildingInfo?.BuildName,
                TypeName = node.NodeType?.Name,
                DeviceCount = node.Equipment?.Count ?? 0
            };
        }

        private void PopulateDropDownLists(NodeViewModel model)
        {
            model.Buildings = _buildingService.GetAllBuildings()
                .Select(b => new SelectListItem
                {
                    Value = b.BuildId.ToString(),
                    Text = b.BuildName,
                    Selected = b.BuildId == model.Building
                });

            model.NodeTypes = _nodeTypeService.GetAllNodeTypes()
                .Select(nt => new SelectListItem
                {
                    Value = nt.Id.ToString(),
                    Text = nt.Name,
                    Selected = nt.Id == model.Type
                });
        }

        private string GenerateTreeData()
        {
            var buildings = _buildingService.GetAllBuildings();
            var treeNodes = new List<TreeNodeViewModel>();

            foreach (var building in buildings)
            {
                var buildingNode = new TreeNodeViewModel
                {
                    Id = $"building_{building.BuildId}",
                    Text = building.BuildName,
                    Icon = "fa fa-building",
                    Children = new List<TreeNodeViewModel>()
                };

                var nodes = _nodeService.GetNodesByBuilding(building.BuildId);
                foreach (var node in nodes)
                {
                    var nodeViewModel = new TreeNodeViewModel
                    {
                        Id = $"node_{node.Id}",
                        Text = node.Name,
                        Icon = "fa fa-sitemap",
                        Data = new { type = "node", id = node.Id }
                    };

                    buildingNode.Children.Add(nodeViewModel);
                }

                treeNodes.Add(buildingNode);
            }

            return JsonConvert.SerializeObject(treeNodes);
        }
    }
}