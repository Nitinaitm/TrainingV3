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
                BindUsers();
                BindLoginHistory();
                BindActivity();
            }
        }

        private bool IsSuperAdmin()
        {
            return Session["Role"] != null &&
                   string.Equals(Session["Role"].ToString(), "SuperAdmin", StringComparison.OrdinalIgnoreCase);
        }

        private string CurrentUser()
        {
            return Session["UserID"] == null ? "" : Session["UserID"].ToString();
        }

        private void BindRoles()
        {
            ddlRole.Items.Clear();
            ddlRole.Items.Add(new ListItem("Admin", "Admin"));
            ddlRole.Items.Add(new ListItem("Manager", "Manager"));
            ddlRole.Items.Add(new ListItem("Trainer", "Trainer"));
            ddlRole.Items.Add(new ListItem("Trainee", "Trainee"));
            ddlRole.Items.Add(new ListItem("SuperAdmin", "SuperAdmin"));
        }

        private void BindUsers()
        {
            string sql = "SELECT L.LoginIDUserID,L.Role,L.CorrespondingEmpID,L.Active,(SELECT MAX(H.LoginTime) FROM UserLoginHistory H WHERE H.UserID=L.LoginIDUserID AND H.LoginStatus='Success') AS LastLogin FROM Login L ORDER BY L.Role,L.LoginIDUserID";
            gvUsers.DataSource = db.GetDataTable(sql);
            gvUsers.DataBind();
        }

        private void BindLoginHistory()
        {
            string sql = "SELECT TOP 500 LoginHistoryID,UserID,UserRole,LoginTime,LogoutTime,LoginStatus,FailureReason,IPAddress,SessionID FROM UserLoginHistory ORDER BY LoginTime DESC";
            gvLoginHistory.DataSource = db.GetDataTable(sql);
            gvLoginHistory.DataBind();
        }

        private void BindActivity()
        {
            string sql = "SELECT TOP 500 ActivityID,UserID,UserRole,ActionType,Module,PageName,RecordType,RecordID,Description,ActivityTime,IPAddress,SessionID FROM UserActivityLog ORDER BY ActivityTime DESC";
            gvActivity.DataSource = db.GetDataTable(sql);
            gvActivity.DataBind();
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
                return Exists("SELECT COUNT(*) FROM EmpBasicMaster WHERE EmpID=@ID", correspondingID) ||
                       Exists("SELECT COUNT(*) FROM TraineeMasterExternal WHERE TraineeID=@ID", correspondingID);

            return false;
        }

        private bool Exists(string sql, string id)
        {
            object value = db.ExecuteScalar(sql, new SqlParameter[] { new SqlParameter("@ID", id) });
            return value != null && Convert.ToInt32(value) > 0;
        }

        protected void btnSave_Click(object sender, EventArgs e)
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

            bool exists = UserExists(loginID);

            if (!exists)
            {
                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    SetMessage("Password is required for a new user.", false);
                    return;
                }

                Encryptor2 enc = new Encryptor2();
                string encrypted = enc.Encrypt(txtPassword.Text.Trim());
                string re = enc.Encrypt("Y");
                string sql = "INSERT INTO Login(LoginIDUserID,Role,CorrespondingEmpID,Password,Active,re) VALUES(@LoginID,@Role,@CorrespondingID,@Password,@Active,@re)";
                db.ExecuteSql(sql, new SqlParameter[] {
                    new SqlParameter("@LoginID",loginID),
                    new SqlParameter("@Role",role),
                    new SqlParameter("@CorrespondingID",correspondingID),
                    new SqlParameter("@Password",encrypted),
                    new SqlParameter("@Active",ddlActive.SelectedValue),
                    new SqlParameter("@re",re)
                });
                clsAuditLog.LogActivity(CurrentUser(),"SuperAdmin","CREATE","UserManagement","UserManagement.aspx","User",loginID,"Created "+role+" user");
                SetMessage("User created successfully. First login will require password reset.", true);
            }
            else
            {
                string sql = "UPDATE Login SET Role=@Role,CorrespondingEmpID=@CorrespondingID,Active=@Active WHERE LoginIDUserID=@LoginID";
                db.ExecuteSql(sql, new SqlParameter[] {
                    new SqlParameter("@Role",role),
                    new SqlParameter("@CorrespondingID",correspondingID),
                    new SqlParameter("@Active",ddlActive.SelectedValue),
                    new SqlParameter("@LoginID",loginID)
                });

                if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    Encryptor2 enc = new Encryptor2();
                    db.ExecuteSql("UPDATE Login SET Password=@Password,re=@re WHERE LoginIDUserID=@LoginID", new SqlParameter[] {
                        new SqlParameter("@Password",enc.Encrypt(txtPassword.Text.Trim())),
                        new SqlParameter("@re",enc.Encrypt("Y")),
                        new SqlParameter("@LoginID",loginID)
                    });
                }

                clsAuditLog.LogActivity(CurrentUser(),"SuperAdmin","UPDATE","UserManagement","UserManagement.aspx","User",loginID,"Updated "+role+" user");
                SetMessage("User updated successfully.", true);
            }

            BindUsers();
            BindLoginHistory();
            BindActivity();
        }

        protected void btnPassword_Click(object sender, EventArgs e)
        {
            string loginID = txtLoginID.Text.Trim().ToUpperInvariant();
            string password = txtPassword.Text.Trim();

            if (loginID == "" || password == "")
            {
                SetMessage("Login ID and new password are required.", false);
                return;
            }

            if (!UserExists(loginID))
            {
                SetMessage("User not found.", false);
                return;
            }

            if (string.Equals(loginID, CurrentUser(), StringComparison.OrdinalIgnoreCase))
            {
                SetMessage("Use the normal password reset/change flow for your own SuperAdmin account.", false);
                return;
            }

            Encryptor2 enc = new Encryptor2();
            db.ExecuteSql("UPDATE Login SET Password=@Password,re=@re WHERE LoginIDUserID=@LoginID", new SqlParameter[] {
                new SqlParameter("@Password",enc.Encrypt(password)),
                new SqlParameter("@re",enc.Encrypt("Y")),
                new SqlParameter("@LoginID",loginID)
            });

            clsAuditLog.LogActivity(CurrentUser(),"SuperAdmin","PASSWORD_CHANGE","UserManagement","UserManagement.aspx","User",loginID,"Password changed by SuperAdmin");
            SetMessage("Password changed successfully.", true);
            BindUsers();
            BindLoginHistory();
            BindActivity();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            string loginID = txtLoginID.Text.Trim().ToUpperInvariant();

            if (loginID == "")
            {
                SetMessage("Select a user first.", false);
                return;
            }

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

            db.ExecuteSql("UPDATE Login SET Active='N' WHERE LoginIDUserID=@LoginID", new SqlParameter[] { new SqlParameter("@LoginID",loginID) });
            clsAuditLog.LogActivity(CurrentUser(),"SuperAdmin","DISABLE","UserManagement","UserManagement.aspx","User",loginID,"User account disabled");
            SetMessage("User disabled successfully.", true);
            BindUsers();
            BindLoginHistory();
            BindActivity();
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "EditUser") return;

            int index = Convert.ToInt32(e.CommandArgument);
            string loginID = gvUsers.DataKeys[index].Value.ToString();
            DataTable dt = db.GetDataTable("SELECT LoginIDUserID,Role,CorrespondingEmpID,Active FROM Login WHERE LoginIDUserID=@LoginID", new SqlParameter[] { new SqlParameter("@LoginID",loginID) });

            if (dt.Rows.Count == 0) return;

            txtLoginID.Text = dt.Rows[0]["LoginIDUserID"].ToString();
            ddlRole.SelectedValue = dt.Rows[0]["Role"].ToString();
            txtCorrespondingID.Text = dt.Rows[0]["CorrespondingEmpID"].ToString();
            ddlActive.SelectedValue = dt.Rows[0]["Active"].ToString() == "N" ? "N" : "Y";
            txtPassword.Text = "";
            SetMessage("User loaded for update.", true);
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtLoginID.Text = "";
            txtCorrespondingID.Text = "";
            txtPassword.Text = "";
            ddlRole.SelectedIndex = 0;
            ddlActive.SelectedValue = "Y";
            lblMessage.Text = "";
        }
    }
}