using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Training.SuperAdmin
{
    public partial class UserManagement : System.Web.UI.Page
    {
        private readonly clsDataAccess db = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsSuperAdmin())
            {
                Response.Redirect("~/Default.aspx");
                return;
            }
            if (!IsPostBack)
            {
                BindRoles();
                BindEditRoles();
                gvUsers.DataSource = null;
                gvUsers.DataBind();
            }
        }

        private bool IsSuperAdmin()
        {
            return Session["Role"] != null && string.Equals(Session["Role"].ToString(), "SuperAdmin", StringComparison.OrdinalIgnoreCase);
        }

        private string CurrentUser()
        {
            return Session["UserID"] == null ? "" : Session["UserID"].ToString();
        }

        private void BindRoles()
        {
            ddlRoleFilter.Items.Clear();
            ddlRoleFilter.Items.Add(new ListItem("-- Select Role --", ""));
            ddlRoleFilter.Items.Add(new ListItem("Admin", "Admin"));
            ddlRoleFilter.Items.Add(new ListItem("Manager", "Manager"));
            ddlRoleFilter.Items.Add(new ListItem("Trainer", "Trainer"));
            ddlRoleFilter.Items.Add(new ListItem("Trainee", "Trainee"));
            ddlRoleFilter.Items.Add(new ListItem("Super Admin", "SuperAdmin"));
        }

        private void BindEditRoles()
        {
            ddlRole.Items.Clear();
            ddlRole.Items.Add(new ListItem("Admin", "Admin"));
            ddlRole.Items.Add(new ListItem("Manager", "Manager"));
            ddlRole.Items.Add(new ListItem("Trainer", "Trainer"));
            ddlRole.Items.Add(new ListItem("Trainee", "Trainee"));
            ddlRole.Items.Add(new ListItem("Super Admin", "SuperAdmin"));
        }

        private void BindUsers()
        {
            string role = ddlRoleFilter.SelectedValue;
            if (string.IsNullOrEmpty(role))
            {
                gvUsers.DataSource = null;
                gvUsers.DataBind();
                return;
            }

            string search = txtSearch.Text.Trim();
            string sql = "SELECT L.LoginIDUserID,L.Role,L.CorrespondingEmpID,L.Password,L.Active,(SELECT MAX(H.LoginTime) FROM UserLoginHistory H WHERE H.UserID=L.LoginIDUserID AND H.LoginStatus='Success') AS LastLogin FROM Login L WHERE L.Role=@Role";
            if (search != "")
                sql += " AND (L.LoginIDUserID LIKE @Search OR L.CorrespondingEmpID LIKE @Search)";
            sql += " ORDER BY L.LoginIDUserID";

            SqlParameter[] parameters = search == ""
                ? new SqlParameter[] { new SqlParameter("@Role", role) }
                : new SqlParameter[] { new SqlParameter("@Role", role), new SqlParameter("@Search", "%" + search + "%") };

            DataTable dt = db.GetDataTable(sql, parameters);
            if (!dt.Columns.Contains("PasswordPlain"))
                dt.Columns.Add("PasswordPlain", typeof(string));
            Encryptor2 encryptor = new Encryptor2();
            foreach (DataRow row in dt.Rows)
            {
                try
                {
                    row["PasswordPlain"] = encryptor.Decrypt(Convert.ToString(row["Password"]));
                }
                catch
                {
                    row["PasswordPlain"] = "[Unable to decrypt]";
                }
            }
            gvUsers.DataSource = dt;
            gvUsers.DataBind();
        }

        private void SetMessage(string message, bool success)
        {
            lblMessage.ForeColor = success ? System.Drawing.Color.Green : System.Drawing.Color.Red;
            lblMessage.Text = message;
        }

        private bool UserExists(string loginID)
        {
            object value = db.ExecuteScalar("SELECT COUNT(*) FROM Login WHERE LoginIDUserID=@LoginID", new SqlParameter[] { new SqlParameter("@LoginID", loginID) });
            return value != null && Convert.ToInt32(value) > 0;
        }

        private bool ValidateCorrespondingID(string role, string correspondingID)
        {
            if (role == "SuperAdmin" || role == "Admin")
                return Exists("SELECT COUNT(*) FROM EmpBasicMaster WHERE EmpID=@ID", correspondingID);
            if (role == "Manager")
                return Exists("SELECT COUNT(*) FROM ManagerMaster WHERE ManagerID=@ID AND ISNULL(ActiveStatus,'Y')='Y'", correspondingID);
            if (role == "Trainer")
                return Exists("SELECT COUNT(*) FROM TrainerMaster WHERE TrainerID=@ID", correspondingID);
            if (role == "Trainee")
                return Exists("SELECT COUNT(*) FROM EmpBasicMaster WHERE EmpID=@ID", correspondingID) || Exists("SELECT COUNT(*) FROM TraineeMasterExternal WHERE TraineeID=@ID", correspondingID);
            return false;
        }

        private bool Exists(string sql, string id)
        {
            object value = db.ExecuteScalar(sql, new SqlParameter[] { new SqlParameter("@ID", id) });
            return value != null && Convert.ToInt32(value) > 0;
        }

        protected void btnLoadUsers_Click(object sender, EventArgs e)
        {
            editCard.Visible = false;
            BindUsers();
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string loginID = Convert.ToString(e.CommandArgument);
            if (string.IsNullOrEmpty(loginID)) return;

            if (e.CommandName == "EditUser")
            {
                DataTable dt = db.GetDataTable("SELECT LoginIDUserID,Role,CorrespondingEmpID,Active FROM Login WHERE LoginIDUserID=@LoginID", new SqlParameter[] { new SqlParameter("@LoginID", loginID) });
                if (dt.Rows.Count == 0) return;
                txtLoginID.Text = dt.Rows[0]["LoginIDUserID"].ToString();
                ddlRole.SelectedValue = dt.Rows[0]["Role"].ToString();
                txtCorrespondingID.Text = dt.Rows[0]["CorrespondingEmpID"].ToString();
                ddlActive.SelectedValue = dt.Rows[0]["Active"].ToString() == "N" ? "N" : "Y";
                editCard.Visible = true;
                SetMessage("User loaded for editing.", true);
                return;
            }

            if (e.CommandName == "ChangePassword")
            {
                DataTable dt = db.GetDataTable("SELECT LoginIDUserID,Role,CorrespondingEmpID,Password,Active FROM Login WHERE LoginIDUserID=@LoginID", new SqlParameter[] { new SqlParameter("@LoginID", loginID) });
                if (dt.Rows.Count == 0) return;
                txtLoginID.Text = dt.Rows[0]["LoginIDUserID"].ToString();
                ddlRole.SelectedValue = dt.Rows[0]["Role"].ToString();
                txtCorrespondingID.Text = dt.Rows[0]["CorrespondingEmpID"].ToString();
                ddlActive.SelectedValue = dt.Rows[0]["Active"].ToString() == "N" ? "N" : "Y";
                txtPassword.Text = "";
                txtConfirmPassword.Text = "";
                editCard.Visible = true;
                SetMessage("User loaded. Enter the new password and save changes.", true);
                return;
            }

            if (e.CommandName == "ToggleStatus")
            {
                if (string.Equals(loginID, CurrentUser(), StringComparison.OrdinalIgnoreCase))
                {
                    SetMessage("SuperAdmin cannot make its own account inactive.", false);
                    BindUsers();
                    return;
                }
                string active = Convert.ToString(db.ExecuteScalar("SELECT Active FROM Login WHERE LoginIDUserID=@LoginID", new SqlParameter[] { new SqlParameter("@LoginID", loginID) }));
                string newStatus = active == "Y" ? "N" : "Y";
                db.ExecuteSql("UPDATE Login SET Active=@Active WHERE LoginIDUserID=@LoginID", new SqlParameter[] { new SqlParameter("@Active", newStatus), new SqlParameter("@LoginID", loginID) });
                clsAuditLog.LogActivity(CurrentUser(), "SuperAdmin", newStatus == "Y" ? "ACTIVATE" : "INACTIVATE", "UserManagement", "UserManagement.aspx", "User", loginID, newStatus == "Y" ? "User account activated" : "User account made inactive");
                SetMessage(newStatus == "Y" ? "User activated successfully." : "User made inactive successfully.", true);
                BindUsers();
                return;
            }

            if (e.CommandName == "DeleteUser")
            {
                if (string.Equals(loginID, CurrentUser(), StringComparison.OrdinalIgnoreCase))
                {
                    SetMessage("SuperAdmin cannot delete its own account.", false);
                    return;
                }
                if (!UserExists(loginID))
                {
                    SetMessage("User not found.", false);
                    return;
                }
                DataTable oldDelete = db.GetDataTable("SELECT LoginIDUserID,Role,CorrespondingEmpID,Active FROM Login WHERE LoginIDUserID=@LoginID", new SqlParameter[] { new SqlParameter("@LoginID", loginID) });
                string deleteDetails = oldDelete.Rows.Count == 0 ? "User account deleted" : "Deleted values: Role=" + Convert.ToString(oldDelete.Rows[0]["Role"]) + "; Corresponding ID=" + Convert.ToString(oldDelete.Rows[0]["CorrespondingEmpID"]) + "; Active=" + Convert.ToString(oldDelete.Rows[0]["Active"]);
                db.ExecuteSql("DELETE FROM Login WHERE LoginIDUserID=@LoginID", new SqlParameter[] { new SqlParameter("@LoginID", loginID) });
                clsAuditLog.LogActivity(CurrentUser(), "SuperAdmin", "DELETE", "UserManagement", "UserManagement.aspx", "User", loginID, deleteDetails);
                SetMessage("User deleted successfully.", true);
                BindUsers();
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            string loginID = txtLoginID.Text.Trim().ToUpperInvariant();
            string role = ddlRole.SelectedValue;
            string correspondingID = txtCorrespondingID.Text.Trim().ToUpperInvariant();

            if (loginID == "" || correspondingID == "")
            {
                SetMessage("Login ID and Corresponding ID are required.", false);
                return;
            }
            if (!ValidateCorrespondingID(role, correspondingID))
            {
                SetMessage("Corresponding ID does not exist for the selected role.", false);
                return;
            }

            DataTable oldUser = db.GetDataTable("SELECT Role,CorrespondingEmpID,Active FROM Login WHERE LoginIDUserID=@LoginID", new SqlParameter[] { new SqlParameter("@LoginID", loginID) });
            if (oldUser.Rows.Count == 0)
            {
                SetMessage("User not found.", false);
                return;
            }

            string oldRole = Convert.ToString(oldUser.Rows[0]["Role"]);
            string oldCorrespondingID = Convert.ToString(oldUser.Rows[0]["CorrespondingEmpID"]);
            string oldActive = Convert.ToString(oldUser.Rows[0]["Active"]);
            string newActive = ddlActive.SelectedValue;

            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;
            if (password != "" || confirmPassword != "")
            {
                if (password != confirmPassword)
                {
                    SetMessage("Password and confirmation do not match.", false);
                    return;
                }
            }

            string changes = "";
            if (!string.Equals(oldRole, role, StringComparison.OrdinalIgnoreCase))
                changes += "Role: " + oldRole + " -> " + role + "; ";
            if (!string.Equals(oldCorrespondingID, correspondingID, StringComparison.OrdinalIgnoreCase))
                changes += "Corresponding ID: " + oldCorrespondingID + " -> " + correspondingID + "; ";
            if (!string.Equals(oldActive, newActive, StringComparison.OrdinalIgnoreCase))
                changes += "Active: " + oldActive + " -> " + newActive + "; ";

            string sql = "UPDATE Login SET Role=@Role,CorrespondingEmpID=@CorrespondingID,Active=@Active";
            if (password != "")
                sql += ",Password=@Password,re=@re";
            sql += " WHERE LoginIDUserID=@LoginID";

            if (password == "")
            {
                db.ExecuteSql(sql, new SqlParameter[] {
                    new SqlParameter("@Role", role),
                    new SqlParameter("@CorrespondingID", correspondingID),
                    new SqlParameter("@Active", newActive),
                    new SqlParameter("@LoginID", loginID)
                });
            }
            else
            {
                Encryptor2 encryptor = new Encryptor2();
                db.ExecuteSql(sql, new SqlParameter[] {
                    new SqlParameter("@Role", role),
                    new SqlParameter("@CorrespondingID", correspondingID),
                    new SqlParameter("@Active", newActive),
                    new SqlParameter("@Password", encryptor.Encrypt(password)),
                    new SqlParameter("@re", encryptor.Encrypt("Y")),
                    new SqlParameter("@LoginID", loginID)
                });
                clsAuditLog.LogActivity(CurrentUser(), "SuperAdmin", "PASSWORD_CHANGE", "UserManagement", "UserManagement.aspx", "User", loginID, "Password changed by SuperAdmin");
            }

            if (changes == "")
                changes = "No profile field changed.";
            clsAuditLog.LogActivity(CurrentUser(), "SuperAdmin", "UPDATE", "UserManagement", "UserManagement.aspx", "User", loginID, "Changed fields: " + changes);
            SetMessage("User updated successfully.", true);
            editCard.Visible = false;
            BindUsers();
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            editCard.Visible = false;
            txtLoginID.Text = "";
            txtCorrespondingID.Text = "";
            txtPassword.Text = "";
            txtConfirmPassword.Text = "";
        }
    }
}