using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using OfficeOpenXml; 

namespace Training.Admin
{
    public partial class TopicMaster : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCategory();
                BindGrid();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            ScriptManager.RegisterStartupScript(this, GetType(), "ddl", "$('#ddlCategory').select2();", true);
        }

        private void BindCategory()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT CategoryName FROM TopicCategoryMaster ORDER BY CategoryName", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlCategory.DataSource = dt;
            ddlCategory.DataTextField = "CategoryName";
            ddlCategory.DataValueField = "CategoryName";
            ddlCategory.DataBind();
            ddlCategory.Items.Insert(0, new ListItem("-- Select Category --", ""));
        }

        private void BindGrid()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT TopicID,TopicName,Category,Description,CreatedOn FROM TopicMaster ORDER BY ID DESC", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            gvTopic.DataSource = dt;
            gvTopic.DataBind();
        }

        private void ClearControls()
        {
            txtTopicName.Text = "";
            ddlCategory.SelectedIndex = 0;
            txtDescription.Text = "";
            ViewState["TopicID"] = null;
            btnSave.Visible = true;
            btnUpdate.Visible = false;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateTopic())
            {
                return;
            }

            string TopicID = "";

            SqlCommand cmdID = new SqlCommand("SELECT 'TOP'+RIGHT('0000'+CAST(ISNULL(MAX(ID),0)+1 AS VARCHAR(4)),4) FROM TopicMaster", con);

            con.Open();
            TopicID = Convert.ToString(cmdID.ExecuteScalar());
            con.Close();

            SqlCommand cmdCheck = new SqlCommand("SELECT COUNT(*) FROM TopicMaster WHERE TopicName=@TopicName", con);
            cmdCheck.Parameters.AddWithValue("@TopicName", txtTopicName.Text.Trim());

            con.Open();
            int Count = Convert.ToInt32(cmdCheck.ExecuteScalar());
            con.Close();

            if (Count > 0)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Topic Name already exists.";
                return;
            }

            SqlCommand cmd = new SqlCommand(@"INSERT INTO TopicMaster
    (
        TopicID,
        TopicName,
        Category,
        Description,
        CreatedBy
    )
    VALUES
    (
        @TopicID,
        @TopicName,
        @Category,
        @Description,
        @CreatedBy
    )", con);

            cmd.Parameters.AddWithValue("@TopicID", TopicID);
            cmd.Parameters.AddWithValue("@TopicName", txtTopicName.Text.Trim());
            cmd.Parameters.AddWithValue("@Category", ddlCategory.SelectedValue);
            cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
            cmd.Parameters.AddWithValue("@CreatedBy", Session["UserID"] == null ? "" : Session["UserID"].ToString());

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            ClearControls();
            BindGrid();

            lblMessage.ForeColor = System.Drawing.Color.Green;
            lblMessage.Text = "Topic saved successfully.";
        }

        protected void gvTopic_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditRecord")
            {
                LoadTopic(e.CommandArgument.ToString());
            }

            if (e.CommandName == "DeleteRecord")
            {
                DeleteTopic(e.CommandArgument.ToString());
            }
        }

        private void LoadTopic(string TopicID)
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM TopicMaster WHERE TopicID=@TopicID", con);
            da.SelectCommand.Parameters.AddWithValue("@TopicID", TopicID);

            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                ViewState["TopicID"] = TopicID;
                txtTopicName.Text = dt.Rows[0]["TopicName"].ToString();
                txtDescription.Text = dt.Rows[0]["Description"].ToString();

                string Category = dt.Rows[0]["Category"].ToString();

                if (ddlCategory.Items.FindByValue(Category) != null)
                {
                    ddlCategory.SelectedValue = Category;
                }

                btnSave.Visible = false;
                btnUpdate.Visible = true;
            }
        }

        private void DeleteTopic(string TopicID)
        {
            SqlCommand cmd = new SqlCommand("DELETE FROM TopicMaster WHERE TopicID=@TopicID", con);
            cmd.Parameters.AddWithValue("@TopicID", TopicID);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            lblMessage.ForeColor = System.Drawing.Color.Green;
            lblMessage.Text = "Deleted Successfully.";

            ClearControls();
            BindGrid();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (ViewState["TopicID"] == null)
            {
                return;
            }

            if (!ValidateTopic())
            {
                return;
            }

            SqlCommand chk = new SqlCommand("SELECT COUNT(*) FROM TopicMaster WHERE TopicName=@TopicName AND TopicID<>@TopicID", con);
            chk.Parameters.AddWithValue("@TopicName", txtTopicName.Text.Trim());
            chk.Parameters.AddWithValue("@TopicID", ViewState["TopicID"].ToString());

            con.Open();
            int cnt = Convert.ToInt32(chk.ExecuteScalar());
            con.Close();

            if (cnt > 0)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Topic Name already exists.";
                return;
            }

            SqlCommand cmd = new SqlCommand(@"UPDATE TopicMaster SET
TopicName=@TopicName,
Category=@Category,
Description=@Description
WHERE TopicID=@TopicID", con);

            cmd.Parameters.AddWithValue("@TopicName", txtTopicName.Text.Trim());
            cmd.Parameters.AddWithValue("@Category", ddlCategory.SelectedValue);
            cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
            cmd.Parameters.AddWithValue("@TopicID", ViewState["TopicID"].ToString());

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            ClearControls();
            BindGrid();

            lblMessage.ForeColor = System.Drawing.Color.Green;
            lblMessage.Text = "Topic updated successfully.";
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearControls();
            lblMessage.Text = "";
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SqlDataAdapter da = new SqlDataAdapter(@"SELECT TopicID,
TopicName,
Category,
Description,
CreatedOn
FROM TopicMaster
WHERE TopicName LIKE @Search
OR Category LIKE @Search
ORDER BY TopicName", con);

            da.SelectCommand.Parameters.AddWithValue("@Search", "%" + txtSearch.Text.Trim() + "%");

            DataTable dt = new DataTable();
            da.Fill(dt);

            gvTopic.PageIndex = 0;
            gvTopic.DataSource = dt;
            gvTopic.DataBind();
        }

        protected void gvTopic_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTopic.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvTopic_Sorting(object sender, GridViewSortEventArgs e)
        {
            BindGrid();
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            SqlDataAdapter da = new SqlDataAdapter(@"SELECT
TopicName AS [Topic Name],
Category,
Description,
CreatedOn AS [Created On]
FROM TopicMaster
ORDER BY TopicName", con);

            DataTable dt = new DataTable();
            da.Fill(dt);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Topic Master");
                ws.Cells["A1"].LoadFromDataTable(dt, true);

                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=TopicMaster.xlsx");
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.End();
            }
        }

        private bool ValidateTopic()
        {
            if (txtTopicName.Text.Trim() == "")
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Enter Topic Name.";
                txtTopicName.Focus();
                return false;
            }

            if (ddlCategory.SelectedIndex == 0)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Select Category.";
                ddlCategory.Focus();
                return false;
            }

            return true;
        }
    }
}
