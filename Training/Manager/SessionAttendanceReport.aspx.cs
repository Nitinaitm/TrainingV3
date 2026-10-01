using System;
using System.Data;
using System.Data.SqlClient;

namespace Training.Manager
{
    public partial class SessionAttendanceReport : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        private string ManagerID { get { return Session["ManagerID"] == null ? "" : Session["ManagerID"].ToString().Trim(); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ManagerID) || Session["SessionID"] == null || Session["TrainingID"] == null)
            {
                Response.Redirect("~/Manager/SessionReport.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadReport();
            }
        }

        private bool HasAccess()
        {
            object value = obj.ExecuteScalar(
                "SELECT COUNT(*) FROM ManagerMaster M INNER JOIN TrainingLocationMaster L ON M.TrainingLocationID=L.TrainingLocationID INNER JOIN TrainingDetails TD ON TD.TrainingLocation=L.TrainingLocation INNER JOIN SessionMaster SM ON SM.TrainingID=TD.TrainingID WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND TD.TrainingID=@TrainingID AND SM.SessionID=@SessionID",
                new SqlParameter[] { new SqlParameter("@ManagerID",ManagerID),new SqlParameter("@TrainingID",Session["TrainingID"].ToString()),new SqlParameter("@SessionID",Session["SessionID"].ToString()) });

            return value != null && value != DBNull.Value && Convert.ToInt32(value) > 0;
        }

        private void LoadReport()
        {
            if (!HasAccess())
            {
                Response.Redirect("~/Manager/SessionReport.aspx");
                return;
            }

            DataTable session = obj.GetDataTable(
                "SELECT SessionNo,SessionName FROM SessionMaster WHERE SessionID=@SessionID AND TrainingID=@TrainingID",
                new SqlParameter[] { new SqlParameter("@SessionID",Session["SessionID"].ToString()),new SqlParameter("@TrainingID",Session["TrainingID"].ToString()) });

            if (session.Rows.Count > 0)
            {
                lblSession.Text = "Session " + session.Rows[0]["SessionNo"] + " - " + session.Rows[0]["SessionName"];
            }

            DataTable dt = obj.GetDataTable(
                "SELECT TA.EmpID,EBM.EmpName,EBM.EmpDesignation,ISNULL(SA.AttendanceStatus,'Pending') AttendanceStatus,ISNULL(SA.Remarks,'') Remarks FROM TrainingAssignment TA INNER JOIN EmpBasicMaster EBM ON TA.EmpID=EBM.EmpID LEFT JOIN SessionAttendance SA ON TA.TrainingID=SA.TrainingID AND TA.EmpID=SA.EmpID AND SA.SessionID=@SessionID WHERE TA.TrainingID=@TrainingID AND TA.AssignmentStatus='Assigned' ORDER BY EBM.EmpName",
                new SqlParameter[] { new SqlParameter("@TrainingID",Session["TrainingID"].ToString()),new SqlParameter("@SessionID",Session["SessionID"].ToString()) });

            gvAttendance.DataSource = dt;
            gvAttendance.DataBind();

            int total = dt.Rows.Count;
            int present = 0;
            int absent = 0;

            foreach (DataRow row in dt.Rows)
            {
                if (row["AttendanceStatus"].ToString().Equals("Present",StringComparison.OrdinalIgnoreCase)) present++;
                if (row["AttendanceStatus"].ToString().Equals("Absent",StringComparison.OrdinalIgnoreCase)) absent++;
            }

            lblTotal.Text = total.ToString();
            lblPresent.Text = present.ToString();
            lblAbsent.Text = absent.ToString();
            lblPending.Text = Math.Max(0,total-present-absent).ToString();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Manager/SessionReportDetails.aspx");
        }
    }
}