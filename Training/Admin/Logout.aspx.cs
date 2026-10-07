using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetNoStore();
            string auditUserID = Session["UserID"] == null ? "" : Session["UserID"].ToString();
            string auditRole = Session["Role"] == null ? "" : Session["Role"].ToString();
            clsAuditLog.LogLogout(Session["LoginHistoryID"]);
            clsAuditLog.LogActivity(auditUserID, auditRole, "LOGOUT", "Authentication", "Logout.aspx", "User", auditUserID, "User logout");
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();

            System.Web.Security.FormsAuthentication.SignOut();
            Response.Redirect("~/Default.aspx");
        }
    }
}