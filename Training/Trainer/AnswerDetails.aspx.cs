using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Training.Trainer
{
    public partial class AnswerDetails : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TrainerID"] == null || String.IsNullOrWhiteSpace(Session["TrainerID"].ToString()))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (Session["TrainerAnswerResultID"] == null || String.IsNullOrWhiteSpace(Session["TrainerAnswerResultID"].ToString()))
            {
                Response.Redirect("~/Trainer/SessionAnswerReport.aspx");
                return;
            }

            if (!IsPostBack)
            {
                if (!LoadResult())
                {
                    Response.Redirect("~/Trainer/SessionAnswerReport.aspx");
                    return;
                }

                LoadAnswers();
            }
        }

        private string ResultID { get { return Session["TrainerAnswerResultID"].ToString().Trim(); } }
        private string TrainerID { get { return Session["TrainerID"].ToString(); } }

        private bool LoadResult()
        {
            string query = "SELECT R.ResultID,R.TestID,R.EmpID,R.TotalQuestions,R.AttemptedQuestions,R.CorrectAnswers,R.Score,R.Status,R.ResultStatus,R.AttemptNo,R.SubmittedOn,R.IsFinalAttempt,TM.TestTitle,TM.TestType,E.EmpName,E.EmpDesignation,TME.TraineeName FROM TestResult R INNER JOIN TestMaster TM ON R.TestID=TM.TestID INNER JOIN SessionMaster SM ON TM.SessionID=SM.SessionID LEFT JOIN EmpBasicMaster E ON R.EmpID=E.EmpID LEFT JOIN TraineeMasterExternal TME ON TME.EmpIDExternal=R.EmpID WHERE R.ResultID=@ResultID AND SM.SessionID=@SessionID AND SM.TrainingID=@TrainingID AND SM.TrainerID=@TrainerID";
            DataTable dt = obj.GetDataTable(query, new SqlParameter[] { new SqlParameter("@ResultID", ResultID), new SqlParameter("@SessionID", Session["SessionID"].ToString()), new SqlParameter("@TrainingID", Session["TrainingID"].ToString()), new SqlParameter("@TrainerID", TrainerID) });
            if (dt.Rows.Count == 0) return false;
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
            lblScore.Text = Convert.ToDecimal(dr["Score"]).ToString("0.00") + "%";
            string status = String.IsNullOrWhiteSpace(dr["ResultStatus"].ToString()) ? dr["Status"].ToString() : dr["ResultStatus"].ToString();
            lblStatus.Text = status;
            lblStatus.CssClass = status.Equals("Pass", StringComparison.OrdinalIgnoreCase) || status.Equals("Passed", StringComparison.OrdinalIgnoreCase) ? "info-value result-pass" : "info-value result-fail";
            return true;
        }

        private void LoadAnswers()
        {
            string query = "SELECT QB.Question,QB.Type,QB.OptionA,QB.OptionB,QB.OptionC,QB.OptionD,QB.Answer AS CorrectAnswer,TA.SelectedAnswer,TA.IsCorrect FROM TestAttempt TA INNER JOIN QuestionBank QB ON TA.QuestionID=QB.QuestionID INNER JOIN TestResult R ON TA.ResultID=R.ResultID INNER JOIN TestMaster TM ON R.TestID=TM.TestID INNER JOIN SessionMaster SM ON TM.SessionID=SM.SessionID WHERE TA.ResultID=@ResultID AND R.ResultID=@ResultID AND SM.SessionID=@SessionID AND SM.TrainingID=@TrainingID AND SM.TrainerID=@TrainerID ORDER BY TA.SequenceNo";
            DataTable dt = obj.GetDataTable(query, new SqlParameter[] { new SqlParameter("@ResultID", ResultID), new SqlParameter("@SessionID", Session["SessionID"].ToString()), new SqlParameter("@TrainingID", Session["TrainingID"].ToString()), new SqlParameter("@TrainerID", TrainerID) });
            gvAnswers.DataSource = dt;
            gvAnswers.DataBind();
        }

        protected bool IsCorrect(object value) { return value != null && value != DBNull.Value && Convert.ToBoolean(value); }

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
                if (String.IsNullOrWhiteSpace(options[i])) continue;
                string css = "option-item";
                if (selectedAns.Equals(labels[i], StringComparison.OrdinalIgnoreCase) && correctAns.Equals(labels[i], StringComparison.OrdinalIgnoreCase)) css += " option-correct option-selected";
                else if (selectedAns.Equals(labels[i], StringComparison.OrdinalIgnoreCase)) css += " option-wrong option-selected";
                else if (correctAns.Equals(labels[i], StringComparison.OrdinalIgnoreCase)) css += " option-answer";
                sb.Append("<div class='" + css + "'><b>" + labels[i] + ".</b> " + HttpUtility.HtmlEncode(options[i]));
                if (selectedAns.Equals(labels[i], StringComparison.OrdinalIgnoreCase)) sb.Append(" <span class='badge bg-secondary'>Selected</span>");
                if (correctAns.Equals(labels[i], StringComparison.OrdinalIgnoreCase)) sb.Append(" <span class='badge bg-primary'>Correct</span>");
                sb.Append("</div>");
            }
            sb.Append("</div>");
            return sb.ToString();
        }

        protected void btnBack_Click(object sender, EventArgs e) { Response.Redirect("~/Trainer/SessionAnswerReport.aspx"); }
    }
}