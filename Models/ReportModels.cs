using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kursovaya.Models
{
    public class ReportField
    {
        public string FieldName { get; set; }
        public string DisplayName { get; set; }
        public string Value { get; set; }
    }

    public class ReportRow
    {
        public List<ReportField> Fields { get; set; } = new List<ReportField>();
    }

    public class ReportRequest
    {
        public string SourceTable { get; set; }
        public List<string> SelectedFields { get; set; }
    }

    public class ReportViewModel
    {
        public List<ReportRow> Rows { get; set; } = new List<ReportRow>();
        public List<string> SelectedFields { get; set; }
        public string SourceTable { get; set; }
    }

    public class ReportConfigViewModel
    {
        public List<string> AvailableSources { get; set; }
        public Dictionary<string, List<string>> AvailableFields { get; set; }
    }
}