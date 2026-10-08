using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class FinanceCostHeadMaster : System.Web.UI.Page
    {
        private readonly clsDataAccess objDB = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindGrid();
        }

        private void BindGrid()
        {
            gvHeads.DataSource = FinanceCommon.GetCostHeads(objDB);
            gvHeads.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string code = txtCode.Text.Trim().ToUpper();
            string name = txtName.Text.Trim();
            string unit = txtUnit.Text.Trim();
            if (code == "" || name == "" || unit == "")
            {
                ShowMessage("Code, Cost Head and Unit are required.", Color.Red);
                return;
            }
            DataTable exists = objDB.GetDataTable("SELECT CostHeadID FROM FinanceCostHeadMaster WHERE CostHeadCode=@Code OR CostHeadName=@Name", new SqlParameter[] { new SqlParameter("@Code", code), new SqlParameter("@Name", name) });
            if (exists.Rows.Count > 0)
            {
                ShowMessage("Cost Head Code or Name already exists.", Color.Red);
                return;
            }
            int result = objDB.ExecuteSql("INSERT INTO FinanceCostHeadMaster(CostHeadCode,CostHeadName,CostLevel,CalculationMode,UnitType,CreatedBy) VALUES(@Code,@Name,@Level,@Mode,@Unit,@CreatedBy)", new SqlParameter[] { new SqlParameter("@Code", code), new SqlParameter("@Name", name), new SqlParameter("@Level", ddlLevel.SelectedValue), new SqlParameter("@Mode", ddlMode.SelectedValue), new SqlParameter("@Unit", unit), new SqlParameter("@CreatedBy", Convert.ToString(Session["UserID"])) });
            if (result > 0)
            {
                ShowMessage("Cost Head saved successfully.", Color.Green);
                ClearForm();
                BindGrid();
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
            lblMessage.Text = "";
        }

        protected void gvHeads_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Toggle") return;
            int rowIndex = Convert.ToInt32(e.CommandArgument);
            int id = Convert.ToInt32(gvHeads.DataKeys[rowIndex].Value);
            objDB.ExecuteSql("UPDATE FinanceCostHeadMaster SET Active=CASE WHEN Active='Y' THEN 'N' ELSE 'Y' END WHERE CostHeadID=@ID", new SqlParameter[] { new SqlParameter("@ID", id) });
            BindGrid();
        }

        private void ClearForm()
        {
            txtCode.Text = "";
            txtName.Text = "";
            txtUnit.Text = "";
            ddlLevel.SelectedIndex = 0;
            ddlMode.SelectedIndex = 0;
        }

        private void ShowMessage(string text, Color color)
        {
            lblMessage.Text = text;
            lblMessage.ForeColor = color;
        }
    }
}