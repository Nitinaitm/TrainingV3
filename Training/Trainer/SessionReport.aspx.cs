using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace Training.Trainer
{
    public partial class SessionReport : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();
        private string TrainerID { get { return Session["TrainerID"].ToString(); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TrainerID"] == null || String.IsNullOrWhiteSpace(Session["TrainerID"].ToString()))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }
            if (!IsPostBack)
            {
                BindCourse();
                BindStatus();
                BindGrid();
            }
        }

        private void BindCourse()
        {
            DataTable dt = obj.GetDataTable("SELECT DISTINCT CM.CourseID,CM.CourseName FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID INNER JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE SM.TrainerID=@TrainerID ORDER BY CM.CourseName", new SqlParameter[] { new SqlParameter("@TrainerID", TrainerID) });
            ddlCourse.DataSource=dt; ddlCourse.DataTextField="CourseName"; ddlCourse.DataValueField="CourseID"; ddlCourse.DataBind(); ddlCourse.Items.Insert(0,new ListItem("-- All Courses --",""));
        }

        private void BindStatus(){DataTable dt=obj.GetDataTable("SELECT DISTINCT ISNULL(SM.SessionStatus,'') SessionStatus FROM SessionMaster SM WHERE SM.TrainerID=@TrainerID ORDER BY SessionStatus",new SqlParameter[] { new SqlParameter("@TrainerID",TrainerID) });ddlStatus.DataSource=dt;ddlStatus.DataTextField="SessionStatus";ddlStatus.DataValueField="SessionStatus";ddlStatus.DataBind();ddlStatus.Items.Insert(0,new ListItem("-- All Status --",""));}

        private void BindGrid()
        {
            string query="SELECT SM.SessionID,SM.TrainingID,CM.CourseName,TD.Batch,SM.SessionNo,SM.SessionName,ISNULL(TP.TopicName,'') TopicName,SM.SessionDate,SM.StartTime,SM.EndTime FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID INNER JOIN CourseMaster CM ON TD.CourseID=CM.CourseID LEFT JOIN TopicMaster TP ON SM.TopicID=TP.TopicID WHERE SM.TrainerID=@TrainerID";
            System.Collections.Generic.List<SqlParameter> p=new System.Collections.Generic.List<SqlParameter>(); p.Add(new SqlParameter("@TrainerID",TrainerID));
            if(!String.IsNullOrWhiteSpace(ddlCourse.SelectedValue)){query+=" AND TD.CourseID=@CourseID";p.Add(new SqlParameter("@CourseID",ddlCourse.SelectedValue));}
            if(!String.IsNullOrWhiteSpace(ddlStatus.SelectedValue)){query+=" AND ISNULL(SM.SessionStatus,'')=@Status";p.Add(new SqlParameter("@Status",ddlStatus.SelectedValue));}
            if(!String.IsNullOrWhiteSpace(txtBatch.Text)){query+=" AND TD.Batch LIKE @Batch";p.Add(new SqlParameter("@Batch","%"+txtBatch.Text.Trim()+"%"));}
            DateTime fromDate,toDate;
            if(ParseDate(txtFromDate.Text,out fromDate)){query+=" AND TRY_CONVERT(date,SM.SessionDate,105)>=@FromDate";p.Add(new SqlParameter("@FromDate",fromDate));}
            if(ParseDate(txtToDate.Text,out toDate)){query+=" AND TRY_CONVERT(date,SM.SessionDate,105)<=@ToDate";p.Add(new SqlParameter("@ToDate",toDate));}
            query+=" ORDER BY TRY_CONVERT(date,SM.SessionDate,105) DESC,TRY_CONVERT(int,SM.SessionNo),SM.SessionID DESC";
            gvSession.DataSource=obj.GetDataTable(query,p.ToArray()); gvSession.DataBind();
        }

        private bool ParseDate(string value,out DateTime date){return DateTime.TryParseExact(value==null?"":value.Trim(),"dd-MM-yyyy",CultureInfo.InvariantCulture,DateTimeStyles.None,out date);}

        protected void btnSearch_Click(object sender,EventArgs e){BindGrid();}
        protected void btnReset_Click(object sender,EventArgs e){ddlCourse.SelectedIndex=0;ddlStatus.SelectedIndex=0;txtBatch.Text="";txtFromDate.Text="";txtToDate.Text="";lblMessage.Text="";BindGrid();}

        protected void gvSession_RowCommand(object sender,GridViewCommandEventArgs e)
        {
            if(e.CommandName!="View")return;
            GridViewRow row=(GridViewRow)((Control)e.CommandSource).NamingContainer;
            Session["SessionID"]=gvSession.DataKeys[row.RowIndex].Values["SessionID"].ToString();
            Session["TrainingID"]=gvSession.DataKeys[row.RowIndex].Values["TrainingID"].ToString();
            Response.Redirect("~/Trainer/SessionReportDetails.aspx");
        }
    }
}