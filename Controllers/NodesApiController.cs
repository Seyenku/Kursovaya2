using Kursovaya.Models;
using Kursovaya.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

[RoutePrefix("api/nodes")]
public class NodesApiController : ApiController
{
    private readonly INodeService _svc;
    public NodesApiController(INodeService svc) { _svc = svc; }

    [HttpGet, Route("")]
    public async Task<IHttpActionResult> GetFiltered([FromUri] NodeFilterViewModel f)
    {
        var page = await _svc.GetNodesAsync(f, 1, 1000);
        return Ok(page.Items.Select(n => new
        {
            id = n.Id,
            name = n.Name,
            building = n.Building,
            buildname = n.BuildingInfo?.BuildName,
            type = n.Type,
            typeName = n.NodeType?.Name,
            verificationDate = n.VerificationDate?.ToString("yyyy-MM-dd"),
            deviceCount = n.Equipment?.Count ?? 0,
            other = n.Other
        }));
    }

    [HttpGet, Route("tree")]
    public async Task<IHttpActionResult> GetTree()
    {
        var all = await _svc.GetAllAsync();
        var tree = all.GroupBy(n => n.Building)
                      .Select(g => new
                      {
                          id = $"building_{g.Key}",
                          text = g.First().BuildingInfo?.BuildName ?? $"Корпус {g.Key}",
                          icon = "fa fa-building",
                          children = g.Select(n => new
                          {
                              id = $"node_{n.Id}",
                              text = n.Name,
                              icon = "fa fa-sitemap",
                              data = new { nodeId = n.Id }
                          })
                      });
        return Ok(tree);
    }

    // Класс для редактирования узла в таблице
    public class NodeUpdateDto
    {
        public int BuildingId { get; set; }   // корпус
        public string Name { get; set; }
        public int TypeId { get; set; }   // тип узла
        public string Other { get; set; }
        public DateTime? VerificationDate { get; set; }
    }


    [HttpPut, Route("{id:int}")]
    public async Task<IHttpActionResult> Update(int id, [FromBody] NodeUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _svc.UpdateAsync(id, dto).ConfigureAwait(false);
        return Ok();
    }

    [HttpDelete, Route("{id:int}")]
    public async Task<IHttpActionResult> Delete(int id)
    {
        await _svc.DeleteAsync(id).ConfigureAwait(false);
        return Ok();
    }

}
