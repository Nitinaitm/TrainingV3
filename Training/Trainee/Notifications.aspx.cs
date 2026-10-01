using System;
using System.Data;
using System.Data.SqlClient;

namespace Training.Trainee
{
    public partial class Notifications : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["EmpID"] == null || Session["Role"] == null || !string.Equals(Session["Role"].ToString(), "Trainee", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid()
        {
            string empID = Session["EmpID"].ToString().Trim();

            string query = "SELECT DISTINCT N.NotificationID,N.Message,N.CreatedOn FROM Notification N WHERE (N.TrainerID LIKE 'MANAGER:%' AND EXISTS (SELECT 1 FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TA.TrainingID=TD.TrainingID INNER JOIN TrainingLocationMaster L ON TD.TrainingLocation=L.TrainingLocation INNER JOIN ManagerMaster M ON M.TrainingLocationID=L.TrainingLocationID WHERE LTRIM(RTRIM(TA.EmpID))=LTRIM(RTRIM(@EmpID)) AND M.ManagerID=SUBSTRING(N.TrainerID,9,100) AND ISNULL(M.ActiveStatus,'Y')='Y')) OR (N.TrainerID NOT LIKE 'MANAGER:%' AND EXISTS (SELECT 1 FROM TrainingAssignment TA INNER JOIN SessionMaster SM ON TA.TrainingID=SM.TrainingID WHERE LTRIM(RTRIM(TA.EmpID))=LTRIM(RTRIM(@EmpID)) AND SM.TrainerID=N.TrainerID)) ORDER BY N.CreatedOn DESC";

            DataTable dt=obj.GetDataTable(query,new SqlParameter[] { new SqlParameter("@EmpID",empID) });

            dt.Columns.Add("HasPdf",typeof(bool));

            foreach(DataRow row in dt.Rows)
            {
                row["HasPdf"]=System.IO.File.Exists(Server.MapPath("~/Uploads/Notifications/"+row["NotificationID"]+".pdf"));
            }

            gvNotifications.DataSource=dt;
            gvNotifications.DataBind();
        }
    }
}