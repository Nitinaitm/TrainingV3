using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Manager
{
    public partial class MyTrainings : Page
    {
        private readonly clsDataAccess objDB=new clsDataAccess();
        private string ManagerID { get { return Session["ManagerID"]==null ? "" : Session["ManagerID"].ToString().Trim(); } }
        private string TrainingLocationID { get { return GetManagerTrainingLocationID(); } }
        private string GetManagerTrainingLocationID()
        {
            DataTable dt=objDB.GetDataTable("SELECT TOP 1 TrainingLocationID FROM ManagerMaster WHERE ManagerID=@ManagerID AND ISNULL(ActiveStatus,'Y')='Y'",new SqlParameter[]{new SqlParameter("@ManagerID",ManagerID)});
            return dt.Rows.Count==0 ? "" : dt.Rows[0]["TrainingLocationID"].ToString().Trim();
        }

        protected void Page_Load(object sender,EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(ManagerID)){Response.Redirect("~/Default.aspx");return;}
            if(!IsPostBack){BindTraining();}
        }

        private void BindTraining()
        {
            string trainingLocationID=Session["ManagerTrainingLocationID"]==null ? TrainingLocationID : Session["ManagerTrainingLocationID"].ToString();
            string sql="SELECT DISTINCT TD.TrainingID,TD.TrainingType,TD.TrainingOrganizer,TD.TrainingLocation,TD.Batch,TD.DateFrom,TD.DateTo,TD.TrainingStatus FROM ManagerMaster M INNER JOIN TrainingLocationMaster L ON M.TrainingLocationID=L.TrainingLocationID INNER JOIN TrainingDetails TD ON TD.TrainingLocation=L.TrainingLocation WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND M.TrainingLocationID=@TrainingLocationID AND ISNULL(TD.TrainingStatus,'') NOT IN ('Completed','TrainingCompleted')";
            if(!string.IsNullOrWhiteSpace(txtTrainingID.Text))sql+=" AND TD.TrainingID LIKE @TrainingID";
            if(!string.IsNullOrWhiteSpace(txtTrainingType.Text))sql+=" AND TD.TrainingType LIKE @TrainingType";
            if(!string.IsNullOrWhiteSpace(txtBatch.Text))sql+=" AND TD.Batch LIKE @Batch";
            if(!string.IsNullOrWhiteSpace(ddlStatus.SelectedValue))sql+=" AND ISNULL(TD.TrainingStatus,'')=@TrainingStatus";
            if(!string.IsNullOrWhiteSpace(txtFromDate.Text))sql+=" AND TRY_CONVERT(date,TD.DateFrom,105)>=TRY_CONVERT(date,@FromDate,105)";
            if(!string.IsNullOrWhiteSpace(txtToDate.Text))sql+=" AND TRY_CONVERT(date,TD.DateTo,105)<=TRY_CONVERT(date,@ToDate,105)";
            sql+=" ORDER BY TRY_CONVERT(date,TD.DateFrom,105) DESC";
            DataTable dt=objDB.GetDataTable(sql,new SqlParameter[]{new SqlParameter("@ManagerID",ManagerID),new SqlParameter("@TrainingLocationID",trainingLocationID),new SqlParameter("@TrainingID","%"+txtTrainingID.Text.Trim()+"%"),new SqlParameter("@TrainingType","%"+txtTrainingType.Text.Trim()+"%"),new SqlParameter("@Batch","%"+txtBatch.Text.Trim()+"%"),new SqlParameter("@TrainingStatus",ddlStatus.SelectedValue),new SqlParameter("@FromDate",txtFromDate.Text.Trim()),new SqlParameter("@ToDate",txtToDate.Text.Trim())});
            gvTraining.DataSource=dt;
            gvTraining.DataBind();
        }

        protected void btnSearch_Click(object sender,EventArgs e){pnlSessions.Visible=false;BindTraining();}
        protected void btnReset_Click(object sender,EventArgs e){txtTrainingID.Text="";txtTrainingType.Text="";txtBatch.Text="";txtFromDate.Text="";txtToDate.Text="";ddlStatus.SelectedIndex=0;pnlSessions.Visible=false;BindTraining();}

        protected void gvTraining_RowCommand(object sender,GridViewCommandEventArgs e)
        {
            if(!string.Equals(e.CommandName,"ViewTraining",StringComparison.OrdinalIgnoreCase))return;
            string trainingID=Convert.ToString(e.CommandArgument);
            if(!HasTrainingAccess(trainingID)){ShowMessage("You are not mapped to this Training Location.");return;}
            Session["TrainingID"]=trainingID;
            Session.Remove("SessionID");
            LoadTraining(trainingID);
        }

        private bool HasTrainingAccess(string trainingID)
        {
            object v=objDB.ExecuteScalar("SELECT COUNT(*) FROM ManagerMaster M INNER JOIN TrainingLocationMaster L ON M.TrainingLocationID=L.TrainingLocationID INNER JOIN TrainingDetails TD ON TD.TrainingLocation=L.TrainingLocation WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND M.TrainingLocationID=@TrainingLocationID AND LTRIM(RTRIM(TD.TrainingLocation))=LTRIM(RTRIM(L.TrainingLocation)) AND TD.TrainingID=@TrainingID",new SqlParameter[]{new SqlParameter("@ManagerID",ManagerID),new SqlParameter("@TrainingLocationID",TrainingLocationID),new SqlParameter("@TrainingID",trainingID)});
            return v!=null&&v!=DBNull.Value&&Convert.ToInt32(v)>0;
        }

        private void LoadTraining(string trainingID)
        {
            DataTable dt=objDB.GetDataTable("SELECT TOP 1 TD.TrainingID,TD.TrainingType,TD.TrainingOrganizer,TD.TrainingLocation,TD.Batch,TD.DateFrom,TD.DateTo,TD.TrainingStatus FROM ManagerMaster M INNER JOIN TrainingLocationMaster L ON M.TrainingLocationID=L.TrainingLocationID INNER JOIN TrainingDetails TD ON TD.TrainingLocation=L.TrainingLocation WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND M.TrainingLocationID=@TrainingLocationID AND LTRIM(RTRIM(TD.TrainingLocation))=LTRIM(RTRIM(L.TrainingLocation)) AND TD.TrainingID=@TrainingID",new SqlParameter[]{new SqlParameter("@ManagerID",ManagerID),new SqlParameter("@TrainingLocationID",TrainingLocationID),new SqlParameter("@TrainingID",trainingID)});
            if(dt.Rows.Count==0){pnlSessions.Visible=false;return;}
            DataRow r=dt.Rows[0];lblDetailTrainingID.Text=r["TrainingID"].ToString();lblDetailType.Text=r["TrainingType"].ToString();lblDetailOrganizer.Text=r["TrainingOrganizer"].ToString();lblDetailLocation.Text=r["TrainingLocation"].ToString();lblDetailBatch.Text=r["Batch"].ToString();lblDetailFrom.Text=r["DateFrom"].ToString();lblDetailTo.Text=r["DateTo"].ToString();lblDetailStatus.Text=r["TrainingStatus"].ToString();
            BindSessions(trainingID);pnlSessions.Visible=true;
        }

        private void BindSessions(string trainingID)
        {
            DataTable dt=objDB.GetDataTable("SELECT SM.SessionID,ISNULL(SM.SessionNo,'') SessionNo,ISNULL(SM.SessionName,'') SessionName,ISNULL(SM.SessionDate,'') SessionDate,ISNULL(EBM.EmpName,TME.NameExternal) TrainerName FROM ManagerMaster M INNER JOIN TrainingLocationMaster L ON M.TrainingLocationID=L.TrainingLocationID INNER JOIN TrainingDetails TD ON TD.TrainingLocation=L.TrainingLocation INNER JOIN SessionMaster SM ON SM.TrainingID=TD.TrainingID LEFT JOIN EmpBasicMaster EBM ON SM.TrainerID=EBM.EmpID LEFT JOIN TrainerMaster TME ON SM.TrainerID=TME.TrainerID WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND M.TrainingLocationID=@TrainingLocationID AND LTRIM(RTRIM(TD.TrainingLocation))=LTRIM(RTRIM(L.TrainingLocation)) AND TD.TrainingID=@TrainingID ORDER BY TRY_CONVERT(int,SM.SessionNo),SM.SessionID",new SqlParameter[]{new SqlParameter("@ManagerID",ManagerID),new SqlParameter("@TrainingLocationID",TrainingLocationID),new SqlParameter("@TrainingID",trainingID)});
            gvSession.DataSource=dt;gvSession.DataBind();
        }

        private bool ValidateSession()
        {
            string trainingID=Session["TrainingID"]==null ? "" : Session["TrainingID"].ToString();
            GridViewRow selected=null;
            foreach(GridViewRow row in gvSession.Rows){RadioButton rb=row.FindControl("rbSession") as RadioButton;if(rb!=null&&rb.Checked){selected=row;break;}}
            if(string.IsNullOrWhiteSpace(trainingID)||selected==null){ShowMessage("Please select a session.");return false;}
            string sessionID=gvSession.DataKeys[selected.RowIndex].Value.ToString();
            object v=objDB.ExecuteScalar("SELECT COUNT(*) FROM ManagerMaster M INNER JOIN TrainingLocationMaster L ON M.TrainingLocationID=L.TrainingLocationID INNER JOIN TrainingDetails TD ON TD.TrainingLocation=L.TrainingLocation INNER JOIN SessionMaster SM ON SM.TrainingID=TD.TrainingID WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND M.TrainingLocationID=@TrainingLocationID AND LTRIM(RTRIM(TD.TrainingLocation))=LTRIM(RTRIM(L.TrainingLocation)) AND TD.TrainingID=@TrainingID AND SM.SessionID=@SessionID",new SqlParameter[]{new SqlParameter("@ManagerID",ManagerID),new SqlParameter("@TrainingLocationID",TrainingLocationID),new SqlParameter("@TrainingID",trainingID),new SqlParameter("@SessionID",sessionID)});
            if(v==null||v==DBNull.Value||Convert.ToInt32(v)==0){ShowMessage("Selected session is not available for this Manager.");return false;}
            Session["SessionID"]=sessionID;Session["TrainerID"]=ManagerID;return true;
        }

        private void Go(string url){if(!ValidateSession())return;Response.Redirect(url);}
        protected void btnMaterial_Click(object sender,EventArgs e){Go("~/Manager/TrainingMaterial.aspx");}
        protected void btnAttendance_Click(object sender,EventArgs e){Go("~/Manager/SessionAttendance.aspx");}
        protected void btnPreTest_Click(object sender,EventArgs e){Go("~/Manager/PreTrainingTest.aspx");}
        protected void btnPostTest_Click(object sender,EventArgs e){Go("~/Manager/PostTrainingTest.aspx");}
        protected void btnResult_Click(object sender,EventArgs e){if(!ValidateSession())return;Response.Redirect("~/Manager/ExamResultReport.aspx");}
        private void ShowMessage(string message){lblMessage.ForeColor=System.Drawing.Color.Red;lblMessage.Text=message;}
    }
}