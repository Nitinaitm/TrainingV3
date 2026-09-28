using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainer
{
    public partial class SessionAnswerReport : System.Web.UI.Page
    {
        clsDataAccess obj=new clsDataAccess();
        private string TrainerID { get { return Session["TrainerID"].ToString(); } }
        protected void Page_Load(object sender,EventArgs e){if(Session["TrainerID"]==null||Session["SessionID"]==null||Session["TrainingID"]==null){Response.Redirect("~/Trainer/SessionReport.aspx");return;}if(!IsPostBack)LoadReport();}
        private bool HasAccess(){object v=obj.ExecuteScalar("SELECT COUNT(*) FROM SessionMaster WHERE SessionID=@SessionID AND TrainingID=@TrainingID AND TrainerID=@TrainerID",new SqlParameter[]{new SqlParameter("@SessionID",Session["SessionID"].ToString()),new SqlParameter("@TrainingID",Session["TrainingID"].ToString()),new SqlParameter("@TrainerID",TrainerID)});return v!=null&&v!=DBNull.Value&&Convert.ToInt32(v)>0;}
        private void LoadReport(){if(!HasAccess()){Response.Redirect("~/Trainer/SessionReport.aspx");return;}DataTable s=obj.GetDataTable("SELECT SessionNo,SessionName FROM SessionMaster WHERE SessionID=@SessionID",new SqlParameter[]{new SqlParameter("@SessionID",Session["SessionID"].ToString())});if(s.Rows.Count>0)lblSession.Text="Session "+s.Rows[0]["SessionNo"]+" - "+s.Rows[0]["SessionName"];string q="SELECT R.ResultID,R.EmpID,COALESCE(E.EmpName,T.TraineeName,R.EmpID) AS TraineeName,TM.TestType,TM.TestTitle,R.AttemptNo,R.Percentage,R.ResultStatus FROM TestResult R INNER JOIN TestMaster TM ON R.TestID=TM.TestID INNER JOIN SessionMaster SM ON TM.SessionID=SM.SessionID LEFT JOIN EmpBasicMaster E ON R.EmpID=E.EmpID LEFT JOIN TraineeMasterExternal T ON R.EmpID=T.EmpIDExternal WHERE R.IsFinalAttempt=1 AND TM.SessionID=@SessionID AND SM.TrainerID=@TrainerID ORDER BY TM.TestType,R.EmpID";gvAnswers.DataSource=obj.GetDataTable(q,new SqlParameter[]{new SqlParameter("@SessionID",Session["SessionID"].ToString()),new SqlParameter("@TrainerID",TrainerID)});gvAnswers.DataBind();}
        protected void gvAnswers_RowCommand(object s,GridViewCommandEventArgs e)
{
    if (e.CommandName != "ViewAnswers") return;
    GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
    if (row.RowIndex < 0 || row.RowIndex >= gvAnswers.DataKeys.Count) return;
    Session["TrainerAnswerResultID"] = gvAnswers.DataKeys[row.RowIndex].Value.ToString();
    Response.Redirect("~/Trainer/AnswerDetails.aspx");
}
        protected void btnBack_Click(object s,EventArgs e){Response.Redirect("~/Trainer/SessionReportDetails.aspx");}
    }
}