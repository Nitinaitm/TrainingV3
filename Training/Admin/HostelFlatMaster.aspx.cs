using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace Training.Admin
{
    public partial class HostelFlatMaster : System.Web.UI.Page
    {

        clsDataAccess obj = new clsDataAccess();

        String constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

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

                BindHostelDropdown();
                ResetBlockDropDown();
                BindGrid();
            }
        }

        private void BindHostelDropdown()
        {
            string query =
                "SELECT ID, HostelName FROM HostelMaster WHERE IsActive='Y' ORDER BY HostelName";

            DataTable dt = obj.GetDataTable(query);

            ddlHostel.DataSource = dt;
            ddlHostel.DataTextField = "HostelName";
            ddlHostel.DataValueField = "ID";
            ddlHostel.DataBind();

            ddlHostel.Items.Insert(0, new ListItem("-- Select Hostel --", ""));
        }

        protected void ddlHostel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlHostel.SelectedValue == "")
            {
                ResetBlockDropDown();
                return;
            }

            BindBlockDropDown(Convert.ToInt32(ddlHostel.SelectedValue));
        }

        private void BindBlockDropDown(int hostelID)
        {
            string query = @"SELECT ID, BlockName FROM HostelBlockMaster WHERE HostelID = @HostelID AND IsActive = 'Y' ORDER BY BlockName";

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@HostelID", hostelID);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlBlock.DataSource = dt;
                ddlBlock.DataTextField = "BlockName";
                ddlBlock.DataValueField = "ID";
                ddlBlock.DataBind();

                ddlBlock.Items.Insert(0, new ListItem("-- Select Block --", ""));
            }
        }

        private void ResetBlockDropDown()
        {
            ddlBlock.Items.Clear();
            ddlBlock.Items.Insert(0, new ListItem("-- Select Hostel First --", ""));
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {

            if (hfFlatID.Value == "")
            {
                lblFlatMessage.Text = "";

                if (ddlBlock.SelectedValue == "")
                {
                    lblFlatMessage.Text = "Please Select Block";
                    lblFlatMessage.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                int blockID = Convert.ToInt32(ddlBlock.SelectedValue);
                string flatName = txtFlatName.Text.Trim();
                string isActive = ddlIsActive.SelectedValue;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    // Flat Name unique within a Block only -- same reasoning
                    // as Block Name being unique within a Hostel.
                    string checkQuery = @"SELECT COUNT(*) FROM HostelFlatMaster WHERE BlockID = @BlockID AND UPPER(FlatName) = UPPER(@FlatName)";

                    SqlCommand cmd = new SqlCommand(checkQuery, con);
                    cmd.Parameters.AddWithValue("@BlockID", blockID);
                    cmd.Parameters.AddWithValue("@FlatName", flatName);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    if (count > 0)
                    {
                        lblFlatMessage.Text = "This Flat already exists for the selected Block.";
                        lblFlatMessage.ForeColor = System.Drawing.Color.Red;
                        return;
                    }

                    string flatID = GenerateFlatID(con);

                    string insertQuery = @"INSERT INTO HostelFlatMaster (FlatID, BlockID, FlatName, IsActive, CreatedOn, CreatedBy)
                                        VALUES (@FlatID, @BlockID, @FlatName, @IsActive, GETDATE(), @CreatedBy)";

                    // IMPORTANT: this is a fresh SqlCommand, separate from
                    // "cmd" above (which was used for the duplicate check and
                    // already has its own parameters attached). Reusing "cmd"
                    // here would throw an error and never actually run the
                    // insert -- that was the bug in the previous version.
                    SqlCommand cmdInsert = new SqlCommand(insertQuery, con);
                    cmdInsert.Parameters.AddWithValue("@FlatID", flatID);
                    cmdInsert.Parameters.AddWithValue("@BlockID", blockID);
                    cmdInsert.Parameters.AddWithValue("@FlatName", flatName);
                    cmdInsert.Parameters.AddWithValue("@IsActive", isActive);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", "Admin");

                    cmdInsert.ExecuteNonQuery();
                }
                lblFlatMessage.Text = "Flat saved successfully.";
                lblFlatMessage.ForeColor = System.Drawing.Color.Green;

                ClearForm();
                BindGrid();
            }
            else
            {
                lblFlatMessage.Text = "";

                if (ddlBlock.SelectedValue == "")
                {
                    lblFlatMessage.Text = "Please select a Block.";
                    lblFlatMessage.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                string flatIDBeingEdited = hfFlatID.Value;
                int blockIDEdit = Convert.ToInt32(ddlBlock.SelectedValue);
                string flatNameEdit = txtFlatName.Text.Trim();
                string isActiveEdit = ddlIsActive.SelectedValue;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    // Scoped to the selected block, excluding the row currently
                    // being edited -- same reasoning as Block's own edit check.
                    string checkQuery = @"SELECT COUNT(*) FROM HostelFlatMaster WHERE BlockID = @BlockID AND UPPER(FlatName) = UPPER(@FlatName) AND ID <> @CurrentFlatID";

                    SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                    checkCmd.Parameters.AddWithValue("@BlockID", blockIDEdit);
                    checkCmd.Parameters.AddWithValue("@FlatName", flatNameEdit);
                    checkCmd.Parameters.AddWithValue("@CurrentFlatID", flatIDBeingEdited);

                    int countExisting = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (countExisting > 0)
                    {
                        lblFlatMessage.Text = "This flat already exists for the selected block.";
                        lblFlatMessage.ForeColor = System.Drawing.Color.Red;
                        return;
                    }

                    string queryUpdate = @"UPDATE HostelFlatMaster SET BlockID = @BlockIDEdit,     FlatName = @FlatNameEdit,     IsActive = @IsActiveEdit WHERE ID = @CurrentFlatID";

                    SqlCommand updateCmd = new SqlCommand(queryUpdate, con);
                    updateCmd.Parameters.AddWithValue("@BlockIDEdit", blockIDEdit);
                    updateCmd.Parameters.AddWithValue("@FlatNameEdit", flatNameEdit);
                    updateCmd.Parameters.AddWithValue("@IsActiveEdit", isActiveEdit);
                    updateCmd.Parameters.AddWithValue("@CurrentFlatID", flatIDBeingEdited);

                    updateCmd.ExecuteNonQuery();
                }

                lblFlatMessage.Text = "Flat updated successfully.";
                lblFlatMessage.ForeColor = System.Drawing.Color.Green;

                hfFlatID.Value = "";
                ClearForm();
                BindGrid();
            }
             
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            lblFlatMessage.Text = "";
            ClearForm();
        }

        private void ClearForm()
        {
            ddlHostel.SelectedIndex = 0;
            ResetBlockDropDown();
            txtFlatName.Text = "";
            ddlIsActive.SelectedValue = "Y";
        }

        private string GenerateFlatID(SqlConnection con)
        {
            string query = "SELECT ISNULL(MAX(ID),0)+1 FROM HostelFlatMaster";

            SqlCommand cmd = new SqlCommand(query, con);
            int nextID = Convert.ToInt32(cmd.ExecuteScalar());

            return "FLT" + nextID.ToString("0000");
        }

        private void BindGrid()
        {
            // Note: joins on HF.BlockID = HB.ID (the real foreign key),
            // not HF.FlatID = HB.ID -- FlatID is the Flat's OWN business
            // code, unrelated to the Block's internal ID. Also cases on
            // HF.IsActive (the Flat's own status), not HB.IsActive.
            string query = @"SELECT HF.ID, HF.FlatID, H.HostelName, HB.BlockName, HF.FlatName, CASE HF.IsActive     WHEN 'Y' THEN 'Active'
                             ELSE 'Inactive' END AS IsActive, HF.CreatedOn FROM HostelFlatMaster HF INNER JOIN HostelBlockMaster HB ON HF.BlockID = HB.ID
                             INNER JOIN HostelMaster H ON HB.HostelID = H.ID ORDER BY HF.ID DESC";

            gvFlat.DataSource = obj.GetDataTable(query);
            gvFlat.DataBind();
        }
        protected void gvFlat_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteFlat")
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(constr))
                    {
                        conn.Open();
                        String id = e.CommandArgument.ToString();
                        String query = @"DELETE FROM HostelFlatMaster where ID = @ID";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.ExecuteNonQuery();
                        lblFlatMessage.Text = "Flat Deleted Successfully";
                    }
                    BindGrid();
                }
                catch (Exception)
                {
                    lblFlatMessage.Text = "Cannot delete: this Flat still has Rooms defined under it. First delete Rooms under this Flat";

                    lblFlatMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
            else if (e.CommandName == "EditFlat")
            {
                string id = e.CommandArgument.ToString();

                using (SqlConnection conn = new SqlConnection(constr))
                {
                    conn.Open();

                    string query = @"SELECT HB.HostelID, HF.BlockID, HF.FlatName, HF.IsActive FROM HostelFlatMaster HF
                                     INNER JOIN HostelBlockMaster HB ON HF.BlockID = HB.ID WHERE HF.ID = @ID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", id);

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        int hostelID = Convert.ToInt32(dr["HostelID"]);
                        int blockID = Convert.ToInt32(dr["BlockID"]);
                        string flatName = dr["FlatName"].ToString();
                        string isActive = dr["IsActive"].ToString();

                        dr.Close();

                        // Cascade has to be rebuilt in order, same as a real user
                        // would trigger it -- Hostel first, THEN populate Block for
                        // that hostel, THEN select the actual block.
                        ddlHostel.SelectedValue = hostelID.ToString();
                        BindBlockDropDown(hostelID);
                        ddlBlock.SelectedValue = blockID.ToString();

                        txtFlatName.Text = flatName;
                        ddlIsActive.SelectedValue = isActive;
                        hfFlatID.Value = id;
                    }
                }
            }
        }
    }
}