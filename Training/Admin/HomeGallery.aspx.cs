using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class HomeGallery : System.Web.UI.Page
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
            string sql = "SELECT ID, MediaType, ThumbnailPath, Caption, VideoUrl, DisplayOrder, IsActive FROM HomeGallery ORDER BY DisplayOrder";

            DataTable dt = objDB.GetDataTable(sql, null);

            gvGallery.DataSource = dt;
            gvGallery.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCaption.Text))
            {
                lblMessage.CssClass = "d-block mt-3 text-danger";
                lblMessage.Text = "Caption is required.";
                return;
            }

            int displayOrder;

            if (!int.TryParse(txtDisplayOrder.Text.Trim(), out displayOrder))
            {
                displayOrder = 1;
            }

            string thumbnailPath;

            try
            {
                thumbnailPath = ResolveThumbnailPath();
            }
            catch (Exception ex)
            {
                lblMessage.CssClass = "d-block mt-3 text-danger";
                lblMessage.Text = ex.Message;
                return;
            }

            if (string.IsNullOrEmpty(thumbnailPath))
            {
                lblMessage.CssClass = "d-block mt-3 text-danger";
                lblMessage.Text = "A thumbnail image is required.";
                return;
            }

            string adminID = Session["AdminID"] == null ? "Admin" : Session["AdminID"].ToString();

            bool isEditing = hfID.Value != "0";

            string sql;
            SqlParameter[] param;

            object videoUrlValue = string.IsNullOrWhiteSpace(txtVideoUrl.Text) ? (object)DBNull.Value : txtVideoUrl.Text.Trim();

            if (isEditing)
            {
                sql =
                    "UPDATE HomeGallery " +
                    "SET MediaType=@MediaType, ThumbnailPath=@ThumbnailPath, Caption=@Caption, VideoUrl=@VideoUrl, " +
                    "DisplayOrder=@DisplayOrder, IsActive=@IsActive " +
                    "WHERE ID=@ID";

                param = new SqlParameter[]
                {
                    new SqlParameter("@MediaType", ddlMediaType.SelectedValue),
                    new SqlParameter("@ThumbnailPath", thumbnailPath),
                    new SqlParameter("@Caption", txtCaption.Text.Trim()),
                    new SqlParameter("@VideoUrl", videoUrlValue),
                    new SqlParameter("@DisplayOrder", displayOrder),
                    new SqlParameter("@IsActive", chkIsActive.Checked),
                    new SqlParameter("@ID", hfID.Value)
                };
            }
            else
            {
                sql =
                    "INSERT INTO HomeGallery (MediaType, ThumbnailPath, Caption, VideoUrl, DisplayOrder, IsActive, CreatedOn, CreatedBy) " +
                    "VALUES (@MediaType, @ThumbnailPath, @Caption, @VideoUrl, @DisplayOrder, @IsActive, GETDATE(), @CreatedBy)";

                param = new SqlParameter[]
                {
                    new SqlParameter("@MediaType", ddlMediaType.SelectedValue),
                    new SqlParameter("@ThumbnailPath", thumbnailPath),
                    new SqlParameter("@Caption", txtCaption.Text.Trim()),
                    new SqlParameter("@VideoUrl", videoUrlValue),
                    new SqlParameter("@DisplayOrder", displayOrder),
                    new SqlParameter("@IsActive", chkIsActive.Checked),
                    new SqlParameter("@CreatedBy", adminID)
                };
            }

            objDB.ExecuteSql(sql, param);

            lblMessage.CssClass = "d-block mt-3 text-success";
            lblMessage.Text = isEditing ? "Media item updated successfully." : "Media item added successfully.";

            ResetForm();
            BindGrid();
        }

        private string ResolveThumbnailPath()
        {
            if (!fuThumbnail.HasFile)
            {
                return hfExistingThumbnailPath.Value;
            }

            string extension = Path.GetExtension(fuThumbnail.FileName).ToLower();

            if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
            {
                throw new Exception("Thumbnail must be a JPG or PNG file.");
            }

            if (fuThumbnail.PostedFile.ContentLength > 2 * 1024 * 1024)
            {
                throw new Exception("Thumbnail size should not exceed 2 MB.");
            }

            string folder = Server.MapPath("~/Uploads/HomePage/Gallery/");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string fileName = "gallery_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + extension;

            fuThumbnail.SaveAs(folder + fileName);

            return "Uploads/HomePage/Gallery/" + fileName;
        }

        private void ResetForm()
        {
            hfID.Value = "0";
            hfExistingThumbnailPath.Value = "";
            ddlMediaType.SelectedIndex = 0;
            txtCaption.Text = "";
            txtVideoUrl.Text = "";
            txtDisplayOrder.Text = "1";
            chkIsActive.Checked = true;
            imgCurrentThumbnail.ImageUrl = "";
            btnSave.Text = "Save";
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        protected void gvGallery_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();

            if (e.CommandName == "EditMedia")
            {
                LoadMediaForEdit(id);
            }
            else if (e.CommandName == "DeleteMedia")
            {
                DeleteMedia(id);
                BindGrid();
            }
        }

        private void LoadMediaForEdit(string id)
        {
            string sql = "SELECT ID, MediaType, ThumbnailPath, Caption, VideoUrl, DisplayOrder, IsActive FROM HomeGallery WHERE ID=@ID";

            SqlParameter[] param = { new SqlParameter("@ID", id) };

            DataTable dt = objDB.GetDataTable(sql, param);

            if (dt.Rows.Count == 0)
            {
                return;
            }

            DataRow dr = dt.Rows[0];

            hfID.Value = dr["ID"].ToString();
            hfExistingThumbnailPath.Value = dr["ThumbnailPath"].ToString();

            ddlMediaType.SelectedValue = dr["MediaType"].ToString();
            txtCaption.Text = dr["Caption"].ToString();
            txtVideoUrl.Text = dr["VideoUrl"] == DBNull.Value ? "" : dr["VideoUrl"].ToString();
            txtDisplayOrder.Text = dr["DisplayOrder"].ToString();
            chkIsActive.Checked = Convert.ToBoolean(dr["IsActive"]);

            string thumbnailPath = dr["ThumbnailPath"].ToString();

            if (!string.IsNullOrWhiteSpace(thumbnailPath))
            {
                imgCurrentThumbnail.ImageUrl = "~/" + thumbnailPath;
            }

            btnSave.Text = "Update Media";
        }

        private void DeleteMedia(string id)
        {
            string sql = "DELETE FROM HomeGallery WHERE ID=@ID";

            SqlParameter[] param = { new SqlParameter("@ID", id) };

            objDB.ExecuteSql(sql, param);
        }
    }
}