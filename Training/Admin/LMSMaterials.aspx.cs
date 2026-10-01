using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class LMSMaterials : System.Web.UI.Page
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
                BindCourse();
                BindGrid();
            }
        }

        private void BindCourse()
        {
            string sql = "SELECT CourseID, CourseName FROM CourseMaster ORDER BY CourseName";

            DataTable dt = objDB.GetDataTable(sql, null);

            ddlCourse.DataTextField = "CourseName";
            ddlCourse.DataValueField = "CourseID";
            ddlCourse.DataSource = dt;
            ddlCourse.DataBind();

            ddlCourse.Items.Insert(0, new ListItem("-- None --", ""));
        }

        private void BindGrid()
        {
            string sql =
                "SELECT M.ID, M.Title, M.MaterialType, ISNULL(C.CourseName,'-') AS CourseName, " +
                "M.Category, M.DisplayOrder, M.IsActive, M.FilePath, M.VideoUrl " +
                "FROM LMSMaterial M " +
                "LEFT JOIN CourseMaster C ON C.CourseID = M.CourseID " +
                "ORDER BY M.DisplayOrder, M.Title";

            DataTable dt = objDB.GetDataTable(sql, null);

            gvMaterials.DataSource = dt;
            gvMaterials.DataBind();
        }

        public string GetViewUrl(object filePath, object videoUrl)
        {
            string file = filePath == DBNull.Value ? "" : filePath.ToString();
            string video = videoUrl == DBNull.Value ? "" : videoUrl.ToString();

            if (!string.IsNullOrWhiteSpace(file))
            {
                return ResolveUrl("~/" + file);
            }

            return string.IsNullOrWhiteSpace(video) ? "#" : video;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                lblMessage.CssClass = "d-block mt-3 text-danger";
                lblMessage.Text = "Title is required.";
                return;
            }

            string materialType = ddlMaterialType.SelectedValue;

            int displayOrder;

            if (!int.TryParse(txtDisplayOrder.Text.Trim(), out displayOrder))
            {
                displayOrder = 1;
            }

            string filePath = "";
            string videoUrl = "";

            if (materialType == "Video")
            {
                if (string.IsNullOrWhiteSpace(txtVideoUrl.Text))
                {
                    lblMessage.CssClass = "d-block mt-3 text-danger";
                    lblMessage.Text = "Please enter a Video URL.";
                    return;
                }

                videoUrl = txtVideoUrl.Text.Trim();
            }
            else
            {
                try
                {
                    filePath = ResolveFilePath();
                }
                catch (Exception ex)
                {
                    lblMessage.CssClass = "d-block mt-3 text-danger";
                    lblMessage.Text = ex.Message;
                    return;
                }

                if (string.IsNullOrEmpty(filePath))
                {
                    lblMessage.CssClass = "d-block mt-3 text-danger";
                    lblMessage.Text = "Please upload a file.";
                    return;
                }
            }

            string adminID = Session["AdminID"] == null ? "Admin" : Session["AdminID"].ToString();

            bool isEditing = hfID.Value != "0";

            string sql;
            SqlParameter[] param;

            object courseIdValue = ddlCourse.SelectedValue == "" ? (object)DBNull.Value : ddlCourse.SelectedValue;

            if (isEditing)
            {
                sql =
                    "UPDATE LMSMaterial SET " +
                    "Title=@Title, Description=@Description, MaterialType=@MaterialType, " +
                    "FilePath=@FilePath, VideoUrl=@VideoUrl, CourseID=@CourseID, Category=@Category, " +
                    "DisplayOrder=@DisplayOrder, IsActive=@IsActive " +
                    "WHERE ID=@ID";

                param = new SqlParameter[]
                {
                    new SqlParameter("@Title", txtTitle.Text.Trim()),
                    new SqlParameter("@Description", string.IsNullOrWhiteSpace(txtDescription.Text) ? (object)DBNull.Value : txtDescription.Text.Trim()),
                    new SqlParameter("@MaterialType", materialType),
                    new SqlParameter("@FilePath", string.IsNullOrEmpty(filePath) ? (object)DBNull.Value : filePath),
                    new SqlParameter("@VideoUrl", string.IsNullOrEmpty(videoUrl) ? (object)DBNull.Value : videoUrl),
                    new SqlParameter("@CourseID", courseIdValue),
                    new SqlParameter("@Category", string.IsNullOrWhiteSpace(txtCategory.Text) ? (object)DBNull.Value : txtCategory.Text.Trim()),
                    new SqlParameter("@DisplayOrder", displayOrder),
                    new SqlParameter("@IsActive", chkIsActive.Checked),
                    new SqlParameter("@ID", hfID.Value)
                };
            }
            else
            {
                sql =
                    "INSERT INTO LMSMaterial (Title, Description, MaterialType, FilePath, VideoUrl, CourseID, Category, DisplayOrder, IsActive, CreatedOn, CreatedBy) " +
                    "VALUES (@Title, @Description, @MaterialType, @FilePath, @VideoUrl, @CourseID, @Category, @DisplayOrder, @IsActive, GETDATE(), @CreatedBy)";

                param = new SqlParameter[]
                {
                    new SqlParameter("@Title", txtTitle.Text.Trim()),
                    new SqlParameter("@Description", string.IsNullOrWhiteSpace(txtDescription.Text) ? (object)DBNull.Value : txtDescription.Text.Trim()),
                    new SqlParameter("@MaterialType", materialType),
                    new SqlParameter("@FilePath", string.IsNullOrEmpty(filePath) ? (object)DBNull.Value : filePath),
                    new SqlParameter("@VideoUrl", string.IsNullOrEmpty(videoUrl) ? (object)DBNull.Value : videoUrl),
                    new SqlParameter("@CourseID", courseIdValue),
                    new SqlParameter("@Category", string.IsNullOrWhiteSpace(txtCategory.Text) ? (object)DBNull.Value : txtCategory.Text.Trim()),
                    new SqlParameter("@DisplayOrder", displayOrder),
                    new SqlParameter("@IsActive", chkIsActive.Checked),
                    new SqlParameter("@CreatedBy", adminID)
                };
            }

            int rowsAffected = objDB.ExecuteSql(sql, param);

            if (rowsAffected == 0)
            {
                lblMessage.CssClass = "d-block mt-3 text-danger";
                lblMessage.Text = "Something went wrong saving this material. Please check the Error Log for details.";
                return;
            }

            lblMessage.CssClass = "d-block mt-3 text-success";
            lblMessage.Text = isEditing ? "Material updated successfully." : "Material added successfully.";

            ResetForm();
            BindGrid();
        }

        private string ResolveFilePath()
        {
            if (!fuFile.HasFile)
            {
                return hfExistingFilePath.Value;
            }

            string extension = Path.GetExtension(fuFile.FileName).ToLower();

            string[] allowedExtensions = { ".pdf", ".ppt", ".pptx", ".doc", ".docx" };

            if (Array.IndexOf(allowedExtensions, extension) < 0)
            {
                throw new Exception("File must be a PDF, PPT, PPTX, DOC, or DOCX.");
            }

            if (fuFile.PostedFile.ContentLength > 100 * 1024 * 1024)
            {
                throw new Exception("File size should not exceed 100 MB.");
            }

            string folder = Server.MapPath("~/Uploads/LMS/");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string fileName = "lms_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + extension;

            fuFile.SaveAs(folder + fileName);

            return "Uploads/LMS/" + fileName;
        }

        private void ResetForm()
        {
            hfID.Value = "0";
            hfExistingFilePath.Value = "";
            txtTitle.Text = "";
            txtDescription.Text = "";
            ddlMaterialType.SelectedIndex = 0;
            ddlCourse.SelectedIndex = 0;
            txtCategory.Text = "";
            txtVideoUrl.Text = "";
            txtDisplayOrder.Text = "1";
            chkIsActive.Checked = true;
            lblExistingFile.Text = "";
            divFileUpload.Attributes["style"] = "display:block;";
            divVideoUrl.Attributes["style"] = "display:none;";
            btnSave.Text = "Save";
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        protected void gvMaterials_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();

            if (e.CommandName == "EditMaterial")
            {
                LoadMaterialForEdit(id);
            }
            else if (e.CommandName == "DeleteMaterial")
            {
                DeleteMaterial(id);
                BindGrid();
            }
        }

        private void LoadMaterialForEdit(string id)
        {
            string sql =
                "SELECT ID, Title, Description, MaterialType, FilePath, VideoUrl, CourseID, Category, DisplayOrder, IsActive " +
                "FROM LMSMaterial WHERE ID=@ID";

            SqlParameter[] param = { new SqlParameter("@ID", id) };

            DataTable dt = objDB.GetDataTable(sql, param);

            if (dt.Rows.Count == 0)
            {
                return;
            }

            DataRow dr = dt.Rows[0];

            hfID.Value = dr["ID"].ToString();
            txtTitle.Text = dr["Title"].ToString();
            txtDescription.Text = dr["Description"] == DBNull.Value ? "" : dr["Description"].ToString();
            ddlMaterialType.SelectedValue = dr["MaterialType"].ToString();
            ddlCourse.SelectedValue = dr["CourseID"] == DBNull.Value ? "" : dr["CourseID"].ToString();
            txtCategory.Text = dr["Category"] == DBNull.Value ? "" : dr["Category"].ToString();
            txtDisplayOrder.Text = dr["DisplayOrder"].ToString();
            chkIsActive.Checked = Convert.ToBoolean(dr["IsActive"]);

            string filePath = dr["FilePath"] == DBNull.Value ? "" : dr["FilePath"].ToString();
            string videoUrl = dr["VideoUrl"] == DBNull.Value ? "" : dr["VideoUrl"].ToString();

            hfExistingFilePath.Value = filePath;

            if (dr["MaterialType"].ToString() == "Video")
            {
                divFileUpload.Attributes["style"] = "display:none;";
                divVideoUrl.Attributes["style"] = "display:block;";
                txtVideoUrl.Text = videoUrl;
            }
            else
            {
                divFileUpload.Attributes["style"] = "display:block;";
                divVideoUrl.Attributes["style"] = "display:none;";
                lblExistingFile.Text = string.IsNullOrWhiteSpace(filePath) ? "" : "Current file: " + filePath;
            }

            btnSave.Text = "Update Material";
        }

        private void DeleteMaterial(string id)
        {
            string sql = "DELETE FROM LMSMaterial WHERE ID=@ID";

            SqlParameter[] param = { new SqlParameter("@ID", id) };

            objDB.ExecuteSql(sql, param);
        }
    }
}