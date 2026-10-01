using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Training
{
    public partial class Home : System.Web.UI.Page
    {
        clsDataAccess objDB = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCampusCarousel();
                BindLeadership();
                BindTrainingCalendar();
                BindTrainingActivities();
                BindMarquee();
                BindNews();
                BindCirculars();
                BindGallery();
                BindBlogs();
                BindTestimonials();

                BindDgMessage();

                lblLastUpdated.Text = DateTime.Now.ToString("dd-MMM-yyyy");

                UpdateVisitorCount();
            }
        }
        private void BindDgMessage()
        {
            string sql =
                "SELECT TOP 1 Name, Designation, PhotoPath, ShortQuote, FullMessage " +
                "FROM HomeDGMessage " +
                "ORDER BY ID DESC";

            DataTable dt = objDB.GetDataTable(sql, null);

            if (dt.Rows.Count == 0)
            {
                return;
            }

            DataRow dr = dt.Rows[0];

            lblDgName.Text = dr["Name"].ToString();
            lblDgDesignation.Text = dr["Designation"].ToString();
            lblDgQuote.Text = dr["ShortQuote"].ToString();
            lblDgFullMessage.Text = dr["FullMessage"].ToString();

            string photoPath = dr["PhotoPath"].ToString();

            if (!string.IsNullOrWhiteSpace(photoPath))
            {
                imgDgPhoto.Src = ResolveUrl("~/" + photoPath);
            }
        }
        //---------------------------------------------------------
        // TRAINING CALENDAR - real data, next 60 days of trainings
        //---------------------------------------------------------

        private void BindTrainingCalendar()
        {
            string sql =
                "SELECT TOP 8 " +
                "CM.CourseName," +
                "TD.TrainingType," +
                "TD.TrainingCategory," +
                "TD.TrainingLocation," +
                "TRY_CONVERT(date,TD.DateFrom,105) AS DateFrom," +
                "TRY_CONVERT(date,TD.DateTo,105) AS DateTo " +
                "FROM TrainingDetails TD " +
                "INNER JOIN CourseMaster CM ON CM.CourseID=TD.CourseID " +
                "WHERE TRY_CONVERT(date,TD.DateFrom,105) >= CAST(GETDATE() AS DATE) " +
                "ORDER BY TRY_CONVERT(date,TD.DateFrom,105)";

            DataTable dt = objDB.GetDataTable(sql, null);

            if (dt.Rows.Count == 0)
            {
                lblNoTrainings.Visible = true;
                return;
            }

            rptTrainingCalendar.DataSource = dt;
            rptTrainingCalendar.DataBind();
        }

        

        private void BindLeadership()
        {
            string sql =
                "SELECT Name, Designation, PhotoPath AS PhotoUrl, ISNULL(ProfileUrl,'#') AS ProfileUrl " +
                "FROM HomeLeadership " +
                "WHERE IsActive=1 " +
                "ORDER BY DisplayOrder";

            DataTable dt = objDB.GetDataTable(sql, null);

            rptLeadership.DataSource = dt;
            rptLeadership.DataBind();
        }

        private void BindCampusCarousel()
        {
            string sql =
                "SELECT TOP 8 ThumbnailPath, Caption " +
                "FROM HomeGallery " +
                "WHERE IsActive=1 AND MediaType='photo' " +
                "ORDER BY DisplayOrder";

            DataTable dt = objDB.GetDataTable(sql, null);

            rptCarousel.DataSource = dt;
            rptCarousel.DataBind();

            rptCarouselIndicators.DataSource = dt;
            rptCarouselIndicators.DataBind();
        }
        private void BindTrainingActivities()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("IconClass");
            dt.Columns.Add("Title");
            dt.Columns.Add("Description");

            dt.Rows.Add("fa-solid fa-chalkboard-user", "Classroom Modules", "Structured theory sessions covering safety codes, standards, and operating procedures.");
            dt.Rows.Add("fa-solid fa-bolt", "Field & Line Practicals", "Hands-on 33/11 kV line construction, maintenance, and fault-handling exercises.");
            dt.Rows.Add("fa-solid fa-users-gear", "Management Orientation", "Leadership and administrative orientation for newly posted officers.");

            rptTrainingActivities.DataSource = dt;
            rptTrainingActivities.DataBind();
        }

        private void BindMarquee()
        {
            string sql = "SELECT Text FROM HomeMarquee WHERE IsActive=1 ORDER BY DisplayOrder";

            DataTable dt = objDB.GetDataTable(sql, null);

            rptMarquee.DataSource = dt;
            rptMarquee.DataBind();
        }

        private void BindNews()
        {
            string sql =
                "SELECT FORMAT(PublishedDate,'dd-MMM') AS Date, Title " +
                "FROM HomeAnnouncement " +
                "WHERE Category='News' AND IsActive=1 " +
                "ORDER BY DisplayOrder";

            DataTable dt = objDB.GetDataTable(sql, null);

            rptNews.DataSource = dt;
            rptNews.DataBind();
        }

        private void BindCirculars()
        {
            string sql =
                "SELECT FORMAT(PublishedDate,'dd-MMM') AS Date, Title " +
                "FROM HomeAnnouncement " +
                "WHERE Category='Circular' AND IsActive=1 " +
                "ORDER BY DisplayOrder";

            DataTable dt = objDB.GetDataTable(sql, null);

            rptCirculars.DataSource = dt;
            rptCirculars.DataBind();
        }

        private void BindGallery()
        {
            string sql =
                "SELECT MediaType, ThumbnailPath AS ThumbnailUrl, Caption " +
                "FROM HomeGallery " +
                "WHERE IsActive=1 " +
                "ORDER BY DisplayOrder";

            DataTable dt = objDB.GetDataTable(sql, null);

            rptGallery.DataSource = dt;
            rptGallery.DataBind();
        }

        private void BindBlogs()
        {
            string sql =
                "SELECT Category, Title, Excerpt, ISNULL(Url,'#') AS Url " +
                "FROM HomeBlog " +
                "WHERE Status='Approved' AND IsActive=1 " +
                "ORDER BY DisplayOrder";

            DataTable dt = objDB.GetDataTable(sql, null);

            rptBlogs.DataSource = dt;
            rptBlogs.DataBind();
        }

        private void BindTestimonials()
        {
            string sql =
                "SELECT Quote, Name, Role " +
                "FROM HomeTestimonial " +
                "WHERE Status='Approved' AND IsActive=1 " +
                "ORDER BY DisplayOrder";

            DataTable dt = objDB.GetDataTable(sql, null);

            rptTestimonials.DataSource = dt;
            rptTestimonials.DataBind();
        }

        protected void rptTestimonials_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                if (e.Item.ItemIndex == 0)
                {
                    HtmlGenericControl itemDiv = (HtmlGenericControl)e.Item.FindControl("testimonialItem");

                    if (itemDiv != null)
                    {
                        itemDiv.Attributes["class"] += " active";
                    }
                }
            }
        }

        //---------------------------------------------------------
        // VISITOR COUNTER - simple file-based counter, one increment
        // per browser session, matching this app's existing
        // file-based logging convention (App_Data/ErrorLog.txt).
        //---------------------------------------------------------

        private void UpdateVisitorCount()
        {
            string filePath = Server.MapPath("~/App_Data/VisitorCount.txt");

            int count = 0;

            try
            {
                if (File.Exists(filePath))
                {
                    int.TryParse(File.ReadAllText(filePath).Trim(), out count);
                }

                if (Session["VisitorCounted"] == null)
                {
                    count++;

                    File.WriteAllText(filePath, count.ToString());

                    Session["VisitorCounted"] = true;
                }
            }
            catch
            {
                // A counter should never be able to break the homepage.
            }

            lblVisitorCount.Text = count.ToString("N0");
        }

        protected void btnSiteSearch_Click(object sender, EventArgs e)
        {
            // Site-wide search has no target yet - wire this up once
            // there's a page/content index to search against.
        }
    }
}
