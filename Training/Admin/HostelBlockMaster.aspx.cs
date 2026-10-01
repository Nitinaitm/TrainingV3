using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class HostelBlockMaster : System.Web.UI.Page
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
            ddlHostel.DataValueField = "ID"; // internal numeric PK, not the HST0001 code
            ddlHostel.DataBind();

            // Blank first option so RequiredFieldValidator (InitialValue="")
            // can force the user to actually pick a hostel.
            ddlHostel.Items.Insert(0, new ListItem("-- Select Hostel --", ""));
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (hfBlockID.Value == "")
            {
                lblBlockMessage.Text = "";

                if (ddlHostel.SelectedValue == "")
                {
                    lblBlockMessage.Text = "Please select a Hostel.";
                    lblBlockMessage.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                int hostelID = Convert.ToInt32(ddlHostel.SelectedValue);
                string blockName = txtBlockName.Text.Trim();
                string isActive = ddlIsActive.SelectedValue;
                string hostelCategory = ddlHostelCategory.SelectedValue;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    // Duplicate check is scoped to the selected hostel --
                    // "Block A" can legally exist under two different hostels.
                    string checkQuery = @"SELECT COUNT(*) FROM HostelBlockMaster WHERE HostelID = @HostelID AND UPPER(BlockName) = UPPER(@BlockName)";

                    SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                    checkCmd.Parameters.AddWithValue("@HostelID", hostelID);
                    checkCmd.Parameters.AddWithValue("@BlockName", blockName);

                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (count > 0)
                    {
                        lblBlockMessage.Text =
                            "This block already exists for the selected hostel.";
                        lblBlockMessage.ForeColor = System.Drawing.Color.Red;
                        return;
                    }

                    string blockID = GenerateBlockID(con);

                    string insertQuery = @"INSERT INTO HostelBlockMaster(BlockID, HostelID, BlockName, HostelCategory, IsActive, CreatedOn, CreatedBy)
                                       VALUES (@BlockID, @HostelID, @BlockName, @HostelCategory,  @IsActive, GETDATE(), @CreatedBy)";

                    SqlCommand cmd = new SqlCommand(insertQuery, con);
                    cmd.Parameters.AddWithValue("@BlockID", blockID);
                    cmd.Parameters.AddWithValue("@HostelID", hostelID);
                    cmd.Parameters.AddWithValue("@BlockName", blockName);
                    cmd.Parameters.AddWithValue("@HostelCategory", hostelCategory);
                    cmd.Parameters.AddWithValue("@IsActive", isActive);
                    cmd.Parameters.AddWithValue("@CreatedBy", "Admin");

                    cmd.ExecuteNonQuery();
                }

                lblBlockMessage.Text = "Block saved successfully.";
                lblBlockMessage.ForeColor = System.Drawing.Color.Green;

                ClearForm();
                BindGrid();
            }
            else
            {

                lblBlockMessage.Text = "";

                    if (ddlHostel.SelectedValue == "")
                    {
                    lblBlockMessage.Text = "Please select a Hostel.";
                    lblBlockMessage.ForeColor = System.Drawing.Color.Red;
                        return;
                    }

                    string blockIDBeingEdited = hfBlockID.Value;
                    int hostelIDEdit = Convert.ToInt32(ddlHostel.SelectedValue);
                    string blockNameEdit = txtBlockName.Text.Trim();
                    string isActiveEdit = ddlIsActive.SelectedValue;
                    string hostelCategoryEdit = ddlHostelCategory.SelectedValue;


                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        con.Open();

                        string checkQuery = @"SELECT COUNT(*) FROM HostelBlockMaster WHERE HostelID = @HostelID AND UPPER(BlockName) = UPPER(@BlockName) AND ID <> @CurrentBlockID";

                        SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                        checkCmd.Parameters.AddWithValue("@HostelID", hostelIDEdit);
                        checkCmd.Parameters.AddWithValue("@BlockName", blockNameEdit);
                        checkCmd.Parameters.AddWithValue("@CurrentBlockID", blockIDBeingEdited);

                        int countExisting = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (countExisting > 0)
                        {
                        lblBlockMessage.Text = "This block already exists for the selected hostel.";
                        lblBlockMessage.ForeColor = System.Drawing.Color.Red;
                            return;
                        }

                        string queryUpdate = @"UPDATE HostelBlockMaster SET HostelID = @HostelIDEdit,     BlockName = @BlockNameEdit, HostelCategory = @HostelCategoryEdit,     IsActive = @IsActiveEdit WHERE ID = @CurrentBlockID";

                        SqlCommand updateCmd = new SqlCommand(queryUpdate, con);
                        updateCmd.Parameters.AddWithValue("@HostelIDEdit", hostelIDEdit);
                        updateCmd.Parameters.AddWithValue("@BlockNameEdit", blockNameEdit);
                        updateCmd.Parameters.AddWithValue("@IsActiveEdit", isActiveEdit);
                        updateCmd.Parameters.AddWithValue("@CurrentBlockID", blockIDBeingEdited);
                        updateCmd.Parameters.AddWithValue("@HostelCategoryEdit", hostelCategoryEdit);

                        updateCmd.ExecuteNonQuery();
                    }

                    lblBlockMessage.Text = "Block updated successfully.";
                    lblBlockMessage.ForeColor = System.Drawing.Color.Green;

                    hfBlockID.Value = "";
                    ClearForm();
                    BindGrid();
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            lblBlockMessage.Text = "";
            ClearForm();
        }

        private void ClearForm()
        {
            ddlHostel.SelectedIndex = 0;
            txtBlockName.Text = "";
            ddlIsActive.SelectedValue = "Y";
            ddlHostelCategory.SelectedValue = "Mixed";
        }

        // Takes the already-open connection so the SELECT MAX(ID) and the
        // INSERT that follows happen on the same connection -- avoids a
        // needless extra round trip to the database.
        private string GenerateBlockID(SqlConnection con)
        {
            string query = "SELECT ISNULL(MAX(ID),0)+1 FROM HostelBlockMaster";

            SqlCommand cmd = new SqlCommand(query, con);
            int nextID = Convert.ToInt32(cmd.ExecuteScalar());

            return "BLK" + nextID.ToString("0000");
        }

        private void BindGrid()
        {
            string query = @"SELECT HB.ID, HB.BlockID, H.HostelName, HB.BlockName, HB.HostelCategory,  CASE HB.IsActive WHEN 'Y' THEN 'Active'
                             ELSE 'Inactive' END AS IsActive, HB.CreatedOn FROM HostelBlockMaster HB INNER JOIN HostelMaster H ON HB.HostelID = H.ID
                             ORDER BY HB.ID DESC";

            gvBlock.DataSource = obj.GetDataTable(query);
            gvBlock.DataBind();
        }

        protected void gvBlock_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteBlock")
            {
                try
                { 
                    using (SqlConnection conn = new SqlConnection(constr))
                    {
                        conn.Open();
                        String id = e.CommandArgument.ToString();
                        String query = @"DELETE FROM HostelBlockMaster where ID = @ID";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.ExecuteNonQuery();
                        lblBlockMessage.Text = "Block Deleted Successfully";
                    }
                    BindGrid();
                    
                }
                catch (Exception )
                {
                    lblBlockMessage.Text = "Cannot delete: this Block still has Flats/Rooms defined under it. First delete Flats/Room under this Hostel";
                    //lblBlockMessage.Text = "DEBUG: " + ex.Message;
                    lblBlockMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
            else if (e.CommandName == "EditBlock")
            {
                string id = e.CommandArgument.ToString();

                using (SqlConnection conn = new SqlConnection(constr))
                {
                    conn.Open();

                    string query = "SELECT HostelID, BlockName, HostelCategory, IsActive FROM HostelBlockMaster WHERE ID = @ID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", id);

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        ddlHostel.SelectedValue = dr["HostelID"].ToString();
                        txtBlockName.Text = dr["BlockName"].ToString();                        
                        ddlIsActive.SelectedValue = dr["IsActive"].ToString();
                        ddlHostelCategory.SelectedValue = dr["HostelCategory"].ToString();
                        hfBlockID.Value = id;
                    }
                }
            }
        }
    }
}
