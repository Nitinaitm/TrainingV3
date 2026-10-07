using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainee
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetNoStore();
            clsAuditLog.LogLogout(Session["LoginHistoryID"]);
            clsAuditLog.LogActivity(Session["EmpID"] == null ? "" : Session["EmpID"].ToString(), Session["Role"] == null ? "" : Session["Role"].ToString(), "LOGOUT", "Authentication", "Logout.aspx", "User", Session["EmpID"] == null ? "" : Session["EmpID"].ToString(), "User logout");
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();

            System.Web.Security.FormsAuthentication.SignOut();
            Response.Redirect("~/Default.aspx");
        }
    }
}