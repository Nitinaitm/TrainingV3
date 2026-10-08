using System;
using System.Web.UI;
using System.Data;
using System.Data.SqlClient;

namespace Training
{
    public partial class TraineeMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string role = Convert.ToString(Session["Role"]);
            if (Session["EmpID"] == null || !role.Equals("Trainee", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadTraineeInfo();
                LoadHeaderCounts();
            }

            //try
            //{
            //    Control lifecycle = LoadControl("~/BatchLifecycle.ascx");
            //    ContentPlaceHolder1.Controls.Add(lifecycle);
            //}
            //catch { }
        }

        private void LoadTraineeInfo()
        {
            string empID=Session["EmpID"].ToString().Trim();
            DataTable dt=new clsDataAccess().GetDataTable("SELECT TOP 1 EmpID,EmpName,EmpDesignation FROM EmpBasicMaster WHERE LTRIM(RTRIM(EmpID))=LTRIM(RTRIM(@EmpID))",new SqlParameter[] { new SqlParameter("@EmpID",empID) });
            if(dt.Rows.Count==0)return;
            lblTraineeID.Text=dt.Rows[0]["EmpID"].ToString();
            lblTraineeName.Text=dt.Rows[0]["EmpName"].ToString();
            lblDesignation.Text=dt.Rows[0]["EmpDesignation"].ToString();
        }

        private void LoadHeaderCounts()
        {
            string empID=Session["EmpID"].ToString().Trim();
            clsDataAccess obj=new clsDataAccess();
            string aq="SELECT COUNT(DISTINCT A.AnnouncementID) FROM Announcement A WHERE A.IsActive=1 AND (EXISTS (SELECT 1 FROM TrainingAssignment TA INNER JOIN SessionMaster SM ON TA.TrainingID=SM.TrainingID WHERE LTRIM(RTRIM(TA.EmpID))=LTRIM(RTRIM(@EmpID)) AND SM.TrainerID=A.TrainerID) OR (A.TrainerID LIKE 'MANAGER:%' AND EXISTS (SELECT 1 FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TA.TrainingID=TD.TrainingID INNER JOIN TrainingLocationMaster L ON TD.TrainingLocation=L.TrainingLocation INNER JOIN ManagerMaster M ON M.TrainingLocationID=L.TrainingLocationID WHERE LTRIM(RTRIM(TA.EmpID))=LTRIM(RTRIM(@EmpID)) AND M.ManagerID=SUBSTRING(A.TrainerID,9,100) AND ISNULL(M.ActiveStatus,'Y')='Y')))";
            string nq="SELECT COUNT(DISTINCT N.NotificationID) FROM Notification N WHERE (N.TrainerID LIKE 'MANAGER:%' AND EXISTS (SELECT 1 FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TA.TrainingID=TD.TrainingID INNER JOIN TrainingLocationMaster L ON TD.TrainingLocation=L.TrainingLocation INNER JOIN ManagerMaster M ON M.TrainingLocationID=L.TrainingLocationID WHERE LTRIM(RTRIM(TA.EmpID))=LTRIM(RTRIM(@EmpID)) AND M.ManagerID=SUBSTRING(N.TrainerID,9,100) AND ISNULL(M.ActiveStatus,'Y')='Y')) OR (N.TrainerID NOT LIKE 'MANAGER:%' AND EXISTS (SELECT 1 FROM TrainingAssignment TA INNER JOIN SessionMaster SM ON TA.TrainingID=SM.TrainingID WHERE LTRIM(RTRIM(TA.EmpID))=LTRIM(RTRIM(@EmpID)) AND SM.TrainerID=N.TrainerID))";
            lblAnnouncementCount.Text=Convert.ToString(obj.ExecuteScalar(aq,new SqlParameter[] { new SqlParameter("@EmpID",empID) }));
            lblNotificationCount.Text=Convert.ToString(obj.ExecuteScalar(nq,new SqlParameter[] { new SqlParameter("@EmpID",empID) }));
        }
    }
}