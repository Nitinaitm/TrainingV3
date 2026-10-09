using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;

namespace Training.Admin
{
    public partial class FinanceTrainingDetail : System.Web.UI.Page
    {
        private readonly clsDataAccess objDB=new clsDataAccess();

        protected void Page_Load(object sender,EventArgs e)
        {
            if(!IsPostBack)
            {
                if(Session["FinanceTrainingID"]==null){ShowMessage("Training selection not found. Please open this page from Final Finance Report.",Color.Red);return;}
                BindAll(Session["FinanceTrainingID"].ToString());
            }
        }

        private void BindAll(string trainingID)
        {
            FinanceCommon.EnsureCostingForTraining(objDB,trainingID,Convert.ToString(Session["UserID"]));
            DataTable h=objDB.GetDataTable("SELECT TD.TrainingID,ISNULL(C.CourseName,'') CourseName,ISNULL(TD.Batch,'') Batch,ISNULL(TD.NoOfDays,0) NoOfDays,COUNT(DISTINCT TA.EmpID) TraineeCount,COUNT(DISTINCT TTM.TrainerID) TrainerCount FROM TrainingDetails TD LEFT JOIN CourseMaster C ON TD.CourseID=C.CourseID LEFT JOIN TrainingAssignment TA ON TD.TrainingID=TA.TrainingID AND ISNULL(TA.Cancelled,0)=0 LEFT JOIN TrainingTrainerMapping TTM ON TD.TrainingID=TTM.TrainingID WHERE TD.TrainingID=@TrainingID GROUP BY TD.TrainingID,C.CourseName,TD.Batch,TD.NoOfDays",new SqlParameter("@TrainingID",trainingID));
            if(h.Rows.Count==0){ShowMessage("Training not found.",Color.Red);return;}
            DataRow r=h.Rows[0];lblTrainingID.Text=r["TrainingID"].ToString();lblCourse.Text=r["CourseName"].ToString();lblBatch.Text=r["Batch"].ToString();lblDuration.Text=r["NoOfDays"].ToString()+" Day(s)";lblTrainees.Text=r["TraineeCount"].ToString();lblTrainers.Text=r["TrainerCount"].ToString();

            DataTable s=objDB.GetDataTable("SELECT ISNULL(SM.SessionName,'') SessionName,SM.SessionDate,H.CostHeadName,D.UnitType,D.Quantity,D.AppliedRate,D.CalculatedAmount,D.FinalAmount FROM FinanceCostingDetail D INNER JOIN FinanceCostHeadMaster H ON D.CostHeadID=H.CostHeadID LEFT JOIN SessionMaster SM ON D.SessionID=SM.SessionID WHERE D.TrainingID=@TrainingID AND D.CostingLevel='Session' ORDER BY SM.SessionDate,SM.SessionNo,H.CostHeadName",new SqlParameter("@TrainingID",trainingID));gvSession.DataSource=s;gvSession.DataBind();
            DataTable b=objDB.GetDataTable("SELECT H.CostHeadName,D.UnitType,D.Quantity,D.AppliedRate,D.CalculatedAmount,D.FinalAmount FROM FinanceCostingDetail D INNER JOIN FinanceCostHeadMaster H ON D.CostHeadID=H.CostHeadID WHERE D.TrainingID=@TrainingID AND D.CostingLevel='Batch' ORDER BY H.CostHeadName",new SqlParameter("@TrainingID",trainingID));gvBatch.DataSource=b;gvBatch.DataBind();

            decimal sessionTotal=Sum(s,"FinalAmount"),batchTotal=Sum(b,"FinalAmount");
            lblFinalCost.Text="₹"+(sessionTotal+batchTotal).ToString("N2");
            DataTable actual=objDB.GetDataTable("SELECT X.ExpenditureDate,H.CostHeadName,ISNULL(SM.SessionName,'Batch Level') SessionName,X.VendorName,X.BillReference,X.Amount FROM FinanceActualExpenditure X LEFT JOIN FinanceCostHeadMaster H ON X.CostHeadID=H.CostHeadID LEFT JOIN SessionMaster SM ON X.SessionID=SM.SessionID WHERE X.TrainingID=@TrainingID ORDER BY X.ExpenditureDate,X.ExpenditureID",new SqlParameter("@TrainingID",trainingID));gvActual.DataSource=actual;gvActual.DataBind();lblActual.Text="₹"+Sum(actual,"Amount").ToString("N2");

            BindTrainerCost(trainingID);BindTraineeCost(trainingID);
        }

