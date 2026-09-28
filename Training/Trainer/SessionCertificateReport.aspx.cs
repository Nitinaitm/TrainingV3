using System;
using System.Data;
using System.Data.SqlClient;

namespace Training.Trainer
{
    public partial class SessionCertificateReport : System.Web.UI.Page
    {
        clsDataAccess obj=new clsDataAccess();
        private string TrainerID { get { return Session["TrainerID"].ToString(); } }
        protected void Page_Load(object sender,EventArgs e){if(Session["TrainerID"]==null||Session["SessionID"]==null||Session["TrainingID"]==null){Response.Redirect("~/Trainer/SessionReport.aspx");return;}if(!IsPostBack)LoadReport();}
        private bool HasAccess(){object v=obj.ExecuteScalar("SELECT COUNT(*) FROM SessionMaster WHERE SessionID=@SessionID AND TrainingID=@TrainingID AND TrainerID=@TrainerID",new SqlParameter[]{new SqlParameter("@SessionID",Session["SessionID"].ToString()),new SqlParameter("@TrainingID",Session["TrainingID"].ToString()),new SqlParameter("@TrainerID",TrainerID)});return v!=null&&v!=DBNull.Value&&Convert.ToInt32(v)>0;}
        private void LoadReport(){if(!HasAccess()){Response.Redirect("~/Trainer/SessionReport.aspx");return;}DataTable s=obj.GetDataTable("SELECT SessionNo,SessionName FROM SessionMaster WHERE SessionID=@SessionID",new SqlParameter[]{new SqlParameter("@SessionID",Session["SessionID"].ToString())});if(s.Rows.Count>0)lblSession.Text="Session "+s.Rows[0]["SessionNo"]+" - "+s.Rows[0]["SessionName"];string q="SELECT TC.EmpID,ISNULL(E.EmpName,TC.EmpID) EmpName,TC.CertificateNo,TC.VerificationCode,TC.PDFName,TC.GeneratedOn FROM TrainingCertificate TC LEFT JOIN EmpBasicMaster E ON TC.EmpID=E.EmpID WHERE TC.TrainingID=@TrainingID ORDER BY TC.GeneratedOn DESC";gvCertificate.DataSource=obj.GetDataTable(q,new SqlParameter[]{new SqlParameter("@TrainingID",Session["TrainingID"].ToString())});gvCertificate.DataBind();}
        protected void btnBack_Click(object s,EventArgs e){Response.Redirect("~/Trainer/SessionReportDetails.aspx");}
    }
}