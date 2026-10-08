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
            DataTable dt = objDB.GetDataTable("SELECT TD.TrainingID,TD.TrainingID + ' - ' + ISNULL(C.CourseName,'') AS TrainingName FROM TrainingDetails TD LEFT JOIN CourseMaster C ON TD.CourseID=C.CourseID ORDER BY TD.TrainingID DESC");
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

            DataTable dt=objDB.GetDataTable("SELECT D.CostingDetailID,H.CostHeadName,H.CostLevel,D.CalculationMode,H.UnitType,D.Quantity,D.AppliedRate,D.CalculatedAmount,D.FinalAmount FROM FinanceCostingDetail D INNER JOIN FinanceCostHeadMaster H ON D.CostHeadID=H.CostHeadID WHERE D.TrainingID=@TrainingID AND D.CostingLevel='Batch' ORDER BY H.CostHeadName",new SqlParameter[]{new SqlParameter("@TrainingID",trainingID)});
            gvCosting.DataSource=dt;
            gvCosting.DataBind();

            DataTable total=objDB.GetDataTable("SELECT ISNULL(SUM(FinalAmount),0) AS Total FROM FinanceCostingDetail WHERE TrainingID=@TrainingID",new SqlParameter[]{new SqlParameter("@TrainingID",trainingID)});
            lblTotal.Text="₹"+Convert.ToDecimal(total.Rows[0]["Total"]).ToString("N2");
        }

        private void EnsureAutomaticBatchEntries(string trainingID)
        {
            DataTable heads=objDB.GetDataTable("SELECT CostHeadID,CalculationMode,UnitType FROM FinanceCostHeadMaster WHERE Active='Y' AND CostLevel='Batch'");
            int trainees=FinanceCommon.GetTraineeCount(objDB,trainingID);
            int days=FinanceCommon.GetTrainingDays(objDB,trainingID);

            foreach(DataRow h in heads.Rows)
            {
                int headID=Convert.ToInt32(h["CostHeadID"]);
                string mode=Convert.ToString(h["CalculationMode"]);
                DataTable existing=objDB.GetDataTable("SELECT CostingDetailID FROM FinanceCostingDetail WHERE TrainingID=@TrainingID AND CostingLevel='Batch' AND CostHeadID=@CostHeadID",new SqlParameter[]{new SqlParameter("@TrainingID",trainingID),new SqlParameter("@CostHeadID",headID)});
                if(existing.Rows.Count>0) continue;

                int rateID;
                decimal rate=FinanceCommon.GetRate(objDB,headID,DateTime.Today,out rateID);
                decimal qty=GetAutomaticQuantity(trainingID,Convert.ToString(h["UnitType"]),trainees,days);
                if(mode=="Manual") qty=0;

                decimal calculated=qty*rate;

                objDB.ExecuteSql("INSERT INTO FinanceCostingDetail(TrainingID,CourseID,CostingLevel,CostHeadID,RateID,CalculationMode,UnitType,Quantity,AppliedRate,CalculatedAmount,Remarks,CreatedBy) SELECT @TrainingID,TD.CourseID,'Batch',@CostHeadID,@RateID,@Mode,@Unit,@Qty,@Rate,@Amount,'Initial automatic/manual costing',@CreatedBy FROM TrainingDetails TD WHERE TD.TrainingID=@TrainingID",
                new SqlParameter[]{new SqlParameter("@TrainingID",trainingID),new SqlParameter("@CostHeadID",headID),new SqlParameter("@RateID",rateID==0?(object)DBNull.Value:rateID),new SqlParameter("@Mode",mode),new SqlParameter("@Unit",Convert.ToString(h["UnitType"])),new SqlParameter("@Qty",qty),new SqlParameter("@Rate",rate),new SqlParameter("@Amount",calculated),new SqlParameter("@CreatedBy",Convert.ToString(Session["UserID"]))});
            }
        }

        private decimal GetAutomaticQuantity(string trainingID,string unit,int trainees,int days)
        {
            if(unit=="Trainee-Day") return trainees*days;
            if(unit=="Trainee") return trainees;
            if(unit=="Day") return days;
            if(unit=="Fixed") return 1;
            if(unit=="Trainee-Night") return GetTraineeHostelOccupancy(trainingID)*Math.Max(0,days-1);
            if(unit=="Trainer-Night") return GetTrainerHostelOccupancy(trainingID)*Math.Max(0,days-1);
            return 0;
        }

        private int GetTraineeHostelOccupancy(string trainingID)
        {
            try
            {
                DataTable dt=objDB.GetDataTable("SELECT COUNT(*) AS Cnt FROM HostelAllotment WHERE TrainingID=@TrainingID AND Status='Allotted'",new SqlParameter[]{new SqlParameter("@TrainingID",trainingID)});
                return dt.Rows.Count==0?0:Convert.ToInt32(dt.Rows[0]["Cnt"]);
            }
            catch{return 0;}
        }

        private int GetTrainerHostelOccupancy(string trainingID)
        {
            try
            {
                DataTable dt=objDB.GetDataTable("SELECT COUNT(DISTINCT HA.EmpID) AS Cnt FROM HostelAllotment HA INNER JOIN TrainerMaster T ON HA.EmpID=T.EmpID WHERE HA.TrainingID=@TrainingID AND HA.Status='Allotted' AND T.TrainerType='Internal'",new SqlParameter[]{new SqlParameter("@TrainingID",trainingID)});
                return dt.Rows.Count==0?0:Convert.ToInt32(dt.Rows[0]["Cnt"]);
            }
            catch{return 0;}
        }

        protected void gvCosting_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName != "SaveOverride") return;
            int rowIndex = Convert.ToInt32(e.CommandArgument);
            long detailID = Convert.ToInt64(gvCosting.DataKeys[rowIndex].Value);
            System.Web.UI.WebControls.TextBox txtOverride = (System.Web.UI.WebControls.TextBox)gvCosting.Rows[rowIndex].FindControl("txtOverride");
            decimal amount;
            object value = DBNull.Value;
            if (!string.IsNullOrWhiteSpace(txtOverride.Text))
            {
                if (!Decimal.TryParse(txtOverride.Text.Trim(), out amount) || amount < 0)
                {
                    lblMessage.Text = "Invalid override amount.";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    return;
                }
                value = amount;
            }
            objDB.ExecuteSql("UPDATE FinanceCostingDetail SET OverrideAmount=@OverrideAmount,ModifiedOn=GETDATE(),ModifiedBy=@ModifiedBy WHERE CostingDetailID=@ID", new SqlParameter[] { new SqlParameter("@OverrideAmount",value),new SqlParameter("@ModifiedBy",Convert.ToString(Session["UserID"])),new SqlParameter("@ID",detailID) });
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