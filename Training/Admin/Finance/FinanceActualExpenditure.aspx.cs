using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class FinanceActualExpenditure : System.Web.UI.Page
    {
        private readonly clsDataAccess objDB = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindTraining();
                txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
                BindCostHeads();
                txtHostelCheckIn.Text = DateTime.Today.ToString("yyyy-MM-dd");
                txtHostelCheckOut.Text = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
                ClearSelection();
            }
        }

        private void BindTraining()
        {
            DataTable dt = objDB.GetDataTable("SELECT TD.TrainingID,TD.TrainingID+' - '+ISNULL(C.CourseName,'')+' - '+ISNULL(TD.Batch,'')+' - '+ISNULL(TD.TrainingLocation,'') AS TrainingName FROM TrainingDetails TD LEFT JOIN CourseMaster C ON TD.CourseID=C.CourseID ORDER BY TD.TrainingID DESC");
            ddlTraining.DataSource=dt; ddlTraining.DataTextField="TrainingName"; ddlTraining.DataValueField="TrainingID"; ddlTraining.DataBind();
            ddlTraining.Items.Insert(0,new ListItem("Select Training / Batch",""));
        }

        private void BindCostHeads()
        {
            DataTable dt=objDB.GetDataTable("SELECT CostHeadID,CostHeadName FROM FinanceCostHeadMaster WHERE Active='Y' ORDER BY CostHeadName");
            ddlCostHead.DataSource=dt; ddlCostHead.DataTextField="CostHeadName"; ddlCostHead.DataValueField="CostHeadID"; ddlCostHead.DataBind();
            ddlCostHead.Items.Insert(0,new ListItem("Select Cost Head",""));
        }

        protected void ddlTraining_SelectedIndexChanged(object sender,EventArgs e)
        {
            if(ddlTraining.SelectedValue!="") FinanceCommon.EnsureCostingForTraining(objDB,ddlTraining.SelectedValue,Convert.ToString(Session["UserID"]));
            BindSessions();
            BindExpenditure();
            BindHostelUsage();
            BindSummary();
        }

        private void BindSessions()
        {
            ddlSession.Items.Clear();
            ddlSession.Items.Insert(0,new ListItem("Batch Level / No Session",""));
            if(ddlTraining.SelectedValue=="") return;
            DataTable dt=FinanceCommon.GetTrainingSessions(objDB,ddlTraining.SelectedValue);
            ddlSession.DataSource=dt; ddlSession.DataTextField="SessionName"; ddlSession.DataValueField="SessionID"; ddlSession.DataBind();
            ddlSession.Items.Insert(0,new ListItem("Batch Level / No Session",""));
        }

        protected void btnSave_Click(object sender,EventArgs e)
        {
            decimal amount; DateTime date;
            string expenditureEmpID = txtExpenditureEmpID.Text.Trim();
            string expenditureTrainerID = txtExpenditureTrainerID.Text.Trim();
            if(ddlTraining.SelectedValue=="" || ddlCostHead.SelectedValue=="" || !Decimal.TryParse(txtAmount.Text.Trim(),out amount) || amount<=0 || !DateTime.TryParse(txtDate.Text,out date) || (expenditureEmpID!="" && expenditureTrainerID!=""))
            {
                ShowMessage("Training, Cost Head, valid Date and positive Amount are required. Tag an expense to either an EmpID or TrainerID, not both.",Color.Red); return;
            }
            DataTable personCheck = null;
            if (expenditureEmpID != "")
            {
                personCheck = objDB.GetDataTable("SELECT TOP 1 EmpID FROM TrainingAssignment WHERE TrainingID=@TrainingID AND EmpID=@EmpID AND ISNULL(AssignmentStatus,'Assigned')<>'Cancelled' AND ISNULL(Cancelled,0)=0", new SqlParameter[] { new SqlParameter("@TrainingID", ddlTraining.SelectedValue), new SqlParameter("@EmpID", expenditureEmpID) });
            }
            else if (expenditureTrainerID != "")
            {
                personCheck = objDB.GetDataTable("SELECT TOP 1 TrainerID FROM TrainingTrainerMapping WHERE TrainingID=@TrainingID AND TrainerID=@TrainerID UNION SELECT TOP 1 TrainerID FROM SessionMaster WHERE TrainingID=@TrainingID AND TrainerID=@TrainerID", new SqlParameter[] { new SqlParameter("@TrainingID", ddlTraining.SelectedValue), new SqlParameter("@TrainerID", expenditureTrainerID) });
            }
            if (personCheck != null && personCheck.Rows.Count == 0)
            {
                ShowMessage("The tagged trainee/trainer is not assigned to this batch.", Color.Red);
                return;
            }
            int result=objDB.ExecuteSql("INSERT INTO FinanceActualExpenditure(TrainingID,CourseID,SessionID,CostHeadID,EmpID,TrainerID,ExpenditureDate,Amount,VendorName,BillReference,Remarks,CreatedBy) SELECT @TrainingID,TD.CourseID,@SessionID,@CostHeadID,NULLIF(@EmpID,''),NULLIF(@TrainerID,''),@Date,@Amount,@Vendor,@Bill,@Remarks,@CreatedBy FROM TrainingDetails TD WHERE TD.TrainingID=@TrainingID",
                new SqlParameter[]{new SqlParameter("@TrainingID",ddlTraining.SelectedValue),new SqlParameter("@SessionID",ddlSession.SelectedValue==""?(object)DBNull.Value:ddlSession.SelectedValue),new SqlParameter("@CostHeadID",Convert.ToInt32(ddlCostHead.SelectedValue)),new SqlParameter("@EmpID",expenditureEmpID),new SqlParameter("@TrainerID",expenditureTrainerID),new SqlParameter("@Date",date.Date),new SqlParameter("@Amount",amount),new SqlParameter("@Vendor",txtVendor.Text.Trim()),new SqlParameter("@Bill",txtBill.Text.Trim()),new SqlParameter("@Remarks",txtRemarks.Text.Trim()),new SqlParameter("@CreatedBy",Convert.ToString(Session["UserID"]))});
            if(result>0){ShowMessage("Actual expenditure saved successfully.",Color.Green);ClearEntry();BindExpenditure();BindSummary();}
        }

        protected void gvExpenditure_RowCommand(object sender,GridViewCommandEventArgs e)
        {
            if(e.CommandName!="DeleteExpenditure") return;
            int row=Convert.ToInt32(e.CommandArgument); long id=Convert.ToInt64(gvExpenditure.DataKeys[row].Value);
            objDB.ExecuteSql("DELETE FROM FinanceActualExpenditure WHERE ExpenditureID=@ID",new SqlParameter[]{new SqlParameter("@ID",id)});
            BindExpenditure(); BindHostelUsage(); BindSummary();
        }

        private void BindExpenditure()
        {
            if(ddlTraining.SelectedValue==""){gvExpenditure.DataSource=null;gvExpenditure.DataBind();return;}
            DataTable dt=objDB.GetDataTable("SELECT X.ExpenditureID,X.ExpenditureDate,H.CostHeadName,ISNULL(SM.SessionName,'Batch Level') SessionName,X.EmpID,X.TrainerID,X.VendorName,X.BillReference,X.Amount,X.Remarks FROM FinanceActualExpenditure X LEFT JOIN FinanceCostHeadMaster H ON X.CostHeadID=H.CostHeadID LEFT JOIN SessionMaster SM ON X.SessionID=SM.SessionID WHERE X.TrainingID=@TrainingID ORDER BY X.ExpenditureDate DESC,X.ExpenditureID DESC",new SqlParameter[]{new SqlParameter("@TrainingID",ddlTraining.SelectedValue)});
            gvExpenditure.DataSource=dt;gvExpenditure.DataBind();
        }

        private void BindHostelUsage()
        {
            if (ddlTraining.SelectedValue == "") { gvHostelUsage.DataSource = null; gvHostelUsage.DataBind(); return; }
            DataTable dt = objDB.GetDataTable("SELECT HostelUsageID,EmpID,TrainerID,CheckInDate,CheckOutDate,ChargeableNights,FoodChargeableDays,UsageStatus,Remarks FROM FinanceHostelUsage WHERE TrainingID=@TrainingID ORDER BY CheckInDate DESC,HostelUsageID DESC", new SqlParameter[] { new SqlParameter("@TrainingID", ddlTraining.SelectedValue) });
            gvHostelUsage.DataSource = dt;
            gvHostelUsage.DataBind();
        }

        protected void btnSaveHostelUsage_Click(object sender, EventArgs e)
        {
            string empID = txtHostelEmpID.Text.Trim();
            string trainerID = txtHostelTrainerID.Text.Trim();
            DateTime checkIn;
            DateTime checkOut;
            decimal nights;
            decimal foodDays;
            if (ddlTraining.SelectedValue == "" || ((empID == "") == (trainerID == "")) || !DateTime.TryParse(txtHostelCheckIn.Text, out checkIn) || !DateTime.TryParse(txtHostelCheckOut.Text, out checkOut) || checkOut.Date < checkIn.Date)
            {
                ShowMessage("Select a batch, enter either EmpID or TrainerID (not both), and provide valid check-in/check-out dates.", Color.Red);
                return;
            }
            if (txtHostelNights.Text.Trim() == "")
            {
                nights = (decimal)(checkOut.Date - checkIn.Date).Days;
            }
            else if (!Decimal.TryParse(txtHostelNights.Text.Trim(), out nights) || nights < 0)
            {
                ShowMessage("Chargeable nights must be a valid non-negative number.", Color.Red);
                return;
            }
            if (nights == 0)
            {
                ShowMessage("Chargeable nights must be greater than zero for actual hostel usage.", Color.Red);
                return;
            }
            if (txtFoodChargeableDays.Text.Trim() == "")
            {
                foodDays = (decimal)(checkOut.Date - checkIn.Date).Days + 1;
            }
            else if (!Decimal.TryParse(txtFoodChargeableDays.Text.Trim(), out foodDays) || foodDays < 0)
            {
                ShowMessage("Food chargeable days must be a valid non-negative number.", Color.Red);
                return;
            }
            DataTable valid;
            if (empID != "")
            {
                valid = objDB.GetDataTable("SELECT TOP 1 EmpID FROM TrainingAssignment WHERE TrainingID=@TrainingID AND EmpID=@EmpID AND ISNULL(AssignmentStatus,'Assigned')<>'Cancelled' AND ISNULL(Cancelled,0)=0", new SqlParameter[] { new SqlParameter("@TrainingID", ddlTraining.SelectedValue), new SqlParameter("@EmpID", empID) });
            }
            else
            {
                valid = objDB.GetDataTable("SELECT TOP 1 TrainerID FROM TrainingTrainerMapping WHERE TrainingID=@TrainingID AND TrainerID=@TrainerID UNION SELECT TOP 1 TrainerID FROM SessionMaster WHERE TrainingID=@TrainingID AND TrainerID=@TrainerID", new SqlParameter[] { new SqlParameter("@TrainingID", ddlTraining.SelectedValue), new SqlParameter("@TrainerID", trainerID) });
            }
            if (valid.Rows.Count == 0)
            {
                ShowMessage("The selected trainee/trainer is not assigned to this batch.", Color.Red);
                return;
            }
            DataTable overlap = objDB.GetDataTable("SELECT TOP 1 HostelUsageID FROM FinanceHostelUsage WHERE TrainingID=@TrainingID AND UsageStatus='Used' AND ((@EmpID<>'' AND EmpID=@EmpID) OR (@TrainerID<>'' AND TrainerID=@TrainerID)) AND CheckInDate<@CheckOut AND ISNULL(CheckOutDate,CONVERT(date,'99991231'))>@CheckIn", new SqlParameter[] { new SqlParameter("@TrainingID", ddlTraining.SelectedValue), new SqlParameter("@EmpID", empID), new SqlParameter("@TrainerID", trainerID), new SqlParameter("@CheckIn", checkIn.Date), new SqlParameter("@CheckOut", checkOut.Date) });
            if (overlap.Rows.Count > 0)
            {
                ShowMessage("This trainee/trainer already has overlapping hostel usage dates in this batch.", Color.Red);
                return;
            }
            int result = objDB.ExecuteSql("INSERT INTO FinanceHostelUsage(TrainingID,EmpID,TrainerID,CheckInDate,CheckOutDate,ChargeableNights,FoodChargeableDays,UsageStatus,Remarks,CreatedOn,CreatedBy) VALUES(@TrainingID,NULLIF(@EmpID,''),NULLIF(@TrainerID,''),@CheckIn,@CheckOut,@Nights,@FoodDays,'Used',@Remarks,GETDATE(),@CreatedBy)", new SqlParameter[] { new SqlParameter("@TrainingID", ddlTraining.SelectedValue), new SqlParameter("@EmpID", empID), new SqlParameter("@TrainerID", trainerID), new SqlParameter("@CheckIn", checkIn.Date), new SqlParameter("@CheckOut", checkOut.Date), new SqlParameter("@Nights", nights), new SqlParameter("@FoodDays", foodDays), new SqlParameter("@Remarks", txtHostelUsageRemarks.Text.Trim()), new SqlParameter("@CreatedBy", Convert.ToString(Session["UserID"])) });
            if (result > 0)
            {
                ShowMessage("Actual hostel usage saved. Recalculate costing to apply the new usage quantity.", Color.Green);
                txtHostelEmpID.Text = "";
                txtHostelTrainerID.Text = "";
                txtHostelNights.Text = "";
                txtFoodChargeableDays.Text = "";
                txtHostelUsageRemarks.Text = "";
                BindHostelUsage();
            }
        }

        protected void gvHostelUsage_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "CancelUsage") return;
            int row = Convert.ToInt32(e.CommandArgument);
            long id = Convert.ToInt64(gvHostelUsage.DataKeys[row].Value);
            objDB.ExecuteSql("UPDATE FinanceHostelUsage SET UsageStatus='Cancelled',ModifiedOn=GETDATE(),ModifiedBy=@UserID WHERE HostelUsageID=@ID", new SqlParameter[] { new SqlParameter("@UserID", Convert.ToString(Session["UserID"])), new SqlParameter("@ID", id) });
            BindHostelUsage();
        }

        private void BindSummary()
        {
            if(ddlTraining.SelectedValue==""){lblFinalCost.Text="Rs. 0.00";lblActual.Text="Rs. 0.00";lblBalance.Text="Rs. 0.00";return;}
            DataTable dt=objDB.GetDataTable("SELECT ISNULL((SELECT SUM(FinalAmount) FROM FinanceCostingDetail WHERE TrainingID=@TrainingID AND CostingLevel IN ('Batch','Session')),0)+ISNULL((SELECT SUM(FinalAmount) FROM FinanceCostingDetail WHERE CourseID=(SELECT CourseID FROM TrainingDetails WHERE TrainingID=@TrainingID) AND CostingLevel='Course'),0)/NULLIF((SELECT COUNT(*) FROM TrainingDetails WHERE CourseID=(SELECT CourseID FROM TrainingDetails WHERE TrainingID=@TrainingID)),0) AS FinalCost",new SqlParameter[]{new SqlParameter("@TrainingID",ddlTraining.SelectedValue)});
            decimal finalCost=Convert.ToDecimal(dt.Rows[0]["FinalCost"]);
            dt=objDB.GetDataTable("SELECT ISNULL(SUM(Amount),0) ActualCost FROM FinanceActualExpenditure WHERE TrainingID=@TrainingID",new SqlParameter[]{new SqlParameter("@TrainingID",ddlTraining.SelectedValue)});
            decimal actual=Convert.ToDecimal(dt.Rows[0]["ActualCost"]);
            lblFinalCost.Text="Rs. "+finalCost.ToString("N2"); lblActual.Text="Rs. "+actual.ToString("N2"); lblBalance.Text="Rs. "+(finalCost-actual).ToString("N2");
        }

        private void ClearSelection(){BindExpenditure();BindHostelUsage();BindSummary();}
        private void ClearEntry(){txtAmount.Text="";txtVendor.Text="";txtBill.Text="";txtRemarks.Text="";txtExpenditureEmpID.Text="";txtExpenditureTrainerID.Text="";txtDate.Text=DateTime.Today.ToString("yyyy-MM-dd");}
        protected void btnClear_Click(object sender,EventArgs e){ClearEntry();lblMessage.Text="";}
        private void ShowMessage(string text,Color color){lblMessage.Text=text;lblMessage.ForeColor=color;}
    }
}