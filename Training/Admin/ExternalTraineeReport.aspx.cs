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
                GridViewRow row = ((LinkButton)e.CommandSource).NamingContainer as GridViewRow;
                SetRowEditMode(row, true);
                return;
            }

            if (e.CommandName == "CancelExternal")
            {
                BindReport();
                return;
            }

            if (e.CommandName == "SaveExternal")
            {
                GridViewRow row = ((LinkButton)e.CommandSource).NamingContainer as GridViewRow;
                UpdateExternalTrainee(row);
                return;
            }

            if (e.CommandName == "DeleteExternal")
            {
                DeleteExternalTrainee(e.CommandArgument.ToString());
            }
        }


        private void SetRowEditMode(GridViewRow row, bool editMode)
        {
            if (row == null)
            {
                return;
            }

            ((Label)row.FindControl("lblEmpName")).Visible = !editMode;
            ((Label)row.FindControl("lblMobileNo")).Visible = !editMode;
            ((Label)row.FindControl("lblEmailId")).Visible = !editMode;
            ((Label)row.FindControl("lblCompany")).Visible = !editMode;
            ((Label)row.FindControl("lblDesignation")).Visible = !editMode;
            ((TextBox)row.FindControl("txtRowEmpName")).Visible = editMode;
            ((TextBox)row.FindControl("txtRowMobileNo")).Visible = editMode;
            ((TextBox)row.FindControl("txtRowEmailId")).Visible = editMode;
            ((TextBox)row.FindControl("txtRowCompany")).Visible = editMode;
            ((TextBox)row.FindControl("txtRowDesignation")).Visible = editMode;

            LinkButton edit = (LinkButton)row.FindControl("btnEditExternal");
            LinkButton save = (LinkButton)row.FindControl("btnSaveExternal");
            LinkButton cancel = (LinkButton)row.FindControl("btnCancelExternal");
            edit.Visible = !editMode;
            save.Visible = editMode;
            cancel.Visible = editMode;
        }

        private void UpdateExternalTrainee(GridViewRow row)
        {
            string empID = ((Label)row.FindControl("lblEmpID")).Text.Trim();

            string sql = "UPDATE EmpBasicMaster SET EmpName=@EmpName,MobileNo=@MobileNo,EmailId=@EmailId,EmpCompany=@EmpCompany,EmpDesignation=@EmpDesignation WHERE EmpID=@EmpID AND EmpType='External'";

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@EmpID", empID);
                    cmd.Parameters.AddWithValue("@EmpName", ((TextBox)row.FindControl("txtRowEmpName")).Text.Trim());
                    cmd.Parameters.AddWithValue("@MobileNo", ((TextBox)row.FindControl("txtRowMobileNo")).Text.Trim());
                    cmd.Parameters.AddWithValue("@EmailId", ((TextBox)row.FindControl("txtRowEmailId")).Text.Trim());
                    cmd.Parameters.AddWithValue("@EmpCompany", ((TextBox)row.FindControl("txtRowCompany")).Text.Trim());
                    cmd.Parameters.AddWithValue("@EmpDesignation", ((TextBox)row.FindControl("txtRowDesignation")).Text.Trim());
                    con.Open();

                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        throw new Exception("External trainee record not found.");
                    }
                }

                BindReport();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "updateError", "alert(" + HttpUtility.JavaScriptStringEncode(ex.Message, true) + ");", true);
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