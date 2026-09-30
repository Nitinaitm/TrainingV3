using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Manager
{
    public partial class TrainingDetails : Page
    {
        private readonly clsDataAccess objDB=new clsDataAccess();
        private string ManagerID { get { return Session["ManagerID"]==null ? "" : Session["ManagerID"].ToString().Trim(); } }
        private string TrainingID { get { return Session["TrainingID"]==null ? "" : Session["TrainingID"].ToString().Trim(); } }

        protected void Page_Load(object sender,EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(ManagerID)){Response.Redirect("~/Default.aspx");return;}
            if(!IsPostBack){LoadTraining();}
        }

        private string GetTrainingLocationID()
        {
            DataTable dt=objDB.GetDataTable("SELECT TOP 1 TrainingLocationID FROM ManagerMaster WHERE ManagerID=@ManagerID AND ISNULL(ActiveStatus,'Y')='Y'",new SqlParameter[]{new SqlParameter("@ManagerID",ManagerID)});
            return dt.Rows.Count==0 ? "" : dt.Rows[0]["TrainingLocationID"].ToString().Trim();
        }

        private bool HasTrainingAccess()
        {
            object v=objDB.ExecuteScalar("SELECT COUNT(*) FROM ManagerMaster M INNER JOIN TrainingLocationMaster L ON M.TrainingLocationID=L.TrainingLocationID INNER JOIN TrainingDetails TD ON TD.TrainingLocation=L.TrainingLocation WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND M.TrainingLocationID=@TrainingLocationID AND TD.TrainingID=@TrainingID",new SqlParameter[]{new SqlParameter("@ManagerID",ManagerID),new SqlParameter("@TrainingLocationID",GetTrainingLocationID()),new SqlParameter("@TrainingID",TrainingID)});
            return v!=null&&v!=DBNull.Value&&Convert.ToInt32(v)>0;
        }

        private void LoadTraining()
        {
            if(string.IsNullOrWhiteSpace(TrainingID)){ShowMessage("Training not selected.");return;}
            if(!HasTrainingAccess()){ShowMessage("You are not mapped to this Training Location.");return;}
            DataTable dt=objDB.GetDataTable("SELECT TOP 1 TD.TrainingID,TD.TrainingType,TD.TrainingOrganizer,TD.TrainingLocation,TD.Batch,TD.DateFrom,TD.DateTo,TD.TrainingStatus FROM ManagerMaster M INNER JOIN TrainingLocationMaster L ON M.TrainingLocationID=L.TrainingLocationID INNER JOIN TrainingDetails TD ON TD.TrainingLocation=L.TrainingLocation WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND M.TrainingLocationID=@TrainingLocationID AND TD.TrainingID=@TrainingID",new SqlParameter[]{new SqlParameter("@ManagerID",ManagerID),new SqlParameter("@TrainingLocationID",GetTrainingLocationID()),new SqlParameter("@TrainingID",TrainingID)});
            if(dt.Rows.Count==0){ShowMessage("Training details not found.");return;}
            DataRow r=dt.Rows[0];
            lblTrainingID.Text=r["TrainingID"].ToString();
            lblTrainingType.Text=r["TrainingType"].ToString();
            lblOrganizer.Text=r["TrainingOrganizer"].ToString();
            lblTrainingLocation.Text=r["TrainingLocation"].ToString();
            lblBatch.Text=r["Batch"].ToString();
            lblDateFrom.Text=r["DateFrom"].ToString();
            lblDateTo.Text=r["DateTo"].ToString();
            lblStatus.Text=r["TrainingStatus"].ToString();
            BindSessions();
        }

        private void BindSessions()
        {
            DataTable dt=objDB.GetDataTable("SELECT SM.SessionID,ISNULL(SM.SessionNo,'') SessionNo,ISNULL(SM.SessionName,'') SessionName,ISNULL(SM.SessionDate,'') SessionDate,ISNULL(EBM.EmpName,TME.NameExternal) TrainerName FROM ManagerMaster M INNER JOIN TrainingLocationMaster L ON M.TrainingLocationID=L.TrainingLocationID INNER JOIN TrainingDetails TD ON TD.TrainingLocation=L.TrainingLocation INNER JOIN SessionMaster SM ON SM.TrainingID=TD.TrainingID LEFT JOIN EmpBasicMaster EBM ON SM.TrainerID=EBM.EmpID LEFT JOIN TrainerMaster TME ON SM.TrainerID=TME.TrainerID WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND M.TrainingLocationID=@TrainingLocationID AND TD.TrainingID=@TrainingID ORDER BY TRY_CONVERT(int,SM.SessionNo),SM.SessionID",new SqlParameter[]{new SqlParameter("@ManagerID",ManagerID),new SqlParameter("@TrainingLocationID",GetTrainingLocationID()),new SqlParameter("@TrainingID",TrainingID)});
            gvSession.DataSource=dt;
            gvSession.DataBind();
        }

        private bool ValidateSession()
        {
            GridViewRow selected=null;
            foreach(GridViewRow row in gvSession.Rows){RadioButton rb=row.FindControl("rbSession") as RadioButton;if(rb!=null&&rb.Checked){selected=row;break;}}
            if(string.IsNullOrWhiteSpace(TrainingID)||selected==null){ShowMessage("Please select a session.");return false;}
            string sessionID=gvSession.DataKeys[selected.RowIndex].Value.ToString();
            object v=objDB.ExecuteScalar("SELECT COUNT(*) FROM ManagerMaster M INNER JOIN TrainingLocationMaster L ON M.TrainingLocationID=L.TrainingLocationID INNER JOIN TrainingDetails TD ON TD.TrainingLocation=L.TrainingLocation INNER JOIN SessionMaster SM ON SM.TrainingID=TD.TrainingID WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND M.TrainingLocationID=@TrainingLocationID AND TD.TrainingID=@TrainingID AND SM.SessionID=@SessionID",new SqlParameter[]{new SqlParameter("@ManagerID",ManagerID),new SqlParameter("@TrainingLocationID",GetTrainingLocationID()),new SqlParameter("@TrainingID",TrainingID),new SqlParameter("@SessionID",sessionID)});
            if(v==null||v==DBNull.Value||Convert.ToInt32(v)==0){ShowMessage("Selected session is not available for this Manager.");return false;}
            Session["SessionID"]=sessionID;
            Session["TrainerID"]=ManagerID;
            return true;
        }

        private void Go(string url){if(!ValidateSession())return;Response.Redirect(url);}
        protected void btnMaterial_Click(object sender,EventArgs e){Go("~/Manager/TrainingMaterial.aspx");}
        protected void btnAttendance_Click(object sender,EventArgs e){Go("~/Manager/SessionAttendance.aspx");}
        protected void btnPreTest_Click(object sender,EventArgs e){Go("~/Manager/PreTrainingTest.aspx");}
        protected void btnPostTest_Click(object sender,EventArgs e){Go("~/Manager/PostTrainingTest.aspx");}
        protected void btnResult_Click(object sender,EventArgs e){Go("~/Manager/ExamResultReport.aspx");}
        private void ShowMessage(string message){lblMessage.ForeColor=System.Drawing.Color.Red;lblMessage.Text=message;}
    }
}