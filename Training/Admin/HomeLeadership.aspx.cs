using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class HomeLeadership : System.Web.UI.Page
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
            string sql = "SELECT ID, Name, Designation, PhotoPath, ProfileUrl, DisplayOrder, IsActive FROM HomeLeadership ORDER BY DisplayOrder";

            DataTable dt = objDB.GetDataTable(sql, null);

            gvLeadership.DataSource = dt;
            gvLeadership.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtDesignation.Text))
            {
                lblMessage.CssClass = "d-block mt-3 text-danger";
                lblMessage.Text = "Name and Designation are required.";
                return;
            }

            int displayOrder;

            if (!int.TryParse(txtDisplayOrder.Text.Trim(), out displayOrder))
            {
                displayOrder = 1;
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

            bool isEditing = hfID.Value != "0";

            string sql;
            SqlParameter[] param;

            if (isEditing)
            {
                sql =
                    "UPDATE HomeLeadership " +
                    "SET Name=@Name, Designation=@Designation, PhotoPath=@PhotoPath, ProfileUrl=@ProfileUrl, " +
                    "DisplayOrder=@DisplayOrder, IsActive=@IsActive " +
                    "WHERE ID=@ID";

                param = new SqlParameter[]
                {
                    new SqlParameter("@Name", txtName.Text.Trim()),
                    new SqlParameter("@Designation", txtDesignation.Text.Trim()),
                    new SqlParameter("@PhotoPath", photoPath),
                    new SqlParameter("@ProfileUrl", string.IsNullOrWhiteSpace(txtProfileUrl.Text) ? "#" : txtProfileUrl.Text.Trim()),
                    new SqlParameter("@DisplayOrder", displayOrder),
                    new SqlParameter("@IsActive", chkIsActive.Checked),
                    new SqlParameter("@ID", hfID.Value)
                };
            }
            else
            {
                sql =
                    "INSERT INTO HomeLeadership (Name, Designation, PhotoPath, ProfileUrl, DisplayOrder, IsActive, CreatedOn, CreatedBy) " +
                    "VALUES (@Name, @Designation, @PhotoPath, @ProfileUrl, @DisplayOrder, @IsActive, GETDATE(), @CreatedBy)";

                param = new SqlParameter[]
                {
                    new SqlParameter("@Name", txtName.Text.Trim()),
                    new SqlParameter("@Designation", txtDesignation.Text.Trim()),
                    new SqlParameter("@PhotoPath", photoPath),
                    new SqlParameter("@ProfileUrl", string.IsNullOrWhiteSpace(txtProfileUrl.Text) ? "#" : txtProfileUrl.Text.Trim()),
                    new SqlParameter("@DisplayOrder", displayOrder),
                    new SqlParameter("@IsActive", chkIsActive.Checked),
                    new SqlParameter("@CreatedBy", adminID)
                };
            }

            objDB.ExecuteSql(sql, param);

            lblMessage.CssClass = "d-block mt-3 text-success";
            lblMessage.Text = isEditing ? "Member updated successfully." : "Member added successfully.";

            ResetForm();
            BindGrid();
        }

        private string ResolvePhotoPath()
        {
            if (!fuPhoto.HasFile)
            {
                return hfExistingPhotoPath.Value;
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

            string folder = Server.MapPath("~/Uploads/HomePage/Leadership/");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string fileName = "member_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + extension;

            fuPhoto.SaveAs(folder + fileName);

            return "Uploads/HomePage/Leadership/" + fileName;
        }

        private void ResetForm()
        {
            hfID.Value = "0";
            hfExistingPhotoPath.Value = "";
            txtName.Text = "";
            txtDesignation.Text = "";
            txtProfileUrl.Text = "";
            txtDisplayOrder.Text = "1";
            chkIsActive.Checked = true;
            imgCurrentPhoto.ImageUrl = "";
            btnSave.Text = "Save";
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        protected void gvLeadership_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();

            if (e.CommandName == "EditMember")
            {
                LoadMemberForEdit(id);
            }
            else if (e.CommandName == "DeleteMember")
            {
                DeleteMember(id);
                BindGrid();
            }
        }

        private void LoadMemberForEdit(string id)
        {
            string sql = "SELECT ID, Name, Designation, PhotoPath, ProfileUrl, DisplayOrder, IsActive FROM HomeLeadership WHERE ID=@ID";

            SqlParameter[] param = { new SqlParameter("@ID", id) };

            DataTable dt = objDB.GetDataTable(sql, param);

            if (dt.Rows.Count == 0)
            {
                return;
            }

            DataRow dr = dt.Rows[0];

            hfID.Value = dr["ID"].ToString();
            hfExistingPhotoPath.Value = dr["PhotoPath"].ToString();

            txtName.Text = dr["Name"].ToString();
            txtDesignation.Text = dr["Designation"].ToString();
            txtProfileUrl.Text = dr["ProfileUrl"].ToString();
            txtDisplayOrder.Text = dr["DisplayOrder"].ToString();
            chkIsActive.Checked = Convert.ToBoolean(dr["IsActive"]);

            string photoPath = dr["PhotoPath"].ToString();

            if (!string.IsNullOrWhiteSpace(photoPath))
            {
                imgCurrentPhoto.ImageUrl = "~/" + photoPath;
            }

            btnSave.Text = "Update Member";
        }

        private void DeleteMember(string id)
        {
            string sql = "DELETE FROM HomeLeadership WHERE ID=@ID";

            SqlParameter[] param = { new SqlParameter("@ID", id) };

            objDB.ExecuteSql(sql, param);
        }
    }
}