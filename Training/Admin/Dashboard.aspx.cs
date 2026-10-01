using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Script.Serialization;

namespace Training.Admin
{
    public partial class Dashboard : System.Web.UI.Page
    {
        clsDataAccess objDB = new clsDataAccess();

        public string CompanyChartLabels { get; set; }
        public string CompanyChartValues { get; set; }
        public string CourseChartLabels { get; set; }
        public string CourseChartValues { get; set; }
        public string YearChartLabels { get; set; }
        public string YearChartValues { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            Response.AppendHeader("Pragma", "no-cache");

            if (Session["InternalRedirect_Admin"] == null)
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            BindCharts();

            if (!IsPostBack)
            {
                BindKpiCards();
                BindNeedsAttention();
                BindHostelCards();
                BindUpcomingTrainings();
            }
        }

        //---------------------------------------------------------
        // KPI CARDS
        //---------------------------------------------------------

        private void BindKpiCards()
        {
            lblActiveTrainings.Text = GetCount(
                "SELECT COUNT(*) FROM TrainingDetails " +
                "WHERE ISNULL(WorkflowStatus,'') LIKE '%E%' " +
                "AND TRY_CONVERT(date,DateTo,105) >= CAST(GETDATE() AS DATE)");

            lblStartingSoon.Text = GetCount(
                "SELECT COUNT(*) FROM TrainingDetails " +
                "WHERE TRY_CONVERT(date,DateFrom,105) BETWEEN CAST(GETDATE() AS DATE) AND DATEADD(DAY,7,CAST(GETDATE() AS DATE))");

            lblPendingConfirmation.Text = GetCount(
                "SELECT COUNT(*) FROM TrainingAssignment TA " +
                "INNER JOIN TrainingDetails TD ON TD.TrainingID=TA.TrainingID " +
                "LEFT JOIN TrainingProgress TP ON TP.TrainingID=TA.TrainingID AND TP.EmpID=TA.EmpID " +
                "WHERE ISNULL(TP.AttendanceConfirmed,0)=0 " +
                "AND DATEDIFF(DAY,GETDATE(),TRY_CONVERT(date,TD.DateFrom,105)) BETWEEN 0 AND 2");

            lblPendingProfileCorrections.Text = GetCount(
                 "SELECT COUNT(*) FROM EmpProfileChangeRequest WHERE Status='Pending'");

            lblPendingApproval.Text = GetCount(
                "SELECT COUNT(*) FROM QuestionBank WHERE ApprovalStatus='Pending'");

            lblPendingCertificates.Text = GetCount(
                "SELECT COUNT(*) FROM TrainingProgress " +
                "WHERE ISNULL(BatchFeedbackCompleted,0)=1 AND ISNULL(CertificateGenerated,0)=0");

            lblTrainingsThisMonth.Text = GetCount(
                "SELECT COUNT(*) FROM TrainingDetails " +
                "WHERE MONTH(TRY_CONVERT(date,DateFrom,105))=MONTH(GETDATE()) " +
                "AND YEAR(TRY_CONVERT(date,DateFrom,105))=YEAR(GETDATE())");

            lblTotalConducted.Text = GetCount(
                "SELECT COUNT(*) FROM TrainingDetails " +
                "WHERE TRY_CONVERT(date,DateTo,105) < CAST(GETDATE() AS DATE)");

            lblTotalTrained.Text = GetCount(
                "SELECT COUNT(DISTINCT EmpID) FROM TrainingProgress " +
                "WHERE ISNULL(AttendanceCompleted,0)=1");
        }


        private void BindCharts()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            DataTable dtCompany = objDB.GetDataTable(
                "SELECT ISNULL(E.EmpCompany,'Unknown') AS EmpCompany, COUNT(DISTINCT TP.EmpID) AS TrainedCount " +
                "FROM TrainingProgress TP " +
                "INNER JOIN EmpBasicMaster E ON E.EmpID = TP.EmpID " +
                "WHERE ISNULL(TP.AttendanceCompleted,0)=1 " +
                "GROUP BY E.EmpCompany " +
                "ORDER BY TrainedCount DESC", null);

            List<string> companyLabels = new List<string>();
            List<int> companyValues = new List<int>();

            foreach (DataRow dr in dtCompany.Rows)
            {
                companyLabels.Add(dr["EmpCompany"].ToString());
                companyValues.Add(Convert.ToInt32(dr["TrainedCount"]));
            }

            CompanyChartLabels = serializer.Serialize(companyLabels);
            CompanyChartValues = serializer.Serialize(companyValues);

