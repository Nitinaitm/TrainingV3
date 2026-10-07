using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainee
{
    public partial class Default : System.Web.UI.Page
    {
        clsDataAccess objDB = new clsDataAccess();
        string EmpID = "";

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["EmpID"] == null || string.IsNullOrWhiteSpace(Session["EmpID"].ToString())) return;
            if (Session["Role"] == null || !string.Equals(Session["Role"].ToString(), "Trainee", StringComparison.OrdinalIgnoreCase)) return;
            EmpID = Session["EmpID"].ToString().Trim().ToUpperInvariant();
            BindClosedTraining();
        }

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
            LoadProgress();
        }

        private void LoadTraineeDetails()
        {
            lblTraineeID.Text = EmpID;
            string sql = "SELECT EmpName,'Internal' AS TraineeType FROM EmpBasicMaster WHERE EmpID=@EmpID UNION ALL SELECT TraineeName AS EmpName,'External' AS TraineeType FROM TraineeMasterExternal WHERE EmpIDExternal=@EmpID";
            SqlParameter[] param = { new SqlParameter("@EmpID", EmpID) };
            DataTable dt = objDB.GetDataTable(sql, param);
            if (dt == null || dt.Rows.Count == 0)
            {
                lblTraineeName.Text = EmpID;
                lblTraineeType.Text = "Trainee";
                return;
            }
            lblTraineeName.Text = dt.Rows[0]["EmpName"].ToString();
            lblTraineeType.Text = dt.Rows[0]["TraineeType"].ToString();
        }

        private void LoadDashboardSummary()
        {
            string sql = "SELECT " +
                "(SELECT COUNT(DISTINCT TA.TrainingID) FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TD.TrainingID=TA.TrainingID WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND ISNULL(TD.TrainingStatus,'')<>'Closed' AND TRY_CONVERT(date,TD.DateFrom,105)<=CONVERT(date,GETDATE()) AND TRY_CONVERT(date,TD.DateTo,105)>=CONVERT(date,GETDATE())) AS ActiveTraining," +
                "(SELECT COUNT(DISTINCT TA.TrainingID) FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TD.TrainingID=TA.TrainingID WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND ISNULL(TD.TrainingStatus,'')='Closed') AS CompletedTraining," +
                "(SELECT COUNT(DISTINCT TA.TrainingID) FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TD.TrainingID=TA.TrainingID WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND ISNULL(TD.TrainingStatus,'')<>'Closed' AND TRY_CONVERT(date,TD.DateFrom,105)>CONVERT(date,GETDATE())) AS FutureTraining," +
                "(SELECT COUNT(DISTINCT TM.TestID) FROM TestMaster TM INNER JOIN SessionMaster SM ON SM.SessionID=TM.SessionID INNER JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID INNER JOIN TrainingAssignment TA ON TA.TrainingID=SM.TrainingID WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND TM.IsPublished=1 AND ((TM.TestType='Pre' AND TD.InitialAssessmentRequired=1 AND ISNULL(SM.PreAssessmentSkipped,0)=0) OR (TM.TestType='Post' AND TD.FinalAssessmentRequired=1 AND ISNULL(SM.PostAssessmentSkipped,0)=0)) AND NOT EXISTS (SELECT 1 FROM TestAttempt TAT WHERE TAT.TestID=TM.TestID AND TAT.EmpID=@EmpID AND TAT.Submitted=1)) AS PendingTests," +
                "(SELECT COUNT(DISTINCT TA.TrainingID) FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TD.TrainingID=TA.TrainingID WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND TD.FeedbackRequired=1 AND ISNULL(TD.FeedbackSkipped,0)=0 AND NOT EXISTS (SELECT 1 FROM Feedback F WHERE F.TrainingID=TA.TrainingID AND F.EmpID=@EmpID AND ISNULL(F.Submitted,0)=1)) AS FeedbackPending," +
                "(SELECT COUNT(DISTINCT TC.CertificateID) FROM TrainingCertificate TC INNER JOIN TrainingAssignment CTA ON CTA.TrainingID=TC.TrainingID AND CTA.EmpID=TC.EmpID WHERE TC.EmpID=@EmpID AND TC.CertificateStatus='A' AND CTA.AssignmentStatus='Assigned') AS CertificateCount";
            DataTable dt = objDB.GetDataTable(sql, new SqlParameter[] { new SqlParameter("@EmpID", EmpID) });
            if (dt == null || dt.Rows.Count == 0) { SetDashboardZero(); return; }
            lblActiveTraining.Text = GetIntValue(dt.Rows[0]["ActiveTraining"]).ToString();
            lblCompletedTraining.Text = GetIntValue(dt.Rows[0]["CompletedTraining"]).ToString();
            lblFutureTraining.Text = GetIntValue(dt.Rows[0]["FutureTraining"]).ToString();
            lblPendingTests.Text = GetIntValue(dt.Rows[0]["PendingTests"]).ToString();
            lblFeedbackPending.Text = GetIntValue(dt.Rows[0]["FeedbackPending"]).ToString();
            lblCertificate.Text = GetIntValue(dt.Rows[0]["CertificateCount"]).ToString();
            lblStatusTraining.Text = lblActiveTraining.Text;
            lblStatusTests.Text = lblPendingTests.Text;
            lblStatusPendingTests.Text = lblPendingTests.Text;
            lblStatusFeedback.Text = lblFeedbackPending.Text;
            lblStatusCertificate.Text = lblCertificate.Text;
        }

        private void BindClosedTraining()
        {
            if (string.IsNullOrWhiteSpace(EmpID)) return;
            string sql = "SELECT DISTINCT TD.TrainingID,TD.TrainingType,TD.TrainingOrganizer,TD.Batch,CONVERT(varchar(10),TRY_CONVERT(date,TD.DateFrom,105),105) AS DateFrom,CONVERT(varchar(10),TRY_CONVERT(date,TD.DateTo,105),105) AS DateTo FROM TrainingDetails TD INNER JOIN TrainingAssignment TA ON TA.TrainingID=TD.TrainingID WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND TD.TrainingStatus='Closed' ORDER BY TRY_CONVERT(date,TD.DateFrom,105) DESC";
            DataTable dt = objDB.GetDataTable(sql, new SqlParameter[] { new SqlParameter("@EmpID", EmpID) });
            ContentPlaceHolder content = Master.FindControl("ContentPlaceHolder1") as ContentPlaceHolder;
            if (content == null) return;
            Literal heading = new Literal();
            heading.Text = "<div class='dashboard-section' style='margin-top:22px;'><div class='section-heading'><i class='fa fa-history'></i> Closed Trainings</div>";
            content.Controls.Add(heading);
            GridView gv = new GridView();
            gv.ID = "gvClosedTraining";
            gv.AutoGenerateColumns = false;
            gv.CssClass = "table table-bordered table-hover";
            gv.Width = Unit.Percentage(100);
            gv.EmptyDataText = "No Closed Training";
            gv.ShowHeaderWhenEmpty = true;
            gv.DataKeyNames = new[] { "TrainingID" };
            gv.RowCommand += gvClosedTraining_RowCommand;
            gv.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(108,117,125);
            gv.HeaderStyle.ForeColor = System.Drawing.Color.White;
            gv.HeaderStyle.Font.Bold = true;
            gv.Columns.Add(new BoundField { DataField = "TrainingID", HeaderText = "Training ID" });
            gv.Columns.Add(new BoundField { DataField = "TrainingType", HeaderText = "Training Type" });
            gv.Columns.Add(new BoundField { DataField = "TrainingOrganizer", HeaderText = "Organizer" });
            gv.Columns.Add(new BoundField { DataField = "Batch", HeaderText = "Batch" });
            gv.Columns.Add(new BoundField { DataField = "DateFrom", HeaderText = "From" });
            gv.Columns.Add(new BoundField { DataField = "DateTo", HeaderText = "To" });
            TemplateField status = new TemplateField();
            status.HeaderText = "Status";
            status.ItemTemplate = new ClosedStatusTemplate();
            gv.Columns.Add(status);
            TemplateField history = new TemplateField();
            history.HeaderText = "History";
            history.ItemTemplate = new ClosedHistoryTemplate();
            gv.Columns.Add(history);
            gv.DataSource = dt;
            gv.DataBind();
            content.Controls.Add(gv);
            content.Controls.Add(new Literal { Text = "</div>" });
        }

        private sealed class ClosedStatusTemplate : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                Label label = new Label { Text = "Closed", CssClass = "badge bg-secondary" };
                container.Controls.Add(label);
            }
        }

        private sealed class ClosedHistoryTemplate : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                Button button = new Button { Text = "View History", CommandName = "History", CssClass = "btn btn-secondary btn-sm", CausesValidation = false };
                container.Controls.Add(button);
            }
        }

        private void gvClosedTraining_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "History") return;
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            GridView gv = (GridView)sender;
            string trainingID = gv.DataKeys[row.RowIndex].Value.ToString();
            object access = objDB.ExecuteScalar("SELECT COUNT(*) FROM TrainingAssignment WHERE TrainingID=@TrainingID AND EmpID=@EmpID AND AssignmentStatus='Assigned' AND EXISTS (SELECT 1 FROM TrainingDetails TD WHERE TD.TrainingID=TrainingAssignment.TrainingID AND TD.TrainingStatus='Closed')", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@EmpID", EmpID) });
            if (access == null || Convert.ToInt32(access) == 0) return;
            Response.Redirect("~/TrainingHistory.aspx?TrainingID=" + Server.UrlEncode(trainingID));
        }

        private int GetRequiredAttendanceCount()
        {
            string sql = "SELECT COUNT(DISTINCT TA.TrainingID) FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TD.TrainingID=TA.TrainingID WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND TD.AttendanceRequired=1 AND EXISTS (SELECT 1 FROM SessionMaster SM WHERE SM.TrainingID=TA.TrainingID AND ISNULL(SM.AttendanceSkipped,0)=0)";
            DataTable dt = objDB.GetDataTable(sql, new SqlParameter[] { new SqlParameter("@EmpID", EmpID) });
            if (dt == null || dt.Rows.Count == 0) return 0;
            return GetIntValue(dt.Rows[0][0]);
        }

        private void LoadProgress()
        {
            string attendanceTotalSql = "SELECT COUNT(*) FROM SessionMaster SM INNER JOIN TrainingAssignment TA ON TA.TrainingID=SM.TrainingID INNER JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND TD.AttendanceRequired=1 AND ISNULL(SM.AttendanceSkipped,0)=0";
            string attendanceDoneSql = "SELECT COUNT(*) FROM SessionMaster SM INNER JOIN TrainingAssignment TA ON TA.TrainingID=SM.TrainingID INNER JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID INNER JOIN SessionAttendance SA ON SA.SessionID=SM.SessionID AND SA.EmpID=TA.EmpID WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND TD.AttendanceRequired=1 AND ISNULL(SM.AttendanceSkipped,0)=0 AND SA.AttendanceStatus IN ('Present','Completed')";
            string testsTotalSql = "SELECT COUNT(DISTINCT TM.TestID) FROM TestMaster TM INNER JOIN SessionMaster SM ON SM.SessionID=TM.SessionID INNER JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID INNER JOIN TrainingAssignment TA ON TA.TrainingID=SM.TrainingID WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND TM.IsPublished=1 AND ((TM.TestType='Pre' AND TD.InitialAssessmentRequired=1 AND ISNULL(SM.PreAssessmentSkipped,0)=0) OR (TM.TestType='Post' AND TD.FinalAssessmentRequired=1 AND ISNULL(SM.PostAssessmentSkipped,0)=0))";
            string testsDoneSql = "SELECT COUNT(DISTINCT TM.TestID) FROM TestMaster TM INNER JOIN SessionMaster SM ON SM.SessionID=TM.SessionID INNER JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID INNER JOIN TrainingAssignment TA ON TA.TrainingID=SM.TrainingID INNER JOIN TestAttempt TAT ON TAT.TestID=TM.TestID AND TAT.EmpID=TA.EmpID AND TAT.Submitted=1 WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND TM.IsPublished=1 AND ((TM.TestType='Pre' AND TD.InitialAssessmentRequired=1 AND ISNULL(SM.PreAssessmentSkipped,0)=0) OR (TM.TestType='Post' AND TD.FinalAssessmentRequired=1 AND ISNULL(SM.PostAssessmentSkipped,0)=0))";
            string feedbackTotalSql = "SELECT COUNT(DISTINCT TA.TrainingID) FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TD.TrainingID=TA.TrainingID WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND TD.FeedbackRequired=1 AND ISNULL(TD.FeedbackSkipped,0)=0";
            string feedbackDoneSql = "SELECT COUNT(DISTINCT F.TrainingID) FROM Feedback F INNER JOIN TrainingAssignment TA ON TA.TrainingID=F.TrainingID AND TA.EmpID=F.EmpID INNER JOIN TrainingDetails TD ON TD.TrainingID=F.TrainingID WHERE F.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND TD.FeedbackRequired=1 AND ISNULL(TD.FeedbackSkipped,0)=0 AND ISNULL(F.Submitted,0)=1";
            string certificateTotalSql = "SELECT COUNT(DISTINCT TA.TrainingID) FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TD.TrainingID=TA.TrainingID WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND TD.CertificateRequired=1 AND ISNULL(TD.CertificateSkipped,0)=0";
            string certificateDoneSql = "SELECT COUNT(DISTINCT TC.TrainingID) FROM TrainingCertificate TC INNER JOIN TrainingAssignment TA ON TA.TrainingID=TC.TrainingID AND TA.EmpID=TC.EmpID WHERE TC.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND TC.CertificateStatus='A'";

            int attendanceTotal = GetCount(attendanceTotalSql);
            int attendanceDone = GetCount(attendanceDoneSql);
            int testsTotal = GetCount(testsTotalSql);
            int testsDone = GetCount(testsDoneSql);
            int feedbackTotal = GetCount(feedbackTotalSql);
            int feedbackDone = GetCount(feedbackDoneSql);
            int certificateTotal = GetCount(certificateTotalSql);
            int certificateDone = GetCount(certificateDoneSql);

            lblProgressAttendance.Text = attendanceDone + "/" + attendanceTotal;
            SetProgressBar(barAttendance, attendanceDone, attendanceTotal);
            lblProgressTests.Text = testsDone + "/" + testsTotal;
            SetProgressBar(barTests, testsDone, testsTotal);
            lblProgressFeedback.Text = feedbackDone + "/" + feedbackTotal;
            SetProgressBar(barFeedback, feedbackDone, feedbackTotal);
            lblProgressCertificate.Text = certificateDone + "/" + certificateTotal;
            SetProgressBar(barCertificate, certificateDone, certificateTotal);
        }

        private int GetCount(string sql)
        {
            object value = objDB.ExecuteScalar(sql, new SqlParameter[] { new SqlParameter("@EmpID", EmpID) });
            return GetIntValue(value);
        }

        private void SetProgressBar(Panel panel, int completed, int total)
        {
            int percentage = 0;
            if (total > 0) percentage = Convert.ToInt32((completed * 100.0) / total);
            if (percentage > 100) percentage = 100;
            if (percentage < 0) percentage = 0;
            panel.Style["width"] = percentage + "%";
            panel.Attributes["aria-valuenow"] = percentage.ToString();
            panel.Attributes["aria-valuemin"] = "0";
            panel.Attributes["aria-valuemax"] = "100";
        }

        private int GetIntValue(object value)
        {
            if (value == null || value == DBNull.Value || string.IsNullOrWhiteSpace(value.ToString())) return 0;
            int result = 0;
            Int32.TryParse(value.ToString(), out result);
            return result;
        }

        private int GetLabelValue(string value)
        {
            int result = 0;
            Int32.TryParse(value, out result);
            return result;
        }

        private void SetDashboardZero()
        {
            lblActiveTraining.Text="0"; lblCompletedTraining.Text="0"; lblFutureTraining.Text="0"; lblPendingTests.Text="0"; lblFeedbackPending.Text="0"; lblCertificate.Text="0";
            lblStatusTraining.Text="0"; lblStatusTests.Text="0"; lblStatusPendingTests.Text="0"; lblStatusFeedback.Text="0"; lblStatusCertificate.Text="0";
        }

        private void OpenDashboardDetails(string type)
        {
            Session["TraineeDashboardType"] = type;
            Response.Redirect("~/Trainee/DashboardDetails.aspx");
        }

        protected void lnkActiveTraining_Click(object sender, EventArgs e) { OpenDashboardDetails("Active"); }
        protected void lnkCompletedTraining_Click(object sender, EventArgs e) { OpenDashboardDetails("Completed"); }
        protected void lnkFutureTraining_Click(object sender, EventArgs e) { OpenDashboardDetails("Future"); }
        protected void lnkPendingTests_Click(object sender, EventArgs e) { OpenDashboardDetails("PendingTests"); }
        protected void lnkFeedbackPending_Click(object sender, EventArgs e) { OpenDashboardDetails("FeedbackPending"); }
        protected void lnkCertificate_Click(object sender, EventArgs e) { OpenDashboardDetails("Certificates"); }
        protected void lnkMyTraining_Click(object sender, EventArgs e) { OpenDashboardDetails("Active"); }
        protected void lnkAttendance_Click(object sender, EventArgs e) { OpenDashboardDetails("Attendance"); }
        protected void lnkBatchFeedback_Click(object sender, EventArgs e) { OpenDashboardDetails("FeedbackPending"); }
        protected void lnkPublishedTests_Click(object sender, EventArgs e) { OpenDashboardDetails("Tests"); }
    }
}
