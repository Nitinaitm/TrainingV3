using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class FinanceFinalReport : System.Web.UI.Page
    {
        private readonly clsDataAccess objDB = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindTraining();
                BindCourse();
                BindTrainers();
                BindCostHeads();
                SetFilterVisibility();
                ShowEmpty();
            }
        }

        private void BindTraining()
        {
            DataTable dt = objDB.GetDataTable("SELECT TD.TrainingID,TD.TrainingID+' - '+ISNULL(C.CourseName,'')+' - '+ISNULL(TD.Batch,'')+' - '+ISNULL(TD.TrainingLocation,'') AS TrainingName FROM TrainingDetails TD LEFT JOIN CourseMaster C ON TD.CourseID=C.CourseID ORDER BY TD.TrainingID DESC");
            ddlTraining.DataSource = dt;
            ddlTraining.DataTextField = "TrainingName";
            ddlTraining.DataValueField = "TrainingID";
            ddlTraining.DataBind();
            ddlTraining.Items.Insert(0, new ListItem("Select Training / Batch", ""));
        }

        private void BindCourse()
        {
            DataTable dt = objDB.GetDataTable("SELECT DISTINCT TD.CourseID,ISNULL(C.CourseName,'') AS CourseName FROM TrainingDetails TD LEFT JOIN CourseMaster C ON TD.CourseID=C.CourseID WHERE ISNULL(TD.CourseID,'')<>'' ORDER BY CourseName");
            ddlCourse.DataSource = dt;
            ddlCourse.DataTextField = "CourseName";
            ddlCourse.DataValueField = "CourseID";
            ddlCourse.DataBind();
            ddlCourse.Items.Insert(0, new ListItem("Select Course", ""));
        }

        private void BindTrainers()
        {
            DataTable dt = objDB.GetDataTable("SELECT TM.TrainerID,CASE WHEN TM.TrainerType='Internal' THEN ISNULL(E.EmpName,'') ELSE ISNULL(TM.NameExternal,'') END+' ('+TM.TrainerID+')' AS TrainerName FROM TrainerMaster TM LEFT JOIN EmpBasicMaster E ON E.EmpID=TM.EmpID WHERE ISNULL(TM.TrainerID,'')<>'' ORDER BY TrainerName");
            ddlTrainer.DataSource = dt;
            ddlTrainer.DataTextField = "TrainerName";
            ddlTrainer.DataValueField = "TrainerID";
            ddlTrainer.DataBind();
            ddlTrainer.Items.Insert(0, new ListItem("Select Trainer", ""));
        }

        private void BindCostHeads()
        {
            DataTable dt = objDB.GetDataTable("SELECT CostHeadID,CostHeadName FROM FinanceCostHeadMaster WHERE Active='Y' ORDER BY CostHeadName");
            ddlCostHead.DataSource = dt;
            ddlCostHead.DataTextField = "CostHeadName";
            ddlCostHead.DataValueField = "CostHeadID";
            ddlCostHead.DataBind();
            ddlCostHead.Items.Insert(0, new ListItem("All Cost Heads", "ALL"));
        }

        protected void ddlReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetFilterVisibility();
            ShowEmpty();
            lblReportName.Text = ddlReportType.SelectedItem.Text;
        }

        private void SetFilterVisibility()
        {
            pnlBatchFilter.Visible = ddlReportType.SelectedValue == "Batch";
            pnlCourseFilter.Visible = ddlReportType.SelectedValue == "Course";
            pnlTraineeFilter.Visible = ddlReportType.SelectedValue == "Trainee";
            pnlTrainerFilter.Visible = ddlReportType.SelectedValue == "Trainer";
            pnlCostHeadFilter.Visible = ddlReportType.SelectedValue == "CostHead";
            lblReportName.Text = ddlReportType.SelectedItem.Text;
        }

        protected void btnShow_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            if (ddlReportType.SelectedValue == "Batch")
            {
                if (ddlTraining.SelectedValue == "") { ShowMessage("Please select Training / Batch.", Color.Red); return; }
                BindBatchReport(ddlTraining.SelectedValue);
                return;
            }
            if (ddlReportType.SelectedValue == "Course")
            {
                if (ddlCourse.SelectedValue == "") { ShowMessage("Please select Course.", Color.Red); return; }
                BindCourseReport(ddlCourse.SelectedValue);
                return;
            }
            if (ddlReportType.SelectedValue == "Trainee")
            {
                if (String.IsNullOrWhiteSpace(txtEmpID.Text)) { ShowMessage("Please enter an EmpID.", Color.Red); return; }
                BindTraineeReport(txtEmpID.Text.Trim());
                return;
            }
            if (ddlReportType.SelectedValue == "Trainer")
            {
                if (ddlTrainer.SelectedValue == "") { ShowMessage("Please select a trainer.", Color.Red); return; }
                BindTrainerReport(ddlTrainer.SelectedValue);
                return;
            }
            if (ddlReportType.SelectedValue == "CostHead")
            {
                int? costHeadID = ddlCostHead.SelectedValue == "ALL" ? (int?)null : Convert.ToInt32(ddlCostHead.SelectedValue);
                BindCostHeadReport(costHeadID);
            }
        }

        private void EnsureCostingForCourse(string courseID)
        {
            DataTable ids = objDB.GetDataTable("SELECT TrainingID FROM TrainingDetails WHERE CourseID=@CourseID", new SqlParameter[] { new SqlParameter("@CourseID", courseID) });
            foreach (DataRow row in ids.Rows)
            {
                FinanceCommon.EnsureCostingForTraining(objDB, Convert.ToString(row["TrainingID"]), Convert.ToString(Session["UserID"]));
            }
        }

        private void EnsureCostingForTraining(string trainingID)
        {
            FinanceCommon.EnsureCostingForTraining(objDB, trainingID, Convert.ToString(Session["UserID"]));
        }

        private string ReportBaseQuery()
        {
            return "SELECT ISNULL(C.CourseName,'') AS CourseName,TD.TrainingID,ISNULL(TD.Batch,'') AS Batch,ISNULL(FTS.ResidentialType,'Unspecified') AS ResidentialType,CAST('' AS VARCHAR(150)) AS CostHeadName,1 AS BatchCount,ISNULL(CC.CourseCost,0)/NULLIF(CAST(CB.BatchCount AS DECIMAL(18,2)),0) AS CourseCost,ISNULL(SC.SessionCost,0) AS SessionCost,ISNULL(BC.BatchCost,0) AS BatchCost,ISNULL(CC.CourseCost,0)/NULLIF(CAST(CB.BatchCount AS DECIMAL(18,2)),0)+ISNULL(SC.SessionCost,0)+ISNULL(BC.BatchCost,0) AS FinalCost,ISNULL(X.ActualPaid,0) AS ActualPaid,ISNULL(CC.CourseCost,0)/NULLIF(CAST(CB.BatchCount AS DECIMAL(18,2)),0)+ISNULL(SC.SessionCost,0)+ISNULL(BC.BatchCost,0)-ISNULL(X.ActualPaid,0) AS Balance,ISNULL(MH.ManHours,0) AS ManHours FROM TrainingDetails TD LEFT JOIN CourseMaster C ON TD.CourseID=C.CourseID LEFT JOIN FinanceTrainingSettings FTS ON FTS.TrainingID=TD.TrainingID LEFT JOIN (SELECT CourseID,SUM(FinalAmount) AS CourseCost FROM FinanceCostingDetail WHERE CostingLevel='Course' GROUP BY CourseID) CC ON CC.CourseID=TD.CourseID LEFT JOIN (SELECT CourseID,COUNT(*) AS BatchCount FROM TrainingDetails GROUP BY CourseID) CB ON CB.CourseID=TD.CourseID LEFT JOIN (SELECT TrainingID,SUM(FinalAmount) AS SessionCost FROM FinanceCostingDetail WHERE CostingLevel='Session' GROUP BY TrainingID) SC ON SC.TrainingID=TD.TrainingID LEFT JOIN (SELECT TrainingID,SUM(FinalAmount) AS BatchCost FROM FinanceCostingDetail WHERE CostingLevel='Batch' GROUP BY TrainingID) BC ON BC.TrainingID=TD.TrainingID LEFT JOIN (SELECT TrainingID,SUM(Amount) AS ActualPaid FROM FinanceActualExpenditure GROUP BY TrainingID) X ON X.TrainingID=TD.TrainingID LEFT JOIN (SELECT SA.TrainingID,SUM(SM.TotalHours) AS ManHours FROM SessionAttendance SA INNER JOIN SessionMaster SM ON SM.TrainingID=SA.TrainingID AND SM.SessionID=SA.SessionID WHERE SA.AttendanceStatus='Present' GROUP BY SA.TrainingID) MH ON MH.TrainingID=TD.TrainingID";
        }

        private void BindBatchReport(string trainingID)
        {
            EnsureCostingForTraining(trainingID);
            DataTable dt = objDB.GetDataTable(ReportBaseQuery() + " WHERE TD.TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
            BindGrid(dt);
            SetSummary(dt);
        }

        private void BindCourseReport(string courseID)
        {
            EnsureCostingForCourse(courseID);
            DataTable dt = objDB.GetDataTable(ReportBaseQuery() + " WHERE TD.CourseID=@CourseID ORDER BY TD.TrainingID", new SqlParameter[] { new SqlParameter("@CourseID", courseID) });
            foreach (DataRow row in dt.Rows) row["BatchCount"] = dt.Rows.Count;
            BindGrid(dt);
            SetSummary(dt);
        }

        private void BindTraineeReport(string empID)
        {
            DataTable assignments = objDB.GetDataTable("SELECT DISTINCT TA.TrainingID FROM TrainingAssignment TA WHERE TA.EmpID=@EmpID AND ISNULL(TA.AssignmentStatus,'Assigned')<>'Cancelled' AND ISNULL(TA.Cancelled,0)=0", new SqlParameter[] { new SqlParameter("@EmpID", empID) });
            if (assignments.Rows.Count == 0) { ShowEmpty(); ShowMessage("No active course/batch assignments found for this EmpID.", Color.Red); return; }
            foreach (DataRow row in assignments.Rows) EnsureCostingForTraining(Convert.ToString(row["TrainingID"]));
            string sql = "WITH PT AS (SELECT DISTINCT TD.TrainingID,TD.CourseID,ISNULL(C.CourseName,'') AS CourseName,ISNULL(TD.Batch,'') AS Batch,ISNULL(FTS.ResidentialType,'Unspecified') AS ResidentialType FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TD.TrainingID=TA.TrainingID LEFT JOIN CourseMaster C ON C.CourseID=TD.CourseID LEFT JOIN FinanceTrainingSettings FTS ON FTS.TrainingID=TD.TrainingID WHERE TA.EmpID=@EmpID AND ISNULL(TA.AssignmentStatus,'Assigned')<>'Cancelled' AND ISNULL(TA.Cancelled,0)=0), HeadSet AS (SELECT PT.TrainingID,D.CostHeadID FROM PT INNER JOIN FinanceCostingDetail D ON D.TrainingID=PT.TrainingID AND D.CostingLevel IN ('Batch','Session') UNION SELECT PT.TrainingID,D.CostHeadID FROM PT INNER JOIN FinanceCostingDetail D ON D.CourseID=PT.CourseID AND D.CostingLevel='Course'), BatchCost AS (SELECT TrainingID,CostHeadID,SUM(CASE WHEN CostingLevel='Session' THEN FinalAmount ELSE 0 END) AS SessionCost,SUM(CASE WHEN CostingLevel='Batch' THEN FinalAmount ELSE 0 END) AS BatchCost FROM FinanceCostingDetail WHERE CostingLevel IN ('Batch','Session') GROUP BY TrainingID,CostHeadID), CourseCost AS (SELECT CourseID,CostHeadID,SUM(FinalAmount) AS CourseCost FROM FinanceCostingDetail WHERE CostingLevel='Course' GROUP BY CourseID,CostHeadID), BatchCounts AS (SELECT TrainingID,COUNT(DISTINCT EmpID) AS TraineeCount FROM TrainingAssignment WHERE ISNULL(AssignmentStatus,'Assigned')<>'Cancelled' AND ISNULL(Cancelled,0)=0 GROUP BY TrainingID), CourseCounts AS (SELECT TD.CourseID,COUNT(DISTINCT TA.EmpID) AS TraineeCount FROM TrainingDetails TD INNER JOIN TrainingAssignment TA ON TA.TrainingID=TD.TrainingID AND ISNULL(TA.AssignmentStatus,'Assigned')<>'Cancelled' AND ISNULL(TA.Cancelled,0)=0 GROUP BY TD.CourseID), Actual AS (SELECT TrainingID,CostHeadID,SUM(Amount) AS ActualPaid FROM FinanceActualExpenditure WHERE EmpID=@EmpID GROUP BY TrainingID,CostHeadID), MH AS (SELECT SA.TrainingID,SUM(SM.TotalHours) AS ManHours FROM SessionAttendance SA INNER JOIN SessionMaster SM ON SM.TrainingID=SA.TrainingID AND SM.SessionID=SA.SessionID WHERE SA.AttendanceStatus='Present' GROUP BY SA.TrainingID) SELECT PT.CourseName,PT.TrainingID,PT.Batch,PT.ResidentialType,H.CostHeadName,1 AS BatchCount,ISNULL(CC.CourseCost,0)/NULLIF(CAST(CQC.TraineeCount AS DECIMAL(18,2)),0) AS CourseCost,ISNULL(BC.SessionCost,0)/NULLIF(CAST(BCN.TraineeCount AS DECIMAL(18,2)),0) AS SessionCost,ISNULL(BC.BatchCost,0)/NULLIF(CAST(BCN.TraineeCount AS DECIMAL(18,2)),0) AS BatchCost,(ISNULL(CC.CourseCost,0)/NULLIF(CAST(CQC.TraineeCount AS DECIMAL(18,2)),0))+(ISNULL(BC.SessionCost,0)/NULLIF(CAST(BCN.TraineeCount AS DECIMAL(18,2)),0))+(ISNULL(BC.BatchCost,0)/NULLIF(CAST(BCN.TraineeCount AS DECIMAL(18,2)),0)) AS FinalCost,ISNULL(A.ActualPaid,0) AS ActualPaid,(ISNULL(CC.CourseCost,0)/NULLIF(CAST(CQC.TraineeCount AS DECIMAL(18,2)),0))+(ISNULL(BC.SessionCost,0)/NULLIF(CAST(BCN.TraineeCount AS DECIMAL(18,2)),0))+(ISNULL(BC.BatchCost,0)/NULLIF(CAST(BCN.TraineeCount AS DECIMAL(18,2)),0))-ISNULL(A.ActualPaid,0) AS Balance,ISNULL(MH.ManHours,0) AS ManHours FROM PT INNER JOIN HeadSet HS ON HS.TrainingID=PT.TrainingID INNER JOIN FinanceCostHeadMaster H ON H.CostHeadID=HS.CostHeadID LEFT JOIN BatchCost BC ON BC.TrainingID=PT.TrainingID AND BC.CostHeadID=HS.CostHeadID LEFT JOIN CourseCost CC ON CC.CourseID=PT.CourseID AND CC.CostHeadID=HS.CostHeadID LEFT JOIN BatchCounts BCN ON BCN.TrainingID=PT.TrainingID LEFT JOIN CourseCounts CQC ON CQC.CourseID=PT.CourseID LEFT JOIN Actual A ON A.TrainingID=PT.TrainingID AND A.CostHeadID=HS.CostHeadID LEFT JOIN MH ON MH.TrainingID=PT.TrainingID ORDER BY PT.TrainingID,H.CostHeadName";
            DataTable dt = objDB.GetDataTable(sql, new SqlParameter[] { new SqlParameter("@EmpID", empID) });
            BindGrid(dt);
            SetSummary(dt);
        }

        private void BindTrainerReport(string trainerID)
        {
            DataTable assignments = objDB.GetDataTable("SELECT TrainingID FROM TrainingTrainerMapping WHERE TrainerID=@TrainerID UNION SELECT TrainingID FROM SessionMaster WHERE TrainerID=@TrainerID", new SqlParameter[] { new SqlParameter("@TrainerID", trainerID) });
            if (assignments.Rows.Count == 0) { ShowEmpty(); ShowMessage("No course/batch assignment or session found for this trainer.", Color.Red); return; }
            foreach (DataRow row in assignments.Rows) EnsureCostingForTraining(Convert.ToString(row["TrainingID"]));
            string sql = "WITH TrainingTrainers AS (SELECT TrainingID,TrainerID FROM TrainingTrainerMapping UNION SELECT TrainingID,TrainerID FROM SessionMaster WHERE ISNULL(TrainerID,'')<>''), TrainerCounts AS (SELECT TrainingID,COUNT(DISTINCT TrainerID) AS Cnt FROM TrainingTrainers GROUP BY TrainingID), TrainerSessionCounts AS (SELECT TrainingID,SessionID,COUNT(DISTINCT TrainerID) AS Cnt FROM SessionMaster WHERE ISNULL(TrainerID,'')<>'' GROUP BY TrainingID,SessionID), TrainerCost AS (SELECT D.TrainingID,D.CostHeadID,H.CostHeadName,SUM(CASE WHEN D.CostingLevel='Session' THEN D.FinalAmount/NULLIF(CAST(TSC.Cnt AS DECIMAL(18,2)),0) ELSE D.FinalAmount/NULLIF(CAST(TC.Cnt AS DECIMAL(18,2)),0) END) AS FinalCost,SUM(CASE WHEN D.CostingLevel='Session' THEN D.FinalAmount/NULLIF(CAST(TSC.Cnt AS DECIMAL(18,2)),0) ELSE 0 END) AS SessionCost,SUM(CASE WHEN D.CostingLevel='Batch' THEN D.FinalAmount/NULLIF(CAST(TC.Cnt AS DECIMAL(18,2)),0) ELSE 0 END) AS BatchCost FROM FinanceCostingDetail D INNER JOIN FinanceCostHeadMaster H ON H.CostHeadID=D.CostHeadID LEFT JOIN SessionMaster SM ON SM.TrainingID=D.TrainingID AND SM.SessionID=D.SessionID LEFT JOIN TrainerCounts TC ON TC.TrainingID=D.TrainingID LEFT JOIN TrainerSessionCounts TSC ON TSC.TrainingID=D.TrainingID AND TSC.SessionID=D.SessionID WHERE H.UnitType IN ('Trainer','Trainer-Day','Trainer-Night') AND ((D.CostingLevel='Batch') OR (D.CostingLevel='Session' AND SM.TrainerID=@TrainerID)) GROUP BY D.TrainingID,D.CostHeadID,H.CostHeadName), Actual AS (SELECT TrainingID,CostHeadID,SUM(Amount) ActualPaid FROM FinanceActualExpenditure WHERE TrainerID=@TrainerID GROUP BY TrainingID,CostHeadID) SELECT ISNULL(C.CourseName,'') AS CourseName,TD.TrainingID,ISNULL(TD.Batch,'') AS Batch,ISNULL(FTS.ResidentialType,'Unspecified') AS ResidentialType,TC.CostHeadName,1 AS BatchCount,CAST(0 AS DECIMAL(18,2)) AS CourseCost,TC.SessionCost,TC.BatchCost,TC.FinalCost,ISNULL(A.ActualPaid,0) AS ActualPaid,TC.FinalCost-ISNULL(A.ActualPaid,0) AS Balance,ISNULL(MH.ManHours,0) AS ManHours FROM TrainerCost TC INNER JOIN TrainingDetails TD ON TD.TrainingID=TC.TrainingID LEFT JOIN CourseMaster C ON C.CourseID=TD.CourseID LEFT JOIN FinanceTrainingSettings FTS ON FTS.TrainingID=TD.TrainingID LEFT JOIN Actual A ON A.TrainingID=TC.TrainingID AND A.CostHeadID=TC.CostHeadID LEFT JOIN (SELECT SM.TrainingID,SUM(SM.TotalHours) ManHours FROM SessionMaster SM WHERE SM.TrainerID=@TrainerID GROUP BY SM.TrainingID) MH ON MH.TrainingID=TD.TrainingID INNER JOIN TrainingTrainers TT ON TT.TrainingID=TD.TrainingID AND TT.TrainerID=@TrainerID ORDER BY TD.TrainingID,TC.CostHeadName";
            DataTable dt = objDB.GetDataTable(sql, new SqlParameter[] { new SqlParameter("@TrainerID", trainerID) });
            BindGrid(dt);
            SetSummary(dt);
        }

        private void BindCostHeadReport(int? costHeadID)
        {
            DataTable ids = objDB.GetDataTable("SELECT DISTINCT TrainingID FROM TrainingDetails");
            foreach (DataRow row in ids.Rows) EnsureCostingForTraining(Convert.ToString(row["TrainingID"]));
            string sql = "SELECT ISNULL(C.CourseName,'') AS CourseName,TD.TrainingID,ISNULL(TD.Batch,'') AS Batch,ISNULL(FTS.ResidentialType,'Unspecified') AS ResidentialType,H.CostHeadName,1 AS BatchCount,ISNULL(CC.CourseCost,0)/NULLIF(CAST(CB.BatchCount AS DECIMAL(18,2)),0) AS CourseCost,ISNULL(D.SessionCost,0) AS SessionCost,ISNULL(D.BatchCost,0) AS BatchCost,ISNULL(CC.CourseCost,0)/NULLIF(CAST(CB.BatchCount AS DECIMAL(18,2)),0)+ISNULL(D.SessionCost,0)+ISNULL(D.BatchCost,0) AS FinalCost,ISNULL(X.ActualPaid,0) AS ActualPaid,ISNULL(CC.CourseCost,0)/NULLIF(CAST(CB.BatchCount AS DECIMAL(18,2)),0)+ISNULL(D.SessionCost,0)+ISNULL(D.BatchCost,0)-ISNULL(X.ActualPaid,0) AS Balance,ISNULL(MH.ManHours,0) AS ManHours FROM TrainingDetails TD CROSS JOIN FinanceCostHeadMaster H LEFT JOIN CourseMaster C ON C.CourseID=TD.CourseID LEFT JOIN FinanceTrainingSettings FTS ON FTS.TrainingID=TD.TrainingID LEFT JOIN (SELECT TrainingID,CostHeadID,SUM(CASE WHEN CostingLevel='Session' THEN FinalAmount ELSE 0 END) AS SessionCost,SUM(CASE WHEN CostingLevel='Batch' THEN FinalAmount ELSE 0 END) AS BatchCost FROM FinanceCostingDetail WHERE CostingLevel IN ('Session','Batch') GROUP BY TrainingID,CostHeadID) D ON D.TrainingID=TD.TrainingID AND D.CostHeadID=H.CostHeadID LEFT JOIN (SELECT CourseID,CostHeadID,SUM(FinalAmount) AS CourseCost FROM FinanceCostingDetail WHERE CostingLevel='Course' GROUP BY CourseID,CostHeadID) CC ON CC.CourseID=TD.CourseID AND CC.CostHeadID=H.CostHeadID LEFT JOIN (SELECT CourseID,COUNT(*) AS BatchCount FROM TrainingDetails GROUP BY CourseID) CB ON CB.CourseID=TD.CourseID LEFT JOIN (SELECT TrainingID,CostHeadID,SUM(Amount) ActualPaid FROM FinanceActualExpenditure GROUP BY TrainingID,CostHeadID) X ON X.TrainingID=TD.TrainingID AND X.CostHeadID=H.CostHeadID LEFT JOIN (SELECT SA.TrainingID,SUM(SM.TotalHours) ManHours FROM SessionAttendance SA INNER JOIN SessionMaster SM ON SM.TrainingID=SA.TrainingID AND SM.SessionID=SA.SessionID WHERE SA.AttendanceStatus='Present' GROUP BY SA.TrainingID) MH ON MH.TrainingID=TD.TrainingID WHERE H.Active='Y' AND (@CostHeadID IS NULL OR H.CostHeadID=@CostHeadID) ORDER BY C.CourseName,TD.TrainingID,H.CostHeadName";
            SqlParameter headParameter = new SqlParameter("@CostHeadID", SqlDbType.Int);
            headParameter.Value = costHeadID.HasValue ? (object)costHeadID.Value : DBNull.Value;
            DataTable dt = objDB.GetDataTable(sql, new SqlParameter[] { headParameter });
            BindGrid(dt);
            SetSummary(dt);
        }

        private void BindGrid(DataTable dt)
        {
            gvReport.Columns[8].Visible = ddlReportType.SelectedValue != "Trainer";
            gvReport.DataSource = dt;
            gvReport.DataBind();
        }

        private void SetSummary(DataTable dt)
        {
            decimal final = 0;
            decimal actual = 0;
            decimal hours = 0;
            HashSet<string> countedTrainingHours = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (DataRow r in dt.Rows)
            {
                final += Convert.ToDecimal(r["FinalCost"]);
                actual += Convert.ToDecimal(r["ActualPaid"]);
                string trainingID = dt.Columns.Contains("TrainingID") ? Convert.ToString(r["TrainingID"]) : "";
                if (countedTrainingHours.Add(trainingID) && dt.Columns.Contains("ManHours") && r["ManHours"] != DBNull.Value)
                {
                    hours += Convert.ToDecimal(r["ManHours"]);
                }
            }
            lblFinalCost.Text = "Rs. " + final.ToString("N2");
            lblActual.Text = "Rs. " + actual.ToString("N2");
            lblBalance.Text = "Rs. " + (final - actual).ToString("N2");
            lblManHours.Text = hours.ToString("N2");
        }

        private void ShowEmpty()
        {
            gvReport.DataSource = null;
            gvReport.DataBind();
            lblFinalCost.Text = "Rs. 0.00";
            lblActual.Text = "Rs. 0.00";
            lblBalance.Text = "Rs. 0.00";
            lblManHours.Text = "0.00";
        }

        protected void gvReport_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Details") return;
            Session["FinanceTrainingID"] = e.CommandArgument.ToString();
            Response.Redirect("~/Admin/Finance/FinanceTrainingDetail.aspx");
        }

        private void ShowMessage(string text, Color color)
        {
            lblMessage.Text = text;
            lblMessage.ForeColor = color;
        }
    }
}
