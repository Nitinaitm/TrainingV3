using System;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;

namespace Training.Admin

{
    public partial class HostelMaster : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
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
                BindGrid();
            }
        }

        protected void btnSave_Click(object sender,EventArgs e)
        {
            if (hfHostelID.Value == "")
            {
                lblMessage.Text = "";
                string hostelName = txtHostelName.Text.Trim();
                string location = txtLocation.Text.Trim();
                string description = txtDescription.Text.Trim();
                string isActive = ddlIsActive.SelectedValue;
                string checkQuery = @"SELECT COUNT(*) FROM HostelMaster WHERE UPPER(HostelName) = UPPER('" + hostelName.Replace("'", "''") + "')";
                int count =
                    Convert.ToInt32(
                    obj.ExecuteScalar(
                    checkQuery));

                if (count > 0)
                {
                    lblMessage.Text =
                        "Hostel name already exists.";
                    lblMessage.ForeColor =
                        System.Drawing.Color.Red;
                    return;
                }
                string hostelID =
                               GenerateHostelID();
                string query = @"INSERT INTO HostelMaster(HostelID,HostelName,Location,Description,IsActive,CreatedOn,CreatedBy)
                            VALUES('" + hostelID + @"','" + hostelName.Replace("'", "''") + @"','" + location.Replace("'", "''") + @"','" + description.Replace("'", "''") + @"',
                            '" + isActive + @"',GETDATE(),'Admin')";
                obj.ExecuteSql(query);
                lblMessage.Text =
                    "Hostel saved successfully.";
                lblMessage.ForeColor =
                    System.Drawing.Color.Green;
                ClearForm();
                BindGrid();
            }
            else
            {
                lblMessage.Text = "";

                string hfhostelID = hfHostelID.Value;
                string hostelNameEdit = txtHostelName.Text.Trim();
                string locationEdit = txtLocation.Text.Trim();
                string descriptionEdit = txtDescription.Text.Trim();
                string isActiveEdit = ddlIsActive.SelectedValue;

                // Excludes the row currently being edited -- otherwise a hostel
                // always "conflicts" with itself, since its name obviously still
                // matches its own current name.
                SqlParameter[] checkParams = new SqlParameter[]
                {
                    new SqlParameter("@HostelName", hostelNameEdit),
                    new SqlParameter("@ID", hfhostelID)
                };

                DataTable checkResult = obj.GetDataTable(
                    "SELECT COUNT(*) AS Cnt FROM HostelMaster WHERE UPPER(HostelName) = UPPER('" + hostelNameEdit.Replace("'", "''") + "') AND ID <> " + hfhostelID);

                int countExisting = Convert.ToInt32(checkResult.Rows[0]["Cnt"]);

                if (countExisting > 0)
                {
                    lblMessage.Text = "Hostel name already exists.";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                // No GenerateHostelID() here -- we're updating a row that already
                // has its business ID, not creating a new one.
                string queryUpdate = @"UPDATE HostelMaster SET HostelName = @HostelNameEdit,     Location = @LocationEdit,     Description = @DescriptionEdit,     IsActive = @IsActiveEdit
                                        WHERE ID = @HostelIDEdit";

                SqlParameter[] updateParams = new SqlParameter[]
                {
                    new SqlParameter("@HostelNameEdit", hostelNameEdit),
                    new SqlParameter("@LocationEdit", locationEdit),
                    new SqlParameter("@DescriptionEdit", descriptionEdit),
                    new SqlParameter("@IsActiveEdit", isActiveEdit),
                    new SqlParameter("@HostelIDEdit", hfhostelID)
                };

                obj.ExecuteSql(queryUpdate, updateParams);

                lblMessage.Text = "Hostel updated successfully.";
                lblMessage.ForeColor = System.Drawing.Color.Green;

                hfHostelID.Value = "";   // back to "add new" mode
                ClearForm();
                BindGrid();
            }


        }
        protected void btnReset_Click(
                   object sender,
                   EventArgs e)
        {
            lblMessage.Text = "";
            ClearForm();
        }
        private void ClearForm()
        {
            txtHostelName.Text = "";
            txtLocation.Text = "";
            txtDescription.Text = "";
            ddlIsActive.SelectedValue = "Y";
        }

        private string GenerateHostelID()
        {
            string query = "SELECT ISNULL(MAX(ID),0)+1 FROM HostelMaster";
            int nextID =   Convert.ToInt32(obj.ExecuteScalar(query));
            return "HST" + nextID.ToString("0000");
        }
        private void BindGrid()
        {
            string query = @"SELECT ID, HostelID, HostelName, Location, Description, CASE IsActive WHEN 'Y' THEN 'Active'
                             ELSE 'Inactive' END AS IsActive, CreatedOn FROM HostelMaster ORDER BY ID DESC";
            gvHostel.DataSource = obj.GetDataTable(query);
            gvHostel.DataBind();
        }

        protected void gvHostel_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteHostel")
            {
                try
                {
                    lblMessage.Text = "";

                    using (SqlConnection conn = new SqlConnection(constr))
                    {
                        conn.Open();
                        String id = e.CommandArgument.ToString();
                        String query = @"DELETE FROM HostelMaster where ID = @ID";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.ExecuteNonQuery();
                        lblMessage.Text = "Hostel Deleted Successfully";
                    }
                    BindGrid();
                   
                }
                catch (Exception)
                {
                    lblMessage.Text = "Cannot delete: this hostel still has blocks defined under it. First delete Blocks under this Hostel";

                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
            else if (e.CommandName == "EditHostel")
            {
                lblMessage.Text = "";
                string id = e.CommandArgument.ToString();
                using (SqlConnection conn = new SqlConnection(constr))
                {
                    conn.Open();

                    string query = "SELECT HostelName, Location, Description, IsActive FROM HostelMaster WHERE ID = @ID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", id);

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        txtHostelName.Text = dr["HostelName"].ToString();
                        txtLocation.Text = dr["Location"].ToString();
                        txtDescription.Text = dr["Description"].ToString();
                        ddlIsActive.SelectedValue = dr["IsActive"].ToString();

                        hfHostelID.Value = id;

                    }
                }
            }
        }


    }
}
