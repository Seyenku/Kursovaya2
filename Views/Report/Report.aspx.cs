using Kursovaya.Models;
using Kursovaya.Services;
using System;
using System.Linq;
using System.Web.UI.WebControls;
using Kursovaya.App_Start;
using Unity;

namespace Kursovaya.Views.Reports
{
    public partial class Report : System.Web.UI.Page
    {
        [Dependency]
        public IReportService ReportService { get; set; }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (ReportService == null)
            {
                UnityConfig.Container.BuildUp(this);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var config = ReportService.GetReportConfig();
                ddlSource.DataSource = config.AvailableSources;
                ddlSource.DataBind();
                ddlSource.Items.Insert(0, new ListItem("-- Выберите источник --", ""));
            }
        }

        protected void ddlSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            cblFields.Items.Clear();

            var config = ReportService.GetReportConfig();
            var selected = ddlSource.SelectedValue;

            if (!string.IsNullOrEmpty(selected) && config.AvailableFields.ContainsKey(selected))
            {
                foreach (var field in config.AvailableFields[selected])
                {
                    cblFields.Items.Add(new ListItem(field, field));
                }
            }
        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            var request = BuildRequest();
            var report = ReportService.GenerateReport(request);

            var resultTable = new System.Data.DataTable();
            foreach (var field in report.SelectedFields)
            {
                resultTable.Columns.Add(field);
            }

            foreach (var row in report.Rows)
            {
                var values = report.SelectedFields.Select(f =>
                    row.Fields.FirstOrDefault(x => x.FieldName == f)?.Value ?? string.Empty).ToArray();
                resultTable.Rows.Add(values);
            }

            gvReport.DataSource = resultTable;
            gvReport.DataBind();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            var request = BuildRequest();
            var bytes = ReportService.ExportToExcel(request);

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=report.xlsx");
            Response.BinaryWrite(bytes);
            Response.End();
        }

        private ReportRequest BuildRequest()
        {
            var selectedFields = cblFields.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToList();
            return new ReportRequest
            {
                SourceTable = ddlSource.SelectedValue,
                SelectedFields = selectedFields
            };
        }
    }
}
