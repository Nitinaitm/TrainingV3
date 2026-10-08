using System;
using System.Data;
using System.Data.SqlClient;

namespace Training.Admin
{
    public partial class FinanceSessionCosting : System.Web.UI.Page
    {
        private readonly clsDataAccess objDB = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindTraining();
            }
        }

        private void BindTraining()
        {
            DataTable dt=objDB.GetDataTable("SELECT TD.TrainingID,TD.TrainingID + ' - ' + ISNULL(C.CourseName,'') AS TrainingName FROM TrainingDetails TD LEFT JOIN CourseMaster C ON TD.CourseID=C.CourseID ORDER BY TD.TrainingID DESC");
            ddlTraining.DataSource=dt;
            ddlTraining.DataTextField="TrainingName";
            ddlTraining.DataValueField="TrainingID";
            ddlTraining.DataBind();
            ddlTraining.Items.Insert(0,new System.Web.UI.WebControls.ListItem("Select Training / Batch",""));
        }

        protected void ddlTraining_SelectedIndexChanged(object sender,EventArgs e)
        {
            BindSessions();
        }

        private void BindSessions()
        {
            ddlSession.Items.Clear();
            if(ddlTraining.SelectedValue=="") return;
            DataTable dt=FinanceCommon.GetTrainingSessions(objDB,ddlTraining.SelectedValue);
            ddlSession.DataSource=dt;
            ddlSession.DataTextField="SessionName";
            ddlSession.DataValueField="SessionID";
            ddlSession.DataBind();
            ddlSession.Items.Insert(0,new System.Web.UI.WebControls.ListItem("Select Session",""));
            gvCosting.DataSource=null;
            gvCosting.DataBind();
            lblTotal.Text="₹0.00";
        }

        protected void ddlSession_SelectedIndexChanged(object sender,EventArgs e)
        {
            BindSelectedSession();
        }

        private void BindSelectedSession()
        {
            if(ddlTraining.SelectedValue=="" || ddlSession.SelectedValue=="") return;
            string trainingID=ddlTraining.SelectedValue;
            string sessionID=ddlSession.SelectedValue;
            DataTable session=objDB.GetDataTable("SELECT TOP 1 SessionDate FROM SessionMaster WHERE TrainingID=@TrainingID AND SessionID=@SessionID",new SqlParameter[]{new SqlParameter("@TrainingID",trainingID),new SqlParameter("@SessionID",sessionID)});
            DateTime asOn=DateTime.Today;
            if(session.Rows.Count>0) DateTime.TryParse(Convert.ToString(session.Rows[0]["SessionDate"]),out asOn);
            DataTable heads=objDB.GetDataTable("SELECT CostHeadID,CalculationMode,UnitType FROM FinanceCostHeadMaster WHERE Active='Y' AND CostLevel='Session'");
            int trainerCount=FinanceCommon.GetSessionTrainerCount(objDB,trainingID,sessionID);
            foreach(DataRow h in heads.Rows)
            {
                int headID=Convert.ToInt32(h["CostHeadID"]);
                DataTable exists=objDB.GetDataTable("SELECT CostingDetailID FROM FinanceCostingDetail WHERE TrainingID=@TrainingID AND SessionID=@SessionID AND CostingLevel='Session' AND CostHeadID=@CostHeadID",new SqlParameter[]{new SqlParameter("@TrainingID",trainingID),new SqlParameter("@SessionID",sessionID),new SqlParameter("@CostHeadID",headID)});
                if(exists.Rows.Count>0) continue;
                int rateID;
                decimal rate=FinanceCommon.GetRate(objDB,headID,asOn,out rateID);
                string mode=Convert.ToString(h["CalculationMode"]);
                string unit=Convert.ToString(h["UnitType"]);
                decimal qty=0;
                if(mode!="Manual" && unit=="Trainer-Day") qty=trainerCount;
                else if(mode!="Manual" && unit=="Day") qty=1;
                else if(mode!="Manual" && unit=="Trainer") qty=trainerCount;
                decimal calculated=qty*rate;
                objDB.ExecuteSql("INSERT INTO FinanceCostingDetail(TrainingID,CourseID,SessionID,CostingLevel,CostHeadID,RateID,CalculationMode,UnitType,Quantity,AppliedRate,CalculatedAmount,Remarks,CreatedBy) SELECT @TrainingID,TD.CourseID,@SessionID,'Session',@CostHeadID,@RateID,@Mode,@Unit,@Qty,@Rate,@Amount,'Initial session calculation',@CreatedBy FROM TrainingDetails TD WHERE TD.TrainingID=@TrainingID",new SqlParameter[]{new SqlParameter("@TrainingID",trainingID),new SqlParameter("@SessionID",sessionID),new SqlParameter("@CostHeadID",headID),new SqlParameter("@RateID",rateID==0?(object)DBNull.Value:rateID),new SqlParameter("@Mode",mode),new SqlParameter("@Unit",unit),new SqlParameter("@Qty",qty),new SqlParameter("@Rate",rate),new SqlParameter("@Amount",calculated),new SqlParameter("@CreatedBy",Convert.ToString(Session["UserID"]))});
            }
            DataTable dt=objDB.GetDataTable("SELECT D.CostingDetailID,H.CostHeadName,D.UnitType,D.Quantity,D.AppliedRate,D.CalculatedAmount,D.OverrideAmount,D.FinalAmount FROM FinanceCostingDetail D INNER JOIN FinanceCostHeadMaster H ON D.CostHeadID=H.CostHeadID WHERE D.TrainingID=@TrainingID AND D.SessionID=@SessionID AND D.CostingLevel='Session' ORDER BY H.CostHeadName",new SqlParameter[]{new SqlParameter("@TrainingID",trainingID),new SqlParameter("@SessionID",sessionID)});
            gvCosting.DataSource=dt;
            gvCosting.DataBind();
            DataTable total=objDB.GetDataTable("SELECT ISNULL(SUM(FinalAmount),0) AS Total FROM FinanceCostingDetail WHERE TrainingID=@TrainingID AND SessionID=@SessionID AND CostingLevel='Session'",new SqlParameter[]{new SqlParameter("@TrainingID",trainingID),new SqlParameter("@SessionID",sessionID)});
            lblTotal.Text="₹"+Convert.ToDecimal(total.Rows[0]["Total"]).ToString("N2");
        }

        protected void gvCosting_RowCommand(object sender,System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if(e.CommandName!="SaveOverride") return;
            int rowIndex=Convert.ToInt32(e.CommandArgument);
            long detailID=Convert.ToInt64(gvCosting.DataKeys[rowIndex].Value);
            System.Web.UI.WebControls.TextBox txtOverride=(System.Web.UI.WebControls.TextBox)gvCosting.Rows[rowIndex].FindControl("txtOverride");
            decimal amount;
            object value=DBNull.Value;
            if(!string.IsNullOrWhiteSpace(txtOverride.Text))
            {
                if(!Decimal.TryParse(txtOverride.Text.Trim(),out amount) || amount<0){lblMessage.Text="Invalid override amount.";lblMessage.ForeColor=System.Drawing.Color.Red;return;}
                value=amount;
            }
            objDB.ExecuteSql("UPDATE FinanceCostingDetail SET OverrideAmount=@OverrideAmount,ModifiedOn=GETDATE(),ModifiedBy=@ModifiedBy WHERE CostingDetailID=@ID",new SqlParameter[]{new SqlParameter("@OverrideAmount",value),new SqlParameter("@ModifiedBy",Convert.ToString(Session["UserID"])),new SqlParameter("@ID",detailID)});
            BindSelectedSession();
        }
    }
}