using System;
using System.Data;
using System.Data.SqlClient;

namespace Training.Admin
{
    public partial class AdminOnlineNominations : System.Web.UI.Page
    {
        clsDataAccess objDB = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["InternalRedirect_Admin"] == null)
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGrid();
        }

        private void BindGrid()
        {
            string sql =
                "SELECT NominationID, Name, Designation, EmpID, Organization, MobileNo, EmailId, CourseName, Remarks, Status, SubmittedOn " +
                "FROM OnlineNomination WHERE 1=1 ";

            if (ddlStatus.SelectedValue != "")
            {
                sql += "AND Status=@Status ";
            }

            sql += "ORDER BY SubmittedOn DESC";

            SqlParameter[] param =
            {
                new SqlParameter("@Status", ddlStatus.SelectedValue)
            };

            DataTable dt = objDB.GetDataTable(sql, param);

            gvNominations.DataSource = dt;
            gvNominations.DataBind();
        }

        protected void gvNominations_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            string nominationID = e.CommandArgument.ToString();

            string adminID = Session["AdminID"] == null ? "Admin" : Session["AdminID"].ToString();

            if (e.CommandName == "ApproveNomination")
            {
                UpdateNominationStatus(nominationID, "Approved", adminID);
            }
            else if (e.CommandName == "RejectNomination")
            {
                UpdateNominationStatus(nominationID, "Rejected", adminID);
            }

            BindGrid();
        }

        private void UpdateNominationStatus(string nominationID, string newStatus, string adminID)
        {
            string sql =
                "UPDATE OnlineNomination " +
                "SET Status=@Status, ReviewedBy=@ReviewedBy, ReviewedOn=GETDATE() " +
                "WHERE NominationID=@NominationID AND Status='Pending'";

            SqlParameter[] param =
            {
        new SqlParameter("@Status", newStatus),
        new SqlParameter("@ReviewedBy", adminID),
        new SqlParameter("@NominationID", nominationID)
    };

            objDB.ExecuteSql(sql, param);
        }
    }
}