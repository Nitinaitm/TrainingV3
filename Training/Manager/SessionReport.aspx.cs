using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Manager
{
    public partial class SessionReport : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        private string ManagerID
        {
            get { return Session["ManagerID"] == null ? "" : Session["ManagerID"].ToString().Trim(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ManagerID))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindCourse();
                BindGrid();
            }
        }

        private string GetTrainingLocationID()
        {
            object value = obj.ExecuteScalar(
                "SELECT TOP 1 TrainingLocationID FROM ManagerMaster WHERE ManagerID=@ManagerID AND ISNULL(ActiveStatus,'Y')='Y'",
                new SqlParameter[] { new SqlParameter("@ManagerID", ManagerID) });

            return value == null || value == DBNull.Value ? "" : value.ToString().Trim();
        }

        private void BindCourse()
        {
            string locationID = GetTrainingLocationID();

            DataTable dt = obj.GetDataTable(
                "SELECT DISTINCT CM.CourseID,CM.CourseName FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID INNER JOIN TrainingLocationMaster L ON TD.TrainingLocation=L.TrainingLocation INNER JOIN ManagerMaster M ON M.TrainingLocationID=L.TrainingLocationID INNER JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND M.TrainingLocationID=@TrainingLocationID ORDER BY CM.CourseName",
                new SqlParameter[] { new SqlParameter("@ManagerID", ManagerID), new SqlParameter("@TrainingLocationID", locationID) });

            ddlCourse.DataSource = dt;
            ddlCourse.DataTextField = "CourseName";
            ddlCourse.DataValueField = "CourseID";
            ddlCourse.DataBind();
            ddlCourse.Items.Insert(0, new ListItem("-- All Courses --", ""));
        }

        private void BindGrid()
        {
            string query = "SELECT DISTINCT SM.SessionID,SM.TrainingID,CM.CourseName,TD.Batch,SM.SessionNo,SM.SessionName,ISNULL(TP.TopicName,'') TopicName,SM.SessionDate,SM.StartTime,SM.EndTime FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID INNER JOIN TrainingLocationMaster L ON TD.TrainingLocation=L.TrainingLocation INNER JOIN ManagerMaster M ON M.TrainingLocationID=L.TrainingLocationID INNER JOIN CourseMaster CM ON TD.CourseID=CM.CourseID LEFT JOIN TopicMaster TP ON SM.TopicID=TP.TopicID WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND M.TrainingLocationID=@TrainingLocationID";
            System.Collections.Generic.List<SqlParameter> parameters = new System.Collections.Generic.List<SqlParameter>();
            parameters.Add(new SqlParameter("@ManagerID", ManagerID));
            parameters.Add(new SqlParameter("@TrainingLocationID", GetTrainingLocationID()));

            if (!string.IsNullOrWhiteSpace(ddlCourse.SelectedValue))
            {
                query += " AND TD.CourseID=@CourseID";
                parameters.Add(new SqlParameter("@CourseID", ddlCourse.SelectedValue));
            }

            if (!string.IsNullOrWhiteSpace(txtBatch.Text))
            {
                query += " AND TD.Batch LIKE @Batch";
                parameters.Add(new SqlParameter("@Batch", "%" + txtBatch.Text.Trim() + "%"));
            }

            DateTime fromDate;
            DateTime toDate;

            if (ParseDate(txtFromDate.Text, out fromDate))
            {
                query += " AND TRY_CONVERT(date,SM.SessionDate,105)>=@FromDate";
                parameters.Add(new SqlParameter("@FromDate", fromDate));
            }

            if (ParseDate(txtToDate.Text, out toDate))
            {
                query += " AND TRY_CONVERT(date,SM.SessionDate,105)<=@ToDate";
                parameters.Add(new SqlParameter("@ToDate", toDate));
            }

            query += " ORDER BY TRY_CONVERT(date,SM.SessionDate,105) DESC,TRY_CONVERT(int,SM.SessionNo),SM.SessionID DESC";

            gvSession.DataSource = obj.GetDataTable(query, parameters.ToArray());
            gvSession.DataBind();
        }

        private bool ParseDate(string value, out DateTime date)
        {
            return DateTime.TryParseExact(value == null ? "" : value.Trim(), "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            ddlCourse.SelectedIndex = 0;
            txtBatch.Text = "";
            txtFromDate.Text = "";
            txtToDate.Text = "";
            lblMessage.Text = "";
            BindGrid();
        }

        protected void gvSession_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "View")
            {
                return;
            }

            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;

            Session["SessionID"] = gvSession.DataKeys[row.RowIndex].Values["SessionID"].ToString();
            Session["TrainingID"] = gvSession.DataKeys[row.RowIndex].Values["TrainingID"].ToString();

            Response.Redirect("~/Manager/SessionReportDetails.aspx");
        }
    }
}