            DataTable dtCourse = objDB.GetDataTable(
                "SELECT TOP 10 CM.CourseName, COUNT(DISTINCT TP.EmpID) AS TrainedCount " +
                "FROM TrainingProgress TP " +
                "INNER JOIN TrainingDetails TD ON TD.TrainingID = TP.TrainingID " +
                "INNER JOIN CourseMaster CM ON CM.CourseID = TD.CourseID " +
                "WHERE ISNULL(TP.AttendanceCompleted,0)=1 " +
                "GROUP BY CM.CourseName " +
                "ORDER BY TrainedCount DESC", null);

            List<string> courseLabels = new List<string>();
            List<int> courseValues = new List<int>();

            foreach (DataRow dr in dtCourse.Rows)
            {
                courseLabels.Add(dr["CourseName"].ToString());
                courseValues.Add(Convert.ToInt32(dr["TrainedCount"]));
            }

            CourseChartLabels = serializer.Serialize(courseLabels);
            CourseChartValues = serializer.Serialize(courseValues);

            DataTable dtYear = objDB.GetDataTable(
                "SELECT YEAR(TRY_CONVERT(date,TD.DateFrom,105)) AS TrainingYear, COUNT(DISTINCT TP.EmpID) AS TrainedCount " +
                "FROM TrainingProgress TP " +
                "INNER JOIN TrainingDetails TD ON TD.TrainingID = TP.TrainingID " +
                "WHERE ISNULL(TP.AttendanceCompleted,0)=1 " +
                "AND TRY_CONVERT(date,TD.DateFrom,105) IS NOT NULL " +
                "GROUP BY YEAR(TRY_CONVERT(date,TD.DateFrom,105)) " +
                "ORDER BY TrainingYear", null);

            List<string> yearLabels = new List<string>();
            List<int> yearValues = new List<int>();

            foreach (DataRow dr in dtYear.Rows)
            {
                yearLabels.Add(dr["TrainingYear"].ToString());
                yearValues.Add(Convert.ToInt32(dr["TrainedCount"]));
            }

