using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class HostelStayHistoryReport : System.Web.UI.Page
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

                BindTrainingFilter();
                BindHostelFilter();
                BindGrid();
            }
        }

        private void BindTrainingFilter()
        {
            // Only trainings that actually have at least one hostel
            // allotment are relevant here.
            string query = @"SELECT DISTINCT TD.TrainingID,
                             TD.TrainingID + ' | ' + TD.TrainingType + ' | ' + TD.Batch AS TrainingName,
                             TD.CreatedOn
                             FROM HostelAllotment HA
                             INNER JOIN TrainingDetails TD ON HA.TrainingID = TD.TrainingID
                             ORDER BY TD.CreatedOn DESC";

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlTrainingFilter.DataSource = dt;
                ddlTrainingFilter.DataTextField = "TrainingName";
                ddlTrainingFilter.DataValueField = "TrainingID";
                ddlTrainingFilter.DataBind();

                ddlTrainingFilter.Items.Insert(0, new ListItem("-- All Trainings --", ""));
            }
        }

        private void BindHostelFilter()
        {
            string query = "SELECT ID, HostelName FROM HostelMaster WHERE IsActive='Y' ORDER BY HostelName";

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlHostelFilter.DataSource = dt;
                ddlHostelFilter.DataTextField = "HostelName";
                ddlHostelFilter.DataValueField = "ID";
                ddlHostelFilter.DataBind();

                ddlHostelFilter.Items.Insert(0, new ListItem("-- All Hostels --", ""));
            }
        }

        private void BindGrid()
        {
            string sql = @"SELECT 
                            HA.AllotmentID,
                            E.EmpID,
                            E.EmpName,
                            E.Gender,
                            E.EmpDesignation,
                            E.EmpCompany,
                            TD.TrainingID,
                            TD.TrainingType,
                            TD.Batch,
                            H.HostelName,
                            HB.BlockName,
                            HF.FlatName,
                            HR.RoomNo,
                            HBed.BedNo,
                            HA.AllotmentDate,
                            HA.VacateDate,
                            HA.Status,
                            CASE WHEN HA.Status = 'Allotted' THEN DATEDIFF(DAY, HA.AllotmentDate, GETDATE())
                                 ELSE DATEDIFF(DAY, HA.AllotmentDate, HA.VacateDate) END AS StayDays
                            FROM HostelAllotment HA
                            INNER JOIN EmpBasicMaster E ON HA.EmpID = E.EmpID
                            INNER JOIN TrainingDetails TD ON HA.TrainingID = TD.TrainingID
                            INNER JOIN HostelBedMaster HBed ON HA.BedID = HBed.ID
                            INNER JOIN HostelRoomMaster HR ON HBed.RoomID = HR.ID
                            LEFT JOIN HostelFlatMaster HF ON HR.FlatID = HF.ID
                            INNER JOIN HostelBlockMaster HB ON HR.BlockID = HB.ID
                            INNER JOIN HostelMaster H ON HB.HostelID = H.ID
                            WHERE 1=1";

            List<SqlParameter> parameter = new List<SqlParameter>();

            if (ddlTrainingFilter.SelectedValue != "")
            {
                sql += " AND HA.TrainingID=@TrainingID";
                parameter.Add(new SqlParameter("@TrainingID", ddlTrainingFilter.SelectedValue));
            }

            if (ddlHostelFilter.SelectedValue != "")
            {
                sql += " AND H.ID=@HostelID";
                parameter.Add(new SqlParameter("@HostelID", ddlHostelFilter.SelectedValue));
            }

            if (ddlStatusFilter.SelectedValue != "")
            {
                sql += " AND HA.Status=@Status";
                parameter.Add(new SqlParameter("@Status", ddlStatusFilter.SelectedValue));
            }

            if (txtEmpSearch.Text.Trim() != "")
            {
                sql += " AND (E.EmpID LIKE @EmpSearch OR E.EmpName LIKE @EmpSearch)";
                parameter.Add(new SqlParameter("@EmpSearch", "%" + txtEmpSearch.Text.Trim() + "%"));
            }

            if (txtDateFrom.Text.Trim() != "")
            {
                DateTime dateFrom;
                if (DateTime.TryParseExact(txtDateFrom.Text.Trim(), "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateFrom))
                {
                    sql += " AND HA.AllotmentDate >= @DateFrom";
                    parameter.Add(new SqlParameter("@DateFrom", dateFrom));
                }
            }

            if (txtDateTo.Text.Trim() != "")
            {
                DateTime dateTo;
                if (DateTime.TryParseExact(txtDateTo.Text.Trim(), "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTo))
                {
                    sql += " AND HA.AllotmentDate <= @DateTo";
                    parameter.Add(new SqlParameter("@DateTo", dateTo));
                }
            }

            sql += " ORDER BY HA.AllotmentDate DESC";

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddRange(parameter.ToArray());

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvHistory.DataSource = dt;
                gvHistory.DataBind();

                lblSummary.Text = dt.Rows.Count + " record(s) found -- " +
                    CountStatus(dt, "Allotted") + " currently staying, " +
                    CountStatus(dt, "Vacated") + " vacated.";
            }
        }

        private int CountStatus(DataTable dt, string status)
        {
            int count = 0;

            foreach (DataRow row in dt.Rows)
            {
                if (row["Status"].ToString() == status)
                {
                    count++;
                }
            }

            return count;
        }

        protected void gvHistory_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            Label lblStatus = (Label)e.Row.FindControl("lblStatus");

            if (lblStatus == null)
            {
                return;
            }

            if (lblStatus.Text == "Allotted")
            {
                lblStatus.CssClass = "badge bg-success";
            }
            else if (lblStatus.Text == "Vacated")
            {
                lblStatus.CssClass = "badge bg-secondary";
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            ddlTrainingFilter.SelectedIndex = 0;
            ddlHostelFilter.SelectedIndex = 0;
            ddlStatusFilter.SelectedIndex = 0;
            txtEmpSearch.Text = "";
            txtDateFrom.Text = "";
            txtDateTo.Text = "";

            BindGrid();
        }
    }
}