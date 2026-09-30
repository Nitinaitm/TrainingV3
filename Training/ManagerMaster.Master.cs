using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Training
{
    public partial class ManagerMaster : MasterPage
    {
        clsDataAccess obj=new clsDataAccess();

        protected void Page_Load(object sender,EventArgs e)
        {
            if(Session["Role"]==null||Session["Role"].ToString()!="Manager"||Session["ManagerID"]==null)
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if(!IsPostBack)
            {
                LoadManagerProfile();
            }
        }

        private void LoadManagerProfile()
        {
            string managerID=Session["ManagerID"].ToString().Trim();
            DataTable dt=obj.GetDataTable("SELECT TOP 1 M.ManagerID,E.EmpName,E.EmpDesignation FROM ManagerMaster M INNER JOIN EmpBasicMaster E ON M.EmpID=E.EmpID WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y'",new SqlParameter[]{new SqlParameter("@ManagerID",managerID)});
            if(dt.Rows.Count==0)return;
            lblManagerID.Text=dt.Rows[0]["ManagerID"].ToString();
            lblManagerName.Text=dt.Rows[0]["EmpName"].ToString();
            lblManagerDesignation.Text=dt.Rows[0]["EmpDesignation"].ToString();
        }
    }
}