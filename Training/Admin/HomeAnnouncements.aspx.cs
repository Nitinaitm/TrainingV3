using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class HomeAnnouncements : System.Web.UI.Page
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
                txtPublishedDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                BindGrid();
            }
        }

        private void BindGrid()
        {
            string sql = "SELECT ID, Category, Title, LinkUrl, PublishedDate, DisplayOrder, IsActive FROM HomeAnnouncement WHERE 1=1 ";

            if (ddlFilterCategory.SelectedValue != "")
            {
                sql += "AND Category=@Category ";
            }

            sql += "ORDER BY Category, DisplayOrder";

            SqlParameter[] param = { new SqlParameter("@Category", ddlFilterCategory.SelectedValue) };

            DataTable dt = objDB.GetDataTable(sql, param);

            gvAnnouncements.DataSource = dt;
            gvAnnouncements.DataBind();
        }

        protected void ddlFilterCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                lblMessage.CssClass = "d-block mt-3 text-danger";
                lblMessage.Text = "Title is required.";
                return;
            }

            int displayOrder;

            if (!int.TryParse(txtDisplayOrder.Text.Trim(), out displayOrder))
            {
                displayOrder = 1;
            }

            DateTime publishedDate;

            if (!DateTime.TryParseExact(txtPublishedDate.Text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out publishedDate))
            {
                publishedDate = DateTime.Now;
            }

            string adminID = Session["AdminID"] == null ? "Admin" : Session["AdminID"].ToString();

            bool isEditing = hfID.Value != "0";

            string sql;
            SqlParameter[] param;

            if (isEditing)
            {
                sql =
                    "UPDATE HomeAnnouncement " +
                    "SET Category=@Category, Title=@Title, LinkUrl=@LinkUrl, PublishedDate=@PublishedDate, " +
                    "DisplayOrder=@DisplayOrder, IsActive=@IsActive " +
                    "WHERE ID=@ID";

                param = new SqlParameter[]
                {
                    new SqlParameter("@Category", ddlCategory.SelectedValue),
                    new SqlParameter("@Title", txtTitle.Text.Trim()),
                    new SqlParameter("@LinkUrl", string.IsNullOrWhiteSpace(txtLinkUrl.Text) ? (object)DBNull.Value : txtLinkUrl.Text.Trim()),
                    new SqlParameter("@PublishedDate", publishedDate),
                    new SqlParameter("@DisplayOrder", displayOrder),
                    new SqlParameter("@IsActive", chkIsActive.Checked),
                    new SqlParameter("@ID", hfID.Value)
                };
            }
            else
            {
                sql =
                    "INSERT INTO HomeAnnouncement (Category, Title, LinkUrl, PublishedDate, DisplayOrder, IsActive, CreatedOn, CreatedBy) " +
                    "VALUES (@Category, @Title, @LinkUrl, @PublishedDate, @DisplayOrder, @IsActive, GETDATE(), @CreatedBy)";

                param = new SqlParameter[]
                {
                    new SqlParameter("@Category", ddlCategory.SelectedValue),
                    new SqlParameter("@Title", txtTitle.Text.Trim()),
                    new SqlParameter("@LinkUrl", string.IsNullOrWhiteSpace(txtLinkUrl.Text) ? (object)DBNull.Value : txtLinkUrl.Text.Trim()),
                    new SqlParameter("@PublishedDate", publishedDate),
                    new SqlParameter("@DisplayOrder", displayOrder),
                    new SqlParameter("@IsActive", chkIsActive.Checked),
                    new SqlParameter("@CreatedBy", adminID)
                };
            }

            objDB.ExecuteSql(sql, param);

            lblMessage.CssClass = "d-block mt-3 text-success";
            lblMessage.Text = isEditing ? "Announcement updated successfully." : "Announcement added successfully.";

            ResetForm();
            BindGrid();
        }

        private void ResetForm()
        {
            hfID.Value = "0";
            ddlCategory.SelectedIndex = 0;
            txtTitle.Text = "";
            txtLinkUrl.Text = "";
            txtPublishedDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtDisplayOrder.Text = "1";
            chkIsActive.Checked = true;
            btnSave.Text = "Save";
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        protected void gvAnnouncements_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();

            if (e.CommandName == "EditAnnouncement")
            {
                LoadAnnouncementForEdit(id);
            }
            else if (e.CommandName == "DeleteAnnouncement")
            {
                DeleteAnnouncement(id);
                BindGrid();
            }
        }

        private void LoadAnnouncementForEdit(string id)
        {
            string sql = "SELECT ID, Category, Title, LinkUrl, PublishedDate, DisplayOrder, IsActive FROM HomeAnnouncement WHERE ID=@ID";

            SqlParameter[] param = { new SqlParameter("@ID", id) };

            DataTable dt = objDB.GetDataTable(sql, param);

            if (dt.Rows.Count == 0)
            {
                return;
            }

            DataRow dr = dt.Rows[0];

            hfID.Value = dr["ID"].ToString();
            ddlCategory.SelectedValue = dr["Category"].ToString();
            txtTitle.Text = dr["Title"].ToString();
            txtLinkUrl.Text = dr["LinkUrl"] == DBNull.Value ? "" : dr["LinkUrl"].ToString();
            txtPublishedDate.Text = Convert.ToDateTime(dr["PublishedDate"]).ToString("yyyy-MM-dd");
            txtDisplayOrder.Text = dr["DisplayOrder"].ToString();
            chkIsActive.Checked = Convert.ToBoolean(dr["IsActive"]);

            btnSave.Text = "Update Announcement";
        }

        private void DeleteAnnouncement(string id)
        {
            string sql = "DELETE FROM HomeAnnouncement WHERE ID=@ID";

            SqlParameter[] param = { new SqlParameter("@ID", id) };

            objDB.ExecuteSql(sql, param);
        }
    }
}