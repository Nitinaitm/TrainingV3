using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Training.Manager
{
    public partial class Default : Page
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
                if (!LoadManager())
                {
                    Response.Redirect("~/Default.aspx");
                    return;
                }

                LoadDashboard();
            }
        }

        private bool LoadManager()
        {
            DataTable dt = objDB.GetDataTable("SELECT TOP 1 M.ManagerID,M.EmpID,E.EmpName,E.EmpDesignation,E.EmpPostingPlace,M.MapForLocation,M.TrainingLocationID,L.TrainingLocation FROM ManagerMaster M INNER JOIN EmpBasicMaster E ON M.EmpID=E.EmpID LEFT JOIN TrainingLocationMaster L ON M.TrainingLocationID=L.TrainingLocationID WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y'", new SqlParameter[] { new SqlParameter("@ManagerID", ManagerID) });

            if (dt.Rows.Count == 0) return false;

            lblManagerID.Text = dt.Rows[0]["ManagerID"].ToString();
            lblEmpID.Text = dt.Rows[0]["EmpID"].ToString();
            lblName.Text = dt.Rows[0]["EmpName"].ToString();
            lblDesignation.Text = dt.Rows[0]["EmpDesignation"].ToString();
            lblPosting.Text = dt.Rows[0]["EmpPostingPlace"].ToString();
            lblMapForLocation.Text = dt.Rows[0]["MapForLocation"].ToString();

            Session["ManagerMapForLocation"] = dt.Rows[0]["MapForLocation"].ToString();
            Session["ManagerTrainingLocationID"] = dt.Rows[0]["TrainingLocationID"].ToString();

            return true;
        }

        private string GetTrainingLocationID()
        {
            return Session["ManagerTrainingLocationID"] == null ? "" : Session["ManagerTrainingLocationID"].ToString().Trim();
        }

        protected void DashboardLink_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
        {
            string command = Convert.ToString(e.CommandName);
            if (command == "TotalTrainings" || command == "ActiveTrainings" || command == "CompletedTrainings" || command == "Planned" || command == "InProgress" || command == "Completed")
            {
                Session["ManagerTrainingDashboardFilter"] = command;
                Response.Redirect("~/Manager/MyTrainings.aspx");
                return;
            }

            Session["ManagerSessionDashboardFilter"] = command;
            Response.Redirect("~/Manager/MySessions.aspx");
        }

        private void LoadDashboard()
        {
            string locationID = GetTrainingLocationID();
            if (string.IsNullOrWhiteSpace(locationID)) return;

            DataTable training = objDB.GetDataTable("SELECT COUNT(*) TotalTrainings,SUM(CASE WHEN LTRIM(RTRIM(ISNULL(X.TrainingStatus,''))) IN ('Planned','InProgress','AttendanceCompleted') THEN 1 ELSE 0 END) ActiveTrainings,SUM(CASE WHEN LTRIM(RTRIM(ISNULL(X.TrainingStatus,''))) IN ('Completed','TrainingCompleted') THEN 1 ELSE 0 END) CompletedTrainings,SUM(CASE WHEN LTRIM(RTRIM(ISNULL(X.TrainingStatus,'')))='Planned' THEN 1 ELSE 0 END) PlannedTrainings,SUM(CASE WHEN LTRIM(RTRIM(ISNULL(X.TrainingStatus,'')))='InProgress' THEN 1 ELSE 0 END) InProgressTrainings,SUM(CASE WHEN LTRIM(RTRIM(ISNULL(X.TrainingStatus,''))) IN ('Completed','TrainingCompleted') THEN 1 ELSE 0 END) CompletedSummary FROM (SELECT DISTINCT TD.TrainingID,TD.TrainingType,TD.TrainingOrganizer,TD.TrainingLocation,TD.Batch,TD.DateFrom,TD.DateTo,TD.TrainingStatus FROM TrainingDetails TD INNER JOIN TrainingLocationMaster L ON TD.TrainingLocation=L.TrainingLocation WHERE L.TrainingLocationID=@TrainingLocationID AND EXISTS (SELECT 1 FROM ManagerMaster M WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND M.TrainingLocationID=L.TrainingLocationID)) X", new SqlParameter[] { new SqlParameter("@ManagerID",ManagerID),new SqlParameter("@TrainingLocationID",locationID) });

            if (training.Rows.Count > 0)
            {
                DataRow r = training.Rows[0];
                lblTotalTrainings.Text = Convert.ToString(r["TotalTrainings"]);
                lblActiveTrainings.Text = Convert.ToString(r["ActiveTrainings"]);
                lblCompletedTrainings.Text = Convert.ToString(r["CompletedTrainings"]);
                lblPlanned.Text = Convert.ToString(r["PlannedTrainings"]);
                lblInProgress.Text = Convert.ToString(r["InProgressTrainings"]);
                lblCompletedSummary.Text = Convert.ToString(r["CompletedSummary"]);
            }

            DataTable sessions = objDB.GetDataTable("SELECT COUNT(DISTINCT SM.SessionID) TotalSessions,COUNT(DISTINCT CASE WHEN ISNULL(SM.AttendanceStatus,'Pending')='Completed' THEN SM.SessionID END) SessionsCompleted,COUNT(DISTINCT CASE WHEN ISNULL(SM.AttendanceStatus,'Pending')<>'Completed' THEN SM.SessionID END) AttendancePending,COUNT(DISTINCT CASE WHEN TRY_CONVERT(date,SM.SessionDate,105)>=CONVERT(date,GETDATE()) THEN SM.SessionID END) UpcomingSessions FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID INNER JOIN TrainingLocationMaster L ON TD.TrainingLocation=L.TrainingLocation WHERE L.TrainingLocationID=@TrainingLocationID AND EXISTS (SELECT 1 FROM ManagerMaster M WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND M.TrainingLocationID=L.TrainingLocationID)", new SqlParameter[] { new SqlParameter("@ManagerID",ManagerID),new SqlParameter("@TrainingLocationID",locationID) });

            if (sessions.Rows.Count > 0)
            {
                DataRow r = sessions.Rows[0];
                lblTotalSessions.Text = Convert.ToString(r["TotalSessions"]);
                lblSessionsCompleted.Text = Convert.ToString(r["SessionsCompleted"]);
                lblAttendancePending.Text = Convert.ToString(r["AttendancePending"]);
                lblUpcomingSessions.Text = Convert.ToString(r["UpcomingSessions"]);
            }

            DataTable upcoming = objDB.GetDataTable("SELECT TOP 10 TD.TrainingID,TD.Batch,SM.SessionNo,SM.SessionName,SM.SessionDate,ISNULL(SM.AttendanceStatus,'Pending') AttendanceStatus FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID INNER JOIN TrainingLocationMaster L ON TD.TrainingLocation=L.TrainingLocation WHERE L.TrainingLocationID=@TrainingLocationID AND TRY_CONVERT(date,SM.SessionDate,105)>=CONVERT(date,GETDATE()) AND EXISTS (SELECT 1 FROM ManagerMaster M WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND M.TrainingLocationID=L.TrainingLocationID) ORDER BY TRY_CONVERT(date,SM.SessionDate,105),TRY_CONVERT(int,SM.SessionNo),SM.SessionID", new SqlParameter[] { new SqlParameter("@ManagerID",ManagerID),new SqlParameter("@TrainingLocationID",locationID) });

            gvUpcomingSessions.DataSource = upcoming;
            gvUpcomingSessions.DataBind();
        }
    }
}
