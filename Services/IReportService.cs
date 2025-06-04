using Kursovaya.Models;
using System.Collections.Generic;

namespace Kursovaya.Services
{
    public interface IReportService
    {
        ReportConfigViewModel GetReportConfig();
        ReportViewModel GenerateReport(ReportRequest request);
        byte[] ExportToExcel(ReportRequest request);
    }
}
