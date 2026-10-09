using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class FinanceFinalReport : System.Web.UI.Page
    {
        private readonly clsDataAccess objDB=new clsDataAccess();

        protected void Page_Load(object sender,EventArgs e)
        {
            if(!IsPostBack){BindTraining();BindCourse();SetFilterVisibility();ShowEmpty();}
        }

        private void BindTraining()
        {
            DataTable dt=objDB.GetDataTable("SELECT TD.TrainingID,TD.TrainingID+' - '+ISNULL(C.CourseName,'')+' - '+ISNULL(TD.Batch,'') TrainingName FROM TrainingDetails TD LEFT JOIN CourseMaster C ON TD.CourseID=C.CourseID ORDER BY TD.TrainingID DESC");
            ddlTraining.DataSource=dt;ddlTraining.DataTextField="TrainingName";ddlTraining.DataValueField="TrainingID";ddlTraining.DataBind();ddlTraining.Items.Insert(0,new ListItem("Select Training / Batch",""));
        }

        private void BindCourse()
        {
            DataTable dt=objDB.GetDataTable("SELECT DISTINCT TD.CourseID,ISNULL(C.CourseName,'') CourseName FROM TrainingDetails TD LEFT JOIN CourseMaster C ON TD.CourseID=C.CourseID WHERE ISNULL(TD.CourseID,'')<>'' ORDER BY CourseName");
            ddlCourse.DataSource=dt;ddlCourse.DataTextField="CourseName";ddlCourse.DataValueField="CourseID";ddlCourse.DataBind();ddlCourse.Items.Insert(0,new ListItem("Select Course",""));
        }

        protected void ddlReportType_SelectedIndexChanged(object sender,EventArgs e){SetFilterVisibility();ShowEmpty();}

        private void SetFilterVisibility(){pnlBatchFilter.Visible=ddlReportType.SelectedValue=="Batch";pnlCourseFilter.Visible=ddlReportType.SelectedValue=="Course";lblReportName.Text=ddlReportType.SelectedValue=="Batch"?"Batch Wise":"Course Wise";}

        protected void btnShow_Click(object sender,EventArgs e)
        {
            if(ddlReportType.SelectedValue=="Batch")
            {
                if(ddlTraining.SelectedValue==""){ShowMessage("Please select Training / Batch.",Color.Red);return;}
                BindBatchReport(ddlTraining.SelectedValue);
            }
            else
            {
                if(ddlCourse.SelectedValue==""){ShowMessage("Please select Course.",Color.Red);return;}
                BindCourseReport(ddlCourse.SelectedValue);
            }
        }

        private void BindBatchReport(string trainingID)
        {
            string q="SELECT ISNULL(C.CourseName,'') CourseName,TD.TrainingID,ISNULL(TD.Batch,'') Batch,1 BatchCount,ISNULL(SC.SessionCost,0) SessionCost,ISNULL(BC.BatchCost,0) BatchCost,ISNULL(SC.SessionCost,0)+ISNULL(BC.BatchCost,0) FinalCost,ISNULL(X.ActualPaid,0) ActualPaid,ISNULL(SC.SessionCost,0)+ISNULL(BC.BatchCost,0)-ISNULL(X.ActualPaid,0) Balance FROM TrainingDetails TD LEFT JOIN CourseMaster C ON TD.CourseID=C.CourseID LEFT JOIN (SELECT TrainingID,SUM(FinalAmount) SessionCost FROM FinanceCostingDetail WHERE CostingLevel='Session' GROUP BY TrainingID) SC ON SC.TrainingID=TD.TrainingID LEFT JOIN (SELECT TrainingID,SUM(FinalAmount) BatchCost FROM FinanceCostingDetail WHERE CostingLevel='Batch' GROUP BY TrainingID) BC ON BC.TrainingID=TD.TrainingID LEFT JOIN (SELECT TrainingID,SUM(Amount) ActualPaid FROM FinanceActualExpenditure GROUP BY TrainingID) X ON X.TrainingID=TD.TrainingID WHERE TD.TrainingID=@TrainingID";
            DataTable dt=objDB.GetDataTable(q,new SqlParameter("@TrainingID",trainingID));BindGrid(dt);SetSummary(dt);
        }

        private void BindCourseReport(string courseID)
        {
            string q="SELECT ISNULL(C.CourseName,'') CourseName,TD.TrainingID,ISNULL(TD.Batch,'') Batch,COUNT(*) OVER() BatchCount,ISNULL(SC.SessionCost,0) SessionCost,ISNULL(BC.BatchCost,0) BatchCost,ISNULL(SC.SessionCost,0)+ISNULL(BC.BatchCost,0) FinalCost,ISNULL(X.ActualPaid,0) ActualPaid,ISNULL(SC.SessionCost,0)+ISNULL(BC.BatchCost,0)-ISNULL(X.ActualPaid,0) Balance FROM TrainingDetails TD LEFT JOIN CourseMaster C ON TD.CourseID=C.CourseID LEFT JOIN (SELECT TrainingID,SUM(FinalAmount) SessionCost FROM FinanceCostingDetail WHERE CostingLevel='Session' GROUP BY TrainingID) SC ON SC.TrainingID=TD.TrainingID LEFT JOIN (SELECT TrainingID,SUM(FinalAmount) BatchCost FROM FinanceCostingDetail WHERE CostingLevel='Batch' GROUP BY TrainingID) BC ON BC.TrainingID=TD.TrainingID LEFT JOIN (SELECT TrainingID,SUM(Amount) ActualPaid FROM FinanceActualExpenditure GROUP BY TrainingID) X ON X.TrainingID=TD.TrainingID WHERE TD.CourseID=@CourseID ORDER BY TD.TrainingID";
            DataTable dt=objDB.GetDataTable(q,new SqlParameter("@CourseID",courseID));BindGrid(dt);SetSummary(dt);
        }

        private void BindGrid(DataTable dt){gvReport.DataSource=dt;gvReport.DataBind();}

        private void SetSummary(DataTable dt)
        {
            decimal final=0,actual=0;
            foreach(DataRow r in dt.Rows){final+=Convert.ToDecimal(r["FinalCost"]);actual+=Convert.ToDecimal(r["ActualPaid"]); }
            lblFinalCost.Text="₹"+final.ToString("N2");lblActual.Text="₹"+actual.ToString("N2");lblBalance.Text="₹"+(final-actual).ToString("N2");
        }

        private void ShowEmpty(){gvReport.DataSource=null;gvReport.DataBind();lblFinalCost.Text="₹0.00";lblActual.Text="₹0.00";lblBalance.Text="₹0.00";}

        protected void gvReport_RowCommand(object sender,GridViewCommandEventArgs e)
        {
            if(e.CommandName!="Details")return;
            Session["FinanceTrainingID"]=e.CommandArgument.ToString();
            Response.Redirect("~/Admin/Finance/FinanceTrainingDetail.aspx");
        }

        private void ShowMessage(string text,Color color){lblMessage.Text=text;lblMessage.ForeColor=color;}
    }
}