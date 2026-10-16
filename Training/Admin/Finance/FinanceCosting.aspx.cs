using System;
using System.Data;
using System.Data.SqlClient;

namespace Training.Admin
{
    public partial class FinanceCosting : System.Web.UI.Page
    {
        private readonly clsDataAccess objDB = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindTraining();
                BindCourseSummary();
                BindResidentialSetting();
            }
        }

        private void BindTraining()
        {
            DataTable dt = objDB.GetDataTable("SELECT TD.TrainingID,TD.TrainingID + ' - ' + ISNULL(C.CourseName,'') + ' - ' + ISNULL(TD.Batch,'') + ' - ' + ISNULL(TD.TrainingLocation,'') AS TrainingName FROM TrainingDetails TD LEFT JOIN CourseMaster C ON TD.CourseID=C.CourseID ORDER BY TD.TrainingID DESC");
            ddlTraining.DataSource = dt;
            ddlTraining.DataTextField = "TrainingName";
            ddlTraining.DataValueField = "TrainingID";
            ddlTraining.DataBind();
            ddlTraining.Items.Insert(0,new System.Web.UI.WebControls.ListItem("Select Training / Batch",""));
        }

        protected void ddlTraining_SelectedIndexChanged(object sender,EventArgs e)
        {
            BindResidentialSetting();
            BindSelectedTraining();
        }

        private void BindResidentialSetting()
        {
            ddlResidentialType.Items.Clear();
            ddlResidentialType.Items.Add(new System.Web.UI.WebControls.ListItem("Select Batch Type", ""));
            ddlResidentialType.Items.Add(new System.Web.UI.WebControls.ListItem("Residential", "Residential"));
            ddlResidentialType.Items.Add(new System.Web.UI.WebControls.ListItem("Non-Residential", "Non-Residential"));
            if (ddlTraining.SelectedValue == "") return;
            DataTable dt = objDB.GetDataTable("SELECT ResidentialType FROM FinanceTrainingSettings WHERE TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@TrainingID", ddlTraining.SelectedValue) });
            if (dt.Rows.Count > 0 && ddlResidentialType.Items.FindByValue(Convert.ToString(dt.Rows[0]["ResidentialType"])) != null)
            {
                ddlResidentialType.SelectedValue = Convert.ToString(dt.Rows[0]["ResidentialType"]);
            }
        }

        protected void btnSaveResidentialType_Click(object sender, EventArgs e)
        {
            if (ddlTraining.SelectedValue == "" || ddlResidentialType.SelectedValue == "")
            {
                lblMessage.Text = "Select a training batch and its residential type first.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }
            int result = objDB.ExecuteSql("IF EXISTS (SELECT 1 FROM FinanceTrainingSettings WHERE TrainingID=@TrainingID) UPDATE FinanceTrainingSettings SET ResidentialType=@ResidentialType,ModifiedOn=GETDATE(),ModifiedBy=@ModifiedBy WHERE TrainingID=@TrainingID ELSE INSERT INTO FinanceTrainingSettings(TrainingID,ResidentialType,ModifiedOn,ModifiedBy) VALUES(@TrainingID,@ResidentialType,GETDATE(),@ModifiedBy)", new SqlParameter[] { new SqlParameter("@TrainingID", ddlTraining.SelectedValue), new SqlParameter("@ResidentialType", ddlResidentialType.SelectedValue), new SqlParameter("@ModifiedBy", Convert.ToString(Session["UserID"])) });
            if (result >= 0)
            {
                RecalculateCosts(ddlTraining.SelectedValue, "Batch type changed to " + ddlResidentialType.SelectedValue);
                BindSelectedTraining();
                BindCourseSummary();
                lblMessage.Text = "Batch type saved and non-overridden costing recalculated. Previous applied rates are archived.";
                lblMessage.ForeColor = System.Drawing.Color.Green;
            }
        }

        protected void btnRecalculateCosting_Click(object sender, EventArgs e)
        {
            if (ddlTraining.SelectedValue == "")
            {
                lblMessage.Text = "Select a training batch first.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }
            string reason = txtRecalculationReason.Text.Trim();
            if (reason == "")
            {
                lblMessage.Text = "Enter a reason before recalculating costing.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }
            RecalculateCosts(ddlTraining.SelectedValue, reason);
            BindSelectedTraining();
            BindCourseSummary();
            lblMessage.Text = "Costing recalculated. Previous applied-rate snapshots are available in history.";
            lblMessage.ForeColor = System.Drawing.Color.Green;
        }

        private void RecalculateCosts(string trainingID, string reason)
        {
            string sql = "INSERT INTO FinanceCostingCalculationHistory(CostingDetailID,TrainingID,CourseID,SessionID,CostingLevel,CostHeadID,RateID,CalculationMode,UnitType,Quantity,AppliedRate,CalculatedAmount,OverrideAmount,FinalAmount,ArchiveReason,ArchivedOn,ArchivedBy) SELECT CostingDetailID,TrainingID,CourseID,SessionID,CostingLevel,CostHeadID,RateID,CalculationMode,UnitType,Quantity,AppliedRate,CalculatedAmount,OverrideAmount,FinalAmount,@Reason,GETDATE(),@UserID FROM FinanceCostingDetail WHERE TrainingID=@TrainingID AND OverrideAmount IS NULL; DELETE FROM FinanceCostingDetail WHERE TrainingID=@TrainingID AND OverrideAmount IS NULL;";
            objDB.ExecuteSql(sql, new SqlParameter[] { new SqlParameter("@Reason", reason), new SqlParameter("@UserID", Convert.ToString(Session["UserID"])), new SqlParameter("@TrainingID", trainingID) });
            FinanceCommon.EnsureCostingForTraining(objDB, trainingID, Convert.ToString(Session["UserID"]));
        }

        private void BindSelectedTraining()
        {
            if (ddlTraining.SelectedValue=="")
            {
                gvCosting.DataSource=null;
                gvCosting.DataBind();
                lblTrainees.Text="0";
                lblDays.Text="0";
                lblTotal.Text="Rs. 0.00";
                return;
            }

            string trainingID=ddlTraining.SelectedValue;
            int trainees=FinanceCommon.GetTraineeCount(objDB,trainingID);
            int days=FinanceCommon.GetTrainingDays(objDB,trainingID);
            lblTrainees.Text=trainees.ToString();
            lblDays.Text=days.ToString();

            EnsureAutomaticBatchEntries(trainingID);

            DataTable dt=objDB.GetDataTable("SELECT D.CostingDetailID,H.CostHeadName,H.CostLevel,D.CalculationMode,H.UnitType,D.Quantity,D.AppliedRate,D.CalculatedAmount,D.OverrideAmount,D.OverrideReason,D.FinalAmount FROM FinanceCostingDetail D INNER JOIN FinanceCostHeadMaster H ON D.CostHeadID=H.CostHeadID WHERE (D.TrainingID=@TrainingID AND D.CostingLevel='Batch') OR (D.CourseID=(SELECT CourseID FROM TrainingDetails WHERE TrainingID=@TrainingID) AND D.CostingLevel='Course') ORDER BY D.CostingLevel,H.CostHeadName",new SqlParameter[]{new SqlParameter("@TrainingID",trainingID)});
            gvCosting.DataSource=dt;
            gvCosting.DataBind();

            DataTable total=objDB.GetDataTable("SELECT ISNULL((SELECT SUM(FinalAmount) FROM FinanceCostingDetail WHERE TrainingID=@TrainingID AND CostingLevel IN ('Batch','Session')),0)+ISNULL((SELECT SUM(FinalAmount) FROM FinanceCostingDetail WHERE CourseID=(SELECT CourseID FROM TrainingDetails WHERE TrainingID=@TrainingID) AND CostingLevel='Course'),0)/NULLIF((SELECT COUNT(*) FROM TrainingDetails WHERE CourseID=(SELECT CourseID FROM TrainingDetails WHERE TrainingID=@TrainingID)),0) AS Total",new SqlParameter[]{new SqlParameter("@TrainingID",trainingID)});
            lblTotal.Text="Rs. "+Convert.ToDecimal(total.Rows[0]["Total"]).ToString("N2");
        }

        private void EnsureAutomaticBatchEntries(string trainingID)
        {
            FinanceCommon.EnsureCostingForTraining(objDB,trainingID,Convert.ToString(Session["UserID"]));
        }

        protected void gvCosting_RowCommand(object sender,System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if(e.CommandName!="SaveOverride") return;
            int rowIndex=Convert.ToInt32(e.CommandArgument);
            long detailID=Convert.ToInt64(gvCosting.DataKeys[rowIndex].Value);
            DataTable policy=objDB.GetDataTable("SELECT H.ManualAllowed,H.OverrideAllowed,D.CalculationMode FROM FinanceCostingDetail D INNER JOIN FinanceCostHeadMaster H ON H.CostHeadID=D.CostHeadID WHERE D.CostingDetailID=@ID",new SqlParameter[]{new SqlParameter("@ID",detailID)});
            if(policy.Rows.Count==0){lblMessage.Text="Costing row not found.";lblMessage.ForeColor=System.Drawing.Color.Red;return;}
            string mode=Convert.ToString(policy.Rows[0]["CalculationMode"]);
            string manualAllowed=Convert.ToString(policy.Rows[0]["ManualAllowed"]);
            string overrideAllowed=Convert.ToString(policy.Rows[0]["OverrideAllowed"]);
            if(!((mode=="Manual" && manualAllowed=="Y") || (mode=="Automatic + Override" && overrideAllowed=="Y"))){lblMessage.Text="This cost head does not permit manual entry/override in its current calculation mode.";lblMessage.ForeColor=System.Drawing.Color.Red;return;}
            System.Web.UI.WebControls.TextBox txtOverride=(System.Web.UI.WebControls.TextBox)gvCosting.Rows[rowIndex].FindControl("txtOverride");
            decimal amount;
            object value=DBNull.Value;
            if(!string.IsNullOrWhiteSpace(txtOverride.Text))
            {
                if(!Decimal.TryParse(txtOverride.Text.Trim(),out amount) || amount<0)
                {
                    lblMessage.Text="Invalid override amount.";
                    lblMessage.ForeColor=System.Drawing.Color.Red;
                    return;
                }
                value=amount;
            }
            System.Web.UI.WebControls.TextBox txtReason=(System.Web.UI.WebControls.TextBox)gvCosting.Rows[rowIndex].FindControl("txtOverrideReason");
            string reason=txtReason==null?"":txtReason.Text.Trim();
            if (reason == "")
            {
                lblMessage.Text="Please enter a reason for the manual override.";
                lblMessage.ForeColor=System.Drawing.Color.Red;
                return;
            }
            string sql="DECLARE @PreviousOverrideAmount DECIMAL(18,2); SELECT @PreviousOverrideAmount=OverrideAmount FROM FinanceCostingDetail WHERE CostingDetailID=@ID; INSERT INTO FinanceCostingOverrideAudit(CostingDetailID,PreviousOverrideAmount,NewOverrideAmount,OverrideReason,ChangedBy,ChangedOn) VALUES(@ID,@PreviousOverrideAmount,@OverrideAmount,@Reason,@ModifiedBy,GETDATE()); UPDATE FinanceCostingDetail SET OverrideAmount=@OverrideAmount,OverrideReason=@Reason,ModifiedOn=GETDATE(),ModifiedBy=@ModifiedBy WHERE CostingDetailID=@ID;";
            objDB.ExecuteSql(sql,new SqlParameter[]{new SqlParameter("@OverrideAmount",value),new SqlParameter("@Reason",reason),new SqlParameter("@ModifiedBy",Convert.ToString(Session["UserID"])),new SqlParameter("@ID",detailID)});
            BindSelectedTraining();
        }

        private void BindCourseSummary()
        {
            DataTable dt=objDB.GetDataTable("SELECT ISNULL(C.CourseName,'') AS CourseName,COUNT(DISTINCT TD.TrainingID) AS BatchCount,ISNULL(SUM(D.FinalAmount),0) AS CourseCost,CASE WHEN COUNT(DISTINCT TD.TrainingID)=0 THEN 0 ELSE ISNULL(SUM(D.FinalAmount),0)/COUNT(DISTINCT TD.TrainingID) END AS AverageBatchCost FROM TrainingDetails TD LEFT JOIN CourseMaster C ON TD.CourseID=C.CourseID LEFT JOIN FinanceCostingDetail D ON TD.TrainingID=D.TrainingID GROUP BY C.CourseName ORDER BY C.CourseName");
            gvCourse.DataSource=dt;
            gvCourse.DataBind();
        }
    }
}