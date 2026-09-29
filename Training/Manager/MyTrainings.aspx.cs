using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Manager
{
    public partial class MyTrainings : Page
    {
        private readonly clsDataAccess objDB = new clsDataAccess();

        private string ManagerID
        {
            get { return Session["ManagerID"] == null ? "" : Session["ManagerID"].ToString().Trim(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ManagerID))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (Session["Role"] == null || Session["Role"].ToString() != "Manager")
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindTraining();
            }
        }

        private string ScopeSql()
        {
            return " TD.TrainingLocation=L.TrainingLocation AND M.TrainingLocationID=L.TrainingLocationID AND M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' ";
        }

        private void BindTraining()
        {
            string sql = "SELECT DISTINCT TD.TrainingID,TD.TrainingType,TD.TrainingOrganizer,TD.TrainingLocation,TD.Batch,TD.DateFrom,TD.DateTo,TD.TrainingStatus FROM ManagerMaster M INNER JOIN TrainingLocationMaster L ON M.TrainingLocationID=L.TrainingLocationID INNER JOIN TrainingDetails TD ON TD.TrainingLocation=L.TrainingLocation WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y'";

            if (!string.IsNullOrWhiteSpace(txtTrainingID.Text)) sql += " AND TD.TrainingID LIKE @TrainingID";
            if (!string.IsNullOrWhiteSpace(txtTrainingType.Text)) sql += " AND TD.TrainingType LIKE @TrainingType";
            if (!string.IsNullOrWhiteSpace(txtBatch.Text)) sql += " AND TD.Batch LIKE @Batch";
            if (!string.IsNullOrWhiteSpace(ddlStatus.SelectedValue)) sql += " AND ISNULL(TD.TrainingStatus,'')=@TrainingStatus";
            if (!string.IsNullOrWhiteSpace(txtFromDate.Text)) sql += " AND TRY_CONVERT(date,TD.DateFrom,105)>=TRY_CONVERT(date,@FromDate,105)";
            if (!string.IsNullOrWhiteSpace(txtToDate.Text)) sql += " AND TRY_CONVERT(date,TD.DateTo,105)<=TRY_CONVERT(date,@ToDate,105)";
            sql += " ORDER BY TRY_CONVERT(date,TD.DateFrom,105) DESC,TD.TrainingID DESC";

            var p = new System.Collections.Generic.List<SqlParameter>();
            p.Add(new SqlParameter("@ManagerID",ManagerID));
            p.Add(new SqlParameter("@TrainingID","%" + txtTrainingID.Text.Trim() + "%"));
            p.Add(new SqlParameter("@TrainingType","%" + txtTrainingType.Text.Trim() + "%"));
            p.Add(new SqlParameter("@Batch","%" + txtBatch.Text.Trim() + "%"));
            p.Add(new SqlParameter("@TrainingStatus",ddlStatus.SelectedValue));
            p.Add(new SqlParameter("@FromDate",txtFromDate.Text.Trim()));
            p.Add(new SqlParameter("@ToDate",txtToDate.Text.Trim()));

            DataTable dt=objDB.GetDataTable(sql,p.ToArray());
            gvTraining.DataSource=dt;
            gvTraining.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e) { pnlDetails.Visible=false; BindTraining(); }
        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtTrainingID.Text="";
            txtTrainingType.Text="";
            txtBatch.Text="";
            txtFromDate.Text="";
            txtToDate.Text="";
            ddlStatus.SelectedIndex=0;
            pnlDetails.Visible=false;
            BindTraining();
        }

        protected void gvTraining_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!string.Equals(e.CommandName,"ViewTraining",StringComparison.OrdinalIgnoreCase)) return;
            string trainingID=Convert.ToString(e.CommandArgument);
            if (string.IsNullOrWhiteSpace(trainingID)) return;
            if (!HasTrainingAccess(trainingID))
            {
                lblMessage.ForeColor=System.Drawing.Color.Red;
                lblMessage.Text="You are not mapped to this Training Location.";
                pnlDetails.Visible=false;
                return;
            }
            Session["TrainingID"]=trainingID;
            Session["TrainerID"]=ManagerID;
            LoadDetails(trainingID);
        }

        private bool HasTrainingAccess(string trainingID)
        {
            object v=objDB.ExecuteScalar("SELECT COUNT(*) FROM ManagerMaster M INNER JOIN TrainingLocationMaster L ON M.TrainingLocationID=L.TrainingLocationID INNER JOIN TrainingDetails TD ON TD.TrainingLocation=L.TrainingLocation WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND TD.TrainingID=@TrainingID",new SqlParameter[]{new SqlParameter("@ManagerID",ManagerID),new SqlParameter("@TrainingID",trainingID)});
            return v!=null && v!=DBNull.Value && Convert.ToInt32(v)>0;
        }

        private void LoadDetails(string trainingID)
        {
            DataTable dt=objDB.GetDataTable("SELECT TOP 1 TD.TrainingID,TD.TrainingType,TD.TrainingOrganizer,TD.TrainingLocation,TD.Batch,TD.DateFrom,TD.DateTo,TD.TrainingStatus FROM ManagerMaster M INNER JOIN TrainingLocationMaster L ON M.TrainingLocationID=L.TrainingLocationID INNER JOIN TrainingDetails TD ON TD.TrainingLocation=L.TrainingLocation WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND TD.TrainingID=@TrainingID",new SqlParameter[]{new SqlParameter("@ManagerID",ManagerID),new SqlParameter("@TrainingID",trainingID)});
            if(dt.Rows.Count==0){pnlDetails.Visible=false;return;}
            DataRow r=dt.Rows[0];
            lblDetailTrainingID.Text=r["TrainingID"].ToString();
            lblDetailType.Text=r["TrainingType"].ToString();
            lblDetailOrganizer.Text=r["TrainingOrganizer"].ToString();
            lblDetailLocation.Text=r["TrainingLocation"].ToString();
            lblDetailBatch.Text=r["Batch"].ToString();
            lblDetailFrom.Text=r["DateFrom"].ToString();
            lblDetailTo.Text=r["DateTo"].ToString();
            lblDetailStatus.Text=r["TrainingStatus"].ToString();
            BindSessions(trainingID);
            pnlDetails.Visible=true;
        }

        private void BindSessions(string trainingID)
        {
            DataTable dt=objDB.GetDataTable("SELECT SM.SessionID,ISNULL(SM.SessionNo,'') SessionNo,ISNULL(SM.SessionName,'') SessionName,ISNULL(SM.SessionDate,'') SessionDate FROM ManagerMaster M INNER JOIN TrainingLocationMaster L ON M.TrainingLocationID=L.TrainingLocationID INNER JOIN TrainingDetails TD ON TD.TrainingLocation=L.TrainingLocation INNER JOIN SessionMaster SM ON SM.TrainingID=TD.TrainingID WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND TD.TrainingID=@TrainingID ORDER BY TRY_CONVERT(int,SM.SessionNo),SM.SessionID",new SqlParameter[]{new SqlParameter("@ManagerID",ManagerID),new SqlParameter("@TrainingID",trainingID)});
            ddlSession.DataSource=dt;
            ddlSession.DataTextField="SessionName";
            ddlSession.DataValueField="SessionID";
            ddlSession.DataBind();
            for(int i=0;i<ddlSession.Items.Count;i++)
            {
                DataRow r=dt.Rows[i];
                ddlSession.Items[i].Text=(string.IsNullOrWhiteSpace(r["SessionNo"].ToString()) ? "" : "Session "+r["SessionNo"]+" - ")+r["SessionName"]+" | "+r["SessionDate"];
            }
            if(ddlSession.Items.Count==0) ddlSession.Items.Insert(0,new ListItem("-- No Session --",""));
            else ddlSession.Items.Insert(0,new ListItem("-- Select Session --",""));
        }

        private bool ValidateSession()
        {
            if(string.IsNullOrWhiteSpace(Session["TrainingID"] as string) || string.IsNullOrWhiteSpace(ddlSession.SelectedValue))
            {
                lblMessage.ForeColor=System.Drawing.Color.Red;
                lblMessage.Text="Please select a session.";
                return false;
            }
            object v=objDB.ExecuteScalar("SELECT COUNT(*) FROM ManagerMaster M INNER JOIN TrainingLocationMaster L ON M.TrainingLocationID=L.TrainingLocationID INNER JOIN TrainingDetails TD ON TD.TrainingLocation=L.TrainingLocation INNER JOIN SessionMaster SM ON SM.TrainingID=TD.TrainingID WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND TD.TrainingID=@TrainingID AND SM.SessionID=@SessionID",new SqlParameter[]{new SqlParameter("@ManagerID",ManagerID),new SqlParameter("@TrainingID",Session["TrainingID"].ToString()),new SqlParameter("@SessionID",ddlSession.SelectedValue)});
            return v!=null && v!=DBNull.Value && Convert.ToInt32(v)>0;
        }

        private void Go(string url)
        {
            if(!ValidateSession()) return;
            Session["SessionID"]=ddlSession.SelectedValue;
            Session["TrainerID"]=ManagerID;
            Response.Redirect(url);
        }

        protected void btnMaterial_Click(object sender,EventArgs e){Go("~/Manager/TrainingMaterial.aspx");}
        protected void btnAttendance_Click(object sender,EventArgs e){Go("~/Manager/SessionAttendance.aspx");}
        protected void btnPreTest_Click(object sender,EventArgs e){Go("~/Manager/PreTrainingTest.aspx");}
        protected void btnPostTest_Click(object sender,EventArgs e){Go("~/Manager/PostTrainingTest.aspx");}
        protected void btnResult_Click(object sender,EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(Session["TrainingID"] as string)){lblMessage.Text="Please select a training.";return;}
            Response.Redirect("~/Manager/ExamResultReport.aspx");
        }
    }
}