            YearChartLabels = serializer.Serialize(yearLabels);
            YearChartValues = serializer.Serialize(yearValues);
        }
        protected void Tile_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName != "DrillDown")
            {
                return;
            }

            string sql;
            string title;

            switch (e.CommandArgument.ToString())
            {
                case "ActiveTrainings":
                    title = "Active Trainings";
                    sql =
                        "SELECT TD.TrainingID, CM.CourseName, TD.Batch, " +
                        "TRY_CONVERT(date,TD.DateFrom,105) AS DateFrom, TRY_CONVERT(date,TD.DateTo,105) AS DateTo, TD.TrainingLocation " +
                        "FROM TrainingDetails TD " +
                        "INNER JOIN CourseMaster CM ON CM.CourseID = TD.CourseID " +
                        "WHERE ISNULL(TD.WorkflowStatus,'') LIKE '%E%' " +
                        "AND TRY_CONVERT(date,TD.DateTo,105) >= CAST(GETDATE() AS DATE) " +
                        "ORDER BY TRY_CONVERT(date,TD.DateFrom,105)";
                    break;

                case "StartingSoon":
                    title = "Trainings Starting in 7 Days";
                    sql =
                        "SELECT TD.TrainingID, CM.CourseName, TD.Batch, " +
                        "TRY_CONVERT(date,TD.DateFrom,105) AS DateFrom, TRY_CONVERT(date,TD.DateTo,105) AS DateTo, TD.TrainingLocation " +
                        "FROM TrainingDetails TD " +
                        "INNER JOIN CourseMaster CM ON CM.CourseID = TD.CourseID " +
                        "WHERE TRY_CONVERT(date,TD.DateFrom,105) BETWEEN CAST(GETDATE() AS DATE) AND DATEADD(DAY,7,CAST(GETDATE() AS DATE)) " +
                        "ORDER BY TRY_CONVERT(date,TD.DateFrom,105)";
                    break;

                case "ThisMonth":
                    title = "Trainings This Month";
                    sql =
                        "SELECT TD.TrainingID, CM.CourseName, TD.Batch, " +
                        "TRY_CONVERT(date,TD.DateFrom,105) AS DateFrom, TRY_CONVERT(date,TD.DateTo,105) AS DateTo, TD.TrainingLocation " +
                        "FROM TrainingDetails TD " +
                        "INNER JOIN CourseMaster CM ON CM.CourseID = TD.CourseID " +
                        "WHERE MONTH(TRY_CONVERT(date,TD.DateFrom,105))=MONTH(GETDATE()) " +
                        "AND YEAR(TRY_CONVERT(date,TD.DateFrom,105))=YEAR(GETDATE()) " +
                        "ORDER BY TRY_CONVERT(date,TD.DateFrom,105)";
                    break;

                case "Confirmation":

                    title = "Attendance Confirmation Pending";
                    sql =
                        "SELECT TA.EmpID, E.EmpName, TD.TrainingID, CM.CourseName, TD.Batch, " +
                        "TRY_CONVERT(date,TD.DateFrom,105) AS DateFrom " +
                        "FROM TrainingAssignment TA " +
                        "INNER JOIN TrainingDetails TD ON TD.TrainingID=TA.TrainingID " +
                        "INNER JOIN CourseMaster CM ON CM.CourseID=TD.CourseID " +
                        "LEFT JOIN TrainingProgress TP ON TP.TrainingID=TA.TrainingID AND TP.EmpID=TA.EmpID " +
                        "LEFT JOIN EmpBasicMaster E ON E.EmpID=TA.EmpID " +
                        "WHERE ISNULL(TP.AttendanceConfirmed,0)=0 " +
                        "AND DATEDIFF(DAY,GETDATE(),TRY_CONVERT(date,TD.DateFrom,105)) BETWEEN 0 AND 2 " +
                        "ORDER BY TRY_CONVERT(date,TD.DateFrom,105)";
                    break;

                case "Certificates":
                    title = "Certificates Pending Generation";
                    sql =
                        "SELECT TP.EmpID, E.EmpName, TP.TrainingID, CM.CourseName, TD.Batch " +
                        "FROM TrainingProgress TP " +
                        "INNER JOIN TrainingDetails TD ON TD.TrainingID=TP.TrainingID " +
                        "INNER JOIN CourseMaster CM ON CM.CourseID=TD.CourseID " +
                        "LEFT JOIN EmpBasicMaster E ON E.EmpID=TP.EmpID " +
                        "WHERE ISNULL(TP.BatchFeedbackCompleted,0)=1 AND ISNULL(TP.CertificateGenerated,0)=0";
                    break;

                default:
                    return;
            }

            DataTable dt = objDB.GetDataTable(sql, null);

            gvDrillDown.DataSource = dt;
            gvDrillDown.DataBind();

            lblDrillDownTitle.Text = title;

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showDrillDown",
                    "window.addEventListener('load', function(){ $('#drillDownModal').modal('show'); });", true);
        }

        private string GetCount(string sql)
        {
            object result = objDB.ExecuteScalar(sql, null);

            if (result == null)
            {
                return "0";
            }

            return result.ToString();
        }

        //---------------------------------------------------------
        // NEEDS ATTENTION
        //---------------------------------------------------------

        private void BindNeedsAttention()
        {
            string sql =
                "SELECT " +
                "TD.TrainingID," +
                "CM.CourseName," +
                "TD.Batch," +
                "TRY_CONVERT(date,TD.DateFrom,105) AS DateFrom," +
                "CASE WHEN NOT (ISNULL(TD.WorkflowStatus,'') LIKE '%B%' AND ISNULL(TD.WorkflowStatus,'') LIKE '%C%') " +
                "THEN 1 ELSE 0 END AS MissingSessions," +
                "CASE WHEN ISNULL(TD.WorkflowStatus,'') NOT LIKE '%D%' " +
                "THEN 1 ELSE 0 END AS MissingTrainees," +
                "CASE WHEN TD.HostelRequiredTrainee='Yes' AND NOT EXISTS " +
                "(SELECT 1 FROM HostelAllotment HA WHERE HA.TrainingID=TD.TrainingID AND HA.Status='Allotted') " +
                "THEN 1 ELSE 0 END AS MissingHostel," +
                "CASE WHEN NOT EXISTS " +
                "(SELECT 1 FROM TrainingCertificateTemplate TCT WHERE TCT.TrainingID=TD.TrainingID " +
                "AND ISNULL(TCT.TemplateID,'')<>'' AND ISNULL(TCT.CourseTitle,'')<>'') " +
                "THEN 1 ELSE 0 END AS MissingCertificate " +
                "FROM TrainingDetails TD " +
                "INNER JOIN CourseMaster CM ON TD.CourseID=CM.CourseID " +
                "WHERE TRY_CONVERT(date,TD.DateTo,105) >= CAST(GETDATE() AS DATE)";

            DataTable dt = objDB.GetDataTable(sql, new SqlParameter[0]);

            DataTable dtFiltered = new DataTable();

            dtFiltered.Columns.Add("TrainingID");
            dtFiltered.Columns.Add("CourseName");
            dtFiltered.Columns.Add("Batch");
            dtFiltered.Columns.Add("DateFrom", typeof(DateTime));
            dtFiltered.Columns.Add("Issues");

            foreach (DataRow dr in dt.Rows)
            {
                List<string> issues = new List<string>();

                if (Convert.ToInt32(dr["MissingSessions"]) == 1)
                {
                    issues.Add("Sessions/Trainers not assigned");
                }

                if (Convert.ToInt32(dr["MissingTrainees"]) == 1)
                {
                    issues.Add("No trainees assigned");
                }

                if (Convert.ToInt32(dr["MissingHostel"]) == 1)
                {
                    issues.Add("Hostel not allotted");
                }

                if (Convert.ToInt32(dr["MissingCertificate"]) == 1)
                {
                    issues.Add("Certificate template not set");
                }

                if (issues.Count > 0)
                {
                    DataRow newRow = dtFiltered.NewRow();

                    newRow["TrainingID"] = dr["TrainingID"];
                    newRow["CourseName"] = dr["CourseName"];
                    newRow["Batch"] = dr["Batch"];
                    newRow["DateFrom"] = dr["DateFrom"];
                    newRow["Issues"] = string.Join(", ", issues);

                    dtFiltered.Rows.Add(newRow);
                }
            }

            gvNeedsAttention.DataSource = dtFiltered;
            gvNeedsAttention.DataBind();
        }

        protected void gvNeedsAttention_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewTraining")
            {
                Session["TrainingID"] = e.CommandArgument.ToString();

                Response.Redirect("ManageTraining.aspx", false);
            }
        }

        //---------------------------------------------------------
        // UPCOMING TRAININGS
        //---------------------------------------------------------

        private void BindUpcomingTrainings()
        {
            string sql =
                "SELECT " +
                "TD.TrainingID," +
                "CM.CourseName," +
                "TD.Batch," +
                "TRY_CONVERT(date,TD.DateFrom,105) AS DateFrom," +
                "TRY_CONVERT(date,TD.DateTo,105) AS DateTo," +
                "ISNULL(TD.WorkflowStatus,'') AS WorkflowStatus " +
                "FROM TrainingDetails TD " +
                "INNER JOIN CourseMaster CM ON TD.CourseID=CM.CourseID " +
                "WHERE TRY_CONVERT(date,TD.DateFrom,105) BETWEEN CAST(GETDATE() AS DATE) AND DATEADD(DAY,14,CAST(GETDATE() AS DATE)) " +
                "ORDER BY TRY_CONVERT(date,TD.DateFrom,105)";

            DataTable dt = objDB.GetDataTable(sql, new SqlParameter[0]);
            gvUpcoming.DataSource = dt;
            gvUpcoming.DataBind();
        }

        private void BindHostelCards()
        {
            string sql =                            
                "SELECT H.HostelName, HB.ID AS BlockID, HB.BlockName, " +
                "COUNT(HBed.ID) AS TotalBeds, " +
                "SUM(CASE WHEN HBed.Status = 'Occupied' THEN 1 ELSE 0 END) AS OccupiedBeds, " +
                "SUM(CASE WHEN HBed.Status = 'Vacant' THEN 1 ELSE 0 END) AS VacantBeds " +
                "FROM HostelMaster H " +
                "INNER JOIN HostelBlockMaster HB ON HB.HostelID = H.ID AND HB.IsActive = 'Y' " +
                "INNER JOIN HostelRoomMaster HR ON HR.BlockID = HB.ID " +
                "INNER JOIN HostelBedMaster HBed ON HBed.RoomID = HR.ID AND HBed.IsActive = 'Y' " +
                "WHERE H.IsActive = 'Y' " +
                "GROUP BY H.HostelName, HB.ID, HB.BlockName " +
                "ORDER BY H.HostelName, HB.BlockName";

            DataTable dt = objDB.GetDataTable(sql, null);

            rptHostelStatus.DataSource = dt;
            rptHostelStatus.DataBind();

            lblNoHostel.Visible = dt.Rows.Count == 0;
        }

        protected void rptHostelStatus_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "DrillDownHostel")
            {
                return;
            }

            string blockID = e.CommandArgument.ToString();

            string sql =
                "SELECT HBed.* " +
                "FROM HostelBedMaster HBed " +
                "INNER JOIN HostelRoomMaster HR ON HR.ID = HBed.RoomID " +
                "WHERE HR.BlockID=@BlockID AND HBed.Status='Vacant' AND HBed.IsActive='Y'";

            SqlParameter[] param = { new SqlParameter("@BlockID", blockID) };

            DataTable dt = objDB.GetDataTable(sql, param);

            gvDrillDown.DataSource = dt;
            gvDrillDown.DataBind();

            lblDrillDownTitle.Text = "Vacant Beds";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showDrillDown", 
                                                     "window.addEventListener('load', function(){ $('#drillDownModal').modal('show'); });", true);
        }
    }
}