using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;

namespace Training.Admin
{
    public partial class FinanceRateMaster : System.Web.UI.Page
    {
        private readonly clsDataAccess objDB = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCostHeads();
                txtFrom.Text = DateTime.Today.ToString("yyyy-MM-dd");
                BindGrid();
            }
        }

        private void BindCostHeads()
        {
            DataTable dt = objDB.GetDataTable("SELECT CostHeadID,CostHeadName FROM FinanceCostHeadMaster WHERE Active='Y' ORDER BY CostHeadName");
            ddlCostHead.DataSource = dt;
            ddlCostHead.DataTextField = "CostHeadName";
            ddlCostHead.DataValueField = "CostHeadID";
            ddlCostHead.DataBind();
            ddlCostHead.Items.Insert(0,new System.Web.UI.WebControls.ListItem("Select Cost Head",""));
        }

        private void BindGrid()
        {
            gvRates.DataSource = objDB.GetDataTable("SELECT R.RateID,H.CostHeadName,R.Rate,R.EffectiveFrom,R.EffectiveTo,R.Active FROM FinanceRateMaster R INNER JOIN FinanceCostHeadMaster H ON R.CostHeadID=H.CostHeadID ORDER BY H.CostHeadName,R.EffectiveFrom DESC,R.RateID DESC");
            gvRates.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            decimal rate;
            DateTime fromDate;
            DateTime toDate;
            if (ddlCostHead.SelectedValue == "" || !Decimal.TryParse(txtRate.Text.Trim(),out rate) || rate < 0 || !DateTime.TryParse(txtFrom.Text,out fromDate))
            {
                ShowMessage("Cost Head, valid Rate and Effective From are required.",Color.Red);
                return;
            }
            DateTime? to = null;
            if (txtTo.Text.Trim() != "")
            {
                if (!DateTime.TryParse(txtTo.Text,out toDate))
                {
                    ShowMessage("Invalid Effective To date.",Color.Red);
                    return;
                }
                to = toDate.Date;
            }
            int result = objDB.ExecuteSql("INSERT INTO FinanceRateMaster(CostHeadID,Rate,EffectiveFrom,EffectiveTo,Remarks,CreatedBy) VALUES(@CostHeadID,@Rate,@From,@To,@Remarks,@CreatedBy)", new SqlParameter[] { new SqlParameter("@CostHeadID",Convert.ToInt32(ddlCostHead.SelectedValue)),new SqlParameter("@Rate",rate),new SqlParameter("@From",fromDate.Date),new SqlParameter("@To",to.HasValue?(object)to.Value:(object)DBNull.Value),new SqlParameter("@Remarks",txtRemarks.Text.Trim()),new SqlParameter("@CreatedBy",Convert.ToString(Session["UserID"])) });
            if(result>0){ShowMessage("Rate saved successfully.",Color.Green);ClearForm();BindGrid();}
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm(){txtRate.Text="";txtFrom.Text=DateTime.Today.ToString("yyyy-MM-dd");txtTo.Text="";txtRemarks.Text="";}
        private void ShowMessage(string text,Color color){lblMessage.Text=text;lblMessage.ForeColor=color;}
    }
}