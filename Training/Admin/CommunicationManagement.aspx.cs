using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class CommunicationManagement : System.Web.UI.Page
    {
        private readonly clsDataAccess obj = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            string role = Convert.ToString(Session["Role"]);
            if (!(role.Equals("Admin", StringComparison.OrdinalIgnoreCase) || role.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase) || role.Equals("Nodal", StringComparison.OrdinalIgnoreCase)))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }
            if (!IsPostBack)\n            {\n                string requestedType = Convert.ToString(Request.QueryString["type"]);\n                if (ddlType.Items.FindByValue(requestedType) != null) ddlType.SelectedValue = requestedType;\n                BindGrid();\n            }
        }

        private void BindGrid()
        {
            string query = @"
SELECT RecordType, RecordID, Title, Message, SenderID, Audience, TargetTrainingID, TargetSessionID, AvailabilityMode, EffectiveFrom, EffectiveTo, CreatedOn
FROM
(
    SELECT 'Announcement' AS RecordType, AnnouncementID AS RecordID, ISNULL(Title,'Announcement') AS Title, Message, TrainerID AS SenderID, Audience, TargetTrainingID, TargetSessionID, AvailabilityMode, EffectiveFrom, EffectiveTo, CreatedOn
    FROM Announcement
    UNION ALL
    SELECT 'Notification' AS RecordType, NotificationID AS RecordID, ISNULL(Title,'Notification') AS Title, Message, TrainerID AS SenderID, Audience, TargetTrainingID, TargetSessionID, AvailabilityMode, EffectiveFrom, EffectiveTo, CreatedOn
    FROM Notification
) C
WHERE (@Type='All' OR RecordType=@Type)
ORDER BY CreatedOn DESC";
            DataTable dt = obj.GetDataTable(query, new SqlParameter[] { new SqlParameter("@Type", ddlType.SelectedValue) });
            gvCommunications.DataSource = dt;
            gvCommunications.DataBind();
        }

        protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlEdit.Visible = false;
            BindGrid();
        }

        protected void gvCommunications_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out index) || index < 0 || index >= gvCommunications.Rows.Count) return;
            string id = Convert.ToString(gvCommunications.DataKeys[index].Values["RecordID"]);
            string type = Convert.ToString(gvCommunications.DataKeys[index].Values["RecordType"]);
            if (e.CommandName == "EditRecord") LoadForEdit(type, id);
            else if (e.CommandName == "DeleteRecord")
            {
                string table = type == "Announcement" ? "Announcement" : "Notification";
                string key = type == "Announcement" ? "AnnouncementID" : "NotificationID";
                obj.ExecuteSql("DELETE FROM " + table + " WHERE " + key + "=@ID", new SqlParameter[] { new SqlParameter("@ID", id) });
                lblMessage.CssClass = "fw-semibold text-success";
                lblMessage.Text = type + " deleted successfully.";
                pnlEdit.Visible = false;
                BindGrid();
            }
        }

        private void LoadForEdit(string type, string id)
        {
            string table = type == "Announcement" ? "Announcement" : "Notification";
            string key = type == "Announcement" ? "AnnouncementID" : "NotificationID";
            DataTable dt = obj.GetDataTable("SELECT " + key + " AS RecordID, ISNULL(Title,'') AS Title, Message, Audience, TargetTrainingID, TargetSessionID, AvailabilityMode, EffectiveFrom, EffectiveTo FROM " + table + " WHERE " + key + "=@ID", new SqlParameter[] { new SqlParameter("@ID", id) });
            if (dt.Rows.Count == 0) return;
            DataRow row = dt.Rows[0];
            hfRecordID.Value = id;
            hfRecordType.Value = type;
            txtEditType.Text = type;
            txtEditTitle.Text = Convert.ToString(row["Title"]);
            txtEditTitle.Enabled = type == "Announcement";
            txtEditMessage.Text = Convert.ToString(row["Message"]);
            ddlAvailabilityMode.SelectedValue = ddlAvailabilityMode.Items.FindByValue(Convert.ToString(row["AvailabilityMode"])) == null ? "Batch" : Convert.ToString(row["AvailabilityMode"]);
            txtEffectiveFrom.Text = row["EffectiveFrom"] == DBNull.Value ? "" : Convert.ToDateTime(row["EffectiveFrom"]).ToString("yyyy-MM-dd");
            txtEffectiveTo.Text = row["EffectiveTo"] == DBNull.Value ? "" : Convert.ToDateTime(row["EffectiveTo"]).ToString("yyyy-MM-dd");
            lblEditTarget.Text = "Target batch: " + Convert.ToString(row["TargetTrainingID"]) + "; session: " + Convert.ToString(row["TargetSessionID"]) + "; audience: " + Convert.ToString(row["Audience"]) + ". Target selection can be changed by the original sender.";
            pnlEdit.Visible = true;
            lblMessage.Text = "";
        }

        protected void btnSaveEdit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(hfRecordID.Value) || string.IsNullOrWhiteSpace(hfRecordType.Value)) return;
            DateTime fromDate, toDate;
            object from = DBNull.Value, to = DBNull.Value;
            if (!string.IsNullOrWhiteSpace(txtEffectiveFrom.Text))
            {
                if (!DateTime.TryParseExact(txtEffectiveFrom.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate))
                { ShowError("Invalid Effective From date."); return; }
                from = fromDate.Date;
            }
            if (!string.IsNullOrWhiteSpace(txtEffectiveTo.Text))
            {
                if (!DateTime.TryParseExact(txtEffectiveTo.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate))
                { ShowError("Invalid Effective To date."); return; }
                to = toDate.Date.AddDays(1).AddTicks(-1);
            }
            if (from != DBNull.Value && to != DBNull.Value && Convert.ToDateTime(from) > Convert.ToDateTime(to))
            { ShowError("Effective From cannot be after Effective To."); return; }

            string table = hfRecordType.Value == "Announcement" ? "Announcement" : "Notification";
            string key = hfRecordType.Value == "Announcement" ? "AnnouncementID" : "NotificationID";
            string sql = table == "Announcement"
                ? "UPDATE Announcement SET Title=@Title, Message=@Message, AvailabilityMode=@Mode, EffectiveFrom=@From, EffectiveTo=@To WHERE AnnouncementID=@ID"
                : "UPDATE Notification SET Title=@Title, Message=@Message, AvailabilityMode=@Mode, EffectiveFrom=@From, EffectiveTo=@To WHERE NotificationID=@ID";
            obj.ExecuteSql(sql, new SqlParameter[] {
                new SqlParameter("@Title", table == "Announcement" ? txtEditTitle.Text.Trim() : (object)DBNull.Value),
                new SqlParameter("@Message", txtEditMessage.Text.Trim()),
                new SqlParameter("@Mode", ddlAvailabilityMode.SelectedValue),
                new SqlParameter("@From", from),
                new SqlParameter("@To", to),
                new SqlParameter("@ID", hfRecordID.Value)
            });
            lblMessage.CssClass = "fw-semibold text-success";
            lblMessage.Text = "Changes saved successfully.";
            pnlEdit.Visible = false;
            BindGrid();
        }

        private void ShowError(string message)
        {
            lblMessage.CssClass = "fw-semibold text-danger";
            lblMessage.Text = message;
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            pnlEdit.Visible = false;
        }
    }
}