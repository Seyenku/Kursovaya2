using Kursovaya.Models;
using Kursovaya.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kursovaya.Services
{
    public class NodeService : INodeService
    {
        private readonly INodeRepository _nodeRepository;

        public NodeService(INodeRepository nodeRepository)
        {
            _nodeRepository = nodeRepository;
        }

        public IEnumerable<Node> GetAllNodes()
        {
            return _nodeRepository.GetAll();
        }

        public Node GetNodeById(int id)
        {
            return _nodeRepository.GetById(id);
        }

        public IEnumerable<Node> GetNodesByBuilding(int buildingId)
        {
            return _nodeRepository.GetByBuilding(buildingId);
        }

        public IEnumerable<Node> GetFilteredNodes(NodeFilterViewModel filter, int page, int pageSize)
        {
            // Применение фильтров
            var query = _nodeRepository.Query();

            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(n => n.Name.Contains(filter.Name));
            }

            if (filter.BuildingId.HasValue)
            {
                query = query.Where(n => n.Building.BuildId == filter.BuildingId);
            }

            if (filter.NodeTypeId.HasValue)
            {
                query = query.Where(n => n.Type.Id == filter.NodeTypeId);
            }

            return query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public int GetFilteredNodesCount(NodeFilterViewModel filter)
        {
            var query = _nodeRepository.Query();

            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(n => n.Name.Contains(filter.Name));
            }

            if (filter.BuildingId.HasValue)
            {
                query = query.Where(n => n.Building.BuildId == filter.BuildingId);
            }

            if (filter.NodeTypeId.HasValue)
            {
                query = query.Where(n => n.Type.Id == filter.NodeTypeId);
            }

            return query.Count();
        }

        public int CreateNode(Node node)
        {
            _nodeRepository.Add(node);
            _nodeRepository.SaveChanges();
            return node.Id;
        }

        public void UpdateNode(Node node)
        {
            _nodeRepository.Update(node);
            _nodeRepository.SaveChanges();
        }

        public void DeleteNode(int id)
        {
            var node = _nodeRepository.GetById(id);
            if (node != null)
            {
                _nodeRepository.Remove(node);
                _nodeRepository.SaveChanges();
            }
        }
    }
}
