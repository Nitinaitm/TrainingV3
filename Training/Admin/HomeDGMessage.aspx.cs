using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Training.Admin
{
    public partial class HomeDGMessage : System.Web.UI.Page
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
                LoadMessage();
            }
        }

        private void LoadMessage()
        {
            string sql = "SELECT TOP 1 ID, Name, Designation, PhotoPath, ShortQuote, FullMessage FROM HomeDGMessage ORDER BY ID DESC";

            DataTable dt = objDB.GetDataTable(sql, null);

            if (dt.Rows.Count == 0)
            {
                ViewState["DGMessageID"] = null;
                ViewState["ExistingPhotoPath"] = "";
                return;
            }

            DataRow dr = dt.Rows[0];

            ViewState["DGMessageID"] = dr["ID"].ToString();

            txtName.Text = dr["Name"].ToString();
            txtDesignation.Text = dr["Designation"].ToString();
            txtShortQuote.Text = dr["ShortQuote"].ToString();
            txtFullMessage.Text = dr["FullMessage"].ToString();

            string photoPath = dr["PhotoPath"].ToString();

            ViewState["ExistingPhotoPath"] = photoPath;

            if (!string.IsNullOrWhiteSpace(photoPath))
            {
                imgCurrentPhoto.ImageUrl = "~/" + photoPath.TrimStart('~', '/');
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtDesignation.Text) ||
                string.IsNullOrWhiteSpace(txtShortQuote.Text) ||
                string.IsNullOrWhiteSpace(txtFullMessage.Text))
            {
                lblMessage.CssClass = "d-block mt-3 text-danger";
                lblMessage.Text = "Please fill in all fields before saving.";
                return;
            }

            string photoPath;

            try
            {
                photoPath = ResolvePhotoPath();
            }
            catch (Exception ex)
            {
                lblMessage.CssClass = "d-block mt-3 text-danger";
                lblMessage.Text = ex.Message;
                return;
            }

            string adminID = Session["AdminID"] == null ? "Admin" : Session["AdminID"].ToString();

            bool exists = ViewState["DGMessageID"] != null;

            string sql;

            SqlParameter[] param;

            if (exists)
            {
                sql =
                    "UPDATE HomeDGMessage " +
                    "SET Name=@Name, Designation=@Designation, PhotoPath=@PhotoPath, " +
                    "ShortQuote=@ShortQuote, FullMessage=@FullMessage, UpdatedOn=GETDATE(), UpdatedBy=@UpdatedBy " +
                    "WHERE ID=@ID";

                param = new SqlParameter[]
                {
                    new SqlParameter("@Name", txtName.Text.Trim()),
                    new SqlParameter("@Designation", txtDesignation.Text.Trim()),
                    new SqlParameter("@PhotoPath", photoPath),
                    new SqlParameter("@ShortQuote", txtShortQuote.Text.Trim()),
                    new SqlParameter("@FullMessage", txtFullMessage.Text.Trim()),
                    new SqlParameter("@UpdatedBy", adminID),
                    new SqlParameter("@ID", ViewState["DGMessageID"])
                };
            }
            else
            {
                sql =
                    "INSERT INTO HomeDGMessage (Name, Designation, PhotoPath, ShortQuote, FullMessage, UpdatedOn, UpdatedBy) " +
                    "VALUES (@Name, @Designation, @PhotoPath, @ShortQuote, @FullMessage, GETDATE(), @UpdatedBy)";

                param = new SqlParameter[]
                {
                    new SqlParameter("@Name", txtName.Text.Trim()),
                    new SqlParameter("@Designation", txtDesignation.Text.Trim()),
                    new SqlParameter("@PhotoPath", photoPath),
                    new SqlParameter("@ShortQuote", txtShortQuote.Text.Trim()),
                    new SqlParameter("@FullMessage", txtFullMessage.Text.Trim()),
                    new SqlParameter("@UpdatedBy", adminID)
                };
            }

            objDB.ExecuteSql(sql, param);

            lblMessage.CssClass = "d-block mt-3 text-success";
            lblMessage.Text = "Saved successfully.";

            LoadMessage();
        }

        private string ResolvePhotoPath()
        {
            if (!fuPhoto.HasFile)
            {
                return ViewState["ExistingPhotoPath"] == null ? "" : ViewState["ExistingPhotoPath"].ToString();
            }

            string extension = Path.GetExtension(fuPhoto.FileName).ToLower();

            if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
            {
                throw new Exception("Photo must be a JPG or PNG file.");
            }

            if (fuPhoto.PostedFile.ContentLength > 2 * 1024 * 1024)
            {
                throw new Exception("Photo size should not exceed 2 MB.");
            }

            string folder = Server.MapPath("~/Uploads/HomePage/DGMessage/");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string fileName = "dg_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + extension;

            fuPhoto.SaveAs(folder + fileName);

            return "Uploads/HomePage/DGMessage/" + fileName;
        }
    }
}