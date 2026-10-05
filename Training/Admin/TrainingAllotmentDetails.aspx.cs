using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class TrainingAllotmentDetails : System.Web.UI.Page
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

                BindTrainingDropdown();
            }
        }

        private void BindTrainingDropdown()
        {
            string query = @"SELECT DISTINCT T.TrainingID, T.TrainingID + ' | ' + TD.TrainingType + ' | ' + TD.Batch AS TrainingName, TD.CreatedOn
                             FROM TrainingAssignment T INNER JOIN TrainingDetails TD ON T.TrainingID = TD.TrainingID
                             ORDER BY TD.CreatedOn DESC";

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlReportTraining.DataSource = dt;
                ddlReportTraining.DataTextField = "TrainingName";
                ddlReportTraining.DataValueField = "TrainingID";
                ddlReportTraining.DataBind();

                ddlReportTraining.Items.Insert(0, new ListItem("-- Select Training --", ""));
            }
        }

        protected void btnShowReport_Click(object sender, EventArgs e)
        {
            lblReportMessage.Text = "";

            if (ddlReportTraining.SelectedValue == "")
            {
                lblReportMessage.Text = "Please select a Training first.";
                lblReportMessage.ForeColor = System.Drawing.Color.Red;
                pnlReportTrainingDetails.Visible = false;
                gvReport.DataSource = null;
                gvReport.DataBind();
                return;
            }

            LoadTrainingDetailsPanel(ddlReportTraining.SelectedValue);
            LoadReportGrid(ddlReportTraining.SelectedValue);
        }

        private void LoadTrainingDetailsPanel(string trainingID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT TrainingID, TrainingType, Batch, DateFrom, DateTo, TrainingLocation, TrainingOrganizer FROM TrainingDetails WHERE TrainingID = @TrainingID",
                    con);
                cmd.Parameters.AddWithValue("@TrainingID", trainingID);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            if (dt.Rows.Count == 0)
            {
                pnlReportTrainingDetails.Visible = false;
                return;
            }

            DataRow dr = dt.Rows[0];

            pnlReportTrainingDetails.Visible = true;

            lblRptTrainingID.Text = dr["TrainingID"].ToString();
            lblRptTrainingType.Text = dr["TrainingType"].ToString();
            lblRptBatch.Text = dr["Batch"].ToString();
            lblRptDates.Text = dr["DateFrom"].ToString() + " to " + dr["DateTo"].ToString();
            lblRptLocation.Text = dr["TrainingLocation"].ToString();
            lblRptOrganizer.Text = dr["TrainingOrganizer"].ToString();
        }

        private void LoadReportGrid(string trainingID)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                String query = @"SELECT
                                HA.ID AS AllotmentPK,
                                HBed.ID AS BedPK,
                                TA.EmpID,
                                E.EmpName,
                                E.gender,
                                H.HostelName,
                                HB.BlockName,
                                HF.FlatName,
                                HR.RoomNo,
                                HBed.BedNo
                                FROM TrainingAssignment TA
                                INNER JOIN EmpBasicMaster E ON TA.EmpID = E.EmpID
                                LEFT JOIN HostelAllotment HA ON HA.TrainingID = TA.TrainingID AND 
                                HA.EmpID = TA.EmpID AND HA.Status = 'Allotted'
                                LEFT JOIN HostelBedMaster HBed ON HA.BedID = HBed.ID
                                LEFT JOIN HostelRoomMaster HR ON HBed.RoomID = HR.ID
                                LEFT JOIN HostelBlockMaster HB ON HR.BlockID = HB.ID
                                LEFT JOIN HostelMaster H ON HB.HostelID = H.ID
                                LEFT JOIN HostelFlatMaster HF ON HR.FlatID = HF.ID
                                WHERE TA.TrainingID = @TrainingID
                                ORDER BY E.EmpName";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@TrainingID", trainingID);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                gvReport.DataSource = dt;
                gvReport.DataBind();
            }
        }

        protected void gvReport_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                object hostelValue = DataBinder.Eval(e.Row.DataItem, "HostelName");

                if (hostelValue == DBNull.Value)
                {
                    Label lblhostelName = (Label)e.Row.FindControl("lblhostelName");
                    lblhostelName.Text = "No Bed Allotted";
                    lblhostelName.ForeColor = System.Drawing.Color.Red;

                    LinkButton btnVacate = (LinkButton)e.Row.FindControl("btnVacate");
                    btnVacate.Visible = false;
                }
                else
                {
                    LinkButton btnVacate = (LinkButton)e.Row.FindControl("btnVacate");
                    btnVacate.Visible = true;
                }
            }
        }

        protected void gvReport_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Vacate")
            {
                int allotmentID = Convert.ToInt32(e.CommandArgument);
                VacateAllotment(allotmentID);
            }
        }

        private void VacateAllotment(int allotmentID)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    string getBedQuery = "SELECT BedID FROM HostelAllotment WHERE ID = @AllotmentID";
                    SqlCommand getBedCmd = new SqlCommand(getBedQuery, con, tran);
                    getBedCmd.Parameters.AddWithValue("@AllotmentID", allotmentID);
                    int bedID = Convert.ToInt32(getBedCmd.ExecuteScalar());

                    string vacateQuery = @"UPDATE HostelAllotment SET Status = 'Vacated', VacateDate = GETDATE() WHERE ID = @AllotmentID";
                    SqlCommand vacateCmd = new SqlCommand(vacateQuery, con, tran);
                    vacateCmd.Parameters.AddWithValue("@AllotmentID", allotmentID);
                    vacateCmd.ExecuteNonQuery();

                    string freeBedQuery = "UPDATE HostelBedMaster SET Status = 'Vacant' WHERE ID = @BedID";
                    SqlCommand freeBedCmd = new SqlCommand(freeBedQuery, con, tran);
                    freeBedCmd.Parameters.AddWithValue("@BedID", bedID);
                    freeBedCmd.ExecuteNonQuery();

                    tran.Commit();

                    lblReportMessage.Text = "Allotment vacated successfully.";
                    lblReportMessage.ForeColor = System.Drawing.Color.Green;
                    HostelStatusDashboard1.BindHostelStatus();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    lblReportMessage.Text = "Could not vacate: " + ex.Message;
                    lblReportMessage.ForeColor = System.Drawing.Color.Red;
                    return;
                }
            }

            LoadReportGrid(ddlReportTraining.SelectedValue);
        }
    }
}