        private void BindTrainerCost(string trainingID)
        {
            string q = "WITH Trainers AS (SELECT DISTINCT TTM.TrainerID FROM TrainingTrainerMapping TTM WHERE TTM.TrainingID=@TrainingID UNION SELECT DISTINCT SM.TrainerID FROM SessionMaster SM WHERE SM.TrainingID=@TrainingID AND ISNULL(SM.TrainerID,'')<>''), TrainerCount AS (SELECT COUNT(*) Cnt FROM Trainers), SessionCost AS (SELECT SM.TrainerID,D.CostHeadID,H.CostHeadName,SUM(D.FinalAmount) FinalAmount FROM SessionMaster SM INNER JOIN FinanceCostingDetail D ON D.SessionID=SM.SessionID AND D.TrainingID=SM.TrainingID AND D.CostingLevel='Session' INNER JOIN FinanceCostHeadMaster H ON D.CostHeadID=H.CostHeadID WHERE SM.TrainingID=@TrainingID AND ISNULL(SM.TrainerID,'')<>'' AND D.UnitType IN ('Trainer','Trainer-Day','Trainer-Night') GROUP BY SM.TrainerID,D.CostHeadID,H.CostHeadName), BatchCost AS (SELECT T.TrainerID,D.CostHeadID,H.CostHeadName,CAST(D.FinalAmount/NULLIF(CAST(TC.Cnt AS decimal(18,2)),0) AS decimal(18,2)) FinalAmount FROM Trainers T CROSS JOIN TrainerCount TC INNER JOIN FinanceCostingDetail D ON D.TrainingID=@TrainingID AND D.CostingLevel='Batch' INNER JOIN FinanceCostHeadMaster H ON D.CostHeadID=H.CostHeadID WHERE D.UnitType IN ('Trainer','Trainer-Day','Trainer-Night')) SELECT T.TrainerID,CASE WHEN TM.TrainerType='Internal' THEN ISNULL(E.EmpName,'') ELSE ISNULL(TM.NameExternal,'') END TrainerName,X.CostHeadName,CAST(SUM(X.FinalAmount) AS decimal(18,2)) FinalAmount FROM Trainers T INNER JOIN TrainerMaster TM ON T.TrainerID=TM.TrainerID LEFT JOIN EmpBasicMaster E ON TM.EmpID=E.EmpID LEFT JOIN (SELECT TrainerID,CostHeadID,CostHeadName,FinalAmount FROM SessionCost UNION ALL SELECT TrainerID,CostHeadID,CostHeadName,FinalAmount FROM BatchCost) X ON X.TrainerID=T.TrainerID GROUP BY T.TrainerID,TM.TrainerType,E.EmpName,TM.NameExternal,X.CostHeadName HAVING SUM(X.FinalAmount)>0 ORDER BY TrainerName,X.CostHeadName";
            DataTable dt=objDB.GetDataTable(q,new SqlParameter("@TrainingID",trainingID));
            gvTrainer.DataSource=dt;
            gvTrainer.DataBind();
        }

        private void BindTraineeCost(string trainingID)
        {
            string q="SELECT TA.EmpID,E.EmpName,H.CostHeadName,CAST(D.FinalAmount/NULLIF(CAST((SELECT COUNT(*) FROM TrainingAssignment TA2 WHERE TA2.TrainingID=D.TrainingID AND ISNULL(TA2.Cancelled,0)=0) AS decimal(18,2)),0) AS decimal(18,2)) FinalAmount FROM TrainingAssignment TA INNER JOIN EmpBasicMaster E ON TA.EmpID=E.EmpID INNER JOIN FinanceCostingDetail D ON D.TrainingID=TA.TrainingID AND D.CostingLevel='Batch' INNER JOIN FinanceCostHeadMaster H ON D.CostHeadID=H.CostHeadID WHERE TA.TrainingID=@TrainingID AND ISNULL(TA.Cancelled,0)=0 AND D.UnitType IN ('Trainee','Trainee-Day','Trainee-Night') ORDER BY E.EmpName,H.CostHeadName";
            DataTable dt=objDB.GetDataTable(q,new SqlParameter("@TrainingID",trainingID));
            gvTrainee.DataSource=dt;
            gvTrainee.DataBind();
        }

        private decimal Sum(DataTable dt,string column){decimal total=0;foreach(DataRow r in dt.Rows)total+=Convert.ToDecimal(r[column]);return total;}

        protected void btnBack_Click(object sender,EventArgs e){Response.Redirect("~/Admin/Finance/FinanceFinalReport.aspx");}
        private void ShowMessage(string text,Color color){lblMessage.Text=text;lblMessage.ForeColor=color;}
    }
}