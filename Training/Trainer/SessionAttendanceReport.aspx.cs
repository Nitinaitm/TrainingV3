using System;
using System.Data;
using System.Data.SqlClient;

namespace Training.Trainer
{
    public partial class SessionAttendanceReport : System.Web.UI.Page
    {
        clsDataAccess obj=new clsDataAccess();
        private string TrainerID { get { return Session["TrainerID"].ToString(); } }
        protected void Page_Load(object sender,EventArgs e){if(Session["TrainerID"]==null||Session["SessionID"]==null||Session["TrainingID"]==null){Response.Redirect("~/Trainer/SessionReport.aspx");return;}if(!IsPostBack)LoadReport();}
        private bool HasAccess(){object v=obj.ExecuteScalar("SELECT COUNT(*) FROM SessionMaster WHERE SessionID=@SessionID AND TrainingID=@TrainingID AND TrainerID=@TrainerID",new SqlParameter[]{new SqlParameter("@SessionID",Session["SessionID"].ToString()),new SqlParameter("@TrainingID",Session["TrainingID"].ToString()),new SqlParameter("@TrainerID",TrainerID)});return v!=null&&v!=DBNull.Value&&Convert.ToInt32(v)>0;}
        private void LoadReport(){if(!HasAccess()){Response.Redirect("~/Trainer/SessionReport.aspx");return;}DataTable s=obj.GetDataTable("SELECT SessionNo,SessionName FROM SessionMaster WHERE SessionID=@SessionID",new SqlParameter[]{new SqlParameter("@SessionID",Session["SessionID"].ToString())});if(s.Rows.Count>0)lblSession.Text="Session "+s.Rows[0]["SessionNo"]+" - "+s.Rows[0]["SessionName"];DataTable d=obj.GetDataTable("SELECT TA.EmpID,EBM.EmpName,EBM.EmpDesignation,ISNULL(SA.AttendanceStatus,'Pending') AttendanceStatus,ISNULL(SA.Remarks,'') Remarks FROM TrainingAssignment TA INNER JOIN EmpBasicMaster EBM ON TA.EmpID=EBM.EmpID LEFT JOIN SessionAttendance SA ON TA.TrainingID=SA.TrainingID AND TA.EmpID=SA.EmpID AND SA.SessionID=@SessionID WHERE TA.TrainingID=@TrainingID AND TA.AssignmentStatus='Assigned' ORDER BY EBM.EmpName",new SqlParameter[]{new SqlParameter("@TrainingID",Session["TrainingID"].ToString()),new SqlParameter("@SessionID",Session["SessionID"].ToString())});gvAttendance.DataSource=d;gvAttendance.DataBind();int total=d.Rows.Count,present=0,absent=0;foreach(DataRow r in d.Rows){if(r["AttendanceStatus"].ToString()=="Present")present++;if(r["AttendanceStatus"].ToString()=="Absent")absent++;}lblTotal.Text=total.ToString();lblPresent.Text=present.ToString();lblAbsent.Text=absent.ToString();lblPending.Text=Math.Max(0,total-present-absent).ToString();}
        protected void btnBack_Click(object s,EventArgs e){Response.Redirect("~/Trainer/SessionReportDetails.aspx");}
    }
}