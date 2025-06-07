using Kursovaya.Data;
using Kursovaya.Models;
using Kursovaya.Models.Reporting;
using Kursovaya.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace Kursovaya.Views.Reports
{
    public partial class Report : Page
    {
        private readonly IReportService _svc = new ReportService(new ApplicationDbContext());
        private readonly JavaScriptSerializer _js = new JavaScriptSerializer();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) return;

            switch (Request.Form["__EVENTTARGET"])
            {
                case "GetConfig":
                    SendJson(_svc.GetReportConfig());
                    break;
                case "GenerateReport":
                    SendJson(Generate());
                    break;
                case "ExportReport":
                    Export();
                    break;
            }

        }

        private object Generate()
        {
            var src = Request.Form["source"];
            var fields = _js.Deserialize<List<string>>(Request.Form["fields"]);

            // Берём отображаемые имена прямо из FieldRegistry
            var descriptors =
                FieldRegistry.Map.TryGetValue(src, out var list) ? list : new List<object>();

            var display = fields.Select(f =>
            {
                var fd = descriptors.Cast<dynamic>().FirstOrDefault(d => d.Name == f);
                return fd != null ? (string)fd.DisplayName : f;      // fallback
            }).ToList();

            var rep = _svc.GenerateReport(new ReportRequest
            {
                SourceTable = src,
                SelectedFields = fields
            });

            var rows = rep.Rows.Select(r =>
                fields.Select(f => r.Fields.FirstOrDefault(x => x.FieldName == f)?.Value ?? "")
                      .ToList())
                .ToList();

            return new { columns = display, rows };
        }

        private void Export()
        {
            var src = Request.Form["source"];
            var encoded = Request.Form["fields"] ?? "";
            var decoded = HttpUtility.UrlDecode(encoded);

            if (string.IsNullOrWhiteSpace(decoded))
            {
                SendJson(new { error = "Empty field list" });
                return;
            }

            List<string> fields;
            try
            {
                fields = _js.Deserialize<List<string>>(decoded);
            }
            catch
            {
                // fallback: "Name,Type,Ip"
                fields = decoded.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(s => s.Trim().Trim('"'))
                                .ToList();
            }

            var bytes = _svc.ExportToExcel(new ReportRequest
            {
                SourceTable = src,
                SelectedFields = fields
            });

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition",
                $"attachment;filename=Report_{src}_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            Response.BinaryWrite(bytes);
            Response.Flush();
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        /* вспомогательные */

        private void SendJson(object payload)
        {
            Response.Clear();
            Response.ContentType = "application/json";
            Response.Write(_js.Serialize(payload));
            try { Response.End(); } catch (ThreadAbortException) { /* подавляем */ }
        }
    }
}
