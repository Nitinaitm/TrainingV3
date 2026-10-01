using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class HomeTestimonials : System.Web.UI.Page
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
            string sql = "SELECT ID, Quote, Name, Role, SubmittedBy, DisplayOrder, IsActive, Status FROM HomeTestimonial WHERE 1=1 ";

            if (ddlStatusFilter.SelectedValue != "")
            {
                sql += "AND Status=@Status ";
            }

            sql += "ORDER BY DisplayOrder";

            SqlParameter[] param = { new SqlParameter("@Status", ddlStatusFilter.SelectedValue) };

            DataTable dt = objDB.GetDataTable(sql, param);

            gvTestimonials.DataSource = dt;
            gvTestimonials.DataBind();
        }

        protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtQuote.Text) || string.IsNullOrWhiteSpace(txtName.Text))
            {
                lblMessage.CssClass = "d-block mt-3 text-danger";
                lblMessage.Text = "Quote and Name are required.";
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
                // Status is deliberately NOT touched here - editing an existing
                // testimonial (whether Pending, Approved, or Rejected) never
                // changes its review status. Only Approve/Reject do that.
                sql =
                    "UPDATE HomeTestimonial " +
                    "SET Quote=@Quote, Name=@Name, Role=@Role, DisplayOrder=@DisplayOrder, IsActive=@IsActive " +
                    "WHERE ID=@ID";

                param = new SqlParameter[]
                {
                    new SqlParameter("@Quote", txtQuote.Text.Trim()),
                    new SqlParameter("@Name", txtName.Text.Trim()),
                    new SqlParameter("@Role", txtRole.Text.Trim()),
                    new SqlParameter("@DisplayOrder", displayOrder),
                    new SqlParameter("@IsActive", chkIsActive.Checked),
                    new SqlParameter("@ID", hfID.Value)
                };
            }
            else
            {
                // Status column is deliberately omitted from this INSERT -
                // its DEFAULT 'Approved' applies automatically, since an
                // admin adding one directly needs no separate approval step.
                sql =
                    "INSERT INTO HomeTestimonial (Quote, Name, Role, DisplayOrder, IsActive, CreatedOn, CreatedBy) " +
                    "VALUES (@Quote, @Name, @Role, @DisplayOrder, @IsActive, GETDATE(), @CreatedBy)";

                param = new SqlParameter[]
                {
                    new SqlParameter("@Quote", txtQuote.Text.Trim()),
                    new SqlParameter("@Name", txtName.Text.Trim()),
                    new SqlParameter("@Role", txtRole.Text.Trim()),
                    new SqlParameter("@DisplayOrder", displayOrder),
                    new SqlParameter("@IsActive", chkIsActive.Checked),
                    new SqlParameter("@CreatedBy", adminID)
                };
            }

            objDB.ExecuteSql(sql, param);

            lblMessage.CssClass = "d-block mt-3 text-success";
            lblMessage.Text = isEditing ? "Testimonial updated successfully." : "Testimonial added successfully.";

            ResetForm();
            BindGrid();
        }

        private void ResetForm()
        {
            hfID.Value = "0";
            txtQuote.Text = "";
            txtName.Text = "";
            txtRole.Text = "";
            txtDisplayOrder.Text = "1";
            chkIsActive.Checked = true;
            btnSave.Text = "Save";
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        protected void gvTestimonials_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();

            switch (e.CommandName)
            {
                case "EditTestimonial":
                    LoadTestimonialForEdit(id);
                    break;

                case "DeleteTestimonial":
                    DeleteTestimonial(id);
                    BindGrid();
                    break;

                case "ApproveTestimonial":
                    UpdateStatus(id, "Approved", setActive: true);
                    BindGrid();
                    break;

                case "RejectTestimonial":
                    UpdateStatus(id, "Rejected", setActive: false);
                    BindGrid();
                    break;
            }
        }

        private void UpdateStatus(string id, string newStatus, bool setActive)
        {
            string adminID = Session["AdminID"] == null ? "Admin" : Session["AdminID"].ToString();

            string sql =
                "UPDATE HomeTestimonial " +
                "SET Status=@Status, IsActive=@IsActive " +
                "WHERE ID=@ID AND Status='Pending'";

            SqlParameter[] param =
            {
                new SqlParameter("@Status", newStatus),
                new SqlParameter("@IsActive", setActive),
                new SqlParameter("@ID", id)
            };

            objDB.ExecuteSql(sql, param);
        }

        private void LoadTestimonialForEdit(string id)
        {
            string sql = "SELECT ID, Quote, Name, Role, DisplayOrder, IsActive FROM HomeTestimonial WHERE ID=@ID";

            SqlParameter[] param = { new SqlParameter("@ID", id) };

            DataTable dt = objDB.GetDataTable(sql, param);

            if (dt.Rows.Count == 0)
            {
                return;
            }

            DataRow dr = dt.Rows[0];

            hfID.Value = dr["ID"].ToString();
            txtQuote.Text = dr["Quote"].ToString();
            txtName.Text = dr["Name"].ToString();
            txtRole.Text = dr["Role"].ToString();
            txtDisplayOrder.Text = dr["DisplayOrder"].ToString();
            chkIsActive.Checked = Convert.ToBoolean(dr["IsActive"]);

            btnSave.Text = "Update Testimonial";
        }

        private void DeleteTestimonial(string id)
        {
            string sql = "DELETE FROM HomeTestimonial WHERE ID=@ID";

            SqlParameter[] param = { new SqlParameter("@ID", id) };

            objDB.ExecuteSql(sql, param);
        }
    }
}