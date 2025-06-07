using Kursovaya.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

[RoutePrefix("api/nodetypes")]
public class NodeTypesApiController : ApiController
{
    private readonly INodeTypeService _service;

    public NodeTypesApiController(INodeTypeService service)
    {
        _service = service;
    }

    [HttpGet, Route("")]
    public async Task<IHttpActionResult> GetAll()
    {
        var list = (await _service.GetAllNodeTypesAsync())
            .Select(t => new { id = t.Id, name = t.Name })
            .ToList();
        return Ok(list);
    }
}
