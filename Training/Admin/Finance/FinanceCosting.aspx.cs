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
            BindSelectedTraining();
        }

        private void BindSelectedTraining()
        {
            if (ddlTraining.SelectedValue=="")
            {
                gvCosting.DataSource=null;
                gvCosting.DataBind();
                lblTrainees.Text="0";
                lblDays.Text="0";
                lblTotal.Text="₹0.00";
                return;
            }

            string trainingID=ddlTraining.SelectedValue;
            int trainees=FinanceCommon.GetTraineeCount(objDB,trainingID);
            int days=FinanceCommon.GetTrainingDays(objDB,trainingID);
            lblTrainees.Text=trainees.ToString();
            lblDays.Text=days.ToString();

            EnsureAutomaticBatchEntries(trainingID);

            DataTable dt=objDB.GetDataTable("SELECT D.CostingDetailID,H.CostHeadName,H.CostLevel,D.CalculationMode,H.UnitType,D.Quantity,D.AppliedRate,D.CalculatedAmount,D.OverrideAmount,D.FinalAmount FROM FinanceCostingDetail D INNER JOIN FinanceCostHeadMaster H ON D.CostHeadID=H.CostHeadID WHERE D.TrainingID=@TrainingID AND D.CostingLevel='Batch' ORDER BY H.CostHeadName",new SqlParameter[]{new SqlParameter("@TrainingID",trainingID)});
            gvCosting.DataSource=dt;
            gvCosting.DataBind();

            DataTable total=objDB.GetDataTable("SELECT ISNULL(SUM(FinalAmount),0) AS Total FROM FinanceCostingDetail WHERE TrainingID=@TrainingID",new SqlParameter[]{new SqlParameter("@TrainingID",trainingID)});
            lblTotal.Text="₹"+Convert.ToDecimal(total.Rows[0]["Total"]).ToString("N2");
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
            objDB.ExecuteSql("UPDATE FinanceCostingDetail SET OverrideAmount=@OverrideAmount,ModifiedOn=GETDATE(),ModifiedBy=@ModifiedBy WHERE CostingDetailID=@ID",new SqlParameter[]{new SqlParameter("@OverrideAmount",value),new SqlParameter("@ModifiedBy",Convert.ToString(Session["UserID"])),new SqlParameter("@ID",detailID)});
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