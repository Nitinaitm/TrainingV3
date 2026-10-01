using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class HomeMarquee : System.Web.UI.Page
    {
        clsDataAccess objDB = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["InternalRedirect_Admin"] == null)
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid()
        {
            string sql = "SELECT ID, Text, DisplayOrder, IsActive FROM HomeMarquee ORDER BY DisplayOrder";

            DataTable dt = objDB.GetDataTable(sql, null);

            gvMarquee.DataSource = dt;
            gvMarquee.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtText.Text))
            {
                lblMessage.CssClass = "d-block mt-3 text-danger";
                lblMessage.Text = "Notice text is required.";
                return;
            }

            int displayOrder;

            if (!int.TryParse(txtDisplayOrder.Text.Trim(), out displayOrder))
            {
                displayOrder = 1;
            }

            string adminID = Session["AdminID"] == null ? "Admin" : Session["AdminID"].ToString();

            bool isEditing = hfID.Value != "0";

            string sql;
            SqlParameter[] param;

            if (isEditing)
            {
                sql = "UPDATE HomeMarquee SET Text=@Text, DisplayOrder=@DisplayOrder, IsActive=@IsActive WHERE ID=@ID";

                param = new SqlParameter[]
                {
                    new SqlParameter("@Text", txtText.Text.Trim()),
                    new SqlParameter("@DisplayOrder", displayOrder),
                    new SqlParameter("@IsActive", chkIsActive.Checked),
                    new SqlParameter("@ID", hfID.Value)
                };
            }
            else
            {
                sql =
                    "INSERT INTO HomeMarquee (Text, DisplayOrder, IsActive, CreatedOn, CreatedBy) " +
                    "VALUES (@Text, @DisplayOrder, @IsActive, GETDATE(), @CreatedBy)";

                param = new SqlParameter[]
                {
                    new SqlParameter("@Text", txtText.Text.Trim()),
                    new SqlParameter("@DisplayOrder", displayOrder),
                    new SqlParameter("@IsActive", chkIsActive.Checked),
                    new SqlParameter("@CreatedBy", adminID)
                };
            }

            objDB.ExecuteSql(sql, param);

            lblMessage.CssClass = "d-block mt-3 text-success";
            lblMessage.Text = isEditing ? "Notice updated successfully." : "Notice added successfully.";

            ResetForm();
            BindGrid();
        }

        private void ResetForm()
        {
            hfID.Value = "0";
            txtText.Text = "";
            txtDisplayOrder.Text = "1";
            chkIsActive.Checked = true;
            btnSave.Text = "Save";
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        protected void gvMarquee_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();

            if (e.CommandName == "EditNotice")
            {
                LoadNoticeForEdit(id);
            }
            else if (e.CommandName == "DeleteNotice")
            {
                DeleteNotice(id);
                BindGrid();
            }
        }

        private void LoadNoticeForEdit(string id)
        {
            string sql = "SELECT ID, Text, DisplayOrder, IsActive FROM HomeMarquee WHERE ID=@ID";

            SqlParameter[] param = { new SqlParameter("@ID", id) };

            DataTable dt = objDB.GetDataTable(sql, param);

            if (dt.Rows.Count == 0)
            {
                return;
            }

            DataRow dr = dt.Rows[0];

            hfID.Value = dr["ID"].ToString();
            txtText.Text = dr["Text"].ToString();
            txtDisplayOrder.Text = dr["DisplayOrder"].ToString();
            chkIsActive.Checked = Convert.ToBoolean(dr["IsActive"]);

            btnSave.Text = "Update Notice";
        }

        private void DeleteNotice(string id)
        {
            string sql = "DELETE FROM HomeMarquee WHERE ID=@ID";

            SqlParameter[] param = { new SqlParameter("@ID", id) };

            objDB.ExecuteSql(sql, param);
        }
    }
}