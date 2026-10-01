using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Training.Manager
{
    public partial class MySessions : Page
    {
        private readonly clsDataAccess objDB = new clsDataAccess();
        private string ManagerID { get { return Session["ManagerID"] == null ? "" : Session["ManagerID"].ToString().Trim(); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ManagerID))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindSessions();
            }
        }

        protected void gvSessions_RowCommand(object sender,System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if(!string.Equals(e.CommandName,"ViewSession",StringComparison.OrdinalIgnoreCase))return;
            string sessionID=Convert.ToString(e.CommandArgument);
            DataTable dt=objDB.GetDataTable("SELECT TOP 1 SM.TrainingID FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID INNER JOIN TrainingLocationMaster L ON TD.TrainingLocation=L.TrainingLocation INNER JOIN ManagerMaster M ON M.TrainingLocationID=L.TrainingLocationID WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND SM.SessionID=@SessionID",new SqlParameter[]{new SqlParameter("@ManagerID",ManagerID),new SqlParameter("@SessionID",sessionID)});
            if(dt.Rows.Count==0)return;
            Session["TrainingID"]=dt.Rows[0]["TrainingID"].ToString();
            Session["SessionID"]=sessionID;
            Response.Redirect("~/Manager/TrainingDetails.aspx");
        }

        private void BindSessions()
        {
            string dashboardFilter = Session["ManagerSessionDashboardFilter"] == null ? "" : Session["ManagerSessionDashboardFilter"].ToString();
            string filter = "";
            if(dashboardFilter=="UpcomingSessions") filter = " AND TRY_CONVERT(date,SM.SessionDate,105)>=CONVERT(date,GETDATE())";
            else if(dashboardFilter=="AttendancePending") filter = " AND ISNULL(SM.AttendanceStatus,'Pending')<>'Completed'";
            else if(dashboardFilter=="SessionsCompleted") filter = " AND ISNULL(SM.AttendanceStatus,'Pending')='Completed'";
            DataTable dt = objDB.GetDataTable("SELECT TD.TrainingID,TD.Batch,SM.SessionID,SM.SessionNo,SM.SessionName,SM.SessionDate,SM.TrainerID,ISNULL(SM.AttendanceStatus,'Pending') AttendanceStatus FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID INNER JOIN TrainingLocationMaster L ON TD.TrainingLocation=L.TrainingLocation WHERE L.TrainingLocationID=(SELECT TOP 1 M.TrainingLocationID FROM ManagerMaster M WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y') AND EXISTS (SELECT 1 FROM ManagerMaster M WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND M.TrainingLocationID=L.TrainingLocationID)" + filter + " ORDER BY TRY_CONVERT(date,SM.SessionDate,105),TRY_CONVERT(int,SM.SessionNo),SM.SessionID", new SqlParameter[] { new SqlParameter("@ManagerID",ManagerID) });
            gvSessions.DataSource = dt;
            gvSessions.DataBind();
        }
    }
}