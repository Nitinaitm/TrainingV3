using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Manager
{
    public partial class AttendanceReport : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        private string ManagerID
        {
            get
            {
                if (Session["ManagerID"] != null && !string.IsNullOrWhiteSpace(Session["ManagerID"].ToString()))
                    return Session["ManagerID"].ToString().Trim();

                if (Session["Role"] != null && Session["Role"].ToString() == "Manager" && Session["UserID"] != null)
                    return Session["UserID"].ToString().Trim();

                return "";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["SessionID"] == null || string.IsNullOrWhiteSpace(Session["SessionID"].ToString()))
            {
                Response.Redirect("~/Manager/SessionReport.aspx");
                return;
            }

            if (string.IsNullOrWhiteSpace(ManagerID))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindTrainings();

                if (Session["SessionID"] != null && ddlTraining.Items.FindByValue(Session["SessionID"].ToString()) != null)
                {
                    ddlTraining.SelectedValue = Session["SessionID"].ToString();
                }

                BindGrid();
            }
        }

        private void BindTrainings()
        {
            string query = "SELECT DISTINCT SM.SessionID,TD.TrainingID + ' | Batch ' + ISNULL(TD.Batch,'') + ' | Session ' + ISNULL(SM.SessionNo,'') AS TrainingName FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID INNER JOIN TrainingLocationMaster L ON TD.TrainingLocation=L.TrainingLocation WHERE L.TrainingLocationID=(SELECT TOP 1 M.TrainingLocationID FROM ManagerMaster M WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y') AND EXISTS (SELECT 1 FROM ManagerMaster M WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND M.TrainingLocationID=L.TrainingLocationID) ORDER BY TD.TrainingID DESC,SM.SessionID DESC";
            DataTable dt = obj.GetDataTable(query,new SqlParameter[] { new SqlParameter("@ManagerID",ManagerID) });
            ddlTraining.DataSource = dt;
            ddlTraining.DataTextField = "TrainingName";
            ddlTraining.DataValueField = "SessionID";
            ddlTraining.DataBind();
            ddlTraining.Items.Insert(0,new ListItem("-- All Trainings --",""));
        }

        protected void ddlTraining_SelectedIndexChanged(object sender,EventArgs e)
        {
            BindGrid();
        }

        protected void btnGenerate_Click(object sender,EventArgs e)
        {
            BindGrid();
        }

        private void BindGrid()
        {
            string query = "SELECT E.EmpID,E.EmpName,COUNT(DISTINCT S.SessionID) AS TotalSessions,COUNT(DISTINCT CASE WHEN SA.AttendanceStatus='Present' THEN S.SessionID END) AS Present,COUNT(DISTINCT CASE WHEN SA.AttendanceStatus='Absent' THEN S.SessionID END) AS Absent,CASE WHEN COUNT(DISTINCT S.SessionID)=0 THEN 0 ELSE CAST(COUNT(DISTINCT CASE WHEN SA.AttendanceStatus='Present' THEN S.SessionID END)*100.0/COUNT(DISTINCT S.SessionID) AS DECIMAL(10,2)) END AS Percentage FROM EmpBasicMaster E INNER JOIN TrainingAssignment TA ON E.EmpID=TA.EmpID AND TA.AssignmentStatus='Assigned' INNER JOIN SessionMaster S ON TA.TrainingID=S.TrainingID INNER JOIN TrainingDetails TD ON S.TrainingID=TD.TrainingID INNER JOIN TrainingLocationMaster L ON TD.TrainingLocation=L.TrainingLocation INNER JOIN ManagerMaster M ON M.TrainingLocationID=L.TrainingLocationID LEFT JOIN SessionAttendance SA ON S.SessionID=SA.SessionID AND SA.EmpID=E.EmpID WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND M.TrainingLocationID=L.TrainingLocationID";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@ManagerID",ManagerID));

            if(!string.IsNullOrEmpty(ddlTraining.SelectedValue))
            {
                query += " AND S.SessionID=@SessionID";
                parameters.Add(new SqlParameter("@SessionID",ddlTraining.SelectedValue));
            }

            if(!string.IsNullOrEmpty(txtFrom.Text.Trim()))
            {
                query += " AND TRY_CONVERT(date,S.SessionDate,105)>=TRY_CONVERT(date,@From,23)";
                parameters.Add(new SqlParameter("@From",txtFrom.Text.Trim()));
            }

            if(!string.IsNullOrEmpty(txtTo.Text.Trim()))
            {
                query += " AND TRY_CONVERT(date,S.SessionDate,105)<=TRY_CONVERT(date,@To,23)";
                parameters.Add(new SqlParameter("@To",txtTo.Text.Trim()));
            }

            query += " GROUP BY E.EmpID,E.EmpName ORDER BY Percentage DESC,E.EmpID";
            DataTable dt = obj.GetDataTable(query,parameters.ToArray());
            gvReport.DataSource = dt;
            gvReport.DataBind();
        }

        protected void btnExportPDF_Click(object sender,EventArgs e)
        {
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition","attachment;filename=AttendanceReport.pdf");
            Response.Write("Attendance Report - " + DateTime.Now.ToString("dd-MM-yyyy"));
            Response.End();
        }

        protected void btnExportExcel_Click(object sender,EventArgs e)
        {
            BindGrid();
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition","attachment;filename=AttendanceReport.xls");
            Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            gvReport.RenderControl(hw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
        }
    }
}