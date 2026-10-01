using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Training.Admin
{
    public partial class HostelStatusDashboard : System.Web.UI.UserControl
    {
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindHostelStatus();
            }
        }

        // Public so the host page can refresh the counts right after an
        // allot/vacate action, without needing a full page reload.
        public void BindHostelStatus()
        {
            string query = @"SELECT H.HostelName, HB.BlockName,
                             COUNT(HBed.ID) AS TotalBeds,
                             SUM(CASE WHEN HBed.Status = 'Occupied' THEN 1 ELSE 0 END) AS OccupiedBeds,
                             SUM(CASE WHEN HBed.Status = 'Vacant' THEN 1 ELSE 0 END) AS VacantBeds
                             FROM HostelMaster H
                             INNER JOIN HostelBlockMaster HB ON HB.HostelID = H.ID AND HB.IsActive = 'Y'
                             INNER JOIN HostelRoomMaster HR ON HR.BlockID = HB.ID
                             INNER JOIN HostelBedMaster HBed ON HBed.RoomID = HR.ID AND HBed.IsActive = 'Y'
                             WHERE H.IsActive = 'Y'
                             GROUP BY H.HostelName, HB.BlockName
                             ORDER BY H.HostelName, HB.BlockName";

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvHostelStatus.DataSource = dt;
                gvHostelStatus.DataBind();
            }
        }
    }
}