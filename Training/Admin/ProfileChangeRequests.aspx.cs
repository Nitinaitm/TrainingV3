using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class ProfileChangeRequests : System.Web.UI.Page
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
                "SELECT " +
                "R.RequestID," +
                "R.EmpID," +
                "E.EmpName," +
                "CASE R.FieldName " +
                "WHEN 'EmpName' THEN 'Name' " +
                "WHEN 'Gender' THEN 'Gender' " +
                "WHEN 'EmpDesignation' THEN 'Designation' " +
                "WHEN 'EmpPostingPlace' THEN 'Posting Place' " +
                "WHEN 'EmpCompany' THEN 'Company' " +
                "WHEN 'MobileNo' THEN 'Mobile' " +
                "WHEN 'EmailId' THEN 'Email' " +
                "WHEN 'EmpPostingHierarchy' THEN 'Posting (Company/Zone/Circle/Division/SubDivision/Section)' " +
                "ELSE R.FieldName END AS FieldLabel," +
                "R.CurrentValue," +
                "R.RequestedValue," +
                "R.Remarks," +
                "R.Status," +
                "R.RequestedOn," +
                "R.ReviewedBy," +
                "R.ReviewedOn " +
                "FROM EmpProfileChangeRequest R " +
                "LEFT JOIN EmpBasicMaster E ON E.EmpID = R.EmpID " +
                "WHERE 1=1 ";

            if (ddlStatus.SelectedValue != "")
            {
                sql += "AND R.Status=@Status ";
            }

            sql += "ORDER BY R.RequestedOn DESC";

            SqlParameter[] param =
            {
                new SqlParameter("@Status", ddlStatus.SelectedValue)
            };

            DataTable dt = objDB.GetDataTable(sql, param);

            gvRequests.DataSource = dt;
            gvRequests.DataBind();
        }

        protected void gvRequests_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string requestID = e.CommandArgument.ToString();

            string adminID = Session["AdminID"] == null ? "Admin" : Session["AdminID"].ToString();

            lblMessage.Text = "";

            if (e.CommandName == "ApproveRequest")
            {
                ApproveRequest(requestID, adminID);
            }
            else if (e.CommandName == "RejectRequest")
            {
                RejectRequest(requestID, adminID);
            }

            BindGrid();
        }

        private void ApproveRequest(string requestID, string adminID)
        {
            string lookupSql =
                "SELECT EmpID, FieldName, RequestedValue, Status FROM EmpProfileChangeRequest WHERE RequestID=@RequestID";

            SqlParameter[] lookupParam =
            {
        new SqlParameter("@RequestID", requestID)
    };

            DataTable dt = objDB.GetDataTable(lookupSql, lookupParam);

            if (dt.Rows.Count == 0)
            {
                return;
            }

            DataRow dr = dt.Rows[0];

            if (dr["Status"].ToString() != "Pending")
            {
                // Already actioned - never process the same request twice.
                return;
            }

            string empID = dr["EmpID"].ToString();
            string fieldName = dr["FieldName"].ToString();
            string requestedValue = dr["RequestedValue"].ToString();

            if (fieldName == "EmpPostingHierarchy")
            {
                string error = ApprovePostingHierarchy(empID, requestedValue, adminID);

                if (error != null)
                {
                    lblMessage.CssClass = "alert alert-danger";
                    lblMessage.Text = error + " The request remains Pending.";
                    return;
                }
            }
            else
            {
                string[] allowedFields = { "EmpName", "Gender", "EmpDesignation", "EmpPostingPlace", "EmpCompany", "MobileNo", "EmailId" };

                if (Array.IndexOf(allowedFields, fieldName) < 0)
                {
                    return;
                }

                string updateSql = "UPDATE EmpBasicMaster SET " + fieldName + "=@Value WHERE EmpID=@EmpID";

                SqlParameter[] updateParam =
                {
            new SqlParameter("@Value", requestedValue),
            new SqlParameter("@EmpID", empID)
        };

                objDB.ExecuteSql(updateSql, updateParam);
            }

            string reviewSql =
                "UPDATE EmpProfileChangeRequest SET Status='Approved', ReviewedBy=@ReviewedBy, ReviewedOn=GETDATE() WHERE RequestID=@RequestID";

            SqlParameter[] reviewParam =
            {
        new SqlParameter("@ReviewedBy", adminID),
        new SqlParameter("@RequestID", requestID)
    };

            objDB.ExecuteSql(reviewSql, reviewParam);
        }

        private string ApprovePostingHierarchy(string empID, string requestedValue, string adminID)
        {
            string[] parts = requestedValue.Split('|');

            string postingPlace;
            string department = "";
            string zone = "";
            string circle = "";
            string division = "";
            string subdivision = "";
            string section = "";

            if (parts.Length > 0 && parts[0] == "HQ")
            {
                department = parts.Length > 1 ? parts[1] : "";
                postingPlace = department;
            }
            else
            {
                zone = parts.Length > 2 ? parts[2] : "";
                circle = parts.Length > 3 ? parts[3] : "";
                division = parts.Length > 4 ? parts[4] : "";
                subdivision = parts.Length > 5 ? parts[5] : "";
                section = parts.Length > 6 ? parts[6] : "";

                if (!string.IsNullOrWhiteSpace(section))
                {
                    postingPlace = "ESS " + section +"(" + division + ")";
                }
                else if (!string.IsNullOrWhiteSpace(subdivision))
                {
                    postingPlace = "ESSD " + subdivision + "(" + division + ")"; ;
                }
                else if (!string.IsNullOrWhiteSpace(division))
                {
                    postingPlace = "ESD " + division;
                }
                else if (!string.IsNullOrWhiteSpace(circle))
                {
                    postingPlace = "ESC " + circle;
                }
                else
                {
                    postingPlace = zone;
                }
            }

            string lengthError =
                CheckFieldLength("Posting Place", postingPlace, 50) ??
                CheckFieldLength("Department", department, 50) ??
                CheckFieldLength("Zone", zone, 50) ??
                CheckFieldLength("Circle", circle, 50) ??
                CheckFieldLength("Division", division, 50) ??
                CheckFieldLength("Sub-Division", subdivision, 50) ??
                CheckFieldLength("Section", section, 50);

            if (lengthError != null)
            {
                return lengthError;
            }

            string updateBasicSql = "UPDATE EmpBasicMaster SET EmpPostingPlace=@EmpPostingPlace WHERE EmpID=@EmpID";

            SqlParameter[] updateBasicParam =
            {
        new SqlParameter("@EmpPostingPlace", postingPlace),
        new SqlParameter("@EmpID", empID)
    };

            int basicRows = objDB.ExecuteSql(updateBasicSql, updateBasicParam);

            if (basicRows == 0)
            {
                return "Could not update the employee's posting place. Please check the Error Log for details.";
            }

            string checkSql = "SELECT COUNT(*) FROM EmpPostingDetails WHERE EmpID=@EmpID";

            SqlParameter[] checkParam = { new SqlParameter("@EmpID", empID) };

            int exists = Convert.ToInt32(objDB.ExecuteScalar(checkSql, checkParam));

            int detailRows;

            if (exists > 0)
            {
                string updateDetailsSql =
                    "UPDATE EmpPostingDetails SET " +
                    "EmpPostingPlace=@EmpPostingPlace, EmpPostingDepartment=@EmpPostingDepartment, " +
                    "AreaBoardZone=@AreaBoardZone, Circle=@Circle, Division=@Division, " +
                    "Subdivision=@Subdivision, Section=@Section " +
                    "WHERE EmpID=@EmpID";

                SqlParameter[] updateDetailsParam =
                {
            new SqlParameter("@EmpPostingPlace", postingPlace),
            new SqlParameter("@EmpPostingDepartment", string.IsNullOrEmpty(department) ? (object)DBNull.Value : department),
            new SqlParameter("@AreaBoardZone", string.IsNullOrEmpty(zone) ? (object)DBNull.Value : zone),
            new SqlParameter("@Circle", string.IsNullOrEmpty(circle) ? (object)DBNull.Value : circle),
            new SqlParameter("@Division", string.IsNullOrEmpty(division) ? (object)DBNull.Value : division),
            new SqlParameter("@Subdivision", string.IsNullOrEmpty(subdivision) ? (object)DBNull.Value : subdivision),
            new SqlParameter("@Section", string.IsNullOrEmpty(section) ? (object)DBNull.Value : section),
            new SqlParameter("@EmpID", empID)
        };

                detailRows = objDB.ExecuteSql(updateDetailsSql, updateDetailsParam);
            }
            else
            {
                string insertDetailsSql =
                    "INSERT INTO EmpPostingDetails " +
                    "(EmpID, EmpPostingPlace, EmpPostingDepartment, AreaBoardZone, Circle, Division, Subdivision, Section, CreatedOn, CreatedBy) " +
                    "VALUES (@EmpID, @EmpPostingPlace, @EmpPostingDepartment, @AreaBoardZone, @Circle, @Division, @Subdivision, @Section, GETDATE(), @CreatedBy)";

                SqlParameter[] insertDetailsParam =
                {
            new SqlParameter("@EmpID", empID),
            new SqlParameter("@EmpPostingPlace", postingPlace),
            new SqlParameter("@EmpPostingDepartment", string.IsNullOrEmpty(department) ? (object)DBNull.Value : department),
            new SqlParameter("@AreaBoardZone", string.IsNullOrEmpty(zone) ? (object)DBNull.Value : zone),
            new SqlParameter("@Circle", string.IsNullOrEmpty(circle) ? (object)DBNull.Value : circle),
            new SqlParameter("@Division", string.IsNullOrEmpty(division) ? (object)DBNull.Value : division),
            new SqlParameter("@Subdivision", string.IsNullOrEmpty(subdivision) ? (object)DBNull.Value : subdivision),
            new SqlParameter("@Section", string.IsNullOrEmpty(section) ? (object)DBNull.Value : section),
            new SqlParameter("@CreatedBy", adminID)
        };

                detailRows = objDB.ExecuteSql(insertDetailsSql, insertDetailsParam);
            }

            if (detailRows == 0)
            {
                return "The employee's posting place was updated, but saving the detailed posting record failed. Please check the Error Log for details.";
            }

            return null;
        }

        private string CheckFieldLength(string label, string value, int maxLength)
        {
            if (!string.IsNullOrEmpty(value) && value.Length > maxLength)
            {
                return "Cannot approve: " + label + " (\"" + value + "\") is " + value.Length +
                       " characters, exceeding the " + maxLength + "-character limit. Please shorten it in the master data first.";
            }

            return null;
        }

        private void RejectRequest(string requestID, string adminID)
        {
            string sql =
                "UPDATE EmpProfileChangeRequest SET Status='Rejected', ReviewedBy=@ReviewedBy, ReviewedOn=GETDATE() " +
                "WHERE RequestID=@RequestID AND Status='Pending'";

            SqlParameter[] param =
            {
                new SqlParameter("@ReviewedBy", adminID),
                new SqlParameter("@RequestID", requestID)
            };

            objDB.ExecuteSql(sql, param);
        }
    }
}