using System;
using System.Data;
using System.Data.SqlClient;

namespace Training.Manager
{
    public partial class SessionCertificateReport : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        private string ManagerID { get { return Session["ManagerID"] == null ? "" : Session["ManagerID"].ToString().Trim(); } }

        protected void Page_Load(object sender,EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ManagerID) || Session["SessionID"] == null || Session["TrainingID"] == null)
            {
                Response.Redirect("~/Manager/SessionReport.aspx");
                return;
            }

            if (!IsPostBack) LoadReport();
        }

        private bool HasAccess()
        {
            object value=obj.ExecuteScalar("SELECT COUNT(*) FROM ManagerMaster M INNER JOIN TrainingLocationMaster L ON M.TrainingLocationID=L.TrainingLocationID INNER JOIN TrainingDetails TD ON TD.TrainingLocation=L.TrainingLocation INNER JOIN SessionMaster SM ON SM.TrainingID=TD.TrainingID WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND TD.TrainingID=@TrainingID AND SM.SessionID=@SessionID",new SqlParameter[]{new SqlParameter("@ManagerID",ManagerID),new SqlParameter("@TrainingID",Session["TrainingID"].ToString()),new SqlParameter("@SessionID",Session["SessionID"].ToString())});
            return value!=null&&value!=DBNull.Value&&Convert.ToInt32(value)>0;
        }

        private void LoadReport()
        {
            if(!HasAccess()){Response.Redirect("~/Manager/SessionReport.aspx");return;}

            DataTable s=obj.GetDataTable("SELECT SessionNo,SessionName FROM SessionMaster WHERE SessionID=@SessionID AND TrainingID=@TrainingID",new SqlParameter[]{new SqlParameter("@SessionID",Session["SessionID"].ToString()),new SqlParameter("@TrainingID",Session["TrainingID"].ToString())});
            if(s.Rows.Count>0) lblSession.Text="Session "+s.Rows[0]["SessionNo"]+" - "+s.Rows[0]["SessionName"];

            string q="SELECT TC.EmpID,ISNULL(E.EmpName,TC.EmpID) EmpName,TC.CertificateNo,TC.VerificationCode,TC.PDFName,TC.GeneratedOn FROM TrainingCertificate TC LEFT JOIN EmpBasicMaster E ON TC.EmpID=E.EmpID INNER JOIN TrainingDetails TD ON TC.TrainingID=TD.TrainingID INNER JOIN TrainingLocationMaster L ON TD.TrainingLocation=L.TrainingLocation INNER JOIN ManagerMaster M ON M.TrainingLocationID=L.TrainingLocationID WHERE TC.TrainingID=@TrainingID AND M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' ORDER BY TC.GeneratedOn DESC";

            gvCertificate.DataSource=obj.GetDataTable(q,new SqlParameter[]{new SqlParameter("@TrainingID",Session["TrainingID"].ToString()),new SqlParameter("@ManagerID",ManagerID)});
            gvCertificate.DataBind();
        }

        protected void btnBack_Click(object sender,EventArgs e)
        {
            Response.Redirect("~/Manager/SessionReportDetails.aspx");
        }
    }
}