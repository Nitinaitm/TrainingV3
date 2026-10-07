using System;
using System.Data;
using System.Data.SqlClient;

namespace Training.Trainee
{
    public partial class Default : System.Web.UI.Page
    {
        clsDataAccess objDB = new clsDataAccess();
        string EmpID = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["EmpID"] == null || string.IsNullOrWhiteSpace(Session["EmpID"].ToString()) || Session["Role"] == null || !string.Equals(Session["Role"].ToString(), "Trainee", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }
            EmpID = Session["EmpID"].ToString().Trim().ToUpperInvariant();
            if (!IsPostBack) LoadDashboard();
        }

        private void LoadDashboard()
        {
            LoadTraineeDetails();
            LoadDashboardSummary();
        }

        private void LoadTraineeDetails()
        {
            lblTraineeID.Text = EmpID;
            string sql = "SELECT EmpName,'Internal' AS TraineeType FROM EmpBasicMaster WHERE EmpID=@EmpID UNION ALL SELECT TraineeName AS EmpName,'External' AS TraineeType FROM TraineeMasterExternal WHERE EmpIDExternal=@EmpID";
            DataTable dt = objDB.GetDataTable(sql, new SqlParameter[] { new SqlParameter("@EmpID", EmpID) });
            if (dt == null || dt.Rows.Count == 0) { lblTraineeName.Text = EmpID; lblTraineeType.Text = "Trainee"; return; }
            lblTraineeName.Text = dt.Rows[0]["EmpName"].ToString();
            lblTraineeType.Text = dt.Rows[0]["TraineeType"].ToString();
        }

        private void LoadDashboardSummary()
        {
            string sql = "SELECT " +
                "(SELECT COUNT(DISTINCT TA.TrainingID) FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TD.TrainingID=TA.TrainingID WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND ISNULL(TD.TrainingStatus,'')<>'Closed' AND TRY_CONVERT(date,TD.DateFrom,105)<=CONVERT(date,GETDATE()) AND TRY_CONVERT(date,TD.DateTo,105)>=CONVERT(date,GETDATE())) AS ActiveTraining," +
                "(SELECT COUNT(DISTINCT TA.TrainingID) FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TD.TrainingID=TA.TrainingID WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND ISNULL(TD.TrainingStatus,'')<>'Closed' AND TRY_CONVERT(date,TD.DateTo,105)<CONVERT(date,GETDATE())) AS PreviousTraining," +
                "(SELECT COUNT(DISTINCT TA.TrainingID) FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TD.TrainingID=TA.TrainingID WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND ISNULL(TD.TrainingStatus,'')='Closed') AS CompletedTraining," +
                "(SELECT COUNT(DISTINCT TA.TrainingID) FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TD.TrainingID=TA.TrainingID WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND ISNULL(TD.TrainingStatus,'')<>'Closed' AND TRY_CONVERT(date,TD.DateFrom,105)>CONVERT(date,GETDATE())) AS FutureTraining," +
                "(SELECT COUNT(DISTINCT TM.TestID) FROM TestMaster TM INNER JOIN SessionMaster SM ON SM.SessionID=TM.SessionID INNER JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID INNER JOIN TrainingAssignment TA ON TA.TrainingID=SM.TrainingID WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND TM.IsPublished=1 AND ((TM.TestType='Pre' AND TD.InitialAssessmentRequired=1 AND ISNULL(SM.PreAssessmentSkipped,0)=0) OR (TM.TestType='Post' AND TD.FinalAssessmentRequired=1 AND ISNULL(SM.PostAssessmentSkipped,0)=0)) AND NOT EXISTS (SELECT 1 FROM TestAttempt TAT WHERE TAT.TestID=TM.TestID AND TAT.EmpID=@EmpID AND TAT.Submitted=1)) AS PendingTests," +
                "(SELECT COUNT(DISTINCT TC.CertificateID) FROM TrainingCertificate TC INNER JOIN TrainingAssignment CTA ON CTA.TrainingID=TC.TrainingID AND CTA.EmpID=TC.EmpID WHERE TC.EmpID=@EmpID AND TC.CertificateStatus='A' AND CTA.AssignmentStatus='Assigned') AS CertificateCount";
            DataTable dt = objDB.GetDataTable(sql, new SqlParameter[] { new SqlParameter("@EmpID", EmpID) });
            if (dt == null || dt.Rows.Count == 0) { SetDashboardZero(); return; }
            lblActiveTraining.Text = GetIntValue(dt.Rows[0]["ActiveTraining"]).ToString();
            lblPreviousTraining.Text = GetIntValue(dt.Rows[0]["PreviousTraining"]).ToString();
            lblCompletedTraining.Text = GetIntValue(dt.Rows[0]["CompletedTraining"]).ToString();
            lblFutureTraining.Text = GetIntValue(dt.Rows[0]["FutureTraining"]).ToString();
            lblPendingTests.Text = GetIntValue(dt.Rows[0]["PendingTests"]).ToString();
            lblCertificate.Text = GetIntValue(dt.Rows[0]["CertificateCount"]).ToString();
        }

        private int GetIntValue(object value)
        {
            if (value == null || value == DBNull.Value || string.IsNullOrWhiteSpace(value.ToString())) return 0;
            int result = 0;
            Int32.TryParse(value.ToString(), out result);
            return result;
        }

        private void SetDashboardZero()
        {
            lblActiveTraining.Text="0";
            lblPreviousTraining.Text="0";
            lblCompletedTraining.Text="0";
            lblFutureTraining.Text="0";
            lblPendingTests.Text="0";
            lblCertificate.Text="0";
        }

        private void OpenDashboardDetails(string type)
        {
            Session["TraineeDashboardType"] = type;
            Response.Redirect("~/Trainee/DashboardDetails.aspx");
        }

        protected void lnkActiveTraining_Click(object sender, EventArgs e) { OpenDashboardDetails("Active"); }
        protected void lnkPreviousTraining_Click(object sender, EventArgs e) { OpenDashboardDetails("Previous"); }
        protected void lnkCompletedTraining_Click(object sender, EventArgs e) { OpenDashboardDetails("Completed"); }
        protected void lnkFutureTraining_Click(object sender, EventArgs e) { OpenDashboardDetails("Future"); }
        protected void lnkPendingTests_Click(object sender, EventArgs e) { OpenDashboardDetails("PendingTests"); }
        protected void lnkCertificate_Click(object sender, EventArgs e) { OpenDashboardDetails("Certificates"); }
    }
}