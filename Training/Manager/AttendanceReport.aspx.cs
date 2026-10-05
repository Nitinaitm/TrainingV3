using System;
using System.Data;
using System.Data.SqlClient;

namespace Training.Manager
{
    public partial class AttendanceReport : System.Web.UI.Page
    {
        clsDataAccess obj=new clsDataAccess();
        private string ManagerID { get { return Session["ManagerID"] == null ? "" : Session["ManagerID"].ToString().Trim(); } }
        private string SessionID { get { return Session["SessionID"] == null ? "" : Session["SessionID"].ToString().Trim(); } }
        private string TrainingID { get { return Session["TrainingID"] == null ? "" : Session["TrainingID"].ToString().Trim(); } }

        protected void Page_Load(object sender,EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(ManagerID)||string.IsNullOrWhiteSpace(SessionID)||string.IsNullOrWhiteSpace(TrainingID))
            {
                Response.Redirect("~/Manager/SessionReport.aspx");
                return;
            }
            if(!IsPostBack) LoadReport();
        }

        private bool HasAccess()
        {
            object v=obj.ExecuteScalar("SELECT COUNT(*) FROM ManagerMaster M INNER JOIN TrainingLocationMaster L ON M.TrainingLocationID=L.TrainingLocationID INNER JOIN TrainingDetails TD ON TD.TrainingLocation=L.TrainingLocation INNER JOIN SessionMaster SM ON SM.TrainingID=TD.TrainingID WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND TD.TrainingID=@TrainingID AND SM.SessionID=@SessionID",new SqlParameter[]{new SqlParameter("@ManagerID",ManagerID),new SqlParameter("@TrainingID",TrainingID),new SqlParameter("@SessionID",SessionID)});
            return v!=null&&v!=DBNull.Value&&Convert.ToInt32(v)>0;
        }

        private void LoadReport()
        {
            if(!HasAccess())
            {
                Response.Redirect("~/Manager/SessionReport.aspx");
                return;
            }
            DataTable s=obj.GetDataTable("SELECT SessionNo,SessionName FROM SessionMaster WHERE SessionID=@SessionID AND TrainingID=@TrainingID",new SqlParameter[]{new SqlParameter("@SessionID",SessionID),new SqlParameter("@TrainingID",TrainingID)});
            if(s.Rows.Count==0)
            {
                Response.Redirect("~/Manager/SessionReport.aspx");
                return;
            }
            lblSession.Text="Session "+s.Rows[0]["SessionNo"]+" - "+s.Rows[0]["SessionName"];
            DataTable d=obj.GetDataTable("SELECT TA.EmpID,EBM.EmpName,EBM.EmpDesignation,ISNULL(SA.AttendanceStatus,'Pending') AttendanceStatus,ISNULL(SA.Remarks,'') Remarks FROM TrainingAssignment TA INNER JOIN EmpBasicMaster EBM ON TA.EmpID=EBM.EmpID LEFT JOIN SessionAttendance SA ON TA.TrainingID=SA.TrainingID AND TA.EmpID=SA.EmpID AND SA.SessionID=@SessionID WHERE TA.TrainingID=@TrainingID AND TA.AssignmentStatus='Assigned' ORDER BY EBM.EmpName",new SqlParameter[]{new SqlParameter("@TrainingID",TrainingID),new SqlParameter("@SessionID",SessionID)});
            gvAttendance.DataSource=d;
            gvAttendance.DataBind();
            int total=d.Rows.Count,present=0,absent=0;
            foreach(DataRow r in d.Rows)
            {
                if(string.Equals(r["AttendanceStatus"].ToString(),"Present",StringComparison.OrdinalIgnoreCase)) present++;
                if(string.Equals(r["AttendanceStatus"].ToString(),"Absent",StringComparison.OrdinalIgnoreCase)) absent++;
            }
            lblTotal.Text=total.ToString();
            lblPresent.Text=present.ToString();
            lblAbsent.Text=absent.ToString();
            lblPending.Text=Math.Max(0,total-present-absent).ToString();
        }

        protected void btnBack_Click(object sender,EventArgs e)
        {
            Response.Redirect("~/Manager/SessionReportDetails.aspx");
        }
    }
}
