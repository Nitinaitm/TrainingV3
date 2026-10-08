using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Training.SuperAdmin
{
    public partial class CreateUser : System.Web.UI.Page
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
                BindRoles();
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
            ddlRole.Items.Clear();
            ddlRole.Items.Add(new ListItem("Admin", "Admin"));
            ddlRole.Items.Add(new ListItem("Manager", "Manager"));
            ddlRole.Items.Add(new ListItem("Trainer", "Trainer"));
            ddlRole.Items.Add(new ListItem("Trainee", "Trainee"));
            ddlRole.Items.Add(new ListItem("Super Admin", "SuperAdmin"));
        }

        private bool Exists(string sql, string id)
        {
            object value = db.ExecuteScalar(sql, new SqlParameter[] { new SqlParameter("@ID", id) });
            return value != null && Convert.ToInt32(value) > 0;
        }

        private bool ValidateCorrespondingID(string role, string id)
        {
            if (role == "SuperAdmin" || role == "Admin")
                return Exists("SELECT COUNT(*) FROM EmpBasicMaster WHERE EmpID=@ID", id);
            if (role == "Manager")
                return Exists("SELECT COUNT(*) FROM ManagerMaster WHERE ManagerID=@ID AND ISNULL(ActiveStatus,'Y')='Y'", id);
            if (role == "Trainer")
                return Exists("SELECT COUNT(*) FROM TrainerMaster WHERE TrainerID=@ID", id);
            if (role == "Trainee")
                return Exists("SELECT COUNT(*) FROM EmpBasicMaster WHERE EmpID=@ID", id) || Exists("SELECT COUNT(*) FROM TraineeMasterExternal WHERE TraineeID=@ID", id);
            return false;
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            string loginID = txtLoginID.Text.Trim().ToUpperInvariant();
            string role = ddlRole.SelectedValue;
            string correspondingID = txtCorrespondingID.Text.Trim().ToUpperInvariant();
            string password = txtPassword.Text.Trim();

            if (loginID == "" || role == "" || correspondingID == "" || password == "")
            {
                SetMessage("Login ID, Role, Corresponding ID and Password are required.", false);
                return;
            }

            object count = db.ExecuteScalar("SELECT COUNT(*) FROM Login WHERE LoginIDUserID=@LoginID", new SqlParameter[] { new SqlParameter("@LoginID", loginID) });
            if (count != null && Convert.ToInt32(count) > 0)
            {
                SetMessage("This Login ID already exists. Use User Management to edit the existing user.", false);
                return;
            }

            if (!ValidateCorrespondingID(role, correspondingID))
            {
                SetMessage("Corresponding ID does not exist for the selected role.", false);
                return;
            }

            Encryptor2 enc = new Encryptor2();
            db.ExecuteSql("INSERT INTO Login(LoginIDUserID,Role,CorrespondingEmpID,Password,Active,re) VALUES(@LoginID,@Role,@CorrespondingID,@Password,@Active,@re)", new SqlParameter[] {
                new SqlParameter("@LoginID", loginID),
                new SqlParameter("@Role", role),
                new SqlParameter("@CorrespondingID", correspondingID),
                new SqlParameter("@Password", enc.Encrypt(password)),
                new SqlParameter("@Active", ddlActive.SelectedValue),
                new SqlParameter("@re", enc.Encrypt("Y"))
            });

            string createDetails = "Login ID: " + loginID + "; Role: " + role + "; Corresponding ID: " + correspondingID + "; Active: " + ddlActive.SelectedValue;
            clsAuditLog.LogActivity(CurrentUser(), "SuperAdmin", "CREATE", "UserManagement", "CreateUser.aspx", "User", loginID, "Created user. Values: " + createDetails);
            SetMessage("User created successfully.", true);
            ClearForm();
        }

        private void ClearForm()
        {
            txtLoginID.Text = "";
            txtCorrespondingID.Text = "";
            txtPassword.Text = "";
            ddlRole.SelectedIndex = 0;
            ddlActive.SelectedValue = "Y";
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
            lblMessage.Text = "";
        }

        private void SetMessage(string message, bool success)
        {
            lblMessage.ForeColor = success ? System.Drawing.Color.Green : System.Drawing.Color.Red;
            lblMessage.Text = message;
        }
    }
}