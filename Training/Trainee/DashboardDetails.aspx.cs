using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainee
{
    public partial class DashboardDetails : System.Web.UI.Page
    {
        private readonly clsDataAccess objDB = new clsDataAccess();
        private string EmpID = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["EmpID"] == null || string.IsNullOrWhiteSpace(Session["EmpID"].ToString()) || Session["Role"] == null || !string.Equals(Session["Role"].ToString(), "Trainee", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }
            EmpID = Session["EmpID"].ToString().Trim().ToUpperInvariant();
            if (!IsPostBack)
            {
                string type = Session["TraineeDashboardType"] == null ? "Active" : Session["TraineeDashboardType"].ToString().Trim();
                LoadDetails(type);
            }
        }

        private void LoadDetails(string type)
        {
            DataTable dt;
            string title;
            switch(type)
            {
                case "Active": title="Active Training"; dt=GetTrainingByCategory("Active"); break;
                case "Previous": title="Previous Training - Not Closed"; dt=GetTrainingByCategory("Previous"); break;
                case "Completed": title="Completed Training"; dt=GetTrainingByCategory("Completed"); break;
                case "Future": title="Future Assigned Training"; dt=GetTrainingByCategory("Future"); break;
                case "Attendance": title="Attendance Completed"; dt=GetAttendanceCompleted(); break;
                case "PendingTests": title="Pending Tests"; dt=GetPublishedTests(true); break;
                case "Certificates": title="All Certificates"; dt=GetCertificates(); gvDetails.DataKeyNames=new string[] { "CertificateID" }; break;
                case "Tests": title="Published Tests"; dt=GetPublishedTests(false); break;
                default: title="Active Training"; dt=GetTrainingByCategory("Active"); break;
            }
            lblTitle.Text=title;
            lblSummary.Text="Showing "+dt.Rows.Count.ToString()+" record(s).";
            gvDetails.DataSource=dt;
            gvDetails.DataBind();
        }

        private DataTable GetTrainingByCategory(string category)
        {
            string condition=category=="Completed" ? "AND ISNULL(TD.TrainingStatus,'') IN ('Closed','Completed')" : category=="Future" ? "AND ISNULL(TD.TrainingStatus,'') NOT IN ('Closed','Completed') AND TRY_CONVERT(date,TD.DateFrom,105)>CONVERT(date,GETDATE())" : category=="Previous" ? "AND ISNULL(TD.TrainingStatus,'') NOT IN ('Closed','Completed') AND TRY_CONVERT(date,TD.DateTo,105)<CONVERT(date,GETDATE())" : "AND ISNULL(TD.TrainingStatus,'') NOT IN ('Closed','Completed') AND TRY_CONVERT(date,TD.DateFrom,105)<=CONVERT(date,GETDATE()) AND TRY_CONVERT(date,TD.DateTo,105)>=CONVERT(date,GETDATE())";
            string sql="SELECT DISTINCT TD.TrainingID,CM.CourseName,TD.TrainingType,TD.TrainingOrganizer,TD.TrainingLocation,TD.Batch,TRY_CONVERT(date,TD.DateFrom,105) AS DateFrom,TRY_CONVERT(date,TD.DateTo,105) AS DateTo,ISNULL(TD.TrainingStatus,'') AS TrainingStatus FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TD.TrainingID=TA.TrainingID INNER JOIN CourseMaster CM ON CM.CourseID=TD.CourseID WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' "+condition+" ORDER BY TRY_CONVERT(date,TD.DateFrom,105) DESC";
            return GetTable(sql);
        }

        private DataTable GetAttendanceCompleted()
        {
            string sql="SELECT DISTINCT TD.TrainingID,CM.CourseName,TD.TrainingType,TD.TrainingOrganizer,TD.Batch,TRY_CONVERT(date,TD.DateFrom,105) AS DateFrom,TRY_CONVERT(date,TD.DateTo,105) AS DateTo FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TD.TrainingID=TA.TrainingID INNER JOIN CourseMaster CM ON CM.CourseID=TD.CourseID WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND TD.AttendanceRequired=1 AND EXISTS (SELECT 1 FROM SessionMaster SM WHERE SM.TrainingID=TA.TrainingID AND ISNULL(SM.AttendanceSkipped,0)=0) AND NOT EXISTS (SELECT 1 FROM SessionMaster SM WHERE SM.TrainingID=TA.TrainingID AND ISNULL(SM.AttendanceSkipped,0)=0 AND NOT EXISTS (SELECT 1 FROM SessionAttendance SA WHERE SA.SessionID=SM.SessionID AND SA.EmpID=@EmpID AND SA.AttendanceStatus IN ('Present','Completed'))) ORDER BY TRY_CONVERT(date,TD.DateFrom,105) DESC";
            return GetTable(sql);
        }

        private DataTable GetPublishedTests(bool pendingOnly)
        {
            string completionCondition="EXISTS (SELECT 1 FROM TestAttempt TA2 WHERE TA2.TestID=TM.TestID AND TA2.EmpID=@EmpID AND TA2.Submitted=1)";
            string whereCompletion=pendingOnly ? "AND NOT "+completionCondition : "";
            string sql="SELECT TM.TestID,SM.TrainingID,CM.CourseName,SM.SessionNo,SM.SessionName,TM.TestType,TM.IsPublished,CASE WHEN "+completionCondition+" THEN 'Completed' ELSE 'Pending' END AS TestStatus FROM TestMaster TM INNER JOIN SessionMaster SM ON SM.SessionID=TM.SessionID INNER JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID INNER JOIN TrainingAssignment TAA ON TAA.TrainingID=SM.TrainingID INNER JOIN CourseMaster CM ON CM.CourseID=TD.CourseID WHERE TAA.EmpID=@EmpID AND TAA.AssignmentStatus='Assigned' AND TM.IsPublished=1 AND ((TM.TestType='Pre' AND TD.InitialAssessmentRequired=1 AND ISNULL(SM.PreAssessmentSkipped,0)=0) OR (TM.TestType='Post' AND TD.FinalAssessmentRequired=1 AND ISNULL(SM.PostAssessmentSkipped,0)=0)) "+whereCompletion+" GROUP BY TM.TestID,SM.TrainingID,CM.CourseName,SM.SessionNo,SM.SessionName,TM.TestType,TM.IsPublished ORDER BY SM.TrainingID,SM.SessionNo,TM.TestType";
            return GetTable(sql);
        }

        private DataTable GetCertificates()
        {
            string sql="SELECT DISTINCT TC.CertificateID,TC.CertificateNo,TC.TrainingID,CM.CourseName,TC.GeneratedOn,TD.TrainingType,TD.TrainingOrganizer,TD.Batch,TRY_CONVERT(date,TD.DateFrom,105) AS DateFrom,TRY_CONVERT(date,TD.DateTo,105) AS DateTo,ISNULL(TD.TrainingStatus,'') AS TrainingStatus FROM TrainingCertificate TC INNER JOIN TrainingAssignment TA ON TA.TrainingID=TC.TrainingID AND TA.EmpID=TC.EmpID INNER JOIN TrainingDetails TD ON TD.TrainingID=TC.TrainingID INNER JOIN CourseMaster CM ON CM.CourseID=TD.CourseID WHERE TC.EmpID=@EmpID AND TC.CertificateStatus='A' AND TA.AssignmentStatus='Assigned' ORDER BY TC.GeneratedOn DESC";
            return GetTable(sql);
        }

        protected void gvDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string type=Session["TraineeDashboardType"] == null ? "" : Session["TraineeDashboardType"].ToString().Trim();
            if (!string.Equals(type,"Certificates",StringComparison.OrdinalIgnoreCase)) return;
            if (e.Row.RowType==DataControlRowType.Header)
            {
                TableCell cell=new TableCell();
                cell.Text="Action";
                cell.CssClass="text-center";
                e.Row.Cells.Add(cell);
                return;
            }
            if (e.Row.RowType!=DataControlRowType.DataRow) return;
            string certificateID=DataBinder.Eval(e.Row.DataItem,"CertificateID")==null ? "" : DataBinder.Eval(e.Row.DataItem,"CertificateID").ToString();
            TableCell actionCell=new TableCell();
            actionCell.CssClass="text-center";
            if (!string.IsNullOrWhiteSpace(certificateID))
            {
                LinkButton btn=new LinkButton();
                btn.ID="btnDownloadCertificate";
                btn.CommandName="DownloadCertificate";
                btn.CommandArgument=certificateID;
                btn.CssClass="btn btn-sm btn-success";
                btn.CausesValidation=false;
                btn.Text="Download PDF";
                actionCell.Controls.Add(btn);
            }
            e.Row.Cells.Add(actionCell);
        }

        protected void gvDetails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName!="DownloadCertificate") return;
            string certificateID=Convert.ToString(e.CommandArgument);
            if (string.IsNullOrWhiteSpace(certificateID)) return;
            DataTable dt=objDB.GetDataTable("SELECT PDFPath,PDFName FROM TrainingCertificate WHERE CertificateID=@CertificateID AND EmpID=@EmpID AND CertificateStatus='A'",new SqlParameter[] { new SqlParameter("@CertificateID",certificateID),new SqlParameter("@EmpID",EmpID) });
            if (dt.Rows.Count==0) return;
            string pdfPath=Convert.ToString(dt.Rows[0]["PDFPath"]);
            if (string.IsNullOrWhiteSpace(pdfPath)) return;
            string physicalPath=Server.MapPath(pdfPath);
            if (!File.Exists(physicalPath)) return;
            string pdfName=Convert.ToString(dt.Rows[0]["PDFName"]);
            if (string.IsNullOrWhiteSpace(pdfName)) pdfName=Path.GetFileName(physicalPath);
            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.ContentType="application/pdf";
            Response.AddHeader("Content-Disposition","attachment; filename=\""+pdfName+"\"");
            Response.AddHeader("Content-Length",new FileInfo(physicalPath).Length.ToString());
            Response.TransmitFile(physicalPath);
            Response.Flush();
            System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        private DataTable GetTable(string sql)
        {
            return objDB.GetDataTable(sql,new SqlParameter[] { new SqlParameter("@EmpID",EmpID) });
        }
    }
}
