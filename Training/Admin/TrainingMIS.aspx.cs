using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class TrainingMIS : Page
    {
        private readonly string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender,EventArgs e)
        {
            if(!IsPostBack){BindStatus();BindMIS();}
        }

        private void BindStatus()
        {
            using(SqlConnection con=new SqlConnection(constr))
            using(SqlCommand cmd=new SqlCommand("SELECT DISTINCT ISNULL(TrainingStatus,'') TrainingStatus FROM TrainingDetails WHERE ISNULL(TrainingStatus,'')<>'' ORDER BY TrainingStatus",con))
            {
                con.Open();
                ddlStatus.DataSource=cmd.ExecuteReader();
                ddlStatus.DataTextField="TrainingStatus";
                ddlStatus.DataValueField="TrainingStatus";
                ddlStatus.DataBind();
                ddlStatus.Items.Insert(0,new ListItem("All Status",""));
            }
        }

        protected void btnSearch_Click(object sender,EventArgs e){BindMIS();}
        protected void btnReset_Click(object sender,EventArgs e){txtFromDate.Text="";txtToDate.Text="";ddlStatus.SelectedIndex=0;BindMIS();}

        private string DateExpr(string field)
        {
            return "COALESCE(TRY_CONVERT(date,"+field+",105),TRY_CONVERT(date,"+field+",23),TRY_CONVERT(date,"+field+"))";
        }

        private string Filter(string alias)
        {
            StringBuilder s=new StringBuilder();
            if(txtFromDate.Text.Trim()!="")s.Append(" AND "+DateExpr(alias+".DateFrom")+">=TRY_CONVERT(date,@FromDate,105)");
            if(txtToDate.Text.Trim()!="")s.Append(" AND "+DateExpr(alias+".DateTo")+"<=TRY_CONVERT(date,@ToDate,105)");
            if(ddlStatus.SelectedValue!="")s.Append(" AND ISNULL("+alias+".TrainingStatus,'')=@Status");
            return s.ToString();
        }

        private void AddCommon(SqlCommand cmd)
        {
            if(txtFromDate.Text.Trim()!="")cmd.Parameters.AddWithValue("@FromDate",txtFromDate.Text.Trim());
            if(txtToDate.Text.Trim()!="")cmd.Parameters.AddWithValue("@ToDate",txtToDate.Text.Trim());
            if(ddlStatus.SelectedValue!="")cmd.Parameters.AddWithValue("@Status",ddlStatus.SelectedValue);
        }

        private DataTable GetTable(string q,params SqlParameter[] p)
        {
            DataTable dt=new DataTable();
            using(SqlConnection con=new SqlConnection(constr))
            using(SqlCommand cmd=new SqlCommand(q,con))
            {
                if(p!=null)cmd.Parameters.AddRange(p);
                using(SqlDataAdapter da=new SqlDataAdapter(cmd))da.Fill(dt);
            }
            return dt;
        }

        private int Scalar(string q)
        {
            using(SqlConnection con=new SqlConnection(constr))
            using(SqlCommand cmd=new SqlCommand(q,con))
            {
                AddCommon(cmd);
                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private decimal ScalarDecimal(string q)
        {
            using(SqlConnection con=new SqlConnection(constr))
            using(SqlCommand cmd=new SqlCommand(q,con))
            {
                AddCommon(cmd);
                con.Open();
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        private void BindMIS()
        {
            string f=Filter("TD");
            lblTotalTrainings.Text=Scalar("SELECT COUNT(*) FROM TrainingDetails TD WHERE 1=1"+f).ToString();
            lblCompletedTrainings.Text=Scalar("SELECT COUNT(*) FROM TrainingDetails TD WHERE ISNULL(TD.TrainingStatus,'') IN ('Closed','Completed')"+f).ToString();
            lblOngoingTrainings.Text=Scalar("SELECT COUNT(*) FROM TrainingDetails TD WHERE ISNULL(TD.TrainingStatus,'') NOT IN ('Closed','Completed') AND "+DateExpr("TD.DateFrom")+"<=CAST(GETDATE() AS date) AND "+DateExpr("TD.DateTo")+">=CAST(GETDATE() AS date)"+f).ToString();
            lblFutureTrainings.Text=Scalar("SELECT COUNT(*) FROM TrainingDetails TD WHERE ISNULL(TD.TrainingStatus,'') NOT IN ('Closed','Completed') AND "+DateExpr("TD.DateFrom")+">CAST(GETDATE() AS date)"+f).ToString();
            lblTotalTrainees.Text=Scalar("SELECT COUNT(DISTINCT TA.EmpID) FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TA.TrainingID=TD.TrainingID WHERE ISNULL(TA.Cancelled,0)=0"+f).ToString();
            lblTotalSessions.Text=Scalar("SELECT COUNT(*) FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID WHERE ISNULL(SM.SessionCancelled,0)=0"+f).ToString();
            decimal attendance=ScalarDecimal("SELECT ISNULL(100.0*SUM(CASE WHEN SA.AttendanceStatus='Present' THEN 1 ELSE 0 END)/NULLIF(COUNT(SA.ID),0),0) FROM SessionAttendance SA INNER JOIN TrainingDetails TD ON SA.TrainingID=TD.TrainingID WHERE 1=1"+f);
            lblAttendance.Text=attendance.ToString("0.00")+"%";
            lblCertificates.Text=Scalar("SELECT COUNT(*) FROM TrainingCertificate TC INNER JOIN TrainingDetails TD ON TC.TrainingID=TD.TrainingID WHERE 1=1"+f).ToString();
            lblFeedback.Text=Scalar("SELECT COUNT(*) FROM Feedback F INNER JOIN TrainingDetails TD ON F.TrainingID=TD.TrainingID WHERE ISNULL(F.Submitted,0)=1"+f).ToString();
            decimal post=ScalarDecimal("SELECT ISNULL(AVG(R.Percentage),0) FROM TestResult R INNER JOIN TestMaster TM ON R.TestID=TM.TestID INNER JOIN SessionMaster SM ON TM.SessionID=SM.SessionID INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID WHERE TM.TestType='POST'"+f);
            lblPostTest.Text=post.ToString("0.00")+"%";

            string statusQ="SELECT ISNULL(TD.TrainingStatus,'') TrainingStatus,COUNT(*) TrainingCount FROM TrainingDetails TD WHERE 1=1"+f+" GROUP BY ISNULL(TD.TrainingStatus,'') ORDER BY TrainingStatus";
            string typeQ="SELECT ISNULL(TD.TrainingType,'') TrainingType,COUNT(*) TrainingCount FROM TrainingDetails TD WHERE 1=1"+f+" GROUP BY ISNULL(TD.TrainingType,'') ORDER BY TrainingType";
            string courseQ="SELECT ISNULL(CM.CourseName,'') CourseName,COUNT(*) TrainingCount,ISNULL(SUM(TD.NoOfDays),0) TotalTrainingDays FROM TrainingDetails TD LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE 1=1"+f+" GROUP BY ISNULL(CM.CourseName,'') ORDER BY TrainingCount DESC,CourseName";
            gvStatus.DataSource=GetFiltered(statusQ);gvStatus.DataBind();
            gvType.DataSource=GetFiltered(typeQ);gvType.DataBind();
            gvCourse.DataSource=GetFiltered(courseQ);gvCourse.DataBind();
        }

        private DataTable GetFiltered(string q)
        {
            using(SqlConnection con=new SqlConnection(constr))
            using(SqlCommand cmd=new SqlCommand(q,con))
            {
                AddCommon(cmd);
                DataTable dt=new DataTable();
                using(SqlDataAdapter da=new SqlDataAdapter(cmd))da.Fill(dt);
                return dt;
            }
        }

        protected void btnExport_Click(object sender,EventArgs e)
        {
            DataTable dt=GetFiltered("SELECT TD.TrainingID,ISNULL(CM.CourseName,'') CourseName,ISNULL(TD.TrainingType,'') TrainingType,ISNULL(TD.TrainingStatus,'') TrainingStatus,TD.NoOfDays FROM TrainingDetails TD LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE 1=1"+Filter("TD")+" ORDER BY "+DateExpr("TD.DateFrom"));
            Response.Clear();Response.Buffer=true;Response.AddHeader("content-disposition","attachment;filename=TrainingMIS.xls");Response.Charset="";Response.ContentType="application/vnd.ms-excel";
            StringBuilder sb=new StringBuilder();foreach(DataColumn c in dt.Columns)sb.Append(c.ColumnName+"\t");sb.Append("\r\n");foreach(DataRow r in dt.Rows){foreach(DataColumn c in dt.Columns)sb.Append(r[c].ToString().Replace("\t"," ")+"\t");sb.Append("\r\n");}Response.Write(sb.ToString());Response.End();
        }

        public override void VerifyRenderingInServerForm(Control control){}
    }
}