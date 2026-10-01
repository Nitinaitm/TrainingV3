using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Training.Trainee
{
    public partial class SubmitBlog : System.Web.UI.Page
    {
        clsDataAccess objDB = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["EmpID"] == null)
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSubmitterName();
            }
        }

        private void LoadSubmitterName()
        {
            string empID = Session["EmpID"].ToString().ToUpperInvariant();

            string sql = "SELECT EmpName FROM EmpBasicMaster WHERE EmpID=@EmpID";

            SqlParameter[] param = { new SqlParameter("@EmpID", empID) };

            object result = objDB.ExecuteScalar(sql, param);

            lblSubmittingAs.Text = (result == null || result == DBNull.Value)
                ? empID
                : result.ToString() + " (" + empID + ")";
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
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

            string empID = Session["EmpID"].ToString().ToUpperInvariant();

            string sql =
                "INSERT INTO HomeBlog (Category, Title, Excerpt, FullContent, ImagePath, Url, Status, IsActive, SubmittedBy, DisplayOrder, PublishedOn, CreatedOn) " +
                "VALUES (@Category, @Title, @Excerpt, @FullContent, @ImagePath, @Url, 'Pending', 0, @SubmittedBy, 1, GETDATE(), GETDATE())";

            SqlParameter[] param =
            {
                new SqlParameter("@Category", txtCategory.Text.Trim()),
                new SqlParameter("@Title", txtTitle.Text.Trim()),
                new SqlParameter("@Excerpt", txtExcerpt.Text.Trim()),
                new SqlParameter("@FullContent", string.IsNullOrWhiteSpace(txtFullContent.Text) ? (object)DBNull.Value : txtFullContent.Text.Trim()),
                new SqlParameter("@ImagePath", string.IsNullOrEmpty(imagePath) ? (object)DBNull.Value : imagePath),
                new SqlParameter("@Url", string.IsNullOrWhiteSpace(txtUrl.Text) ? (object)DBNull.Value : txtUrl.Text.Trim()),
                new SqlParameter("@SubmittedBy", empID)
            };

            int rowsAffected = objDB.ExecuteSql(sql, param);

            if (rowsAffected > 0)
            {
                lblMessage.CssClass = "d-block mt-3 text-success fw-semibold";
                lblMessage.Text = "Thank you - your blog post has been submitted and will appear once reviewed.";

                ClearForm();
            }
            else
            {
                lblMessage.CssClass = "d-block mt-3 text-danger fw-semibold";
                lblMessage.Text = "Something went wrong. Please try again.";
            }
        }

        private string ResolveImagePath()
        {
            if (!fuImage.HasFile)
            {
                return "";
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

        private void ClearForm()
        {
            txtCategory.Text = "";
            txtTitle.Text = "";
            txtExcerpt.Text = "";
            txtFullContent.Text = "";
            txtUrl.Text = "";
        }
    }
}