using System;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
namespace Training.SuperAdmin
{
 public partial class PasswordManagement : Page
 {
  private readonly clsDataAccess db=new clsDataAccess();
  protected void Page_Load(object sender,EventArgs e){if(!IsSuperAdmin()){Response.Redirect("~/Default.aspx");return;}}
  private bool IsSuperAdmin(){return Session["Role"]!=null&&string.Equals(Session["Role"].ToString(),"SuperAdmin",StringComparison.OrdinalIgnoreCase);}
  protected void btnChange_Click(object sender,EventArgs e)
  {
   string id=txtLoginID.Text.Trim().ToUpperInvariant(); string p=txtPassword.Text; string cp=txtConfirmPassword.Text;
   if(id==""||p==""||cp==""){Set("Login ID, password and confirmation are required.",false);return;}
   if(p!=cp){Set("Password and confirmation do not match.",false);return;}
   object v=db.ExecuteScalar("SELECT COUNT(*) FROM Login WHERE LoginIDUserID=@ID",new SqlParameter[]{new SqlParameter("@ID",id)});
   if(v==null||Convert.ToInt32(v)==0){Set("User not found.",false);return;}
   Encryptor2 enc=new Encryptor2();
   db.ExecuteSql("UPDATE Login SET Password=@Password,re=@re WHERE LoginIDUserID=@ID",new SqlParameter[]{new SqlParameter("@Password",enc.Encrypt(p)),new SqlParameter("@re",enc.Encrypt("Y")),new SqlParameter("@ID",id)});
   clsAuditLog.LogActivity(Session["UserID"].ToString(),"SuperAdmin","PASSWORD_CHANGE","UserManagement","PasswordManagement.aspx","User",id,"Password changed by SuperAdmin");
   Set("Password changed successfully.",true); txtPassword.Text=""; txtConfirmPassword.Text="";
  }
  private void Set(string m,bool ok){lblMessage.ForeColor=ok?System.Drawing.Color.Green:System.Drawing.Color.Red;lblMessage.Text=m;}
 }
}