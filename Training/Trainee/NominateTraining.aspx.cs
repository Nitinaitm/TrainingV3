using System;
using System.Data;
using System.Data.SqlClient;

namespace Training.Trainee
{
    public partial class NominateTraining : System.Web.UI.Page
    {
        clsDataAccess objDB = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["EmpID"] == null)
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadProfileDetails();
                BindTrainingDropdown();
            }
        }

        private void LoadProfileDetails()
        {
            string empID = Session["EmpID"].ToString().ToUpperInvariant();

            lblEmpID.Text = empID;

            string sql =
                "SELECT EmpName, EmpDesignation, EmpCompany, EmpPostingPlace, MobileNo, EmailId " +
                "FROM EmpBasicMaster WHERE EmpID=@EmpID";

            SqlParameter[] param =
            {
                new SqlParameter("@EmpID", empID)
            };

            DataTable dt = objDB.GetDataTable(sql, param);

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];

                lblName.Text = dr["EmpName"].ToString();
                lblDesignation.Text = dr["EmpDesignation"].ToString();
                lblOrganization.Text = dr["EmpCompany"].ToString();
                lblCurrentPosting.Text = dr["EmpPostingPlace"].ToString();
                lblMobile.Text = dr["MobileNo"].ToString();
                lblEmail.Text = dr["EmailId"].ToString();
            }
        }

        private void BindTrainingDropdown()
        {
            string sql =
                "SELECT TD.TrainingID, " +
                "CM.CourseName + ' (Batch ' + TD.Batch + ') - ' + " +
                "CONVERT(varchar,TRY_CONVERT(date,TD.DateFrom,105),106) + ' to ' + " +
                "CONVERT(varchar,TRY_CONVERT(date,TD.DateTo,105),106) AS TrainingDisplay " +
                "FROM TrainingDetails TD " +
                "INNER JOIN CourseMaster CM ON CM.CourseID = TD.CourseID " +
                "WHERE TRY_CONVERT(date,TD.DateFrom,105) >= CAST(GETDATE() AS DATE) " +
                "ORDER BY TRY_CONVERT(date,TD.DateFrom,105)";

            DataTable dt = objDB.GetDataTable(sql, null);

            ddlTraining.DataSource = dt;
            ddlTraining.DataTextField = "TrainingDisplay";
            ddlTraining.DataValueField = "TrainingID";
            ddlTraining.DataBind();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            string empID = Session["EmpID"].ToString().ToUpperInvariant();

            string nominationID = GenerateNominationID();

            string sql =
                "INSERT INTO OnlineNomination " +
                "(NominationID, Name, Designation, Organization, EmpID, CurrentPosting, MobileNo, EmailId, TrainingID, CourseName, Remarks, Status, SubmittedOn) " +
                "VALUES " +
                "(@NominationID, @Name, @Designation, @Organization, @EmpID, @CurrentPosting, @MobileNo, @EmailId, @TrainingID, @CourseName, @Remarks, 'Pending', GETDATE())";

            SqlParameter[] param =
            {
        new SqlParameter("@NominationID", nominationID),
        new SqlParameter("@Name", lblName.Text),
        new SqlParameter("@Designation", lblDesignation.Text),
        new SqlParameter("@Organization", lblOrganization.Text),
        new SqlParameter("@EmpID", empID),
        new SqlParameter("@CurrentPosting", lblCurrentPosting.Text),
        new SqlParameter("@MobileNo", lblMobile.Text),
        new SqlParameter("@EmailId", lblEmail.Text),
        new SqlParameter("@TrainingID", ddlTraining.SelectedValue),
        new SqlParameter("@CourseName", ddlTraining.SelectedItem.Text),
        new SqlParameter("@Remarks", string.IsNullOrWhiteSpace(txtRemarks.Text) ? (object)DBNull.Value : txtRemarks.Text.Trim())
    };

            int rowsAffected = objDB.ExecuteSql(sql, param);

            if (rowsAffected > 0)
            {
                lblMessage.CssClass = "d-block mt-3 text-success fw-semibold";
                lblMessage.Text = "Your nomination has been submitted. Reference Number: " + nominationID;

                ddlTraining.SelectedIndex = 0;
                txtRemarks.Text = "";
            }
            else
            {
                lblMessage.CssClass = "d-block mt-3 text-danger fw-semibold";
                lblMessage.Text = "Something went wrong while submitting your nomination. Please try again.";
            }
        }

        private string GenerateNominationID()
        {
            string sql = "SELECT ISNULL(MAX(CAST(RIGHT(NominationID,6) AS INT)),0)+1 FROM OnlineNomination";

            object result = objDB.ExecuteScalar(sql, null);

            int id = Convert.ToInt32(result);

            return "NOM" + id.ToString("000000");
        }
    }
}