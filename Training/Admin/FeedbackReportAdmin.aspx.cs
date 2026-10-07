using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class FeedbackReportAdmin : Page
    {
        private readonly clsDataAccess obj = new clsDataAccess();
        protected void Page_Load(object sender, EventArgs e) { if (!IsPostBack) BindReport(); }
        protected void btnSearch_Click(object sender, EventArgs e) { BindReport(); }
        protected void btnReset_Click(object sender, EventArgs e) { txtTrainingID.Text = ""; txtFromDate.Text = ""; txtToDate.Text = ""; BindReport(); }
        private void BindReport() { try { string query = @"SELECT TD.TrainingID,TD.Batch,E.EmpID,E.EmpName,E.EmpDesignation,COUNT(DISTINCT FR.ID) TopicFeedback,COUNT(DISTINCT FTR.ID) TrainingFeedback,COUNT(DISTINCT FO.ID) OverallFeedback,MAX(FR.CreatedOn) LastTopicFeedback,MAX(FTR.CreatedOn) LastTrainingFeedback,MAX(FO.CreatedOn) LastOverallFeedback FROM TrainingDetails TD INNER JOIN TrainingAssignment TA ON TD.TrainingID=TA.TrainingID INNER JOIN EmpBasicMaster E ON TA.EmpID=E.EmpID LEFT JOIN FeedbackReport FR ON TD.TrainingID=FR.TrainingID AND E.EmpID=FR.EmpID LEFT JOIN FeedbackTrainingRelated FTR ON TD.TrainingID=FTR.TrainingID AND E.EmpID=FTR.EmpID LEFT JOIN FeedbackOverall FO ON TD.TrainingID=FO.TrainingID AND E.EmpID=FO.EmpID WHERE (@TrainingID='' OR TD.TrainingID LIKE '%' + @TrainingID + '%') GROUP BY TD.TrainingID,TD.Batch,E.EmpID,E.EmpName,E.EmpDesignation ORDER BY TD.TrainingID,E.EmpID"; SqlParameter[] p = { new SqlParameter("@TrainingID", txtTrainingID.Text.Trim()), new SqlParameter("@FromDate", txtFromDate.Text.Trim()), new SqlParameter("@ToDate", txtToDate.Text.Trim()) }; DataTable dt = obj.GetDataTable(query, p); gvReport.DataSource = dt; gvReport.DataBind(); Session["FeedbackReportAdminData"] = dt; lblMsg.Text = dt.Rows.Count + " record(s) found."; } catch (Exception ex) { lblMsg.Text = ex.Message; } }
        protected void btnExcel_Click(object sender, EventArgs e) { DataTable dt = Session["FeedbackReportAdminData"] as DataTable; if (dt == null) { BindReport(); dt = Session["FeedbackReportAdminData"] as DataTable; } gvReport.DataSource = dt; gvReport.DataBind(); Response.Clear(); Response.Buffer = true; Response.AddHeader("content-disposition", "attachment;filename=FeedbackReportAdmin.xls"); Response.ContentType = "application/ms-excel"; StringWriter sw = new StringWriter(); HtmlTextWriter hw = new HtmlTextWriter(sw); gvReport.RenderControl(hw); Response.Write(sw.ToString()); Response.End(); }
        public override void VerifyRenderingInServerForm(Control control) { }
    }
}