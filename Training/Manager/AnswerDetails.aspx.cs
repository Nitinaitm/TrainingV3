using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;

namespace Training.Manager
{
    public partial class AnswerDetails : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        private string ManagerID { get { return Session["ManagerID"] == null ? "" : Session["ManagerID"].ToString().Trim(); } }
        private string ResultID { get { return Session["ManagerAnswerResultID"] == null ? "" : Session["ManagerAnswerResultID"].ToString().Trim(); } }
        private string SessionID { get { return Session["SessionID"] == null ? "" : Session["SessionID"].ToString().Trim(); } }
        private string TrainingID { get { return Session["TrainingID"] == null ? "" : Session["TrainingID"].ToString().Trim(); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ManagerID))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (string.IsNullOrWhiteSpace(ResultID) || string.IsNullOrWhiteSpace(SessionID) || string.IsNullOrWhiteSpace(TrainingID))
            {
                Response.Redirect("~/Manager/SessionAnswerReport.aspx");
                return;
            }

            if (!IsPostBack)
            {
                if (!LoadResult())
                {
                    Response.Redirect("~/Manager/SessionAnswerReport.aspx");
                    return;
                }

                LoadAnswers();
            }
        }

        private bool HasAccess()
        {
            object value = obj.ExecuteScalar("SELECT COUNT(*) FROM TestResult R INNER JOIN TestMaster TM ON R.TestID=TM.TestID INNER JOIN SessionMaster SM ON TM.SessionID=SM.SessionID INNER JOIN TrainingDetails TD ON TM.TrainingID=TD.TrainingID INNER JOIN TrainingLocationMaster L ON TD.TrainingLocation=L.TrainingLocation INNER JOIN ManagerMaster M ON M.TrainingLocationID=L.TrainingLocationID WHERE R.ResultID=@ResultID AND SM.SessionID=@SessionID AND SM.TrainingID=@TrainingID AND M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y'", new SqlParameter[] { new SqlParameter("@ResultID", ResultID), new SqlParameter("@SessionID", SessionID), new SqlParameter("@TrainingID", TrainingID), new SqlParameter("@ManagerID", ManagerID) });
            return value != null && value != DBNull.Value && Convert.ToInt32(value) > 0;
        }

        private bool LoadResult()
        {
            if (!HasAccess())
            {
                return false;
            }

            DataTable dt = obj.GetDataTable("SELECT R.ResultID,R.TestID,R.EmpID,R.TotalQuestions,R.AttemptedQuestions,R.CorrectAnswers,R.TotalMarks,R.ResultStatus,R.AttemptNo,R.SubmittedOn,R.IsFinalAttempt,TM.TestTitle,TM.TestType,E.EmpName,E.EmpDesignation,TME.TraineeName FROM TestResult R INNER JOIN TestMaster TM ON R.TestID=TM.TestID INNER JOIN SessionMaster SM ON TM.SessionID=SM.SessionID LEFT JOIN EmpBasicMaster E ON R.EmpID=E.EmpID LEFT JOIN TraineeMasterExternal TME ON TME.EmpIDExternal=R.EmpID WHERE R.ResultID=@ResultID AND SM.SessionID=@SessionID AND SM.TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@ResultID", ResultID), new SqlParameter("@SessionID", SessionID), new SqlParameter("@TrainingID", TrainingID) });

            if (dt.Rows.Count == 0)
            {
                return false;
            }

            DataRow dr = dt.Rows[0];
            lblEmpID.Text = dr["EmpID"].ToString();
            lblEmpName.Text = !String.IsNullOrWhiteSpace(dr["EmpName"].ToString()) ? dr["EmpName"].ToString() : (!String.IsNullOrWhiteSpace(dr["TraineeName"].ToString()) ? dr["TraineeName"].ToString() : dr["EmpID"].ToString());
            lblDesignation.Text = dr["EmpDesignation"].ToString();
            lblTestID.Text = dr["TestID"].ToString();
            lblTestType.Text = dr["TestType"].ToString() == "Pre" ? "Pre Training" : dr["TestType"].ToString() == "Post" ? "Post Training" : dr["TestType"].ToString();
            lblAttempt.Text = dr["AttemptNo"].ToString() + (Convert.ToBoolean(dr["IsFinalAttempt"]) ? " (Final)" : "");
            lblSubmittedOn.Text = Convert.ToDateTime(dr["SubmittedOn"]).ToString("dd-MM-yyyy hh:mm tt");
            lblTotalQ.Text = dr["TotalQuestions"].ToString();
            lblAttempted.Text = dr["AttemptedQuestions"].ToString();
            lblCorrect.Text = dr["CorrectAnswers"].ToString();
            lblScore.Text = Convert.ToDecimal(dr["TotalMarks"]).ToString("0.00") + "%";
            lblStatus.Text = dr["ResultStatus"].ToString();
            lblStatus.CssClass = dr["ResultStatus"].ToString().Equals("Pass", StringComparison.OrdinalIgnoreCase) || dr["ResultStatus"].ToString().Equals("Passed", StringComparison.OrdinalIgnoreCase) ? "info-value result-pass" : "info-value result-fail";
            return true;
        }

        private void LoadAnswers()
        {
            DataTable dt = obj.GetDataTable("SELECT QB.Question,QB.QuestionType,QB.OptionA,QB.OptionB,QB.OptionC,QB.OptionD,TAA.CorrectOption AS CorrectAnswer,TAA.SelectedOption AS SelectedAnswer,TAA.IsCorrect FROM TestResult R INNER JOIN TestMaster TM ON R.TestID=TM.TestID INNER JOIN SessionMaster SM ON TM.SessionID=SM.SessionID INNER JOIN TestAttempt TA ON TA.TestID=R.TestID AND TA.EmpID=R.EmpID AND TA.AttemptNo=R.AttemptNo AND TA.Submitted=1 INNER JOIN TestAttemptAnswer TAA ON TAA.AttemptID=TA.AttemptID INNER JOIN QuestionBank QB ON TAA.QuestionID=QB.QuestionID INNER JOIN TrainingDetails TD ON TM.TrainingID=TD.TrainingID INNER JOIN TrainingLocationMaster L ON TD.TrainingLocation=L.TrainingLocation INNER JOIN ManagerMaster M ON M.TrainingLocationID=L.TrainingLocationID WHERE R.ResultID=@ResultID AND SM.SessionID=@SessionID AND SM.TrainingID=@TrainingID AND M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' ORDER BY TAA.DisplayOrder", new SqlParameter[] { new SqlParameter("@ResultID", ResultID), new SqlParameter("@SessionID", SessionID), new SqlParameter("@TrainingID", TrainingID), new SqlParameter("@ManagerID", ManagerID) });
            gvAnswers.DataSource = dt;
            gvAnswers.DataBind();
        }

        protected bool IsCorrect(object value)
        {
            return value != null && value != DBNull.Value && Convert.ToBoolean(value);
        }

        protected string GetOptions(object optA, object optB, object optC, object optD, object selected, object correct)
        {
            StringBuilder sb = new StringBuilder();
            string[] options = { optA == null || optA == DBNull.Value ? "" : optA.ToString(), optB == null || optB == DBNull.Value ? "" : optB.ToString(), optC == null || optC == DBNull.Value ? "" : optC.ToString(), optD == null || optD == DBNull.Value ? "" : optD.ToString() };
            string[] labels = { "A", "B", "C", "D" };
            string selectedAns = selected == null || selected == DBNull.Value ? "" : selected.ToString();
            string correctAns = correct == null || correct == DBNull.Value ? "" : correct.ToString();
            sb.Append("<div class='option-list'>");

            for (int i = 0; i < options.Length; i++)
            {
                if (String.IsNullOrWhiteSpace(options[i]))
                {
                    continue;
                }

                string css = "option-item";

                if (selectedAns.Equals(labels[i], StringComparison.OrdinalIgnoreCase) && correctAns.Equals(labels[i], StringComparison.OrdinalIgnoreCase))
                {
                    css += " option-correct option-selected";
                }
                else if (selectedAns.Equals(labels[i], StringComparison.OrdinalIgnoreCase))
                {
                    css += " option-wrong option-selected";
                }
                else if (correctAns.Equals(labels[i], StringComparison.OrdinalIgnoreCase))
                {
                    css += " option-answer";
                }

                sb.Append("<div class='" + css + "'><b>" + labels[i] + ".</b> " + HttpUtility.HtmlEncode(options[i]));

                if (selectedAns.Equals(labels[i], StringComparison.OrdinalIgnoreCase))
                {
                    sb.Append(" <span class='badge bg-secondary'>Selected</span>");
                }

                if (correctAns.Equals(labels[i], StringComparison.OrdinalIgnoreCase))
                {
                    sb.Append(" <span class='badge bg-primary'>Correct</span>");
                }

                sb.Append("</div>");
            }

            sb.Append("</div>");
            return sb.ToString();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Manager/SessionAnswerReport.aspx");
        }
    }
}