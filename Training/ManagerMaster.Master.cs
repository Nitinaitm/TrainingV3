using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Training
{
    public partial class ManagerMaster : MasterPage
    {
        clsDataAccess obj = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Manager" || Session["ManagerID"] == null)
            {
                Response.Redirect("~/Default.aspx");
                string role = Convert.ToString(Session["Role"]);
              

              
            }
        }
           
        }
    }