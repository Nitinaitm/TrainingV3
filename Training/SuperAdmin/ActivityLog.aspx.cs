using System;
using System.Data.SqlClient;
using System.Web.UI;

namespace Training.SuperAdmin
{
    public partial class ActivityLog : Page
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
                gvActivity.DataSource = null;
                gvActivity.DataBind();
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            DateTime from;
            DateTime to;

            if (!DateTime.TryParse(txtFromDate.Text, out from) || !DateTime.TryParse(txtToDate.Text, out to))
            {
                SetMessage("Please select both From Date and To Date.", false);
                gvActivity.DataSource = null;
                gvActivity.DataBind();
                return;
            }

            if (to < from)
            {
                SetMessage("To Date cannot be earlier than From Date.", false);
                gvActivity.DataSource = null;
                gvActivity.DataBind();
                return;
            }

            DateTime toExclusive = to.Date.AddDays(1);
            string sql = "SELECT ActivityID,UserID,UserRole,ActionType,Module,PageName,RecordType,RecordID,Description,ActivityTime,IPAddress,SessionID FROM UserActivityLog WHERE ActivityTime>=@FromDate AND ActivityTime<@ToDate ORDER BY ActivityTime DESC";

            gvActivity.DataSource = db.GetDataTable(sql, new SqlParameter[]
            {
                new SqlParameter("@FromDate", from.Date),
                new SqlParameter("@ToDate", toExclusive)
            });
            gvActivity.DataBind();
            SetMessage("Activity log loaded successfully.", true);
        }

        private void SetMessage(string message, bool success)
        {
            lblMessage.ForeColor = success ? System.Drawing.Color.Green : System.Drawing.Color.Red;
            lblMessage.Text = message;
        }
    }
}