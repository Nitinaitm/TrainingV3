using System;
using System.Data;
using System.Data.SqlClient;

namespace Training.Trainer
{
    public partial class SessionReportDetails : System.Web.UI.Page
    {
        clsDataAccess obj=new clsDataAccess();
        private string TrainerID { get { return Session["TrainerID"].ToString(); } }

        protected void Page_Load(object sender,EventArgs e)
        {
            if(Session["TrainerID"]==null||Session["SessionID"]==null||Session["TrainingID"]==null){Response.Redirect("~/Trainer/SessionReport.aspx");return;}
            if(!IsPostBack)LoadSession();
        }

        private bool HasAccess()
        {
            object value=obj.ExecuteScalar("SELECT COUNT(*) FROM SessionMaster WHERE SessionID=@SessionID AND TrainingID=@TrainingID AND TrainerID=@TrainerID",new SqlParameter[]{new SqlParameter("@SessionID",Session["SessionID"].ToString()),new SqlParameter("@TrainingID",Session["TrainingID"].ToString()),new SqlParameter("@TrainerID",TrainerID)});
            return value!=null&&value!=DBNull.Value&&Convert.ToInt32(value)>0;
        }

        private void LoadSession()
        {
            if(!HasAccess()){Response.Redirect("~/Trainer/SessionReport.aspx");return;}
            DataTable dt=obj.GetDataTable("SELECT SM.TrainingID,CM.CourseName,TD.Batch,SM.SessionNo,SM.SessionName,ISNULL(TP.TopicName,'') TopicName,SM.SessionDate,SM.StartTime,SM.EndTime FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID INNER JOIN CourseMaster CM ON TD.CourseID=CM.CourseID LEFT JOIN TopicMaster TP ON SM.TopicID=TP.TopicID WHERE SM.SessionID=@SessionID AND SM.TrainingID=@TrainingID AND SM.TrainerID=@TrainerID",new SqlParameter[]{new SqlParameter("@SessionID",Session["SessionID"].ToString()),new SqlParameter("@TrainingID",Session["TrainingID"].ToString()),new SqlParameter("@TrainerID",TrainerID)});
            if(dt.Rows.Count==0){Response.Redirect("~/Trainer/SessionReport.aspx");return;}
            DataRow r=dt.Rows[0];lblTrainingID.Text=r["TrainingID"].ToString();lblCourse.Text=r["CourseName"].ToString();lblBatch.Text=r["Batch"].ToString();lblSession.Text=r["SessionNo"].ToString()+" - "+r["SessionName"].ToString();lblTopic.Text=r["TopicName"].ToString();lblDate.Text=r["SessionDate"].ToString();lblStart.Text=r["StartTime"].ToString();lblEnd.Text=r["EndTime"].ToString();
        }

        private void SetReportMode(string type){Session["SessionReportTestType"]=type;Response.Redirect("~/Trainer/ExamResultReport.aspx");}
        protected void btnAttendance_Click(object s,EventArgs e){Response.Redirect("~/Trainer/SessionAttendanceReport.aspx");}
        protected void btnPreTest_Click(object s,EventArgs e){SetReportMode("Pre");}
        protected void btnPostTest_Click(object s,EventArgs e){SetReportMode("Post");}
        protected void btnTestResult_Click(object s,EventArgs e){Session["SessionReportTestType"]="";Response.Redirect("~/Trainer/ExamResultReport.aspx");}
        protected void btnAnswers_Click(object s,EventArgs e){Response.Redirect("~/Trainer/SessionAnswerReport.aspx");}
        protected void btnFeedback_Click(object s,EventArgs e){Response.Redirect("~/Trainer/FeedbackReport.aspx");}
        protected void btnCertificate_Click(object s,EventArgs e){Response.Redirect("~/Trainer/SessionCertificateReport.aspx");}
        protected void btnBack_Click(object s,EventArgs e){Response.Redirect("~/Trainer/SessionReport.aspx");}
    }
}