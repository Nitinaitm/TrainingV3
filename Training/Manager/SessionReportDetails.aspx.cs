using System;
using System.Data;
using System.Data.SqlClient;

namespace Training.Manager
{
    public partial class SessionReportDetails : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        private string ManagerID
        {
            get { return Session["ManagerID"] == null ? "" : Session["ManagerID"].ToString().Trim(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ManagerID) || Session["SessionID"] == null || Session["TrainingID"] == null)
            {
                Response.Redirect("~/Manager/MyTrainings.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSession();
            }
        }

        private bool HasAccess()
        {
            object value = obj.ExecuteScalar(
                "SELECT COUNT(*) FROM ManagerMaster M INNER JOIN TrainingLocationMaster L ON M.TrainingLocationID=L.TrainingLocationID INNER JOIN TrainingDetails TD ON TD.TrainingLocation=L.TrainingLocation INNER JOIN SessionMaster SM ON SM.TrainingID=TD.TrainingID WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND TD.TrainingID=@TrainingID AND SM.SessionID=@SessionID",
                new SqlParameter[] { new SqlParameter("@ManagerID", ManagerID), new SqlParameter("@TrainingID", Session["TrainingID"].ToString()), new SqlParameter("@SessionID", Session["SessionID"].ToString()) });

            return value != null && value != DBNull.Value && Convert.ToInt32(value) > 0;
        }

        private void LoadSession()
        {
            if (!HasAccess())
            {
                Response.Redirect("~/Manager/SessionReport.aspx");
                return;
            }

            DataTable dt = obj.GetDataTable(
                "SELECT TOP 1 SM.TrainingID,CM.CourseName,TD.Batch,SM.SessionNo,SM.SessionName,ISNULL(TP.TopicName,'') TopicName,SM.SessionDate,SM.StartTime,SM.EndTime FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID LEFT JOIN TopicMaster TP ON SM.TopicID=TP.TopicID WHERE SM.SessionID=@SessionID AND SM.TrainingID=@TrainingID",
                new SqlParameter[] { new SqlParameter("@SessionID", Session["SessionID"].ToString()), new SqlParameter("@TrainingID", Session["TrainingID"].ToString()) });

            if (dt.Rows.Count == 0)
            {
                Response.Redirect("~/Manager/SessionReport.aspx");
                return;
            }

            DataRow row = dt.Rows[0];

            lblTrainingID.Text = row["TrainingID"].ToString();
            lblCourse.Text = row["CourseName"].ToString();
            lblBatch.Text = row["Batch"].ToString();
            lblSession.Text = row["SessionNo"].ToString() + " - " + row["SessionName"].ToString();
            lblTopic.Text = row["TopicName"].ToString();
            lblDate.Text = row["SessionDate"].ToString();
            lblStart.Text = row["StartTime"].ToString();
            lblEnd.Text = row["EndTime"].ToString();
        }

        private void SetReportMode(string type)
        {
            Session["SessionReportTestType"] = type;
            Session["ManagerReportFromDetails"] = "ExamResultReport";
            Response.Redirect("~/Manager/ExamResultReport.aspx");
        }

        protected void btnAttendance_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Manager/SessionAttendanceReport.aspx");
        }

        protected void btnPreTest_Click(object sender, EventArgs e)
        {
            SetReportMode("Pre");
        }

        protected void btnPostTest_Click(object sender, EventArgs e)
        {
            SetReportMode("Post");
        }

        protected void btnTestResult_Click(object sender, EventArgs e)
        {
            Session["SessionReportTestType"] = "";
            Session["ManagerReportFromDetails"] = "ExamResultReport";
            Response.Redirect("~/Manager/ExamResultReport.aspx");
        }

        protected void btnAnswers_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Manager/SessionAnswerReport.aspx");
        }

        protected void btnFeedback_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Manager/FeedbackReport.aspx");
        }

        protected void btnCertificate_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Manager/SessionCertificateReport.aspx");
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Session.Remove("SessionID");
            Session.Remove("TrainingID");
            Session.Remove("SessionReportTestType");
            Session.Remove("ManagerReportFromDetails");
            Response.Redirect("~/Manager/SessionReport.aspx");
        }
    }
}