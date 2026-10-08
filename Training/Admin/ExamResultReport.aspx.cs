using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class ExamResultReport : Page
    {
        clsDataAccess objDB=new clsDataAccess();

        protected void Page_Load(object sender,EventArgs e)
        {
            if(!IsPostBack)
            {
                BindCourseFilter();
                BindStatusFilter();
                BindTrainingList();
                ShowLevel(1);
            }
        }

        private void BindCourseFilter()
        {
            string q="SELECT CourseID,CourseName FROM CourseMaster WHERE ISNULL(CourseName,'')<>'' ORDER BY CourseName";
            DataTable dt=objDB.GetDataTable(q);
            ddlCourseFilter.DataSource=dt;
            ddlCourseFilter.DataTextField="CourseName";
            ddlCourseFilter.DataValueField="CourseID";
            ddlCourseFilter.DataBind();
            ddlCourseFilter.Items.Insert(0,new ListItem("All Courses",""));
        }

        private void BindStatusFilter()
        {
            ddlStatusFilter.Items.Clear();
            ddlStatusFilter.Items.Add(new ListItem("All Status",""));
            ddlStatusFilter.Items.Add(new ListItem("Completed","Closed"));
            ddlStatusFilter.Items.Add(new ListItem("Completed","Completed"));
            ddlStatusFilter.Items.Add(new ListItem("Ongoing","InProgress"));
            ddlStatusFilter.Items.Add(new ListItem("Future","Future"));
        }

        private void BindTrainingList()
        {
            string q="SELECT TD.TrainingID,ISNULL(CM.CourseName,'') CourseName,ISNULL(TD.Batch,'') Batch,ISNULL(TD.TrainingType,'') TrainingType,ISNULL(TD.TrainingStatus,'') TrainingStatus,CONVERT(varchar(10),COALESCE(TRY_CONVERT(date,TD.DateFrom,105),TRY_CONVERT(date,TD.DateFrom,23),TRY_CONVERT(date,TD.DateFrom)),105) DateFrom,CONVERT(varchar(10),COALESCE(TRY_CONVERT(date,TD.DateTo,105),TRY_CONVERT(date,TD.DateTo,23),TRY_CONVERT(date,TD.DateTo)),105) DateTo,(SELECT COUNT(*) FROM SessionMaster SM WHERE SM.TrainingID=TD.TrainingID AND ISNULL(SM.SessionCancelled,0)=0) SessionCount,(SELECT COUNT(DISTINCT TA.EmpID) FROM TrainingAssignment TA WHERE TA.TrainingID=TD.TrainingID AND ISNULL(TA.Cancelled,0)=0) TraineeCount,(SELECT COUNT(*) FROM TestResult TR INNER JOIN TestMaster TM ON TR.TestID=TM.TestID INNER JOIN SessionMaster SM ON TM.SessionID=SM.SessionID WHERE SM.TrainingID=TD.TrainingID AND TM.TestType='Pre') PreResultCount,(SELECT COUNT(*) FROM TestResult TR INNER JOIN TestMaster TM ON TR.TestID=TM.TestID INNER JOIN SessionMaster SM ON TM.SessionID=SM.SessionID WHERE SM.TrainingID=TD.TrainingID AND TM.TestType='Post') PostResultCount FROM TrainingDetails TD LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE 1=1";
            SqlCommand cmd=new SqlCommand();
            if(!string.IsNullOrWhiteSpace(ddlCourseFilter.SelectedValue))
            {
                q+=" AND TD.CourseID=@CourseID";
                cmd.Parameters.AddWithValue("@CourseID",ddlCourseFilter.SelectedValue);
            }
            if(!string.IsNullOrWhiteSpace(ddlStatusFilter.SelectedValue))
            {
                if(ddlStatusFilter.SelectedValue=="Future")
                    q+=" AND COALESCE(TRY_CONVERT(date,TD.DateFrom,105),TRY_CONVERT(date,TD.DateFrom,23))>CAST(GETDATE() AS date)";
                else
                {
                    q+=" AND ISNULL(TD.TrainingStatus,'')=@Status";
                    cmd.Parameters.AddWithValue("@Status",ddlStatusFilter.SelectedValue);
                }
            }
            if(!string.IsNullOrWhiteSpace(txtTrainingSearch.Text))
            {
                q+=" AND (TD.TrainingID LIKE @Search OR ISNULL(TD.Batch,'') LIKE @Search)";
                cmd.Parameters.AddWithValue("@Search","%"+txtTrainingSearch.Text.Trim()+"%");
            }
            q+=" ORDER BY COALESCE(TRY_CONVERT(date,TD.DateFrom,105),TRY_CONVERT(date,TD.DateFrom,23)) DESC,TD.TrainingID DESC";
            cmd.CommandText=q;
            DataTable dt=new DataTable();
            using(SqlConnection con=new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
            {
                cmd.Connection=con;
                using(SqlDataAdapter da=new SqlDataAdapter(cmd))da.Fill(dt);
            }
            gvTrainingList.DataSource=dt;
            gvTrainingList.DataBind();
            lblTrainingCount.Text=dt.Rows.Count+" training(s)";
        }

        protected void btnSearchTraining_Click(object sender,EventArgs e){BindTrainingList();ShowLevel(1);}
        protected void btnResetTraining_Click(object sender,EventArgs e){ddlCourseFilter.SelectedIndex=0;ddlStatusFilter.SelectedIndex=0;txtTrainingSearch.Text="";BindTrainingList();ShowLevel(1);}

        protected void ShowTrainingInfo(string trainingID)
        {
            string q="SELECT TD.TrainingID,ISNULL(CM.CourseName,'') CourseName,ISNULL(TD.Batch,'') Batch,ISNULL(TD.TrainingType,'') TrainingType,ISNULL(TD.TrainingOrganizer,'') TrainingOrganizer,ISNULL(TD.TrainingLocation,'') TrainingLocation,TD.DateFrom,TD.DateTo,ISNULL(TD.NoOfDays,0) NoOfDays,ISNULL(TD.TrainingStatus,'') TrainingStatus FROM TrainingDetails TD LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE TD.TrainingID=@TrainingID";
            DataTable dt=objDB.GetDataTable(q,new SqlParameter[] { new SqlParameter("@TrainingID",trainingID) });
            if(dt.Rows.Count==0)return;
            DataRow r=dt.Rows[0];
            lblInfoTrainingID.Text=Convert.ToString(r["TrainingID"]);
            lblInfoCourse.Text=Convert.ToString(r["CourseName"]);
            lblInfoBatch.Text=Convert.ToString(r["Batch"]);
            lblInfoType.Text=Convert.ToString(r["TrainingType"]);
            lblInfoOrganizer.Text=Convert.ToString(r["TrainingOrganizer"]);
            lblInfoLocation.Text=Convert.ToString(r["TrainingLocation"]);
            lblInfoFrom.Text=Convert.ToString(r["DateFrom"]);
            lblInfoTo.Text=Convert.ToString(r["DateTo"]);
            lblInfoDays.Text=Convert.ToString(r["NoOfDays"]);
            lblInfoStatus.Text=Convert.ToString(r["TrainingStatus"]);
            ClientScript.RegisterStartupScript(GetType(),"showTrainingInfo","showTrainingInfo();",true);
        }

        protected void gvTrainingList_RowCommand(object sender,GridViewCommandEventArgs e)
        {
            if(e.CommandName=="TrainingInfo"){ShowTrainingInfo(Convert.ToString(e.CommandArgument));return;}
            if(e.CommandName!="ViewTraining")return;
            string trainingID=Convert.ToString(e.CommandArgument);
            ViewState["TrainingID"]=trainingID;
            BindSessionList(trainingID);
            ShowLevel(2);
        }

        private void BindSessionList(string trainingID)
        {
            string q="SELECT SM.SessionID,SM.SessionNo,ISNULL(SM.SessionName,'') SessionName,ISNULL(TP.TopicName,'') TopicName,SM.SessionDate,SM.StartTime,SM.EndTime,ISNULL(SM.SessionStatus,'') SessionStatus,ISNULL(CASE WHEN TM.TrainerType='Internal' THEN E.EmpName ELSE TM.NameExternal END,'') TrainerName,(SELECT COUNT(*) FROM TestResult TR INNER JOIN TestMaster T1 ON TR.TestID=T1.TestID WHERE T1.SessionID=SM.SessionID AND T1.TestType='Pre') PreResults,(SELECT COUNT(*) FROM TestResult TR INNER JOIN TestMaster T2 ON TR.TestID=T2.TestID WHERE T2.SessionID=SM.SessionID AND T2.TestType='Post') PostResults FROM SessionMaster SM LEFT JOIN TopicMaster TP ON SM.TopicID=TP.TopicID LEFT JOIN TrainerMaster TM ON SM.TrainerID=TM.TrainerID LEFT JOIN EmpBasicMaster E ON TM.EmpID=E.EmpID WHERE SM.TrainingID=@TrainingID AND ISNULL(SM.SessionCancelled,0)=0 ORDER BY TRY_CONVERT(date,SM.SessionDate,105),TRY_CONVERT(int,SM.SessionNo)";
            DataTable dt=objDB.GetDataTable(q,new SqlParameter[] { new SqlParameter("@TrainingID",trainingID) });
            gvSessionList.DataSource=dt;
            gvSessionList.DataBind();
            lblSelectedTraining.Text=trainingID+" - "+GetTrainingName(trainingID);
        }

        private string GetTrainingName(string trainingID)
        {
            string q="SELECT ISNULL(CM.CourseName,'')+' | Batch '+ISNULL(TD.Batch,'') FROM TrainingDetails TD LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE TD.TrainingID=@TrainingID";
            DataTable dt=objDB.GetDataTable(q,new SqlParameter[] { new SqlParameter("@TrainingID",trainingID) });
            return dt.Rows.Count==0?"":Convert.ToString(dt.Rows[0][0]);
        }

        protected void gvSessionList_RowCommand(object sender,GridViewCommandEventArgs e)
        {
            if(e.CommandName!="ViewResults")return;
            string sessionID=Convert.ToString(e.CommandArgument);
            ViewState["SessionID"]=sessionID;
            BindResultList(sessionID);
            ShowLevel(3);
        }

        private void BindResultList(string sessionID)
        {
            string q="SELECT TM.TestType,ISNULL(TM.TestTitle,'') TestTitle,TR.ResultID,TR.EmpID,ISNULL(EBM.EmpName,TME.TraineeName) TraineeName,TR.AttemptNo,TR.TotalQuestions,TR.AttemptedQuestions,TR.CorrectAnswers,TR.WrongAnswers,TR.TotalMarks,TR.ObtainedMarks,TR.Percentage,TR.ResultStatus,TR.RankNo,TR.SubmittedOn FROM TestResult TR INNER JOIN TestMaster TM ON TR.TestID=TM.TestID LEFT JOIN EmpBasicMaster EBM ON EBM.EmpID=TR.EmpID LEFT JOIN TraineeMasterExternal TME ON TME.EmpIDExternal=TR.EmpID WHERE TM.SessionID=@SessionID ORDER BY CASE WHEN TM.TestType='Pre' THEN 1 ELSE 2 END,TR.EmpID,TR.AttemptNo DESC";
            DataTable dt=objDB.GetDataTable(q,new SqlParameter[] { new SqlParameter("@SessionID",sessionID) });
            gvResultList.DataSource=dt;
            gvResultList.DataBind();
            lblResultCount.Text=dt.Rows.Count.ToString();
            lblPassed.Text=Convert.ToString(dt.Compute("COUNT(ResultStatus)","ResultStatus='PASS' OR ResultStatus='Passed'"));
            lblFailed.Text=Convert.ToString(dt.Compute("COUNT(ResultStatus)","ResultStatus='FAIL' OR ResultStatus='Failed'"));
            object avg=dt.Compute("AVG(Percentage)","");
            lblAveragePercentage.Text=avg==DBNull.Value?"0.00 %":Convert.ToDecimal(avg).ToString("0.00")+" %";
            lblSelectedSession.Text="Session "+sessionID;
        }

        protected void btnBackTraining_Click(object sender,EventArgs e){ShowLevel(1);}
        protected void btnBackSessions_Click(object sender,EventArgs e){string id=Convert.ToString(ViewState["TrainingID"]);BindSessionList(id);ShowLevel(2);}

        private void ShowLevel(int level)
        {
            pnlTrainingList.Visible=level==1;
            pnlSessionList.Visible=level==2;
            pnlResultList.Visible=level==3;
            lblMessage.Text="";
        }

        protected void gvResultList_RowDataBound(object sender,GridViewRowEventArgs e)
        {
            if(e.Row.RowType!=DataControlRowType.DataRow)return;
            string result=Convert.ToString(DataBinder.Eval(e.Row.DataItem,"ResultStatus")).ToUpperInvariant();
            for(int i=0;i<e.Row.Cells.Count;i++)
            {
                if(e.Row.Cells[i].Text=="PASS"||e.Row.Cells[i].Text=="Passed")e.Row.Cells[i].CssClass="result-pass";
                if(e.Row.Cells[i].Text=="FAIL"||e.Row.Cells[i].Text=="Failed")e.Row.Cells[i].CssClass="result-fail";
            }
        }

        protected void btnExportTraining_Click(object sender,EventArgs e){ExportDataTable(GetTrainingExportData(),"ExamResult_TrainingList");}
        protected void btnExportSessions_Click(object sender,EventArgs e){string id=Convert.ToString(ViewState["TrainingID"]);ExportDataTable(GetSessionExportData(id),"ExamResult_Sessions");}
        protected void btnExportResults_Click(object sender,EventArgs e){string id=Convert.ToString(ViewState["SessionID"]);ExportDataTable(GetResultExportData(id),"ExamResult_SessionResults");}

        private DataTable GetTrainingExportData(){return GetTrainingDataForExport();}
        private DataTable GetTrainingDataForExport()
        {
            string q="SELECT TD.TrainingID,ISNULL(CM.CourseName,'') CourseName,TD.Batch,TD.TrainingType,TD.TrainingStatus,TD.DateFrom,TD.DateTo FROM TrainingDetails TD LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID ORDER BY TD.TrainingID DESC";
            return objDB.GetDataTable(q);
        }
        private DataTable GetSessionExportData(string id){return objDB.GetDataTable("SELECT SessionID,SessionNo,SessionName,SessionDate,StartTime,EndTime,SessionStatus FROM SessionMaster WHERE TrainingID=@TrainingID AND ISNULL(SessionCancelled,0)=0 ORDER BY TRY_CONVERT(int,SessionNo)",new SqlParameter[] { new SqlParameter("@TrainingID",id) });}
        private DataTable GetResultExportData(string id){return objDB.GetDataTable("SELECT TM.TestType,TM.TestTitle,TR.EmpID,ISNULL(EBM.EmpName,TME.TraineeName) TraineeName,TR.AttemptNo,TR.TotalQuestions,TR.AttemptedQuestions,TR.CorrectAnswers,TR.WrongAnswers,TR.TotalMarks,TR.ObtainedMarks,TR.Percentage,TR.ResultStatus,TR.RankNo,TR.SubmittedOn FROM TestResult TR INNER JOIN TestMaster TM ON TR.TestID=TM.TestID LEFT JOIN EmpBasicMaster EBM ON EBM.EmpID=TR.EmpID LEFT JOIN TraineeMasterExternal TME ON TME.EmpIDExternal=TR.EmpID WHERE TM.SessionID=@SessionID ORDER BY TM.TestType,TR.EmpID,TR.AttemptNo DESC",new SqlParameter[] { new SqlParameter("@SessionID",id) });}

        private void ExportDataTable(DataTable dt,string fileName)
        {
            if(dt==null||dt.Rows.Count==0){lblMessage.Text="No data available for export.";return;}
            GridView g=new GridView();
            g.DataSource=dt;
            g.DataBind();
            Response.Clear();
            Response.Buffer=true;
            Response.AddHeader("content-disposition","attachment;filename="+fileName+"_"+DateTime.Now.ToString("yyyyMMddHHmmss")+".xls");
            Response.Charset="";
            Response.ContentType="application/vnd.ms-excel";
            using(StringWriter sw=new StringWriter())
            {
                using(HtmlTextWriter hw=new HtmlTextWriter(sw))g.RenderControl(hw);
                Response.Output.Write(sw.ToString());
            }
            Response.Flush();
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        public override void VerifyRenderingInServerForm(Control control){}
    }
}