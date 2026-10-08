using System;
using System.Data.SqlClient;
using System.Web.UI;

namespace Training.SuperAdmin
{
    public partial class LoginHistory : Page
    {
        private readonly clsDataAccess db = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Role"] == null || !string.Equals(Session["Role"].ToString(), "SuperAdmin", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                gvLoginHistory.DataSource = null;
                gvLoginHistory.DataBind();
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            DateTime from;
            DateTime to;

            if (!DateTime.TryParse(txtFromDate.Text, out from) || !DateTime.TryParse(txtToDate.Text, out to))
            {
                SetMessage("Please select both From Date and To Date.", false);
                gvLoginHistory.DataSource = null;
                gvLoginHistory.DataBind();
                return;
            }

            if (to < from)
            {
                SetMessage("To Date cannot be earlier than From Date.", false);
                gvLoginHistory.DataSource = null;
                gvLoginHistory.DataBind();
                return;
            }

            DateTime toExclusive = to.Date.AddDays(1);
            string sql = "SELECT LoginHistoryID,UserID,UserRole,LoginTime,LogoutTime,LoginStatus,FailureReason,IPAddress,SessionID FROM UserLoginHistory WHERE LoginTime>=@FromDate AND LoginTime<@ToDate ORDER BY LoginTime DESC";

            gvLoginHistory.DataSource = db.GetDataTable(sql, new SqlParameter[]
            {
                new SqlParameter("@FromDate", from.Date),
                new SqlParameter("@ToDate", toExclusive)
            });
            gvLoginHistory.DataBind();
            SetMessage("Login history loaded successfully.", true);
        }

        private void SetMessage(string message, bool success)
        {
            lblMessage.ForeColor = success ? System.Drawing.Color.Green : System.Drawing.Color.Red;
            lblMessage.Text = message;
        }
    }
}