using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class TrainingSearch : Page
    {
        private readonly string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindFilters();
                lblResultCount.Text = "";
            }
        }

        private void BindFilters()
        {
            BindList(lstDesignation,"SELECT DISTINCT EmpDesignation FROM EmpBasicMaster WHERE ISNULL(EmpDesignation,'')<>'' ORDER BY EmpDesignation","EmpDesignation");
            BindList(lstCompany,"SELECT DISTINCT EmpCompany FROM EmpBasicMaster WHERE ISNULL(EmpCompany,'')<>'' ORDER BY EmpCompany","EmpCompany");
            BindList(lstPostingPlace,"SELECT DISTINCT EmpPostingPlace FROM EmpBasicMaster WHERE ISNULL(EmpPostingPlace,'')<>'' ORDER BY EmpPostingPlace","EmpPostingPlace");
            BindList(lstPostingDetails,"SELECT DISTINCT EmpPostingPlace FROM EmpPostingDetails WHERE ISNULL(EmpPostingPlace,'')<>'' ORDER BY EmpPostingPlace","EmpPostingPlace");
            BindList(lstDepartment,"SELECT DISTINCT EmpPostingDepartment FROM EmpPostingDetails WHERE ISNULL(EmpPostingDepartment,'')<>'' ORDER BY EmpPostingDepartment","EmpPostingDepartment");
            BindList(lstZone,"SELECT DISTINCT AreaBoardZone FROM EmpPostingDetails WHERE ISNULL(AreaBoardZone,'')<>'' ORDER BY AreaBoardZone","AreaBoardZone");
            BindList(lstCircle,"SELECT DISTINCT Circle FROM EmpPostingDetails WHERE ISNULL(Circle,'')<>'' ORDER BY Circle","Circle");
            BindList(lstDivision,"SELECT DISTINCT Division FROM EmpPostingDetails WHERE ISNULL(Division,'')<>'' ORDER BY Division","Division");
            BindList(lstSubdivision,"SELECT DISTINCT Subdivision FROM EmpPostingDetails WHERE ISNULL(Subdivision,'')<>'' ORDER BY Subdivision","Subdivision");
            BindList(lstSection,"SELECT DISTINCT Section FROM EmpPostingDetails WHERE ISNULL(Section,'')<>'' ORDER BY Section","Section");
        }

        private void BindList(ListBox list,string query,string field)
        {
            DataTable dt=new DataTable();
            using(SqlConnection con=new SqlConnection(constr))
            using(SqlCommand cmd=new SqlCommand(query,con))
            using(SqlDataAdapter da=new SqlDataAdapter(cmd)){da.Fill(dt);}
            list.DataSource=dt;
            list.DataTextField=field;
            list.DataValueField=field;
            list.DataBind();
        }

        protected void btnSearch_Click(object sender,EventArgs e){BindGrid();}

        protected void btnReset_Click(object sender,EventArgs e)
        {
            txtEmpID.Text="";
            txtEmpName.Text="";
            txtMobile.Text="";
            txtEmail.Text="";
            ClearList(lstDesignation);
            ClearList(lstCompany);
            ClearList(lstPostingPlace);
            ClearList(lstPostingDetails);
            ClearList(lstDepartment);
            ClearList(lstZone);
            ClearList(lstCircle);
            ClearList(lstDivision);
            ClearList(lstSubdivision);
            ClearList(lstSection);
            BindFilters();
            gvTraining.DataSource=null;
            gvTraining.DataBind();
            lblResultCount.Text="";
        }

        private void ClearList(ListBox list){foreach(ListItem item in list.Items)item.Selected=false;}

        private void BindGrid()
        {
            DataTable dt=GetData();
            gvTraining.DataSource=dt;
            gvTraining.DataBind();
            lblResultCount.Text=dt.Rows.Count+" trainee training record(s) found.";
        }

        private DataTable GetData()
        {
            StringBuilder q=new StringBuilder();
            q.Append("SELECT E.EmpID,E.EmpName,E.EmpDesignation,E.EmpCompany,E.EmpPostingPlace,E.MobileNo,E.EmailId,ISNULL(P.EmpPostingPlace,'') [Posting Details],ISNULL(P.EmpPostingDepartment,'') [Department / Office / Cell],ISNULL(P.AreaBoardZone,'') [Area Board / Zone],ISNULL(P.Circle,'') Circle,ISNULL(P.Division,'') Division,ISNULL(P.Subdivision,'') Subdivision,ISNULL(P.Section,'') Section,TD.TrainingID,ISNULL(CM.CourseName,'') CourseName,TD.TrainingType,TD.TrainingOrganizer,TD.Batch,TD.TrainingLocation,TD.NoOfDays,CONVERT(varchar(10),TRY_CONVERT(date,TD.DateFrom),105) DateFrom,CONVERT(varchar(10),TRY_CONVERT(date,TD.DateTo),105) DateTo,ISNULL(TD.TrainingStatus,'') TrainingStatus,(SELECT STUFF((SELECT DISTINCT ', '+ISNULL(TP.TopicName,'') FROM SessionMaster SX LEFT JOIN TopicMaster TP ON SX.TopicID=TP.TopicID WHERE SX.TrainingID=TD.TrainingID AND ISNULL(TP.TopicName,'')<>'' FOR XML PATH(''),TYPE).value('.','nvarchar(max)'),1,2,'')) Topics FROM TrainingAssignment TA INNER JOIN EmpBasicMaster E ON TA.EmpID=E.EmpID INNER JOIN TrainingDetails TD ON TA.TrainingID=TD.TrainingID LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID OUTER APPLY (SELECT TOP 1 EP.EmpPostingPlace,EP.EmpPostingDepartment,EP.AreaBoardZone,EP.Circle,EP.Division,EP.Subdivision,EP.Section FROM EmpPostingDetails EP WHERE EP.EmpID=E.EmpID ORDER BY EP.ID DESC) P WHERE ISNULL(TA.Cancelled,0)=0");
            SqlCommand cmd=new SqlCommand();
            AddLike(q,cmd,"E.EmpID","EmpID",txtEmpID.Text);
            AddLike(q,cmd,"E.EmpName","EmpName",txtEmpName.Text);
            AddLike(q,cmd,"E.MobileNo","Mobile",txtMobile.Text);
            AddLike(q,cmd,"E.EmailId","Email",txtEmail.Text);
            AddIn(q,cmd,"E.EmpDesignation","Designation",lstDesignation);
            AddIn(q,cmd,"E.EmpCompany","Company",lstCompany);
            AddIn(q,cmd,"E.EmpPostingPlace","HRMSPostingPlace",lstPostingPlace);
            AddIn(q,cmd,"P.EmpPostingPlace","PostingDetails",lstPostingDetails);
            AddIn(q,cmd,"P.EmpPostingDepartment","Department",lstDepartment);
            AddIn(q,cmd,"P.AreaBoardZone","Zone",lstZone);
            AddIn(q,cmd,"P.Circle","Circle",lstCircle);
            AddIn(q,cmd,"P.Division","Division",lstDivision);
            AddIn(q,cmd,"P.Subdivision","Subdivision",lstSubdivision);
            AddIn(q,cmd,"P.Section","Section",lstSection);
            q.Append(" ORDER BY E.EmpID,TRY_CONVERT(date,TD.DateFrom),TD.TrainingID");
            cmd.CommandText=q.ToString();
            DataTable dt=new DataTable();
            using(SqlConnection con=new SqlConnection(constr))
            using(SqlDataAdapter da=new SqlDataAdapter(cmd)){cmd.Connection=con;da.Fill(dt);}
            return dt;
        }

        private void AddLike(StringBuilder q,SqlCommand cmd,string field,string name,string value)
        {
            if(string.IsNullOrWhiteSpace(value))return;
            q.Append(" AND "+field+" LIKE @"+name);
            cmd.Parameters.AddWithValue("@"+name,"%"+value.Trim()+"%");
        }

        private void AddIn(StringBuilder q,SqlCommand cmd,string field,string name,ListBox list)
        {
            List<string> values=new List<string>();
            foreach(ListItem item in list.Items)if(item.Selected&&!string.IsNullOrWhiteSpace(item.Value))values.Add(item.Value);
            if(values.Count==0)return;
            List<string> parameters=new List<string>();
            for(int i=0;i<values.Count;i++){string p="@"+name+i;parameters.Add(p);cmd.Parameters.AddWithValue(p,values[i]);}
            q.Append(" AND "+field+" IN ("+string.Join(",",parameters.ToArray())+")");
        }

        protected void btnExport_Click(object sender,EventArgs e){ExportExcel(GetData(),"EmployeeTrainingReport.xls");}

        private void ExportExcel(DataTable dt,string fileName)
        {
            Response.Clear();
            Response.Buffer=true;
            Response.AddHeader("content-disposition","attachment;filename="+fileName);
            Response.Charset="";
            Response.ContentType="application/vnd.ms-excel";
            StringBuilder sb=new StringBuilder();
            foreach(DataColumn c in dt.Columns)sb.Append(c.ColumnName+"\t");
            sb.Append("\r\n");
            foreach(DataRow r in dt.Rows){foreach(DataColumn c in dt.Columns)sb.Append(r[c].ToString().Replace("\t"," ")+"\t");sb.Append("\r\n");}
            Response.Write(sb.ToString());
            Response.End();
        }

        public override void VerifyRenderingInServerForm(Control control){}
    }
}