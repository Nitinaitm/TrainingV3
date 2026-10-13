using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Globalization;
using System.Web.UI.WebControls;

namespace Training.Manager
{
    public partial class Announcement : System.Web.UI.Page
    {
        private readonly clsDataAccess obj = new clsDataAccess();
        private string ManagerID { get { return Convert.ToString(Session["ManagerID"]); } }
        private string Sender { get { return "MANAGER:" + ManagerID; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ManagerID"] == null) { Response.Redirect("~/Default.aspx"); return; }
            if (!IsPostBack) { BindTargetLists(); BindGrid(); }
        }

        private void BindTargetLists()
        {
            ddlTargetTraining.Items.Clear();
            ddlTargetTraining.Items.Add(new ListItem("Select batch / training", ""));
            DataTable batches = obj.GetDataTable(@"SELECT DISTINCT TD.TrainingID, ISNULL(TD.Batch,'') AS BatchName, ISNULL(TD.TrainingLocation,'') AS TrainingLocation FROM TrainingDetails TD INNER JOIN TrainingLocationMaster L ON TD.TrainingLocation=L.TrainingLocation INNER JOIN ManagerMaster M ON M.TrainingLocationID=L.TrainingLocationID WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' ORDER BY TD.TrainingID DESC", new SqlParameter[] { new SqlParameter("@ManagerID", ManagerID) });
            foreach (DataRow row in batches.Rows) ddlTargetTraining.Items.Add(new ListItem(row["TrainingID"] + " - " + row["BatchName"] + " (" + row["TrainingLocation"] + ")", Convert.ToString(row["TrainingID"])));
            ddlTargetSession.Items.Clear();
            ddlTargetSession.Items.Add(new ListItem("Select session", ""));
            DataTable sessions = obj.GetDataTable(@"SELECT SM.SessionID, SM.SessionName, SM.TrainingID, SM.SessionDate FROM SessionMaster SM INNER JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID INNER JOIN TrainingLocationMaster L ON TD.TrainingLocation=L.TrainingLocation INNER JOIN ManagerMaster M ON M.TrainingLocationID=L.TrainingLocationID WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' ORDER BY SM.SessionDate DESC", new SqlParameter[] { new SqlParameter("@ManagerID", ManagerID) });
            foreach (DataRow row in sessions.Rows) ddlTargetSession.Items.Add(new ListItem(row["SessionName"] + " - " + Convert.ToDateTime(row["SessionDate"]).ToString("dd-MM-yyyy"), Convert.ToString(row["SessionID"])));
        }

        private bool TryResolveWindow(out DateTime from, out DateTime to, out string error)
        {
            from = DateTime.MinValue; to = DateTime.MinValue; error = "";
            if (ddlAvailabilityMode.SelectedValue == "Custom")
            {
                if (!DateTime.TryParseExact(txtEffectiveFrom.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out from) || !DateTime.TryParseExact(txtEffectiveTo.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out to))
                { error = "Select both Effective From and Effective To dates."; return false; }
                to = to.Date.AddDays(1).AddTicks(-1);
            }
            else if (ddlAvailabilityMode.SelectedValue == "Session")
            {
                if (String.IsNullOrWhiteSpace(ddlTargetTraining.SelectedValue) || String.IsNullOrWhiteSpace(ddlTargetSession.SelectedValue))
                { error = "Select the batch and session for session-only availability."; return false; }
                DataTable dt = obj.GetDataTable(@"SELECT SM.SessionDate FROM SessionMaster SM INNER JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID INNER JOIN TrainingLocationMaster L ON TD.TrainingLocation=L.TrainingLocation INNER JOIN ManagerMaster M ON M.TrainingLocationID=L.TrainingLocationID WHERE SM.SessionID=@SessionID AND SM.TrainingID=@TrainingID AND M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y'", new SqlParameter[] { new SqlParameter("@SessionID", ddlTargetSession.SelectedValue), new SqlParameter("@TrainingID", ddlTargetTraining.SelectedValue), new SqlParameter("@ManagerID", ManagerID) });
                if (dt.Rows.Count == 0) { error = "The selected session is not in a batch managed by you."; return false; }
                from = Convert.ToDateTime(dt.Rows[0]["SessionDate"]).Date; to = from.Date.AddDays(1).AddTicks(-1);
            }
            else
            {
                if (String.IsNullOrWhiteSpace(ddlTargetTraining.SelectedValue)) { error = "Select a batch for complete batch-duration availability, or choose Custom date range."; return false; }
                DataTable dt = obj.GetDataTable(@"SELECT TD.DateFrom, TD.DateTo FROM TrainingDetails TD INNER JOIN TrainingLocationMaster L ON TD.TrainingLocation=L.TrainingLocation INNER JOIN ManagerMaster M ON M.TrainingLocationID=L.TrainingLocationID WHERE TD.TrainingID=@TrainingID AND M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y'", new SqlParameter[] { new SqlParameter("@TrainingID", ddlTargetTraining.SelectedValue), new SqlParameter("@ManagerID", ManagerID) });
                if (dt.Rows.Count == 0 || dt.Rows[0]["DateFrom"] == DBNull.Value || dt.Rows[0]["DateTo"] == DBNull.Value) { error = "Batch start and end dates are not available."; return false; }
                from = Convert.ToDateTime(dt.Rows[0]["DateFrom"]).Date; to = Convert.ToDateTime(dt.Rows[0]["DateTo"]).Date.AddDays(1).AddTicks(-1);
            }
            if (from > to) { error = "Effective From cannot be after Effective To."; return false; }
            return true;
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtTitle.Text) || String.IsNullOrWhiteSpace(txtMessage.Text))
            { lblMessage.Text = "Title and Message are required."; lblMessage.ForeColor = System.Drawing.Color.Red; return; }
            DateTime from, to; string error;
            if (!TryResolveWindow(out from, out to, out error)) { lblMessage.Text = error; lblMessage.ForeColor = System.Drawing.Color.Red; return; }
            string id = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();
            if (fuPdf.HasFile)
            {
                if (Path.GetExtension(fuPdf.FileName).ToLowerInvariant() != ".pdf") { lblMessage.Text = "Only PDF file is allowed."; lblMessage.ForeColor = System.Drawing.Color.Red; return; }
                string folder = Server.MapPath("~/Uploads/Announcements/");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                fuPdf.SaveAs(Path.Combine(folder, id + ".pdf"));
            }
            obj.ExecuteSql(@"INSERT INTO Announcement(AnnouncementID,TrainerID,Title,Message,Audience,CreatedOn,IsActive,TargetTrainingID,TargetSessionID,AvailabilityMode,EffectiveFrom,EffectiveTo) VALUES(@ID,@Sender,@Title,@Message,@Audience,GETDATE(),1,@TrainingID,@SessionID,@Mode,@From,@To)",
                new SqlParameter[] { new SqlParameter("@ID",id), new SqlParameter("@Sender",Sender), new SqlParameter("@Title",txtTitle.Text.Trim()), new SqlParameter("@Message",txtMessage.Text.Trim()), new SqlParameter("@Audience",String.IsNullOrWhiteSpace(ddlTargetTraining.SelectedValue)?"All":"Batch"), new SqlParameter("@TrainingID",String.IsNullOrWhiteSpace(ddlTargetTraining.SelectedValue)?(object)DBNull.Value:ddlTargetTraining.SelectedValue), new SqlParameter("@SessionID",String.IsNullOrWhiteSpace(ddlTargetSession.SelectedValue)?(object)DBNull.Value:ddlTargetSession.SelectedValue), new SqlParameter("@Mode",ddlAvailabilityMode.SelectedValue), new SqlParameter("@From",from), new SqlParameter("@To",to) });
            txtTitle.Text = ""; txtMessage.Text = ""; txtEffectiveFrom.Text = ""; txtEffectiveTo.Text = "";
            ddlTargetTraining.SelectedIndex = 0; ddlTargetSession.SelectedIndex = 0; ddlAvailabilityMode.SelectedValue = "Batch";
            lblMessage.Text = "Announcement published successfully."; lblMessage.ForeColor = System.Drawing.Color.Green; BindGrid();
        }

        protected void gvAnnouncements_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "DeleteAnnouncement") return;
            string id = Convert.ToString(e.CommandArgument);
            obj.ExecuteSql("DELETE FROM Announcement WHERE AnnouncementID=@ID AND TrainerID=@Sender", new SqlParameter[] { new SqlParameter("@ID", id), new SqlParameter("@Sender", Sender) });
            string pdf = Server.MapPath("~/Uploads/Announcements/" + id + ".pdf");
            if (File.Exists(pdf)) File.Delete(pdf);
            BindGrid();
        }

        private void BindGrid()
        {
            DataTable d = obj.GetDataTable("SELECT AnnouncementID,Title,Message,CreatedOn FROM Announcement WHERE TrainerID=@Sender ORDER BY CreatedOn DESC", new SqlParameter[] { new SqlParameter("@Sender", Sender) });
            d.Columns.Add("HasPdf", typeof(bool));
            foreach (DataRow row in d.Rows) row["HasPdf"] = File.Exists(Server.MapPath("~/Uploads/Announcements/" + row["AnnouncementID"] + ".pdf"));
            gvAnnouncements.DataSource = d; gvAnnouncements.DataBind();
        }
    }
}