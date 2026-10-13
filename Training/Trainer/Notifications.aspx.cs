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
            if (!IsPostBack) { BindTargetLists(); BindGrid(); }
        }

        private string TrainerID => Session["TrainerID"].ToString();

        protected void btnSend_Click(object sender,EventArgs e){if(string.IsNullOrWhiteSpace(txtMessage.Text)){lblMessage.Text="Notification is required.";lblMessage.ForeColor=System.Drawing.Color.Red;return;}string id=Guid.NewGuid().ToString("N").Substring(0,12).ToUpper();if(fuPdf.HasFile){string ext=Path.GetExtension(fuPdf.FileName).ToLower();if(ext!=".pdf"){lblMessage.Text="Only PDF file is allowed.";lblMessage.ForeColor=System.Drawing.Color.Red;return;}string folder=Server.MapPath("~/Uploads/Notifications/");if(!Directory.Exists(folder))Directory.CreateDirectory(folder);fuPdf.SaveAs(Path.Combine(folder,id+".pdf"));}DateTime effectiveFrom; DateTime effectiveTo; string windowError;
            if (!TryResolveWindow(out effectiveFrom, out effectiveTo, out windowError)) { lblMessage.Text = windowError; lblMessage.ForeColor = System.Drawing.Color.Red; return; }
            obj.ExecuteSql("INSERT INTO Notification(NotificationID,TrainerID,Title,Message,Audience,IsRead,CreatedOn,TargetTrainingID,TargetSessionID,AvailabilityMode,EffectiveFrom,EffectiveTo) VALUES(@NotificationID,@TrainerID,@Title,@Message,@Audience,0,GETDATE(),@TrainingID,@SessionID,@Mode,@From,@To)",new SqlParameter[]{new SqlParameter("@NotificationID",id),new SqlParameter("@TrainerID",TrainerID),new SqlParameter("@Title","Notification"),new SqlParameter("@Message",txtMessage.Text.Trim()),new SqlParameter("@Audience",string.IsNullOrWhiteSpace(ddlTargetTraining.SelectedValue)?"All":"Batch"),new SqlParameter("@TrainingID",string.IsNullOrWhiteSpace(ddlTargetTraining.SelectedValue)?(object)DBNull.Value:ddlTargetTraining.SelectedValue),new SqlParameter("@SessionID",string.IsNullOrWhiteSpace(ddlTargetSession.SelectedValue)?(object)DBNull.Value:ddlTargetSession.SelectedValue),new SqlParameter("@Mode",ddlAvailabilityMode.SelectedValue),new SqlParameter("@From",effectiveFrom),new SqlParameter("@To",effectiveTo)});txtMessage.Text="";lblMessage.Text="Notification published successfully.";lblMessage.ForeColor=System.Drawing.Color.Green;BindGrid();}

        private void BindTargetLists()
        {
            ddlTargetTraining.Items.Clear();
            ddlTargetTraining.Items.Add(new System.Web.UI.WebControls.ListItem("Select batch / training", ""));
            DataTable batches = obj.GetDataTable(@"SELECT DISTINCT TD.TrainingID, ISNULL(TD.Batch,'') AS BatchName, ISNULL(TD.TrainingLocation,'') AS TrainingLocation FROM TrainingDetails TD INNER JOIN SessionMaster SM ON SM.TrainingID=TD.TrainingID WHERE SM.TrainerID=@TrainerID ORDER BY TD.TrainingID DESC", new SqlParameter[] { new SqlParameter("@TrainerID", TrainerID) });
            foreach (DataRow row in batches.Rows) ddlTargetTraining.Items.Add(new System.Web.UI.WebControls.ListItem(row["TrainingID"] + " - " + row["BatchName"] + " (" + row["TrainingLocation"] + ")", Convert.ToString(row["TrainingID"])));
            ddlTargetSession.Items.Clear();
            ddlTargetSession.Items.Add(new System.Web.UI.WebControls.ListItem("Select session", ""));
            DataTable sessions = obj.GetDataTable(@"SELECT SM.SessionID, SM.SessionName, SM.TrainingID, SM.SessionDate FROM SessionMaster SM WHERE SM.TrainerID=@TrainerID ORDER BY SM.SessionDate DESC", new SqlParameter[] { new SqlParameter("@TrainerID", TrainerID) });
            foreach (DataRow row in sessions.Rows) ddlTargetSession.Items.Add(new System.Web.UI.WebControls.ListItem(row["SessionName"] + " - " + Convert.ToDateTime(row["SessionDate"]).ToString("dd-MM-yyyy"), Convert.ToString(row["SessionID"])));
        }

        private bool TryResolveWindow(out DateTime effectiveFrom, out DateTime effectiveTo, out string error)
        {
            effectiveFrom = DateTime.MinValue; effectiveTo = DateTime.MinValue; error = "";
            if (ddlAvailabilityMode.SelectedValue == "Custom")
            {
                if (!DateTime.TryParseExact(txtEffectiveFrom.Text, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out effectiveFrom) || !DateTime.TryParseExact(txtEffectiveTo.Text, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out effectiveTo))
                { error = "Select both Effective From and Effective To dates."; return false; }
                effectiveTo = effectiveTo.Date.AddDays(1).AddTicks(-1);
            }
            else if (ddlAvailabilityMode.SelectedValue == "Session")
            {
                if (string.IsNullOrWhiteSpace(ddlTargetTraining.SelectedValue) || string.IsNullOrWhiteSpace(ddlTargetSession.SelectedValue)) { error = "Select the batch and session for session-only availability."; return false; }
                DataTable dt = obj.GetDataTable(@"SELECT SM.SessionDate FROM SessionMaster SM WHERE SM.SessionID=@SessionID AND SM.TrainingID=@TrainingID AND SM.TrainerID=@TrainerID", new SqlParameter[] { new SqlParameter("@SessionID", ddlTargetSession.SelectedValue), new SqlParameter("@TrainingID", ddlTargetTraining.SelectedValue), new SqlParameter("@TrainerID", TrainerID) });
                if (dt.Rows.Count == 0) { error = "The selected session does not belong to your assigned batch."; return false; }
                effectiveFrom = Convert.ToDateTime(dt.Rows[0]["SessionDate"]).Date; effectiveTo = effectiveFrom.Date.AddDays(1).AddTicks(-1);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(ddlTargetTraining.SelectedValue)) { error = "Select a batch for complete batch-duration availability, or choose Custom date range."; return false; }
                DataTable dt = obj.GetDataTable(@"SELECT TD.DateFrom, TD.DateTo FROM TrainingDetails TD WHERE TD.TrainingID=@TrainingID AND EXISTS (SELECT 1 FROM SessionMaster SM WHERE SM.TrainingID=TD.TrainingID AND SM.TrainerID=@TrainerID)", new SqlParameter[] { new SqlParameter("@TrainingID", ddlTargetTraining.SelectedValue), new SqlParameter("@TrainerID", TrainerID) });
                if (dt.Rows.Count == 0 || dt.Rows[0]["DateFrom"] == DBNull.Value || dt.Rows[0]["DateTo"] == DBNull.Value) { error = "Batch start and end dates are not available."; return false; }
                effectiveFrom = Convert.ToDateTime(dt.Rows[0]["DateFrom"]).Date; effectiveTo = Convert.ToDateTime(dt.Rows[0]["DateTo"]).Date.AddDays(1).AddTicks(-1);
            }
            if (effectiveFrom > effectiveTo) { error = "Effective From cannot be after Effective To."; return false; }
            return true;
        }

        private void BindGrid()
        {
            string query = "SELECT NotificationID, Message, IsRead, CreatedOn FROM Notification WHERE TrainerID=@TrainerID ORDER BY CreatedOn DESC";
            SqlParameter[] param = new SqlParameter[] { new SqlParameter("@TrainerID", TrainerID) };
            DataTable dt = obj.GetDataTable(query, param);
            dt.Columns.Add("HasPdf",typeof(bool));foreach(DataRow row in dt.Rows)row["HasPdf"]=File.Exists(Server.MapPath("~/Uploads/Notifications/"+row["NotificationID"]+".pdf"));
            gvNotifications.DataSource = dt;
            gvNotifications.DataBind();
        }

        protected void gvNotifications_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string id = Convert.ToString(e.CommandArgument);
            if (e.CommandName == "MarkRead")
            {
                obj.ExecuteSql("UPDATE Notification SET IsRead=1 WHERE NotificationID=@ID AND TrainerID=@TrainerID", new SqlParameter[] { new SqlParameter("@ID", id), new SqlParameter("@TrainerID", TrainerID) });
            }
            else if (e.CommandName == "DeleteNotification")
            {
                obj.ExecuteSql("DELETE FROM Notification WHERE NotificationID=@ID AND TrainerID=@TrainerID", new SqlParameter[] { new SqlParameter("@ID", id), new SqlParameter("@TrainerID", TrainerID) });
                string pdfPath = Server.MapPath("~/Uploads/Notifications/" + id + ".pdf");
                if (File.Exists(pdfPath)) File.Delete(pdfPath);
            }
            BindGrid();
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