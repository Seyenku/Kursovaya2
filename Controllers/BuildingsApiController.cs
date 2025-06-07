using Kursovaya.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

[RoutePrefix("api/buildings")]
public class BuildingsApiController : ApiController
{
    private readonly IBuildingService _service;

    public BuildingsApiController(IBuildingService service)
    {
        _service = service;
    }

    [HttpGet, Route("")]
    public async Task<IHttpActionResult> GetAll()
    {
        var list = (await _service.GetAllBuildingsAsync())
            .Select(b => new { id = b.BuildId, name = b.BuildName })
            .ToList();
        return Ok(list);
    }
}
