using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Manager
{
    public partial class SessionAnswerReport : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        private string ManagerID { get { return Session["ManagerID"] == null ? "" : Session["ManagerID"].ToString().Trim(); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ManagerID) || Session["SessionID"] == null || Session["TrainingID"] == null)
            {
                Response.Redirect("~/Manager/SessionReport.aspx");
                return;
            }

            if (!IsPostBack) LoadReport();
        }

        private bool HasAccess()
        {
            object value = obj.ExecuteScalar(
                "SELECT COUNT(*) FROM ManagerMaster M INNER JOIN TrainingLocationMaster L ON M.TrainingLocationID=L.TrainingLocationID INNER JOIN TrainingDetails TD ON TD.TrainingLocation=L.TrainingLocation INNER JOIN SessionMaster SM ON SM.TrainingID=TD.TrainingID WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND TD.TrainingID=@TrainingID AND SM.SessionID=@SessionID",
                new SqlParameter[] { new SqlParameter("@ManagerID",ManagerID),new SqlParameter("@TrainingID",Session["TrainingID"].ToString()),new SqlParameter("@SessionID",Session["SessionID"].ToString()) });

            return value != null && value != DBNull.Value && Convert.ToInt32(value) > 0;
        }

        private void LoadReport()
        {
            if (!HasAccess())
            {
                Response.Redirect("~/Manager/SessionReport.aspx");
                return;
            }

            DataTable session = obj.GetDataTable("SELECT SessionNo,SessionName FROM SessionMaster WHERE SessionID=@SessionID AND TrainingID=@TrainingID",new SqlParameter[]{new SqlParameter("@SessionID",Session["SessionID"].ToString()),new SqlParameter("@TrainingID",Session["TrainingID"].ToString())});

            if (session.Rows.Count > 0)
            {
                lblSession.Text = "Session " + session.Rows[0]["SessionNo"] + " - " + session.Rows[0]["SessionName"];
            }

            string query = "SELECT R.ResultID,R.EmpID,COALESCE(E.EmpName,T.TraineeName,R.EmpID) AS TraineeName,TM.TestType,TM.TestTitle,R.AttemptNo,R.Percentage,R.ResultStatus FROM TestResult R INNER JOIN TestMaster TM ON R.TestID=TM.TestID INNER JOIN SessionMaster SM ON TM.SessionID=SM.SessionID LEFT JOIN EmpBasicMaster E ON R.EmpID=E.EmpID LEFT JOIN TraineeMasterExternal T ON R.EmpID=T.EmpIDExternal INNER JOIN TrainingDetails TD ON TM.TrainingID=TD.TrainingID INNER JOIN TrainingLocationMaster L ON TD.TrainingLocation=L.TrainingLocation INNER JOIN ManagerMaster M ON M.TrainingLocationID=L.TrainingLocationID WHERE R.IsFinalAttempt=1 AND TM.SessionID=@SessionID AND SM.TrainingID=@TrainingID AND M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y'";

            query += " ORDER BY TM.TestType,R.EmpID";

            gvAnswers.DataSource = obj.GetDataTable(query,new SqlParameter[]{new SqlParameter("@SessionID",Session["SessionID"].ToString()),new SqlParameter("@TrainingID",Session["TrainingID"].ToString()),new SqlParameter("@ManagerID",ManagerID)});
            gvAnswers.DataBind();
        }

        protected void gvAnswers_RowCommand(object sender,GridViewCommandEventArgs e)
        {
            if (e.CommandName != "ViewAnswers") return;

            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;

            if (row.RowIndex < 0 || row.RowIndex >= gvAnswers.DataKeys.Count) return;

            Session["ManagerAnswerResultID"] = gvAnswers.DataKeys[row.RowIndex].Value.ToString();
            Response.Redirect("~/Manager/AnswerDetails.aspx");
        }

        protected void btnBack_Click(object sender,EventArgs e)
        {
            Response.Redirect("~/Manager/SessionReportDetails.aspx");
        }
    }
}