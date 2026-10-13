using System;
using System.Web.UI;
using System.Data;
using System.Data.SqlClient;

namespace Training
{
    public partial class AdminMaster : System.Web.UI.MasterPage
    {
        private readonly clsDataAccess objDB = new clsDataAccess();
        protected void Page_Load(object sender, EventArgs e)
        {
            string role = Convert.ToString(Session["Role"]);
            if (string.IsNullOrWhiteSpace(role) ||
                !(role.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                  role.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase) ||
                  role.Equals("Nodal", StringComparison.OrdinalIgnoreCase)))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack) LoadCommunicationCounts();

            //Page.ClientScript.RegisterClientScriptInclude("EmployeeGridEdit", ResolveUrl("~/Scripts/employee-edit-grid.js"));
            //Page.ClientScript.RegisterStartupScript(GetType(), "Dashboard2Menu", "(function(){var s=document.getElementById('adminSidebar');if(!s||document.getElementById('dashboard2Menu'))return;var a=document.createElement('a');a.id='dashboard2Menu';a.href='Dashboard.aspx';a.className='menu-item';a.innerHTML='<i class=\"fas fa-tachometer-alt mr-2\"></i>Dashboard 2';s.insertBefore(a,s.firstElementChild);})();", true);
        }
        private void LoadCommunicationCounts()
        {
            try
            {
                DataTable announcements = objDB.GetDataTable("SELECT COUNT(*) AS Cnt FROM Announcement WHERE ISNULL(IsActive,1)=1", new SqlParameter[0]);
                DataTable notifications = objDB.GetDataTable("SELECT COUNT(*) AS Cnt FROM Notification", new SqlParameter[0]);
                lblAnnouncementCount.Text = announcements.Rows.Count == 0 ? "0" : announcements.Rows[0]["Cnt"].ToString();
                lblNotificationCount.Text = notifications.Rows.Count == 0 ? "0" : notifications.Rows[0]["Cnt"].ToString();
            }
            catch
            {
                lblAnnouncementCount.Text = "0";
                lblNotificationCount.Text = "0";
            }
        }

    }
}
