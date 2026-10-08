using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainee
{
    public partial class MyTrainings : System.Web.UI.Page
    {
        clsDataAccess objDB = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["EmpID"] == null ||
                string.IsNullOrWhiteSpace(Session["EmpID"].ToString()) ||
                Session["Role"] == null ||
                !string.Equals(Session["Role"].ToString(), "Trainee", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadCourse();
                LoadStatus();
                LoadProfileDetails();
                LoadCorrectionForm();

                ViewState["SortExpression"] = "TrainingID";
                ViewState["SortDirection"] = "DESC";

                ddlStatus.SelectedIndex = 0;

                LoadTraining();
            }
        }

        private string EmpID
        {
            get
            {
                return Session["EmpID"] == null
                    ? ""
                    : Session["EmpID"].ToString().Trim().ToUpperInvariant();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            gvTraining.PageIndex = 0;
            LoadTraining();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtTrainingID.Text = "";
            ddlCourse.SelectedIndex = 0;
            ddlStatus.SelectedIndex = 0;

            ViewState["SortExpression"] = "TrainingID";
            ViewState["SortDirection"] = "DESC";

            gvTraining.PageIndex = 0;

            LoadTraining();
        }

        private string SortColumn()
        {
            string sortColumn = Convert.ToString(ViewState["SortExpression"]);

            switch (sortColumn)
            {
                case "TrainingID":
                case "CourseName":
                case "TrainingType":
                case "TrainingOrganizer":
                case "Batch":
                case "DateFrom":
                case "DateTo":
                    return sortColumn;

                default:
                    return "TrainingID";
            }
        }

        private string SortDirection()
        {
            return Convert.ToString(ViewState["SortDirection"]) == "ASC"
                ? "ASC"
                : "DESC";
        }

        protected void gvTraining_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTraining.PageIndex = e.NewPageIndex;
            LoadTraining();
        }

        protected void gvTraining_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (Convert.ToString(ViewState["SortExpression"]) == e.SortExpression)
            {
                ViewState["SortDirection"] =
                    Convert.ToString(ViewState["SortDirection"]) == "ASC"
                    ? "DESC"
                    : "ASC";
            }
            else
            {
                ViewState["SortExpression"] = e.SortExpression;
                ViewState["SortDirection"] = "ASC";
            }

            LoadTraining();
        }

        private void LoadStatus()
        {
            ddlStatus.Items.Clear();
            ddlStatus.Items.Add(new ListItem("All", ""));
            ddlStatus.Items.Add(new ListItem("Pending", "Pending"));
            ddlStatus.Items.Add(new ListItem("In Progress", "In Progress"));
            ddlStatus.Items.Add(new ListItem("Completed", "Completed"));
        }

        private void LoadCourse()
        {
            DataTable dt = objDB.GetDataTable(
                "SELECT DISTINCT CM.CourseID,CM.CourseName FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TA.TrainingID=TD.TrainingID LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE TA.EmpID=@EmpID ORDER BY CM.CourseName",
                new SqlParameter[]
                {
                    new SqlParameter("@EmpID", EmpID)
                });

            ddlCourse.DataSource = dt;
            ddlCourse.DataTextField = "CourseName";
            ddlCourse.DataValueField = "CourseID";
            ddlCourse.DataBind();

            ddlCourse.Items.Insert(0, new ListItem("All", ""));
        }

        private void LoadTraining()
        {
            string sql =
                "SELECT " +
                "TA.TrainingID," +
                "ISNULL(CM.CourseName,'') AS CourseName," +
                "TD.TrainingType," +
                "TD.TrainingOrganizer," +
                "TD.Batch," +
                "TRY_CONVERT(date,TD.DateFrom,105) AS DateFrom," +
                "TRY_CONVERT(date,TD.DateTo,105) AS DateTo," +
                "ISNULL(TD.AttendanceRequired,0) AS AttendanceRequired," +
                "ISNULL(TD.InitialAssessmentRequired,0) AS InitialAssessmentRequired," +
                "ISNULL(TD.FinalAssessmentRequired,0) AS FinalAssessmentRequired," +
                "ISNULL(TD.FeedbackRequired,0) AS FeedbackRequired," +
                "ISNULL(TD.FeedbackSkipped,0) AS FeedbackSkipped," +
                "ISNULL(TD.CertificateRequired,0) AS CertificateRequired," +
                "ISNULL(TD.CertificateSkipped,0) AS CertificateSkipped," +
                "TD.MinimumAttendancePercentage," +
                "ISNULL(TD.TrainingStatus,'') AS TrainingStatus " +
                "FROM TrainingAssignment TA " +
                "INNER JOIN TrainingDetails TD ON TD.TrainingID=TA.TrainingID " +
                "LEFT JOIN CourseMaster CM ON CM.CourseID=TD.CourseID " +
                "WHERE LTRIM(RTRIM(TA.EmpID))=LTRIM(RTRIM(@EmpID)) ";

            if (txtTrainingID.Text.Trim() != "")
            {
                sql += " AND TA.TrainingID LIKE @TrainingID ";
            }

            if (ddlCourse.SelectedValue != "")
            {
                sql += " AND TD.CourseID=@CourseID ";
            }

            sql += " ORDER BY " + SortColumn() + " " + SortDirection();

            SqlParameter[] parameters =
            {
                new SqlParameter("@EmpID", EmpID),
                new SqlParameter("@TrainingID", "%" + txtTrainingID.Text.Trim() + "%"),
                new SqlParameter("@CourseID", ddlCourse.SelectedValue)
            };

            DataTable dt = objDB.GetDataTable(sql, parameters);

            dt.Columns.Add("AttendanceDone", typeof(bool));
            dt.Columns.Add("PreDone", typeof(bool));
            dt.Columns.Add("PostDone", typeof(bool));
            dt.Columns.Add("FeedbackDone", typeof(bool));
            dt.Columns.Add("CertificateDone", typeof(bool));
            dt.Columns.Add("AttendanceConfirmed", typeof(bool));
            dt.Columns.Add("CanConfirmAttendance", typeof(bool));
            dt.Columns.Add("ProgressPercent", typeof(int));
            dt.Columns.Add("StatusText");
            dt.Columns.Add("StatusClass");

            foreach (DataRow row in dt.Rows)
            {
                string trainingID = Convert.ToString(row["TrainingID"]);

                bool attendanceRequired =
                    Convert.ToBoolean(row["AttendanceRequired"]);

                bool preRequired =
                    Convert.ToBoolean(row["InitialAssessmentRequired"]);

                bool postRequired =
                    Convert.ToBoolean(row["FinalAssessmentRequired"]);

                bool feedbackRequired =
                    Convert.ToBoolean(row["FeedbackRequired"]);

                bool feedbackSkipped =
                    Convert.ToBoolean(row["FeedbackSkipped"]);

                bool certificateRequired =
                    Convert.ToBoolean(row["CertificateRequired"]);

                bool certificateSkipped =
                    Convert.ToBoolean(row["CertificateSkipped"]);

                bool attendanceDone =
                    !attendanceRequired ||
                    IsAttendancePercentageEligible(
                        trainingID,
                        EmpID,
                        row["MinimumAttendancePercentage"]);

                bool preDone =
                    !preRequired ||
                    IsPreTestCompleted(trainingID, EmpID);

                bool postDone =
                    !postRequired ||
                    IsPostTestCompleted(trainingID, EmpID);

                bool feedbackDone =
                    !feedbackRequired ||
                    feedbackSkipped ||
                    IsFeedbackCompleted(trainingID, EmpID);

                bool certificateDone =
                    !certificateRequired ||
                    certificateSkipped ||
                    IsCertificateGenerated(trainingID, EmpID);

                row["AttendanceDone"] = attendanceDone;
                row["PreDone"] = preDone;
                row["PostDone"] = postDone;
                row["FeedbackDone"] = feedbackDone;
                row["CertificateDone"] = certificateDone;

                bool attendanceConfirmed =
                    IsAttendanceConfirmed(trainingID, EmpID);

                row["AttendanceConfirmed"] = attendanceConfirmed;

                bool canConfirmAttendance =
                    CanConfirmAttendance(
                        trainingID,
                        attendanceConfirmed);

                row["CanConfirmAttendance"] =
                    canConfirmAttendance;

                int done = 0;
                int total = 0;

                if (attendanceRequired)
                {
                    total++;

                    if (attendanceDone)
                    {
                        done++;
                    }
                }

                if (preRequired)
                {
                    total++;

                    if (preDone)
                    {
                        done++;
                    }
                }

                if (postRequired)
                {
                    total++;

                    if (postDone)
                    {
                        done++;
                    }
                }

                if (feedbackRequired && !feedbackSkipped)
                {
                    total++;

                    if (feedbackDone)
                    {
                        done++;
                    }
                }

                if (certificateRequired && !certificateSkipped)
                {
                    total++;

                    if (certificateDone)
                    {
                        done++;
                    }
                }

                row["ProgressPercent"] =
                    total == 0
                    ? 100
                    : done * 100 / total;

                if (done == total)
                {
                    row["StatusText"] = "Completed";
                    row["StatusClass"] =
                        "badge badge-success badge-status";
                }
                else if (done > 0)
                {
                    row["StatusText"] = "In Progress";
                    row["StatusClass"] =
                        "badge badge-warning badge-status";
                }
                else
                {
                    row["StatusText"] = "Pending";
                    row["StatusClass"] =
                        "badge badge-secondary badge-status";
                }
            }

            string selectedStatus = ddlStatus.SelectedValue;

            if (selectedStatus != "")
            {
                DataView view = dt.DefaultView;
                view.RowFilter = "StatusText = '" + selectedStatus.Replace("'", "''") + "'";
                gvTraining.DataSource = view;
            }
            else
            {
                gvTraining.DataSource = dt;
            }

            gvTraining.DataBind();
        }

        private bool IsAttendanceConfirmed(string trainingID, string empID)
        {
            object value = objDB.ExecuteScalar(
                "SELECT ISNULL(AttendanceConfirmed,0) FROM TrainingProgress WHERE TrainingID=@TrainingID AND EmpID=@EmpID",
                new SqlParameter[]
                {
                    new SqlParameter("@TrainingID", trainingID),
                    new SqlParameter("@EmpID", empID)
                });

            if (value == null || value == DBNull.Value)
            {
                return false;
            }

            return Convert.ToBoolean(value);
        }

        private bool CanConfirmAttendance(
            string trainingID,
            bool attendanceConfirmed)
        {
            if (attendanceConfirmed)
            {
                return false;
            }

            object dateValue = objDB.ExecuteScalar(
                "SELECT TRY_CONVERT(date,DateFrom,105) FROM TrainingDetails WHERE TrainingID=@TrainingID",
                new SqlParameter[]
                {
                    new SqlParameter("@TrainingID", trainingID)
                });

            if (dateValue == null || dateValue == DBNull.Value)
            {
                return false;
            }

            DateTime trainingDate;

            if (!DateTime.TryParse(
                dateValue.ToString(),
                out trainingDate))
            {
                return false;
            }

            int days =
                (trainingDate.Date - DateTime.Today).Days;

            return days >= 0 && days <= 2;
        }

        private void ConfirmAttendance(string trainingID)
        {
            object exists = objDB.ExecuteScalar(
                "SELECT COUNT(*) FROM TrainingProgress WHERE TrainingID=@TrainingID AND EmpID=@EmpID",
                new SqlParameter[]
                {
                    new SqlParameter("@TrainingID", trainingID),
                    new SqlParameter("@EmpID", EmpID)
                });

            if (exists == null ||
                exists == DBNull.Value ||
                Convert.ToInt32(exists) == 0)
            {
                return;
            }

            objDB.ExecuteSql(
                "UPDATE TrainingProgress SET AttendanceConfirmed=1,AttendanceConfirmedOn=GETDATE() WHERE TrainingID=@TrainingID AND EmpID=@EmpID",
                new SqlParameter[]
                {
                    new SqlParameter("@TrainingID", trainingID),
                    new SqlParameter("@EmpID", EmpID)
                });
        }

        protected void gvTraining_RowDataBound(
            object sender,
            GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            LinkButton feedback =
                e.Row.FindControl("lnkFeedback") as LinkButton;

            LinkButton certificate =
                e.Row.FindControl("lnkCertificate") as LinkButton;

            LinkButton attendance =
                e.Row.FindControl("lnkAttendance") as LinkButton;

            LinkButton confirm =
                e.Row.FindControl("lnkConfirmAttendance") as LinkButton;

            Label confirmed =
                e.Row.FindControl("lblConfirmed") as Label;

            DataRowView data =
                e.Row.DataItem as DataRowView;

            if (data == null)
            {
                return;
            }

            bool attendanceRequired =
                Convert.ToBoolean(data["AttendanceRequired"]);

            bool preRequired =
                Convert.ToBoolean(data["InitialAssessmentRequired"]);

            bool postRequired =
                Convert.ToBoolean(data["FinalAssessmentRequired"]);

            bool feedbackRequired =
                Convert.ToBoolean(data["FeedbackRequired"]);

            bool feedbackSkipped =
                Convert.ToBoolean(data["FeedbackSkipped"]);

            bool certificateRequired =
                Convert.ToBoolean(data["CertificateRequired"]);

            bool certificateSkipped =
                Convert.ToBoolean(data["CertificateSkipped"]);

            bool attendanceDone =
                Convert.ToBoolean(data["AttendanceDone"]);

            bool preDone =
                Convert.ToBoolean(data["PreDone"]);

            bool postDone =
                Convert.ToBoolean(data["PostDone"]);

            bool feedbackDone =
                Convert.ToBoolean(data["FeedbackDone"]);

            bool attendanceConfirmed =
                Convert.ToBoolean(data["AttendanceConfirmed"]);

            if (feedback != null)
            {
                if (!feedbackRequired || feedbackSkipped)
                {
                    feedback.Visible = false;
                }
                else if (feedbackDone)
                {
                    feedback.Text = "Feedback";
                    feedback.Enabled = true;
                    feedback.CssClass =
                        "btn btn-success btn-sm";
                    feedback.ToolTip =
                        "View submitted feedback.";
                }
                else if (attendanceRequired && !attendanceDone)
                {
                    feedback.Enabled = false;
                    feedback.CssClass =
                        "btn btn-warning btn-sm disabled";
                    feedback.ToolTip =
                        "Complete required attendance first.";
                }
                else if (preRequired && !preDone)
                {
                    feedback.Enabled = false;
                    feedback.CssClass =
                        "btn btn-warning btn-sm disabled";
                    feedback.ToolTip =
                        "Complete all required Pre-Training Tests first.";
                }
                else if (postRequired && !postDone)
                {
                    feedback.Enabled = false;
                    feedback.CssClass =
                        "btn btn-warning btn-sm disabled";
                    feedback.ToolTip =
                        "Complete all required Post-Training Tests first.";
                }
                else
                {
                    feedback.Enabled = true;
                    feedback.CssClass =
                        "btn btn-warning btn-sm";
                    feedback.Text = "Feedback";
                    feedback.ToolTip =
                        "Submit Feedback.";
                }
            }

            if (certificate != null)
            {
                if (!certificateRequired || certificateSkipped)
                {
                    certificate.Visible = false;
                }
                else
                {
                    bool allowed =
                        attendanceRequired
                        ? attendanceDone
                        : true;

                    if (feedbackRequired &&
                        !feedbackSkipped)
                    {
                        allowed =
                            allowed &&
                            feedbackDone;
                    }

                    if (allowed)
                    {
                        allowed =
                            IsCertificateTestEligible(
                                Convert.ToString(data["TrainingID"]),
                                EmpID);
                    }

                    certificate.Enabled = allowed;

                    certificate.CssClass =
                        allowed
                        ? "btn btn-info btn-sm"
                        : "btn btn-info btn-sm disabled";

                    certificate.ToolTip =
                        allowed
                        ? "Download Certificate"
                        : "Complete the required training workflow before downloading the certificate.";
                }
            }

            if (attendance != null &&
                !attendance.Enabled)
            {
                attendance.CssClass =
                    "btn btn-primary btn-sm disabled";
            }

            if (confirm != null &&
                confirmed != null)
            {
                if (attendanceConfirmed)
                {
                    confirm.Visible = false;
                    confirmed.Visible = true;
                }
                else
                {
                    confirm.Visible = true;
                    confirmed.Visible = false;

                    if (!confirm.Enabled)
                    {
                        confirm.CssClass =
                            "btn btn-secondary btn-sm disabled";
                    }
                }
            }
        }

        protected void gvTraining_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            string trainingID =
                Convert.ToString(e.CommandArgument);

            if (string.IsNullOrWhiteSpace(trainingID))
            {
                return;
            }

            Session["TrainingID"] = trainingID;

            if (e.CommandName == "ViewTraining")
            {
                Response.Redirect(
                    "TrainingDetails.aspx",
                    false);

                return;
            }

            if (e.CommandName == "Attendance")
            {
                Response.Redirect(
                    "Attendance.aspx",
                    false);

                return;
            }

            if (e.CommandName == "BatchFeedback")
            {
                object feedbackValue =
                    objDB.ExecuteScalar(
                        "SELECT COUNT(*) FROM Feedback WHERE TrainingID=@TrainingID AND EmpID=@EmpID AND ISNULL(Submitted,0)=1",
                        new SqlParameter[]
                        {
                            new SqlParameter("@TrainingID", trainingID),
                            new SqlParameter("@EmpID", EmpID)
                        });

                bool feedbackSubmitted =
                    feedbackValue != null &&
                    feedbackValue != DBNull.Value &&
                    Convert.ToInt32(feedbackValue) > 0;

                Response.Redirect(
                    feedbackSubmitted
                    ? "TraineeFeedback.aspx?mode=view"
                    : "TraineeFeedback.aspx",
                    false);

                return;
            }

            if (e.CommandName == "Certificate")
            {
                Session["CertificateFromTraining"] = true;

                Response.Redirect(
                    "MyCertificate.aspx",
                    false);

                return;
            }

            if (e.CommandName == "ConfirmAttendance")
            {
                ConfirmAttendance(trainingID);

                LoadTraining();

                return;
            }
        }

        private bool IsAttendancePercentageEligible(
            string trainingID,
            string empID,
            object minimumValue)
        {
            object totalValue =
                objDB.ExecuteScalar(
                    "SELECT COUNT(*) FROM SessionMaster WHERE TrainingID=@TrainingID AND ISNULL(AttendanceSkipped,0)=0",
                    new SqlParameter[]
                    {
                        new SqlParameter("@TrainingID", trainingID)
                    });

            int total =
                totalValue == null ||
                totalValue == DBNull.Value
                ? 0
                : Convert.ToInt32(totalValue);

            if (total == 0)
            {
                return true;
            }

            if (minimumValue == null ||
                minimumValue == DBNull.Value ||
                string.IsNullOrWhiteSpace(minimumValue.ToString()))
            {
                return false;
            }

            decimal minimum;

            if (!decimal.TryParse(
                minimumValue.ToString(),
                out minimum))
            {
                return false;
            }

            object presentValue =
                objDB.ExecuteScalar(
                    "SELECT COUNT(*) FROM SessionMaster SM INNER JOIN SessionAttendance SA ON SA.SessionID=SM.SessionID AND SA.EmpID=@EmpID WHERE SM.TrainingID=@TrainingID AND ISNULL(SM.AttendanceSkipped,0)=0 AND SA.AttendanceStatus IN ('Present','Completed')",
                    new SqlParameter[]
                    {
                        new SqlParameter("@TrainingID", trainingID),
                        new SqlParameter("@EmpID", empID)
                    });

            int present =
                presentValue == null ||
                presentValue == DBNull.Value
                ? 0
                : Convert.ToInt32(presentValue);

            decimal percentage =
                present * 100m / total;

            return percentage >= minimum;
        }

        private bool IsPreTestCompleted(
            string trainingID,
            string empID)
        {
            object value =
                objDB.ExecuteScalar(
                    "SELECT COUNT(*) FROM SessionMaster SM WHERE SM.TrainingID=@TrainingID AND ISNULL(SM.PreAssessmentSkipped,0)=0 AND (NOT EXISTS (SELECT 1 FROM TestMaster TM WHERE TM.SessionID=SM.SessionID AND TM.TestType='Pre' AND TM.IsPublished=1) OR NOT EXISTS (SELECT 1 FROM TestMaster TM INNER JOIN TestAttempt AT ON AT.TestID=TM.TestID WHERE TM.SessionID=SM.SessionID AND TM.TestType='Pre' AND TM.IsPublished=1 AND AT.EmpID=@EmpID AND AT.Submitted=1))",
                    new SqlParameter[]
                    {
                        new SqlParameter("@TrainingID", trainingID),
                        new SqlParameter("@EmpID", empID)
                    });

            return value != null &&
                   value != DBNull.Value &&
                   Convert.ToInt32(value) == 0;
        }

        private bool IsPostTestCompleted(
            string trainingID,
            string empID)
        {
            object value =
                objDB.ExecuteScalar(
                    "SELECT COUNT(*) FROM SessionMaster SM WHERE SM.TrainingID=@TrainingID AND ISNULL(SM.PostAssessmentSkipped,0)=0 AND (NOT EXISTS (SELECT 1 FROM TestMaster TM WHERE TM.SessionID=SM.SessionID AND TM.TestType='Post' AND TM.IsPublished=1) OR NOT EXISTS (SELECT 1 FROM TestMaster TM INNER JOIN TestAttempt AT ON AT.TestID=TM.TestID WHERE TM.SessionID=SM.SessionID AND TM.TestType='Post' AND TM.IsPublished=1 AND AT.EmpID=@EmpID AND AT.Submitted=1))",
                    new SqlParameter[]
                    {
                        new SqlParameter("@TrainingID", trainingID),
                        new SqlParameter("@EmpID", empID)
                    });

            return value != null &&
                   value != DBNull.Value &&
                   Convert.ToInt32(value) == 0;
        }

        private bool IsFeedbackCompleted(
            string trainingID,
            string empID)
        {
            object value =
                objDB.ExecuteScalar(
                    "SELECT COUNT(*) FROM Feedback WHERE TrainingID=@TrainingID AND EmpID=@EmpID AND ISNULL(Submitted,0)=1",
                    new SqlParameter[]
                    {
                        new SqlParameter("@TrainingID", trainingID),
                        new SqlParameter("@EmpID", empID)
                    });

            return value != null &&
                   value != DBNull.Value &&
                   Convert.ToInt32(value) > 0;
        }

        private bool IsCertificateGenerated(
            string trainingID,
            string empID)
        {
            object value =
                objDB.ExecuteScalar(
                    "SELECT COUNT(*) FROM TrainingCertificate WHERE TrainingID=@TrainingID AND EmpID=@EmpID AND CertificateStatus='A'",
                    new SqlParameter[]
                    {
                        new SqlParameter("@TrainingID", trainingID),
                        new SqlParameter("@EmpID", empID)
                    });

            return value != null &&
                   value != DBNull.Value &&
                   Convert.ToInt32(value) > 0;
        }

        private bool IsCertificateTestEligible(
            string trainingID,
            string empID)
        {
            DataTable dt =
                objDB.GetDataTable(
                    "SELECT InitialAssessmentRequired,FinalAssessmentRequired FROM TrainingDetails WHERE TrainingID=@TrainingID",
                    new SqlParameter[]
                    {
                        new SqlParameter("@TrainingID", trainingID)
                    });

            if (dt.Rows.Count == 0)
            {
                return false;
            }

            bool preRequired =
                Convert.ToBoolean(
                    dt.Rows[0]["InitialAssessmentRequired"]);

            bool postRequired =
                Convert.ToBoolean(
                    dt.Rows[0]["FinalAssessmentRequired"]);

            if (preRequired &&
                !AreSessionWiseTestsEligible(
                    trainingID,
                    empID,
                    "Pre"))
            {
                return false;
            }

            if (postRequired &&
                !AreSessionWiseTestsEligible(
                    trainingID,
                    empID,
                    "Post"))
            {
                return false;
            }

            return true;
        }

        private bool AreSessionWiseTestsEligible(
            string trainingID,
            string empID,
            string testType)
        {
            string skipColumn =
                testType == "Pre"
                ? "PreAssessmentSkipped"
                : "PostAssessmentSkipped";

            string ruleColumn =
                testType == "Pre"
                ? "PreTestCertificateRule"
                : "PostTestCertificateRule";

            string requiredColumn =
                testType == "Pre"
                ? "InitialAssessmentRequired"
                : "FinalAssessmentRequired";

            string sql =
                "SELECT SM.SessionID,ISNULL(SM." +
                skipColumn +
                ",0) Skipped,ISNULL(SM." +
                ruleColumn +
                ",'') RuleValue " +
                "FROM SessionMaster SM " +
                "INNER JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID " +
                "WHERE SM.TrainingID=@TrainingID " +
                "AND ISNULL(TD." +
                requiredColumn +
                ",0)=1 " +
                "ORDER BY SM.SessionID";

            DataTable sessions =
                objDB.GetDataTable(
                    sql,
                    new SqlParameter[]
                    {
                        new SqlParameter("@TrainingID", trainingID)
                    });

            foreach (DataRow session in sessions.Rows)
            {
                if (Convert.ToBoolean(session["Skipped"]))
                {
                    continue;
                }

                string ruleValue =
                    Convert.ToString(
                        session["RuleValue"])
                    .Trim()
                    .ToUpperInvariant();

                string rule =
                    ruleValue == "ALL"
                    ? "ALL"
                    : ruleValue.StartsWith("PASS|")
                        ? "PASS"
                        : ruleValue;

                if (rule != "PASS" &&
                    rule != "ALL")
                {
                    return false;
                }

                string sessionID =
                    Convert.ToString(
                        session["SessionID"]);

                object testCount =
                    objDB.ExecuteScalar(
                        "SELECT COUNT(*) FROM TestMaster WHERE SessionID=@SessionID AND TestType=@TestType AND IsPublished=1",
                        new SqlParameter[]
                        {
                            new SqlParameter("@SessionID", sessionID),
                            new SqlParameter("@TestType", testType)
                        });

                if (testCount == null ||
                    Convert.ToInt32(testCount) == 0)
                {
                    return false;
                }

                object submittedCount =
                    objDB.ExecuteScalar(
                        "SELECT COUNT(*) FROM TestMaster TM INNER JOIN TestAttempt TA ON TA.TestID=TM.TestID WHERE TM.SessionID=@SessionID AND TM.TestType=@TestType AND TM.IsPublished=1 AND TA.EmpID=@EmpID AND TA.Submitted=1",
                        new SqlParameter[]
                        {
                            new SqlParameter("@SessionID", sessionID),
                            new SqlParameter("@TestType", testType),
                            new SqlParameter("@EmpID", empID)
                        });

                if (submittedCount == null ||
                    Convert.ToInt32(submittedCount) == 0)
                {
                    return false;
                }

                if (rule == "PASS")
                {
                    decimal passingPercentage;

                    if (!TryGetPassingPercentage(
                        ruleValue,
                        out passingPercentage))
                    {
                        return false;
                    }

                    object passCount =
                        objDB.ExecuteScalar(
                            "SELECT COUNT(*) FROM TestMaster TM INNER JOIN TestResult TR ON TR.TestID=TM.TestID AND TR.EmpID=@EmpID WHERE TM.SessionID=@SessionID AND TM.TestType=@TestType AND TM.IsPublished=1 AND TR.IsFinalAttempt=1 AND ISNULL(TR.Percentage,0)>=@PassingPercentage",
                            new SqlParameter[]
                            {
                                new SqlParameter("@SessionID", sessionID),
                                new SqlParameter("@TestType", testType),
                                new SqlParameter("@EmpID", empID),
                                new SqlParameter("@PassingPercentage", passingPercentage)
                            });

                    if (passCount == null ||
                        Convert.ToInt32(passCount) == 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool TryGetPassingPercentage(
            string ruleValue,
            out decimal passingPercentage)
        {
            passingPercentage = 0;

            if (string.IsNullOrWhiteSpace(ruleValue) ||
                !ruleValue.StartsWith("PASS|"))
            {
                return false;
            }

            return decimal.TryParse(
                ruleValue.Substring(5),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out passingPercentage) &&
                passingPercentage >= 0 &&
                passingPercentage <= 100;
        }

        // ============================================================
        // TRAINEE PROFILE DETAILS
        // ============================================================

        private void LoadProfileDetails()
        {
            string empID =
                Session["EmpID"]
                .ToString()
                .Trim()
                .ToUpperInvariant();

            lblEmpID.Text = empID;

            string sql =
                "SELECT EmpName,Gender,EmpDesignation,EmpPostingPlace,EmpCompany,MobileNo,EmailId " +
                "FROM EmpBasicMaster " +
                "WHERE EmpID=@EmpID";

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    new SqlParameter[]
                    {
                        new SqlParameter("@EmpID", empID)
                    });

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                lblName.Text =
                    Convert.ToString(row["EmpName"]);

                lblGender.Text =
                    Convert.ToString(row["Gender"]);

                lblDesignation.Text =
                    Convert.ToString(row["EmpDesignation"]);

                lblPostingPlace.Text =
                    Convert.ToString(row["EmpPostingPlace"]);

                lblCompany.Text =
                    Convert.ToString(row["EmpCompany"]);

                lblMobile.Text =
                    Convert.ToString(row["MobileNo"]);

                lblEmail.Text =
                    Convert.ToString(row["EmailId"]);
            }

            LoadPostingHierarchy(empID);

            object lastConfirmed =
                objDB.ExecuteScalar(
                    "SELECT LastConfirmedOn FROM EmpProfileConfirmation WHERE EmpID=@EmpID",
                    new SqlParameter[]
                    {
                        new SqlParameter("@EmpID", empID)
                    });

            DateTime confirmedOn;

            if (lastConfirmed == null ||
                lastConfirmed == DBNull.Value ||
                !DateTime.TryParse(
                    lastConfirmed.ToString(),
                    out confirmedOn))
            {
                lblConfirmStatus.Text =
                    "Not yet confirmed - please review and confirm below.";

                lblConfirmStatus.CssClass =
                    "small text-danger";
            }
            else
            {
                lblConfirmStatus.Text =
                    "Last confirmed on " +
                    confirmedOn.ToString("dd-MMM-yyyy");

                lblConfirmStatus.CssClass =
                    "small text-success";
            }
        }

        private void LoadPostingHierarchy(string empID)
        {
            string sql =
                "SELECT AreaBoardZone,Circle,Division,Subdivision,Section " +
                "FROM EmpPostingDetails " +
                "WHERE EmpID=@EmpID";

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    new SqlParameter[]
                    {
                        new SqlParameter("@EmpID", empID)
                    });

            if (dt.Rows.Count == 0)
            {
                lblZone.Text = "-";
                lblCircle.Text = "-";
                lblDivision.Text = "-";
                lblSubDivision.Text = "-";
                lblSection.Text = "-";

                return;
            }

            DataRow row = dt.Rows[0];

            lblZone.Text =
                FormatOrDash(row["AreaBoardZone"]);

            lblCircle.Text =
                FormatOrDash(row["Circle"]);

            lblDivision.Text =
                FormatOrDash(row["Division"]);

            lblSubDivision.Text =
                FormatOrDash(row["Subdivision"]);

            lblSection.Text =
                FormatOrDash(row["Section"]);
        }

        private string FormatOrDash(object value)
        {
            if (value == null ||
                value == DBNull.Value ||
                string.IsNullOrWhiteSpace(value.ToString()))
            {
                return "-";
            }

            return value.ToString();
        }

        // ============================================================
        // PROFILE CONFIRMATION
        // ============================================================

        protected void btnConfirmProfile_Click(
            object sender,
            EventArgs e)
        {
            string sql =
                "IF EXISTS (SELECT 1 FROM EmpProfileConfirmation WHERE EmpID=@EmpID) " +
                "UPDATE EmpProfileConfirmation SET LastConfirmedOn=GETDATE() WHERE EmpID=@EmpID " +
                "ELSE " +
                "INSERT INTO EmpProfileConfirmation(EmpID,LastConfirmedOn) VALUES(@EmpID,GETDATE())";

            objDB.ExecuteSql(
                sql,
                new SqlParameter[]
                {
                    new SqlParameter("@EmpID", EmpID)
                });

            lblConfirmMessage.Text =
                "Your details have been confirmed successfully.";

            lblConfirmMessage.CssClass =
                "text-success d-block mt-2";

            LoadProfileDetails();
        }

        // ============================================================
        // CORRECTION FORM
        // ============================================================

        private void LoadCorrectionForm()
        {
            BindRequestedGender();
            BindRequestedDesignationList();

            // IMPORTANT:
            // Company dropdown must use CompanyMaster because
            // BindZone() expects CompanyID.
            BindPostingCompany();

            string empID = EmpID;

            DataTable dt =
                objDB.GetDataTable(
                    "SELECT EmpName,Gender,EmpDesignation,EmpCompany,MobileNo,EmailId,EmpPostingPlace FROM EmpBasicMaster WHERE EmpID=@EmpID",
                    new SqlParameter[]
                    {
                        new SqlParameter("@EmpID", empID)
                    });

            if (dt.Rows.Count == 0)
            {
                return;
            }

            DataRow row = dt.Rows[0];

            SetDropDownValue(
                ddlRequestedGender,
                Convert.ToString(row["Gender"]));

            SetDropDownValue(
                ddlRequestedDesignation,
                Convert.ToString(row["EmpDesignation"]));

            txtCompany.Text =
                Convert.ToString(row["EmpCompany"]);

            txtMobile.Text =
                Convert.ToString(row["MobileNo"]);

            txtEmail.Text =
                Convert.ToString(row["EmailId"]);

            lblCurrentPosting.Text =
                Convert.ToString(row["EmpPostingPlace"]);

            txtRemarks.Text = "";
            lblCorrectionMessage.Text = "";

            divPostingTypeChoice.Visible = false;
            dvHQ.Visible = false;
            dvField.Visible = false;

            ClearPostingDropdowns();
        }

        private void BindRequestedGender()
        {
            DataTable dt =
                objDB.GetDataTable(
                    "SELECT DISTINCT Gender FROM EmpBasicMaster WHERE ISNULL(Gender,'')<>'' ORDER BY Gender",
                    null);

            ddlRequestedGender.DataSource = dt;
            ddlRequestedGender.DataTextField = "Gender";
            ddlRequestedGender.DataValueField = "Gender";
            ddlRequestedGender.DataBind();
        }

        private void BindRequestedDesignationList()
        {
            DataTable dt =
                objDB.GetDataTable(
                    "SELECT DISTINCT EmpDesignation FROM EmpBasicMaster WHERE ISNULL(EmpDesignation,'')<>'' ORDER BY EmpDesignation",
                    null);

            ddlRequestedDesignation.DataSource = dt;
            ddlRequestedDesignation.DataTextField = "EmpDesignation";
            ddlRequestedDesignation.DataValueField = "EmpDesignation";
            ddlRequestedDesignation.DataBind();
        }

        private void SetDropDownValue(
            DropDownList ddl,
            string value)
        {
            if (ddl == null)
            {
                return;
            }

            ListItem item =
                ddl.Items.FindByValue(value);

            if (item != null)
            {
                ddl.ClearSelection();
                item.Selected = true;
            }
        }

        // ============================================================
        // COMPANY / POSTING HIERARCHY
        // ============================================================

        private void BindPostingCompany()
        {
            string sql =
                "SELECT CompanyID,CompanyName FROM CompanyMaster ORDER BY CompanyName";

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    null);

            ddlPostingCompany.DataSource = dt;
            ddlPostingCompany.DataTextField = "CompanyName";
            ddlPostingCompany.DataValueField = "CompanyID";
            ddlPostingCompany.DataBind();

            ddlPostingCompany.Items.Insert(
                0,
                new ListItem("Select a Company", ""));
        }

        private void BindDepartment()
        {
            string sql =
                "SELECT DepartmentID,DepartmentName FROM DepartmentMaster ORDER BY DepartmentName";

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    null);

            ddlDepartment.DataSource = dt;
            ddlDepartment.DataTextField = "DepartmentName";
            ddlDepartment.DataValueField = "DepartmentID";
            ddlDepartment.DataBind();

            ddlDepartment.Items.Insert(
                0,
                new ListItem("Select Department/Cell/Office", ""));
        }

        private void BindZone(string companyID)
        {
            ddlZone.Items.Clear();

            if (!string.IsNullOrWhiteSpace(companyID))
            {
                string sql =
                    "SELECT ZoneID,ZoneName FROM ZoneMaster WHERE CompanyID=@CompanyID ORDER BY ZoneName";

                SqlParameter[] param =
                {
                    new SqlParameter("@CompanyID", companyID)
                };

                DataTable dt =
                    objDB.GetDataTable(
                        sql,
                        param);

                ddlZone.DataTextField = "ZoneName";
                ddlZone.DataValueField = "ZoneID";
                ddlZone.DataSource = dt;
                ddlZone.DataBind();
            }

            ddlZone.Items.Insert(
                0,
                new ListItem("Select Areaboard/Zone", ""));
        }

        private void BindCircle(string zoneID)
        {
            ddlCircle.Items.Clear();

            if (!string.IsNullOrWhiteSpace(zoneID))
            {
                string sql =
                    "SELECT CircleID,CircleName FROM CircleMaster WHERE ZoneID=@ZoneID ORDER BY CircleName";

                SqlParameter[] param =
                {
                    new SqlParameter("@ZoneID", zoneID)
                };

                DataTable dt =
                    objDB.GetDataTable(
                        sql,
                        param);

                ddlCircle.DataTextField = "CircleName";
                ddlCircle.DataValueField = "CircleID";
                ddlCircle.DataSource = dt;
                ddlCircle.DataBind();
            }

            ddlCircle.Items.Insert(
                0,
                new ListItem("Select a Circle", ""));
        }

        private void BindDivision(string circleID)
        {
            ddlDivision.Items.Clear();

            if (!string.IsNullOrWhiteSpace(circleID))
            {
                string sql =
                    "SELECT DivisionID,DivisionName FROM DivisionMaster WHERE CircleID=@CircleID ORDER BY DivisionName";

                SqlParameter[] param =
                {
                    new SqlParameter("@CircleID", circleID)
                };

                DataTable dt =
                    objDB.GetDataTable(
                        sql,
                        param);

                ddlDivision.DataTextField = "DivisionName";
                ddlDivision.DataValueField = "DivisionID";
                ddlDivision.DataSource = dt;
                ddlDivision.DataBind();
            }

            ddlDivision.Items.Insert(
                0,
                new ListItem("Select a Division", ""));
        }

        private void BindSubDivision(string divisionID)
        {
            ddlSubDivision.Items.Clear();

            if (!string.IsNullOrWhiteSpace(divisionID))
            {
                string sql =
                    "SELECT SubdivisionID,SubdivisionName FROM SubDivisionMaster WHERE DivisionID=@DivisionID ORDER BY SubdivisionName";

                SqlParameter[] param =
                {
                    new SqlParameter("@DivisionID", divisionID)
                };

                DataTable dt =
                    objDB.GetDataTable(
                        sql,
                        param);

                ddlSubDivision.DataTextField = "SubdivisionName";
                ddlSubDivision.DataValueField = "SubdivisionID";
                ddlSubDivision.DataSource = dt;
                ddlSubDivision.DataBind();
            }

            ddlSubDivision.Items.Insert(
                0,
                new ListItem("Select a Sub-Division", ""));
        }

        private void BindSection(string subdivisionID)
        {
            ddlSection.Items.Clear();

            if (!string.IsNullOrWhiteSpace(subdivisionID))
            {
                string sql =
                    "SELECT SectionID,SectionName FROM SectionMaster WHERE SubdivisionID=@SubdivisionID ORDER BY SectionName";

                SqlParameter[] param =
                {
                    new SqlParameter("@SubdivisionID", subdivisionID)
                };

                DataTable dt =
                    objDB.GetDataTable(
                        sql,
                        param);

                ddlSection.DataTextField = "SectionName";
                ddlSection.DataValueField = "SectionID";
                ddlSection.DataSource = dt;
                ddlSection.DataBind();
            }

            ddlSection.Items.Insert(
                0,
                new ListItem("Select a Section", ""));
        }

        private void ClearPostingDropdowns()
        {
            ddlDepartment.Items.Clear();
            ddlDepartment.Items.Insert(
                0,
                new ListItem("Select Department/Cell/Office", ""));

            ddlZone.Items.Clear();
            ddlZone.Items.Insert(
                0,
                new ListItem("Select Areaboard/Zone", ""));

            ddlCircle.Items.Clear();
            ddlCircle.Items.Insert(
                0,
                new ListItem("Select a Circle", ""));

            ddlDivision.Items.Clear();
            ddlDivision.Items.Insert(
                0,
                new ListItem("Select a Division", ""));

            ddlSubDivision.Items.Clear();
            ddlSubDivision.Items.Insert(
                0,
                new ListItem("Select a Sub-Division", ""));

            ddlSection.Items.Clear();
            ddlSection.Items.Insert(
                0,
                new ListItem("Select a Section", ""));
        }

        // ============================================================
        // COMPANY CHANGE
        // ============================================================

        protected void ddlPostingCompany_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(
                ddlPostingCompany.SelectedValue))
            {
                divPostingTypeChoice.Visible = false;
                dvHQ.Visible = false;
                dvField.Visible = false;

                ClearPostingDropdowns();

                return;
            }

            string companyName =
                ddlPostingCompany.SelectedItem.Text.Trim();

            bool isHQOnlyCompany =
                string.Equals(
                    companyName,
                    "BSPHCL",
                    StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    companyName,
                    "BSPGCL",
                    StringComparison.OrdinalIgnoreCase);

            if (isHQOnlyCompany)
            {
                // BSPHCL/BSPGCL = HQ only.
                divPostingTypeChoice.Visible = false;
                rblPostingType.SelectedValue = "HQ";

                dvHQ.Visible = true;
                dvField.Visible = false;

                BindDepartment();

                // Zone is not required for HQ.
                BindZone(
                    ddlPostingCompany.SelectedValue);

                BindCircle("");
                BindDivision("");
                BindSubDivision("");
                BindSection("");

                return;
            }

            // Other companies can select HQ or Field.
            divPostingTypeChoice.Visible = true;

            if (string.IsNullOrWhiteSpace(
                rblPostingType.SelectedValue))
            {
                rblPostingType.SelectedValue = "HQ";
            }

            BindDepartment();

            BindZone(
                ddlPostingCompany.SelectedValue);

            BindCircle("");
            BindDivision("");
            BindSubDivision("");
            BindSection("");

            if (rblPostingType.SelectedValue == "HQ")
            {
                dvHQ.Visible = true;
                dvField.Visible = false;
            }
            else
            {
                dvHQ.Visible = false;
                dvField.Visible = true;
            }
        }

        // ============================================================
        // HQ / FIELD CHANGE
        // ============================================================

        protected void rblPostingType_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            if (rblPostingType.SelectedValue == "HQ")
            {
                dvHQ.Visible = true;
                dvField.Visible = false;

                BindDepartment();

                BindCircle("");
                BindDivision("");
                BindSubDivision("");
                BindSection("");

                return;
            }

            dvHQ.Visible = false;
            dvField.Visible = true;

            // Important:
            // Zone is loaded using selected Company's CompanyID.
            BindZone(
                ddlPostingCompany.SelectedValue);

            BindCircle("");
            BindDivision("");
            BindSubDivision("");
            BindSection("");
        }

        // ============================================================
        // ZONE CHANGE
        // ============================================================

        protected void ddlZone_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            string zoneID =
                ddlZone.SelectedValue;

            BindCircle(zoneID);

            BindDivision("");
            BindSubDivision("");
            BindSection("");
        }

        // ============================================================
        // CIRCLE CHANGE
        // ============================================================

        protected void ddlCircle_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            string circleID =
                ddlCircle.SelectedValue;

            BindDivision(circleID);

            BindSubDivision("");
            BindSection("");
        }

        // ============================================================
        // DIVISION CHANGE
        // ============================================================

        protected void ddlDivision_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            string divisionID =
                ddlDivision.SelectedValue;

            BindSubDivision(divisionID);

            BindSection("");
        }

        // ============================================================
        // SUB-DIVISION CHANGE
        // ============================================================

        protected void ddlSubDivision_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            string subdivisionID =
                ddlSubDivision.SelectedValue;

            BindSection(subdivisionID);
        }

        // ============================================================
        // SUBMIT CORRECTION
        // ============================================================

        protected void btnSubmitCorrection_Click(
            object sender,
            EventArgs e)
        {
            string empID =
                Session["EmpID"]
                .ToString()
                .ToUpperInvariant();

            int submittedCount = 0;

            submittedCount +=
                SubmitFieldIfChanged(
                    empID,
                    "Gender",
                    ddlRequestedGender.SelectedValue);

            submittedCount +=
                SubmitFieldIfChanged(
                    empID,
                    "EmpDesignation",
                    ddlRequestedDesignation.SelectedValue);

            submittedCount +=
                SubmitFieldIfChanged(
                    empID,
                    "EmpCompany",
                    txtCompany.Text.Trim());

            submittedCount +=
                SubmitFieldIfChanged(
                    empID,
                    "MobileNo",
                    txtMobile.Text.Trim());

            submittedCount +=
                SubmitFieldIfChanged(
                    empID,
                    "EmailId",
                    txtEmail.Text.Trim());

            if (!string.IsNullOrWhiteSpace(
                ddlPostingCompany.SelectedValue))
            {
                string postingResult =
                    SubmitPostingCorrection(empID);

                if (!string.IsNullOrWhiteSpace(
                    postingResult))
                {
                    lblCorrectionMessage.CssClass =
                        "d-block mb-2 text-danger";

                    lblCorrectionMessage.Text =
                        postingResult;

                    ReopenCorrectionModal();

                    return;
                }

                submittedCount++;
            }

            if (submittedCount == 0)
            {
                lblCorrectionMessage.CssClass =
                    "d-block mb-2 text-danger";

                lblCorrectionMessage.Text =
                    "No changes were made. Please update at least one field before submitting.";

                ReopenCorrectionModal();

                return;
            }

            lblCorrectionMessage.CssClass =
                "d-block mb-2 text-success";

            lblCorrectionMessage.Text =
                "Your correction request has been submitted successfully.";

            ReopenCorrectionModal();
        }

        private int SubmitFieldIfChanged(
            string empID,
            string fieldName,
            string newValue)
        {
            string currentValue =
                GetCurrentFieldValue(
                    empID,
                    fieldName);

            string normalizedCurrent =
                currentValue == null
                ? ""
                : currentValue.Trim();

            string normalizedNew =
                newValue == null
                ? ""
                : newValue.Trim();

            if (string.Equals(
                normalizedCurrent,
                normalizedNew,
                StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }

            if (string.IsNullOrWhiteSpace(
                normalizedNew))
            {
                return 0;
            }

            InsertCorrectionRequest(
                empID,
                fieldName,
                currentValue,
                normalizedNew);

            return 1;
        }

        private string SubmitPostingCorrection(
            string empID)
        {
            string requestedValue;

            if (rblPostingType.SelectedValue == "HQ")
            {
                if (ddlDepartment.SelectedValue == "")
                {
                    return
                        "Please select a Department for your posting correction.";
                }

                requestedValue =
                    "HQ-" +
                    ddlPostingCompany.SelectedItem.Text +
                    "-" +
                    ddlDepartment.SelectedItem.Text;
            }
            else
            {
                if (ddlZone.SelectedValue == "")
                {
                    return
                        "Please select at least the AreaBoard/Zone for your posting correction.";
                }

                requestedValue =
                    "FIELD-" +
                    ddlPostingCompany.SelectedItem.Text +
                    "-" +
                    ddlZone.SelectedItem.Text +
                    "-" +
                    (ddlCircle.SelectedValue == ""
                        ? ""
                        : ddlCircle.SelectedItem.Text) +
                    "-" +
                    (ddlDivision.SelectedValue == ""
                        ? ""
                        : ddlDivision.SelectedItem.Text) +
                    "-" +
                    (ddlSubDivision.SelectedValue == ""
                        ? ""
                        : ddlSubDivision.SelectedItem.Text) +
                    "-" +
                    (ddlSection.SelectedValue == ""
                        ? ""
                        : ddlSection.SelectedItem.Text);
            }

            string currentValue =
                BuildPostingSnapshot(empID);

            InsertCorrectionRequest(
                empID,
                "EmpPostingHierarchy",
                currentValue,
                requestedValue);

            return "";
        }

        private string BuildPostingSnapshot(
            string empID)
        {
            string sql =
                "SELECT EmpPostingPlace,EmpPostingDepartment,AreaBoardZone,Circle,Division,Subdivision,Section " +
                "FROM EmpPostingDetails " +
                "WHERE EmpID=@EmpID";

            SqlParameter[] param =
            {
                new SqlParameter("@EmpID", empID)
            };

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    param);

            if (dt.Rows.Count == 0)
            {
                return "Not yet recorded.";
            }

            DataRow dr =
                dt.Rows[0];

            string department =
                dr["EmpPostingDepartment"] == DBNull.Value
                ? ""
                : dr["EmpPostingDepartment"].ToString();

            if (!string.IsNullOrWhiteSpace(
                department))
            {
                return "HQ - " + department;
            }

            List<string> parts =
                new List<string>();

            AddIfPresent(
                parts,
                "Zone",
                dr["AreaBoardZone"]);

            AddIfPresent(
                parts,
                "Circle",
                dr["Circle"]);

            AddIfPresent(
                parts,
                "Division",
                dr["Division"]);

            AddIfPresent(
                parts,
                "Sub-Division",
                dr["Subdivision"]);

            AddIfPresent(
                parts,
                "Section",
                dr["Section"]);

            return parts.Count > 0
                ? string.Join(" > ", parts)
                : Convert.ToString(
                    dr["EmpPostingPlace"]);
        }

        private void AddIfPresent(
            List<string> parts,
            string label,
            object value)
        {
            string text =
                value == null ||
                value == DBNull.Value
                ? ""
                : value.ToString();

            if (!string.IsNullOrWhiteSpace(text))
            {
                parts.Add(
                    label +
                    ": " +
                    text);
            }
        }

        private string GetCurrentFieldValue(
            string empID,
            string fieldName)
        {
            string[] allowedFields =
            {
                "EmpName",
                "Gender",
                "EmpDesignation",
                "EmpPostingPlace",
                "EmpCompany",
                "MobileNo",
                "EmailId"
            };

            if (Array.IndexOf(
                allowedFields,
                fieldName) < 0)
            {
                return "";
            }

            string sql =
                "SELECT " +
                fieldName +
                " FROM EmpBasicMaster WHERE EmpID=@EmpID";

            SqlParameter[] param =
            {
                new SqlParameter("@EmpID", empID)
            };

            object result =
                objDB.ExecuteScalar(
                    sql,
                    param);

            if (result == null ||
                result == DBNull.Value)
            {
                return "";
            }

            return result.ToString();
        }

        private void InsertCorrectionRequest(
            string empID,
            string fieldName,
            string currentValue,
            string requestedValue)
        {
            string checkSql =
                "SELECT RequestID FROM EmpProfileChangeRequest WHERE EmpID=@EmpID AND FieldName=@FieldName AND Status='Pending'";

            SqlParameter[] checkParam =
            {
                new SqlParameter("@EmpID", empID),
                new SqlParameter("@FieldName", fieldName)
            };

            object existingRequestID =
                objDB.ExecuteScalar(
                    checkSql,
                    checkParam);

            if (existingRequestID != null &&
                existingRequestID != DBNull.Value)
            {
                string updateSql =
                    "UPDATE EmpProfileChangeRequest SET CurrentValue=@CurrentValue,RequestedValue=@RequestedValue,Remarks=@Remarks,RequestedOn=GETDATE() WHERE RequestID=@RequestID";

                SqlParameter[] updateParam =
                {
                    new SqlParameter(
                        "@CurrentValue",
                        string.IsNullOrEmpty(currentValue)
                        ? (object)DBNull.Value
                        : currentValue),

                    new SqlParameter(
                        "@RequestedValue",
                        requestedValue),

                    new SqlParameter(
                        "@Remarks",
                        string.IsNullOrWhiteSpace(
                            txtRemarks.Text)
                        ? (object)DBNull.Value
                        : txtRemarks.Text.Trim()),

                    new SqlParameter(
                        "@RequestID",
                        existingRequestID.ToString())
                };

                objDB.ExecuteSql(
                    updateSql,
                    updateParam);

                return;
            }

            string requestID =
                GenerateRequestID();

            string insertSql =
                "INSERT INTO EmpProfileChangeRequest (RequestID,EmpID,FieldName,CurrentValue,RequestedValue,Remarks,Status,RequestedOn) VALUES (@RequestID,@EmpID,@FieldName,@CurrentValue,@RequestedValue,@Remarks,'Pending',GETDATE())";

            SqlParameter[] insertParam =
            {
                new SqlParameter(
                    "@RequestID",
                    requestID),

                new SqlParameter(
                    "@EmpID",
                    empID),

                new SqlParameter(
                    "@FieldName",
                    fieldName),

                new SqlParameter(
                    "@CurrentValue",
                    string.IsNullOrEmpty(currentValue)
                    ? (object)DBNull.Value
                    : currentValue),

                new SqlParameter(
                    "@RequestedValue",
                    requestedValue),

                new SqlParameter(
                    "@Remarks",
                    string.IsNullOrWhiteSpace(
                        txtRemarks.Text)
                    ? (object)DBNull.Value
                    : txtRemarks.Text.Trim())
            };

            objDB.ExecuteSql(
                insertSql,
                insertParam);
        }

        private void ReopenCorrectionModal()
        {
            ScriptManager.RegisterStartupScript(
                UpdatePanelCorrection,
                UpdatePanelCorrection.GetType(),
                "reopenCorrectionModal",
                "$('#correctionModal').modal('show');",
                true);
        }

        private string GenerateRequestID()
        {
            object result =
                objDB.ExecuteScalar(
                    "SELECT ISNULL(MAX(CAST(RIGHT(RequestID,6) AS INT)),0)+1 FROM EmpProfileChangeRequest",
                    null);

            int id =
                result == null ||
                result == DBNull.Value
                ? 1
                : Convert.ToInt32(result);

            return "PCR" +
                id.ToString("000000");
        }
    }
}