using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Configuration;

using System.Data;

using System.Data.SqlClient;

using System.Drawing;

using System.Globalization;
using System.Web.Services;

namespace Training.Admin
{

    public partial class AssignSession : System.Web.UI.Page
    {

        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

            if (Session["TrainingID"] == null)
            {
                Response.Redirect("~/Admin/TrainingList.aspx");
                return;
            }
            TrainingSummary1.LoadTraining(Session["TrainingID"].ToString());
            lblTrainingID.Text = Session["TrainingID"].ToString();

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT Hours FROM TrainingDetails WHERE TrainingID=@TrainingID",
                    con);

                cmd.Parameters.AddWithValue(
                    "@TrainingID",
                    Session["TrainingID"]);

                con.Open();

                object obj = cmd.ExecuteScalar();

                lblTrainingHours.Text =
                    obj == null ? "0" : obj.ToString();

                con.Close();
            }
            if (!IsPostBack)
            {
                BindStartTime();

                BindTopic();

                BindTrainer();


                GenerateSessionNo();

                GenerateSessionID();

                BindGrid();

                BindSummary();

                ScriptManager.RegisterStartupScript(
                       this,
                       GetType(),
                       "Init",
                       "initControls();calculateHours();",
                       true);
            }
        }

        private void LoadTrainingDates(
      out DateTime batchFrom,
      out DateTime batchTo)
        {
            batchFrom = DateTime.MinValue;

            batchTo = DateTime.MinValue;

            using (SqlConnection con =
                new SqlConnection(constr))
            {
                SqlCommand cmd =
                new SqlCommand(@"

SELECT

DateFrom,
DateTo

FROM TrainingDetails

WHERE TrainingID=@TrainingID

", con);

                cmd.Parameters.AddWithValue(
                    "@TrainingID",
                    lblTrainingID.Text.Trim());

                con.Open();

                SqlDataReader dr =
                    cmd.ExecuteReader();

                if (dr.Read())
                {
                    if (!TryReadTrainingDate(dr["DateFrom"], out batchFrom))
                    {
                        throw new Exception(
                            "Invalid Batch From Date in TrainingDetails.");
                    }

                    if (!TryReadTrainingDate(dr["DateTo"], out batchTo))
                    {
                        throw new Exception(
                            "Invalid Batch To Date in TrainingDetails.");
                    }
                }
                else
                {
                    throw new Exception(
                        "Training details not found.");
                }

                dr.Close();
            }
        }
        private bool TryReadTrainingDate(object value, out DateTime result)
        {
            result = DateTime.MinValue;

            if (value == null || value == DBNull.Value)
                return false;

            if (value is DateTime)
            {
                result = (DateTime)value;
                return true;
            }

            string text = Convert.ToString(value).Trim();

            if (DateTime.TryParseExact(text, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                return true;

            if (DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out result))
                return true;

            return DateTime.TryParse(text, out result);
        }

        private void BindStartTime()
        {
            ddlStartTime.Items.Clear();
            ddlEndTime.Items.Clear();

            ddlStartTime.Items.Add(new ListItem("-- Select Time --", ""));
            ddlEndTime.Items.Add(new ListItem("-- Select Time --", ""));

            DateTime dt = DateTime.Today.AddHours(6);
            DateTime end = DateTime.Today.AddHours(22);

            while (dt <= end)
            {
                string tm = dt.ToString("hh:mm tt");

                ddlStartTime.Items.Add(new ListItem(tm, tm));
                ddlEndTime.Items.Add(new ListItem(tm, tm));

                dt = dt.AddMinutes(15);
            }
        }
        private void BindTopic()
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlDataAdapter da = new SqlDataAdapter(@"
SELECT
TopicID,
TopicName
FROM TopicMaster
ORDER BY TopicName", con);

                DataTable dt = new DataTable();

                da.Fill(dt);

                ddlTopic.DataSource = dt;
                ddlTopic.DataTextField = "TopicName";
                ddlTopic.DataValueField = "TopicID";
                ddlTopic.DataBind();

                ddlTopic.Items.Insert(0, new ListItem("-- Select Topic --", ""));
            }
        }
        private void BindTrainer()
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlDataAdapter da = new SqlDataAdapter(@"

SELECT

T.TrainerID,

CASE

WHEN T.TrainerType='Internal'


THEN

ISNULL(E.EmpID,'')
+ ' | '
+ ISNULL(E.EmpName,'')

+ CASE
WHEN ISNULL(E.EmpDesignation,'')=''
THEN ''
ELSE ' | ' + E.EmpDesignation
END

+ ' | Internal'


ELSE

T.TrainerID
+ ' | '
+ ISNULL(T.NameExternal,'')

+ CASE
WHEN ISNULL(T.DesignationExternal,'')=''
THEN ''
ELSE ' | ' + T.DesignationExternal
END

+ ' | External'

END AS TrainerDisplay

FROM TrainerMaster T

LEFT JOIN EmpBasicMaster E
ON T.EmpID=E.EmpID

ORDER BY
T.TrainerType,
T.TrainerID

", con);
                DataTable dt = new DataTable();

                da.Fill(dt);

                ddlTrainer.DataSource = dt;

                ddlTrainer.DataTextField = "TrainerDisplay";

                ddlTrainer.DataValueField = "TrainerID";

                ddlTrainer.DataBind();

                ddlTrainer.Items.Insert(0,
                    new ListItem("-- Select Trainer --", ""));
            }
        }
        private void GenerateSessionNo()
        {
            using (SqlConnection con =
                new SqlConnection(constr))
            {
                SqlCommand cmd =
                new SqlCommand(@"

SELECT
ISNULL(MAX(CAST(SessionNo AS INT)),0)+1

FROM SessionMaster

WHERE TrainingID=@TrainingID

", con);

                cmd.Parameters.AddWithValue(
                    "@TrainingID",
                    Session["TrainingID"]);

                con.Open();

                txtSessionNo.Text =
                    cmd.ExecuteScalar().ToString();

                con.Close();
            }
        }
        private void BindGrid()
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlDataAdapter da = new SqlDataAdapter(@"

SELECT

S.SessionID,
S.SessionNo,
S.SessionName,
S.SessionDate,
S.StartTime,
S.EndTime,
S.TotalHours,

TM.TopicName,

TR.TrainerID,
CASE

WHEN TR.TrainerType='Internal'

THEN E.EmpID

ELSE TR.TrainerID

END AS DisplayTrainerID,
CASE
WHEN TR.TrainerType='Internal'
THEN E.EmpName
ELSE TR.NameExternal
END AS TrainerName,

CASE
WHEN TR.TrainerType='Internal'
THEN E.EmpDesignation
ELSE TR.DesignationExternal
END AS Designation,

TR.TrainerType

FROM SessionMaster S

LEFT JOIN TopicMaster TM
ON TM.TopicID = S.TopicID

LEFT JOIN TrainerMaster TR
ON TR.TrainerID = S.TrainerID

LEFT JOIN EmpBasicMaster E
ON E.EmpID = TR.EmpID

WHERE S.TrainingID=@TrainingID

ORDER BY CAST(S.SessionNo AS INT)

", con);

                da.SelectCommand.Parameters.AddWithValue("@TrainingID", lblTrainingID.Text);

                DataTable dt = new DataTable();

                da.Fill(dt);

                gvSession.DataSource = dt;

                gvSession.DataBind();
            }
        }
        private void BindSummary()
        {
            using (SqlConnection con = new SqlConnection(constr))
            {

                SqlCommand cmd = new SqlCommand(@"

SELECT

COUNT(*) AS TotalSession,

ISNULL(SUM(CAST(TotalHours AS decimal(10,2))),0) AS UsedHours

FROM SessionMaster

WHERE TrainingID=@TrainingID

", con);

                cmd.Parameters.AddWithValue("@TrainingID", lblTrainingID.Text);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    lblTotalSessions.Text = dr["TotalSession"].ToString();

                    decimal used = Convert.ToDecimal(dr["UsedHours"]);

                    lblUsedHours.Text = used.ToString("0.00");

                    decimal planned = 0;

                    decimal.TryParse(lblTrainingHours.Text, out planned);

                    decimal remaining = planned - used;

                    if (remaining < 0)
                        remaining = 0;

                    lblRemainingHours.Text = remaining.ToString("0.00");
                }

                dr.Close();

                con.Close();

            }
        }
        [WebMethod]
        public static string GetTrainerExpertise(string trainerID)
        {
            string constr =
            ConfigurationManager
            .ConnectionStrings["constr"]
            .ConnectionString;

            using (SqlConnection con =
                new SqlConnection(constr))
            {
                SqlCommand cmd =
                new SqlCommand(@"

SELECT

ISNULL(A.ExpertiseName,'')

FROM TrainerMaster T

LEFT JOIN AreaOfExpertiseMaster A

ON T.AreaOfExpertiseID=A.ExpertiseID

WHERE T.TrainerID=@TrainerID

", con);

                cmd.Parameters.AddWithValue(
                "@TrainerID",
                trainerID);

                con.Open();

                object obj =
                cmd.ExecuteScalar();

                if (obj == null)
                    return "";

                return obj.ToString();
            }
        }
        protected void ddlTrainer_SelectedIndexChanged(
    object sender,
    EventArgs e)
        {
            lblTrainerExpertise.Text = "";

            if (ddlTrainer.SelectedIndex == 0)
                return;

            using (SqlConnection con =
                new SqlConnection(constr))
            {
                SqlCommand cmd =
                new SqlCommand(@"

SELECT

ISNULL(A.ExpertiseName,'')

FROM TrainerMaster T

LEFT JOIN AreaOfExpertiseMaster A
ON T.AreaOfExpertiseID=A.ExpertiseID

WHERE T.TrainerID=@TrainerID

", con);

                cmd.Parameters.AddWithValue(
                    "@TrainerID",
                    ddlTrainer.SelectedValue);

                con.Open();

                object obj =
                    cmd.ExecuteScalar();

                if (obj != null)
                    lblTrainerExpertise.Text =
                        obj.ToString();
                con.Close();
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {

            lblMessage.Text = "";

            if (ddlTopic.SelectedIndex == 0)
            {
                ShowMessage("Please select Topic.", Color.Red);
                return;
            }

            if (ddlTrainer.SelectedIndex == 0)
            {
                ShowMessage("Please select Trainer.", Color.Red);
                return;
            }

            if (!Page.IsValid)
                return;


            DateTime batchFrom;

            DateTime batchTo;

            LoadTrainingDates(
                out batchFrom,
                out batchTo);


            DateTime sessionDate;

            if (!DateTime.TryParseExact(
                    txtSessionDate.Text.Trim(),
                    "dd-MM-yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out sessionDate))
            {
                lblMessage.Text = "Please select a valid Session Date.";
                lblMessage.ForeColor = Color.Red;
                return;
            }

            if (sessionDate < batchFrom || sessionDate > batchTo)
            {
                lblMessage.Text = "Session Date must be within Training Duration.";
                lblMessage.ForeColor = Color.Red;
                return;
            }


            decimal sessionHours;

            if (!decimal.TryParse(
                    hfTotalHours.Value,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out sessionHours))
            {
                ShowMessage("Invalid Session Hours.", Color.Red);
                return;
            }

            if (sessionHours <= 0)
            {
                lblMessage.Text = "Session Hours should be greater than zero.";
                lblMessage.ForeColor = Color.Red;
                return;
            }

            //decimal plannedHours = Convert.ToDecimal(lblTrainingHours.Text);
            decimal plannedHours = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(@"

SELECT Hours

FROM TrainingDetails

WHERE TrainingID=@TrainingID

", con);

                cmd.Parameters.AddWithValue(
                    "@TrainingID",
                    lblTrainingID.Text);

                con.Open();

                decimal.TryParse(
                    Convert.ToString(cmd.ExecuteScalar()),
                    out plannedHours);

                con.Close();
            }
            decimal usedHours = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(@"
SELECT ISNULL(SUM(CAST(TotalHours AS DECIMAL(10,2))),0)
FROM SessionMaster
WHERE TrainingID=@TrainingID", con);

                cmd.Parameters.AddWithValue("@TrainingID", lblTrainingID.Text);

                con.Open();

                usedHours = Convert.ToDecimal(cmd.ExecuteScalar());

                con.Close();
            }

            if ((usedHours + sessionHours) > plannedHours)
            {
                lblMessage.Text = "Total Session Hours cannot exceed Planned Training Hours.";
                lblMessage.ForeColor = Color.Red;
                return;
            }

            if (IsDuplicateSession())
            {
                ShowMessage("Session timing overlaps with an existing session.", Color.Red);
                return;
            }

            if (IsTrainerBusy())
            {
                ShowMessage("Selected trainer is already assigned to another session during this time.", Color.Red);
                return;
            }

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    SqlCommand chk = new SqlCommand(@"
SELECT COUNT(*)
FROM SessionMaster
WHERE TrainingID=@TrainingID
AND SessionNo=@SessionNo", con, tran);

                    chk.Parameters.AddWithValue("@TrainingID", lblTrainingID.Text);
                    chk.Parameters.AddWithValue("@SessionNo", txtSessionNo.Text);

                    int cnt = Convert.ToInt32(chk.ExecuteScalar());

                    if (cnt > 0)
                    {
                        lblMessage.Text = "Session No already exists.";
                        lblMessage.ForeColor = Color.Red;

                        tran.Rollback();
                        return;
                    }

                    // INSERT COMMAND STARTS HERE...
                    // Continue in Part 2A-2
                    SqlCommand cmd = new SqlCommand(@"

INSERT INTO SessionMaster
(
    SessionID,
    TrainingID,
    SessionNo,
    SessionName,
    SessionDate,
    StartTime,
    EndTime,
    TotalHours,
    TopicID,
    TrainerID,
    SessionStatus,
    Remarks,
    CreatedOn,
    CreatedBy
)

VALUES
(
    @SessionID,
    @TrainingID,
    @SessionNo,
    @SessionName,
    @SessionDate,
    @StartTime,
    @EndTime,
    @TotalHours,
    @TopicID,
    @TrainerID,
    'Draft',
    @Remarks,
    GETDATE(),
    @CreatedBy
)

", con, tran);

                    cmd.Parameters.AddWithValue("@SessionID", txtSessionID.Text.Trim());

                    cmd.Parameters.AddWithValue("@TrainingID", lblTrainingID.Text.Trim());

                    cmd.Parameters.AddWithValue("@SessionNo", Convert.ToInt32(txtSessionNo.Text));

                    cmd.Parameters.AddWithValue("@SessionName", txtSessionName.Text.Trim());

                    cmd.Parameters.AddWithValue("@SessionDate", sessionDate.ToString("dd-MM-yyyy"));

                    cmd.Parameters.AddWithValue("@StartTime", ddlStartTime.SelectedValue);

                    cmd.Parameters.AddWithValue("@EndTime", ddlEndTime.SelectedValue);

                    cmd.Parameters.AddWithValue("@TotalHours", sessionHours);

                    cmd.Parameters.AddWithValue("@TopicID", ddlTopic.SelectedValue);

                    cmd.Parameters.AddWithValue("@TrainerID", ddlTrainer.SelectedValue);

                    cmd.Parameters.AddWithValue("@Remarks", txtRemarks.Text.Trim());

                    cmd.Parameters.AddWithValue("@CreatedBy", Session["UserID"] == null
                                                            ? "Admin"
                                                            : Session["UserID"].ToString());

                    cmd.ExecuteNonQuery();

                    tran.Commit();

                    //clsWorkflow.UpdateWorkflow(lblTrainingID.Text.Trim(), "SessionCreated", 23);
                    clsWorkflow.UpdateWorkflow(Session["TrainingID"].ToString(), "SessionCreated", "BC");
                    lblMessage.Text =
                    "Session created successfully";

                    lblMessage.ForeColor = Color.Green;

                    ClearControls();

                    GenerateSessionNo();

                    GenerateSessionID();

                    BindGrid();

                    BindSummary();

                }
                catch (Exception ex)
                {
                    tran.Rollback();

                    lblMessage.Text = ex.Message;

                    lblMessage.ForeColor = Color.Red;
                }
                finally
                {
                    con.Close();
                }

            }
        }
        private void ClearControls()
        {
            txtSessionName.Text = "";

            txtSessionDate.Text = "";

            ddlStartTime.SelectedIndex = 0;

            ddlEndTime.SelectedIndex = 0;

            ddlTopic.SelectedIndex = 0;

            ddlTrainer.SelectedIndex = 0;

            txtTotalHours.Text = "";

            txtRemarks.Text = "";

            ViewState["SessionID"] = null;

            btnSave.Visible = true;

            btnUpdate.Visible = false;

            btnDelete.Visible = false;

            txtSessionName.Focus();
        }