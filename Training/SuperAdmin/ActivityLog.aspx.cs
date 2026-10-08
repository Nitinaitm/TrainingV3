using System;
using System.Web.UI;
using System.Data.SqlClient;
namespace Training.SuperAdmin
{
 public partial class ActivityLog : Page
 {
  private readonly clsDataAccess db=new clsDataAccess();
  protected void Page_Load(object sender,EventArgs e)
  {
   if(Session["Role"]==null||!string.Equals(Session["Role"].ToString(),"SuperAdmin",StringComparison.OrdinalIgnoreCase)){Response.Redirect("~/Default.aspx");return;}
   if(!IsPostBack){gvActivity.DataSource=null;gvActivity.DataBind();}
  }
 }
}