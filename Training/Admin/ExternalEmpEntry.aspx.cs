using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class ExternalEmpEntry : System.Web.UI.Page
    {
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadNextEmpID();
            }


        }
        private void LoadNextEmpID()
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT ISNULL(MAX(ID),0)+1 FROM EmpBasicMaster";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();

                    int nextId = Convert.ToInt32(cmd.ExecuteScalar());

                    txtEmpID.Text = "Ext" + nextId.ToString("000");

                    con.Close();
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblSingleMessage.Text = "";

            try
            {

                using (SqlConnection con =
                    new SqlConnection(constr))
                {
                    string checkQuery =
                        @"SELECT COUNT(*)
                          FROM EmpBasicMaster
                          WHERE EmpID=@EmpID";

                    using (SqlCommand cmdCheck =
                        new SqlCommand(checkQuery, con))
                    {
                        cmdCheck.Parameters.AddWithValue(
                            "@EmpID",
                            txtEmpID.Text.Trim());

                        con.Open();

                        int count =
                            Convert.ToInt32(
                                cmdCheck.ExecuteScalar());

                        con.Close();

                        if (count > 0)
                        {
                            lblSingleMessage.Text =
                                "Employee ID already exists.";

                            lblSingleMessage.ForeColor =
                                Color.Red;

                            return;
                        }
                    }

                    string query =
                        @"INSERT INTO EmpBasicMaster
                        (
                            EmpID,
                            EmpName,
                           
                            MobileNo,
                            EmailId,
                            EmpCompany,
                            EmpDesignation,                            
                            CreatedOn,
                            CreatedBy,
EmpType
                        )
                        VALUES
                        (
                            @EmpID,
                            @EmpName,
                           
                            @MobileNo,
                            @EmailId,
                            @EmpCompany,
                            @EmpDesignation,
                           
                            GETDATE(),
                            @CreatedBy,
@EmpType
                        )";

                    using (SqlCommand cmd =
                        new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue(
                            "@EmpID",
                            txtEmpID.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@EmpName",
                            txtEmpName.Text.Trim());



                        cmd.Parameters.AddWithValue(
                            "@MobileNo",
                            txtMobileNo.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@EmailId",
                            txtEmailId.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@EmpCompany",
                            txtOrganization.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@EmpDesignation",
                            txtDesignation.Text.Trim());



                        cmd.Parameters.AddWithValue(
                            "@CreatedBy",
                            "Admin");
                        cmd.Parameters.AddWithValue(
                            "@EmpType",
                           "External");

                        con.Open();

                        cmd.ExecuteNonQuery();

                        Encryptor2 encryptor = new Encryptor2();
                        string password = encryptor.Encrypt("Bsphcl" + "*123");
                        string firstLogin = encryptor.Encrypt("Y");
                        string loginCheckQuery = "SELECT COUNT(*) FROM Login WHERE LoginIDUserID=@LoginIDUserID";
                        using (SqlCommand cmdLoginCheck = new SqlCommand(loginCheckQuery, con))
                        {
                            cmdLoginCheck.Parameters.AddWithValue("@LoginIDUserID", txtEmpID.Text.Trim().ToUpperInvariant());
                            int loginExists = Convert.ToInt32(cmdLoginCheck.ExecuteScalar());
                            if (loginExists == 0)
                            {
                                string loginQuery = "INSERT INTO Login(LoginIDUserID,Password,Role,' + String.fromCharCode(67,111,114,114,101,115,112,111,110,100,105,110,103,69,109,112,73,68) + ",Active,re) VALUES(@LoginIDUserID,@Password,'Trainee',@CorrespondingEmpID,'Y',@FirstLogin)";
                                using (SqlCommand cmdLogin = new SqlCommand(loginQuery, con))
                                {
                                    cmdLogin.Parameters.AddWithValue("@LoginIDUserID", txtEmpID.Text.Trim().ToUpperInvariant());
                                    cmdLogin.Parameters.AddWithValue("@Password", password);
                                    cmdLogin.Parameters.AddWithValue("@CorrespondingEmpID", txtEmpID.Text.Trim().ToUpperInvariant());
                                    cmdLogin.Parameters.AddWithValue("@FirstLogin", firstLogin);
                                    cmdLogin.ExecuteNonQuery();
                                }
                            }
                        }

                        con.Close();
                    }
                }

                lblSingleMessage.Text =
                    txtEmpID.Text + " Employee saved successfully.";

                lblSingleMessage.ForeColor =
                    Color.Green;

                ClearControls();
                LoadNextEmpID();



            }
            catch (Exception ex)
            {
                lblSingleMessage.Text =
                    ex.Message;

                lblSingleMessage.ForeColor =
                    Color.Red;
            }
        }
       



        private void ClearControls()
        {
            txtEmpID.Text = "";
            txtEmpName.Text = "";

            txtMobileNo.Text = "";
            txtEmailId.Text = "";
            txtDesignation.Text = "";
            txtOrganization.Text = "";


        }


    }
}
