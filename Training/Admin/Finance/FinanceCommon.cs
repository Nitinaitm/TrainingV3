using System;
using System.Data;
using System.Data.SqlClient;

namespace Training.Admin
{
    public static class FinanceCommon
    {
        public static DataTable GetCostHeads(clsDataAccess db)
        {
            return db.GetDataTable("SELECT CostHeadID,CostHeadCode,CostHeadName,CostLevel,CalculationMode,UnitType,ManualAllowed,OverrideAllowed,Active FROM FinanceCostHeadMaster ORDER BY CostHeadName");
        }

        public static decimal GetRate(clsDataAccess db, int costHeadID, DateTime asOnDate, out int rateID)
        {
            rateID = 0;
            DataTable dt = db.GetDataTable("SELECT TOP 1 RateID,Rate FROM FinanceRateMaster WHERE CostHeadID=@CostHeadID AND Active='Y' AND EffectiveFrom<=@AsOnDate AND (EffectiveTo IS NULL OR EffectiveTo>=@AsOnDate) ORDER BY EffectiveFrom DESC,RateID DESC", new SqlParameter[] { new SqlParameter("@CostHeadID", costHeadID), new SqlParameter("@AsOnDate", asOnDate.Date) });
            if (dt.Rows.Count == 0) return 0;
            rateID = Convert.ToInt32(dt.Rows[0]["RateID"]);
            return Convert.ToDecimal(dt.Rows[0]["Rate"]);
        }

        public static int GetTrainingDays(clsDataAccess db, string trainingID)
        {
            DataTable dt = db.GetDataTable("SELECT TOP 1 DateFrom,DateTo FROM TrainingDetails WHERE TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
            if (dt.Rows.Count == 0) return 0;
            DateTime fromDate;
            DateTime toDate;
            if (!DateTime.TryParse(Convert.ToString(dt.Rows[0]["DateFrom"]), out fromDate)) return 0;
            if (!DateTime.TryParse(Convert.ToString(dt.Rows[0]["DateTo"]), out toDate)) return 0;
            return Math.Max(1,(toDate.Date-fromDate.Date).Days+1);
        }

        public static int GetTraineeCount(clsDataAccess db, string trainingID)
        {
            DataTable dt = db.GetDataTable("SELECT COUNT(*) AS Cnt FROM TrainingAssignment WHERE TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
            return dt.Rows.Count == 0 ? 0 : Convert.ToInt32(dt.Rows[0]["Cnt"]);
        }

        public static int GetSessionTrainerCount(clsDataAccess db, string trainingID, string sessionID)
        {
            DataTable dt = db.GetDataTable("SELECT COUNT(DISTINCT TrainerID) AS Cnt FROM SessionMaster WHERE TrainingID=@TrainingID AND SessionID=@SessionID AND ISNULL(TrainerID,'')<>''", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@SessionID", sessionID) });
            return dt.Rows.Count == 0 ? 0 : Convert.ToInt32(dt.Rows[0]["Cnt"]);
        }

        public static DataTable GetTrainingSessions(clsDataAccess db, string trainingID)
        {
            return db.GetDataTable("SELECT SessionID,ISNULL(SessionName,'') AS SessionName,SessionDate,ISNULL(TrainerID,'') AS TrainerID FROM SessionMaster WHERE TrainingID=@TrainingID ORDER BY TRY_CONVERT(date,SessionDate,105),SessionID", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
        }
    }
}
