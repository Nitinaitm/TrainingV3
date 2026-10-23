using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class RoleMaster : Page
    {
        private clsDataAccess DB()
        {
            return new clsDataAccess();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindRoles();
            }
        }

        private void BindRoles()
        {
            DataTable dt = DB().GetDataTable("SELECT RoleID,RoleName,IsActive FROM dbo.RoleMaster ORDER BY RoleName");
            gvRoles.DataSource = dt;
            gvRoles.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string roleName = txtRoleName.Text.Trim();
            if (roleName.Length == 0)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Enter a role name.";
                return;
            }

            int exists = Convert.ToInt32(DB().ExecuteScalar("SELECT COUNT(*) FROM dbo.RoleMaster WHERE RoleName=@RoleName", new SqlParameter[] { new SqlParameter("@RoleName", roleName) }));
            if (exists > 0)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "This role already exists.";
                return;
            }

            DB().ExecuteSql("INSERT INTO dbo.RoleMaster(RoleName,IsActive,CreatedOn) VALUES(@RoleName,1,GETDATE())", new SqlParameter[] { new SqlParameter("@RoleName", roleName) });
            txtRoleName.Text = "";
            lblMessage.ForeColor = System.Drawing.Color.Green;
            lblMessage.Text = "Role added successfully.";
            BindRoles();
        }

        protected void gvRoles_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "ToggleRole")
            {
                return;
            }

            int roleID;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out roleID))
            {
                return;
            }

            DB().ExecuteSql("UPDATE dbo.RoleMaster SET IsActive=CASE WHEN IsActive=1 THEN 0 ELSE 1 END WHERE RoleID=@RoleID", new SqlParameter[] { new SqlParameter("@RoleID", roleID) });
            BindRoles();
        }
    }
}