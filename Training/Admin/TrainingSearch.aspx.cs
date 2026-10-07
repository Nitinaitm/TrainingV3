using System;
using System.Collections.Generic;
using System.Configuration;
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
                BindCompany();
                BindDesignation();
                BindPostingPlace();
                ClearPostingDetailControls();
                BindPostingPlaceRadio();
                SetPostingDetailVisibility();
                gridScrollTop.Visible = false;
                lblResultCount.Text = "";
            }
        }

        private clsDataAccess DB()
        {
            return new clsDataAccess();
        }

        private List<string> SelectedValues(ListBox listBox)
        {
            List<string> values = new List<string>();
            foreach (ListItem item in listBox.Items)
            {
                if (item.Selected && item.Value != "ALL") values.Add(item.Value);
            }
            return values;
        }

        private bool IsAllSelected(ListBox listBox)
        {
            foreach (ListItem item in listBox.Items)
            {
                if (item.Selected && item.Value == "ALL") return true;
            }
            return false;
        }

        private bool HasCompanySelection()
        {
            return IsAllSelected(lstCompany) || SelectedValues(lstCompany).Count > 0;
        }

        private void BindList(ListBox listBox, DataTable dt, string field)
        {
            listBox.Items.Clear();
            foreach (DataRow row in dt.Rows)
            {
                string value = Convert.ToString(row[field]);
                if (!string.IsNullOrWhiteSpace(value)) listBox.Items.Add(new ListItem(value,value));
            }
        }

        private void BindAllPostingDetails()
        {
            BindList(lstPostingDepartment,DB().GetDataTable("SELECT DISTINCT DepartmentName FROM DepartmentMaster WHERE ISNULL(DepartmentName,'')<>'' ORDER BY DepartmentName"),"DepartmentName");
            BindList(lstAreaBoardZone,DB().GetDataTable("SELECT DISTINCT ZoneName FROM ZoneMaster WHERE ISNULL(ZoneName,'')<>'' ORDER BY ZoneName"),"ZoneName");
            BindList(lstCircle,DB().GetDataTable("SELECT DISTINCT CircleName FROM CircleMaster WHERE ISNULL(CircleName,'')<>'' ORDER BY CircleName"),"CircleName");
            BindList(lstDivision,DB().GetDataTable("SELECT DISTINCT DivisionName FROM DivisionMaster WHERE ISNULL(DivisionName,'')<>'' ORDER BY DivisionName"),"DivisionName");
            BindList(lstSubdivision,DB().GetDataTable("SELECT DISTINCT SubdivisionName FROM SubdivisionMaster WHERE ISNULL(SubdivisionName,'')<>'' ORDER BY SubdivisionName"),"SubdivisionName");
            BindList(lstSection,DB().GetDataTable("SELECT DISTINCT SectionName FROM SectionMaster WHERE ISNULL(SectionName,'')<>'' ORDER BY SectionName"),"SectionName");
        }

        private void ClearPostingDetailControls()
        {
            rblPostingPlace.ClearSelection();
            lstPostingDepartment.Items.Clear();
            lstAreaBoardZone.Items.Clear();
            lstCircle.Items.Clear();
            lstDivision.Items.Clear();
            lstSubdivision.Items.Clear();
            lstSection.Items.Clear();
        }

        private void BindCompany()
        {
            DataTable dt = DB().GetDataTable("SELECT CompanyName FROM CompanyMaster WHERE ISNULL(CompanyName,'')<>'' ORDER BY CompanyName");
            lstCompany.Items.Clear();
            lstCompany.Items.Add(new ListItem("ALL COMPANIES","ALL"));
            foreach (DataRow row in dt.Rows)
            {
                string value = Convert.ToString(row["CompanyName"]);
                if (!string.IsNullOrWhiteSpace(value)) lstCompany.Items.Add(new ListItem(value,value));
            }
        }

        private string CompanyWhere(string column)
        {
            List<string> values = SelectedValues(lstCompany);
            if (values.Count == 0 || IsAllSelected(lstCompany)) return "";
            List<string> parameters = new List<string>();
            for (int i=0;i<values.Count;i++) parameters.Add("@COMP"+i);
            return " AND " + column + " IN (" + string.Join(",",parameters) + ")";
        }

        private SqlParameter[] CompanyParameters()
        {
            List<string> values = SelectedValues(lstCompany);
            List<SqlParameter> parameters = new List<SqlParameter>();
            if (values.Count == 0 || IsAllSelected(lstCompany)) return parameters.ToArray();
            for (int i=0;i<values.Count;i++) parameters.Add(new SqlParameter("@COMP"+i,values[i]));
            return parameters.ToArray();
        }

        private List<string> GetSelectedCompanyIDs()
        {
            List<string> ids = new List<string>();
            if (IsAllSelected(lstCompany)) return ids;
            List<string> companies = SelectedValues(lstCompany);
            if (companies.Count == 0) return ids;
            List<string> names = new List<string>();
            List<SqlParameter> parameters = new List<SqlParameter>();
            for (int i=0;i<companies.Count;i++){string p="@CMNAME"+i;names.Add(p);parameters.Add(new SqlParameter(p,companies[i]));}
            DataTable dt = DB().GetDataTable("SELECT DISTINCT CompanyID FROM CompanyMaster WHERE CompanyName IN ("+string.Join(",",names)+") ORDER BY CompanyID",parameters.ToArray());
            foreach(DataRow row in dt.Rows){string id=Convert.ToString(row["CompanyID"]);if(!string.IsNullOrWhiteSpace(id))ids.Add(id);}
            return ids;
        }

        private string CompanyIDWhere(string column,string prefix,List<SqlParameter> parameters)
        {
            if (IsAllSelected(lstCompany)) return "";
            List<string> ids = GetSelectedCompanyIDs();
            if (ids.Count==0) return " AND 1=0";
            List<string> p = new List<string>();
            for(int i=0;i<ids.Count;i++){string n="@"+prefix+i;p.Add(n);parameters.Add(new SqlParameter(n,ids[i]));}
            return " AND "+column+" IN ("+string.Join(",",p)+")";
        }

        private void BindDesignation()
        {
            DataTable dt=DB().GetDataTable("SELECT DISTINCT EmpDesignation FROM EmpBasicMaster WHERE EmpType='Internal' AND ISNULL(EmpDesignation,'')<>''"+CompanyWhere("EmpCompany")+" ORDER BY EmpDesignation",CompanyParameters());
            BindList(lstDesignation,dt,"EmpDesignation");
        }

        private void BindPostingPlace()
        {
            DataTable dt=DB().GetDataTable("SELECT DISTINCT EmpPostingPlace FROM EmpBasicMaster WHERE EmpType='Internal' AND ISNULL(EmpPostingPlace,'')<>''"+CompanyWhere("EmpCompany")+" ORDER BY EmpPostingPlace",CompanyParameters());
            BindList(lstPostingPlace,dt,"EmpPostingPlace");
        }

        private bool IsHqOnlyCompany(string company)
        {
            return !string.IsNullOrWhiteSpace(company) && (company.Equals("BSPHCL",StringComparison.OrdinalIgnoreCase) || company.Equals("BSPGCL",StringComparison.OrdinalIgnoreCase));
        }

        private bool IsHqOnlySelection()
        {
            if(IsAllSelected(lstCompany)) return false;
            List<string> companies=SelectedValues(lstCompany);
            if(companies.Count==0) return false;
            foreach(string company in companies) if(!IsHqOnlyCompany(company)) return false;
            return true;
        }

        private void BindPostingPlaceRadio()
        {
            string old=rblPostingPlace.SelectedValue;
            rblPostingPlace.Items.Clear();
            if(!HasCompanySelection()) return;
            rblPostingPlace.Items.Add(new ListItem("HQ","HQ"));
            if(!IsHqOnlySelection()) rblPostingPlace.Items.Add(new ListItem("Field Office","Field"));
            if(!string.IsNullOrWhiteSpace(old) && rblPostingPlace.Items.FindByValue(old)!=null) rblPostingPlace.SelectedValue=old;
            if(IsHqOnlySelection()) rblPostingPlace.SelectedValue="HQ";
            if(HasCompanySelection() && string.IsNullOrWhiteSpace(rblPostingPlace.SelectedValue)) rblPostingPlace.SelectedValue="HQ";
        }

        private void SetPostingDetailVisibility()
        {
            grpPostingDetails.Visible=true;
            bool hasCompany=HasCompanySelection();
            grpPostingPlace.Visible=hasCompany;
            bool hq=rblPostingPlace.SelectedValue=="HQ";
            bool field=rblPostingPlace.SelectedValue=="Field";
            grpPostingDepartment.Visible=hasCompany && hq;
            grpAreaBoardZone.Visible=hasCompany && field;
            grpCircle.Visible=hasCompany && field;
            grpDivision.Visible=hasCompany && field;
            grpSubdivision.Visible=hasCompany && field;
            grpSection.Visible=hasCompany && field;
        }

        private void BindPostingDepartment()
        {
            lstPostingDepartment.Items.Clear();
            if(!HasCompanySelection()) return;
            DataTable dt=DB().GetDataTable("SELECT DISTINCT DepartmentName FROM DepartmentMaster WHERE ISNULL(DepartmentName,'')<>'' ORDER BY DepartmentName");
            BindList(lstPostingDepartment,dt,"DepartmentName");
        }

        private void BindAreaBoardZone()
        {
            lstAreaBoardZone.Items.Clear();
            if(!HasCompanySelection()) return;
            List<SqlParameter> parameters=new List<SqlParameter>();
            string sql="SELECT DISTINCT ZM.ZoneName FROM ZoneMaster ZM WHERE ISNULL(ZM.ZoneName,'')<>''";
            sql+=CompanyIDWhere("ZM.CompanyID","ZONECOMP",parameters);
            sql+=" ORDER BY ZM.ZoneName";
            BindList(lstAreaBoardZone,DB().GetDataTable(sql,parameters.ToArray()),"ZoneName");
        }

        private void BindCircle()
        {
            lstCircle.Items.Clear();
            if(!HasCompanySelection()) return;
            List<string> zones=SelectedValues(lstAreaBoardZone);
            if(zones.Count==0) return;
            List<SqlParameter> parameters=new List<SqlParameter>();
            string sql="SELECT DISTINCT CM.CircleName FROM CircleMaster CM INNER JOIN ZoneMaster ZM ON ZM.CompanyID=CM.CompanyID AND ZM.ZoneID=CM.ZoneID WHERE ISNULL(CM.CircleName,'')<>''";
            sql+=CompanyIDWhere("CM.CompanyID","CIRCOMP",parameters);
            List<string> p=new List<string>();
            for(int i=0;i<zones.Count;i++){string n="@ZONE"+i;p.Add(n);parameters.Add(new SqlParameter(n,zones[i]));}
            sql+=" AND ZM.ZoneName IN ("+string.Join(",",p)+") ORDER BY CM.CircleName";
            BindList(lstCircle,DB().GetDataTable(sql,parameters.ToArray()),"CircleName");
        }

        private void BindDivision()
        {
            lstDivision.Items.Clear();
            if(!HasCompanySelection()) return;
            List<string> zones=SelectedValues(lstAreaBoardZone);List<string> circles=SelectedValues(lstCircle);
            if(zones.Count==0||circles.Count==0)return;
            List<SqlParameter> parameters=new List<SqlParameter>();
            string sql="SELECT DISTINCT DM.DivisionName FROM DivisionMaster DM INNER JOIN ZoneMaster ZM ON ZM.CompanyID=DM.CompanyID AND ZM.ZoneID=DM.ZoneID INNER JOIN CircleMaster CM ON CM.CompanyID=DM.CompanyID AND CM.ZoneID=DM.ZoneID AND CM.CircleID=DM.CircleID WHERE ISNULL(DM.DivisionName,'')<>''";
            sql+=CompanyIDWhere("DM.CompanyID","DIVCOMP",parameters);
            List<string> pz=new List<string>();for(int i=0;i<zones.Count;i++){string n="@DIVZONE"+i;pz.Add(n);parameters.Add(new SqlParameter(n,zones[i]));}
            sql+=" AND ZM.ZoneName IN ("+string.Join(",",pz)+")";
            List<string> pc=new List<string>();for(int i=0;i<circles.Count;i++){string n="@DIVCIRCLE"+i;pc.Add(n);parameters.Add(new SqlParameter(n,circles[i]));}
            sql+=" AND CM.CircleName IN ("+string.Join(",",pc)+") ORDER BY DM.DivisionName";
            BindList(lstDivision,DB().GetDataTable(sql,parameters.ToArray()),"DivisionName");
        }

        private void BindSubdivision()
        {
            lstSubdivision.Items.Clear();
            if(!HasCompanySelection()) return;
            List<string> zones=SelectedValues(lstAreaBoardZone);List<string> circles=SelectedValues(lstCircle);List<string> divisions=SelectedValues(lstDivision);
            if(zones.Count==0||circles.Count==0||divisions.Count==0)return;
            List<SqlParameter> parameters=new List<SqlParameter>();
            string sql="SELECT DISTINCT SM.SubdivisionName FROM SubdivisionMaster SM INNER JOIN ZoneMaster ZM ON ZM.CompanyID=SM.CompanyID AND ZM.ZoneID=SM.ZoneID INNER JOIN CircleMaster CM ON CM.CompanyID=SM.CompanyID AND CM.ZoneID=SM.ZoneID AND CM.CircleID=SM.CircleID INNER JOIN DivisionMaster DM ON DM.CompanyID=SM.CompanyID AND DM.ZoneID=SM.ZoneID AND DM.CircleID=SM.CircleID AND DM.DivisionID=SM.DivisionID WHERE ISNULL(SM.SubdivisionName,'')<>''";
            sql+=CompanyIDWhere("SM.CompanyID","SUBCOMP",parameters);
            List<string> pz=new List<string>();for(int i=0;i<zones.Count;i++){string n="@SUBZONE"+i;pz.Add(n);parameters.Add(new SqlParameter(n,zones[i]));}sql+=" AND ZM.ZoneName IN ("+string.Join(",",pz)+")";
            List<string> pc=new List<string>();for(int i=0;i<circles.Count;i++){string n="@SUBCIRCLE"+i;pc.Add(n);parameters.Add(new SqlParameter(n,circles[i]));}sql+=" AND CM.CircleName IN ("+string.Join(",",pc)+")";
            List<string> pd=new List<string>();for(int i=0;i<divisions.Count;i++){string n="@SUBDIV"+i;pd.Add(n);parameters.Add(new SqlParameter(n,divisions[i]));}sql+=" AND DM.DivisionName IN ("+string.Join(",",pd)+") ORDER BY SM.SubdivisionName";
            BindList(lstSubdivision,DB().GetDataTable(sql,parameters.ToArray()),"SubdivisionName");
        }

        private void BindSection()
        {
            lstSection.Items.Clear();
            if(!HasCompanySelection()) return;
            List<string> zones=SelectedValues(lstAreaBoardZone);List<string> circles=SelectedValues(lstCircle);List<string> divisions=SelectedValues(lstDivision);List<string> subdivisions=SelectedValues(lstSubdivision);
            if(zones.Count==0||circles.Count==0||divisions.Count==0||subdivisions.Count==0)return;
            List<SqlParameter> parameters=new List<SqlParameter>();
            string sql="SELECT DISTINCT SEM.SectionName FROM SectionMaster SEM INNER JOIN ZoneMaster ZM ON ZM.CompanyID=SEM.CompanyID AND ZM.ZoneID=SEM.ZoneID INNER JOIN CircleMaster CM ON CM.CompanyID=SEM.CompanyID AND CM.ZoneID=SEM.ZoneID AND CM.CircleID=SEM.CircleID INNER JOIN DivisionMaster DM ON DM.CompanyID=SEM.CompanyID AND DM.ZoneID=SEM.ZoneID AND DM.CircleID=SEM.CircleID AND DM.DivisionID=SEM.DivisionID INNER JOIN SubdivisionMaster SM ON SM.CompanyID=SEM.CompanyID AND SM.ZoneID=SEM.ZoneID AND SM.CircleID=SEM.CircleID AND SM.DivisionID=SEM.DivisionID AND SM.SubdivisionID=SEM.SubdivisionID WHERE ISNULL(SEM.SectionName,'')<>''";
            sql+=CompanyIDWhere("SEM.CompanyID","SECCOMP",parameters);
            List<string> pz=new List<string>();for(int i=0;i<zones.Count;i++){string n="@SECZONE"+i;pz.Add(n);parameters.Add(new SqlParameter(n,zones[i]));}sql+=" AND ZM.ZoneName IN ("+string.Join(",",pz)+")";
            List<string> pc=new List<string>();for(int i=0;i<circles.Count;i++){string n="@SECCIRCLE"+i;pc.Add(n);parameters.Add(new SqlParameter(n,circles[i]));}sql+=" AND CM.CircleName IN ("+string.Join(",",pc)+")";
            List<string> pd=new List<string>();for(int i=0;i<divisions.Count;i++){string n="@SECDIV"+i;pd.Add(n);parameters.Add(new SqlParameter(n,divisions[i]));}sql+=" AND DM.DivisionName IN ("+string.Join(",",pd)+")";
            List<string> ps=new List<string>();for(int i=0;i<subdivisions.Count;i++){string n="@SECSUB"+i;ps.Add(n);parameters.Add(new SqlParameter(n,subdivisions[i]));}sql+=" AND SM.SubdivisionName IN ("+string.Join(",",ps)+") ORDER BY SEM.SectionName";
            BindList(lstSection,DB().GetDataTable(sql,parameters.ToArray()),"SectionName");
        }

        protected void btnDependency_Click(object sender,EventArgs e)
        {
            string dependency=hfDependency.Value;
            if(dependency=="Company")
            {
                BindDesignation();
                BindPostingPlace();
                ClearPostingDetailControls();
                BindPostingPlaceRadio();
                if(rblPostingPlace.SelectedValue=="HQ") BindPostingDepartment();
                SetPostingDetailVisibility();
            }
            else if(dependency=="AreaBoard")
            {
                lstCircle.Items.Clear();lstDivision.Items.Clear();lstSubdivision.Items.Clear();lstSection.Items.Clear();
                BindCircle();
            }
            else if(dependency=="Circle")
            {
                lstDivision.Items.Clear();lstSubdivision.Items.Clear();lstSection.Items.Clear();
                BindDivision();
            }
            else if(dependency=="Division")
            {
                lstSubdivision.Items.Clear();lstSection.Items.Clear();
                BindSubdivision();
            }
            else if(dependency=="Subdivision")
            {
                lstSection.Items.Clear();
                BindSection();
            }
            hfDependency.Value="";
            SetPostingDetailVisibility();
        }

        protected void lstCompany_SelectedIndexChanged(object sender,EventArgs e)
        {
            BindDesignation();BindPostingPlace();ClearPostingDetailControls();BindPostingPlaceRadio();
            if(rblPostingPlace.SelectedValue=="HQ")BindPostingDepartment();
            if(rblPostingPlace.SelectedValue=="Field")BindAreaBoardZone();
            SetPostingDetailVisibility();
        }

        protected void rblPostingPlace_SelectedIndexChanged(object sender,EventArgs e)
        {
            lstPostingDepartment.Items.Clear();lstAreaBoardZone.Items.Clear();lstCircle.Items.Clear();lstDivision.Items.Clear();lstSubdivision.Items.Clear();lstSection.Items.Clear();
            if(rblPostingPlace.SelectedValue=="HQ")BindPostingDepartment();
            if(rblPostingPlace.SelectedValue=="Field")BindAreaBoardZone();
            SetPostingDetailVisibility();
        }

        protected void lstAreaBoardZone_SelectedIndexChanged(object sender,EventArgs e)
        {
            lstCircle.Items.Clear();lstDivision.Items.Clear();lstSubdivision.Items.Clear();lstSection.Items.Clear();BindCircle();SetPostingDetailVisibility();
        }

        protected void lstCircle_SelectedIndexChanged(object sender,EventArgs e)
        {
            lstDivision.Items.Clear();lstSubdivision.Items.Clear();lstSection.Items.Clear();BindDivision();SetPostingDetailVisibility();
        }

        protected void lstDivision_SelectedIndexChanged(object sender,EventArgs e)
        {
            lstSubdivision.Items.Clear();lstSection.Items.Clear();BindSubdivision();SetPostingDetailVisibility();
        }

        protected void lstSubdivision_SelectedIndexChanged(object sender,EventArgs e)
        {
            lstSection.Items.Clear();BindSection();SetPostingDetailVisibility();
        }

        protected void lstSection_SelectedIndexChanged(object sender,EventArgs e)
        {
            SetPostingDetailVisibility();
        }

        protected void btnAttended_Click(object sender,EventArgs e){ViewState["ReportType"]="Attended";BindGrid("Attended");}

        protected void btnNotAttended_Click(object sender,EventArgs e){ViewState["ReportType"]="NotAttended";BindGrid("NotAttended");}

        protected void btnAllEmployees_Click(object sender,EventArgs e){ViewState["ReportType"]="All";BindGrid("All");}

        protected void btnReset_Click(object sender,EventArgs e)
        {
            txtEmpID.Text="";txtEmpName.Text="";txtMobile.Text="";txtEmail.Text="";
            ClearPostingDetailControls();BindCompany();BindDesignation();BindPostingPlace();BindPostingPlaceRadio();SetPostingDetailVisibility();
            gvTraining.DataSource=null;gvTraining.DataBind();gridScrollTop.Visible=false;lblResultCount.Text="";ViewState["ReportType"]="";
        }

        private void BindGrid(string reportType)
        {
            DataTable dt=GetData(reportType);
            gvTraining.DataSource=dt;gvTraining.DataBind();gridScrollTop.Visible=dt.Rows.Count>0;lblResultCount.Text=GetReportTitle(reportType)+" - "+dt.Rows.Count+" employee/training record(s) found.";
        }

        private string GetReportTitle(string reportType)
        {
            if(reportType=="Attended") return "List of Employee Attended Training";
            if(reportType=="NotAttended") return "List of Employee Not Attended Training";
            return "All Employee";
        }

        private void AddTextFilter(StringBuilder q,SqlCommand cmd,string field,string value,string parameter)
        {
            if(string.IsNullOrWhiteSpace(value))return;
            q.Append(" AND "+field+" LIKE "+parameter);cmd.Parameters.Add(new SqlParameter(parameter,"%"+value.Trim()+"%"));
        }

        private void AddMultiSelectFilter(StringBuilder q,SqlCommand cmd,ListBox list,string field,string prefix)
        {
            if(IsAllSelected(list))return;
            List<string> p=new List<string>();int count=0;
            foreach(ListItem item in list.Items)if(item.Selected&&item.Value!="ALL"){string n="@"+prefix+count;p.Add(n);cmd.Parameters.Add(new SqlParameter(n,item.Value));count++;}
            if(p.Count>0)q.Append(" AND "+field+" IN ("+string.Join(",",p)+")");
        }

        private DataTable GetData(string reportType)
        {
            StringBuilder q=new StringBuilder();
            SqlCommand cmd=new SqlCommand();
            q.Append("SELECT E.EmpID,E.EmpName,E.EmpDesignation,E.EmpCompany,E.EmpPostingPlace,E.MobileNo,E.EmailId,ISNULL(P.EmpPostingPlace,'') [Posting Details],ISNULL(P.EmpPostingDepartment,'') [Department / Office / Cell],ISNULL(P.AreaBoardZone,'') [Area Board / Zone],ISNULL(P.Circle,'') Circle,ISNULL(P.Division,'') Division,ISNULL(P.Subdivision,'') Subdivision,ISNULL(P.Section,'') Section,ISNULL(TD.TrainingID,'') TrainingID,ISNULL(CM.CourseName,'') CourseName,ISNULL(TD.TrainingType,'') TrainingType,ISNULL(TD.TrainingOrganizer,'') TrainingOrganizer,ISNULL(TD.Batch,'') Batch,ISNULL(TD.TrainingLocation,'') TrainingLocation,ISNULL(TD.NoOfDays,0) NoOfDays,CONVERT(varchar(10),COALESCE(TRY_CONVERT(date,TD.DateFrom,105),TRY_CONVERT(date,TD.DateFrom,23),TRY_CONVERT(date,TD.DateFrom)),105) DateFrom,CONVERT(varchar(10),COALESCE(TRY_CONVERT(date,TD.DateTo,105),TRY_CONVERT(date,TD.DateTo,23),TRY_CONVERT(date,TD.DateTo)),105) DateTo,ISNULL(TD.TrainingStatus,'') TrainingStatus,CASE WHEN TD.TrainingID IS NULL THEN 'No Training' WHEN EXISTS (SELECT 1 FROM SessionAttendance SA WHERE SA.TrainingID=TD.TrainingID AND SA.EmpID=E.EmpID AND SA.AttendanceStatus='Present') THEN 'Attended' ELSE 'Not Attended' END TrainingAttendanceStatus,(SELECT STUFF((SELECT DISTINCT ', '+ISNULL(TP.TopicName,'') FROM SessionMaster SX LEFT JOIN TopicMaster TP ON SX.TopicID=TP.TopicID WHERE SX.TrainingID=TD.TrainingID AND ISNULL(TP.TopicName,'')<>'' FOR XML PATH(''),TYPE).value('.','nvarchar(max)'),1,2,'')) Topics FROM EmpBasicMaster E LEFT JOIN TrainingAssignment TA ON TA.EmpID=E.EmpID AND ISNULL(TA.Cancelled,0)=0 LEFT JOIN TrainingDetails TD ON TA.TrainingID=TD.TrainingID LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID OUTER APPLY (SELECT TOP 1 EP.EmpPostingPlace,EP.EmpPostingDepartment,EP.AreaBoardZone,EP.Circle,EP.Division,EP.Subdivision,EP.Section FROM EmpPostingDetails EP WHERE EP.EmpID=E.EmpID ORDER BY EP.ID DESC) P WHERE E.EmpType='Internal'");
            AddTextFilter(q,cmd,"E.EmpID",txtEmpID.Text,"@EmpID");
            AddTextFilter(q,cmd,"E.EmpName",txtEmpName.Text,"@EmpName");
            AddTextFilter(q,cmd,"E.MobileNo",txtMobile.Text,"@MobileNo");
            AddTextFilter(q,cmd,"E.EmailId",txtEmail.Text,"@EmailId");
            AddMultiSelectFilter(q,cmd,lstCompany,"E.EmpCompany","Company");
            AddMultiSelectFilter(q,cmd,lstDesignation,"E.EmpDesignation","Designation");
            AddMultiSelectFilter(q,cmd,lstPostingPlace,"E.EmpPostingPlace","PostingPlace");
            if(!string.IsNullOrWhiteSpace(rblPostingPlace.SelectedValue)){q.Append(" AND P.EmpPostingPlace=@DetailPlace");cmd.Parameters.Add(new SqlParameter("@DetailPlace",rblPostingPlace.SelectedValue));}
            AddMultiSelectFilter(q,cmd,lstPostingDepartment,"P.EmpPostingDepartment","Department");
            AddMultiSelectFilter(q,cmd,lstAreaBoardZone,"P.AreaBoardZone","AreaBoard");
            AddMultiSelectFilter(q,cmd,lstCircle,"P.Circle","Circle");
            AddMultiSelectFilter(q,cmd,lstDivision,"P.Division","Division");
            AddMultiSelectFilter(q,cmd,lstSubdivision,"P.Subdivision","Subdivision");
            AddMultiSelectFilter(q,cmd,lstSection,"P.Section","Section");
            if(reportType=="Attended") q.Append(" AND TD.TrainingID IS NOT NULL AND EXISTS (SELECT 1 FROM SessionAttendance SA2 WHERE SA2.TrainingID=TD.TrainingID AND SA2.EmpID=E.EmpID AND SA2.AttendanceStatus='Present')");
            else if(reportType=="NotAttended") q.Append(" AND (TD.TrainingID IS NULL OR NOT EXISTS (SELECT 1 FROM SessionAttendance SA3 WHERE SA3.TrainingID=TD.TrainingID AND SA3.EmpID=E.EmpID AND SA3.AttendanceStatus='Present'))");
            q.Append(" ORDER BY E.EmpID,COALESCE(TRY_CONVERT(date,TD.DateFrom,105),TRY_CONVERT(date,TD.DateFrom,23),TRY_CONVERT(date,TD.DateFrom)) DESC,TD.TrainingID");
            cmd.CommandText=q.ToString();
            DataTable dt=new DataTable();
            using(SqlConnection con=new SqlConnection(constr)){cmd.Connection=con;using(SqlDataAdapter da=new SqlDataAdapter(cmd))da.Fill(dt);}
            return dt;
        }

        protected void btnExport_Click(object sender,EventArgs e){string reportType=Convert.ToString(ViewState["ReportType"]);if(string.IsNullOrWhiteSpace(reportType))reportType="All";ExportExcel(GetData(reportType),"EmployeeTrainingReport.xls");}

        private void ExportExcel(DataTable dt,string fileName)
        {
            Response.Clear();Response.Buffer=true;Response.AddHeader("content-disposition","attachment;filename="+fileName);Response.Charset="";Response.ContentType="application/vnd.ms-excel";
            StringBuilder sb=new StringBuilder();foreach(DataColumn c in dt.Columns)sb.Append(c.ColumnName+"\t");sb.Append("\r\n");foreach(DataRow r in dt.Rows){foreach(DataColumn c in dt.Columns)sb.Append(r[c].ToString().Replace("\t"," ")+"\t");sb.Append("\r\n");}Response.Write(sb.ToString());Response.End();
        }

        public override void VerifyRenderingInServerForm(Control control){}
    }
}