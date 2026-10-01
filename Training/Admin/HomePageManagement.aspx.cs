using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class HomePageManagement : System.Web.UI.Page
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
                BindStats();
            }
        }

        private void BindStats()
        {
            object dgUpdated = objDB.ExecuteScalar("SELECT TOP 1 UpdatedOn FROM HomeDGMessage ORDER BY ID DESC", null);

            lblDgLastUpdated.Text =
                (dgUpdated == null || dgUpdated == DBNull.Value)
                ? "Not set yet"
                : Convert.ToDateTime(dgUpdated).ToString("dd-MMM-yyyy");

            lblLeadershipCount.Text = objDB.ExecuteScalar("SELECT COUNT(*) FROM HomeLeadership", null).ToString();

            lblTestimonialCount.Text = objDB.ExecuteScalar("SELECT COUNT(*) FROM HomeTestimonial", null).ToString();

            lblPendingNominations.Text = objDB.ExecuteScalar("SELECT COUNT(*) FROM OnlineNomination WHERE Status='Pending'", null).ToString();

            lblGalleryCount.Text = objDB.ExecuteScalar("SELECT COUNT(*) FROM HomeGallery", null).ToString();

            lblPendingBlogs.Text = objDB.ExecuteScalar("SELECT COUNT(*) FROM HomeBlog WHERE Status='Pending'", null).ToString();
        }
    }
}


 



