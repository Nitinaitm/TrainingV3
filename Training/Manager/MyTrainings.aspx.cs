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

        protected void Page_Load(object sender,EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(ManagerID)){Response.Redirect("~/Default.aspx");return;}
            if(!IsPostBack)
            {
                string sessionID=Request.QueryString["SessionID"];
                if(!string.IsNullOrWhiteSpace(sessionID))
                {
                    DataTable sessionTable=objDB.GetDataTable("SELECT TOP 1 SM.TrainingID FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID INNER JOIN ManagerMaster M ON M.TrainingLocationID IN (SELECT TrainingLocationID FROM TrainingLocationMaster WHERE TrainingLocation=TD.TrainingLocation) WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND SM.SessionID=@SessionID",new SqlParameter[]{new SqlParameter("@ManagerID",ManagerID),new SqlParameter("@SessionID",sessionID)});
                    if(sessionTable.Rows.Count>0)
                    {
                        Session["TrainingID"]=sessionTable.Rows[0]["TrainingID"].ToString();
                        Session["SessionID"]=sessionID;
                        Response.Redirect("~/Manager/TrainingDetails.aspx",true);
                        return;
                    }
                    Response.Redirect("~/Manager/MyTrainings.aspx",true);
                    return;
                }
                BindTraining();
            }
        }

        private string GetTrainingLocationID()
        {
            DataTable dt=objDB.GetDataTable("SELECT TOP 1 TrainingLocationID FROM ManagerMaster WHERE ManagerID=@ManagerID AND ISNULL(ActiveStatus,'Y')='Y'",new SqlParameter[]{new SqlParameter("@ManagerID",ManagerID)});
            return dt.Rows.Count==0 ? "" : dt.Rows[0]["TrainingLocationID"].ToString().Trim();
        }

        private void BindTraining()
        {
            string trainingLocationID=GetTrainingLocationID();
            string sql="SELECT DISTINCT TD.TrainingID,TD.TrainingType,TD.TrainingOrganizer,TD.TrainingLocation,TD.Batch,TD.DateFrom,TD.DateTo,TD.TrainingStatus,TRY_CONVERT(date,TD.DateFrom,105) AS SortDate FROM ManagerMaster M INNER JOIN TrainingLocationMaster L ON M.TrainingLocationID=L.TrainingLocationID INNER JOIN TrainingDetails TD ON TD.TrainingLocation=L.TrainingLocation WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND M.TrainingLocationID=@TrainingLocationID";
            if(!string.IsNullOrWhiteSpace(txtTrainingID.Text))sql+=" AND TD.TrainingID LIKE @TrainingID";
            if(!string.IsNullOrWhiteSpace(txtTrainingType.Text))sql+=" AND TD.TrainingType LIKE @TrainingType";
            if(!string.IsNullOrWhiteSpace(txtBatch.Text))sql+=" AND TD.Batch LIKE @Batch";
            string dashboardFilter = Session["ManagerTrainingDashboardFilter"] == null ? "" : Session["ManagerTrainingDashboardFilter"].ToString();
            if(dashboardFilter=="ActiveTrainings" || dashboardFilter=="InProgress")sql+=" AND ISNULL(TD.TrainingStatus,'') IN ('Planned','InProgress','AttendanceCompleted')";
            else if(dashboardFilter=="CompletedTrainings" || dashboardFilter=="Completed")sql+=" AND ISNULL(TD.TrainingStatus,'') IN ('Completed','TrainingCompleted')";
            else if(dashboardFilter=="Planned")sql+=" AND ISNULL(TD.TrainingStatus,'')='Planned'";
            else if(!string.IsNullOrWhiteSpace(ddlStatus.SelectedValue))sql+=" AND ISNULL(TD.TrainingStatus,'')=@TrainingStatus";
            if(!string.IsNullOrWhiteSpace(txtFromDate.Text))sql+=" AND TRY_CONVERT(date,TD.DateFrom,105)>=TRY_CONVERT(date,@FromDate,105)";
            if(!string.IsNullOrWhiteSpace(txtToDate.Text))sql+=" AND TRY_CONVERT(date,TD.DateTo,105)<=TRY_CONVERT(date,@ToDate,105)";
            sql+=" ORDER BY SortDate DESC";
            DataTable dt=objDB.GetDataTable(sql,new SqlParameter[]{new SqlParameter("@ManagerID",ManagerID),new SqlParameter("@TrainingLocationID",trainingLocationID),new SqlParameter("@TrainingID","%"+txtTrainingID.Text.Trim()+"%"),new SqlParameter("@TrainingType","%"+txtTrainingType.Text.Trim()+"%"),new SqlParameter("@Batch","%"+txtBatch.Text.Trim()+"%"),new SqlParameter("@TrainingStatus",ddlStatus.SelectedValue),new SqlParameter("@FromDate",txtFromDate.Text.Trim()),new SqlParameter("@ToDate",txtToDate.Text.Trim())});
            gvTraining.DataSource=dt;
            gvTraining.DataBind();
        }

        protected void btnSearch_Click(object sender,EventArgs e){Session.Remove("ManagerTrainingDashboardFilter");BindTraining();}
        protected void btnReset_Click(object sender,EventArgs e){Session.Remove("ManagerTrainingDashboardFilter");txtTrainingID.Text="";txtTrainingType.Text="";txtBatch.Text="";txtFromDate.Text="";txtToDate.Text="";ddlStatus.SelectedIndex=0;BindTraining();}

        protected void gvTraining_RowCommand(object sender,GridViewCommandEventArgs e)
        {
            if(!string.Equals(e.CommandName,"ViewTraining",StringComparison.OrdinalIgnoreCase))return;
            string trainingID=Convert.ToString(e.CommandArgument);
            if(string.IsNullOrWhiteSpace(trainingID)){ShowMessage("Training ID is missing.");return;}
            if(!HasTrainingAccess(trainingID)){ShowMessage("You are not mapped to this Training Location.");return;}
            Session["TrainingID"]=trainingID;
            Session.Remove("SessionID");
            Response.Redirect("~/Manager/TrainingDetails.aspx");
        }

        private bool HasTrainingAccess(string trainingID)
        {
            object v=objDB.ExecuteScalar("SELECT COUNT(*) FROM ManagerMaster M INNER JOIN TrainingLocationMaster L ON M.TrainingLocationID=L.TrainingLocationID INNER JOIN TrainingDetails TD ON TD.TrainingLocation=L.TrainingLocation WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND M.TrainingLocationID=@TrainingLocationID AND TD.TrainingID=@TrainingID",new SqlParameter[]{new SqlParameter("@ManagerID",ManagerID),new SqlParameter("@TrainingLocationID",GetTrainingLocationID()),new SqlParameter("@TrainingID",trainingID)});
            return v!=null&&v!=DBNull.Value&&Convert.ToInt32(v)>0;
        }

        private void ShowMessage(string message){lblMessage.ForeColor=System.Drawing.Color.Red;lblMessage.Text=message;}
    }
}