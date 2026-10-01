using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainer
{
    public partial class Notifications : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TrainerID"] == null) Response.Redirect("~/Default.aspx");
            if (!IsPostBack) BindGrid();
        }

        private string TrainerID => Session["TrainerID"].ToString();

        protected void btnSend_Click(object sender,EventArgs e){if(string.IsNullOrWhiteSpace(txtMessage.Text)){lblMessage.Text="Notification is required.";lblMessage.ForeColor=System.Drawing.Color.Red;return;}string id=Guid.NewGuid().ToString("N").Substring(0,12).ToUpper();if(fuPdf.HasFile){string ext=Path.GetExtension(fuPdf.FileName).ToLower();if(ext!=".pdf"){lblMessage.Text="Only PDF file is allowed.";lblMessage.ForeColor=System.Drawing.Color.Red;return;}string folder=Server.MapPath("~/Uploads/Notifications/");if(!Directory.Exists(folder))Directory.CreateDirectory(folder);fuPdf.SaveAs(Path.Combine(folder,id+".pdf"));}obj.ExecuteSql("INSERT INTO Notification(NotificationID,TrainerID,Message,IsRead,CreatedOn) VALUES(@NotificationID,@TrainerID,@Message,0,GETDATE())",new SqlParameter[]{new SqlParameter("@NotificationID",id),new SqlParameter("@TrainerID",TrainerID),new SqlParameter("@Message",txtMessage.Text.Trim())});txtMessage.Text="";lblMessage.Text="Notification published successfully.";lblMessage.ForeColor=System.Drawing.Color.Green;BindGrid();}

        private void BindGrid()
        {
            string query = "SELECT NotificationID, Message, IsRead, CreatedOn FROM Notification WHERE TrainerID=@TrainerID ORDER BY CreatedOn DESC";
            SqlParameter[] param = new SqlParameter[] { new SqlParameter("@TrainerID", TrainerID) };
            DataTable dt = obj.GetDataTable(query, param);
            dt.Columns.Add("HasPdf",typeof(bool));foreach(DataRow row in dt.Rows)row["HasPdf"]=File.Exists(Server.MapPath("~/Uploads/Notifications/"+row["NotificationID"]+".pdf"));
            gvNotifications.DataSource = dt;
            gvNotifications.DataBind();
        }

        protected void btnMarkAllRead_Click(object sender, EventArgs e)
        {
            string query = "UPDATE Notification SET IsRead=1 WHERE TrainerID=@TrainerID";
            SqlParameter[] param = new SqlParameter[] { new SqlParameter("@TrainerID", TrainerID) };
            obj.ExecuteSql(query, param);
            BindGrid();
        }
    }
}