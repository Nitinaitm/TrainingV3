using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class TrainingDetailsReport : Page
    {
        private readonly string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindStatus();
                BindTrainingList();
            }
        }

        private void BindStatus()
        {
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT ISNULL(TrainingStatus,'') TrainingStatus FROM TrainingDetails WHERE ISNULL(TrainingStatus,'')<>'' ORDER BY TrainingStatus", con))
            {
                con.Open();
                ddlStatus.DataSource = cmd.ExecuteReader();
                ddlStatus.DataTextField = "TrainingStatus";
                ddlStatus.DataValueField = "TrainingStatus";
                ddlStatus.DataBind();
                ddlStatus.Items.Insert(0, new ListItem("All Status", ""));
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            pnlDetails.Visible = false;
            pnlTrainingList.Visible = true;
            BindTrainingList();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtTrainingID.Text = "";
            txtBatch.Text = "";
            txtDateFrom.Text = "";
            txtDateTo.Text = "";
            ddlStatus.SelectedIndex = 0;
            pnlDetails.Visible = false;
            pnlTrainingList.Visible = true;
            BindTrainingList();
        }

        private void BindTrainingList()
        {
            StringBuilder q = new StringBuilder();
            q.Append("SELECT TD.TrainingID,ISNULL(CM.CourseName,'') CourseName,TD.TrainingType,TD.TrainingOrganizer,TD.TrainingLocation,TD.Batch,CONVERT(varchar(10),COALESCE(TRY_CONVERT(date,TD.DateFrom,105),TRY_CONVERT(date,TD.DateFrom,23),TRY_CONVERT(date,TD.DateFrom)),105) DateFrom,CONVERT(varchar(10),COALESCE(TRY_CONVERT(date,TD.DateTo,105),TRY_CONVERT(date,TD.DateTo,23),TRY_CONVERT(date,TD.DateTo)),105) DateTo,TD.NoOfDays,ISNULL(TD.TrainingStatus,'') TrainingStatus FROM TrainingDetails TD LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE 1=1");
            SqlCommand cmd = new SqlCommand();
            if (txtTrainingID.Text.Trim() != "") { q.Append(" AND TD.TrainingID LIKE @TrainingID"); cmd.Parameters.AddWithValue("@TrainingID","%" + txtTrainingID.Text.Trim() + "%"); }
            if (txtBatch.Text.Trim() != "") { q.Append(" AND TD.Batch LIKE @Batch"); cmd.Parameters.AddWithValue("@Batch","%" + txtBatch.Text.Trim() + "%"); }
            if (ddlStatus.SelectedValue != "") { q.Append(" AND ISNULL(TD.TrainingStatus,'')=@Status"); cmd.Parameters.AddWithValue("@Status",ddlStatus.SelectedValue); }
            if (txtDateFrom.Text.Trim() != "") { q.Append(" AND COALESCE(TRY_CONVERT(date,TD.DateFrom,105),TRY_CONVERT(date,TD.DateFrom,23),TRY_CONVERT(date,TD.DateFrom))>=TRY_CONVERT(date,@DateFrom,105)"); cmd.Parameters.AddWithValue("@DateFrom",txtDateFrom.Text.Trim()); }
            if (txtDateTo.Text.Trim() != "") { q.Append(" AND COALESCE(TRY_CONVERT(date,TD.DateTo,105),TRY_CONVERT(date,TD.DateTo,23),TRY_CONVERT(date,TD.DateTo))<=TRY_CONVERT(date,@DateTo,105)"); cmd.Parameters.AddWithValue("@DateTo",txtDateTo.Text.Trim()); }
            q.Append(" ORDER BY COALESCE(TRY_CONVERT(date,TD.DateFrom,105),TRY_CONVERT(date,TD.DateFrom,23),TRY_CONVERT(date,TD.DateFrom)),TD.TrainingID");
            cmd.CommandText = q.ToString();
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Connection = con;
                da.Fill(dt);
            }
            gvTraining.DataSource = dt;
            gvTraining.DataBind();
            lblTrainingCount.Text = dt.Rows.Count + " training(s)";
        }

        protected void gvTraining_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "ViewTraining") return;
            string trainingID = e.CommandArgument.ToString();
            ViewState["TrainingID"] = trainingID;
            LoadTrainingHeader(trainingID);
            BindSessions(trainingID);
            gvReport.DataSource = null;
            gvReport.DataBind();
            lblDetailMessage.Text = "";
            pnlTrainingList.Visible = false;
            pnlDetails.Visible = true;
        }

        private void LoadTrainingHeader(string trainingID)
        {
            DataTable dt = GetTable("SELECT TD.TrainingID,ISNULL(CM.CourseName,'') CourseName,TD.TrainingType,TD.TrainingOrganizer,TD.TrainingLocation,TD.Batch,CONVERT(varchar(10),COALESCE(TRY_CONVERT(date,TD.DateFrom,105),TRY_CONVERT(date,TD.DateFrom,23),TRY_CONVERT(date,TD.DateFrom)),105) DateFrom,CONVERT(varchar(10),COALESCE(TRY_CONVERT(date,TD.DateTo,105),TRY_CONVERT(date,TD.DateTo,23),TRY_CONVERT(date,TD.DateTo)),105) DateTo,ISNULL(TD.NoOfDays,0) NoOfDays,ISNULL(TD.TrainingStatus,'') TrainingStatus FROM TrainingDetails TD LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE TD.TrainingID=@TrainingID", new SqlParameter("@TrainingID",trainingID));
            if (dt.Rows.Count == 0) return;
            DataRow r = dt.Rows[0];
            lblDetailTrainingID.Text = r["TrainingID"].ToString();
            lblCourse.Text = r["CourseName"].ToString();
            lblBatch.Text = r["Batch"].ToString();
            lblStatus.Text = r["TrainingStatus"].ToString();
            lblType.Text = r["TrainingType"].ToString();
            lblOrganizer.Text = r["TrainingOrganizer"].ToString();
            lblLocation.Text = r["TrainingLocation"].ToString();
            lblDateFrom.Text = r["DateFrom"].ToString();
            lblDateTo.Text = r["DateTo"].ToString();
            lblDuration.Text = r["NoOfDays"].ToString() + " Day(s)";
        }

        private void BindSessions(string trainingID)
        {
            string q = "SELECT SM.SessionID,SM.SessionNo,SM.SessionName,ISNULL(TP.TopicName,'') TopicName,CONVERT(varchar(10),COALESCE(TRY_CONVERT(date,SM.SessionDate,105),TRY_CONVERT(date,SM.SessionDate,23),TRY_CONVERT(date,SM.SessionDate)),105) SessionDate,SM.StartTime,SM.EndTime,CASE WHEN ISNULL(TM.TrainerType,'')='Internal' THEN ISNULL(E.EmpName,'') ELSE ISNULL(TM.NameExternal,'') END TrainerName,SM.SessionStatus FROM SessionMaster SM LEFT JOIN TopicMaster TP ON SM.TopicID=TP.TopicID LEFT JOIN TrainerMaster TM ON SM.TrainerID=TM.TrainerID LEFT JOIN EmpBasicMaster E ON TM.EmpID=E.EmpID WHERE SM.TrainingID=@TrainingID AND ISNULL(SM.SessionCancelled,0)=0 ORDER BY TRY_CONVERT(date,SM.SessionDate,105),TRY_CONVERT(int,SM.SessionNo)";
            gvSessions.DataSource = GetTable(q,new SqlParameter("@TrainingID",trainingID));
            gvSessions.DataBind();
        }

        protected void gvSessions_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (ViewState["TrainingID"] == null) return;
            string sessionID = e.CommandArgument.ToString();
            if (e.CommandName == "Attendance") ShowAttendance(sessionID);
            else if (e.CommandName == "PreTest") ShowTest(sessionID,"PRE");
            else if (e.CommandName == "PostTest") ShowTest(sessionID,"POST");
        }

        private void ShowAttendance(string sessionID)
        {
            string q = "SELECT E.EmpID,E.EmpName,E.EmpDesignation,E.EmpCompany,E.EmpPostingPlace,ISNULL(SA.AttendanceStatus,'Pending') AttendanceStatus,ISNULL(SA.Remarks,'') Remarks,SA.CreatedBy AttendanceMarkedBy,SA.CreatedOn AttendanceMarkedOn FROM TrainingAssignment TA INNER JOIN EmpBasicMaster E ON TA.EmpID=E.EmpID LEFT JOIN SessionAttendance SA ON SA.EmpID=TA.EmpID AND SA.TrainingID=TA.TrainingID AND SA.SessionID=@SessionID WHERE TA.TrainingID=@TrainingID AND ISNULL(TA.Cancelled,0)=0 ORDER BY E.EmpName";
            BindReport(q,new SqlParameter("@SessionID",sessionID),new SqlParameter("@TrainingID",ViewState["TrainingID"].ToString()));
            lblDetailMessage.Text = "Session Attendance: trainee-wise Present / Absent / Pending with attendance marked by.";
        }

        private void ShowTest(string sessionID,string testType)
        {
            string q = "SELECT E.EmpID,E.EmpName,E.EmpDesignation,TM.TestID,TM.TestTitle,TM.TestStatus,ISNULL(R.TotalQuestions,ISNULL(A.TotalQuestions,ISNULL(TM.TotalQuestions,0))) TotalQuestions,ISNULL(R.AttemptedQuestions,CASE WHEN A.AttemptID IS NULL THEN 0 ELSE ISNULL(A.TotalQuestions,0) END) AttemptedQuestions,ISNULL(R.CorrectAnswers,ISNULL(A.CorrectAnswers,0)) CorrectAnswers,ISNULL(R.WrongAnswers,ISNULL(A.WrongAnswers,0)) WrongAnswers,ISNULL(NM.NegativeMarks,0) NegativeMarks,ISNULL(R.TotalMarks,ISNULL(A.TotalMarks,ISNULL(TM.TotalMarks,0))) TotalMarks,ISNULL(R.ObtainedMarks,ISNULL(A.ObtainedMarks,0)) ObtainedMarks,ISNULL(R.Percentage,ISNULL(A.Percentage,0)) Percentage,CASE WHEN A.AttemptID IS NULL THEN 'No' ELSE 'Yes' END Attempted,CASE WHEN ISNULL(A.Submitted,0)=1 THEN 'Yes' ELSE 'No' END Submitted,A.AttemptNo,ISNULL(R.ResultStatus,ISNULL(A.Result,'')) ResultStatus FROM TrainingAssignment TA INNER JOIN EmpBasicMaster E ON TA.EmpID=E.EmpID INNER JOIN TestMaster TM ON TM.SessionID=@SessionID AND TM.TestType=@TestType OUTER APPLY (SELECT TOP 1 TA2.AttemptID,TA2.AttemptNo,TA2.Submitted,TA2.TotalQuestions,TA2.CorrectAnswers,TA2.WrongAnswers,TA2.ObtainedMarks,TA2.TotalMarks,TA2.Percentage,TA2.Result FROM TestAttempt TA2 WHERE TA2.TestID=TM.TestID AND TA2.EmpID=E.EmpID ORDER BY TA2.AttemptNo DESC,TA2.ID DESC) A OUTER APPLY (SELECT TOP 1 R2.ResultStatus,R2.Percentage,R2.TotalQuestions,R2.AttemptedQuestions,R2.CorrectAnswers,R2.WrongAnswers,R2.TotalMarks,R2.ObtainedMarks FROM TestResult R2 WHERE R2.TestID=TM.TestID AND R2.EmpID=E.EmpID ORDER BY CASE WHEN R2.AttemptID=A.AttemptID THEN 0 WHEN R2.AttemptNo=A.AttemptNo THEN 1 ELSE 2 END,R2.AttemptNo DESC,R2.ID DESC) R OUTER APPLY (SELECT SUM(CASE WHEN ISNULL(TAA.IsCorrect,0)=0 AND ISNULL(TAA.SelectedOption,'')<>'' THEN ISNULL(TQ.NegativeMarks,0) ELSE 0 END) NegativeMarks FROM TestAttemptAnswer TAA LEFT JOIN TestQuestion TQ ON TQ.TestID=TAA.TestID AND TQ.QuestionID=TAA.QuestionID WHERE TAA.AttemptID=A.AttemptID AND TAA.TestID=TM.TestID AND TAA.EmpID=E.EmpID) NM WHERE TA.TrainingID=@TrainingID AND ISNULL(TA.Cancelled,0)=0 ORDER BY E.EmpName";
            BindReport(q,new SqlParameter("@SessionID",sessionID),new SqlParameter("@TestType",testType),new SqlParameter("@TrainingID",ViewState["TrainingID"].ToString()));
            lblDetailMessage.Text = testType == "PRE" ? "Pre Test trainee-wise attempt, submission and result." : "Post Test trainee-wise attempt, submission and result.";
        }

        protected void btnTrainees_Click(object sender, EventArgs e)
        {
            string q = "SELECT E.EmpID,E.EmpName,E.EmpDesignation,E.EmpCompany,E.EmpPostingPlace,E.MobileNo,E.EmailId,TA.AssignmentStatus,TA.AssignedBy,TA.AssignedOn FROM TrainingAssignment TA INNER JOIN EmpBasicMaster E ON TA.EmpID=E.EmpID WHERE TA.TrainingID=@TrainingID AND ISNULL(TA.Cancelled,0)=0 ORDER BY E.EmpName";
            BindReport(q,new SqlParameter("@TrainingID",ViewState["TrainingID"].ToString()));
            lblDetailMessage.Text = "Assigned trainee list.";
        }

        protected void btnTrainers_Click(object sender, EventArgs e)
        {
            string q = "WITH AssignedTrainers AS (SELECT TrainerID FROM TrainingTrainerMapping WHERE TrainingID=@TrainingID UNION SELECT TrainerID FROM SessionMaster WHERE TrainingID=@TrainingID AND ISNULL(TrainerID,'')<>'' ) SELECT AT.TrainerID,ISNULL(TM.TrainerType,'') TrainerType,CASE WHEN ISNULL(TM.TrainerType,'')='Internal' THEN ISNULL(E.EmpName,'') ELSE ISNULL(TM.NameExternal,'') END TrainerName,CASE WHEN ISNULL(TM.TrainerType,'')='Internal' THEN ISNULL(E.EmpDesignation,'') ELSE ISNULL(TM.DesignationExternal,'') END Designation,CASE WHEN ISNULL(TM.TrainerType,'')='Internal' THEN ISNULL(E.MobileNo,'') ELSE ISNULL(TM.MobileNo,'') END MobileNo,CASE WHEN ISNULL(TM.TrainerType,'')='Internal' THEN ISNULL(E.EmailId,'') ELSE ISNULL(TM.EmailID,'') END EmailID FROM AssignedTrainers AT INNER JOIN TrainerMaster TM ON AT.TrainerID=TM.TrainerID LEFT JOIN EmpBasicMaster E ON TM.EmpID=E.EmpID ORDER BY TrainerName";
            BindReport(q,new SqlParameter("@TrainingID",ViewState["TrainingID"].ToString()));
            lblDetailMessage.Text = "Assigned trainer list.";
        }

        protected void btnFeedback_Click(object sender, EventArgs e)
        {
            string q = "SELECT E.EmpID,E.EmpName,E.EmpDesignation,ISNULL(F.Submitted,0) FeedbackSubmitted,F.SubmittedOn,ISNULL(FD.DetailCount,0) FeedbackDetailCount,ISNULL(FD.AverageRating,0) AverageRating,ISNULL(BF.Submitted,0) BatchFeedbackSubmitted,BF.SubmittedOn BatchFeedbackSubmittedOn FROM TrainingAssignment TA INNER JOIN EmpBasicMaster E ON TA.EmpID=E.EmpID LEFT JOIN Feedback F ON F.TrainingID=TA.TrainingID AND F.EmpID=TA.EmpID LEFT JOIN (SELECT TrainingID,EmpID,COUNT(*) DetailCount,AVG(CAST(Rating AS decimal(10,2))) AverageRating FROM FeedbackDetail GROUP BY TrainingID,EmpID) FD ON FD.TrainingID=TA.TrainingID AND FD.EmpID=TA.EmpID LEFT JOIN BatchFeedback BF ON BF.TrainingID=TA.TrainingID AND BF.EmpID=TA.EmpID WHERE TA.TrainingID=@TrainingID AND ISNULL(TA.Cancelled,0)=0 ORDER BY E.EmpName";
            BindReport(q,new SqlParameter("@TrainingID",ViewState["TrainingID"].ToString()));
            lblDetailMessage.Text = "Trainee-wise session/topic feedback and batch feedback status.";
        }

        protected void btnCertificates_Click(object sender, EventArgs e)
        {
            string q = "SELECT TC.CertificateID,TC.CertificateNo,E.EmpID,E.EmpName,E.EmpDesignation,TC.GeneratedOn,TC.CertificateStatus,TC.PDFName,TC.DownloadCount,TC.LastDownloadedOn,TC.LastDownloadedBy FROM TrainingCertificate TC INNER JOIN EmpBasicMaster E ON TC.EmpID=E.EmpID WHERE TC.TrainingID=@TrainingID ORDER BY TC.GeneratedOn DESC";
            BindReport(q,new SqlParameter("@TrainingID",ViewState["TrainingID"].ToString()));
            lblDetailMessage.Text = "Generated certificates for this training.";
        }

        protected void btnHostel_Click(object sender, EventArgs e)
        {
            string q = "SELECT HA.AllotmentID,E.EmpID,E.EmpName,HM.HostelName,HBM.BedNo,HRM.RoomNo,HBl.BlockName,HA.AllotmentDate,HA.VacateDate,HA.Status FROM HostelAllotment HA INNER JOIN EmpBasicMaster E ON HA.EmpID=E.EmpID INNER JOIN HostelBedMaster HBM ON HA.BedID=HBM.ID INNER JOIN HostelRoomMaster HRM ON HBM.RoomID=HRM.ID INNER JOIN HostelBlockMaster HBl ON HRM.BlockID=HBl.ID INNER JOIN HostelMaster HM ON HBl.HostelID=HM.ID WHERE HA.TrainingID=@TrainingID ORDER BY E.EmpName";
            BindReport(q,new SqlParameter("@TrainingID",ViewState["TrainingID"].ToString()));
            lblDetailMessage.Text = "Trainee-wise hostel allotment details.";
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            pnlDetails.Visible = false;
            pnlTrainingList.Visible = true;
        }

        private void BindReport(string query, params SqlParameter[] parameters)
        {
            ViewState["ReportQuery"] = query;
            ViewState["ReportTrainingID"] = null;
            ViewState["ReportSessionID"] = null;
            ViewState["ReportTestType"] = null;
            if (parameters != null)
            {
                foreach (SqlParameter p in parameters)
                {
                    if (p.ParameterName == "@TrainingID") ViewState["ReportTrainingID"] = p.Value.ToString();
                    if (p.ParameterName == "@SessionID") ViewState["ReportSessionID"] = p.Value.ToString();
                    if (p.ParameterName == "@TestType") ViewState["ReportTestType"] = p.Value.ToString();
                }
            }
            DataTable dt = GetTable(query,parameters);
            gvReport.DataSource = dt;
            gvReport.DataBind();
        }

        private DataTable GetTable(string query, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand(query,con))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd)) da.Fill(dt);
            }
            return dt;
        }

        protected void btnExportReport_Click(object sender, EventArgs e)
        {
            string query = ViewState["ReportQuery"] == null ? "" : ViewState["ReportQuery"].ToString();
            if (query == "") return;
            DataTable dt = GetTable(query,GetReportParameters());
            ExportExcel(dt,"TrainingDetailReport.xls");
        }

        private SqlParameter[] GetReportParameters()
        {
            System.Collections.Generic.List<SqlParameter> list = new System.Collections.Generic.List<SqlParameter>();
            if (ViewState["ReportTrainingID"] != null) list.Add(new SqlParameter("@TrainingID",ViewState["ReportTrainingID"].ToString()));
            if (ViewState["ReportSessionID"] != null) list.Add(new SqlParameter("@SessionID",ViewState["ReportSessionID"].ToString()));
            if (ViewState["ReportTestType"] != null) list.Add(new SqlParameter("@TestType",ViewState["ReportTestType"].ToString()));
            return list.ToArray();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            DataTable dt = GetTrainingList();
            ExportExcel(dt,"TrainingReport.xls");
        }

        private DataTable GetTrainingList()
        {
            StringBuilder q = new StringBuilder();
            q.Append("SELECT TD.TrainingID,ISNULL(CM.CourseName,'') CourseName,TD.TrainingType,TD.TrainingOrganizer,TD.TrainingLocation,TD.Batch,CONVERT(varchar(10),COALESCE(TRY_CONVERT(date,TD.DateFrom,105),TRY_CONVERT(date,TD.DateFrom,23),TRY_CONVERT(date,TD.DateFrom)),105) DateFrom,CONVERT(varchar(10),COALESCE(TRY_CONVERT(date,TD.DateTo,105),TRY_CONVERT(date,TD.DateTo,23),TRY_CONVERT(date,TD.DateTo)),105) DateTo,TD.NoOfDays,ISNULL(TD.TrainingStatus,'') TrainingStatus FROM TrainingDetails TD LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE 1=1");
            SqlCommand cmd = new SqlCommand();
            if (txtTrainingID.Text.Trim() != "") { q.Append(" AND TD.TrainingID LIKE @TrainingID"); cmd.Parameters.AddWithValue("@TrainingID","%" + txtTrainingID.Text.Trim() + "%"); }
            if (txtBatch.Text.Trim() != "") { q.Append(" AND TD.Batch LIKE @Batch"); cmd.Parameters.AddWithValue("@Batch","%" + txtBatch.Text.Trim() + "%"); }
            if (ddlStatus.SelectedValue != "") { q.Append(" AND ISNULL(TD.TrainingStatus,'')=@Status"); cmd.Parameters.AddWithValue("@Status",ddlStatus.SelectedValue); }
            if (txtDateFrom.Text.Trim() != "") { q.Append(" AND COALESCE(TRY_CONVERT(date,TD.DateFrom,105),TRY_CONVERT(date,TD.DateFrom,23),TRY_CONVERT(date,TD.DateFrom))>=TRY_CONVERT(date,@DateFrom,105)"); cmd.Parameters.AddWithValue("@DateFrom",txtDateFrom.Text.Trim()); }
            if (txtDateTo.Text.Trim() != "") { q.Append(" AND COALESCE(TRY_CONVERT(date,TD.DateTo,105),TRY_CONVERT(date,TD.DateTo,23),TRY_CONVERT(date,TD.DateTo))<=TRY_CONVERT(date,@DateTo,105)"); cmd.Parameters.AddWithValue("@DateTo",txtDateTo.Text.Trim()); }
            q.Append(" ORDER BY COALESCE(TRY_CONVERT(date,TD.DateFrom,105),TRY_CONVERT(date,TD.DateFrom,23),TRY_CONVERT(date,TD.DateFrom)),TD.TrainingID");
            cmd.CommandText = q.ToString();
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { cmd.Connection = con; DataTable dt = new DataTable(); da.Fill(dt); return dt; }
        }

        private void ExportExcel(DataTable dt,string fileName)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition","attachment;filename=" + fileName);
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            StringBuilder sb = new StringBuilder();
            foreach (DataColumn c in dt.Columns) sb.Append(c.ColumnName + "\t");
            sb.Append("\r\n");
            foreach (DataRow r in dt.Rows) { foreach (DataColumn c in dt.Columns) sb.Append(r[c].ToString().Replace("\t"," ") + "\t"); sb.Append("\r\n"); }
            Response.Write(sb.ToString());
            Response.End();
        }

        public override void VerifyRenderingInServerForm(Control control) { }
    }
}