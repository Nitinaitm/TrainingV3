using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainee
{
    public partial class DashboardDetails : System.Web.UI.Page
    {
        private readonly clsDataAccess objDB = new clsDataAccess();
        private string EmpID = "";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ConfigureCertificateGrid();
        }

        private void ConfigureCertificateGrid()
        {
            string type = Session["TraineeDashboardType"] == null ? "" : Session["TraineeDashboardType"].ToString().Trim();
            if (!string.Equals(type, "Certificates", StringComparison.OrdinalIgnoreCase)) return;
            gvDetails.AutoGenerateColumns = false;
            gvDetails.DataKeyNames = new string[] { "CertificateID" };
            gvDetails.Columns.Clear();
            BoundField certificateID = new BoundField();
            certificateID.DataField = "CertificateID";
            certificateID.Visible = false;
            gvDetails.Columns.Add(certificateID);
            BoundField certificateNo = new BoundField();
            certificateNo.DataField = "CertificateNo";
            certificateNo.HeaderText = "Certificate No.";
            gvDetails.Columns.Add(certificateNo);
            BoundField trainingID = new BoundField();
            trainingID.DataField = "TrainingID";
            trainingID.HeaderText = "Training ID";
            gvDetails.Columns.Add(trainingID);
            BoundField courseName = new BoundField();
            courseName.DataField = "CourseName";
            courseName.HeaderText = "Course Name";
            gvDetails.Columns.Add(courseName);
            BoundField generatedOn = new BoundField();
            generatedOn.DataField = "GeneratedOn";
            generatedOn.HeaderText = "Generated On";
            generatedOn.DataFormatString = "{0:dd-MM-yyyy hh:mm tt}";
            gvDetails.Columns.Add(generatedOn);
            BoundField trainingType = new BoundField();
            trainingType.DataField = "TrainingType";
            trainingType.HeaderText = "Training Type";
            gvDetails.Columns.Add(trainingType);
            BoundField organizer = new BoundField();
            organizer.DataField = "TrainingOrganizer";
            organizer.HeaderText = "Training Organizer";
            gvDetails.Columns.Add(organizer);
            BoundField batch = new BoundField();
            batch.DataField = "Batch";
            batch.HeaderText = "Batch";
            gvDetails.Columns.Add(batch);
            BoundField dateFrom = new BoundField();
            dateFrom.DataField = "DateFrom";
            dateFrom.HeaderText = "Date From";
            dateFrom.DataFormatString = "{0:dd-MM-yyyy}";
            gvDetails.Columns.Add(dateFrom);
            BoundField dateTo = new BoundField();
            dateTo.DataField = "DateTo";
            dateTo.HeaderText = "Date To";
            dateTo.DataFormatString = "{0:dd-MM-yyyy}";
            gvDetails.Columns.Add(dateTo);
            TemplateField action = new TemplateField();
            action.HeaderText = "Action";
            action.ItemTemplate = new CertificateDownloadTemplate();
            gvDetails.Columns.Add(action);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["EmpID"] == null || string.IsNullOrWhiteSpace(Session["EmpID"].ToString()) || Session["Role"] == null || !string.Equals(Session["Role"].ToString(), "Trainee", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }
            EmpID = Session["EmpID"].ToString().Trim().ToUpperInvariant();
            if (!IsPostBack)
            {
                string type = Session["TraineeDashboardType"] == null ? "Active" : Session["TraineeDashboardType"].ToString().Trim();
                LoadDetails(type);
            }
        }

        private void LoadDetails(string type)
        {
            DataTable dt;
            string title;
            switch (type)
            {
                case "Active": title = "Active Training"; dt = GetTrainingByCategory("Active"); break;
                case "Previous": title = "Previous Training - Not Closed"; dt = GetTrainingByCategory("Previous"); break;
                case "Completed": title = "Completed Training"; dt = GetTrainingByCategory("Completed"); break;
                case "Future": title = "Future Assigned Training"; dt = GetTrainingByCategory("Future"); break;
                case "Attendance": title = "Attendance Completed"; dt = GetAttendanceCompleted(); break;
                case "PendingTests": title = "Pending Tests"; dt = GetPublishedTests(true); break;
                case "Certificates": title = "All Certificates"; dt = GetCertificates(); gvDetails.DataKeyNames = new string[] { "CertificateID" }; break;
                case "Tests": title = "Published Tests"; dt = GetPublishedTests(false); break;
                default: title = "Active Training"; dt = GetTrainingByCategory("Active"); break;
            }
            lblTitle.Text = title;
            lblSummary.Text = "Showing " + dt.Rows.Count.ToString() + " record(s).";
            gvDetails.DataSource = dt;
            gvDetails.DataBind();
        }

        private DataTable GetTrainingByCategory(string category)
        {
            string condition = category == "Completed" ? "AND UPPER(LTRIM(RTRIM(ISNULL(TD.TrainingStatus,'')))) IN ('CLOSED','COMPLETED')" : category == "Future" ? "AND UPPER(LTRIM(RTRIM(ISNULL(TD.TrainingStatus,'')))) NOT IN ('CLOSED','COMPLETED') AND TRY_CONVERT(date,TD.DateFrom,105)>CONVERT(date,GETDATE())" : category == "Previous" ? "AND UPPER(LTRIM(RTRIM(ISNULL(TD.TrainingStatus,'')))) NOT IN ('CLOSED','COMPLETED') AND TRY_CONVERT(date,TD.DateTo,105)<CONVERT(date,GETDATE())" : "AND UPPER(LTRIM(RTRIM(ISNULL(TD.TrainingStatus,'')))) NOT IN ('CLOSED','COMPLETED') AND TRY_CONVERT(date,TD.DateFrom,105)<=CONVERT(date,GETDATE()) AND TRY_CONVERT(date,TD.DateTo,105)>=CONVERT(date,GETDATE())";
            string sql = "SELECT DISTINCT TD.TrainingID,CM.CourseName,TD.TrainingType,TD.TrainingOrganizer,TD.TrainingLocation,TD.Batch,TRY_CONVERT(date,TD.DateFrom,105) AS DateFrom,TRY_CONVERT(date,TD.DateTo,105) AS DateTo,ISNULL(TD.TrainingStatus,'') AS TrainingStatus FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TD.TrainingID=TA.TrainingID INNER JOIN CourseMaster CM ON CM.CourseID=TD.CourseID WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' " + condition + " ORDER BY TRY_CONVERT(date,TD.DateFrom,105) DESC";
            return GetTable(sql);
        }

        private DataTable GetAttendanceCompleted()
        {
            string sql = "SELECT DISTINCT TD.TrainingID,CM.CourseName,TD.TrainingType,TD.TrainingOrganizer,TD.Batch,TRY_CONVERT(date,TD.DateFrom,105) AS DateFrom,TRY_CONVERT(date,TD.DateTo,105) AS DateTo FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TD.TrainingID=TA.TrainingID INNER JOIN CourseMaster CM ON CM.CourseID=TD.CourseID WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND TD.AttendanceRequired=1 AND EXISTS (SELECT 1 FROM SessionMaster SM WHERE SM.TrainingID=TA.TrainingID AND ISNULL(SM.AttendanceSkipped,0)=0) AND NOT EXISTS (SELECT 1 FROM SessionMaster SM WHERE SM.TrainingID=TA.TrainingID AND ISNULL(SM.AttendanceSkipped,0)=0 AND NOT EXISTS (SELECT 1 FROM SessionAttendance SA WHERE SA.SessionID=SM.SessionID AND SA.EmpID=@EmpID AND SA.AttendanceStatus IN ('Present','Completed'))) ORDER BY TRY_CONVERT(date,TD.DateFrom,105) DESC";
            return GetTable(sql);
        }

        private DataTable GetPublishedTests(bool pendingOnly)
        {
            string completionCondition = "EXISTS (SELECT 1 FROM TestAttempt TA2 WHERE TA2.TestID=TM.TestID AND TA2.EmpID=@EmpID AND TA2.Submitted=1)";
            string whereCompletion = pendingOnly ? "AND NOT " + completionCondition : "";
            string sql = "SELECT TM.TestID,SM.TrainingID,CM.CourseName,SM.SessionNo,SM.SessionName,TM.TestType,TM.IsPublished,CASE WHEN " + completionCondition + " THEN 'Completed' ELSE 'Pending' END AS TestStatus FROM TestMaster TM INNER JOIN SessionMaster SM ON SM.SessionID=TM.SessionID INNER JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID INNER JOIN TrainingAssignment TAA ON TAA.TrainingID=SM.TrainingID INNER JOIN CourseMaster CM ON CM.CourseID=TD.CourseID WHERE TAA.EmpID=@EmpID AND TAA.AssignmentStatus='Assigned' AND ISNULL(TD.TrainingStatus,'') NOT IN ('Closed','Completed') AND TM.IsPublished=1 AND ((TM.TestType='Pre' AND TD.InitialAssessmentRequired=1 AND ISNULL(SM.PreAssessmentSkipped,0)=0) OR (TM.TestType='Post' AND TD.FinalAssessmentRequired=1 AND ISNULL(SM.PostAssessmentSkipped,0)=0)) " + whereCompletion + " GROUP BY TM.TestID,SM.TrainingID,CM.CourseName,SM.SessionNo,SM.SessionName,TM.TestType,TM.IsPublished ORDER BY SM.TrainingID,SM.SessionNo,TM.TestType";
            return GetTable(sql);
        }

        private DataTable GetCertificates()
        {
            string sql = "SELECT DISTINCT TC.CertificateID,TC.CertificateNo,TC.TrainingID,CM.CourseName,TC.GeneratedOn,TD.TrainingType,TD.TrainingOrganizer,TD.Batch,TRY_CONVERT(date,TD.DateFrom,105) AS DateFrom,TRY_CONVERT(date,TD.DateTo,105) AS DateTo,ISNULL(TD.TrainingStatus,'') AS TrainingStatus FROM TrainingCertificate TC INNER JOIN TrainingAssignment TA ON TA.TrainingID=TC.TrainingID AND TA.EmpID=TC.EmpID INNER JOIN TrainingDetails TD ON TD.TrainingID=TC.TrainingID INNER JOIN CourseMaster CM ON CM.CourseID=TD.CourseID WHERE TC.EmpID=@EmpID AND TC.CertificateStatus='A' AND TA.AssignmentStatus='Assigned' ORDER BY TC.GeneratedOn DESC";
            return GetTable(sql);
        }

        private DataTable GetTable(string sql)
        {
            return objDB.GetDataTable(sql, new SqlParameter[] { new SqlParameter("@EmpID", EmpID) });
        }
        public class CertificateDownloadTemplate : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                HyperLink link = new HyperLink();
                link.ID = "lnkDownloadCertificate";
                link.CssClass = "btn btn-sm btn-success";
                link.Text = "Download PDF";
                link.DataBinding += DownloadLink_DataBinding;
                container.Controls.Add(link);
            }

            private void DownloadLink_DataBinding(object sender, EventArgs e)
            {
                HyperLink link = (HyperLink)sender;
                GridViewRow row = (GridViewRow)link.NamingContainer;
                object value = DataBinder.Eval(row.DataItem, "CertificateID");
                string certificateID = value == null ? "" : value.ToString();
                link.NavigateUrl = "DownloadCertificate.aspx?CertificateID=" + System.Web.HttpUtility.UrlEncode(certificateID);
            }
        }
    }
}
