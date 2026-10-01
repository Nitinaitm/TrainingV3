using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainer
{
    public partial class Announcement : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TrainerID"] == null) Response.Redirect("~/Default.aspx");
            if (!IsPostBack) BindGrid();
        }

        private string TrainerID => Session["TrainerID"].ToString();

        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTitle.Text.Trim()))
            { lblMessage.Text = "Please enter Title."; lblMessage.ForeColor = System.Drawing.Color.Red; return; }

            if (string.IsNullOrEmpty(txtMessage.Text.Trim()))
            { lblMessage.Text = "Please enter Message."; lblMessage.ForeColor = System.Drawing.Color.Red; return; }

            string query = @"INSERT INTO Announcement (AnnouncementID, TrainerID, Title, Message, Audience, CreatedOn, IsActive) 
                             VALUES (@AnnouncementID, @TrainerID, @Title, @Message, @Audience, GETDATE(), 1)";
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@AnnouncementID", Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper()),
                new SqlParameter("@TrainerID", TrainerID),
                new SqlParameter("@Title", txtTitle.Text.Trim()),
                new SqlParameter("@Message", txtMessage.Text.Trim()),
                new SqlParameter("@Audience", ddlAudience.SelectedValue)
            };
            string announcementID=param[0].Value.ToString();
            if(fuPdf.HasFile){string ext=Path.GetExtension(fuPdf.FileName).ToLower();if(ext!=".pdf"){lblMessage.Text="Only PDF file is allowed.";lblMessage.ForeColor=System.Drawing.Color.Red;return;}string folder=Server.MapPath("~/Uploads/Announcements/");if(!Directory.Exists(folder))Directory.CreateDirectory(folder);fuPdf.SaveAs(Path.Combine(folder,announcementID+".pdf"));}
            obj.ExecuteSql(query, param);

            lblMessage.Text = "Announcement sent successfully!";
            lblMessage.ForeColor = System.Drawing.Color.Green;
            ClearForm();
            BindGrid();
        }

        protected void btnClear_Click(object sender, EventArgs e) => ClearForm();

        private void ClearForm()
        {
            txtTitle.Text = "";
            txtMessage.Text = "";
            ddlAudience.SelectedIndex = 0;
        }

        private void BindGrid()
        {
            string query = "SELECT AnnouncementID,Title, Message, Audience, CreatedOn FROM Announcement WHERE TrainerID=@TrainerID AND IsActive=1 ORDER BY CreatedOn DESC";
            SqlParameter[] param = new SqlParameter[] { new SqlParameter("@TrainerID", TrainerID) };
            DataTable dt = obj.GetDataTable(query, param);
            dt.Columns.Add("HasPdf",typeof(bool));foreach(DataRow row in dt.Rows)row["HasPdf"]=File.Exists(Server.MapPath("~/Uploads/Announcements/"+row["AnnouncementID"]+".pdf"));
            gvAnnouncements.DataSource = dt;
            gvAnnouncements.DataBind();
        }
    }
}