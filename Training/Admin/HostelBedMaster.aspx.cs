using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace Training.Admin
{
    public partial class HostelBedMaster : System.Web.UI.Page
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
                ResetRoomDropdown();
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
            ResetRoomDropdown();

            if (ddlHostel.SelectedValue == "")
            {
                ResetBlockDropdown();
                return;
            }

            BindBlockDropdown(Convert.ToInt32(ddlHostel.SelectedValue));
        }

        protected void ddlBlock_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlBlock.SelectedValue == "")
            {
                ResetRoomDropdown();
                return;
            }

            BindRoomDropdown(Convert.ToInt32(ddlBlock.SelectedValue));
        }

        private void BindBlockDropdown(int hostelID)
        {
            string query = @"SELECT ID, BlockName FROM HostelBlockMaster WHERE HostelID = @HostelID AND IsActive = 'Y'
                             ORDER BY BlockName";

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

        private void BindRoomDropdown(int blockID)
        {
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

                ddlRoom.DataSource = dt;
                ddlRoom.DataTextField = "RoomDisplay";
                ddlRoom.DataValueField = "ID";
                ddlRoom.DataBind();

                ddlRoom.Items.Insert(0, new ListItem("-- Select Room --", ""));
            }
        }

        private void ResetBlockDropdown()
        {
            ddlBlock.Items.Clear();
            ddlBlock.Items.Insert(0, new ListItem("-- Select Hostel First --", ""));
        }

        private void ResetRoomDropdown()
        {
            ddlRoom.Items.Clear();
            ddlRoom.Items.Insert(0, new ListItem("-- Select Block First --", ""));
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {

            if (hfBedID.Value == "")
            {
                lblBedMessage.Text = "";

                if (ddlRoom.SelectedValue == "")
                {
                    lblBedMessage.Text = "Please select a Room.";
                    lblBedMessage.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                int roomID = Convert.ToInt32(ddlRoom.SelectedValue);
                string bedNo = ddlBedNo.SelectedValue;


                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    string capacityQuery =
                        "SELECT Capacity FROM HostelRoomMaster WHERE ID = @RoomID";

                    SqlCommand capacityCmd = new SqlCommand(capacityQuery, con);
                    capacityCmd.Parameters.AddWithValue("@RoomID", roomID);
                    int capacity = Convert.ToInt32(capacityCmd.ExecuteScalar());

                    string bedCountQuery =
                        "SELECT COUNT(*) FROM HostelBedMaster WHERE RoomID = @RoomID";

                    SqlCommand bedCountCmd = new SqlCommand(bedCountQuery, con);
                    bedCountCmd.Parameters.AddWithValue("@RoomID", roomID);
                    int existingBeds = Convert.ToInt32(bedCountCmd.ExecuteScalar());

                    if (existingBeds >= capacity)
                    {
                        lblBedMessage.Text =
                            "This room has reached its bed capacity (" + capacity + ").";
                        lblBedMessage.ForeColor = System.Drawing.Color.Red;
                        return;
                    }

                    // BedNo unique within a room only.
                    string checkQuery = @"SELECT COUNT(*) FROM HostelBedMaster WHERE RoomID = @RoomID AND UPPER(BedNo) = UPPER(@BedNo)";

                    SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                    checkCmd.Parameters.AddWithValue("@RoomID", roomID);
                    checkCmd.Parameters.AddWithValue("@BedNo", bedNo);

                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (count > 0)
                    {
                        lblBedMessage.Text =
                            "This bed number already exists in the selected room.";
                        lblBedMessage.ForeColor = System.Drawing.Color.Red;
                        return;
                    }

                    string bedID = GenerateBedID(con);

                    string insertQuery = @"INSERT INTO HostelBedMaster (BedID, RoomID, BedNo, Status, IsActive, CreatedOn, CreatedBy)
                                           VALUES (@BedID, @RoomID, @BedNo, 'Vacant', @IsActive, GETDATE(), @CreatedBy)";

                    SqlCommand cmd = new SqlCommand(insertQuery, con);
                    cmd.Parameters.AddWithValue("@BedID", bedID);
                    cmd.Parameters.AddWithValue("@RoomID", roomID);
                    cmd.Parameters.AddWithValue("@BedNo", bedNo);
                    cmd.Parameters.AddWithValue("@IsActive", ddlIsActive.SelectedValue);
                    cmd.Parameters.AddWithValue("@CreatedBy", "Admin");

                    cmd.ExecuteNonQuery();
                }

                lblBedMessage.Text = "Bed saved successfully.";
                lblBedMessage.ForeColor = System.Drawing.Color.Green;

                ClearForm();
                BindGrid();
            }
            else
            {
                lblBedMessage.Text = "";

                if (ddlRoom.SelectedValue == "")
                {
                    lblBedMessage.Text = "Please select a Room.";
                    lblBedMessage.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                if (ddlBedNo.SelectedValue == "")
                {
                    lblBedMessage.Text = "Please select a Bed No.";
                    lblBedMessage.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                string bedIDBeingEdited = hfBedID.Value;
                int roomIDEdit = Convert.ToInt32(ddlRoom.SelectedValue);
                string bedNoEdit = ddlBedNo.SelectedValue;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                   
                    string checkQuery = @"SELECT COUNT(*) FROM HostelBedMaster WHERE RoomID = @RoomID AND UPPER(BedNo) = UPPER(@BedNo) AND ID <> @CurrentBedID";

                    SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                    checkCmd.Parameters.AddWithValue("@RoomID", roomIDEdit);
                    checkCmd.Parameters.AddWithValue("@BedNo", bedNoEdit);
                    checkCmd.Parameters.AddWithValue("@CurrentBedID", bedIDBeingEdited);

                    int countExisting = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (countExisting > 0)
                    {
                        lblBedMessage.Text = "This bed number already exists in the selected room.";
                        lblBedMessage.ForeColor = System.Drawing.Color.Red;
                        return;
                    }

                   
                    string queryUpdate = @"UPDATE HostelBedMaster SET RoomID = @RoomIDEdit, BedNo = @BedNoEdit, IsActive = @IsActiveEdit WHERE ID = @CurrentBedID";

                    SqlCommand updateCmd = new SqlCommand(queryUpdate, con);
                    updateCmd.Parameters.AddWithValue("@RoomIDEdit", roomIDEdit);
                    updateCmd.Parameters.AddWithValue("@BedNoEdit", bedNoEdit);
                    updateCmd.Parameters.AddWithValue("@IsActiveEdit", ddlIsActive.SelectedValue);
                    updateCmd.Parameters.AddWithValue("@CurrentBedID", bedIDBeingEdited);

                    updateCmd.ExecuteNonQuery();
                }

                lblBedMessage.Text = "Bed updated successfully.";
                lblBedMessage.ForeColor = System.Drawing.Color.Green;

                hfBedID.Value = "";
                ClearForm();
                BindGrid();
            }



        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            lblBedMessage.Text = "";
            ClearForm();
        }

        private void ClearForm()
        {
            ddlHostel.SelectedIndex = 0;
            ResetBlockDropdown();
            ResetRoomDropdown();
            ddlBedNo.Items.Clear();
            ddlIsActive.SelectedValue = "Y";
        }

        private string GenerateBedID(SqlConnection con)
        {
            string query = "SELECT ISNULL(MAX(ID),0)+1 FROM HostelBedMaster";

            SqlCommand cmd = new SqlCommand(query, con);
            int nextID = Convert.ToInt32(cmd.ExecuteScalar());

            return "BED" + nextID.ToString("0000");
        }

        

        protected void ddlRoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlRoom.SelectedValue == "")
                return;
            else
              PopulateBedDropdown(Convert.ToInt32(ddlRoom.SelectedValue));        
        }

        private void PopulateBedDropdown(int roomID)
        {
            int capacity = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                string query = "SELECT Capacity FROM HostelRoomMaster WHERE ID = @RoomID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@RoomID", roomID);
                capacity = Convert.ToInt32(cmd.ExecuteScalar());

                String existingBedQuery = @"Select BedNo from HostelBedMaster where RoomID = @RoomID";
                SqlCommand cmd1 = new SqlCommand(existingBedQuery, con);
                cmd1.Parameters.AddWithValue("@RoomID", roomID);

                SqlDataAdapter adapter1 = new SqlDataAdapter(cmd1);
                DataTable existingBed = new DataTable();
                adapter1.Fill(existingBed);

                ddlBedNo.Items.Clear();

                for (int i = 1; i <= capacity; i++)
                {
                    String candidiate = "Bed-" + i;
                    DataRow[] matches = existingBed.Select("BedNo = '" + candidiate + "'");
                    if (matches.Length == 0)
                    {
                        ddlBedNo.Items.Add(new ListItem(candidiate, candidiate));
                    }
                }

                if (ddlBedNo.Items.Count == 0)
                {
                    ddlBedNo.Items.Insert(0, new ListItem("-- Room Full, No Beds Available --", ""));
                }
                else
                {
                    ddlBedNo.Items.Insert(0, new ListItem("-- Select Bed --", ""));
                }
            }
        }

        private void PopulateBedDropdown(int roomID, int excludeBedID)
        {
            int capacity = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                string query = "SELECT Capacity FROM HostelRoomMaster WHERE ID = @RoomID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@RoomID", roomID);
                capacity = Convert.ToInt32(cmd.ExecuteScalar());

                String existingBedQuery = @"Select BedNo from HostelBedMaster where RoomID = @RoomID AND ID <> @ExcludeBedID";
                SqlCommand cmd1 = new SqlCommand(existingBedQuery, con);
                cmd1.Parameters.AddWithValue("@RoomID", roomID);
                cmd1.Parameters.AddWithValue("@ExcludeBedID", excludeBedID);

                SqlDataAdapter adapter1 = new SqlDataAdapter(cmd1);
                DataTable existingBed = new DataTable();
                adapter1.Fill(existingBed);

                ddlBedNo.Items.Clear();

                for (int i = 1; i <= capacity; i++)
                {
                    String candidiate = "Bed-" + i;
                    DataRow[] matches = existingBed.Select("BedNo = '" + candidiate + "'");
                    if (matches.Length == 0)
                    {
                        ddlBedNo.Items.Add(new ListItem(candidiate, candidiate));
                    }
                }

                if (ddlBedNo.Items.Count == 0)
                {
                    ddlBedNo.Items.Insert(0, new ListItem("-- Room Full, No Beds Available --", ""));
                }
                else
                {
                    ddlBedNo.Items.Insert(0, new ListItem("-- Select Bed --", ""));
                }
            }
        }

        private void BindGrid()
        {
            string query = @"SELECT HBed.ID, HBed.BedID, H.HostelName, HB.BlockName, HF.FlatName, HR.RoomNo, HBed.BedNo, HBed.Status, 
                             CASE HBed.IsActive WHEN 'Y' THEN 'Active' ELSE 'Inactive' END AS IsActive, HBed.CreatedOn FROM HostelBedMaster HBed
                             INNER JOIN HostelRoomMaster HR ON HBed.RoomID = HR.ID
                             INNER JOIN HostelBlockMaster HB ON HR.BlockID = HB.ID
                             INNER JOIN HostelMaster H ON HB.HostelID = H.ID
                             LEFT JOIN HostelFlatMaster HF ON HR.FlatID = HF.ID
                             ORDER BY H.HostelName ASC";

            gvBed.DataSource = obj.GetDataTable(query);
            gvBed.DataBind();
        }

        protected void gvBed_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteBed")
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(constr))
                    {
                        conn.Open();
                        String id = e.CommandArgument.ToString();
                        String query = @"DELETE FROM HostelBedMaster where ID = @ID";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.ExecuteNonQuery();
                        lblBedMessage.Text = "Bed Successfully Deleted";
                        lblBedMessage.ForeColor = System.Drawing.Color.Green;

                    }
                    BindGrid();
                     
                }
                catch (Exception)
                {
                    using (SqlConnection conn = new SqlConnection(constr))
                    {
                        conn.Open();
                        String id = e.CommandArgument.ToString();
                        String query = @"UPDATE HostelBedMaster SET IsActive='N' where ID = @ID";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.ExecuteNonQuery();
                    }

                    lblBedMessage.Text = "This bed has allotment history and can't be permanently deleted, so it has been marked Inactive instead.";
                    lblBedMessage.ForeColor = System.Drawing.Color.Green;

                    BindGrid();
                }
            }
            else if (e.CommandName == "EditBed")
            {
                string id = e.CommandArgument.ToString();

                using (SqlConnection conn = new SqlConnection(constr))
                {
                    conn.Open();

                    string query = @"SELECT HB.HostelID, HR.ID AS RoomPK, HR.BlockID, HBed.BedNo, HBed.IsActive FROM HostelBedMaster HBed
                                     INNER JOIN HostelRoomMaster HR ON HBed.RoomID = HR.ID 
                                     INNER JOIN HostelBlockMaster HB ON HR.BlockID = HB.ID
                                     WHERE HBed.ID = @ID";


                    SqlCommand cmd = new SqlCommand(query, conn);
                     
                    cmd.Parameters.AddWithValue("@ID", id);
                    

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        int hostelID = Convert.ToInt32(dr["HostelID"]);
                        int roomID = Convert.ToInt32(dr["RoomPK"]);
                        int blockID = Convert.ToInt32(dr["BlockID"]);
                        string bedNo = dr["BedNo"].ToString();
                        string isActive = dr["IsActive"].ToString();

                        dr.Close();

                      
                        ddlHostel.SelectedValue = hostelID.ToString();
                        BindBlockDropdown(hostelID);
                        ddlBlock.SelectedValue = blockID.ToString();
                        BindRoomDropdown(blockID);
                        ddlRoom.SelectedValue = roomID.ToString();

                      
                        PopulateBedDropdown(roomID, Convert.ToInt32(id));
                        ddlBedNo.SelectedValue = bedNo;
                        ddlIsActive.SelectedValue = isActive;

                        hfBedID.Value = id;
                    }
                }
            }
        }
    }
}
