using System;
using System.Data;
using System.Data.SqlClient;

namespace Training.Manager
{
    public partial class Profile : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ManagerID"] == null || string.IsNullOrWhiteSpace(Session["ManagerID"].ToString()))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadProfile();
            }
        }

        private void LoadProfile()
        {
            DataTable dt = obj.GetDataTable(
                "SELECT TOP 1 M.ManagerID,M.EmpID,E.EmpName,E.DOB,E.DOJ,E.MobileNo,E.EmailId,E.EmpDesignation FROM ManagerMaster M INNER JOIN EmpBasicMaster E ON M.EmpID=E.EmpID WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y'",
                new SqlParameter[] { new SqlParameter("@ManagerID", Session["ManagerID"].ToString().Trim()) });

            if (dt.Rows.Count == 0)
            {
                return;
            }

            DataRow row = dt.Rows[0];

            lblManagerID.Text = row["ManagerID"].ToString();
            lblEmpID.Text = row["EmpID"].ToString();
            lblName.Text = row["EmpName"].ToString();
            lblDOB.Text = row["DOB"].ToString();
            lblDOJ.Text = row["DOJ"].ToString();
            lblMobile.Text = row["MobileNo"].ToString();
            lblEmail.Text = row["EmailId"].ToString();
            lblDesignation.Text = row["EmpDesignation"].ToString();
        }
    }
}