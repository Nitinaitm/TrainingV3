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
        public static int GetTrainerCount(clsDataAccess db, string trainingID)
        {
            DataTable dt = db.GetDataTable("SELECT COUNT(DISTINCT TrainerID) AS Cnt FROM TrainingTrainerMapping WHERE TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
            int count = dt.Rows.Count == 0 ? 0 : Convert.ToInt32(dt.Rows[0]["Cnt"]);
            if (count > 0) return count;
            dt = db.GetDataTable("SELECT COUNT(DISTINCT TrainerID) AS Cnt FROM SessionMaster WHERE TrainingID=@TrainingID AND ISNULL(TrainerID,'')<>''", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
            return dt.Rows.Count == 0 ? 0 : Convert.ToInt32(dt.Rows[0]["Cnt"]);
        }

        public static void EnsureCostingForTraining(clsDataAccess db, string trainingID, string createdBy)
        {
            int trainees = GetTraineeCount(db, trainingID);
            int days = GetTrainingDays(db, trainingID);
            int trainers = GetTrainerCount(db, trainingID);
            DataTable heads = db.GetDataTable("SELECT CostHeadID,CostLevel,CalculationMode,UnitType FROM FinanceCostHeadMaster WHERE Active='Y'");
            foreach (DataRow h in heads.Rows)
            {
                int headID = Convert.ToInt32(h["CostHeadID"]);
                string level = Convert.ToString(h["CostLevel"]);
                string mode = Convert.ToString(h["CalculationMode"]);
                string unit = Convert.ToString(h["UnitType"]);
                if (level == "Batch")
                {
                    DataTable exists = db.GetDataTable("SELECT CostingDetailID FROM FinanceCostingDetail WHERE TrainingID=@TrainingID AND CostingLevel='Batch' AND CostHeadID=@CostHeadID", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@CostHeadID", headID) });
                    if (exists.Rows.Count > 0) continue;
                    int rateID;
                    decimal rate = GetRate(db, headID, DateTime.Today, out rateID);
                    decimal qty = 0;
                    if (mode != "Manual")
                    {
                        if (unit == "Trainee-Day") qty = trainees * days;
                        else if (unit == "Trainee") qty = trainees;
                        else if (unit == "Trainer-Day") qty = trainers * days;
                        else if (unit == "Trainer") qty = trainers;
                        else if (unit == "Day") qty = days;
                        else if (unit == "Fixed") qty = 1;
                        else if (unit == "Trainee-Night") qty = GetTraineeHostelCount(db, trainingID) * Math.Max(0, days - 1);
                        else if (unit == "Trainer-Night") qty = GetTrainerHostelCount(db, trainingID) * Math.Max(0, days - 1);
                    }
                    InsertCosting(db, trainingID, null, "Batch", headID, rateID, mode, unit, qty, rate, qty * rate, createdBy);
                }
            }

            DataTable sessions = GetTrainingSessions(db, trainingID);
            foreach (DataRow s in sessions.Rows)
            {
                string sessionID = Convert.ToString(s["SessionID"]);
                DateTime asOn = DateTime.Today;
                DateTime.TryParse(Convert.ToString(s["SessionDate"]), out asOn);
                int sessionTrainers = GetSessionTrainerCount(db, trainingID, sessionID);
                DataTable sessionHeads = db.GetDataTable("SELECT CostHeadID,CalculationMode,UnitType FROM FinanceCostHeadMaster WHERE Active='Y' AND CostLevel='Session'");
                foreach (DataRow h in sessionHeads.Rows)
                {
                    int headID = Convert.ToInt32(h["CostHeadID"]);
                    DataTable exists = db.GetDataTable("SELECT CostingDetailID FROM FinanceCostingDetail WHERE TrainingID=@TrainingID AND SessionID=@SessionID AND CostingLevel='Session' AND CostHeadID=@CostHeadID", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@SessionID", sessionID), new SqlParameter("@CostHeadID", headID) });
                    if (exists.Rows.Count > 0) continue;
                    string mode = Convert.ToString(h["CalculationMode"]);
                    string unit = Convert.ToString(h["UnitType"]);
                    int rateID;
                    decimal rate = GetRate(db, headID, asOn, out rateID);
                    decimal qty = 0;
                    if (mode != "Manual")
                    {
                        if (unit == "Trainer-Day" || unit == "Trainer") qty = sessionTrainers;
                        else if (unit == "Day" || unit == "Fixed") qty = 1;
                        else if (unit == "Trainee") qty = trainees;
                    }
                    InsertCosting(db, trainingID, sessionID, "Session", headID, rateID, mode, unit, qty, rate, qty * rate, createdBy);
                }
            }
        }

        private static void InsertCosting(clsDataAccess db, string trainingID, string sessionID, string level, int headID, int rateID, string mode, string unit, decimal qty, decimal rate, decimal amount, string createdBy)
        {
            db.ExecuteSql("INSERT INTO FinanceCostingDetail(TrainingID,CourseID,SessionID,CostingLevel,CostHeadID,RateID,CalculationMode,UnitType,Quantity,AppliedRate,CalculatedAmount,Remarks,CreatedBy) SELECT @TrainingID,TD.CourseID,@SessionID,@Level,@CostHeadID,@RateID,@Mode,@Unit,@Qty,@Rate,@Amount,'Automatic finance costing',@CreatedBy FROM TrainingDetails TD WHERE TD.TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@SessionID", sessionID == null ? (object)DBNull.Value : sessionID), new SqlParameter("@Level", level), new SqlParameter("@CostHeadID", headID), new SqlParameter("@RateID", rateID == 0 ? (object)DBNull.Value : rateID), new SqlParameter("@Mode", mode), new SqlParameter("@Unit", unit), new SqlParameter("@Qty", qty), new SqlParameter("@Rate", rate), new SqlParameter("@Amount", amount), new SqlParameter("@CreatedBy", createdBy) });
        }

        private static int GetTraineeHostelCount(clsDataAccess db, string trainingID)
        {
            try { DataTable dt = db.GetDataTable("SELECT COUNT(*) Cnt FROM HostelAllotment WHERE TrainingID=@TrainingID AND Status='Allotted'", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) }); return dt.Rows.Count == 0 ? 0 : Convert.ToInt32(dt.Rows[0]["Cnt"]); } catch { return 0; }
        }

        private static int GetTrainerHostelCount(clsDataAccess db, string trainingID)
        {
            try { DataTable dt = db.GetDataTable("SELECT COUNT(DISTINCT HA.EmpID) Cnt FROM HostelAllotment HA INNER JOIN TrainerMaster T ON HA.EmpID=T.EmpID WHERE HA.TrainingID=@TrainingID AND HA.Status='Allotted' AND T.TrainerType='Internal'", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) }); return dt.Rows.Count == 0 ? 0 : Convert.ToInt32(dt.Rows[0]["Cnt"]); } catch { return 0; }
        }

    }
}
