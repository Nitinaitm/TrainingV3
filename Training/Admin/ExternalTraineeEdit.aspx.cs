using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;

namespace Training.Admin
{
    public partial class ExternalTraineeEdit : System.Web.UI.Page
    {
        private string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["constr"].ConnectionString; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) LoadEmployee();
        }

        private void LoadEmployee()
        {
            string empID = Request.QueryString["EmpID"];
            if (string.IsNullOrWhiteSpace(empID))
            {
                lblMessage.Text = "Employee ID is missing.";
                lblMessage.ForeColor = Color.Red;
                return;
            }

            string sql = "SELECT EmpID,EmpName,MobileNo,EmailId,EmpCompany,EmpDesignation FROM EmpBasicMaster WHERE EmpID=@EmpID AND EmpType='External'";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@EmpID", empID);
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                    {
                        lblMessage.Text = "External trainee not found.";
                        lblMessage.ForeColor = Color.Red;
                        return;
                    }

                    txtEmpID.Text = dr["EmpID"].ToString();
                    txtEmpName.Text = dr["EmpName"].ToString();
                    txtMobileNo.Text = dr["MobileNo"].ToString();
                    txtEmailId.Text = dr["EmailId"].ToString();
                    txtOrganization.Text = dr["EmpCompany"].ToString();
                    txtDesignation.Text = dr["EmpDesignation"].ToString();
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            if (string.IsNullOrWhiteSpace(txtEmpID.Text))
            {
                lblMessage.Text = "Employee ID is missing.";
                lblMessage.ForeColor = Color.Red;
                return;
            }

            try
            {
                string sql = "UPDATE EmpBasicMaster SET EmpName=@EmpName,MobileNo=@MobileNo,EmailId=@EmailId,EmpCompany=@EmpCompany,EmpDesignation=@EmpDesignation WHERE EmpID=@EmpID AND EmpType='External'";
                using (SqlConnection con = new SqlConnection(ConnectionString))
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@EmpID", txtEmpID.Text.Trim());
                    cmd.Parameters.AddWithValue("@EmpName", txtEmpName.Text.Trim());
                    cmd.Parameters.AddWithValue("@MobileNo", txtMobileNo.Text.Trim());
                    cmd.Parameters.AddWithValue("@EmailId", txtEmailId.Text.Trim());
                    cmd.Parameters.AddWithValue("@EmpCompany", txtOrganization.Text.Trim());
                    cmd.Parameters.AddWithValue("@EmpDesignation", txtDesignation.Text.Trim());
                    con.Open();
                    int affected = cmd.ExecuteNonQuery();
                    if (affected == 0)
                    {
                        lblMessage.Text = "External trainee not found.";
                        lblMessage.ForeColor = Color.Red;
                        return;
                    }
                }

                lblMessage.Text = "External trainee updated successfully.";
                lblMessage.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
                lblMessage.ForeColor = Color.Red;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("ExternalTraineeReport.aspx");
        }
    }
}