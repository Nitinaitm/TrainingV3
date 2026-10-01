using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;


namespace Training.Admin
{
    public partial class HostelRoomMaster : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        string constr =
            ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

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
                ResetBlockDropdown();
                ResetFlatDropdown();
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

        // Fires automatically whenever the user picks a different Hostel
        // (AutoPostBack="true" on ddlHostel in the markup).
        protected void ddlHostel_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetFlatDropdown();
            if (ddlHostel.SelectedValue == "")
            {
                ResetBlockDropdown();
                return;
            }

            int hostelID = Convert.ToInt32(ddlHostel.SelectedValue);
            BindBlockDropdown(hostelID);
        }

        protected void ddlBlock_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlBlock.SelectedValue=="")
            {
                ResetFlatDropdown();
                return;
            }
            BindFlatDropDown(Convert.ToInt32(ddlBlock.SelectedValue));
        }



        // there's no HasFlats column anywhere. Here We simply query whether any HostelFlatMaster rows exist
        //for this Block or not. If yes which means if any row exists, the dropdown is populated and usable 
        //(still optional). If no means no row exists, then it will be disabled with an explanatory placeholder, and nothing
        // else in the app needs to know or care why.

        private void BindFlatDropDown(int blockID)
        {
            String query = @"Select ID, FlatName from HostelFlatMaster where BlockID = @BlockID and IsActive = 'Y' order by FlatName " ;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@BlockID", blockID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count ==0)
                {
                    ddlFlat.Items.Clear();
                    ddlFlat.Items.Insert(0, new ListItem("--No Flat For This Block-",""));
                    ddlFlat.Enabled = false;
                    return;
                }

                ddlFlat.Enabled = true;
                ddlFlat.DataSource = dt;
                ddlFlat.DataTextField = "FlatName";
                ddlFlat.DataValueField = "ID";
                ddlFlat.DataBind();
                // Even when flats exist, leaving one unselected is valid
                // -- Flat is optional, not required, even for blocks
                // that have some defined.
                ddlFlat.Items.Insert(0, new ListItem("-- No Specific Flat --", ""));
            }
        }


        private void ResetFlatDropdown()
        {
            ddlFlat.Items.Clear();
            ddlFlat.Items.Insert(0, new ListItem("-- Select Block First --", ""));
            ddlFlat.Enabled = false;
        }

        private void BindBlockDropdown(int hostelID)
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

        private void ResetBlockDropdown()
        {
            ddlBlock.Items.Clear();
            ddlBlock.Items.Insert(0, new ListItem("-- Select Hostel First --", ""));
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (hfRoomID.Value == "")
            {
                lblRoomMessage.Text = "";

                if (ddlBlock.SelectedValue == "")
                {
                    lblRoomMessage.Text = "Please select a Block.";
                    lblRoomMessage.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                int blockID = Convert.ToInt32(ddlBlock.SelectedValue);
                string roomNo = txtRoomNo.Text.Trim();
                int floorNo = Convert.ToInt32(txtFloorNo.Text.Trim());
                string roomType = ddlRoomType.SelectedValue;
                int capacity = Convert.ToInt32(txtCapacity.Text.Trim());
                string isActive = ddlIsActive.SelectedValue;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    // RoomNo unique within a block only.
                    string checkQuery = @"SELECT COUNT(*) FROM HostelRoomMaster WHERE BlockID = @BlockID AND UPPER(RoomNo) = UPPER(@RoomNo)";

                    SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                    checkCmd.Parameters.AddWithValue("@BlockID", blockID);
                    checkCmd.Parameters.AddWithValue("@RoomNo", roomNo);

                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (count > 0)
                    {
                        lblRoomMessage.Text =
                            "This room number already exists in the selected block.";
                        lblRoomMessage.ForeColor = System.Drawing.Color.Red;
                        return;
                    }

                    string roomID = GenerateRoomID(con);

                    string insertQuery = @" INSERT INTO HostelRoomMaster (RoomID, BlockID, FlatID, RoomNo, FloorNo, RoomType, Capacity, IsActive, CreatedOn, CreatedBy)
                                        VALUES (@RoomID, @BlockID, @FlatID,  @RoomNo, @FloorNo, @RoomType, @Capacity, @IsActive, GETDATE(), @CreatedBy)";


                    SqlCommand cmd = new SqlCommand(insertQuery, con);
                    cmd.Parameters.AddWithValue("@RoomID", roomID);
                    cmd.Parameters.AddWithValue("@BlockID", blockID);

                    // FlatID is nullable -- if nothing was picked (either
                    // because the block has no flats, or the admin left it
                    // on "No Specific Flat"), we write DBNull.Value, which
                    // ADO.NET translates into a real database NULL. Writing
                    // an empty string here instead would fail, since the
                    // column is an int.
                    if (ddlFlat.SelectedValue == "")
                    {
                        cmd.Parameters.AddWithValue("@FlatID", DBNull.Value);
                    }
                    else
                        cmd.Parameters.AddWithValue("@FlatID", Convert.ToInt32(ddlFlat.SelectedValue));


                    cmd.Parameters.AddWithValue("@RoomNo", roomNo);
                    cmd.Parameters.AddWithValue("@FloorNo", floorNo);
                    cmd.Parameters.AddWithValue("@RoomType", roomType);
                    cmd.Parameters.AddWithValue("@Capacity", capacity);
                    cmd.Parameters.AddWithValue("@IsActive", isActive);
                    cmd.Parameters.AddWithValue("@CreatedBy", "Admin");

                    cmd.ExecuteNonQuery();
                }

                lblRoomMessage.Text = "Room saved successfully.";
                lblRoomMessage.ForeColor = System.Drawing.Color.Green;

                ClearForm();
                BindGrid();
            }
            else
            {
                lblRoomMessage.Text = "";

                if (ddlBlock.SelectedValue == "")
                {
                    lblRoomMessage.Text = "Please select a Block.";
                    lblRoomMessage.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                string roomIDBeingEdited = hfRoomID.Value;
                int blockIDEdit = Convert.ToInt32(ddlBlock.SelectedValue);
                string roomNoEdit = txtRoomNo.Text.Trim();
                int floorNoEdit = Convert.ToInt32(txtFloorNo.Text.Trim());
                string roomTypeEdit = ddlRoomType.SelectedValue;
                int capacityEdit = Convert.ToInt32(txtCapacity.Text.Trim());
                string isActiveEdit = ddlIsActive.SelectedValue;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    // Scoped to the selected block, excluding the row currently
                    // being edited -- same pattern as Block/Flat's own edit checks.
                    string checkQuery = @"SELECT COUNT(*) FROM HostelRoomMaster WHERE BlockID = @BlockID AND UPPER(RoomNo) = UPPER(@RoomNo) AND ID <> @CurrentRoomID";

                    SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                    checkCmd.Parameters.AddWithValue("@BlockID", blockIDEdit);
                    checkCmd.Parameters.AddWithValue("@RoomNo", roomNoEdit);
                    checkCmd.Parameters.AddWithValue("@CurrentRoomID", roomIDBeingEdited);

                    int countExisting = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (countExisting > 0)
                    {
                        lblRoomMessage.Text = "This room number already exists in the selected block.";
                        lblRoomMessage.ForeColor = System.Drawing.Color.Red;
                        return;
                    }

                    string queryUpdate = @"UPDATE HostelRoomMaster SET BlockID = @BlockIDEdit,     FlatID = @FlatIDEdit,     RoomNo = @RoomNoEdit,     FloorNo = @FloorNoEdit,
                                           RoomType = @RoomTypeEdit,     Capacity = @CapacityEdit,     IsActive = @IsActiveEdit WHERE ID = @CurrentRoomID";

                    SqlCommand updateCmd = new SqlCommand(queryUpdate, con);
                    updateCmd.Parameters.AddWithValue("@BlockIDEdit", blockIDEdit);

                    // Same DBNull.Value handling as the insert branch -- Flat is
                    // still optional during an edit, not suddenly required.
                    if (ddlFlat.SelectedValue == "")
                        updateCmd.Parameters.AddWithValue("@FlatIDEdit", DBNull.Value);
                    else
                        updateCmd.Parameters.AddWithValue("@FlatIDEdit", Convert.ToInt32(ddlFlat.SelectedValue));

                    updateCmd.Parameters.AddWithValue("@RoomNoEdit", roomNoEdit);
                    updateCmd.Parameters.AddWithValue("@FloorNoEdit", floorNoEdit);
                    updateCmd.Parameters.AddWithValue("@RoomTypeEdit", roomTypeEdit);
                    updateCmd.Parameters.AddWithValue("@CapacityEdit", capacityEdit);
                    updateCmd.Parameters.AddWithValue("@IsActiveEdit", isActiveEdit);
                    updateCmd.Parameters.AddWithValue("@CurrentRoomID", roomIDBeingEdited);

                    updateCmd.ExecuteNonQuery();
                }

                lblRoomMessage.Text = "Room updated successfully.";
                lblRoomMessage.ForeColor = System.Drawing.Color.Green;

                hfRoomID.Value = "";
                ClearForm();
                BindGrid();
            }

        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            lblRoomMessage.Text = "";
            ClearForm();
        }

        private void ClearForm()
        {
            ddlHostel.SelectedIndex = 0;
            ResetBlockDropdown();
            ResetFlatDropdown();
            txtRoomNo.Text = "";
            txtFloorNo.Text = "";

            ddlRoomType.SelectedIndex = 0;
            txtCapacity.Text = "";
            ddlIsActive.SelectedValue = "Y";
        }

        private string GenerateRoomID(SqlConnection con)
        {
            string query = "SELECT ISNULL(MAX(ID),0)+1 FROM HostelRoomMaster";

            SqlCommand cmd = new SqlCommand(query, con);
            int nextID = Convert.ToInt32(cmd.ExecuteScalar());

            return "ROM" + nextID.ToString("0000");
        }

        private void BindGrid()
        {
            string query = @"SELECT HR.ID,HR.RoomID, H.HostelName, HB.BlockName, HF.FlatName, HR.RoomNo, HR.FloorNo, HR.RoomType, HR.Capacity, 
                             CASE HR.IsActive WHEN 'Y' THEN 'Active' ELSE 'Inactive' END AS IsActive, HR.CreatedOn FROM HostelRoomMaster 
                             HR INNER JOIN HostelBlockMaster HB ON HR.BlockID = HB.ID INNER JOIN HostelMaster H ON HB.HostelID = H.ID 
                             INNER JOIN HostelFlatMaster HF on HR.FlatID = HF.ID ORDER BY HB.BlockName ASC";

            gvRoom.DataSource = obj.GetDataTable(query);
            gvRoom.DataBind();
        }

        protected void gvRoom_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRoom")
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(constr))
                    {
                        conn.Open();
                        String id = e.CommandArgument.ToString();
                        String query = @"DELETE FROM HostelRoomMaster where ID = @ID";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.ExecuteNonQuery();
                        lblRoomMessage.Text = "Room Successfully Deleted";
                        lblRoomMessage.ForeColor = System.Drawing.Color.Green;
                    }
                    BindGrid();
                }
                catch (Exception)
                {
                    lblRoomMessage.Text = "Cannot delete: this Room still has Beds defined under it. First delete Beds under this Room";

                    lblRoomMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
            else if (e.CommandName == "EditRoom")
            {
                string id = e.CommandArgument.ToString();

                using (SqlConnection conn = new SqlConnection(constr))
                {
                    conn.Open();

                    string query = @"SELECT HB.HostelID, HR.BlockID, HR.FlatID, HR.RoomNo, HR.FloorNo, HR.RoomType, HR.Capacity, HR.IsActive
                                     FROM HostelRoomMaster HR INNER JOIN HostelBlockMaster HB ON HR.BlockID = HB.ID WHERE HR.ID = @ID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", id);

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        int hostelID = Convert.ToInt32(dr["HostelID"]);
                        int blockID = Convert.ToInt32(dr["BlockID"]);
                        object flatIdValue = dr["FlatID"];   // could genuinely be DBNull
                        string roomNo = dr["RoomNo"].ToString();
                        string floorNo = dr["FloorNo"].ToString();
                        string roomType = dr["RoomType"].ToString();
                        string capacity = dr["Capacity"].ToString();
                        string isActive = dr["IsActive"].ToString();

                        dr.Close();

                        // Rebuild the cascade in the correct order, same reasoning
                        // as Flat's edit -- Hostel first, then populate+select Block,
                        // then populate+select Flat (only if this room actually has one).
                        ddlHostel.SelectedValue = hostelID.ToString();
                        BindBlockDropdown(hostelID);
                        ddlBlock.SelectedValue = blockID.ToString();

                        BindFlatDropDown(blockID);

                        if (flatIdValue != DBNull.Value)
                        {
                            ddlFlat.SelectedValue = Convert.ToInt32(flatIdValue).ToString();
                        }
                        // else: leave ddlFlat on whatever BindFlatDropdown set it to
                        // (either "No Specific Flat" or the disabled "No Flats" state)
                        // -- this room genuinely has no flat, nothing more to select.

                        txtRoomNo.Text = roomNo;
                        txtFloorNo.Text = floorNo;
                        ddlRoomType.SelectedValue = roomType;
                        txtCapacity.Text = capacity;
                        ddlIsActive.SelectedValue = isActive;

                        hfRoomID.Value = id;
                    }
                }
            }
        }
    }
}
