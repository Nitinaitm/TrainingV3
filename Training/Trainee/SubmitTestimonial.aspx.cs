using System;
using System.Data;
using System.Data.SqlClient;

namespace Training.Trainee
{
    public partial class SubmitTestimonial : System.Web.UI.Page
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
            }
        }

        private void LoadProfileDetails()
        {
            string empID = Session["EmpID"].ToString().ToUpperInvariant();

            string sql = "SELECT EmpName, EmpDesignation FROM EmpBasicMaster WHERE EmpID=@EmpID";

            SqlParameter[] param = { new SqlParameter("@EmpID", empID) };

            DataTable dt = objDB.GetDataTable(sql, param);

            if (dt.Rows.Count > 0)
            {
                lblName.Text = dt.Rows[0]["EmpName"].ToString();
                lblRole.Text = dt.Rows[0]["EmpDesignation"].ToString();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            string empID = Session["EmpID"].ToString().ToUpperInvariant();

            string sql =
                "INSERT INTO HomeTestimonial (Quote, Name, Role, Status, IsActive, SubmittedBy, DisplayOrder, CreatedOn) " +
                "VALUES (@Quote, @Name, @Role, 'Pending', 0, @SubmittedBy, 1, GETDATE())";

            SqlParameter[] param =
            {
                new SqlParameter("@Quote", txtQuote.Text.Trim()),
                new SqlParameter("@Name", lblName.Text),
                new SqlParameter("@Role", lblRole.Text),
                new SqlParameter("@SubmittedBy", empID)
            };

            int rowsAffected = objDB.ExecuteSql(sql, param);

            if (rowsAffected > 0)
            {
                lblMessage.CssClass = "d-block mt-3 text-success fw-semibold";
                lblMessage.Text = "Thank you - your feedback has been submitted and will appear once reviewed.";

                txtQuote.Text = "";
            }
            else
            {
                lblMessage.CssClass = "d-block mt-3 text-danger fw-semibold";
                lblMessage.Text = "Something went wrong. Please try again.";
            }
        }
    }
}