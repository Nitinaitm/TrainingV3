using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class TrainingCalenderAdmin : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["InternalRedirect_Admin"] == null)
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindYearDropdown();
                BindMonthDropdown();
                BindLocationDropdown();
                BindOrganizerDropdown();

                RefreshPage();
            }
        }

        protected void Filter_Changed(object sender, EventArgs e)
        {
            RefreshPage();
        }

        private void RefreshPage()
        {
            GenerateCalendar();
            BindYearTable();
        }

        //---------------------------------------------------------
        // FILTER DROPDOWNS
        //---------------------------------------------------------

        private void BindYearDropdown()
        {
            string sql =
                "SELECT DISTINCT YEAR(TRY_CONVERT(date,DateFrom,105)) AS Yr " +
                "FROM TrainingDetails " +
                "WHERE TRY_CONVERT(date,DateFrom,105) IS NOT NULL " +
                "ORDER BY Yr";

            DataTable dt = obj.GetDataTable(sql, null);

            ddlYear.Items.Clear();

            foreach (DataRow dr in dt.Rows)
            {
                ddlYear.Items.Add(new ListItem(dr["Yr"].ToString(), dr["Yr"].ToString()));
            }

            if (ddlYear.Items.FindByValue(DateTime.Now.Year.ToString()) == null)
            {
                ddlYear.Items.Add(new ListItem(DateTime.Now.Year.ToString(), DateTime.Now.Year.ToString()));
            }

            if (ddlYear.Items.FindByValue(DateTime.Now.Year.ToString()) != null)
            {
                ddlYear.SelectedValue = DateTime.Now.Year.ToString();
            }
        }

        private void BindMonthDropdown()
        {
            ddlMonth.Items.Clear();

            for (int m = 1; m <= 12; m++)
            {
                ddlMonth.Items.Add(new ListItem(
                    System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(m),
                    m.ToString()));
            }

            ddlMonth.SelectedValue = DateTime.Now.Month.ToString();
        }

        private void BindLocationDropdown()
        {
            string sql = "SELECT TrainingLocation FROM TrainingLocationMaster ORDER BY TrainingLocation";

            DataTable dt = obj.GetDataTable(sql, null);

            ddlLocation.DataSource = dt;
            ddlLocation.DataTextField = "TrainingLocation";
            ddlLocation.DataValueField = "TrainingLocation";
            ddlLocation.DataBind();

            ddlLocation.Items.Insert(0, new ListItem("-- All --", ""));
        }

        private void BindOrganizerDropdown()
        {
            string sql = "SELECT TrainingOrganizer FROM TrainingOrganizerMaster ORDER BY TrainingOrganizer";

            DataTable dt = obj.GetDataTable(sql, null);

            ddlOrganizer.DataSource = dt;
            ddlOrganizer.DataTextField = "TrainingOrganizer";
            ddlOrganizer.DataValueField = "TrainingOrganizer";
            ddlOrganizer.DataBind();

            ddlOrganizer.Items.Insert(0, new ListItem("-- All --", ""));
        }

        //---------------------------------------------------------
        // CALENDAR (selected month only, count badge per day)
        //---------------------------------------------------------

        private void GenerateCalendar()
        {
            int year = Convert.ToInt32(ddlYear.SelectedValue);
            int month = Convert.ToInt32(ddlMonth.SelectedValue);

            DateTime monthStart = new DateTime(year, month, 1);
            DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);

            lblMonthYear.Text = monthStart.ToString("MMMM yyyy");

            tblCalendar.Rows.Clear();

            TableRow headerRow = new TableRow();
            string[] days = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            foreach (string d in days)
            {
                TableCell cell = new TableCell();
                cell.Text = d;
                cell.HorizontalAlign = HorizontalAlign.Center;
                cell.Font.Bold = true;
                headerRow.Cells.Add(cell);
            }
            tblCalendar.Rows.Add(headerRow);

            DataTable dt = GetTrainings(monthStart, monthEnd, false);

            int startDay = (int)monthStart.DayOfWeek;
            int daysInMonth = DateTime.DaysInMonth(year, month);

            TableRow row = new TableRow();
            for (int i = 0; i < startDay; i++)
            {
                row.Cells.Add(new TableCell());
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                TableCell cell = new TableCell();
                DateTime currentDay = new DateTime(year, month, day);

                string cellHtml = "<div class='day-number'>" + day + "</div>";

                if (currentDay.Date == DateTime.Now.Date)
                {
                    cell.CssClass = "today";
                }

                int count = 0;
                string dayColor = "bg-info";

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["DateFrom"] == DBNull.Value || dr["DateTo"] == DBNull.Value)
                    {
                        continue;
                    }

                    DateTime tFrom = Convert.ToDateTime(dr["DateFrom"]);
                    DateTime tTo = Convert.ToDateTime(dr["DateTo"]);

                    if (currentDay.Date >= tFrom.Date && currentDay.Date <= tTo.Date)
                    {
                        count++;
                    }
                }

                if (count > 0)
                {
                    if (currentDay.Date < DateTime.Now.Date)
                    {
                        dayColor = "bg-success";
                    }
                    else if (currentDay.Date > DateTime.Now.Date)
                    {
                        dayColor = "bg-info";
                    }
                    else
                    {
                        dayColor = "bg-warning text-dark";
                    }

                    string dateParam = currentDay.ToString("yyyy-MM-dd");

                    cellHtml +=
                        "<a href='TrainingList.aspx?Date=" + dateParam + "' class='count-badge " + dayColor + "'>" +
                        count +
                        "</a>";
                }

                cell.Text = cellHtml;

                row.Cells.Add(cell);

                if ((startDay + day) % 7 == 0 || day == daysInMonth)
                {
                    tblCalendar.Rows.Add(row);
                    row = new TableRow();
                }
            }
        }

        //---------------------------------------------------------
        // YEAR-WIDE TABLE
        //---------------------------------------------------------

        private void BindYearTable()
        {
            int year = Convert.ToInt32(ddlYear.SelectedValue);

            DateTime yearStart = new DateTime(year, 1, 1);
            DateTime yearEnd = new DateTime(year, 12, 31);

            lblYearTableCaption.Text = year.ToString();

            DataTable dt = GetTrainings(yearStart, yearEnd, true);

            gvYearList.DataSource = dt;
            gvYearList.DataBind();
        }

        //---------------------------------------------------------
        // SHARED QUERY - month view passes ignoreMonthCheck=false,
        // year table passes true (range is already the whole year)
        //---------------------------------------------------------

        private DataTable GetTrainings(DateTime rangeStart, DateTime rangeEnd, bool forYearTable)
        {
            string sql =
                "SELECT " +
                "TD.TrainingID," +
                "TD.Batch," +
                "TD.TrainingType," +
                "TD.TrainingOrganizer," +
                "TD.TrainingLocation," +
                "TD.BatchStrength," +
                "CM.CourseName," +
                "TRY_CONVERT(date,TD.DateFrom,105) AS DateFrom," +
                "TRY_CONVERT(date,TD.DateTo,105) AS DateTo " +
                "FROM TrainingDetails TD " +
                "INNER JOIN CourseMaster CM ON CM.CourseID=TD.CourseID " +
                "WHERE TRY_CONVERT(date,TD.DateFrom,105) <= @RangeEnd " +
                "AND TRY_CONVERT(date,TD.DateTo,105) >= @RangeStart ";

            if (ddlLocation.SelectedValue != "")
            {
                sql += "AND TD.TrainingLocation=@Location ";
            }

            if (ddlOrganizer.SelectedValue != "")
            {
                sql += "AND TD.TrainingOrganizer=@Organizer ";
            }

            sql += "ORDER BY TRY_CONVERT(date,TD.DateFrom,105)";

            SqlParameter[] param =
            {
                new SqlParameter("@RangeStart", rangeStart),
                new SqlParameter("@RangeEnd", rangeEnd),
                new SqlParameter("@Location", ddlLocation.SelectedValue),
                new SqlParameter("@Organizer", ddlOrganizer.SelectedValue)
            };

            return obj.GetDataTable(sql, param);
        }

        //---------------------------------------------------------
        // YEAR TABLE ROW COMMAND
        //---------------------------------------------------------

        protected void gvYearList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewTraining")
            {
                Session["TrainingID"] = e.CommandArgument.ToString();

                Response.Redirect("ManageTraining.aspx", false);
            }
        }
    }
}