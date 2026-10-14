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
            return GetRate(db, costHeadID, asOnDate, null, null, null, null, null, out rateID);
        }

        public static decimal GetRate(clsDataAccess db, int costHeadID, DateTime asOnDate, string courseID, string trainingID, string location, string residentialType, string trainerID, out int rateID)
        {
            rateID = 0;
            string query = "SELECT TOP 1 RateID,Rate FROM FinanceRateMaster WHERE CostHeadID=@CostHeadID AND Active='Y' AND EffectiveFrom<=@AsOnDate AND (EffectiveTo IS NULL OR EffectiveTo>=@AsOnDate) AND (CourseID IS NULL OR CourseID=@CourseID) AND (TrainingID IS NULL OR TrainingID=@TrainingID) AND (TrainingLocation IS NULL OR TrainingLocation=@Location) AND (ResidentialType IS NULL OR ResidentialType=@ResidentialType) AND (TrainerID IS NULL OR TrainerID=@TrainerID) ORDER BY (CASE WHEN CourseID IS NULL THEN 0 ELSE 1 END + CASE WHEN TrainingID IS NULL THEN 0 ELSE 4 END + CASE WHEN TrainingLocation IS NULL THEN 0 ELSE 2 END + CASE WHEN ResidentialType IS NULL THEN 0 ELSE 1 END + CASE WHEN TrainerID IS NULL THEN 0 ELSE 2 END) DESC,EffectiveFrom DESC,RateID DESC";
            DataTable dt = db.GetDataTable(query, new SqlParameter[] { new SqlParameter("@CostHeadID", costHeadID), new SqlParameter("@AsOnDate", asOnDate.Date), new SqlParameter("@CourseID", (object)courseID ?? DBNull.Value), new SqlParameter("@TrainingID", (object)trainingID ?? DBNull.Value), new SqlParameter("@Location", (object)location ?? DBNull.Value), new SqlParameter("@ResidentialType", (object)residentialType ?? DBNull.Value), new SqlParameter("@TrainerID", (object)trainerID ?? DBNull.Value) });
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
            DataTable dt = db.GetDataTable("SELECT COUNT(DISTINCT EmpID) AS Cnt FROM TrainingAssignment WHERE TrainingID=@TrainingID AND ISNULL(AssignmentStatus,'Assigned')<>'Cancelled' AND ISNULL(Cancelled,0)=0", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
            return dt.Rows.Count == 0 ? 0 : Convert.ToInt32(dt.Rows[0]["Cnt"]);
        }

        public static int GetManagerCount(clsDataAccess db, string location)
        {
            if (String.IsNullOrWhiteSpace(location)) return 0;
            DataTable dt = db.GetDataTable("SELECT COUNT(DISTINCT ManagerID) AS Cnt FROM ManagerMaster WHERE (MapForLocation=@Location OR TrainingLocationID=@Location) AND ISNULL(ActiveStatus,'Active') NOT IN ('N','No','Inactive','Disabled')", new SqlParameter[] { new SqlParameter("@Location", location) });
            return dt.Rows.Count == 0 ? 0 : Convert.ToInt32(dt.Rows[0]["Cnt"]);
        }

        public static int GetTrainerDays(clsDataAccess db, string trainingID)
        {
            DataTable dt = db.GetDataTable("SELECT COUNT(*) AS Cnt FROM (SELECT DISTINCT TrainerID,TRY_CONVERT(date,SessionDate,105) AS SessionDate FROM SessionMaster WHERE TrainingID=@TrainingID AND ISNULL(TrainerID,'')<>'') X WHERE SessionDate IS NOT NULL", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
            int count = dt.Rows.Count == 0 ? 0 : Convert.ToInt32(dt.Rows[0]["Cnt"]);
            if (count > 0) return count;
            return GetTrainerCount(db, trainingID) * GetTrainingDays(db, trainingID);
        }

        public static int GetSessionTrainerCount(clsDataAccess db, string trainingID, string sessionID)
        {
            DataTable dt = db.GetDataTable("SELECT COUNT(DISTINCT TrainerID) AS Cnt FROM SessionMaster WHERE TrainingID=@TrainingID AND SessionID=@SessionID AND ISNULL(TrainerID,'')<>''", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@SessionID", sessionID) });
            return dt.Rows.Count == 0 ? 0 : Convert.ToInt32(dt.Rows[0]["Cnt"]);
        }

        public static DataTable GetTrainingSessions(clsDataAccess db, string trainingID)
        {
            return db.GetDataTable("SELECT SessionID,ISNULL(SessionName,'') AS SessionName,SessionDate,ISNULL(TrainerID,'') AS TrainerID,ISNULL(TotalHours,0) AS SessionHours FROM SessionMaster WHERE TrainingID=@TrainingID ORDER BY TRY_CONVERT(date,SessionDate,105),SessionID", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
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
            int attendedTraineeDays = GetAttendedTraineeDays(db, trainingID);
            int days = GetTrainingDays(db, trainingID);
            int trainers = GetTrainerCount(db, trainingID);
            int trainerDays = GetTrainerDays(db, trainingID);
            DataTable training = db.GetDataTable("SELECT TOP 1 CourseID,ISNULL(TrainingLocation,'') AS TrainingLocation,DateFrom FROM TrainingDetails WHERE TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
            string courseID = training.Rows.Count == 0 ? null : Convert.ToString(training.Rows[0]["CourseID"]);
            string location = training.Rows.Count == 0 ? null : Convert.ToString(training.Rows[0]["TrainingLocation"]);
            DateTime costingDate = DateTime.Today;
            if (training.Rows.Count > 0) DateTime.TryParse(Convert.ToString(training.Rows[0]["DateFrom"]), out costingDate);
            DataTable settings = db.GetDataTable("SELECT ResidentialType FROM FinanceTrainingSettings WHERE TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
            string residentialType = settings.Rows.Count == 0 ? "Unspecified" : Convert.ToString(settings.Rows[0]["ResidentialType"]);
            int managers = GetManagerCount(db, location);
            DataTable heads = db.GetDataTable("SELECT CostHeadID,CostLevel,CalculationMode,UnitType FROM FinanceCostHeadMaster WHERE Active='Y'");
            foreach (DataRow h in heads.Rows)
            {
                int headID = Convert.ToInt32(h["CostHeadID"]);
                string level = Convert.ToString(h["CostLevel"]);
                string mode = Convert.ToString(h["CalculationMode"]);
                string unit = Convert.ToString(h["UnitType"]);
                if (level == "Course" || level == "Batch" || level == "Person")
                {
                    string costingLevel = level == "Course" ? "Course" : "Batch";
                    DataTable exists;
                    if (costingLevel == "Course")
                    {
                        if (String.IsNullOrWhiteSpace(courseID)) continue;
                        exists = db.GetDataTable("SELECT CostingDetailID FROM FinanceCostingDetail WHERE CourseID=@CourseID AND CostingLevel='Course' AND CostHeadID=@CostHeadID", new SqlParameter[] { new SqlParameter("@CourseID", courseID), new SqlParameter("@CostHeadID", headID) });
                    }
                    else
                    {
                        exists = db.GetDataTable("SELECT CostingDetailID FROM FinanceCostingDetail WHERE TrainingID=@TrainingID AND CostingLevel='Batch' AND CostHeadID=@CostHeadID", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@CostHeadID", headID) });
                    }
                    if (exists.Rows.Count > 0) continue;
                    int rateID;
                    decimal rate = GetRate(db, headID, costingDate, courseID, trainingID, location, residentialType == "Unspecified" ? null : residentialType, null, out rateID);
                    decimal qty = 0;
                    if (mode != "Manual")
                    {
                        if (unit == "Trainee-Day") qty = attendedTraineeDays;
                        else if (unit == "Trainee") qty = trainees;
                        else if (unit == "Trainer-Day") qty = trainerDays;
                        else if (unit == "Trainer") qty = trainers;
                        else if (unit == "Manager-Day") qty = managers * days;
                        else if (unit == "Manager") qty = managers;
                        else if (unit == "Trainee-Hour") qty = GetTraineeManHours(db, trainingID);
                        else if (unit == "Day") qty = days;
                        else if (unit == "Fixed") qty = 1;
                        else if (unit == "Trainee-Night") qty = residentialType == "Residential" ? GetTraineeHostelNights(db, trainingID) : 0;
                        else if (unit == "Trainer-Night") qty = residentialType == "Residential" ? GetTrainerHostelNights(db, trainingID) : 0;
                    }
                    InsertCosting(db, trainingID, null, costingLevel, headID, rateID, mode, unit, qty, rate, qty * rate, createdBy);
                }
            }

            DataTable sessions = GetTrainingSessions(db, trainingID);
            foreach (DataRow s in sessions.Rows)
            {
                string sessionID = Convert.ToString(s["SessionID"]);
                DateTime asOn = DateTime.Today;
                DateTime.TryParse(Convert.ToString(s["SessionDate"]), out asOn);
                int sessionTrainers = GetSessionTrainerCount(db, trainingID, sessionID);
                int sessionTrainees = GetSessionPresentCount(db, trainingID, sessionID);
                DataTable sessionHeads = db.GetDataTable("SELECT CostHeadID,CalculationMode,UnitType FROM FinanceCostHeadMaster WHERE Active='Y' AND CostLevel='Session'");
                foreach (DataRow h in sessionHeads.Rows)
                {
                    int headID = Convert.ToInt32(h["CostHeadID"]);
                    DataTable exists = db.GetDataTable("SELECT CostingDetailID FROM FinanceCostingDetail WHERE TrainingID=@TrainingID AND SessionID=@SessionID AND CostingLevel='Session' AND CostHeadID=@CostHeadID", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@SessionID", sessionID), new SqlParameter("@CostHeadID", headID) });
                    if (exists.Rows.Count > 0) continue;
                    string mode = Convert.ToString(h["CalculationMode"]);
                    string unit = Convert.ToString(h["UnitType"]);
                    int rateID;
                    decimal rate = GetRate(db, headID, asOn, courseID, trainingID, location, residentialType == "Unspecified" ? null : residentialType, Convert.ToString(s["TrainerID"]), out rateID);
                    decimal qty = 0;
                    if (mode != "Manual")
                    {
                        if (unit == "Trainer-Day" || unit == "Trainer") qty = sessionTrainers;
                        else if (unit == "Day" || unit == "Fixed" || unit == "Session" || unit == "Session-Day") qty = 1;
                        else if (unit == "Trainee" || unit == "Trainee-Day") qty = sessionTrainees;
                        else if (unit == "Trainee-Hour") qty = sessionTrainees * Convert.ToDecimal(s["SessionHours"]);
                    }
                    InsertCosting(db, trainingID, sessionID, "Session", headID, rateID, mode, unit, qty, rate, qty * rate, createdBy);
                }
            }
        }

        private static void InsertCosting(clsDataAccess db, string trainingID, string sessionID, string level, int headID, int rateID, string mode, string unit, decimal qty, decimal rate, decimal amount, string createdBy)
        {
            db.ExecuteSql("INSERT INTO FinanceCostingDetail(TrainingID,CourseID,SessionID,CostingLevel,CostHeadID,RateID,CalculationMode,UnitType,Quantity,AppliedRate,CalculatedAmount,Remarks,CreatedBy) SELECT @TrainingID,TD.CourseID,@SessionID,@Level,@CostHeadID,@RateID,@Mode,@Unit,@Qty,@Rate,@Amount,'Automatic finance costing',@CreatedBy FROM TrainingDetails TD WHERE TD.TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@SessionID", sessionID == null ? (object)DBNull.Value : sessionID), new SqlParameter("@Level", level), new SqlParameter("@CostHeadID", headID), new SqlParameter("@RateID", rateID == 0 ? (object)DBNull.Value : rateID), new SqlParameter("@Mode", mode), new SqlParameter("@Unit", unit), new SqlParameter("@Qty", qty), new SqlParameter("@Rate", rate), new SqlParameter("@Amount", amount), new SqlParameter("@CreatedBy", createdBy) });
        }

        public static int GetAttendedTraineeDays(clsDataAccess db, string trainingID)
        {
            DataTable dt = db.GetDataTable("SELECT COUNT(*) AS Cnt FROM (SELECT DISTINCT SA.EmpID,TRY_CONVERT(date,SM.SessionDate,105) AS AttendanceDate FROM SessionAttendance SA INNER JOIN SessionMaster SM ON SA.SessionID=SM.SessionID AND SA.TrainingID=SM.TrainingID WHERE SA.TrainingID=@TrainingID AND SA.AttendanceStatus='Present') A", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
            return dt.Rows.Count == 0 ? 0 : Convert.ToInt32(dt.Rows[0]["Cnt"]);
        }

        public static int GetSessionPresentCount(clsDataAccess db, string trainingID, string sessionID)
        {
            DataTable dt = db.GetDataTable("SELECT COUNT(DISTINCT EmpID) AS Cnt FROM SessionAttendance WHERE TrainingID=@TrainingID AND SessionID=@SessionID AND AttendanceStatus='Present'", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@SessionID", sessionID) });
            return dt.Rows.Count == 0 ? 0 : Convert.ToInt32(dt.Rows[0]["Cnt"]);
        }

        public static decimal GetTraineeManHours(clsDataAccess db, string trainingID)
        {
            DataTable dt = db.GetDataTable("SELECT ISNULL(SUM(SM.TotalHours),0) AS TotalHours FROM SessionAttendance SA INNER JOIN SessionMaster SM ON SA.SessionID=SM.SessionID AND SA.TrainingID=SM.TrainingID WHERE SA.TrainingID=@TrainingID AND SA.AttendanceStatus='Present'", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
            return dt.Rows.Count == 0 ? 0 : Convert.ToDecimal(dt.Rows[0]["TotalHours"]);
        }

        private static decimal GetTraineeHostelNights(clsDataAccess db, string trainingID)
        {
            DataTable dt = db.GetDataTable("SELECT ISNULL(SUM(ChargeableNights),0) AS Cnt FROM FinanceHostelUsage WHERE TrainingID=@TrainingID AND UsageStatus='Used' AND EmpID IS NOT NULL", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
            return dt.Rows.Count == 0 ? 0 : Convert.ToDecimal(dt.Rows[0]["Cnt"]);
        }

        private static decimal GetTrainerHostelNights(clsDataAccess db, string trainingID)
        {
            DataTable dt = db.GetDataTable("SELECT ISNULL(SUM(ChargeableNights),0) AS Cnt FROM FinanceHostelUsage WHERE TrainingID=@TrainingID AND UsageStatus='Used' AND TrainerID IS NOT NULL", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
            return dt.Rows.Count == 0 ? 0 : Convert.ToDecimal(dt.Rows[0]["Cnt"]);
        }

    }
}
