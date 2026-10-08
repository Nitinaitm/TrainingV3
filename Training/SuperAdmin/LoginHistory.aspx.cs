using System;
using System.Web.UI;
namespace Training.SuperAdmin
{
 public partial class LoginHistory : Page
 {
  private readonly clsDataAccess db=new clsDataAccess();
  protected void Page_Load(object sender,EventArgs e)
  {
   if(Session["Role"]==null||!string.Equals(Session["Role"].ToString(),"SuperAdmin",StringComparison.OrdinalIgnoreCase)){Response.Redirect("~/Default.aspx");return;}
   if(!IsPostBack){gvLoginHistory.DataSource=db.GetDataTable("SELECT TOP 1000 LoginHistoryID,UserID,UserRole,LoginTime,LogoutTime,LoginStatus,FailureReason,IPAddress,SessionID FROM UserLoginHistory ORDER BY LoginTime DESC");gvLoginHistory.DataBind();}
  }
 }
}