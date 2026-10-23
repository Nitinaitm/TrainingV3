using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class RoleMaster : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string role = Convert.ToString(Session["Role"]);
            if (!role.Equals("Admin", StringComparison.OrdinalIgnoreCase) && !role.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }
            if (!IsPostBack) BindRoles();
        }

        private clsDataAccess DB()
        {
            return new clsDataAccess();
        }

        private void BindRoles()
        {
            DataTable dt = DB().GetDataTable("SELECT RoleID,RoleName,IsActive,CreatedOn FROM dbo.RoleMaster ORDER BY RoleName");
            gvRoles.DataSource = dt;
            gvRoles.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string roleName = (txtRoleName.Text ?? "").Trim();
            if (roleName.Length == 0)
            {
                ShowMessage("Enter a role name.", false);
                return;
            }
            int existing = Convert.ToInt32(DB().ExecuteScalar("SELECT COUNT(*) FROM dbo.RoleMaster WHERE RoleName=@RoleName", new SqlParameter[] { new SqlParameter("@RoleName", roleName) }));
            if (existing > 0)
            {
                ShowMessage("This role already exists.", false);
                return;
            }
            int affected = DB().ExecuteSql("INSERT INTO dbo.RoleMaster(RoleName,IsActive,CreatedOn) VALUES(@RoleName,1,GETDATE())", new SqlParameter[] { new SqlParameter("@RoleName", roleName) });
            if (affected <= 0)
            {
                ShowMessage("Role was not saved. Check the application error log and database migration.", false);
                return;
            }
            txtRoleName.Text = "";
            ShowMessage("Role added successfully.", true);
            BindRoles();
        }

        protected void gvRoles_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!string.Equals(e.CommandName, "ToggleRole", StringComparison.OrdinalIgnoreCase)) return;
            int roleID;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out roleID)) return;
            int affected = DB().ExecuteSql("UPDATE dbo.RoleMaster SET IsActive=CASE WHEN IsActive=1 THEN 0 ELSE 1 END WHERE RoleID=@RoleID", new SqlParameter[] { new SqlParameter("@RoleID", roleID) });
            ShowMessage(affected > 0 ? "Role status updated." : "Role status could not be updated.", affected > 0);
            BindRoles();
        }

        private void ShowMessage(string message, bool success)
        {
            lblMessage.ForeColor = success ? System.Drawing.Color.Green : System.Drawing.Color.Red;
            lblMessage.Text = message;
        }
    }
}
