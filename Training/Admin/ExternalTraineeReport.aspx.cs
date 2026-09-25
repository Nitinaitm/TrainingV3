using System;
using System.Data;
using System.Data.SqlClient;
using OfficeOpenXml;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web;

namespace Training.Admin
{
    public partial class ExternalTraineeReport : System.Web.UI.Page
    {
        private clsDataAccess DB()
        {
            return new clsDataAccess();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindReport();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtEmpID.Text = "";
            txtEmpName.Text = "";
            txtCompany.Text = "";
            txtDesignation.Text = "";
            gvExternalTrainee.DataSource = null;
            gvExternalTrainee.DataBind();
        }

        private void BindReport()
        {
            string sql = "SELECT EmpID,EmpName,MobileNo,EmailId,EmpCompany,EmpDesignation,CreatedOn FROM EmpBasicMaster WHERE EmpType='External'";
            sql += " AND (@EmpID='' OR EmpID LIKE @EmpID)";
            sql += " AND (@EmpName='' OR EmpName LIKE @EmpName)";
            sql += " AND (@Company='' OR EmpCompany LIKE @Company)";
            sql += " AND (@Designation='' OR EmpDesignation LIKE @Designation)";
            sql += " ORDER BY ID DESC";

            DataTable dt = DB().GetDataTable(sql, new SqlParameter[] {
                new SqlParameter("@EmpID", "%" + txtEmpID.Text.Trim() + "%"),
                new SqlParameter("@EmpName", "%" + txtEmpName.Text.Trim() + "%"),
                new SqlParameter("@Company", "%" + txtCompany.Text.Trim() + "%"),
                new SqlParameter("@Designation", "%" + txtDesignation.Text.Trim() + "%")
            });

            gvExternalTrainee.DataSource = dt;
            gvExternalTrainee.DataBind();
        }

        protected void gvExternalTrainee_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditExternal")
            {
                string empID = e.CommandArgument.ToString();
                Response.Redirect("ExternalTraineeEdit.aspx?EmpID=" + Server.UrlEncode(empID));
                return;
            }

            if (e.CommandName == "DeleteExternal")
            {
                DeleteExternalTrainee(e.CommandArgument.ToString());
            }
        }

        private void DeleteExternalTrainee(string empID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlTransaction transaction = null;

                try
                {
                    con.Open();
                    transaction = con.BeginTransaction();

                    string deleteLogin = "DELETE FROM Login WHERE CorrespondingEmpID=@EmpID OR LoginIDUserID=@EmpID";
                    using (SqlCommand cmdLogin = new SqlCommand(deleteLogin, con, transaction))
                    {
                        cmdLogin.Parameters.AddWithValue("@EmpID", empID);
                        cmdLogin.ExecuteNonQuery();
                    }

                    string deleteEmployee = "DELETE FROM EmpBasicMaster WHERE EmpID=@EmpID AND EmpType='External'";
                    using (SqlCommand cmdEmployee = new SqlCommand(deleteEmployee, con, transaction))
                    {
                        cmdEmployee.Parameters.AddWithValue("@EmpID", empID);
                        int affected = cmdEmployee.ExecuteNonQuery();

                        if (affected == 0)
                        {
                            throw new Exception("External trainee record not found.");
                        }
                    }

                    transaction.Commit();
                    BindReport();
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }

                    ScriptManager.RegisterStartupScript(this, GetType(), "deleteError", "alert(" + HttpUtility.JavaScriptStringEncode(ex.Message, true) + ");", true);
                }
            }
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            BindReport();

            if (gvExternalTrainee.Rows.Count == 0)
            {
                return;
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("External Trainee");
                for (int i = 0; i < gvExternalTrainee.HeaderRow.Cells.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = gvExternalTrainee.HeaderRow.Cells[i].Text;
                }

                for (int rowIndex = 0; rowIndex < gvExternalTrainee.Rows.Count; rowIndex++)
                {
                    for (int cellIndex = 0; cellIndex < gvExternalTrainee.Rows[rowIndex].Cells.Count; cellIndex++)
                    {
                        worksheet.Cells[rowIndex + 2, cellIndex + 1].Value = Server.HtmlDecode(gvExternalTrainee.Rows[rowIndex].Cells[cellIndex].Text);
                    }
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=ExternalTraineeReport.xlsx");
                Response.BinaryWrite(package.GetAsByteArray());
                Response.End();
            }
        }
    }
}