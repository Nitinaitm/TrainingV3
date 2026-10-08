using System;
using System.Web.UI;
namespace Training.SuperAdmin
{
 public partial class ActivityLog : Page
 {
  private readonly clsDataAccess db=new clsDataAccess();
  protected void Page_Load(object sender,EventArgs e)
  {
   if(Session["Role"]==null||!string.Equals(Session["Role"].ToString(),"SuperAdmin",StringComparison.OrdinalIgnoreCase)){Response.Redirect("~/Default.aspx");return;}
   if(!IsPostBack){gvActivity.DataSource=db.GetDataTable("SELECT TOP 1000 ActivityID,UserID,UserRole,ActionType,Module,PageName,RecordType,RecordID,Description,ActivityTime,IPAddress,SessionID FROM UserActivityLog ORDER BY ActivityTime DESC");gvActivity.DataBind();}
  }
 }
}