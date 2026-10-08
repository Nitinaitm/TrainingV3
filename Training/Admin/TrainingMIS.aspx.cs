using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class TrainingMIS : Page
    {
        private readonly string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender,EventArgs e)
        {
            if(!IsPostBack){BindStatus();BindMIS();}
        }

        private void BindStatus()
        {
            using(SqlConnection con=new SqlConnection(constr))
            using(SqlCommand cmd=new SqlCommand("SELECT DISTINCT ISNULL(TrainingStatus,'') TrainingStatus FROM TrainingDetails WHERE ISNULL(TrainingStatus,'')<>'' ORDER BY TrainingStatus",con))
            {
                con.Open();
                ddlStatus.DataSource=cmd.ExecuteReader();
                ddlStatus.DataTextField="TrainingStatus";
                ddlStatus.DataValueField="TrainingStatus";
                ddlStatus.DataBind();
                ddlStatus.Items.Insert(0,new ListItem("All Status",""));
            }
        }

        protected void btnSearch_Click(object sender,EventArgs e){BindMIS();}
        protected void btnReset_Click(object sender,EventArgs e){txtFromDate.Text="";txtToDate.Text="";ddlStatus.SelectedIndex=0;BindMIS();}

        private string DateExpr(string field)
        {
            return "COALESCE(TRY_CONVERT(date,"+field+",105),TRY_CONVERT(date,"+field+",23),TRY_CONVERT(date,"+field+"))";
        }

        private string Filter(string alias)
        {
            StringBuilder s=new StringBuilder();
            if(txtFromDate.Text.Trim()!="")s.Append(" AND "+DateExpr(alias+".DateFrom")+">=TRY_CONVERT(date,@FromDate,105)");
            if(txtToDate.Text.Trim()!="")s.Append(" AND "+DateExpr(alias+".DateTo")+"<=TRY_CONVERT(date,@ToDate,105)");
            if(ddlStatus.SelectedValue!="")s.Append(" AND ISNULL("+alias+".TrainingStatus,'')=@Status");
            return s.ToString();
        }

        private void AddCommon(SqlCommand cmd)
        {
            if(txtFromDate.Text.Trim()!="")cmd.Parameters.AddWithValue("@FromDate",txtFromDate.Text.Trim());
            if(txtToDate.Text.Trim()!="")cmd.Parameters.AddWithValue("@ToDate",txtToDate.Text.Trim());
            if(ddlStatus.SelectedValue!="")cmd.Parameters.AddWithValue("@Status",ddlStatus.SelectedValue);
        }

        private DataTable GetTable(string q,params SqlParameter[] p)
        {
            DataTable dt=new DataTable();
            using(SqlConnection con=new SqlConnection(constr))
            using(SqlCommand cmd=new SqlCommand(q,con))
            {
                if(p!=null)cmd.Parameters.AddRange(p);
                using(SqlDataAdapter da=new SqlDataAdapter(cmd))da.Fill(dt);
            }
            return dt;
        }

        private int Scalar(string q)
        {
            using(SqlConnection con=new SqlConnection(constr))
            using(SqlCommand cmd=new SqlCommand(q,con))
            {
                AddCommon(cmd);
                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private decimal ScalarDecimal(string q)
        {
            using(SqlConnection con=new SqlConnection(constr))
            using(SqlCommand cmd=new SqlCommand(q,con))
            {
                AddCommon(cmd);
                con.Open();
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        private void BindMIS()
        {
            string f=Filter("TD");
            lblTotalTrainings.Text=Scalar("SELECT COUNT(*) FROM TrainingDetails TD WHERE 1=1"+f).ToString();
            lblCompletedTrainings.Text=Scalar("SELECT COUNT(*) FROM TrainingDetails TD WHERE ISNULL(TD.TrainingStatus,'') IN ('Completed','TrainingCompleted')"+f).ToString();
            lblOngoingTrainings.Text=Scalar("SELECT COUNT(*) FROM TrainingDetails TD WHERE ISNULL(TD.TrainingStatus,'') NOT IN ('Completed','TrainingCompleted') AND "+DateExpr("TD.DateFrom")+"<=CAST(GETDATE() AS date) AND "+DateExpr("TD.DateTo")+">=CAST(GETDATE() AS date)"+f).ToString();
            lblFutureTrainings.Text=Scalar("SELECT COUNT(*) FROM TrainingDetails TD WHERE ISNULL(TD.TrainingStatus,'') NOT IN ('Completed','TrainingCompleted') AND "+DateExpr("TD.DateFrom")+">CAST(GETDATE() AS date)"+f).ToString();
            lblTotalTrainees.Text=Scalar("SELECT COUNT(DISTINCT TA.EmpID) FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TA.TrainingID=TD.TrainingID WHERE ISNULL(TA.Cancelled,0)=0"+f).ToString();
            lblTotalSessions.Text=Scalar("SELECT COUNT(*) FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID WHERE ISNULL(SM.SessionCancelled,0)=0"+f).ToString();
            decimal attendance=ScalarDecimal("SELECT ISNULL(100.0*SUM(X.IsPresent)/NULLIF(COUNT(*),0),0) FROM (SELECT CASE WHEN EXISTS (SELECT 1 FROM SessionAttendance SA WHERE SA.TrainingID=TD.TrainingID AND SA.SessionID=SM.SessionID AND SA.EmpID=TA.EmpID AND SA.AttendanceStatus='Present') THEN 1 ELSE 0 END IsPresent FROM TrainingDetails TD INNER JOIN SessionMaster SM ON SM.TrainingID=TD.TrainingID INNER JOIN TrainingAssignment TA ON TA.TrainingID=TD.TrainingID AND ISNULL(TA.Cancelled,0)=0 WHERE ISNULL(SM.SessionCancelled,0)=0 AND ISNULL(SM.AttendanceSkipped,0)=0 AND ISNULL(TD.AttendanceRequired,0)=1"+f+") X");
            lblAttendance.Text=attendance.ToString("0.00")+"%";
            lblCertificates.Text=Scalar("SELECT COUNT(*) FROM TrainingCertificate TC INNER JOIN TrainingDetails TD ON TC.TrainingID=TD.TrainingID WHERE 1=1"+f).ToString();
            lblFeedback.Text=Scalar("SELECT COUNT(*) FROM Feedback F INNER JOIN TrainingDetails TD ON F.TrainingID=TD.TrainingID WHERE ISNULL(F.Submitted,0)=1"+f).ToString();
            decimal post=ScalarDecimal("SELECT ISNULL(AVG(R.Percentage),0) FROM TestResult R INNER JOIN TestMaster TM ON R.TestID=TM.TestID INNER JOIN SessionMaster SM ON TM.SessionID=SM.SessionID INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID WHERE TM.TestType='POST'"+f);
            decimal scheduledManHours=ScalarDecimal("SELECT ISNULL(SUM((DATEDIFF(MINUTE,TRY_CONVERT(time,SM.StartTime),TRY_CONVERT(time,SM.EndTime))/60.0)*ISNULL(TC.TraineeCount,0)),0) FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID LEFT JOIN (SELECT TA.TrainingID,COUNT(DISTINCT TA.EmpID) TraineeCount FROM TrainingAssignment TA WHERE ISNULL(TA.Cancelled,0)=0 GROUP BY TA.TrainingID) TC ON TC.TrainingID=SM.TrainingID WHERE ISNULL(SM.SessionCancelled,0)=0 AND TRY_CONVERT(time,SM.StartTime) IS NOT NULL AND TRY_CONVERT(time,SM.EndTime) IS NOT NULL"+f);
            decimal completedManHours=ScalarDecimal("SELECT ISNULL(SUM((DATEDIFF(MINUTE,TRY_CONVERT(time,SM.StartTime),TRY_CONVERT(time,SM.EndTime))/60.0)*ISNULL(TC.TraineeCount,0)),0) FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID LEFT JOIN (SELECT TA.TrainingID,COUNT(DISTINCT TA.EmpID) TraineeCount FROM TrainingAssignment TA WHERE ISNULL(TA.Cancelled,0)=0 GROUP BY TA.TrainingID) TC ON TC.TrainingID=SM.TrainingID WHERE ISNULL(SM.SessionCancelled,0)=0 AND ISNULL(TD.TrainingStatus,'') IN ('Completed','TrainingCompleted') AND TRY_CONVERT(time,SM.StartTime) IS NOT NULL AND TRY_CONVERT(time,SM.EndTime) IS NOT NULL"+f);
            lblScheduledManHours.Text=scheduledManHours.ToString("0.00");
            lblCompletedManHours.Text=completedManHours.ToString("0.00");
            lblTotalManHours.Text=scheduledManHours.ToString("0.00");
            lblPostTest.Text=post.ToString("0.00")+"%";
            string statusQ="SELECT ISNULL(TD.TrainingStatus,'') TrainingStatus,COUNT(*) TrainingCount FROM TrainingDetails TD WHERE 1=1"+f+" GROUP BY ISNULL(TD.TrainingStatus,'') ORDER BY TrainingStatus";
            string typeQ="SELECT ISNULL(TD.TrainingType,'') TrainingType,COUNT(*) TrainingCount FROM TrainingDetails TD WHERE 1=1"+f+" GROUP BY ISNULL(TD.TrainingType,'') ORDER BY TrainingType";
            string courseQ="WITH TraineeCounts AS (SELECT TA.TrainingID,COUNT(DISTINCT TA.EmpID) TraineeCount FROM TrainingAssignment TA WHERE ISNULL(TA.Cancelled,0)=0 GROUP BY TA.TrainingID), SessionHours AS (SELECT SM.TrainingID,SUM((DATEDIFF(MINUTE,TRY_CONVERT(time,SM.StartTime),TRY_CONVERT(time,SM.EndTime))/60.0)*ISNULL(TC.TraineeCount,0)) ScheduledManHours,SUM(CASE WHEN ISNULL(TD2.TrainingStatus,'') IN ('Completed','TrainingCompleted') THEN (DATEDIFF(MINUTE,TRY_CONVERT(time,SM.StartTime),TRY_CONVERT(time,SM.EndTime))/60.0)*ISNULL(TC.TraineeCount,0) ELSE 0 END) CompletedManHours FROM SessionMaster SM INNER JOIN TrainingDetails TD2 ON SM.TrainingID=TD2.TrainingID LEFT JOIN TraineeCounts TC ON TC.TrainingID=SM.TrainingID WHERE ISNULL(SM.SessionCancelled,0)=0 AND TRY_CONVERT(time,SM.StartTime) IS NOT NULL AND TRY_CONVERT(time,SM.EndTime) IS NOT NULL GROUP BY SM.TrainingID) SELECT ISNULL(CM.CourseName,'') CourseName,COUNT(*) TrainingCount,ISNULL(SUM(TRY_CONVERT(decimal(18,2),NULLIF(LTRIM(RTRIM(TD.NoOfDays)),''))),0) TotalTrainingDays,ISNULL(SUM(CASE WHEN LTRIM(RTRIM(ISNULL(TD.TrainingStatus,''))) IN ('Completed','TrainingCompleted') THEN COALESCE(TRY_CONVERT(decimal(18,2),NULLIF(LTRIM(RTRIM(TD.NoOfDays)),'')),CONVERT(decimal(18,2),DATEDIFF(day,COALESCE(TRY_CONVERT(date,TD.DateFrom,105),TRY_CONVERT(date,TD.DateFrom,23),TRY_CONVERT(date,TD.DateFrom)),COALESCE(TRY_CONVERT(date,TD.DateTo,105),TRY_CONVERT(date,TD.DateTo,23),TRY_CONVERT(date,TD.DateTo)))+1)) ELSE 0 END),0) CompletedTrainingDays,ISNULL(SUM(ISNULL(SH.ScheduledManHours,0)),0) ScheduledManHours,ISNULL(SUM(ISNULL(SH.CompletedManHours,0)),0) CompletedManHours FROM TrainingDetails TD LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID LEFT JOIN SessionHours SH ON SH.TrainingID=TD.TrainingID WHERE 1=1"+f+" GROUP BY ISNULL(CM.CourseName,'') ORDER BY TrainingCount DESC,CourseName";
            gvStatus.DataSource=GetFiltered(statusQ);gvStatus.DataBind();
            gvType.DataSource=GetFiltered(typeQ);gvType.DataBind();
            gvCourse.DataSource=GetFiltered(courseQ);gvCourse.DataBind();
        }

        protected void kpiCard_Click(object sender,EventArgs e)
        {
            LinkButton card=(LinkButton)sender;
            BindKpiDetails(card.CommandArgument);
        }

        private void BindKpiDetails(string type)
        {
            string q="";
            string title="";
            if(type=="TotalTrainings")
            {
                q="SELECT TD.TrainingID,ISNULL(CM.CourseName,'') CourseName,ISNULL(TD.TrainingType,'') TrainingType,ISNULL(TD.TrainingOrganizer,'') TrainingOrganizer,ISNULL(TD.Batch,'') Batch,ISNULL(TD.TrainingLocation,'') TrainingLocation,TD.NoOfDays,CONVERT(varchar(10),"+DateExpr("TD.DateFrom")+",105) DateFrom,CONVERT(varchar(10),"+DateExpr("TD.DateTo")+",105) DateTo,ISNULL(TD.TrainingStatus,'') TrainingStatus FROM TrainingDetails TD LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE 1=1"+Filter("TD")+" ORDER BY "+DateExpr("TD.DateFrom")+" DESC";
                title="Total Trainings - "+lblTotalTrainings.Text+" row(s)";
            }
            else if(type=="CompletedTrainings")
            {
                q="SELECT TD.TrainingID,ISNULL(CM.CourseName,'') CourseName,ISNULL(TD.TrainingType,'') TrainingType,ISNULL(TD.Batch,'') Batch,CONVERT(varchar(10),"+DateExpr("TD.DateFrom")+",105) DateFrom,CONVERT(varchar(10),"+DateExpr("TD.DateTo")+",105) DateTo,ISNULL(TD.TrainingStatus,'') TrainingStatus FROM TrainingDetails TD LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE ISNULL(TD.TrainingStatus,'') IN ('Completed','TrainingCompleted')"+Filter("TD")+" ORDER BY "+DateExpr("TD.DateFrom")+" DESC";
                title="Completed Trainings - "+lblCompletedTrainings.Text+" row(s)";
            }
            else if(type=="OngoingTrainings")
            {
                q="SELECT TD.TrainingID,ISNULL(CM.CourseName,'') CourseName,ISNULL(TD.TrainingType,'') TrainingType,ISNULL(TD.Batch,'') Batch,CONVERT(varchar(10),"+DateExpr("TD.DateFrom")+",105) DateFrom,CONVERT(varchar(10),"+DateExpr("TD.DateTo")+",105) DateTo,ISNULL(TD.TrainingStatus,'') TrainingStatus FROM TrainingDetails TD LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE ISNULL(TD.TrainingStatus,'') NOT IN ('Completed','TrainingCompleted') AND "+DateExpr("TD.DateFrom")+"<=CAST(GETDATE() AS date) AND "+DateExpr("TD.DateTo")+">=CAST(GETDATE() AS date)"+Filter("TD")+" ORDER BY "+DateExpr("TD.DateFrom")+" DESC";
                title="Ongoing Trainings - "+lblOngoingTrainings.Text+" row(s)";
            }
            else if(type=="FutureTrainings")
            {
                q="SELECT TD.TrainingID,ISNULL(CM.CourseName,'') CourseName,ISNULL(TD.TrainingType,'') TrainingType,ISNULL(TD.Batch,'') Batch,CONVERT(varchar(10),"+DateExpr("TD.DateFrom")+",105) DateFrom,CONVERT(varchar(10),"+DateExpr("TD.DateTo")+",105) DateTo,ISNULL(TD.TrainingStatus,'') TrainingStatus FROM TrainingDetails TD LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE ISNULL(TD.TrainingStatus,'') NOT IN ('Completed','TrainingCompleted') AND "+DateExpr("TD.DateFrom")+">CAST(GETDATE() AS date)"+Filter("TD")+" ORDER BY "+DateExpr("TD.DateFrom");
                title="Future Trainings - "+lblFutureTrainings.Text+" row(s)";
            }
            else if(type=="TotalTrainees")
            {
                q="SELECT TA.EmpID,ISNULL(MAX(E.EmpName),'') EmpName,ISNULL(MAX(E.EmpDesignation),'') EmpDesignation,ISNULL(MAX(E.EmpCompany),'') EmpCompany,ISNULL(MAX(E.EmpPostingPlace),'') EmpPostingPlace,COUNT(DISTINCT TA.TrainingID) TrainingCount FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TA.TrainingID=TD.TrainingID LEFT JOIN EmpBasicMaster E ON TA.EmpID=E.EmpID WHERE ISNULL(TA.Cancelled,0)=0"+Filter("TD")+" GROUP BY TA.EmpID ORDER BY TA.EmpID";
                title="Total Trainees - "+lblTotalTrainees.Text+" unique trainee(s)";
            }
            else if(type=="TotalSessions")
            {
                q="SELECT SM.SessionID,SM.TrainingID,SM.SessionNo,SM.SessionName,ISNULL(TP.TopicName,'') TopicName,SM.SessionDate,SM.StartTime,SM.EndTime,ISNULL(SM.SessionStatus,'') SessionStatus,ISNULL(CM.CourseName,'') CourseName FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID LEFT JOIN TopicMaster TP ON SM.TopicID=TP.TopicID LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE ISNULL(SM.SessionCancelled,0)=0"+Filter("TD")+" ORDER BY "+DateExpr("TD.DateFrom")+","+DateExpr("SM.SessionDate");
                title="Total Sessions - "+lblTotalSessions.Text+" row(s)";
            }
            else if(type=="Attendance")
            {
                q="SELECT SA.ID,SA.TrainingID,SA.SessionID,SA.EmpID,E.EmpName,SA.AttendanceStatus,ISNULL(SA.Remarks,'') Remarks,SA.CreatedBy AttendanceMarkedBy,SA.CreatedOn AttendanceMarkedOn FROM SessionAttendance SA INNER JOIN TrainingDetails TD ON SA.TrainingID=TD.TrainingID LEFT JOIN EmpBasicMaster E ON SA.EmpID=E.EmpID WHERE 1=1"+Filter("TD")+" ORDER BY SA.CreatedOn DESC";
                title="Attendance Records - "+lblAttendance.Text+"";
            }
            else if(type=="Certificates")
            {
                q="SELECT TC.CertificateID,TC.TrainingID,TC.EmpID,E.EmpName,TC.CertificateStatus,TC.GeneratedOn FROM TrainingCertificate TC INNER JOIN TrainingDetails TD ON TC.TrainingID=TD.TrainingID LEFT JOIN EmpBasicMaster E ON TC.EmpID=E.EmpID WHERE 1=1"+Filter("TD")+" ORDER BY TC.GeneratedOn DESC";
                title="Certificates Generated - "+lblCertificates.Text+" row(s)";
            }
            else if(type=="Feedback")
            {
                q="SELECT F.ID,F.TrainingID,F.EmpID,E.EmpName,F.Submitted,F.SubmittedOn FROM Feedback F INNER JOIN TrainingDetails TD ON F.TrainingID=TD.TrainingID LEFT JOIN EmpBasicMaster E ON F.EmpID=E.EmpID WHERE ISNULL(F.Submitted,0)=1"+Filter("TD")+" ORDER BY F.SubmittedOn DESC";
                title="Feedback Submitted - "+lblFeedback.Text+" row(s)";
            }
            else if(type=="ScheduledManHours" || type=="CompletedManHours")
            {
                string completedCondition=type=="CompletedManHours" ? " AND ISNULL(TD.TrainingStatus,'') IN ('Completed','TrainingCompleted')" : "";
                q="SELECT SM.SessionID,SM.TrainingID,SM.SessionNo,SM.SessionName,ISNULL(TP.TopicName,'') TopicName,SM.SessionDate,SM.StartTime,SM.EndTime,CAST(DATEDIFF(MINUTE,TRY_CONVERT(time,SM.StartTime),TRY_CONVERT(time,SM.EndTime))/60.0 AS decimal(18,2)) SessionHours,ISNULL(TC.TraineeCount,0) TraineeCount,CAST((DATEDIFF(MINUTE,TRY_CONVERT(time,SM.StartTime),TRY_CONVERT(time,SM.EndTime))/60.0)*ISNULL(TC.TraineeCount,0) AS decimal(18,2)) ManHours,ISNULL(CM.CourseName,'') CourseName FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID LEFT JOIN TopicMaster TP ON SM.TopicID=TP.TopicID LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID LEFT JOIN (SELECT TA.TrainingID,COUNT(DISTINCT TA.EmpID) TraineeCount FROM TrainingAssignment TA WHERE ISNULL(TA.Cancelled,0)=0 GROUP BY TA.TrainingID) TC ON TC.TrainingID=SM.TrainingID WHERE ISNULL(SM.SessionCancelled,0)=0 AND TRY_CONVERT(time,SM.StartTime) IS NOT NULL AND TRY_CONVERT(time,SM.EndTime) IS NOT NULL"+completedCondition+Filter("TD")+" ORDER BY "+DateExpr("TD.DateFrom")+","+DateExpr("SM.SessionDate");
                title=(type=="CompletedManHours" ? "Man Hours - Training Completed - "+lblCompletedManHours.Text : "Man Hours - Training Scheduled - "+lblScheduledManHours.Text)+" hour(s)";
            }
            else if(type=="TotalManHours")
            {
                q="SELECT SM.SessionID,SM.TrainingID,SM.SessionNo,SM.SessionName,ISNULL(TP.TopicName,'') TopicName,SM.SessionDate,SM.StartTime,SM.EndTime,CAST(DATEDIFF(MINUTE,TRY_CONVERT(time,SM.StartTime),TRY_CONVERT(time,SM.EndTime))/60.0 AS decimal(18,2)) SessionHours,ISNULL(TC.TraineeCount,0) TraineeCount,CAST((DATEDIFF(MINUTE,TRY_CONVERT(time,SM.StartTime),TRY_CONVERT(time,SM.EndTime))/60.0)*ISNULL(TC.TraineeCount,0) AS decimal(18,2)) ManHours,ISNULL(CM.CourseName,'') CourseName FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID LEFT JOIN TopicMaster TP ON SM.TopicID=TP.TopicID LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID LEFT JOIN (SELECT TA.TrainingID,COUNT(DISTINCT TA.EmpID) TraineeCount FROM TrainingAssignment TA WHERE ISNULL(TA.Cancelled,0)=0 GROUP BY TA.TrainingID) TC ON TC.TrainingID=SM.TrainingID WHERE ISNULL(SM.SessionCancelled,0)=0 AND TRY_CONVERT(time,SM.StartTime) IS NOT NULL AND TRY_CONVERT(time,SM.EndTime) IS NOT NULL"+Filter("TD")+" ORDER BY "+DateExpr("TD.DateFrom")+","+DateExpr("SM.SessionDate");
                title="Total Man Hours - "+lblTotalManHours.Text+" hour(s)";
            }
            else if(type=="PostTest")
            {
                q="SELECT R.ID,R.TestID,TM.TestTitle,R.EmpID,E.EmpName,R.AttemptNo,R.TotalQuestions,R.AttemptedQuestions,R.CorrectAnswers,R.WrongAnswers,R.TotalMarks,R.ObtainedMarks,R.Percentage,R.ResultStatus FROM TestResult R INNER JOIN TestMaster TM ON R.TestID=TM.TestID INNER JOIN SessionMaster SM ON TM.SessionID=SM.SessionID INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID LEFT JOIN EmpBasicMaster E ON R.EmpID=E.EmpID WHERE TM.TestType='POST'"+Filter("TD")+" ORDER BY R.Percentage DESC";
                title="Post Test Results - Average "+lblPostTest.Text;
            }
            if(q=="")return;
            ViewState["KpiDetailQuery"]=q;
            ViewState["KpiDetailSummaryKey"]=null;
            DataTable dt=GetFiltered(q);
            gvKpiDetails.DataSource=dt;
            gvKpiDetails.DataBind();
            lblKpiDetailsTitle.Text=title+" | "+dt.Rows.Count+" row(s)";
            ScriptManager.RegisterStartupScript(this,GetType(),"showKpiDetails","showKpiDetails();",true);
        }


        protected void gvKpiDetails_RowDataBound(object sender,GridViewRowEventArgs e)
        {
            if(e.Row.RowType==DataControlRowType.DataRow)
            {
                TableCell cell=new TableCell();
                cell.Text=(e.Row.RowIndex+1).ToString();
                e.Row.Cells.AddAt(0,cell);
            }
            else if(e.Row.RowType==DataControlRowType.Header)
            {
                TableCell cell=new TableCell();
                cell.Text="S.No.";
                e.Row.Cells.AddAt(0,cell);
            }
        }

        protected void gvSummary_RowCommand(object sender,GridViewCommandEventArgs e)
        {
            if(e.CommandName=="StatusSummary" || e.CommandName=="TypeSummary" || e.CommandName=="CourseSummary")
            {
                string key=Convert.ToString(e.CommandArgument);
                string q="";
                string title="";
                if(e.CommandName=="StatusSummary")
                {
                    q="SELECT TD.TrainingID,ISNULL(CM.CourseName,'') CourseName,ISNULL(TD.TrainingType,'') TrainingType,ISNULL(TD.TrainingOrganizer,'') TrainingOrganizer,ISNULL(TD.Batch,'') Batch,ISNULL(TD.TrainingLocation,'') TrainingLocation,TD.NoOfDays,CONVERT(varchar(10),"+DateExpr("TD.DateFrom")+",105) DateFrom,CONVERT(varchar(10),"+DateExpr("TD.DateTo")+",105) DateTo,ISNULL(TD.TrainingStatus,'') TrainingStatus FROM TrainingDetails TD LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE ISNULL(TD.TrainingStatus,'')=@SummaryKey"+Filter("TD")+" ORDER BY "+DateExpr("TD.DateFrom")+" DESC";
                    title="Training Status: "+key;
                }
                else if(e.CommandName=="TypeSummary")
                {
                    q="SELECT TD.TrainingID,ISNULL(CM.CourseName,'') CourseName,ISNULL(TD.TrainingType,'') TrainingType,ISNULL(TD.TrainingOrganizer,'') TrainingOrganizer,ISNULL(TD.Batch,'') Batch,ISNULL(TD.TrainingLocation,'') TrainingLocation,TD.NoOfDays,CONVERT(varchar(10),"+DateExpr("TD.DateFrom")+",105) DateFrom,CONVERT(varchar(10),"+DateExpr("TD.DateTo")+",105) DateTo,ISNULL(TD.TrainingStatus,'') TrainingStatus FROM TrainingDetails TD LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE ISNULL(TD.TrainingType,'')=@SummaryKey"+Filter("TD")+" ORDER BY "+DateExpr("TD.DateFrom")+" DESC";
                    title="Training Type: "+key;
                }
                else
                {
                    q="WITH TraineeCounts AS (SELECT TA.TrainingID,COUNT(DISTINCT TA.EmpID) TraineeCount FROM TrainingAssignment TA WHERE ISNULL(TA.Cancelled,0)=0 GROUP BY TA.TrainingID), SessionHours AS (SELECT SM.TrainingID,SUM((DATEDIFF(MINUTE,TRY_CONVERT(time,SM.StartTime),TRY_CONVERT(time,SM.EndTime))/60.0)*ISNULL(TC.TraineeCount,0)) ScheduledManHours,SUM(CASE WHEN ISNULL(TD2.TrainingStatus,'') IN ('Completed','TrainingCompleted') THEN (DATEDIFF(MINUTE,TRY_CONVERT(time,SM.StartTime),TRY_CONVERT(time,SM.EndTime))/60.0)*ISNULL(TC.TraineeCount,0) ELSE 0 END) CompletedManHours FROM SessionMaster SM INNER JOIN TrainingDetails TD2 ON SM.TrainingID=TD2.TrainingID LEFT JOIN TraineeCounts TC ON TC.TrainingID=SM.TrainingID WHERE ISNULL(SM.SessionCancelled,0)=0 AND TRY_CONVERT(time,SM.StartTime) IS NOT NULL AND TRY_CONVERT(time,SM.EndTime) IS NOT NULL GROUP BY SM.TrainingID) SELECT TD.TrainingID,ISNULL(CM.CourseName,'') CourseName,ISNULL(TD.TrainingType,'') TrainingType,ISNULL(TD.TrainingStatus,'') TrainingStatus,TD.NoOfDays,ISNULL(SH.ScheduledManHours,0) ScheduledManHours,ISNULL(SH.CompletedManHours,0) CompletedManHours FROM TrainingDetails TD LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID LEFT JOIN SessionHours SH ON SH.TrainingID=TD.TrainingID WHERE ISNULL(CM.CourseName,'')=@SummaryKey"+Filter("TD")+" ORDER BY "+DateExpr("TD.DateFrom")+" DESC";                    title="Course: "+key;
                }
                SqlParameter p=new SqlParameter("@SummaryKey",key);
                DataTable dt=GetFiltered(q,p);
                ViewState["KpiDetailQuery"]=q;
                ViewState["KpiDetailSummaryKey"]=key;
                gvKpiDetails.DataSource=dt;
                gvKpiDetails.DataBind();
                lblKpiDetailsTitle.Text=title+" | "+dt.Rows.Count+" row(s)";
                ScriptManager.RegisterStartupScript(this,GetType(),"showKpiDetails","showKpiDetails();",true);
            }
        }

        protected void btnExportKpiDetails_Click(object sender,EventArgs e)
        {
            string q=ViewState["KpiDetailQuery"] as string;
            if(string.IsNullOrWhiteSpace(q))return;
            DataTable dt=ViewState["KpiDetailSummaryKey"]==null ? GetFiltered(q) : GetFiltered(q,new SqlParameter("@SummaryKey",ViewState["KpiDetailSummaryKey"].ToString()));
            Response.Clear();
            Response.Buffer=true;
            Response.AddHeader("Content-Disposition","attachment;filename=TrainingMIS_Details.xls");
            Response.ContentType="application/vnd.ms-excel";
            Response.ContentEncoding=Encoding.UTF8;
            StringBuilder sb=new StringBuilder();
            for(int i=0;i<dt.Columns.Count;i++){if(i>0)sb.Append("\t");sb.Append(dt.Columns[i].ColumnName);}
            sb.AppendLine();
            foreach(DataRow row in dt.Rows)
            {
                for(int i=0;i<dt.Columns.Count;i++){if(i>0)sb.Append("\t");string value=Convert.ToString(row[i]).Replace("\t"," ").Replace("\r"," ").Replace("\n"," ");sb.Append(value);}
                sb.AppendLine();
            }
            Response.Write(sb.ToString());
            System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        private DataTable GetFiltered(string q,params SqlParameter[] p)
        {
            using(SqlConnection con=new SqlConnection(constr))
            using(SqlCommand cmd=new SqlCommand(q,con))
            {
                AddCommon(cmd);
                if(p!=null)cmd.Parameters.AddRange(p);
                DataTable dt=new DataTable();
                using(SqlDataAdapter da=new SqlDataAdapter(cmd))da.Fill(dt);
                return dt;
            }
        }

        private DataTable GetFiltered(string q)
        {
            using(SqlConnection con=new SqlConnection(constr))
            using(SqlCommand cmd=new SqlCommand(q,con))
            {
                AddCommon(cmd);
                DataTable dt=new DataTable();
                using(SqlDataAdapter da=new SqlDataAdapter(cmd))da.Fill(dt);
                return dt;
            }
        }

        protected void btnExport_Click(object sender,EventArgs e)
        {
            DataTable dt=GetFiltered("SELECT TD.TrainingID,ISNULL(CM.CourseName,'') CourseName,ISNULL(TD.TrainingType,'') TrainingType,ISNULL(TD.TrainingStatus,'') TrainingStatus,TD.NoOfDays FROM TrainingDetails TD LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE 1=1"+Filter("TD")+" ORDER BY "+DateExpr("TD.DateFrom"));
            Response.Clear();Response.Buffer=true;Response.AddHeader("content-disposition","attachment;filename=TrainingMIS.xls");Response.Charset="";Response.ContentType="application/vnd.ms-excel";
            StringBuilder sb=new StringBuilder();foreach(DataColumn c in dt.Columns)sb.Append(c.ColumnName+"\t");sb.Append("\r\n");foreach(DataRow r in dt.Rows){foreach(DataColumn c in dt.Columns)sb.Append(r[c].ToString().Replace("\t"," ")+"\t");sb.Append("\r\n");}Response.Write(sb.ToString());Response.End();
        }

        public override void VerifyRenderingInServerForm(Control control){}
    }
}