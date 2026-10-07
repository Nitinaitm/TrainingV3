using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class CertificateReport : Page
    {
        private readonly clsDataAccess obj = new clsDataAccess();
        protected void Page_Load(object sender, EventArgs e) { if (!IsPostBack) BindReport(); }
        protected void btnSearch_Click(object sender, EventArgs e) { BindReport(); }
        protected void btnReset_Click(object sender, EventArgs e) { txtTrainingID.Text = \"\"; txtFromDate.Text = \"\"; txtToDate.Text = \"\"; BindReport(); }
        private void BindReport() { try { string query = @\"SELECT TC.CertificateID,TC.CertificateNo,TC.TrainingID,E.EmpID,E.EmpName,E.EmpDesignation,TD.Batch,TC.GeneratedOn,TC.GeneratedBy,TC.CertificateStatus,TC.PDFName,TC.DownloadCount,TC.LastDownloadedOn,TC.LastDownloadedBy FROM TrainingCertificate TC LEFT JOIN EmpBasicMaster E ON TC.EmpID=E.EmpID LEFT JOIN TrainingDetails TD ON TC.TrainingID=TD.TrainingID WHERE (@TrainingID='' OR TC.TrainingID LIKE '%' + @TrainingID + '%') ORDER BY TC.GeneratedOn DESC\"; SqlParameter[] p = { new SqlParameter(\"@TrainingID\", txtTrainingID.Text.Trim()), new SqlParameter(\"@FromDate\", txtFromDate.Text.Trim()), new SqlParameter(\"@ToDate\", txtToDate.Text.Trim()) }; DataTable dt = obj.GetDataTable(query, p); gvReport.DataSource = dt; gvReport.DataBind(); Session[\"CertificateReportData\"] = dt; lblMsg.Text = dt.Rows.Count + \" record(s) found.\"; } catch (Exception ex) { lblMsg.Text = ex.Message; } }
        protected void btnExcel_Click(object sender, EventArgs e) { DataTable dt = Session[\"CertificateReportData\"] as DataTable; if (dt == null) { BindReport(); dt = Session[\"CertificateReportData\"] as DataTable; } gvReport.DataSource = dt; gvReport.DataBind(); Response.Clear(); Response.Buffer = true; Response.AddHeader(\"content-disposition\", \"attachment;filename=CertificateReport.xls\"); Response.ContentType = \"application/ms-excel\"; StringWriter sw = new StringWriter(); HtmlTextWriter hw = new HtmlTextWriter(sw); gvReport.RenderControl(hw); Response.Write(sw.ToString()); Response.End(); }
        public override void VerifyRenderingInServerForm(Control control) { }
    }
}