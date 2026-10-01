using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace Training.LMS
{
    public partial class LMSBrowse : System.Web.UI.Page
    {
        clsDataAccess objDB = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            bool isAdmin = Session["InternalRedirect_Admin"] != null;
            bool isSuperAdmin = Session["InternalRedirect_SuperAdmin"] != null;
            bool isTrainer = Session["TrainerID"] != null && Session["TrainerID"].ToString() != "";
            bool isTrainee = Session["EmpID"] != null;

            if (!isAdmin && !isSuperAdmin && !isTrainer && !isTrainee)
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            lnkDashboard.NavigateUrl = GetDashboardUrl();

            if (!IsPostBack)
            {
                BindCategoryFilter();
                BindCourseFilter();
                BindMaterials();
            }
        }

        private string GetDashboardUrl()
        {
            if (Session["InternalRedirect_Admin"] != null)
            {
                return "~/Admin/Dashboard.aspx";
            }

            if (Session["InternalRedirect_SuperAdmin"] != null)
            {
                return "~/SuperAdmin/Default.aspx";
            }

            if (Session["TrainerID"] != null && Session["TrainerID"].ToString() != "")
            {
                return "~/Trainer/Default.aspx";
            }

            if (Session["EmpID"] != null)
            {
                return "~/Trainee/MyTrainings.aspx";
            }

            return "~/Default.aspx";
        }

        private void BindCategoryFilter()
        {
            string sql =
                "SELECT DISTINCT Category FROM LMSMaterial " +
                "WHERE ISNULL(Category,'') <> '' AND IsActive=1 " +
                "ORDER BY Category";

            DataTable dt = objDB.GetDataTable(sql, null);

            ddlFilterCategory.DataTextField = "Category";
            ddlFilterCategory.DataValueField = "Category";
            ddlFilterCategory.DataSource = dt;
            ddlFilterCategory.DataBind();

            ddlFilterCategory.Items.Insert(0, new ListItem("All Categories", ""));
        }

        private void BindCourseFilter()
        {
            string sql =
                "SELECT DISTINCT C.CourseID, C.CourseName " +
                "FROM LMSMaterial M " +
                "INNER JOIN CourseMaster C ON C.CourseID = M.CourseID " +
                "WHERE M.IsActive=1 " +
                "ORDER BY C.CourseName";

            DataTable dt = objDB.GetDataTable(sql, null);

            ddlFilterCourse.DataTextField = "CourseName";
            ddlFilterCourse.DataValueField = "CourseID";
            ddlFilterCourse.DataSource = dt;
            ddlFilterCourse.DataBind();

            ddlFilterCourse.Items.Insert(0, new ListItem("All Courses", ""));
        }

        private void BindMaterials()
        {
            string sql =
                "SELECT M.ID, M.Title, M.Description, M.MaterialType, M.FilePath, M.VideoUrl, " +
                "M.Category, ISNULL(C.CourseName,'') AS CourseName " +
                "FROM LMSMaterial M " +
                "LEFT JOIN CourseMaster C ON C.CourseID = M.CourseID " +
                "WHERE M.IsActive=1 ";

            List<SqlParameter> paramList = new List<SqlParameter>();

            if (ddlFilterType.SelectedValue != "")
            {
                sql += "AND M.MaterialType=@MaterialType ";
                paramList.Add(new SqlParameter("@MaterialType", ddlFilterType.SelectedValue));
            }

            if (ddlFilterCategory.SelectedValue != "")
            {
                sql += "AND M.Category=@Category ";
                paramList.Add(new SqlParameter("@Category", ddlFilterCategory.SelectedValue));
            }

            if (ddlFilterCourse.SelectedValue != "")
            {
                sql += "AND M.CourseID=@CourseID ";
                paramList.Add(new SqlParameter("@CourseID", ddlFilterCourse.SelectedValue));
            }

            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                sql += "AND (M.Title LIKE @Search OR M.Description LIKE @Search) ";
                paramList.Add(new SqlParameter("@Search", "%" + txtSearch.Text.Trim() + "%"));
            }

            sql += "ORDER BY M.DisplayOrder, M.Title";

            DataTable dt = objDB.GetDataTable(sql, paramList.ToArray());

            rptMaterials.DataSource = dt;
            rptMaterials.DataBind();

            lblEmpty.Visible = dt.Rows.Count == 0;
        }

        public string GetOpenUrl(object filePath, object videoUrl)
        {
            string file = filePath == DBNull.Value ? "" : filePath.ToString();
            string video = videoUrl == DBNull.Value ? "" : videoUrl.ToString();

            if (!string.IsNullOrWhiteSpace(file))
            {
                return ResolveUrl("~/" + file);
            }

            return string.IsNullOrWhiteSpace(video) ? "#" : video;
        }

        public string GetOpenButtonHtml(object materialType, object filePath, object videoUrl)
        {
            string type = materialType == null ? "" : materialType.ToString();
            string file = filePath == null || filePath == DBNull.Value ? "" : filePath.ToString();
            string video = videoUrl == null || videoUrl == DBNull.Value ? "" : videoUrl.ToString().Trim();

            if (type == "Video" && !string.IsNullOrWhiteSpace(video))
            {
                string embedHtml;

                if (video.StartsWith("<iframe", StringComparison.OrdinalIgnoreCase))
                {
                    embedHtml = video;
                }
                else
                {
                    embedHtml = "<iframe src=\"" + video + "\" width=\"100%\" height=\"480\" style=\"border:0;\" allow=\"autoplay; fullscreen\" allowfullscreen></iframe>";
                }

                string encodedEmbed = HttpUtility.HtmlEncode(embedHtml);
                string plainVideoUrl = ExtractVideoSrcUrl(video);

                return
                    "<a href=\"#\" class=\"btn btn-sm btn-outline-primary me-1\" onclick=\"playVideo(this); return false;\" data-embed=\"" + encodedEmbed + "\">Watch Here</a>" +
                    "<a href=\"" + plainVideoUrl + "\" target=\"_blank\" class=\"btn btn-sm btn-outline-secondary\">Open in New Tab</a>";
            }

            string openUrl = string.IsNullOrWhiteSpace(file) ? "#" : ResolveUrl("~/" + file);

            return "<a href=\"" + openUrl + "\" target=\"_blank\" class=\"btn btn-sm btn-outline-primary\">Open</a>";
        }

        private string ExtractVideoSrcUrl(string video)
        {
            if (video.StartsWith("<iframe", StringComparison.OrdinalIgnoreCase))
            {
                Match match = Regex.Match(video, "src\\s*=\\s*\"([^\"]+)\"", RegexOptions.IgnoreCase);

                return match.Success ? match.Groups[1].Value : "#";
            }

            return video;
        }

        protected void Filter_Changed(object sender, EventArgs e)
        {
            BindMaterials();
        }
    }
}