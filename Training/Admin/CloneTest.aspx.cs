using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class CloneTest : System.Web.UI.Page
    {
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["InternalRedirect_Admin"] == null)
                {
                    Response.Redirect("~/Default.aspx");
                    return;
                }

                BindSourceTests();
                BindTargetTraining();
            }
        }

        private void BindSourceTests()
        {
            string query = @"SELECT TM.TestID,
                             TM.TestTitle + ' | ' + TM.TestType + ' | ' + TD.TrainingID + ' | ' + TP.TopicName AS TestDisplay
                             FROM TestMaster TM
                             INNER JOIN TrainingDetails TD ON TM.TrainingID = TD.TrainingID
                             INNER JOIN TopicMaster TP ON TM.TopicID = TP.TopicID
                             WHERE TM.IsPublished = 1
                             ORDER BY TM.CreatedOn DESC";

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlSourceTest.DataSource = dt;
                ddlSourceTest.DataTextField = "TestDisplay";
                ddlSourceTest.DataValueField = "TestID";
                ddlSourceTest.DataBind();

                ddlSourceTest.Items.Insert(0, new ListItem("-- Select A Published Test --", ""));
            }
        }

        private void BindTargetTraining()
        {
            string query = @"SELECT  TrainingID, TrainingID + ' | ' + TrainingType + ' | ' + Batch AS TrainingName
                             FROM TrainingDetails
                             ORDER BY CreatedOn DESC";

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlTargetTraining.DataSource = dt;
                ddlTargetTraining.DataTextField = "TrainingName";
                ddlTargetTraining.DataValueField = "TrainingID";
                ddlTargetTraining.DataBind();

                ddlTargetTraining.Items.Insert(0, new ListItem("-- Select Training --", ""));
            }
        }

        protected void ddlSourceTest_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            if (ddlSourceTest.SelectedValue == "")
            {
                pnlSourceDetails.Visible = false;
                return;
            }

            string query = @"SELECT TestType, Duration, TotalQuestions, TotalMarks
                             FROM TestMaster WHERE TestID = @TestID";

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@TestID", ddlSourceTest.SelectedValue);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    lblSrcType.Text = dr["TestType"].ToString();
                    lblSrcDuration.Text = dr["Duration"].ToString();
                    lblSrcTotalQuestions.Text = dr["TotalQuestions"].ToString();
                    lblSrcTotalMarks.Text = dr["TotalMarks"].ToString();

                    pnlSourceDetails.Visible = true;
                }
            }
        }

        protected void ddlTargetTraining_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlTargetSession.Items.Clear();

            if (ddlTargetTraining.SelectedValue == "")
            {
                ddlTargetSession.Items.Insert(0, new ListItem("-- Select Training First --", ""));
                return;
            }

            string query = @"SELECT SM.SessionID,
                             SM.SessionName + ' | ' + CONVERT(varchar,SM.SessionDate,105) + ' | ' + TP.TopicName AS SessionDisplay
                             FROM SessionMaster SM
                             INNER JOIN TopicMaster TP ON SM.TopicID = TP.TopicID
                             WHERE SM.TrainingID = @TrainingID
                             ORDER BY SM.SessionDate";

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@TrainingID", ddlTargetTraining.SelectedValue);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlTargetSession.DataSource = dt;
                ddlTargetSession.DataTextField = "SessionDisplay";
                ddlTargetSession.DataValueField = "SessionID";
                ddlTargetSession.DataBind();

                ddlTargetSession.Items.Insert(0, new ListItem("-- Select Session --", ""));
            }
        }

        protected void btnClone_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            if (ddlSourceTest.SelectedValue == "")
            {
                lblMessage.Text = "Please select a published test to clone.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            if (ddlTargetSession.SelectedValue == "")
            {
                lblMessage.Text = "Please select a target training and session.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            string sourceTestID = ddlSourceTest.SelectedValue;
            string targetSessionID = ddlTargetSession.SelectedValue;
            string targetTrainingID = ddlTargetTraining.SelectedValue;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    // Source test's settings
                    SqlCommand cmdSource = new SqlCommand(
                        @"SELECT TestType, TestTitle, Duration, TotalQuestions, TotalMarks, PassingPercentage,
                          RandomQuestion, ShuffleOption, AllowRetest, MaxAttempt
                          FROM TestMaster WHERE TestID = @TestID", con, tran);
                    cmdSource.Parameters.AddWithValue("@TestID", sourceTestID);

                    DataTable sourceTest = new DataTable();
                    new SqlDataAdapter(cmdSource).Fill(sourceTest);

                    if (sourceTest.Rows.Count == 0)
                    {
                        throw new Exception("The selected test could not be found.");
                    }

                    DataRow src = sourceTest.Rows[0];
                    string testType = src["TestType"].ToString();

                    // Target session's Topic and Trainer -- looked up fresh here
                    // rather than trusted from the dropdown, in case the list
                    // was bound a while ago.
                    SqlCommand cmdTargetSession = new SqlCommand(
                        "SELECT TopicID, TrainerID FROM SessionMaster WHERE SessionID = @SessionID",
                        con, tran);
                    cmdTargetSession.Parameters.AddWithValue("@SessionID", targetSessionID);

                    DataTable targetSession = new DataTable();
                    new SqlDataAdapter(cmdTargetSession).Fill(targetSession);

                    if (targetSession.Rows.Count == 0)
                    {
                        throw new Exception("The selected session could not be found.");
                    }

                    string targetTopicID = targetSession.Rows[0]["TopicID"].ToString();
                    string targetTrainerID = targetSession.Rows[0]["TrainerID"].ToString();

                    // Don't allow two Pre (or two Post) tests on the same session.
                    SqlCommand cmdDuplicate = new SqlCommand(
                        "SELECT COUNT(*) FROM TestMaster WHERE SessionID = @SessionID AND TestType = @TestType",
                        con, tran);
                    cmdDuplicate.Parameters.AddWithValue("@SessionID", targetSessionID);
                    cmdDuplicate.Parameters.AddWithValue("@TestType", testType);

                    if (Convert.ToInt32(cmdDuplicate.ExecuteScalar()) > 0)
                    {
                        throw new Exception("The target session already has a " + testType + " test. Choose a different session.");
                    }

                    // New TestMaster row, same ID convention already used
                    // elsewhere in this app.
                    string newTestID =
                        "TST" +
                        DateTime.Now.ToString("yyyyMMddHHmmssfff") +
                        new Random().Next(100, 999);

                    SqlCommand cmdInsertTest = new SqlCommand(
                        @"INSERT INTO TestMaster
                          (TestID, TrainingID, SessionID, TopicID, TrainerID, TestType, TestTitle,
                           Duration, TotalQuestions, TotalMarks, PassingPercentage,
                           RandomQuestion, ShuffleOption, AllowRetest, MaxAttempt, IsPublished, CreatedOn)
                          VALUES
                          (@TestID, @TrainingID, @SessionID, @TopicID, @TrainerID, @TestType, @TestTitle,
                           @Duration, @TotalQuestions, @TotalMarks, @PassingPercentage,
                           @RandomQuestion, @ShuffleOption, @AllowRetest, @MaxAttempt, 1, GETDATE())",
                        con, tran);

                    cmdInsertTest.Parameters.AddWithValue("@TestID", newTestID);
                    cmdInsertTest.Parameters.AddWithValue("@TrainingID", targetTrainingID);
                    cmdInsertTest.Parameters.AddWithValue("@SessionID", targetSessionID);
                    cmdInsertTest.Parameters.AddWithValue("@TopicID", targetTopicID);
                    cmdInsertTest.Parameters.AddWithValue("@TrainerID", targetTrainerID);
                    cmdInsertTest.Parameters.AddWithValue("@TestType", testType);
                    cmdInsertTest.Parameters.AddWithValue("@TestTitle", src["TestTitle"].ToString());
                    cmdInsertTest.Parameters.AddWithValue("@Duration", src["Duration"].ToString());
                    cmdInsertTest.Parameters.AddWithValue("@TotalQuestions", src["TotalQuestions"].ToString());
                    cmdInsertTest.Parameters.AddWithValue("@TotalMarks", src["TotalMarks"]);
                    cmdInsertTest.Parameters.AddWithValue("@PassingPercentage", src["PassingPercentage"].ToString());
                    cmdInsertTest.Parameters.AddWithValue("@RandomQuestion", src["RandomQuestion"]);
                    cmdInsertTest.Parameters.AddWithValue("@ShuffleOption", src["ShuffleOption"]);
                    cmdInsertTest.Parameters.AddWithValue("@AllowRetest", src["AllowRetest"]);
                    cmdInsertTest.Parameters.AddWithValue("@MaxAttempt", src["MaxAttempt"].ToString());

                    cmdInsertTest.ExecuteNonQuery();

                    // Copy the source test's exact question set across.
                    SqlCommand cmdSourceQuestions = new SqlCommand(
                        "SELECT QuestionID, QuestionOrder, Marks FROM TestQuestion WHERE TestID = @TestID ORDER BY QuestionOrder",
                        con, tran);
                    cmdSourceQuestions.Parameters.AddWithValue("@TestID", sourceTestID);

                    DataTable sourceQuestions = new DataTable();
                    new SqlDataAdapter(cmdSourceQuestions).Fill(sourceQuestions);

                    if (sourceQuestions.Rows.Count == 0)
                    {
                        throw new Exception("The selected test has no questions saved against it -- nothing to clone.");
                    }

                    foreach (DataRow q in sourceQuestions.Rows)
                    {
                        string newTestQuestionID =
                            "TQ" +
                            DateTime.Now.ToString("yyyyMMddHHmmssfff") +
                            new Random().Next(100, 999);

                        SqlCommand cmdInsertQuestion = new SqlCommand(
                            @"INSERT INTO TestQuestion (TestQuestionID, TestID, QuestionID, QuestionOrder, Marks, CreatedOn)
                              VALUES (@TestQuestionID, @TestID, @QuestionID, @QuestionOrder, @Marks, GETDATE())",
                            con, tran);

                        cmdInsertQuestion.Parameters.AddWithValue("@TestQuestionID", newTestQuestionID);
                        cmdInsertQuestion.Parameters.AddWithValue("@TestID", newTestID);
                        cmdInsertQuestion.Parameters.AddWithValue("@QuestionID", q["QuestionID"]);
                        cmdInsertQuestion.Parameters.AddWithValue("@QuestionOrder", q["QuestionOrder"]);
                        cmdInsertQuestion.Parameters.AddWithValue("@Marks", q["Marks"]);

                        cmdInsertQuestion.ExecuteNonQuery();
                    }

                    // Give every trainee assigned to the target training their own
                    // answer-key snapshot for this cloned test - the same thing
                    // normal test publishing already does. Without this, a trainee
                    // opening the cloned test would see zero questions.
                    SqlCommand cmdGenerateCandidateQuestions = new SqlCommand(
                        @"INSERT INTO TestCandidateQuestion
                          (TestCandidateQuestionID, TestID, EmpID, QuestionID, QuestionOrder, Marks, SelectedOption, CorrectOption, IsCorrect, CreatedOn)
                          SELECT
                              'TCQ' + FORMAT(GETDATE(),'yyyyMMddHHmmssfff') +
                                  RIGHT('000000' + CAST(ROW_NUMBER() OVER (ORDER BY TA.EmpID, TQ.QuestionOrder) AS VARCHAR(6)), 6),
                              @NewTestID,
                              TA.EmpID,
                              TQ.QuestionID,
                              TQ.QuestionOrder,
                              TQ.Marks,
                              NULL,
                              QB.CorrectOption,
                              NULL,
                              GETDATE()
                          FROM TrainingAssignment TA
                          CROSS JOIN TestQuestion TQ
                          INNER JOIN QuestionBank QB ON QB.QuestionID = TQ.QuestionID
                          WHERE TQ.TestID = @NewTestID
                          AND TA.TrainingID = @TrainingID",
                        con, tran);

                    cmdGenerateCandidateQuestions.Parameters.AddWithValue("@NewTestID", newTestID);
                    cmdGenerateCandidateQuestions.Parameters.AddWithValue("@TrainingID", targetTrainingID);

                    cmdGenerateCandidateQuestions.ExecuteNonQuery();

                    tran.Commit();

                    lblMessage.Text = "Test cloned and published successfully (" + sourceQuestions.Rows.Count + " question(s) copied, answer keys generated for all assigned trainees).";
                    lblMessage.ForeColor = System.Drawing.Color.Green;
                }
                catch (Exception ex)
                {
                    tran.Rollback();

                    lblMessage.Text = ex.Message;
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
        }
    }
}