using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using OfficeOpenXml;

namespace Training.Admin 
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCompany();
                BindDesignation();
                BindPostingPlace();


                ClearPostingDetailControls();
                SetPostingDetailVisibility();
            }

            LoadPlugins();
        }

        private clsDataAccess DB()
        {
            return new clsDataAccess();
        }

        /* =========================================================
           BASIC LIST HELPERS
        ========================================================= */

        private List<string> SelectedValues(ListBox listBox)
        {
            List<string> values = new List<string>();

            foreach (ListItem item in listBox.Items)
            {
                if (item.Selected && item.Value != "ALL")
                {
                    values.Add(item.Value);
                }
            }

            return values;
        }

        private bool IsAllSelected(ListBox listBox)
        {
            foreach (ListItem item in listBox.Items)
            {
                if (item.Selected && item.Value == "ALL")
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasCompanySelection()
        {
            if (IsAllSelected(lstCompany))
            {
                return true;
            }

            return SelectedValues(lstCompany).Count > 0;
        }

        private void BindList(ListBox listBox, DataTable dt, string field)
        {
            listBox.Items.Clear();

            foreach (DataRow row in dt.Rows)
            {
                string value = Convert.ToString(row[field]);

                if (!string.IsNullOrWhiteSpace(value))
                {
                    listBox.Items.Add(new ListItem(value, value));
                }
            }
        }

        private void ClearList(ListBox listBox)
        {
            listBox.Items.Clear();
        }

        private void ClearPostingDetailControls()
        {
            rblPostingPlace.ClearSelection();

            lstPostingDepartment.Items.Clear();
            lstAreaBoardZone.Items.Clear();
            lstCircle.Items.Clear();
            lstDivision.Items.Clear();
            lstSubdivision.Items.Clear();
            lstSection.Items.Clear();
        }

        /* =========================================================
           COMPANY
        ========================================================= */

        private void BindCompany()
        {
            DataTable dt = DB().GetDataTable(
                "SELECT CompanyName FROM CompanyMaster WHERE ISNULL(CompanyName,'')<>'' ORDER BY CompanyName"
            );

            lstCompany.Items.Clear();

            lstCompany.Items.Add(
                new ListItem("ALL COMPANIES", "ALL")
            );

            foreach (DataRow row in dt.Rows)
            {
                string companyName = Convert.ToString(row["CompanyName"]);

                if (!string.IsNullOrWhiteSpace(companyName))
                {
                    lstCompany.Items.Add(
                        new ListItem(companyName, companyName)
                    );
                }
            }
        }

        /* =========================================================
           COMPANY NAME FILTER
        ========================================================= */

        private string CompanyWhere(string column)
        {
            List<string> values = SelectedValues(lstCompany);

            if (values.Count == 0 || IsAllSelected(lstCompany))
            {
                return "";
            }

            List<string> parameters = new List<string>();

            for (int i = 0; i < values.Count; i++)
            {
                parameters.Add("@COMP" + i);
            }

            return " AND " + column + " IN (" + string.Join(",", parameters) + ")";
        }

        private SqlParameter[] CompanyParameters()
        {
            List<string> values = SelectedValues(lstCompany);
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (values.Count == 0 || IsAllSelected(lstCompany))
            {
                return parameters.ToArray();
            }

            for (int i = 0; i < values.Count; i++)
            {
                parameters.Add(
                    new SqlParameter("@COMP" + i, values[i])
                );
            }

            return parameters.ToArray();
        }

        /* =========================================================
           COMPANY ID - MULTIPLE COMPANIES
        ========================================================= */

        private List<string> GetSelectedCompanyIDs()
        {
            List<string> companyIDs = new List<string>();

            if (IsAllSelected(lstCompany))
            {
                return companyIDs;
            }

            List<string> companyNames = SelectedValues(lstCompany);

            if (companyNames.Count == 0)
            {
                return companyIDs;
            }

            List<string> parameters = new List<string>();
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            for (int i = 0; i < companyNames.Count; i++)
            {
                string parameterName = "@CMNAME" + i;

                parameters.Add(parameterName);

                sqlParameters.Add(
                    new SqlParameter(parameterName, companyNames[i])
                );
            }

            string sql =
                "SELECT DISTINCT CompanyID FROM CompanyMaster " +
                "WHERE CompanyName IN (" + string.Join(",", parameters) + ") " +
                "ORDER BY CompanyID";

            DataTable dt = DB().GetDataTable(
                sql,
                sqlParameters.ToArray()
            );

            foreach (DataRow row in dt.Rows)
            {
                string companyID = Convert.ToString(row["CompanyID"]);

                if (!string.IsNullOrWhiteSpace(companyID))
                {
                    companyIDs.Add(companyID);
                }
            }

            return companyIDs;
        }

        private string CompanyIDWhere(string column, string prefix, List<SqlParameter> parameters)
        {
            if (IsAllSelected(lstCompany))
            {
                return "";
            }

            List<string> companyIDs = GetSelectedCompanyIDs();

            if (companyIDs.Count == 0)
            {
                return " AND 1=0";
            }

            List<string> placeholders = new List<string>();

            for (int i = 0; i < companyIDs.Count; i++)
            {
                string parameterName = "@" + prefix + i;

                placeholders.Add(parameterName);

                parameters.Add(
                    new SqlParameter(parameterName, companyIDs[i])
                );
            }

            return " AND " + column + " IN (" + string.Join(",", placeholders) + ")";
        }

        /* =========================================================
           DESIGNATION
        ========================================================= */

        private void BindDesignation()
        {
            string sql =
                "SELECT DISTINCT EmpDesignation " +
                "FROM EmpBasicMaster " +
                "WHERE EmpType='Internal' AND ISNULL(EmpDesignation,'')<>''" +
                CompanyWhere("EmpCompany") +
                " ORDER BY EmpDesignation";

            DataTable dt = DB().GetDataTable(
                sql,
                CompanyParameters()
            );

            BindList(
                lstDesignation,
                dt,
                "EmpDesignation"
            );
        }

        /* =========================================================
           HRMS POSTING PLACE
        ========================================================= */

        private void BindPostingPlace()
        {
            string sql =
                "SELECT DISTINCT EmpPostingPlace " +
                "FROM EmpBasicMaster " +
                "WHERE EmpType='Internal' AND ISNULL(EmpPostingPlace,'')<>''" +
                CompanyWhere("EmpCompany") +
                " ORDER BY EmpPostingPlace";

            DataTable dt = DB().GetDataTable(
                sql,
                CompanyParameters()
            );

            BindList(
                lstPostingPlace,
                dt,
                "EmpPostingPlace"
            );
        }

        /* =========================================================
           POSTING DETAILS VISIBILITY
        ========================================================= */

        private void SetPostingDetailVisibility()
        {
            bool hasCompany = HasCompanySelection();

            grpPostingDetails.Visible = hasCompany;
            grpPostingPlace.Visible = hasCompany;

            bool isHQ = rblPostingPlace.SelectedValue == "HQ";
            bool isField = rblPostingPlace.SelectedValue == "Field";

            grpPostingDepartment.Visible = hasCompany && isHQ;

            grpAreaBoardZone.Visible = hasCompany && isField;
            grpCircle.Visible = hasCompany && isField;
            grpDivision.Visible = hasCompany && isField;
            grpSubdivision.Visible = hasCompany && isField;
            grpSection.Visible = hasCompany && isField;
        }

        /* =========================================================
           HQ ONLY COMPANY CHECK
        ========================================================= */

        private bool IsHqOnlyCompany(string company)
        {
            if (string.IsNullOrWhiteSpace(company))
            {
                return false;
            }

            return company.Equals(
                       "BSPHCL",
                       StringComparison.OrdinalIgnoreCase
                   )
                   ||
                   company.Equals(
                       "BSPGCL",
                       StringComparison.OrdinalIgnoreCase
                   );
        }

        private bool IsHqOnlySelection()
        {
            if (IsAllSelected(lstCompany))
            {
                return false;
            }

            List<string> companies = SelectedValues(lstCompany);

            if (companies.Count == 0)
            {
                return false;
            }

            foreach (string company in companies)
            {
                if (!IsHqOnlyCompany(company))
                {
                    return false;
                }
            }

            return true;
        }

        /* =========================================================
           POSTING PLACE RADIO
        ========================================================= */

        private void BindPostingPlaceRadio()
        {
            string oldValue = rblPostingPlace.SelectedValue;

            rblPostingPlace.Items.Clear();

            rblPostingPlace.Items.Add(
                new ListItem("HQ", "HQ")
            );

            if (!IsHqOnlySelection())
            {
                rblPostingPlace.Items.Add(
                    new ListItem("Field Office", "Field")
                );
            }

            if (!string.IsNullOrWhiteSpace(oldValue))
            {
                ListItem item =
                    rblPostingPlace.Items.FindByValue(oldValue);

                if (item != null)
                {
                    item.Selected = true;
                }
            }

            if (IsHqOnlySelection())
            {
                rblPostingPlace.SelectedValue = "HQ";
            }
        }

        /* =========================================================
           DEPARTMENT / OFFICE / CELL
           SOURCE = EmpBasicMaster
        ========================================================= */


        private void BindPostingDepartment()
        {
            lstPostingDepartment.Items.Clear();

            if (!HasCompanySelection())
            {
                return;
            }

            DataTable dt = DB().GetDataTable(
                "SELECT DISTINCT DepartmentName FROM DepartmentMaster WHERE ISNULL(DepartmentName,'')<>'' ORDER BY DepartmentName"
            );

            BindList(
                lstPostingDepartment,
                dt,
                "DepartmentName"
            );
        }
        /* =========================================================
           ZONE
           SOURCE = ZoneMaster
           VALUE = ZoneName
        ========================================================= */

        private void BindAreaBoardZone()
        {
            lstAreaBoardZone.Items.Clear();

            if (!HasCompanySelection())
            {
                return;
            }

            List<SqlParameter> parameters = new List<SqlParameter>();

            string sql =
                "SELECT DISTINCT ZM.ZoneName " +
                "FROM ZoneMaster ZM " +
                "WHERE ISNULL(ZM.ZoneName,'')<>''";

            sql += CompanyIDWhere(
                "ZM.CompanyID",
                "ZONECOMP",
                parameters
            );

            sql += " ORDER BY ZM.ZoneName";

            DataTable dt = DB().GetDataTable(
                sql,
                parameters.ToArray()
            );

            BindList(
                lstAreaBoardZone,
                dt,
                "ZoneName"
            );
        }

        /* =========================================================
           CIRCLE
           SOURCE = CircleMaster
           FILTER = SELECTED COMPANY + SELECTED ZONE NAMES
        ========================================================= */

        private void BindCircle()
        {
            lstCircle.Items.Clear();

            if (!HasCompanySelection())
            {
                return;
            }

            List<string> zones = SelectedValues(lstAreaBoardZone);

            if (zones.Count == 0)
            {
                return;
            }

            List<SqlParameter> parameters = new List<SqlParameter>();

            string sql =
                "SELECT DISTINCT CM.CircleName " +
                "FROM CircleMaster CM " +
                "INNER JOIN ZoneMaster ZM " +
                "ON ZM.CompanyID=CM.CompanyID " +
                "AND ZM.ZoneID=CM.ZoneID " +
                "WHERE ISNULL(CM.CircleName,'')<>''";

            sql += CompanyIDWhere(
                "CM.CompanyID",
                "CIRCOMP",
                parameters
            );

            List<string> zoneParameters = new List<string>();

            for (int i = 0; i < zones.Count; i++)
            {
                string parameterName = "@ZONE" + i;

                zoneParameters.Add(parameterName);

                parameters.Add(
                    new SqlParameter(parameterName, zones[i])
                );
            }

            sql +=
                " AND ZM.ZoneName IN (" +
                string.Join(",", zoneParameters) +
                ")";

            sql += " ORDER BY CM.CircleName";

            DataTable dt = DB().GetDataTable(
                sql,
                parameters.ToArray()
            );

            BindList(
                lstCircle,
                dt,
                "CircleName"
            );
        }

        /* =========================================================
           DIVISION
           SOURCE = DivisionMaster
        ========================================================= */

        private void BindDivision()
        {
            lstDivision.Items.Clear();

            if (!HasCompanySelection())
            {
                return;
            }

            List<string> zones = SelectedValues(lstAreaBoardZone);
            List<string> circles = SelectedValues(lstCircle);

            if (zones.Count == 0 || circles.Count == 0)
            {
                return;
            }

            List<SqlParameter> parameters = new List<SqlParameter>();

            string sql =
                "SELECT DISTINCT DM.DivisionName " +
                "FROM DivisionMaster DM " +
                "INNER JOIN ZoneMaster ZM " +
                "ON ZM.CompanyID=DM.CompanyID " +
                "AND ZM.ZoneID=DM.ZoneID " +
                "INNER JOIN CircleMaster CM " +
                "ON CM.CompanyID=DM.CompanyID " +
                "AND CM.ZoneID=DM.ZoneID " +
                "AND CM.CircleID=DM.CircleID " +
                "WHERE ISNULL(DM.DivisionName,'')<>''";

            sql += CompanyIDWhere(
                "DM.CompanyID",
                "DIVCOMP",
                parameters
            );

            List<string> zoneParameters = new List<string>();

            for (int i = 0; i < zones.Count; i++)
            {
                string parameterName = "@DIVZONE" + i;

                zoneParameters.Add(parameterName);

                parameters.Add(
                    new SqlParameter(parameterName, zones[i])
                );
            }

            sql +=
                " AND ZM.ZoneName IN (" +
                string.Join(",", zoneParameters) +
                ")";

            List<string> circleParameters = new List<string>();

            for (int i = 0; i < circles.Count; i++)
            {
                string parameterName = "@DIVCIRCLE" + i;

                circleParameters.Add(parameterName);

                parameters.Add(
                    new SqlParameter(parameterName, circles[i])
                );
            }

            sql +=
                " AND CM.CircleName IN (" +
                string.Join(",", circleParameters) +
                ")";

            sql += " ORDER BY DM.DivisionName";

            DataTable dt = DB().GetDataTable(
                sql,
                parameters.ToArray()
            );

            BindList(
                lstDivision,
                dt,
                "DivisionName"
            );
        }

        /* =========================================================
           SUBDIVISION
           SOURCE = SubdivisionMaster
        ========================================================= */

        private void BindSubdivision()
        {
            lstSubdivision.Items.Clear();

            if (!HasCompanySelection())
            {
                return;
            }

            List<string> zones = SelectedValues(lstAreaBoardZone);
            List<string> circles = SelectedValues(lstCircle);
            List<string> divisions = SelectedValues(lstDivision);

            if (
                zones.Count == 0 ||
                circles.Count == 0 ||
                divisions.Count == 0
            )
            {
                return;
            }

            List<SqlParameter> parameters = new List<SqlParameter>();

            string sql =
                "SELECT DISTINCT SM.SubdivisionName " +
                "FROM SubdivisionMaster SM " +
                "INNER JOIN ZoneMaster ZM " +
                "ON ZM.CompanyID=SM.CompanyID " +
                "AND ZM.ZoneID=SM.ZoneID " +
                "INNER JOIN CircleMaster CM " +
                "ON CM.CompanyID=SM.CompanyID " +
                "AND CM.ZoneID=SM.ZoneID " +
                "AND CM.CircleID=SM.CircleID " +
                "INNER JOIN DivisionMaster DM " +
                "ON DM.CompanyID=SM.CompanyID " +
                "AND DM.ZoneID=SM.ZoneID " +
                "AND DM.CircleID=SM.CircleID " +
                "AND DM.DivisionID=SM.DivisionID " +
                "WHERE ISNULL(SM.SubdivisionName,'')<>''";

            sql += CompanyIDWhere(
                "SM.CompanyID",
                "SUBCOMP",
                parameters
            );

            List<string> zoneParameters = new List<string>();

            for (int i = 0; i < zones.Count; i++)
            {
                string parameterName = "@SUBZONE" + i;

                zoneParameters.Add(parameterName);

                parameters.Add(
                    new SqlParameter(parameterName, zones[i])
                );
            }

            sql +=
                " AND ZM.ZoneName IN (" +
                string.Join(",", zoneParameters) +
                ")";

            List<string> circleParameters = new List<string>();

            for (int i = 0; i < circles.Count; i++)
            {
                string parameterName = "@SUBCIRCLE" + i;

                circleParameters.Add(parameterName);

                parameters.Add(
                    new SqlParameter(parameterName, circles[i])
                );
            }

            sql +=
                " AND CM.CircleName IN (" +
                string.Join(",", circleParameters) +
                ")";

            List<string> divisionParameters = new List<string>();

            for (int i = 0; i < divisions.Count; i++)
            {
                string parameterName = "@SUBDIV" + i;

                divisionParameters.Add(parameterName);

                parameters.Add(
                    new SqlParameter(parameterName, divisions[i])
                );
            }

            sql +=
                " AND DM.DivisionName IN (" +
                string.Join(",", divisionParameters) +
                ")";

            sql += " ORDER BY SM.SubdivisionName";

            DataTable dt = DB().GetDataTable(
                sql,
                parameters.ToArray()
            );

            BindList(
                lstSubdivision,
                dt,
                "SubdivisionName"
            );
        }

        /* =========================================================
           SECTION
           SOURCE = SectionMaster
        ========================================================= */

        private void BindSection()
        {
            lstSection.Items.Clear();

            if (!HasCompanySelection())
            {
                return;
            }

            List<string> zones = SelectedValues(lstAreaBoardZone);
            List<string> circles = SelectedValues(lstCircle);
            List<string> divisions = SelectedValues(lstDivision);
            List<string> subdivisions = SelectedValues(lstSubdivision);

            if (
                zones.Count == 0 ||
                circles.Count == 0 ||
                divisions.Count == 0 ||
                subdivisions.Count == 0
            )
            {
                return;
            }

            List<SqlParameter> parameters = new List<SqlParameter>();

            string sql =
                "SELECT DISTINCT SEM.SectionName " +
                "FROM SectionMaster SEM " +
                "INNER JOIN ZoneMaster ZM " +
                "ON ZM.CompanyID=SEM.CompanyID " +
                "AND ZM.ZoneID=SEM.ZoneID " +
                "INNER JOIN CircleMaster CM " +
                "ON CM.CompanyID=SEM.CompanyID " +
                "AND CM.ZoneID=SEM.ZoneID " +
                "AND CM.CircleID=SEM.CircleID " +
                "INNER JOIN DivisionMaster DM " +
                "ON DM.CompanyID=SEM.CompanyID " +
                "AND DM.ZoneID=SEM.ZoneID " +
                "AND DM.CircleID=SEM.CircleID " +
                "AND DM.DivisionID=SEM.DivisionID " +
                "INNER JOIN SubdivisionMaster SM " +
                "ON SM.CompanyID=SEM.CompanyID " +
                "AND SM.ZoneID=SEM.ZoneID " +
                "AND SM.CircleID=SEM.CircleID " +
                "AND SM.DivisionID=SEM.DivisionID " +
                "AND SM.SubdivisionID=SEM.SubdivisionID " +
                "WHERE ISNULL(SEM.SectionName,'')<>''";

            sql += CompanyIDWhere(
                "SEM.CompanyID",
                "SECCOMP",
                parameters
            );

            List<string> zoneParameters = new List<string>();

            for (int i = 0; i < zones.Count; i++)
            {
                string parameterName = "@SECZONE" + i;

                zoneParameters.Add(parameterName);

                parameters.Add(
                    new SqlParameter(parameterName, zones[i])
                );
            }

            sql +=
                " AND ZM.ZoneName IN (" +
                string.Join(",", zoneParameters) +
                ")";

            List<string> circleParameters = new List<string>();

            for (int i = 0; i < circles.Count; i++)
            {
                string parameterName = "@SECCIRCLE" + i;

                circleParameters.Add(parameterName);

                parameters.Add(
                    new SqlParameter(parameterName, circles[i])
                );
            }

            sql +=
                " AND CM.CircleName IN (" +
                string.Join(",", circleParameters) +
                ")";

            List<string> divisionParameters = new List<string>();

            for (int i = 0; i < divisions.Count; i++)
            {
                string parameterName = "@SECDIV" + i;

                divisionParameters.Add(parameterName);

                parameters.Add(
                    new SqlParameter(parameterName, divisions[i])
                );
            }

            sql +=
                " AND DM.DivisionName IN (" +
                string.Join(",", divisionParameters) +
                ")";

            List<string> subdivisionParameters = new List<string>();

            for (int i = 0; i < subdivisions.Count; i++)
            {
                string parameterName = "@SECSUB" + i;

                subdivisionParameters.Add(parameterName);

                parameters.Add(
                    new SqlParameter(parameterName, subdivisions[i])
                );
            }

            sql +=
                " AND SM.SubdivisionName IN (" +
                string.Join(",", subdivisionParameters) +
                ")";

            sql += " ORDER BY SEM.SectionName";

            DataTable dt = DB().GetDataTable(
                sql,
                parameters.ToArray()
            );

            BindList(
                lstSection,
                dt,
                "SectionName"
            );
        }

        /* =========================================================
           COMPANY CHANGE
        ========================================================= */

        //protected void lstCompany_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    BindDesignation();

        //    BindPostingPlace();

        //    ClearPostingDetailControls();

        //    BindPostingPlaceRadio();

        //    SetPostingDetailVisibility();

        //    LoadPlugins();
        //}

        protected void lstCompany_SelectedIndexChanged(
object sender,
EventArgs e)
        {
            BindDesignation();

            BindPostingPlace();

            ClearPostingDetailControls();

            BindPostingPlaceRadio();

            if (rblPostingPlace.SelectedValue == "HQ")
            {
                BindPostingDepartment();
            }

            if (rblPostingPlace.SelectedValue == "Field")
            {
                BindAreaBoardZone();
            }

            SetPostingDetailVisibility();

            LoadPlugins();
        }
        /* =========================================================
           HQ / FIELD CHANGE
        ========================================================= */

        protected void rblPostingPlace_SelectedIndexChanged(
      object sender,
      EventArgs e)
        {
            lstPostingDepartment.Items.Clear();

            lstAreaBoardZone.Items.Clear();
            lstCircle.Items.Clear();
            lstDivision.Items.Clear();
            lstSubdivision.Items.Clear();
            lstSection.Items.Clear();

            if (rblPostingPlace.SelectedValue == "HQ")
            {
                BindPostingDepartment();
            }

            if (rblPostingPlace.SelectedValue == "Field")
            {
                BindAreaBoardZone();
            }

            SetPostingDetailVisibility();

            LoadPlugins();
        }

        /* =========================================================
           ZONE CHANGE
        ========================================================= */

        protected void lstAreaBoardZone_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstCircle.Items.Clear();
            lstDivision.Items.Clear();
            lstSubdivision.Items.Clear();
            lstSection.Items.Clear();

            BindCircle();

            SetPostingDetailVisibility();

            LoadPlugins();
        }

        /* =========================================================
           CIRCLE CHANGE
        ========================================================= */

        protected void lstCircle_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstDivision.Items.Clear();
            lstSubdivision.Items.Clear();
            lstSection.Items.Clear();

            BindDivision();

            SetPostingDetailVisibility();

            LoadPlugins();
        }

        /* =========================================================
           DIVISION CHANGE
        ========================================================= */

        protected void lstDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstSubdivision.Items.Clear();
            lstSection.Items.Clear();

            BindSubdivision();

            SetPostingDetailVisibility();

            LoadPlugins();
        }

        /* =========================================================
           SUBDIVISION CHANGE
        ========================================================= */

        protected void lstSubdivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstSection.Items.Clear();

            BindSection();

            SetPostingDetailVisibility();

            LoadPlugins();
        }

        /* =========================================================
           SEARCH
        ========================================================= */

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindEmployee();

            LoadPlugins();
        }

        /* =========================================================
           EMPLOYEE SEARCH
        ========================================================= */

        private void BindEmployee()
        {
            StringBuilder query = new StringBuilder();

            query.Append(
                "SELECT " +
                "EBM.ID, " +
                "EBM.EmpID, " +
                "EBM.EmpName, " +
                "EBM.MobileNo, " +
                "EBM.EmailId, " +
                "EBM.EmpCompany, " +
                "EBM.EmpDesignation, " +
                "EBM.EmpPostingPlace, " +
                "EPD.EmpPostingPlace AS DetailPostingPlace, " +
                "EPD.EmpPostingDepartment, " +
                "EPD.AreaBoardZone, " +
                "EPD.Circle, " +
                "EPD.Division, " +
                "EPD.Subdivision, " +
                "EPD.Section " +
                "FROM EmpBasicMaster EBM " +
                "LEFT JOIN EmpPostingDetails EPD " +
                "ON EPD.ID = (" +
                "SELECT TOP 1 X.ID " +
                "FROM EmpPostingDetails X " +
                "WHERE X.EmpID=EBM.EmpID " +
                "ORDER BY X.ID DESC" +
                ") " +
                "WHERE EBM.EmpType='Internal'"
            );

            using (
                SqlConnection con =
                    new SqlConnection(
                        System.Configuration.ConfigurationManager
                        .ConnectionStrings["constr"]
                        .ConnectionString
                    )
            )
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;

                AddTextFilter(
                    query,
                    cmd,
                    "EBM.EmpID",
                    txtEmpID.Text,
                    "@EmpID"
                );

                AddTextFilter(
                    query,
                    cmd,
                    "EBM.EmpName",
                    txtEmpName.Text,
                    "@EmpName"
                );

                AddTextFilter(
                    query,
                    cmd,
                    "EBM.MobileNo",
                    txtMobile.Text,
                    "@MobileNo"
                );

                AddTextFilter(
                    query,
                    cmd,
                    "EBM.EmailId",
                    txtEmail.Text,
                    "@EmailId"
                );

                AddMultiSelectFilter(
                    query,
                    cmd,
                    lstCompany,
                    "EBM.EmpCompany",
                    "Company"
                );

                AddMultiSelectFilter(
                    query,
                    cmd,
                    lstDesignation,
                    "EBM.EmpDesignation",
                    "Designation"
                );

                AddMultiSelectFilter(
                    query,
                    cmd,
                    lstPostingPlace,
                    "EBM.EmpPostingPlace",
                    "PostingPlace"
                );

                /* =================================================
                   POSTING DETAILS PLACE - RADIO
                ================================================= */

                if (!string.IsNullOrWhiteSpace(rblPostingPlace.SelectedValue))
                {
                    query.Append(
                        " AND EPD.EmpPostingPlace=@DetailPlace"
                    );

                    cmd.Parameters.Add(
                        new SqlParameter(
                            "@DetailPlace",
                            rblPostingPlace.SelectedValue
                        )
                    );
                }

                /* =================================================
                   HQ DEPARTMENT
                ================================================= */

                AddMultiSelectFilter(
                    query,
                    cmd,
                    lstPostingDepartment,
                    "EPD.EmpPostingDepartment",
                    "Department"
                );

                /* =================================================
                   FIELD HIERARCHY
                ================================================= */

                AddMultiSelectFilter(
                    query,
                    cmd,
                    lstAreaBoardZone,
                    "EPD.AreaBoardZone",
                    "AreaBoard"
                );

                AddMultiSelectFilter(
                    query,
                    cmd,
                    lstCircle,
                    "EPD.Circle",
                    "Circle"
                );

                AddMultiSelectFilter(
                    query,
                    cmd,
                    lstDivision,
                    "EPD.Division",
                    "Division"
                );

                AddMultiSelectFilter(
                    query,
                    cmd,
                    lstSubdivision,
                    "EPD.Subdivision",
                    "Subdivision"
                );

                AddMultiSelectFilter(
                    query,
                    cmd,
                    lstSection,
                    "EPD.Section",
                    "Section"
                );

                query.Append(
                    " ORDER BY EBM.EmpID"
                );

                cmd.CommandText = query.ToString();

                SqlDataAdapter da =
                    new SqlDataAdapter(cmd);

                DataTable dt =
                    new DataTable();

                da.Fill(dt);

                gvEmployee.DataSource = dt;

                gvEmployee.DataBind();

                gridScrollTop.Visible = dt.Rows.Count > 0;
            }
        }

        /* =========================================================
           TEXT FILTER
        ========================================================= */

        private void AddTextFilter(
            StringBuilder query,
            SqlCommand cmd,
            string column,
            string value,
            string parameter)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                query.Append(
                    " AND " +
                    column +
                    " LIKE " +
                    parameter
                );

                cmd.Parameters.Add(
                    new SqlParameter(
                        parameter,
                        "%" + value.Trim() + "%"
                    )
                );
            }
        }

        /* =========================================================
           MULTI SELECT FILTER
        ========================================================= */

        private void AddMultiSelectFilter(
            StringBuilder query,
            SqlCommand cmd,
            ListBox listBox,
            string columnName,
            string parameterPrefix)
        {
            if (IsAllSelected(listBox))
            {
                return;
            }

            List<string> parameters =
                new List<string>();

            int count = 0;

            foreach (ListItem item in listBox.Items)
            {
                if (
                    item.Selected &&
                    item.Value != "ALL"
                )
                {
                    string parameterName =
                        "@" +
                        parameterPrefix +
                        count;

                    parameters.Add(parameterName);

                    cmd.Parameters.Add(
                        new SqlParameter(
                            parameterName,
                            item.Value
                        )
                    );

                    count++;
                }
            }

            if (parameters.Count > 0)
            {
                query.Append(
                    " AND " +
                    columnName +
                    " IN (" +
                    string.Join(",", parameters) +
                    ")"
                );
            }
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            BindEmployee();

            if (gvEmployee.Rows.Count == 0)
            {
                return;
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Employee Data");

                int exportColumnIndex = 1;

                for (int i = 2; i < gvEmployee.HeaderRow.Cells.Count; i++)
                {
                    worksheet.Cells[1, exportColumnIndex].Value = gvEmployee.HeaderRow.Cells[i].Text;
                    exportColumnIndex++;
                }

                for (int rowIndex = 0; rowIndex < gvEmployee.Rows.Count; rowIndex++)
                {
                    exportColumnIndex = 1;

                    for (int cellIndex = 2; cellIndex < gvEmployee.Rows[rowIndex].Cells.Count; cellIndex++)
                    {
                        worksheet.Cells[rowIndex + 2, exportColumnIndex].Value = Server.HtmlDecode(gvEmployee.Rows[rowIndex].Cells[cellIndex].Text);
                        exportColumnIndex++;
                    }
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=EmployeeData.xlsx");
                Response.BinaryWrite(package.GetAsByteArray());
                Response.End();
            }
        }

        /* =========================================================
           RESET
        ========================================================= */

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtEmpID.Text = "";
            txtEmpName.Text = "";
            txtMobile.Text = "";
            txtEmail.Text = "";

            lstCompany.ClearSelection();
            lstDesignation.ClearSelection();
            lstPostingPlace.ClearSelection();

            ClearPostingDetailControls();

            BindCompany();

            BindDesignation();

            BindPostingPlace();

            grpPostingDetails.Visible = false;
            grpPostingPlace.Visible = false;
            grpPostingDepartment.Visible = false;
            grpAreaBoardZone.Visible = false;
            grpCircle.Visible = false;
            grpDivision.Visible = false;
            grpSubdivision.Visible = false;
            grpSection.Visible = false;

            gvEmployee.DataSource = null;

            gvEmployee.DataBind();

            gridScrollTop.Visible = false;

            LoadPlugins();
        }

        /* =========================================================
           SELECT2
        ========================================================= */
        private void LoadPlugins()
        {
            string script =
                "if(typeof LoadSearchableDropdowns==='function'){LoadSearchableDropdowns();}";

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                Guid.NewGuid().ToString(),
                script,
                true
            );
        }
        //private void LoadPlugins()
        //{
        //    string script =
        //        "setTimeout(function(){" +
        //        "if(typeof LoadSearchableDropdowns==='function')" +
        //        "{LoadSearchableDropdowns();}" +
        //        "},100);";

        //    ScriptManager.RegisterStartupScript(
        //        this,
        //        GetType(),
        //        Guid.NewGuid().ToString(),
        //        script,
        //        true
        //    );
        //}
    }

}
