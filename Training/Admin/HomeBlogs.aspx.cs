using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class HomeBlogs : System.Web.UI.Page
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
            string sql =
                "SELECT ID, Category, Title, Excerpt, FullContent, ImagePath, Url, SubmittedBy, DisplayOrder, IsActive, Status " +
                "FROM HomeBlog WHERE 1=1 ";

            if (ddlStatusFilter.SelectedValue != "")
            {
                sql += "AND Status=@Status ";
            }

            sql += "ORDER BY DisplayOrder";

            SqlParameter[] param = { new SqlParameter("@Status", ddlStatusFilter.SelectedValue) };

            DataTable dt = objDB.GetDataTable(sql, param);

            gvBlogs.DataSource = dt;
            gvBlogs.DataBind();
        }

        protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCategory.Text) ||
                string.IsNullOrWhiteSpace(txtTitle.Text) ||
                string.IsNullOrWhiteSpace(txtExcerpt.Text))
            {
                lblMessage.CssClass = "d-block mt-3 text-danger";
                lblMessage.Text = "Category, Title, and Excerpt are required.";
                return;
            }

            int displayOrder;

            if (!int.TryParse(txtDisplayOrder.Text.Trim(), out displayOrder))
            {
                displayOrder = 1;
            }

            string imagePath;

            try
            {
                imagePath = ResolveImagePath();
            }
            catch (Exception ex)
            {
                lblMessage.CssClass = "d-block mt-3 text-danger";
                lblMessage.Text = ex.Message;
                return;
            }

            string adminID = Session["AdminID"] == null ? "Admin" : Session["AdminID"].ToString();

            bool isEditing = hfID.Value != "0";

            string sql;
            SqlParameter[] param;

            object imagePathValue = string.IsNullOrEmpty(imagePath) ? (object)DBNull.Value : imagePath;
            object fullContentValue = string.IsNullOrWhiteSpace(txtFullContent.Text) ? (object)DBNull.Value : txtFullContent.Text.Trim();
            object urlValue = string.IsNullOrWhiteSpace(txtUrl.Text) ? (object)DBNull.Value : txtUrl.Text.Trim();

            if (isEditing)
            {
                // Status is deliberately NOT touched here - same rule as
                // Testimonials. Editing never changes review status;
                // only Approve/Reject do that.
                sql =
                    "UPDATE HomeBlog " +
                    "SET Category=@Category, Title=@Title, Excerpt=@Excerpt, FullContent=@FullContent, " +
                    "ImagePath=@ImagePath, Url=@Url, DisplayOrder=@DisplayOrder, IsActive=@IsActive " +
                    "WHERE ID=@ID";

                param = new SqlParameter[]
                {
                    new SqlParameter("@Category", txtCategory.Text.Trim()),
                    new SqlParameter("@Title", txtTitle.Text.Trim()),
                    new SqlParameter("@Excerpt", txtExcerpt.Text.Trim()),
                    new SqlParameter("@FullContent", fullContentValue),
                    new SqlParameter("@ImagePath", imagePathValue),
                    new SqlParameter("@Url", urlValue),
                    new SqlParameter("@DisplayOrder", displayOrder),
                    new SqlParameter("@IsActive", chkIsActive.Checked),
                    new SqlParameter("@ID", hfID.Value)
                };
            }
            else
            {
                // Status column omitted from this INSERT - its DEFAULT
                // 'Approved' applies automatically, same as Testimonials.
                sql =
                    "INSERT INTO HomeBlog (Category, Title, Excerpt, FullContent, ImagePath, Url, DisplayOrder, IsActive, PublishedOn, CreatedOn, CreatedBy) " +
                    "VALUES (@Category, @Title, @Excerpt, @FullContent, @ImagePath, @Url, @DisplayOrder, @IsActive, GETDATE(), GETDATE(), @CreatedBy)";

                param = new SqlParameter[]
                {
                    new SqlParameter("@Category", txtCategory.Text.Trim()),
                    new SqlParameter("@Title", txtTitle.Text.Trim()),
                    new SqlParameter("@Excerpt", txtExcerpt.Text.Trim()),
                    new SqlParameter("@FullContent", fullContentValue),
                    new SqlParameter("@ImagePath", imagePathValue),
                    new SqlParameter("@Url", urlValue),
                    new SqlParameter("@DisplayOrder", displayOrder),
                    new SqlParameter("@IsActive", chkIsActive.Checked),
                    new SqlParameter("@CreatedBy", adminID)
                };
            }

            objDB.ExecuteSql(sql, param);

            lblMessage.CssClass = "d-block mt-3 text-success";
            lblMessage.Text = isEditing ? "Blog post updated successfully." : "Blog post added successfully.";

            ResetForm();
            BindGrid();
        }

        private string ResolveImagePath()
        {
            if (!fuImage.HasFile)
            {
                return hfExistingImagePath.Value;
            }

            string extension = Path.GetExtension(fuImage.FileName).ToLower();

            if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
            {
                throw new Exception("Image must be a JPG or PNG file.");
            }

            if (fuImage.PostedFile.ContentLength > 2 * 1024 * 1024)
            {
                throw new Exception("Image size should not exceed 2 MB.");
            }

            string folder = Server.MapPath("~/Uploads/HomePage/Blogs/");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string fileName = "blog_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + extension;

            fuImage.SaveAs(folder + fileName);

            return "Uploads/HomePage/Blogs/" + fileName;
        }

        private void ResetForm()
        {
            hfID.Value = "0";
            hfExistingImagePath.Value = "";
            txtCategory.Text = "";
            txtTitle.Text = "";
            txtExcerpt.Text = "";
            txtFullContent.Text = "";
            txtUrl.Text = "";
            txtDisplayOrder.Text = "1";
            chkIsActive.Checked = true;
            imgCurrentImage.ImageUrl = "";
            btnSave.Text = "Save";
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        protected void gvBlogs_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();

            switch (e.CommandName)
            {
                case "EditBlog":
                    LoadBlogForEdit(id);
                    break;

                case "DeleteBlog":
                    DeleteBlog(id);
                    BindGrid();
                    break;

                case "ApproveBlog":
                    UpdateStatus(id, "Approved", setActive: true);
                    BindGrid();
                    break;

                case "RejectBlog":
                    UpdateStatus(id, "Rejected", setActive: false);
                    BindGrid();
                    break;
            }
        }

        private void UpdateStatus(string id, string newStatus, bool setActive)
        {
            string sql =
                "UPDATE HomeBlog " +
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

        private void LoadBlogForEdit(string id)
        {
            string sql =
                "SELECT ID, Category, Title, Excerpt, FullContent, ImagePath, Url, DisplayOrder, IsActive " +
                "FROM HomeBlog WHERE ID=@ID";

            SqlParameter[] param = { new SqlParameter("@ID", id) };

            DataTable dt = objDB.GetDataTable(sql, param);

            if (dt.Rows.Count == 0)
            {
                return;
            }

            DataRow dr = dt.Rows[0];

            hfID.Value = dr["ID"].ToString();
            hfExistingImagePath.Value = dr["ImagePath"] == DBNull.Value ? "" : dr["ImagePath"].ToString();

            txtCategory.Text = dr["Category"].ToString();
            txtTitle.Text = dr["Title"].ToString();
            txtExcerpt.Text = dr["Excerpt"].ToString();
            txtFullContent.Text = dr["FullContent"] == DBNull.Value ? "" : dr["FullContent"].ToString();
            txtUrl.Text = dr["Url"] == DBNull.Value ? "" : dr["Url"].ToString();
            txtDisplayOrder.Text = dr["DisplayOrder"].ToString();
            chkIsActive.Checked = Convert.ToBoolean(dr["IsActive"]);

            string imagePath = dr["ImagePath"] == DBNull.Value ? "" : dr["ImagePath"].ToString();

            if (!string.IsNullOrWhiteSpace(imagePath))
            {
                imgCurrentImage.ImageUrl = "~/" + imagePath;
            }

            btnSave.Text = "Update Blog Post";
        }

        private void DeleteBlog(string id)
        {
            string sql = "DELETE FROM HomeBlog WHERE ID=@ID";

            SqlParameter[] param = { new SqlParameter("@ID", id) };

            objDB.ExecuteSql(sql, param);
        }
    }
}