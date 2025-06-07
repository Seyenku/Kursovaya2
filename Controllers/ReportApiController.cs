using Kursovaya.Data;
using Kursovaya.Models;
using Kursovaya.Services;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Kursovaya.Controllers
{
    [Route("api/report")]
    public class ReportApiController : ApiController
    {
        private readonly IReportService _svc =
            new ReportService(new ApplicationDbContext());

        [HttpGet, Route("config")]
        public IHttpActionResult GetConfig()
            => Ok(_svc.GetReportConfig());

        [HttpPost, Route("generate")]
        public IHttpActionResult Generate(ReportRequest req)
        {
            var rep = _svc.GenerateReport(req);
            return Ok(new
            {
                columns = req.SelectedFields,
                rows = rep.Rows.ConvertAll(r =>
                    req.SelectedFields.ConvertAll(f =>
                        r.Fields.Find(x => x.FieldName == f)?.Value ?? ""))
            });
        }

        [HttpPost, Route("export")]
        public HttpResponseMessage ExportToExcel(ReportRequest request)
        {
            var fileContent = _svc.ExportToExcel(request);
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(fileContent)
            };
            result.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"report_{request.SourceTable}.xlsx"
                };
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            return result;
        }
    }
}
