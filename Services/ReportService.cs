using Kursovaya.Data;
using Kursovaya.Models;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;

namespace Kursovaya.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public ReportConfigViewModel GetReportConfig()
        {
            return new ReportConfigViewModel
            {
                AvailableSources = new List<string> { "Nodes", "Equipment", "Models" },
                AvailableFields = new Dictionary<string, List<string>>
                {
                    { "Nodes", new List<string> { "Name", "Building", "Type" } },
                    { "Equipment", new List<string> { "Mac", "Ip", "SerialNumber" } },
                    { "Models", new List<string> { "Name", "Manufacturer", "Type" } }
                }
            };
        }

        private string GetDisplayName(string field)
        {
            switch (field)
            {
                case "Name": return "Название";
                case "Building": return "Здание";
                case "Type": return "Тип";
                case "Mac": return "MAC";
                case "Ip": return "IP";
                case "SerialNumber": return "Серийный номер";
                case "Manufacturer": return "Производитель";
                default: return field;
            }
        }

        public ReportViewModel GenerateReport(ReportRequest request)
        {
            var rows = new List<ReportRow>();

            switch (request.SourceTable)
            {
                case "Nodes":
                    var nodes = _context.Nodes.ToList();
                    foreach (var node in nodes)
                    {
                        var row = new ReportRow();
                        if (request.SelectedFields.Contains("Name"))
                            row.Fields.Add(new ReportField { FieldName = "Name", DisplayName = "Название", Value = node.Name });
                        if (request.SelectedFields.Contains("Building"))
                            row.Fields.Add(new ReportField { FieldName = "Building", DisplayName = "Здание", Value = node.BuildingInfo?.BuildName });
                        if (request.SelectedFields.Contains("Type"))
                            row.Fields.Add(new ReportField { FieldName = "Type", DisplayName = "Тип", Value = node.NodeType?.Name });
                        rows.Add(row);
                    }
                    break;

                case "Equipment":
                    var equipment = _context.Equipment.ToList();
                    foreach (var eq in equipment)
                    {
                        var row = new ReportRow();
                        if (request.SelectedFields.Contains("Mac"))
                            row.Fields.Add(new ReportField { FieldName = "Mac", DisplayName = "MAC", Value = eq.Mac });
                        if (request.SelectedFields.Contains("Ip"))
                            row.Fields.Add(new ReportField { FieldName = "Ip", DisplayName = "IP", Value = eq.Ip });
                        if (request.SelectedFields.Contains("SerialNumber"))
                            row.Fields.Add(new ReportField { FieldName = "SerialNumber", DisplayName = "Серийный номер", Value = eq.SerialNumber });
                        rows.Add(row);
                    }
                    break;

                case "Models":
                    var models = _context.EquipmentModels.ToList();
                    foreach (var model in models)
                    {
                        var row = new ReportRow();
                        if (request.SelectedFields.Contains("Name"))
                            row.Fields.Add(new ReportField { FieldName = "Name", DisplayName = "Название", Value = model.Name });
                        if (request.SelectedFields.Contains("Manufacturer"))
                            row.Fields.Add(new ReportField { FieldName = "Manufacturer", DisplayName = "Производитель", Value = model.Manufacturer?.Name });
                        if (request.SelectedFields.Contains("Type"))
                            row.Fields.Add(new ReportField { FieldName = "Type", DisplayName = "Тип", Value = model.Type?.Name });
                        rows.Add(row);
                    }
                    break;
            }

            return new ReportViewModel { Rows = rows, SelectedFields = request.SelectedFields, SourceTable = request.SourceTable };
        }

        public byte[] ExportToExcel(ReportRequest request)
        {
            var report = GenerateReport(request);
            if (report.Rows == null || !report.Rows.Any())
                return new byte[0];

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Report");

                // Заголовки
                for (int i = 0; i < request.SelectedFields.Count; i++)
                {
                    string field = request.SelectedFields[i];
                    string header = GetDisplayName(field);
                    ws.Cells[1, i + 1].Value = header;
                }

                // Данные
                for (int rowIndex = 0; rowIndex < report.Rows.Count; rowIndex++)
                {
                    var row = report.Rows[rowIndex];
                    for (int colIndex = 0; colIndex < request.SelectedFields.Count; colIndex++)
                    {
                        string fieldName = request.SelectedFields[colIndex];
                        string value = row.Fields.FirstOrDefault(f => f.FieldName == fieldName)?.Value ?? "";
                        ws.Cells[rowIndex + 2, colIndex + 1].Value = value;
                    }
                }

                return package.GetAsByteArray();
            }
        }
    }
}
