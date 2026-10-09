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
                ClearSelection();
            }
        }

        private void BindTraining()
        {
            DataTable dt = objDB.GetDataTable("SELECT TD.TrainingID,TD.TrainingID+' - '+ISNULL(C.CourseName,'')+' - '+ISNULL(TD.Batch,'') AS TrainingName FROM TrainingDetails TD LEFT JOIN CourseMaster C ON TD.CourseID=C.CourseID ORDER BY TD.TrainingID DESC");
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
            if(ddlTraining.SelectedValue=="" || ddlCostHead.SelectedValue=="" || !Decimal.TryParse(txtAmount.Text.Trim(),out amount) || amount<=0 || !DateTime.TryParse(txtDate.Text,out date))
            {
                ShowMessage("Training, Cost Head, valid Date and positive Amount are required.",Color.Red); return;
            }
            int result=objDB.ExecuteSql("INSERT INTO FinanceActualExpenditure(TrainingID,CourseID,SessionID,CostHeadID,ExpenditureDate,Amount,VendorName,BillReference,Remarks,CreatedBy) SELECT @TrainingID,TD.CourseID,@SessionID,@CostHeadID,@Date,@Amount,@Vendor,@Bill,@Remarks,@CreatedBy FROM TrainingDetails TD WHERE TD.TrainingID=@TrainingID",
                new SqlParameter[]{new SqlParameter("@TrainingID",ddlTraining.SelectedValue),new SqlParameter("@SessionID",ddlSession.SelectedValue==""?(object)DBNull.Value:ddlSession.SelectedValue),new SqlParameter("@CostHeadID",Convert.ToInt32(ddlCostHead.SelectedValue)),new SqlParameter("@Date",date.Date),new SqlParameter("@Amount",amount),new SqlParameter("@Vendor",txtVendor.Text.Trim()),new SqlParameter("@Bill",txtBill.Text.Trim()),new SqlParameter("@Remarks",txtRemarks.Text.Trim()),new SqlParameter("@CreatedBy",Convert.ToString(Session["UserID"]))});
            if(result>0){ShowMessage("Actual expenditure saved successfully.",Color.Green);ClearEntry();BindExpenditure();BindSummary();}
        }

        protected void gvExpenditure_RowCommand(object sender,GridViewCommandEventArgs e)
        {
            if(e.CommandName!="DeleteExpenditure") return;
            int row=Convert.ToInt32(e.CommandArgument); long id=Convert.ToInt64(gvExpenditure.DataKeys[row].Value);
            objDB.ExecuteSql("DELETE FROM FinanceActualExpenditure WHERE ExpenditureID=@ID",new SqlParameter[]{new SqlParameter("@ID",id)});
            BindExpenditure(); BindSummary();
        }

        private void BindExpenditure()
        {
            if(ddlTraining.SelectedValue==""){gvExpenditure.DataSource=null;gvExpenditure.DataBind();return;}
            DataTable dt=objDB.GetDataTable("SELECT X.ExpenditureID,X.ExpenditureDate,H.CostHeadName,ISNULL(SM.SessionName,'Batch Level') SessionName,X.VendorName,X.BillReference,X.Amount,X.Remarks FROM FinanceActualExpenditure X LEFT JOIN FinanceCostHeadMaster H ON X.CostHeadID=H.CostHeadID LEFT JOIN SessionMaster SM ON X.SessionID=SM.SessionID WHERE X.TrainingID=@TrainingID ORDER BY X.ExpenditureDate DESC,X.ExpenditureID DESC",new SqlParameter[]{new SqlParameter("@TrainingID",ddlTraining.SelectedValue)});
            gvExpenditure.DataSource=dt;gvExpenditure.DataBind();
        }

        private void BindSummary()
        {
            if(ddlTraining.SelectedValue==""){lblFinalCost.Text="₹0.00";lblActual.Text="₹0.00";lblBalance.Text="₹0.00";return;}
            DataTable dt=objDB.GetDataTable("SELECT ISNULL(SUM(FinalAmount),0) FinalCost FROM FinanceCostingDetail WHERE TrainingID=@TrainingID",new SqlParameter[]{new SqlParameter("@TrainingID",ddlTraining.SelectedValue)});
            decimal finalCost=Convert.ToDecimal(dt.Rows[0]["FinalCost"]);
            dt=objDB.GetDataTable("SELECT ISNULL(SUM(Amount),0) ActualCost FROM FinanceActualExpenditure WHERE TrainingID=@TrainingID",new SqlParameter[]{new SqlParameter("@TrainingID",ddlTraining.SelectedValue)});
            decimal actual=Convert.ToDecimal(dt.Rows[0]["ActualCost"]);
            lblFinalCost.Text="₹"+finalCost.ToString("N2"); lblActual.Text="₹"+actual.ToString("N2"); lblBalance.Text="₹"+(finalCost-actual).ToString("N2");
        }

        private void ClearSelection(){BindExpenditure();BindSummary();}
        private void ClearEntry(){txtAmount.Text="";txtVendor.Text="";txtBill.Text="";txtRemarks.Text="";txtDate.Text=DateTime.Today.ToString("yyyy-MM-dd");}
        protected void btnClear_Click(object sender,EventArgs e){ClearEntry();lblMessage.Text="";}
        private void ShowMessage(string text,Color color){lblMessage.Text=text;lblMessage.ForeColor=color;}
    }
}