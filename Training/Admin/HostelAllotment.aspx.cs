using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class HostelAllotment : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        // Small holding class for a validated row selection at Save time --
        // avoids re-reading the GridView controls twice (once to validate,
        // once to insert).
        private class AllotmentSelection
        {
            public string EmpID;
            public string EmpName;
            public int BedID;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
            if (!IsPostBack)
            {
                if (Session["InternalRedirect_Admin"] == null)
                {
                    Response.Redirect("~/Default.aspx");
                    return;
                }

                BindHostelDropdownControl(ddlBulkHostel);

                mvAllotment.ActiveViewIndex = 0;
                BindAutoHostelDropdown();

                string handoffTrainingID = Session["HostelTrainingID"] as string;
                Session.Remove("HostelTrainingID");

                if (!string.IsNullOrWhiteSpace(handoffTrainingID))
                {
                    LoadTrainingDirect(handoffTrainingID);
                }
                else
                {
                    BindTrainingDropdown();
                }

                UpdateBulkTrainingLabel();
                UpdateSummaryTrainingLabel();
            }
        }

        // Keeps the Bulk tab's read-only training display in sync with
        // whatever training is currently active on the Manual tab, since Bulk
        // no longer has its own separate training picker.
        private void UpdateBulkTrainingLabel()
        {
            lblBulkTrainingInfo.Text = ddlTraining.SelectedValue == ""
                ? "No training selected yet -- pick one in Manual Allotment first."
                : ddlTraining.SelectedItem.Text;
        }

        // Keeps the Allotment Summary tab's read-only training display in sync
        // with whatever training is currently active on the Manual tab, since
        // Summary no longer has its own separate training picker.
        private void UpdateSummaryTrainingLabel()
        {
            lblSummaryTrainingInfo.Text = ddlTraining.SelectedValue == ""
                ? "No training selected yet -- pick one in Manual Allotment first."
                : ddlTraining.SelectedItem.Text;
        }
        private void LoadTrainingDirect(string trainingID)
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
                // Training not found -- fall back to the normal picker instead of
                // showing an empty details panel.
                BindTrainingDropdown();
                return;
            }

            DataRow dr = dt.Rows[0];

            // Feed ddlTraining a single pre-selected item instead of the full
            // list, so every existing method that reads ddlTraining.SelectedValue
            // (RowDataBound, btnSaveAllotment_Click, the row cascade handlers)
            // keeps working completely unchanged.
            ddlTraining.Items.Clear();
            ddlTraining.Items.Add(new ListItem(trainingID, trainingID));
            ddlTraining.SelectedValue = trainingID;

            pnlTrainingPicker.Visible = false;
            pnlTrainingDetails.Visible = true;

            lblSelectedTrainingID.Text = dr["TrainingID"].ToString();
            lblSelectedTrainingType.Text = dr["TrainingType"].ToString();
            lblSelectedBatch.Text = dr["Batch"].ToString();
            lblSelectedDates.Text = dr["DateFrom"].ToString() + " to " + dr["DateTo"].ToString();
            lblSelectedLocation.Text = dr["TrainingLocation"].ToString();
            lblSelectedOrganizer.Text = dr["TrainingOrganizer"].ToString();

            BindParticipantGrid(trainingID);
            UpdateBulkTrainingLabel();
            UpdateSummaryTrainingLabel();
        }
    



        //private void BindAutoTrainingDropdown()
        //{
        //    using (SqlConnection con = new SqlConnection(constr))
        //    {
        //        string query = @"SELECT DISTINCT T.TrainingID, T.TrainingID + ' | ' + TD.TrainingType + ' | ' + TD.Batch AS TrainingName
        //                     FROM TrainingAssignment T INNER JOIN TrainingDetails TD ON T.TrainingID = TD.TrainingID
        //                     ORDER BY T.TrainingID DESC";
        //        SqlDataAdapter adapter = new SqlDataAdapter(query, con);
        //        DataTable dt = new DataTable();
        //        adapter.Fill(dt);

        //        ddlAutoTraining.DataSource = dt;
        //        ddlAutoTraining.DataTextField = "TrainingName";
        //        ddlAutoTraining.DataValueField = "TrainingID";
        //        ddlAutoTraining.DataBind();
        //        ddlAutoTraining.Items.Insert(0, new ListItem("--Select Training--", ""));
        //    }
        //}

        private void BindAutoHostelDropdown()
        {
            string query = "SELECT ID, HostelName FROM HostelMaster WHERE IsActive='Y' ORDER BY HostelName";

            DataTable dt = obj.GetDataTable(query);

            ddlAutoHostel.DataSource = dt;
            ddlAutoHostel.DataTextField = "HostelName";
            ddlAutoHostel.DataValueField = "ID";
            ddlAutoHostel.DataBind();

            ddlAutoHostel.Items.Insert(0, new ListItem("-- Select Hostel --", ""));
           

        }

        private void BindTrainingDropdown()
        {
            // Only trainings that actually have trainees assigned make
            // sense to allot hostel rooms for. Rather than trusting
            // TrainingDetails.TrainingStatus (which may not be reliably
            // set to a particular string), pull straight from
            // TrainingAssignment -- same working approach as
            // TrainingAttendance.aspx.cs's BindTraining(). A training only
            // shows up here once someone has actually been assigned to it.
             
            string query = @"SELECT DISTINCT T.TrainingID, T.TrainingID + ' | ' + TD.TrainingType + ' | ' + TD.Batch AS TrainingName, TD.CreatedOn
                             FROM TrainingAssignment T INNER JOIN TrainingDetails TD ON T.TrainingID = TD.TrainingID
                             ORDER BY TD.CreatedOn DESC";

            DataTable dt = obj.GetDataTable(query);

            ddlTraining.DataSource = dt;
            ddlTraining.DataTextField = "TrainingName";
            ddlTraining.DataValueField = "TrainingID";
            ddlTraining.DataBind();

            ddlTraining.Items.Insert(0, new ListItem("-- Select Training --", ""));
        }

        protected void ddlTraining_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            if (ddlTraining.SelectedValue == "")
            {
                gvParticipants.DataSource = null;
                gvParticipants.DataBind();
                UpdateBulkTrainingLabel();
                UpdateSummaryTrainingLabel();
                return;
            }

            BindParticipantGrid(ddlTraining.SelectedValue);
            UpdateBulkTrainingLabel();
            UpdateSummaryTrainingLabel();
        }

        private void BindParticipantGrid(string trainingID)
        {
            string query = @"SELECT TA.EmpID, E.EmpName, E.EmpDesignation, E.EmpCompany, E.Gender FROM TrainingAssignment TA 
                             INNER JOIN EmpBasicMaster E ON TA.EmpID = E.EmpID WHERE TA.TrainingID = @TrainingID
                             ORDER BY E.EmpName";

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@TrainingID", trainingID);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // This DataBind() is what fires RowDataBound below, once
                // per row -- that's where each row gets its own Hostel
                // dropdown populated. We only call this when the Training
                // changes, never on every postback, or every row's
                // in-progress Block/Room/Bed selections would be wiped out.
                gvParticipants.DataSource = dt;
                gvParticipants.DataBind();
            }
        }

        //private void BindddlSummaryTraining()
        //{
        //    string query = @"SELECT DISTINCT T.TrainingID, T.TrainingID + ' | ' + TD.TrainingType + ' | ' + TD.Batch AS TrainingName
        //                     FROM TrainingAssignment T INNER JOIN TrainingDetails TD ON T.TrainingID = TD.TrainingID
        //                     ORDER BY T.TrainingID DESC";

        //    DataTable dt = obj.GetDataTable(query);

        //    ddlSummaryTraining.DataSource = dt;
        //    ddlSummaryTraining.DataTextField = "TrainingName";
        //    ddlSummaryTraining.DataValueField = "TrainingID";
        //    ddlSummaryTraining.DataBind();
        //    ddlSummaryTraining.Items.Insert(0, new ListItem("--Select Training--", ""));
        //}
        // Fires once per data row, right after BindParticipantGrid's
        // DataBind(). This is where we give each row its own independent
        // Hostel dropdown and reset its downstream dropdowns to their
        // "pick the level above first" placeholder state.
        // Fires once per data row, right after BindParticipantGrid's
        // DataBind(). This is where we give each row its own independent
        // Hostel dropdown and reset its downstream dropdowns to their
        // "pick the level above first" placeholder state.
        protected void gvParticipants_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            DropDownList ddlHostelRow = (DropDownList)e.Row.FindControl("ddlHostelRow");
            BindHostelDropdownControl(ddlHostelRow);

            DropDownList ddlBlockRow = (DropDownList)e.Row.FindControl("ddlBlockRow");
            ResetDropdown(ddlBlockRow, "-- Select Hostel First --");

            DropDownList ddlRoomRow = (DropDownList)e.Row.FindControl("ddlRoomRow");
            ResetDropdown(ddlRoomRow, "-- Select Block First --");

            DropDownList ddlBedRow = (DropDownList)e.Row.FindControl("ddlBedRow");
            ResetDropdown(ddlBedRow, "-- Select Room First --");

            // Already-allotted check happens HERE, before the user can touch
            // anything -- not inside btnSaveAllotment_Click, where it would run
            // too late to actually stop a duplicate allotment.
            string empID = gvParticipants.DataKeys[e.Row.RowIndex].Value.ToString();

            string query = "SELECT COUNT(*) FROM HostelAllotment WHERE TrainingID = @TrainingID AND EmpID = @EmpID AND Status = 'Allotted'";

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@TrainingID", ddlTraining.SelectedValue);
                cmd.Parameters.AddWithValue("@EmpID", empID);

                int existingCount = Convert.ToInt32(cmd.ExecuteScalar());

                if (existingCount > 0)
                {
                    CheckBox chkSelect = (CheckBox)e.Row.FindControl("chkSelect");
                    chkSelect.Enabled = false;

                    // Also lock the dropdowns -- a disabled checkbox alone still
                    // lets someone fiddle with Block/Room/Bed for a row that can
                    // never actually be saved.
                    ddlHostelRow.Enabled = false;
                    ddlBlockRow.Enabled = false;
                    ddlRoomRow.Enabled = false;
                    ddlBedRow.Enabled = false;

                    // The dropdowns above only have their placeholder item at this
                    // point -- fill in what was actually allotted, so a disabled
                    // row still shows where this person is staying instead of
                    // "-- Select Hostel --" etc.
                    string detailQuery = @"SELECT H.HostelName, HB.BlockName,
                                    CASE WHEN HF.FlatName IS NULL THEN HR.RoomNo ELSE HR.RoomNo + ' (' + HF.FlatName + ')' END AS RoomDisplay,
                                    HBed.BedNo
                                    FROM HostelAllotment HA
                                    INNER JOIN HostelBedMaster HBed ON HA.BedID = HBed.ID
                                    INNER JOIN HostelRoomMaster HR ON HBed.RoomID = HR.ID
                                    LEFT JOIN HostelFlatMaster HF ON HR.FlatID = HF.ID
                                    INNER JOIN HostelBlockMaster HB ON HR.BlockID = HB.ID
                                    INNER JOIN HostelMaster H ON HB.HostelID = H.ID
                                    WHERE HA.TrainingID = @TrainingID AND HA.EmpID = @EmpID AND HA.Status = 'Allotted'";

                    SqlCommand detailCmd = new SqlCommand(detailQuery, con);
                    detailCmd.Parameters.AddWithValue("@TrainingID", ddlTraining.SelectedValue);
                    detailCmd.Parameters.AddWithValue("@EmpID", empID);

                    using (SqlDataReader detailReader = detailCmd.ExecuteReader())
                    {
                        if (detailReader.Read())
                        {
                            SetSingleSelectedItem(ddlHostelRow, detailReader["HostelName"].ToString());
                            SetSingleSelectedItem(ddlBlockRow, detailReader["BlockName"].ToString());
                            SetSingleSelectedItem(ddlRoomRow, detailReader["RoomDisplay"].ToString());
                            SetSingleSelectedItem(ddlBedRow, detailReader["BedNo"].ToString());
                        }
                    }
                }
            }
        }

        // Shows one fixed value in a dropdown that's about to be disabled --
        // used for already-allotted rows so they display where the person
        // actually got placed, instead of an empty placeholder.
        private void SetSingleSelectedItem(DropDownList ddl, string text)
        {
            ddl.Items.Clear();
            ddl.Items.Add(new ListItem(text, text));
            ddl.SelectedIndex = 0;
        }

        private void BindHostelDropdownControl(DropDownList ddl)
        {
            string query = "SELECT ID, HostelName FROM HostelMaster WHERE IsActive='Y' ORDER BY HostelName";

            DataTable dt = obj.GetDataTable(query);

            ddl.DataSource = dt;
            ddl.DataTextField = "HostelName";
            ddl.DataValueField = "ID";
            ddl.DataBind();

            ddl.Items.Insert(0, new ListItem("-- Select Hostel --", ""));
        }

        private void BindBlockDropdownForRow(DropDownList ddl, int hostelID, string gender)
        {
            string query = @"SELECT ID, BlockName FROM HostelBlockMaster WHERE HostelID = @HostelID AND IsActive = 'Y'
                             AND (HostelCategory = @Gender OR HostelCategory = 'Mixed') ORDER BY BlockName";

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@HostelID", hostelID);
                cmd.Parameters.AddWithValue("@Gender", gender);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddl.DataSource = dt;
                ddl.DataTextField = "BlockName";
                ddl.DataValueField = "ID";
                ddl.DataBind();

                ddl.Items.Insert(0, new ListItem("-- Select Block --", ""));
            }
        }

        private void BindRoomDropdownForRow(DropDownList ddl, int blockID)
        {
            //string query = @"SELECT ID, RoomNo FROM HostelRoomMaster WHERE BlockID = @BlockID
            //                 AND IsActive = 'Y' ORDER BY RoomNo";

            string query = @"SELECT HR.ID, CASE 
                                                WHEN HF.FlatName IS NULL THEN HR.RoomNo
                                                ELSE HR.RoomNo + ' (' + HF.FlatName + ')' END AS RoomDisPlay 
                                                FROM HostelRoomMaster HR LEFT JOIN HostelFlatMaster HF ON HR.FlatID = HF.ID
                                                WHERE HR.BlockID = @BlockID AND HR.IsActive = 'Y' ORDER BY HR.RoomNo";

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@BlockID", blockID);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddl.DataSource = dt;
                ddl.DataTextField = "RoomDisPlay";
                ddl.DataValueField = "ID";
                ddl.DataBind();

                ddl.Items.Insert(0, new ListItem("-- Select Room --", ""));
            }
        }

        private void BindBedDropdownForRow(DropDownList ddl, int roomID)
        {
            // Only vacant beds -- this is the whole point of tracking
            // Status on HostelBedMaster.
            string query = @"SELECT ID, BedNo FROM HostelBedMaster WHERE RoomID = @RoomID AND Status = 'Vacant'
                             ORDER BY BedNo";

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@RoomID", roomID);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddl.DataSource = dt;
                ddl.DataTextField = "BedNo";
                ddl.DataValueField = "ID";
                ddl.DataBind();

                if (dt.Rows.Count == 0)
                    ddl.Items.Insert(0, new ListItem("-- No Vacant Beds --", ""));
                else
                    ddl.Items.Insert(0, new ListItem("-- Select Bed --", ""));
            }
        }

        private void ResetDropdown(DropDownList ddl, string placeholderText)
        {
            ddl.Items.Clear();
            ddl.Items.Insert(0, new ListItem(placeholderText, ""));
        }

        // ---- Per-row cascade handlers ----
        // Each dropdown's postback tells us WHICH control fired via
        // "sender" -- NamingContainer walks up to that control's parent
        // GridViewRow, and FindControl locates its sibling dropdowns in
        // the SAME row. This is the one new technique this page relies on.

        protected void ddlHostelRow_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlHostelRow = (DropDownList)sender;
            GridViewRow row = (GridViewRow)ddlHostelRow.NamingContainer;
            string gender = gvParticipants.DataKeys[row.RowIndex]["Gender"].ToString();

            DropDownList ddlBlockRow = (DropDownList)row.FindControl("ddlBlockRow");
            DropDownList ddlRoomRow = (DropDownList)row.FindControl("ddlRoomRow");
            DropDownList ddlBedRow = (DropDownList)row.FindControl("ddlBedRow");

            ResetDropdown(ddlRoomRow, "-- Select Block First --");
            ResetDropdown(ddlBedRow, "-- Select Room First --");

            if (ddlHostelRow.SelectedValue == "")
            {
                ResetDropdown(ddlBlockRow, "-- Select Hostel First --");
                return;
            }

            BindBlockDropdownForRow(ddlBlockRow, Convert.ToInt32(ddlHostelRow.SelectedValue),gender);
        }

        protected void ddlBlockRow_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlBlockRow = (DropDownList)sender;
            GridViewRow row = (GridViewRow)ddlBlockRow.NamingContainer;

            DropDownList ddlRoomRow = (DropDownList)row.FindControl("ddlRoomRow");
            DropDownList ddlBedRow = (DropDownList)row.FindControl("ddlBedRow");

            ResetDropdown(ddlBedRow, "-- Select Room First --");

            if (ddlBlockRow.SelectedValue == "")
            {
                ResetDropdown(ddlRoomRow, "-- Select Block First --");
                return;
            }

            BindRoomDropdownForRow(ddlRoomRow, Convert.ToInt32(ddlBlockRow.SelectedValue));
        }

        protected void ddlRoomRow_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlRoomRow = (DropDownList)sender;
            GridViewRow row = (GridViewRow)ddlRoomRow.NamingContainer;

            DropDownList ddlBedRow = (DropDownList)row.FindControl("ddlBedRow");

            if (ddlRoomRow.SelectedValue == "")
            {
                ResetDropdown(ddlBedRow, "-- Select Room First --");
                return;
            }

            BindBedDropdownForRow(ddlBedRow, Convert.ToInt32(ddlRoomRow.SelectedValue));
        }

        // ---- Bulk apply ----

        protected void btnApplyBulk_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            if (ddlBulkHostel.SelectedValue == "")
            {
                lblMessage.Text = "Select a hostel to bulk-apply first.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            int hostelID = Convert.ToInt32(ddlBulkHostel.SelectedValue);
            bool anyChecked = false;

            foreach (GridViewRow row in gvParticipants.Rows)
            {
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");

                if (chkSelect == null || !chkSelect.Checked)
                    continue;

                anyChecked = true;
                string gender = gvParticipants.DataKeys[row.RowIndex]["Gender"].ToString();

                DropDownList ddlHostelRow = (DropDownList)row.FindControl("ddlHostelRow");
                DropDownList ddlBlockRow = (DropDownList)row.FindControl("ddlBlockRow");
                DropDownList ddlRoomRow = (DropDownList)row.FindControl("ddlRoomRow");
                DropDownList ddlBedRow = (DropDownList)row.FindControl("ddlBedRow");
 
                ddlHostelRow.SelectedValue = hostelID.ToString();

                BindBlockDropdownForRow(ddlBlockRow, hostelID, gender);
                ResetDropdown(ddlRoomRow, "-- Select Block First --");
                ResetDropdown(ddlBedRow, "-- Select Room First --");
            }

            if (!anyChecked)
            {
                lblMessage.Text = "Check at least one trainee before applying a bulk hostel.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

        // ---- Save ----

        protected void btnSaveAllotment_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            List<string> errors = new List<string>();
            List<AllotmentSelection> selections = new List<AllotmentSelection>();

            foreach (GridViewRow row in gvParticipants.Rows)
            {
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");

                if (chkSelect == null || !chkSelect.Checked)
                    continue;

                Label lblEmpName = (Label)row.FindControl("lblEmpName");
                DropDownList ddlBedRow = (DropDownList)row.FindControl("ddlBedRow");

                if (ddlBedRow.SelectedValue == "")
                {
                    errors.Add("Select a bed for " + lblEmpName.Text + ".");
                    continue;
                }

                selections.Add(new AllotmentSelection
                {
                    EmpID = gvParticipants.DataKeys[row.RowIndex].Value.ToString(),
                    EmpName = lblEmpName.Text,
                    BedID = Convert.ToInt32(ddlBedRow.SelectedValue)
                });
            }

            if (selections.Count == 0 && errors.Count == 0)
            {
                lblMessage.Text = "Please check at least one trainee to allot.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            if (errors.Count > 0)
            {
                lblMessage.Text = string.Join("<br/>", errors);
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                // All-or-nothing: if ANY selected bed was taken by someone
                // else between the dropdown loading and clicking Save
                // (two admins allotting at the same time), the whole
                // batch rolls back rather than leaving a half-done mess.
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    foreach (AllotmentSelection sel in selections)
                    {
                        SqlCommand checkBedCmd = new SqlCommand(
                            "SELECT Status FROM HostelBedMaster WHERE ID = @BedID",
                            con, tran);
                        checkBedCmd.Parameters.AddWithValue("@BedID", sel.BedID);

                        string bedStatus = checkBedCmd.ExecuteScalar().ToString();

                        if (bedStatus != "Vacant")
                        {
                            throw new Exception(
                                "The bed selected for " + sel.EmpName +
                                " was just taken. Please refresh and try again.");
                        }

                        string allotmentID = GenerateAllotmentID(con, tran);

                        SqlCommand insertCmd = new SqlCommand(@"INSERT INTO HostelAllotment(AllotmentID, TrainingID, EmpID, BedID, AllotmentDate, Status, CreatedOn, CreatedBy)
                                                                VALUES (@AllotmentID, @TrainingID, @EmpID, @BedID, GETDATE(), 'Allotted', GETDATE(), @CreatedBy)",
                            con, tran);

                        insertCmd.Parameters.AddWithValue("@AllotmentID", allotmentID);
                        insertCmd.Parameters.AddWithValue("@TrainingID", ddlTraining.SelectedValue);
                        insertCmd.Parameters.AddWithValue("@EmpID", sel.EmpID);
                        insertCmd.Parameters.AddWithValue("@BedID", sel.BedID);
                        insertCmd.Parameters.AddWithValue("@CreatedBy", "Admin");

                        insertCmd.ExecuteNonQuery();

                        SqlCommand updateBedCmd = new SqlCommand(
                            "UPDATE HostelBedMaster SET Status = 'Occupied' WHERE ID = @BedID",
                            con, tran);
                        updateBedCmd.Parameters.AddWithValue("@BedID", sel.BedID);
                        updateBedCmd.ExecuteNonQuery();
                    }

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();

                    lblMessage.Text = ex.Message;
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    return;
                }
            }

            lblMessage.Text = selections.Count + " trainee(s) allotted successfully.";
            lblMessage.ForeColor = System.Drawing.Color.Green;

            BindParticipantGrid(ddlTraining.SelectedValue);
            HostelStatusDashboard1.BindHostelStatus();
        }

        private string GenerateAllotmentID(SqlConnection con, SqlTransaction tran)
        {
            string query = "SELECT ISNULL(MAX(ID),0)+1 FROM HostelAllotment";

            SqlCommand cmd = new SqlCommand(query, con, tran);
            int nextID = Convert.ToInt32(cmd.ExecuteScalar());

            return "ALT" + nextID.ToString("0000");
        }

        protected void btnManualAllot_Click(object sender, EventArgs e)
        {
            mvAllotment.ActiveViewIndex = 0;
        }

        protected void btnBulkAllot_Click(object sender, EventArgs e)
        {
            mvAllotment.ActiveViewIndex = 1;
            UpdateBulkTrainingLabel();
        }

        protected void btnRunAutoAllot_Click(object sender, EventArgs e)
        {
            lblAutoMessage.Text = "";

            if (ddlTraining.SelectedValue == "")
            {
                lblAutoMessage.Text = "Please select a Training in the Manual Allotment tab first.";
                lblAutoMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            if (ddlAutoHostel.SelectedValue == "")
            {
                lblAutoMessage.Text = "Please select a Hostel.";
                lblAutoMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            String traininID = ddlTraining.SelectedValue;
            int hostelID = Convert.ToInt32(ddlAutoHostel.SelectedValue);

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                String query = @"SELECT E.EmpID, E.EmpName FROM TrainingAssignment TA INNER JOIN EmpBasicMaster E ON TA.EmpID = E.EmpID
                         WHERE TA.TrainingID = @TrainingID AND NOT EXISTS (SELECT 1 FROM HostelAllotment HA  WHERE 
                         HA.TrainingID = TA.TrainingID AND HA.EmpID = TA.EmpID AND HA.Status = 'Allotted')";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@TrainingID", traininID);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    lblAutoMessage.Text = "Every Employee in this training already got Hostel Bed Assigned";
                    lblAutoMessage.ForeColor = System.Drawing.Color.Green;
                    return;
                }

                String q = @"SELECT HBed.ID, HBed.BedID, H.HostelName, HB.BlockName, HR.RoomNo, HBed.BedNo, HBed.Status,
                         HBed.CreatedOn FROM HostelBedMaster HBed INNER JOIN HostelRoomMaster HR ON HBed.RoomID = HR.ID
                         INNER JOIN HostelBlockMaster HB ON HR.BlockID = HB.ID
                         INNER JOIN HostelMaster H ON HB.HostelID = H.ID WHERE HB.HostelID = @HostelID AND HBed.Status='Vacant' 
                         ORDER BY HB.BlockName,  HR.RoomNo, HBed.BedNo ";
                SqlCommand command = new SqlCommand(q, con);
                command.Parameters.AddWithValue("@HostelID", hostelID);

                SqlDataAdapter adap = new SqlDataAdapter(command);
                DataTable vacantBeds = new DataTable();
                adap.Fill(vacantBeds);

                if (vacantBeds.Rows.Count == 0)
                {
                    lblAutoMessage.Text = "No vacant beds available in this hostel";
                    lblAutoMessage.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                int allotCount = Math.Min(dt.Rows.Count, vacantBeds.Rows.Count);
                SqlTransaction tran = con.BeginTransaction();
                for (int i = 0; i < allotCount; i++)
                {
                    String empID = dt.Rows[i]["EmpID"].ToString();
                    String employeeName = dt.Rows[i]["EmpName"].ToString();
                    int bedID = Convert.ToInt32(vacantBeds.Rows[i]["ID"]);

                    String Query = @"INSERT INTO HostelAllotment(AllotmentID, TrainingID, EmpID, BedID, AllotmentDate, Status, CreatedOn, CreatedBy) 
                            VALUES(@AllotmentID, @TrainingID, @EmpID, @BedID, GETDATE(), @Status, GETDATE(), @CreatedBy)";
                    SqlCommand sqlCmd = new SqlCommand(Query, con, tran);
                    sqlCmd.Parameters.AddWithValue("@AllotmentID", GenerateAllotmentID(con, tran));
                    sqlCmd.Parameters.AddWithValue("@TrainingID", traininID);
                    sqlCmd.Parameters.AddWithValue("@EmpID", empID);
                    sqlCmd.Parameters.AddWithValue("@BedID", bedID);
                    sqlCmd.Parameters.AddWithValue("@Status", "Allotted");
                    sqlCmd.Parameters.AddWithValue("@CreatedBy", "Admin");
                    sqlCmd.ExecuteNonQuery();

                    SqlCommand updateBedCmd = new SqlCommand(
                            "UPDATE HostelBedMaster SET Status = 'Occupied' WHERE ID = @BedID",
                            con, tran);
                    updateBedCmd.Parameters.AddWithValue("@BedID", bedID);
                    updateBedCmd.ExecuteNonQuery();

                }
                tran.Commit();

                lblAutSuccessMessage.Text = allotCount + " trainee(s) allotted successfully.<br/>";
                lblAutSuccessMessage.ForeColor = System.Drawing.Color.Green;
                HostelStatusDashboard1.BindHostelStatus();

                for (int i = allotCount; i < dt.Rows.Count; i++)
                {
                    String empID = dt.Rows[i]["EmpID"].ToString();
                    String employeeName = dt.Rows[i]["EmpName"].ToString();

                    lblAutoMessage.Text += "Bed Not allotted to   " + employeeName + " (" + empID + ") <br/>";
                    lblAutoMessage.ForeColor = System.Drawing.Color.Red;
                }
            }

        }

    protected void btnViewAllot_Click(object sender, EventArgs e)
    {
        mvAllotment.ActiveViewIndex = 2;
        UpdateSummaryTrainingLabel();
    }

    protected void btnLoadSummary_Click(object sender, EventArgs e)
    {
        if (ddlTraining.SelectedValue == "")
        {
            lblMessageOnSummary.Text = "Please select Training First";
            lblMessageOnSummary.ForeColor = System.Drawing.Color.Red;
            HostelStatusDashboard1.BindHostelStatus();
            }
        else
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
                cmd.Parameters.AddWithValue("@TrainingID", ddlTraining.SelectedValue);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                gvSummary.DataSource = dt;
                gvSummary.DataBind();
            }
        }

    }

    protected void gvSummary_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                object hostelValue = DataBinder.Eval(e.Row.DataItem, "HostelName");

                if (hostelValue == DBNull.Value)
                {
                    Label lblhostelName = (Label)e.Row.FindControl("lblhostelName");
                    lblhostelName.Text = "No Bed Allotted";
                    lblhostelName.ForeColor = System.Drawing.Color.Red;
                    LinkButton btnEditSummary = (LinkButton)e.Row.FindControl("btnEditSummary");
                    btnEditSummary.Visible = false;
                }
                else
                {
                    LinkButton btnEditSummary = (LinkButton)e.Row.FindControl("btnEditSummary");
                    btnEditSummary.Visible = true;
                }

            }
        }

        protected void gvSummary_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
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

                    lblMessageOnSummary.Text = "Allotment vacated successfully.";
                    lblMessageOnSummary.ForeColor = System.Drawing.Color.Green;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    lblMessageOnSummary.Text = "Could not vacate: " + ex.Message;
                    lblMessageOnSummary.ForeColor = System.Drawing.Color.Red;
                    return;
                }
            }

            
            btnLoadSummary_Click(this, EventArgs.Empty);
        }

    }
}
