using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;

namespace Training.Admin
{
    public partial class TrainingSearch : Page
    {
        private readonly string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) gvTraining.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtEmpID.Text="";
            txtEmpName.Text="";
            txtDesignation.Text="";
            txtCompany.Text="";
            txtPostingPlace.Text="";
            txtMobile.Text="";
            txtEmail.Text="";
            txtDetailPostingPlace.Text="";
            txtDepartment.Text="";
            txtZone.Text="";
            txtCircle.Text="";
            txtDivision.Text="";
            txtSubdivision.Text="";
            txtSection.Text="";
            gvTraining.DataSource=null;
            gvTraining.DataBind();
            lblResultCount.Text="";
        }

        private void BindGrid()
        {
            DataTable dt = GetData();
            gvTraining.DataSource=dt;
            gvTraining.DataBind();
            lblResultCount.Text=dt.Rows.Count + " trainee training record(s) found.";
        }

        private DataTable GetData()
        {
            StringBuilder q = new StringBuilder();
            q.Append("SELECT E.EmpID,E.EmpName,E.EmpDesignation,E.EmpCompany,E.EmpPostingPlace,E.MobileNo,E.EmailId,ISNULL(P.EmpPostingPlace,'') DetailPostingPlace,ISNULL(P.EmpPostingDepartment,'') [Department / Office / Cell],ISNULL(P.AreaBoardZone,'') [Area Board / Zone],ISNULL(P.Circle,'') Circle,ISNULL(P.Division,'') Division,ISNULL(P.Subdivision,'') Subdivision,ISNULL(P.Section,'') Section,TD.TrainingID,ISNULL(CM.CourseName,'') CourseName,TD.TrainingType,TD.TrainingOrganizer,TD.Batch,TD.TrainingLocation,TD.NoOfDays,CONVERT(varchar(10),TRY_CONVERT(date,TD.DateFrom),105) DateFrom,CONVERT(varchar(10),TRY_CONVERT(date,TD.DateTo),105) DateTo,ISNULL(TD.TrainingStatus,'') TrainingStatus,(SELECT STUFF((SELECT DISTINCT ', ' + ISNULL(TP.TopicName,'') FROM SessionMaster SX LEFT JOIN TopicMaster TP ON SX.TopicID=TP.TopicID WHERE SX.TrainingID=TD.TrainingID AND ISNULL(TP.TopicName,'')<>'' FOR XML PATH(''),TYPE).value('.','nvarchar(max)'),1,2,'')) Topics FROM TrainingAssignment TA INNER JOIN EmpBasicMaster E ON TA.EmpID=E.EmpID INNER JOIN TrainingDetails TD ON TA.TrainingID=TD.TrainingID LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID OUTER APPLY (SELECT TOP 1 EP.EmpPostingPlace,EP.EmpPostingDepartment,EP.AreaBoardZone,EP.Circle,EP.Division,EP.Subdivision,EP.Section FROM EmpPostingDetails EP WHERE EP.EmpID=E.EmpID ORDER BY EP.ID DESC) P WHERE ISNULL(TA.Cancelled,0)=0");
            SqlCommand cmd = new SqlCommand();
            AddLike(q,cmd,"E.EmpID","EmpID",txtEmpID.Text);
            AddLike(q,cmd,"E.EmpName","EmpName",txtEmpName.Text);
            AddLike(q,cmd,"E.EmpDesignation","Designation",txtDesignation.Text);
            AddLike(q,cmd,"E.EmpCompany","Company",txtCompany.Text);
            AddLike(q,cmd,"E.EmpPostingPlace","PostingPlace",txtPostingPlace.Text);
            AddLike(q,cmd,"E.MobileNo","Mobile",txtMobile.Text);
            AddLike(q,cmd,"E.EmailId","Email",txtEmail.Text);
            AddLike(q,cmd,"P.EmpPostingPlace","DetailPostingPlace",txtDetailPostingPlace.Text);
            AddLike(q,cmd,"P.EmpPostingDepartment","Department",txtDepartment.Text);
            AddLike(q,cmd,"P.AreaBoardZone","Zone",txtZone.Text);
            AddLike(q,cmd,"P.Circle","Circle",txtCircle.Text);
            AddLike(q,cmd,"P.Division","Division",txtDivision.Text);
            AddLike(q,cmd,"P.Subdivision","Subdivision",txtSubdivision.Text);
            AddLike(q,cmd,"P.Section","Section",txtSection.Text);
            q.Append(" ORDER BY E.EmpID,TRY_CONVERT(date,TD.DateFrom),TD.TrainingID");
            cmd.CommandText=q.ToString();
            DataTable dt=new DataTable();
            using(SqlConnection con=new SqlConnection(constr))
            using(SqlDataAdapter da=new SqlDataAdapter(cmd)){cmd.Connection=con;da.Fill(dt);}
            return dt;
        }

        private void AddLike(StringBuilder q,SqlCommand cmd,string field,string parameter,string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            q.Append(" AND " + field + " LIKE @" + parameter);
            cmd.Parameters.AddWithValue("@" + parameter,"%" + value.Trim() + "%");
        }

        protected void btnExport_Click(object sender,EventArgs e)
        {
            ExportExcel(GetData(),"EmployeeTrainingReport.xls");
        }

        private void ExportExcel(DataTable dt,string fileName)
        {
            Response.Clear();
            Response.Buffer=true;
            Response.AddHeader("content-disposition","attachment;filename=" + fileName);
            Response.Charset="";
            Response.ContentType="application/vnd.ms-excel";
            StringBuilder sb=new StringBuilder();
            foreach(DataColumn c in dt.Columns) sb.Append(c.ColumnName+"\t");
            sb.Append("\r\n");
            foreach(DataRow r in dt.Rows){foreach(DataColumn c in dt.Columns) sb.Append(r[c].ToString().Replace("\t"," ")+"\t");sb.Append("\r\n");}
            Response.Write(sb.ToString());
            Response.End();
        }

        public override void VerifyRenderingInServerForm(Control control){}
    }
}