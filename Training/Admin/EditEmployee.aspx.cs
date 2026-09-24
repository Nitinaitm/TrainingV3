using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class EditEmployee : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string role = Convert.ToString(Session["Role"]);

            if (!role.Equals("Admin", StringComparison.OrdinalIgnoreCase) &&
                !role.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase) &&
                !role.Equals("Nodal", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadEmployee(Request.QueryString["EmpID"]);
            }
        }


        private clsDataAccess DB()
        {
            return new clsDataAccess();
        }


        private void LoadEmployee(string empID)
        {
            if (string.IsNullOrWhiteSpace(empID))
            {
                lblMessage.Text = "Invalid Employee ID.";
                return;
            }

            DataTable dt = DB().GetDataTable(
                "SELECT TOP 1 EmpID,EmpName,MobileNo,EmailId,EmpCompany,EmpDesignation,EmpPostingPlace FROM EmpBasicMaster WHERE EmpID=@EmpID",
                new SqlParameter[]
                {
                    new SqlParameter("@EmpID", empID)
                }
            );

            if (dt.Rows.Count == 0)
            {
                lblMessage.Text = "Employee not found.";
                return;
            }

            DataRow row = dt.Rows[0];

            hfEmpID.Value = Convert.ToString(row["EmpID"]);

            txtEmpID.Text =
                Convert.ToString(row["EmpID"]);

            txtEmpName.Text =
                Convert.ToString(row["EmpName"]);

            txtMobile.Text =
                Convert.ToString(row["MobileNo"]);

            txtEmail.Text =
                Convert.ToString(row["EmailId"]);

            BindCompany(
                Convert.ToString(row["EmpCompany"])
            );

            BindDesignation(
                Convert.ToString(row["EmpDesignation"])
            );

            BindHRMSPostingPlace(
                Convert.ToString(row["EmpPostingPlace"])
            );

            LoadPostingDetails();
        }


        private void BindCompany(string selected)
        {
            DataTable dt = DB().GetDataTable(
                "SELECT CompanyID,CompanyName,CompanyAlias FROM CompanyMaster ORDER BY CompanyName"
            );

            ddlCompany.Items.Clear();

            ddlCompany.Items.Add(
                new ListItem(
                    "Select Company",
                    ""
                )
            );

            foreach (DataRow row in dt.Rows)
            {
                string companyName =
                    Convert.ToString(
                        row["CompanyName"]
                    );

                string companyAlias =
                    Convert.ToString(
                        row["CompanyAlias"]
                    );

                string displayName =
                    string.IsNullOrWhiteSpace(companyAlias)
                        ? companyName
                        : companyName + " (" + companyAlias + ")";

                ddlCompany.Items.Add(
                    new ListItem(
                        displayName,
                        companyName
                    )
                );
            }

            if (ddlCompany.Items.FindByValue(selected) != null)
            {
                ddlCompany.SelectedValue = selected;
            }
            else
            {
                object companyName =
                    DB().ExecuteScalar(
                        "SELECT TOP 1 CompanyName FROM CompanyMaster WHERE CompanyAlias=@Company",
                        new SqlParameter[]
                        {
                            new SqlParameter(
                                "@Company",
                                selected
                            )
                        }
                    );

                if (companyName != null &&
                    ddlCompany.Items.FindByValue(
                        Convert.ToString(companyName)
                    ) != null)
                {
                    ddlCompany.SelectedValue =
                        Convert.ToString(companyName);
                }
            }
        }


        private void BindDesignation(string selected)
        {
            BindList(
                ddlDesignation,
                "SELECT DISTINCT EmpDesignation FROM EmpBasicMaster WHERE ISNULL(EmpDesignation,'')<>'' ORDER BY EmpDesignation",
                "EmpDesignation",
                "Select Designation",
                selected
            );
        }


        private void BindHRMSPostingPlace(string selected)
        {
            BindList(
                ddlPostingPlace,
                "SELECT DISTINCT EmpPostingPlace FROM EmpBasicMaster WHERE ISNULL(EmpPostingPlace,'')<>'' ORDER BY EmpPostingPlace",
                "EmpPostingPlace",
                "Select HRMS Posting Place",
                selected
            );
        }


        private void BindList(
            DropDownList ddl,
            string sql,
            string field,
            string firstItem,
            string selected)
        {
            DataTable dt =
                DB().GetDataTable(sql);

            ddl.Items.Clear();

            ddl.Items.Add(
                new ListItem(
                    firstItem,
                    ""
                )
            );

            foreach (DataRow row in dt.Rows)
            {
                string value =
                    Convert.ToString(
                        row[field]
                    );

                ddl.Items.Add(
                    new ListItem(
                        value,
                        value
                    )
                );
            }

            if (ddl.Items.FindByValue(selected) != null)
            {
                ddl.SelectedValue = selected;
            }
        }


        private string GetCompanyID()
        {
            object value =
                DB().ExecuteScalar(
                    "SELECT TOP 1 CompanyID FROM CompanyMaster WHERE CompanyName=@Company OR CompanyAlias=@Company",
                    new SqlParameter[]
                    {
                        new SqlParameter(
                            "@Company",
                            ddlCompany.SelectedValue
                        )
                    }
                );

            if (value == null)
            {
                return "";
            }

            return Convert.ToString(value);
        }


        private bool IsHqOnlyCompany()
        {
            return
                ddlCompany.SelectedValue.Equals(
                    "BSPHCL",
                    StringComparison.OrdinalIgnoreCase
                )
                ||
                ddlCompany.SelectedValue.Equals(
                    "BSPGCL",
                    StringComparison.OrdinalIgnoreCase
                );
        }


        private void ClearList(
            DropDownList ddl,
            string firstItem)
        {
            ddl.Items.Clear();

            ddl.Items.Add(
                new ListItem(
                    firstItem,
                    ""
                )
            );
        }


        private void BindPostingPlaceOptions(
            string selected)
        {
            ddlPostingDetailPlace.Items.Clear();

            ddlPostingDetailPlace.Items.Add(
                new ListItem(
                    "Select Posting Place",
                    ""
                )
            );

            ddlPostingDetailPlace.Items.Add(
                new ListItem(
                    "HQ",
                    "HQ"
                )
            );

            if (!IsHqOnlyCompany())
            {
                ddlPostingDetailPlace.Items.Add(
                    new ListItem(
                        "Field Office",
                        "Field"
                    )
                );
            }

            if (!string.IsNullOrWhiteSpace(selected) &&
                ddlPostingDetailPlace.Items.FindByValue(
                    selected
                ) != null)
            {
                ddlPostingDetailPlace.SelectedValue =
                    selected;
            }

            if (IsHqOnlyCompany())
            {
                ddlPostingDetailPlace.SelectedValue =
                    "HQ";
            }
        }


        /*
         * ============================================================
         * DEPARTMENT / OFFICE / CELL
         *
         * SOURCE:
         * DepartmentMaster
         * ============================================================
         */

        private void BindDepartment(string selected)
        {
            ClearList(
                ddlPostingDepartment,
                "Select Department / Office / Cell"
            );

            DataTable dt =
                DB().GetDataTable(
                    "SELECT DISTINCT DepartmentName FROM DepartmentMaster WHERE ISNULL(DepartmentName,'')<>'' ORDER BY DepartmentName"
                );

            foreach (DataRow row in dt.Rows)
            {
                string department =
                    Convert.ToString(
                        row["DepartmentName"]
                    );

                ddlPostingDepartment.Items.Add(
                    new ListItem(
                        department,
                        department
                    )
                );
            }

            if (!string.IsNullOrWhiteSpace(selected) &&
                ddlPostingDepartment.Items.FindByValue(
                    selected
                ) != null)
            {
                ddlPostingDepartment.SelectedValue =
                    selected;
            }
        }


        /*
         * ============================================================
         * AREA BOARD / ZONE
         * ============================================================
         */

        private void BindZone(string selected)
        {
            ClearList(
                ddlAreaBoardZone,
                "Select Area Board / Zone"
            );

            string companyID =
                GetCompanyID();

            if (string.IsNullOrWhiteSpace(companyID))
            {
                return;
            }

            DataTable dt =
                DB().GetDataTable(
                    "SELECT ZoneID,ZoneName FROM ZoneMaster WHERE CompanyID=@CompanyID ORDER BY ZoneName",
                    new SqlParameter[]
                    {
                        new SqlParameter(
                            "@CompanyID",
                            companyID
                        )
                    }
                );

            foreach (DataRow row in dt.Rows)
            {
                ddlAreaBoardZone.Items.Add(
                    new ListItem(
                        Convert.ToString(
                            row["ZoneName"]
                        ),
                        Convert.ToString(
                            row["ZoneID"]
                        )
                    )
                );
            }

            SelectByText(
                ddlAreaBoardZone,
                selected
            );
        }


        /*
         * ============================================================
         * CIRCLE
         * ============================================================
         */

        private void BindCircle(string selected)
        {
            ClearList(
                ddlCircle,
                "Select Circle"
            );

            string companyID =
                GetCompanyID();

            int zoneID;

            if (string.IsNullOrWhiteSpace(companyID) ||
                !int.TryParse(
                    ddlAreaBoardZone.SelectedValue,
                    out zoneID
                ))
            {
                return;
            }

            DataTable dt =
                DB().GetDataTable(
                    "SELECT CircleID,CircleName FROM CircleMaster WHERE CompanyID=@CompanyID AND ZoneID=@ZoneID ORDER BY CircleName",
                    new SqlParameter[]
                    {
                        new SqlParameter(
                            "@CompanyID",
                            companyID
                        ),
                        new SqlParameter(
                            "@ZoneID",
                            zoneID
                        )
                    }
                );

            foreach (DataRow row in dt.Rows)
            {
                ddlCircle.Items.Add(
                    new ListItem(
                        Convert.ToString(
                            row["CircleName"]
                        ),
                        Convert.ToString(
                            row["CircleID"]
                        )
                    )
                );
            }

            SelectByText(
                ddlCircle,
                selected
            );
        }

        private string GetSelectedText(DropDownList ddl)
        {
            if (ddl == null)
            {
                return "";
            }

            if (ddl.SelectedIndex <= 0)
            {
                return "";
            }

            if (ddl.SelectedItem == null)
            {
                return "";
            }

            return ddl.SelectedItem.Text.Trim();
        }
        /*
         * ============================================================
         * DIVISION
         * ============================================================
         */

        private void BindDivision(string selected)
        {
            ClearList(
                ddlDivision,
                "Select Division"
            );

            string companyID =
                GetCompanyID();

            int zoneID;

            int circleID;

            if (string.IsNullOrWhiteSpace(companyID) ||
                !int.TryParse(
                    ddlAreaBoardZone.SelectedValue,
                    out zoneID
                ) ||
                !int.TryParse(
                    ddlCircle.SelectedValue,
                    out circleID
                ))
            {
                return;
            }

            DataTable dt =
                DB().GetDataTable(
                    "SELECT DivisionID,DivisionName FROM DivisionMaster WHERE CompanyID=@CompanyID AND ZoneID=@ZoneID AND CircleID=@CircleID ORDER BY DivisionName",
                    new SqlParameter[]
                    {
                        new SqlParameter(
                            "@CompanyID",
                            companyID
                        ),
                        new SqlParameter(
                            "@ZoneID",
                            zoneID
                        ),
                        new SqlParameter(
                            "@CircleID",
                            circleID
                        )
                    }
                );

            foreach (DataRow row in dt.Rows)
            {
                ddlDivision.Items.Add(
                    new ListItem(
                        Convert.ToString(
                            row["DivisionName"]
                        ),
                        Convert.ToString(
                            row["DivisionID"]
                        )
                    )
                );
            }

            SelectByText(
                ddlDivision,
                selected
            );
        }


        /*
         * ============================================================
         * SUBDIVISION
         * ============================================================
         */

        private void BindSubdivision(string selected)
        {
            ClearList(
                ddlSubdivision,
                "Select Subdivision"
            );

            string companyID =
                GetCompanyID();

            int zoneID;

            int circleID;

            int divisionID;

            if (string.IsNullOrWhiteSpace(companyID) ||
                !int.TryParse(
                    ddlAreaBoardZone.SelectedValue,
                    out zoneID
                ) ||
                !int.TryParse(
                    ddlCircle.SelectedValue,
                    out circleID
                ) ||
                !int.TryParse(
                    ddlDivision.SelectedValue,
                    out divisionID
                ))
            {
                return;
            }

            DataTable dt =
                DB().GetDataTable(
                    "SELECT SubdivisionID,SubdivisionName FROM SubdivisionMaster WHERE CompanyID=@CompanyID AND ZoneID=@ZoneID AND CircleID=@CircleID AND DivisionID=@DivisionID ORDER BY SubdivisionName",
                    new SqlParameter[]
                    {
                        new SqlParameter(
                            "@CompanyID",
                            companyID
                        ),
                        new SqlParameter(
                            "@ZoneID",
                            zoneID
                        ),
                        new SqlParameter(
                            "@CircleID",
                            circleID
                        ),
                        new SqlParameter(
                            "@DivisionID",
                            divisionID
                        )
                    }
                );

            foreach (DataRow row in dt.Rows)
            {
                ddlSubdivision.Items.Add(
                    new ListItem(
                        Convert.ToString(
                            row["SubdivisionName"]
                        ),
                        Convert.ToString(
                            row["SubdivisionID"]
                        )
                    )
                );
            }

            SelectByText(
                ddlSubdivision,
                selected
            );
        }


        /*
         * ============================================================
         * SECTION
         * ============================================================
         */

        private void BindSection(string selected)
        {
            ClearList(
                ddlSection,
                "Select Section"
            );

            string companyID =
                GetCompanyID();

            int zoneID;

            int circleID;

            int divisionID;

            int subdivisionID;

            if (string.IsNullOrWhiteSpace(companyID) ||
                !int.TryParse(
                    ddlAreaBoardZone.SelectedValue,
                    out zoneID
                ) ||
                !int.TryParse(
                    ddlCircle.SelectedValue,
                    out circleID
                ) ||
                !int.TryParse(
                    ddlDivision.SelectedValue,
                    out divisionID
                ) ||
                !int.TryParse(
                    ddlSubdivision.SelectedValue,
                    out subdivisionID
                ))
            {
                return;
            }

            DataTable dt =
                DB().GetDataTable(
                    "SELECT SectionID,SectionName FROM SectionMaster WHERE CompanyID=@CompanyID AND ZoneID=@ZoneID AND CircleID=@CircleID AND DivisionID=@DivisionID AND SubdivisionID=@SubdivisionID ORDER BY SectionName",
                    new SqlParameter[]
                    {
                        new SqlParameter(
                            "@CompanyID",
                            companyID
                        ),
                        new SqlParameter(
                            "@ZoneID",
                            zoneID
                        ),
                        new SqlParameter(
                            "@CircleID",
                            circleID
                        ),
                        new SqlParameter(
                            "@DivisionID",
                            divisionID
                        ),
                        new SqlParameter(
                            "@SubdivisionID",
                            subdivisionID
                        )
                    }
                );

            foreach (DataRow row in dt.Rows)
            {
                ddlSection.Items.Add(
                    new ListItem(
                        Convert.ToString(
                            row["SectionName"]
                        ),
                        Convert.ToString(
                            row["SectionID"]
                        )
                    )
                );
            }

            SelectByText(
                ddlSection,
                selected
            );
        }


        private void SelectByText(
            DropDownList ddl,
            string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            foreach (ListItem item in ddl.Items)
            {
                if (string.Equals(
                    item.Text,
                    text,
                    StringComparison.OrdinalIgnoreCase))
                {
                    ddl.SelectedValue =
                        item.Value;

                    return;
                }
            }
        }


        /*
         * ============================================================
         * LOAD EXISTING POSTING DETAILS
         * ============================================================
         */

        private void LoadPostingDetails()
        {
            DataTable dt =
                DB().GetDataTable(
                    "SELECT TOP 1 EmpPostingPlace,EmpPostingDepartment,AreaBoardZone,Circle,Division,Subdivision,Section FROM EmpPostingDetails WHERE EmpID=@EmpID ORDER BY ID DESC",
                    new SqlParameter[]
                    {
                        new SqlParameter(
                            "@EmpID",
                            hfEmpID.Value
                        )
                    }
                );

            string place = "";

            string department = "";

            string zone = "";

            string circle = "";

            string division = "";

            string subdivision = "";

            string section = "";


            if (dt.Rows.Count > 0)
            {
                DataRow row =
                    dt.Rows[0];

                place =
                    Convert.ToString(
                        row["EmpPostingPlace"]
                    );

                department =
                    Convert.ToString(
                        row["EmpPostingDepartment"]
                    );

                zone =
                    Convert.ToString(
                        row["AreaBoardZone"]
                    );

                circle =
                    Convert.ToString(
                        row["Circle"]
                    );

                division =
                    Convert.ToString(
                        row["Division"]
                    );

                subdivision =
                    Convert.ToString(
                        row["Subdivision"]
                    );

                section =
                    Convert.ToString(
                        row["Section"]
                    );
            }


            BindPostingPlaceOptions(
                place
            );

            BindDepartment(
                department
            );

            BindZone(
                zone
            );

            BindCircle(
                circle
            );

            BindDivision(
                division
            );

            BindSubdivision(
                subdivision
            );

            BindSection(
                section
            );

            SetMode();
        }


        /*
         * ============================================================
         * HQ / FIELD VISIBILITY
         * ============================================================
         */

        private void SetMode()
        {
            if (IsHqOnlyCompany())
            {
                if (ddlPostingDetailPlace.Items.FindByValue(
                    "Field"
                ) != null)
                {
                    ddlPostingDetailPlace.Items.Remove(
                        ddlPostingDetailPlace.Items.FindByValue(
                            "Field"
                        )
                    );
                }

                ddlPostingDetailPlace.SelectedValue =
                    "HQ";
            }


            bool isHQ =
                ddlPostingDetailPlace.SelectedValue == "HQ";


            bool isField =
                ddlPostingDetailPlace.SelectedValue == "Field";


            divPostingDepartment.Visible =
                isHQ;


            divAreaBoardZone.Visible =
                isField;


            divCircle.Visible =
                isField;


            divDivision.Visible =
                isField;


            divSubdivision.Visible =
                isField;


            divSection.Visible =
                isField;
        }


        /*
         * ============================================================
         * COMPANY CHANGE
         * ============================================================
         */

        protected void ddlCompany_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            BindPostingPlaceOptions("");

            BindDepartment("");

            BindZone("");

            ClearList(
                ddlCircle,
                "Select Circle"
            );

            ClearList(
                ddlDivision,
                "Select Division"
            );

            ClearList(
                ddlSubdivision,
                "Select Subdivision"
            );

            ClearList(
                ddlSection,
                "Select Section"
            );

            SetMode();

            LoadSelect2();
        }


        /*
         * ============================================================
         * POSTING PLACE CHANGE
         * ============================================================
         */

        protected void ddlPostingDetailPlace_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            SetMode();

            LoadSelect2();
        }


        /*
         * ============================================================
         * ZONE CHANGE
         * ============================================================
         */

        protected void ddlAreaBoardZone_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            BindCircle("");

            ClearList(
                ddlDivision,
                "Select Division"
            );

            ClearList(
                ddlSubdivision,
                "Select Subdivision"
            );

            ClearList(
                ddlSection,
                "Select Section"
            );

            LoadSelect2();
        }


        /*
         * ============================================================
         * CIRCLE CHANGE
         * ============================================================
         */

        protected void ddlCircle_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            BindDivision("");

            ClearList(
                ddlSubdivision,
                "Select Subdivision"
            );

            ClearList(
                ddlSection,
                "Select Section"
            );

            LoadSelect2();
        }


        /*
         * ============================================================
         * DIVISION CHANGE
         * ============================================================
         */

        protected void ddlDivision_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            BindSubdivision("");

            ClearList(
                ddlSection,
                "Select Section"
            );

            LoadSelect2();
        }


        /*
         * ============================================================
         * SUBDIVISION CHANGE
         * ============================================================
         */

        protected void ddlSubdivision_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            BindSection("");

            LoadSelect2();
        }


        /*
         * ============================================================
         * RELOAD SELECT2
         * ============================================================
         */

        private void LoadSelect2()
        {
            ClientScript.RegisterStartupScript(
                GetType(),
                "LoadSearchableDropdown",
                "LoadSearchableDropdown();",
                true
            );
        }


        /*
         * ============================================================
         * UPDATE EMPLOYEE
         * ============================================================
         */

        protected void btnUpdate_Click(
            object sender,
            EventArgs e)
        {
            /*
             * EMPLOYEE ID
             */
            if (string.IsNullOrWhiteSpace(
                hfEmpID.Value))
            {
                lblMessage.Text =
                    "Invalid Employee ID.";

                return;
            }


            /*
             * MOBILE
             */
            if (string.IsNullOrWhiteSpace(
                txtMobile.Text) ||
                !Regex.IsMatch(
                    txtMobile.Text.Trim(),
                    "^[0-9]{10}$"))
            {
                lblMessage.Text =
                    "Enter valid 10 digit mobile number.";

                return;
            }


            /*
             * POSTING PLACE
             */
            if (string.IsNullOrWhiteSpace(
                ddlPostingDetailPlace.SelectedValue))
            {
                lblMessage.Text =
                    "Select Posting Details Place.";

                LoadSelect2();

                return;
            }


            /*
             * BSPHCL / BSPGCL
             *
             * HQ ONLY
             */
            if (IsHqOnlyCompany() &&
                ddlPostingDetailPlace.SelectedValue != "HQ")
            {
                lblMessage.Text =
                    "BSPHCL and BSPGCL allow HQ posting only.";

                LoadSelect2();

                return;
            }


            /*
             * HQ
             *
             * Department is mandatory.
             */
            if (ddlPostingDetailPlace.SelectedValue == "HQ")
            {
                if (string.IsNullOrWhiteSpace(
                    ddlPostingDepartment.SelectedValue))
                {
                    lblMessage.Text =
                        "Select Department / Office / Cell.";

                    LoadSelect2();

                    return;
                }
            }


            /*
             * FIELD OFFICE
             *
             * Area Board / Zone is mandatory.
             *
             * Circle is optional.
             *
             * Division is optional.
             *
             * Subdivision is optional.
             *
             * Section is optional.
             *
             * But lower level cannot be selected
             * without its parent.
             */
            if (ddlPostingDetailPlace.SelectedValue == "Field")
            {
                /*
                 * Minimum level
                 */
                if (string.IsNullOrWhiteSpace(
                    ddlAreaBoardZone.SelectedValue))
                {
                    lblMessage.Text =
                        "Select Area Board / Zone.";

                    LoadSelect2();

                    return;
                }


                /*
                 * Division requires Circle
                 */
                if (!string.IsNullOrWhiteSpace(
                    ddlDivision.SelectedValue) &&
                    string.IsNullOrWhiteSpace(
                    ddlCircle.SelectedValue))
                {
                    lblMessage.Text =
                        "Select Circle before selecting Division.";

                    LoadSelect2();

                    return;
                }


                /*
                 * Subdivision requires Division
                 */
                if (!string.IsNullOrWhiteSpace(
                    ddlSubdivision.SelectedValue) &&
                    string.IsNullOrWhiteSpace(
                    ddlDivision.SelectedValue))
                {
                    lblMessage.Text =
                        "Select Division before selecting Subdivision.";

                    LoadSelect2();

                    return;
                }


                /*
                 * Section requires Subdivision
                 */
                if (!string.IsNullOrWhiteSpace(
                    ddlSection.SelectedValue) &&
                    string.IsNullOrWhiteSpace(
                    ddlSubdivision.SelectedValue))
                {
                    lblMessage.Text =
                        "Select Subdivision before selecting Section.";

                    LoadSelect2();

                    return;
                }
            }


            clsDataAccess db =
                DB();


            try
            {
                db.BeginTransaction();


                /*
                 * ====================================================
                 * UPDATE EmpBasicMaster
                 * ====================================================
                 */

                db.ExecuteSql(
                    "UPDATE EmpBasicMaster SET MobileNo=@MobileNo,EmailId=@EmailId,EmpCompany=@EmpCompany,EmpDesignation=@EmpDesignation,EmpPostingPlace=@EmpPostingPlace WHERE EmpID=@EmpID",
                    new SqlParameter[]
                    {
                        new SqlParameter(
                            "@MobileNo",
                            txtMobile.Text.Trim()
                        ),

                        new SqlParameter(
                            "@EmailId",
                            txtEmail.Text.Trim()
                        ),

                        new SqlParameter(
                            "@EmpCompany",
                            ddlCompany.SelectedValue
                        ),

                        new SqlParameter(
                            "@EmpDesignation",
                            ddlDesignation.SelectedValue
                        ),

                        new SqlParameter(
                            "@EmpPostingPlace",
                            ddlPostingPlace.SelectedValue
                        ),

                        new SqlParameter(
                            "@EmpID",
                            hfEmpID.Value
                        )
                    },
                    db.Transaction
                );


                string department = "";

                string zone = "";

                string circle = "";

                string division = "";

                string subdivision = "";

                string section = "";

                if (ddlPostingDetailPlace.SelectedValue == "HQ")
                {
                    department =
                        GetSelectedText(ddlPostingDepartment);
                }

                if (ddlPostingDetailPlace.SelectedValue == "Field")
                {
                    zone =
                        GetSelectedText(ddlAreaBoardZone);

                    circle =
                        GetSelectedText(ddlCircle);

                    division =
                        GetSelectedText(ddlDivision);

                    subdivision =
                        GetSelectedText(ddlSubdivision);

                    section =
                        GetSelectedText(ddlSection);
                }


                /*
                 * ====================================================
                 * CHECK EXISTING POSTING DETAILS
                 * ====================================================
                 */

                int count =
                    Convert.ToInt32(
                        db.ExecuteScalar(
                            "SELECT COUNT(*) FROM EmpPostingDetails WHERE EmpID=@EmpID",
                            new SqlParameter[]
                            {
                                new SqlParameter(
                                    "@EmpID",
                                    hfEmpID.Value
                                )
                            },
                            db.Transaction
                        )
                    );


                /*
                 * ====================================================
                 * UPDATE EXISTING POSTING DETAILS
                 * ====================================================
                 */

                if (count > 0)
                {
                    db.ExecuteSql(
                        "UPDATE EmpPostingDetails SET EmpPostingPlace=@Place,EmpPostingDepartment=@Dept,AreaBoardZone=@Zone,Circle=@Circle,Division=@Division,Subdivision=@Subdivision,Section=@Section,CreatedOn=GETDATE(),CreatedBy=@CreatedBy WHERE ID=(SELECT TOP 1 ID FROM EmpPostingDetails WHERE EmpID=@EmpID ORDER BY ID DESC)",
                        new SqlParameter[]
                        {
                            new SqlParameter(
                                "@Place",
                                ddlPostingDetailPlace.SelectedValue
                            ),

                            new SqlParameter(
                                "@Dept",
                                department
                            ),

                            new SqlParameter(
                                "@Zone",
                                zone
                            ),

                            new SqlParameter(
                                "@Circle",
                                circle
                            ),

                            new SqlParameter(
                                "@Division",
                                division
                            ),

                            new SqlParameter(
                                "@Subdivision",
                                subdivision
                            ),

                            new SqlParameter(
                                "@Section",
                                section
                            ),

                            new SqlParameter(
                                "@CreatedBy",
                                "Admin"
                            ),

                            new SqlParameter(
                                "@EmpID",
                                hfEmpID.Value
                            )
                        },
                        db.Transaction
                    );
                }


                /*
                 * ====================================================
                 * INSERT NEW POSTING DETAILS
                 * ====================================================
                 */

                else
                {
                    db.ExecuteSql(
                        "INSERT INTO EmpPostingDetails(EmpID,EmpPostingPlace,EmpPostingDepartment,AreaBoardZone,Circle,Division,Subdivision,Section,EmpOnDeputation,CreatedOn,CreatedBy,AssessmentYear) VALUES(@EmpID,@Place,@Dept,@Zone,@Circle,@Division,@Subdivision,@Section,@Deputation,GETDATE(),@CreatedBy,@AssessmentYear)",
                        new SqlParameter[]
                        {
                            new SqlParameter(
                                "@EmpID",
                                hfEmpID.Value
                            ),

                            new SqlParameter(
                                "@Place",
                                ddlPostingDetailPlace.SelectedValue
                            ),

                            new SqlParameter(
                                "@Dept",
                                department
                            ),

                            new SqlParameter(
                                "@Zone",
                                zone
                            ),

                            new SqlParameter(
                                "@Circle",
                                circle
                            ),

                            new SqlParameter(
                                "@Division",
                                division
                            ),

                            new SqlParameter(
                                "@Subdivision",
                                subdivision
                            ),

                            new SqlParameter(
                                "@Section",
                                section
                            ),

                            new SqlParameter(
                                "@Deputation",
                                "NO"
                            ),

                            new SqlParameter(
                                "@CreatedBy",
                                "Admin"
                            ),

                            new SqlParameter(
                                "@AssessmentYear",
                                AssessmentYear()
                            )
                        },
                        db.Transaction
                    );
                }


                /*
                 * ====================================================
                 * COMMIT
                 * ====================================================
                 */

                db.Commit();


                lblMessage.ForeColor =
                    Color.Green;

                lblMessage.Text =
                    "Employee and Posting Details updated successfully.";


                LoadSelect2();
            }
            catch (Exception ex)
            {
                try
                {
                    db.Rollback();
                }
                catch
                {
                }


                lblMessage.ForeColor =
                    Color.Red;

                lblMessage.Text =
                    ex.Message;
            }
        }


        /*
         * ============================================================
         * ASSESSMENT YEAR
         * ============================================================
         */

        private string AssessmentYear()
        {
            DateTime now =
                DateTime.Now;

            int year =
                now.Month >= 4
                    ? now.Year
                    : now.Year - 1;

            return
                year +
                "-" +
                (year + 1)
                .ToString()
                .Substring(2);
        }
    }
}