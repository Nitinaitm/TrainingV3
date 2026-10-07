using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class AssessmentReport : Page
    {
        private readonly clsDataAccess obj = new clsDataAccess();
        protected void Page_Load(object sender, EventArgs e) { if (!IsPostBack) BindReport(); }
        protected void btnSearch_Click(object sender, EventArgs e) { BindReport(); }
        protected void btnReset_Click(object sender, EventArgs e) { txtTrainingID.Text = \"\"; txtFromDate.Text = \"\"; txtToDate.Text = \"\"; BindReport(); }
        private void BindReport() { try { string query = @\"SELECT TM.TrainingID,TD.Batch,TM.SessionID,TM.TestID,TM.TestType,TM.TestTitle,TM.TotalQuestions,TM.TotalMarks,TM.PassingMarks,TM.PassingPercentage,TM.TestStatus,COUNT(DISTINCT TA.EmpID) Attempted,COUNT(DISTINCT CASE WHEN TA.Submitted=1 THEN TA.EmpID END) Submitted,COUNT(DISTINCT CASE WHEN TR.ResultStatus='Pass' THEN TR.EmpID END) Passed,COUNT(DISTINCT CASE WHEN TR.ResultStatus='Fail' THEN TR.EmpID END) Failed,AVG(CASE WHEN TR.IsFinalAttempt=1 THEN TR.Percentage END) AveragePercentage FROM TestMaster TM INNER JOIN TrainingDetails TD ON TM.TrainingID=TD.TrainingID LEFT JOIN TestAttempt TA ON TM.TestID=TA.TestID LEFT JOIN TestResult TR ON TM.TestID=TR.TestID AND TA.EmpID=TR.EmpID AND TA.AttemptID=TR.AttemptID WHERE (@TrainingID='' OR TM.TrainingID LIKE '%' + @TrainingID + '%') GROUP BY TM.TrainingID,TD.Batch,TM.SessionID,TM.TestID,TM.TestType,TM.TestTitle,TM.TotalQuestions,TM.TotalMarks,TM.PassingMarks,TM.PassingPercentage,TM.TestStatus ORDER BY TM.TrainingID,TM.TestType\"; SqlParameter[] p = { new SqlParameter(\"@TrainingID\", txtTrainingID.Text.Trim()), new SqlParameter(\"@FromDate\", txtFromDate.Text.Trim()), new SqlParameter(\"@ToDate\", txtToDate.Text.Trim()) }; DataTable dt = obj.GetDataTable(query, p); gvReport.DataSource = dt; gvReport.DataBind(); Session[\"AssessmentReportData\"] = dt; lblMsg.Text = dt.Rows.Count + \" record(s) found.\"; } catch (Exception ex) { lblMsg.Text = ex.Message; } }
        protected void btnExcel_Click(object sender, EventArgs e) { DataTable dt = Session[\"AssessmentReportData\"] as DataTable; if (dt == null) { BindReport(); dt = Session[\"AssessmentReportData\"] as DataTable; } gvReport.DataSource = dt; gvReport.DataBind(); Response.Clear(); Response.Buffer = true; Response.AddHeader(\"content-disposition\", \"attachment;filename=AssessmentReport.xls\"); Response.ContentType = \"application/ms-excel\"; StringWriter sw = new StringWriter(); HtmlTextWriter hw = new HtmlTextWriter(sw); gvReport.RenderControl(hw); Response.Write(sw.ToString()); Response.End(); }
        public override void VerifyRenderingInServerForm(Control control) { }
    }
}