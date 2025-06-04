using System.Web.Mvc;

public class ReportsController : Controller
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    public ActionResult Index()
    {
        var model = _reportService.GetReportConfig();
        return View(model);
    }

    [HttpPost]
    public ActionResult Generate(ReportRequest request)
    {
        var result = _reportService.GenerateReport(request);
        return View("Preview", result);
    }

    [HttpPost]
    public FileResult ExportToExcel(ReportRequest request)
    {
        var fileContent = _reportService.ExportToExcel(request);
        return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "report.xlsx");
    }
}
