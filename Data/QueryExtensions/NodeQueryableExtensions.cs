using Kursovaya.Models;
using System.Linq;

namespace Kursovaya.Data.QueryExtentions
{
    public static class NodeQueryableExtensions
    {
        public static IQueryable<Node> ApplyFilter(this IQueryable<Node> query, NodeFilterViewModel filter)
        {
            if (filter == null) return query;

            if (!string.IsNullOrWhiteSpace(filter.NodeNameFilter))
                query = query.Where(n => n.Name.Contains(filter.NodeNameFilter));

            if (filter.BuildingFilter.HasValue)
                query = query.Where(n => n.Building == filter.BuildingFilter.Value);

            if (filter.TypeFilter.HasValue)
                query = query.Where(n => n.Type == filter.TypeFilter.Value);

            return query;
        }
    }
}