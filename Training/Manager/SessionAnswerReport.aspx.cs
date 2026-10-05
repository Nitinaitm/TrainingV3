using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Manager
{
    public partial class SessionAnswerReport : System.Web.UI.Page
    {
        clsDataAccess obj=new clsDataAccess();

        private string ManagerID { get { return Session["ManagerID"] == null ? "" : Session["ManagerID"].ToString().Trim(); } }

        protected void Page_Load(object sender,EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(ManagerID)||Session["SessionID"]==null||string.IsNullOrWhiteSpace(Session["SessionID"].ToString())||Session["TrainingID"]==null||string.IsNullOrWhiteSpace(Session["TrainingID"].ToString()))
            {
                Response.Redirect("~/Manager/SessionReport.aspx");
                return;
            }

            if(!IsPostBack)
                LoadReport();
        }

        private bool HasAccess()
        {
            object v=obj.ExecuteScalar("SELECT COUNT(*) FROM ManagerMaster M INNER JOIN TrainingLocationMaster L ON M.TrainingLocationID=L.TrainingLocationID INNER JOIN TrainingDetails TD ON TD.TrainingLocation=L.TrainingLocation INNER JOIN SessionMaster SM ON SM.TrainingID=TD.TrainingID WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND TD.TrainingID=@TrainingID AND SM.SessionID=@SessionID",new SqlParameter[]{new SqlParameter("@ManagerID",ManagerID),new SqlParameter("@TrainingID",Session["TrainingID"].ToString()),new SqlParameter("@SessionID",Session["SessionID"].ToString())});
            return v!=null&&v!=DBNull.Value&&Convert.ToInt32(v)>0;
        }

        private void LoadReport()
        {
            if(!HasAccess())
            {
                Response.Redirect("~/Manager/SessionReport.aspx");
                return;
            }

            DataTable s=obj.GetDataTable("SELECT SessionNo,SessionName FROM SessionMaster WHERE SessionID=@SessionID AND TrainingID=@TrainingID",new SqlParameter[]{new SqlParameter("@SessionID",Session["SessionID"].ToString()),new SqlParameter("@TrainingID",Session["TrainingID"].ToString())});

            if(s.Rows.Count>0)
                lblSession.Text="Session "+s.Rows[0]["SessionNo"]+" - "+s.Rows[0]["SessionName"];

            string q="SELECT R.ResultID,R.EmpID,COALESCE(E.EmpName,T.TraineeName,R.EmpID) AS TraineeName,TM.TestType,TM.TestTitle,R.AttemptNo,R.Percentage,R.ResultStatus FROM TestResult R INNER JOIN TestMaster TM ON R.TestID=TM.TestID INNER JOIN SessionMaster SM ON TM.SessionID=SM.SessionID LEFT JOIN EmpBasicMaster E ON R.EmpID=E.EmpID LEFT JOIN TraineeMasterExternal T ON R.EmpID=T.EmpIDExternal INNER JOIN TrainingDetails TD ON TM.TrainingID=TD.TrainingID INNER JOIN TrainingLocationMaster L ON TD.TrainingLocation=L.TrainingLocation INNER JOIN ManagerMaster M ON M.TrainingLocationID=L.TrainingLocationID WHERE R.IsFinalAttempt=1 AND TM.SessionID=@SessionID AND SM.TrainingID=@TrainingID AND M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' ORDER BY TM.TestType,R.EmpID";

            gvAnswers.DataSource=obj.GetDataTable(q,new SqlParameter[]{new SqlParameter("@SessionID",Session["SessionID"].ToString()),new SqlParameter("@TrainingID",Session["TrainingID"].ToString()),new SqlParameter("@ManagerID",ManagerID)});
            gvAnswers.DataBind();
        }

        protected void gvAnswers_RowCommand(object s,GridViewCommandEventArgs e)
        {
            if(e.CommandName!="ViewAnswers") return;
            GridViewRow row=(GridViewRow)((Control)e.CommandSource).NamingContainer;
            if(row.RowIndex<0||row.RowIndex>=gvAnswers.DataKeys.Count) return;
            Session["ManagerAnswerResultID"]=gvAnswers.DataKeys[row.RowIndex].Value.ToString();
            Response.Redirect("~/Manager/AnswerDetails.aspx");
        }

        protected void btnBack_Click(object s,EventArgs e)
        {
            Response.Redirect("~/Manager/SessionReportDetails.aspx");
        }
    }
}