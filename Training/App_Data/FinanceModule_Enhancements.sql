/*
    Finance module enhancement migration.
    SQL Server 2012 compatible; additive only; no foreign keys.
    Run after FinanceModule.sql. Existing finance records are retained.
*/
SET NOCOUNT ON;
GO

/* Optional rate dimensions allow the most specific configured rate to win. */
IF COL_LENGTH('dbo.FinanceRateMaster','CourseID') IS NULL
    ALTER TABLE dbo.FinanceRateMaster ADD CourseID VARCHAR(50) NULL;
GO
IF COL_LENGTH('dbo.FinanceRateMaster','TrainingID') IS NULL
    ALTER TABLE dbo.FinanceRateMaster ADD TrainingID VARCHAR(50) NULL;
GO
IF COL_LENGTH('dbo.FinanceRateMaster','TrainingLocation') IS NULL
    ALTER TABLE dbo.FinanceRateMaster ADD TrainingLocation VARCHAR(200) NULL;
GO
IF COL_LENGTH('dbo.FinanceRateMaster','ResidentialType') IS NULL
    ALTER TABLE dbo.FinanceRateMaster ADD ResidentialType VARCHAR(20) NULL;
GO
IF COL_LENGTH('dbo.FinanceRateMaster','TrainerID') IS NULL
    ALTER TABLE dbo.FinanceRateMaster ADD TrainerID VARCHAR(50) NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.FinanceRateMaster') AND name='IX_FinanceRateMaster_ScopeDate')
    CREATE INDEX IX_FinanceRateMaster_ScopeDate ON dbo.FinanceRateMaster(CostHeadID,CourseID,TrainingID,TrainingLocation,ResidentialType,TrainerID,EffectiveFrom,EffectiveTo,Active);
GO

/* Preserve the person/type scope of a costing line and explain overrides. */
IF COL_LENGTH('dbo.FinanceCostingDetail','EmpID') IS NULL
    ALTER TABLE dbo.FinanceCostingDetail ADD EmpID VARCHAR(50) NULL;
GO
IF COL_LENGTH('dbo.FinanceCostingDetail','TrainerID') IS NULL
    ALTER TABLE dbo.FinanceCostingDetail ADD TrainerID VARCHAR(50) NULL;
GO
IF COL_LENGTH('dbo.FinanceCostingDetail','ResidentialType') IS NULL
    ALTER TABLE dbo.FinanceCostingDetail ADD ResidentialType VARCHAR(20) NULL;
GO
IF COL_LENGTH('dbo.FinanceCostingDetail','OverrideReason') IS NULL
    ALTER TABLE dbo.FinanceCostingDetail ADD OverrideReason VARCHAR(1000) NULL;
GO
IF COL_LENGTH('dbo.FinanceCostingDetail','ApprovalStatus') IS NULL
    ALTER TABLE dbo.FinanceCostingDetail ADD ApprovalStatus VARCHAR(20) NOT NULL CONSTRAINT DF_FinanceCostingDetail_ApprovalStatus DEFAULT('NotRequired');
GO
IF COL_LENGTH('dbo.FinanceCostingDetail','ApprovedBy') IS NULL
    ALTER TABLE dbo.FinanceCostingDetail ADD ApprovedBy VARCHAR(100) NULL;
GO
IF COL_LENGTH('dbo.FinanceCostingDetail','ApprovedOn') IS NULL
    ALTER TABLE dbo.FinanceCostingDetail ADD ApprovedOn DATETIME NULL;
GO
IF COL_LENGTH('dbo.FinanceCostingDetail','ApprovalRemarks') IS NULL
    ALTER TABLE dbo.FinanceCostingDetail ADD ApprovalRemarks VARCHAR(1000) NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.FinanceCostingDetail') AND name='IX_FinanceCostingDetail_Person')
    CREATE INDEX IX_FinanceCostingDetail_Person ON dbo.FinanceCostingDetail(TrainingID,EmpID,TrainerID,CostHeadID);
GO

/* Append-only audit trail for manual amount changes. */
IF OBJECT_ID('dbo.FinanceCostingOverrideAudit','U') IS NULL
BEGIN
    CREATE TABLE dbo.FinanceCostingOverrideAudit
    (
        AuditID BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        CostingDetailID BIGINT NOT NULL,
        PreviousOverrideAmount DECIMAL(18,2) NULL,
        NewOverrideAmount DECIMAL(18,2) NULL,
        OverrideReason VARCHAR(1000) NOT NULL,
        ChangedBy VARCHAR(100) NULL,
        ChangedOn DATETIME NOT NULL DEFAULT(GETDATE())
    );
    CREATE INDEX IX_FinanceCostingOverrideAudit_Detail ON dbo.FinanceCostingOverrideAudit(CostingDetailID,ChangedOn);
END;
GO

/* Actual hostel use is distinct from a bed being merely allotted. */
IF OBJECT_ID('dbo.FinanceHostelUsage','U') IS NULL
BEGIN
    CREATE TABLE dbo.FinanceHostelUsage
    (
        HostelUsageID BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TrainingID VARCHAR(50) NOT NULL,
        EmpID VARCHAR(50) NOT NULL,
        HostelID INT NULL,
        CheckInDate DATE NOT NULL,
        CheckOutDate DATE NULL,
        ChargeableNights DECIMAL(18,4) NOT NULL DEFAULT(0),
        UsageStatus VARCHAR(20) NOT NULL DEFAULT('Used'),
        Remarks VARCHAR(1000) NULL,
        CreatedOn DATETIME NOT NULL DEFAULT(GETDATE()),
        CreatedBy VARCHAR(100) NULL,
        ModifiedOn DATETIME NULL,
        ModifiedBy VARCHAR(100) NULL
    );
    CREATE INDEX IX_FinanceHostelUsage_TrainingPerson ON dbo.FinanceHostelUsage(TrainingID,EmpID,UsageStatus);
END;
GO

/* Actual expenditure may be attributable to a particular trainee or trainer. */
IF COL_LENGTH('dbo.FinanceActualExpenditure','EmpID') IS NULL
    ALTER TABLE dbo.FinanceActualExpenditure ADD EmpID VARCHAR(50) NULL;
GO
IF COL_LENGTH('dbo.FinanceActualExpenditure','TrainerID') IS NULL
    ALTER TABLE dbo.FinanceActualExpenditure ADD TrainerID VARCHAR(50) NULL;
GO
IF COL_LENGTH('dbo.FinanceActualExpenditure','ResidentialType') IS NULL
    ALTER TABLE dbo.FinanceActualExpenditure ADD ResidentialType VARCHAR(20) NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.FinanceActualExpenditure') AND name='IX_FinanceActualExpenditure_Person')
    CREATE INDEX IX_FinanceActualExpenditure_Person ON dbo.FinanceActualExpenditure(TrainingID,EmpID,TrainerID,CostHeadID);
GO

/* Finance-specific batch configuration avoids assuming that every batch is residential. */
IF OBJECT_ID('dbo.FinanceTrainingSettings','U') IS NULL
BEGIN
    CREATE TABLE dbo.FinanceTrainingSettings
    (
        TrainingID VARCHAR(50) NOT NULL PRIMARY KEY,
        ResidentialType VARCHAR(20) NOT NULL DEFAULT('Unspecified'),
        Notes VARCHAR(1000) NULL,
        ModifiedOn DATETIME NOT NULL DEFAULT(GETDATE()),
        ModifiedBy VARCHAR(100) NULL
    );
END;
GO
