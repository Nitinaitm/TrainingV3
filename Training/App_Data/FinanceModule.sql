/*
    Training Finance Module
    Independent from the normal Training workflow.
*/
IF OBJECT_ID('dbo.FinanceCostHeadMaster','U') IS NULL
BEGIN
    CREATE TABLE dbo.FinanceCostHeadMaster
    (
        CostHeadID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        CostHeadCode VARCHAR(30) NOT NULL UNIQUE,
        CostHeadName VARCHAR(150) NOT NULL,
        Description VARCHAR(500) NULL,
        CostLevel VARCHAR(20) NOT NULL DEFAULT('Batch'),
        CalculationMode VARCHAR(30) NOT NULL DEFAULT('Automatic'),
        UnitType VARCHAR(30) NOT NULL DEFAULT('Fixed'),
        ManualAllowed CHAR(1) NOT NULL DEFAULT('Y'),
        OverrideAllowed CHAR(1) NOT NULL DEFAULT('Y'),
        Active CHAR(1) NOT NULL DEFAULT('Y'),
        CreatedOn DATETIME NOT NULL DEFAULT(GETDATE()),
        CreatedBy VARCHAR(100) NULL
    );
END;

IF OBJECT_ID('dbo.FinanceRateMaster','U') IS NULL
BEGIN
    CREATE TABLE dbo.FinanceRateMaster
    (
        RateID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        CostHeadID INT NOT NULL,
        Rate DECIMAL(18,2) NOT NULL DEFAULT(0),
        EffectiveFrom DATE NOT NULL,
        EffectiveTo DATE NULL,
        Active CHAR(1) NOT NULL DEFAULT('Y'),
        Remarks VARCHAR(500) NULL,
        CreatedOn DATETIME NOT NULL DEFAULT(GETDATE()),
        CreatedBy VARCHAR(100) NULL
    );
    CREATE INDEX IX_FinanceRateMaster_CostHeadDate ON dbo.FinanceRateMaster(CostHeadID,EffectiveFrom,EffectiveTo,Active);
END;

IF OBJECT_ID('dbo.FinanceCostingDetail','U') IS NULL
BEGIN
    CREATE TABLE dbo.FinanceCostingDetail
    (
        CostingDetailID BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TrainingID VARCHAR(50) NOT NULL,
        CourseID VARCHAR(50) NULL,
        SessionID VARCHAR(50) NULL,
        CostingLevel VARCHAR(20) NOT NULL,
        CostHeadID INT NOT NULL,
        RateID INT NULL,
        CalculationMode VARCHAR(30) NOT NULL,
        UnitType VARCHAR(30) NULL,
        Quantity DECIMAL(18,4) NOT NULL DEFAULT(0),
        AppliedRate DECIMAL(18,2) NOT NULL DEFAULT(0),
        CalculatedAmount DECIMAL(18,2) NOT NULL DEFAULT(0),
        OverrideAmount DECIMAL(18,2) NULL,
        FinalAmount AS (CONVERT(DECIMAL(18,2),ISNULL(OverrideAmount,CalculatedAmount))) PERSISTED,
        Remarks VARCHAR(1000) NULL,
        CreatedOn DATETIME NOT NULL DEFAULT(GETDATE()),
        CreatedBy VARCHAR(100) NULL,
        ModifiedOn DATETIME NULL,
        ModifiedBy VARCHAR(100) NULL
    );
    CREATE INDEX IX_FinanceCostingDetail_Training ON dbo.FinanceCostingDetail(TrainingID,CostingLevel);
    CREATE INDEX IX_FinanceCostingDetail_Course ON dbo.FinanceCostingDetail(CourseID,CostingLevel);
    CREATE INDEX IX_FinanceCostingDetail_Session ON dbo.FinanceCostingDetail(SessionID,CostingLevel);
END;

IF OBJECT_ID('dbo.FinanceActualExpenditure','U') IS NULL
BEGIN
    CREATE TABLE dbo.FinanceActualExpenditure
    (
        ExpenditureID BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TrainingID VARCHAR(50) NOT NULL,
        CourseID VARCHAR(50) NULL,
        SessionID VARCHAR(50) NULL,
        CostHeadID INT NULL,
        ExpenditureDate DATE NULL,
        Amount DECIMAL(18,2) NOT NULL DEFAULT(0),
        VendorName VARCHAR(200) NULL,
        BillReference VARCHAR(100) NULL,
        Remarks VARCHAR(1000) NULL,
        CreatedOn DATETIME NOT NULL DEFAULT(GETDATE()),
        CreatedBy VARCHAR(100) NULL,
        ModifiedOn DATETIME NULL,
        ModifiedBy VARCHAR(100) NULL
    );
    CREATE INDEX IX_FinanceActualExpenditure_Training ON dbo.FinanceActualExpenditure(TrainingID,SessionID,CostHeadID);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.FinanceCostHeadMaster WHERE CostHeadCode='TRAINER_TEACH')
INSERT INTO dbo.FinanceCostHeadMaster(CostHeadCode,CostHeadName,CostLevel,CalculationMode,UnitType) VALUES('TRAINER_TEACH','Trainer Teaching','Session','Automatic','Trainer-Day');
IF NOT EXISTS (SELECT 1 FROM dbo.FinanceCostHeadMaster WHERE CostHeadCode='MANAGER_COST')
INSERT INTO dbo.FinanceCostHeadMaster(CostHeadCode,CostHeadName,CostLevel,CalculationMode,UnitType) VALUES('MANAGER_COST','Manager Cost','Batch','Automatic','Manager-Day');
IF NOT EXISTS (SELECT 1 FROM dbo.FinanceCostHeadMaster WHERE CostHeadCode='TRAINEE_FOOD')
INSERT INTO dbo.FinanceCostHeadMaster(CostHeadCode,CostHeadName,CostLevel,CalculationMode,UnitType) VALUES('TRAINEE_FOOD','Trainee Food','Batch','Automatic','Trainee-Day');
IF NOT EXISTS (SELECT 1 FROM dbo.FinanceCostHeadMaster WHERE CostHeadCode='TRAINER_FOOD')
INSERT INTO dbo.FinanceCostHeadMaster(CostHeadCode,CostHeadName,CostLevel,CalculationMode,UnitType) VALUES('TRAINER_FOOD','Trainer Food','Batch','Automatic','Trainer-Day');
IF NOT EXISTS (SELECT 1 FROM dbo.FinanceCostHeadMaster WHERE CostHeadCode='TRAINEE_HOSTEL')
INSERT INTO dbo.FinanceCostHeadMaster(CostHeadCode,CostHeadName,CostLevel,CalculationMode,UnitType) VALUES('TRAINEE_HOSTEL','Trainee Accommodation','Batch','Automatic','Trainee-Night');
IF NOT EXISTS (SELECT 1 FROM dbo.FinanceCostHeadMaster WHERE CostHeadCode='TRAINER_HOSTEL')
INSERT INTO dbo.FinanceCostHeadMaster(CostHeadCode,CostHeadName,CostLevel,CalculationMode,UnitType) VALUES('TRAINER_HOSTEL','Trainer Accommodation','Batch','Automatic','Trainer-Night');
IF NOT EXISTS (SELECT 1 FROM dbo.FinanceCostHeadMaster WHERE CostHeadCode='HALL')
INSERT INTO dbo.FinanceCostHeadMaster(CostHeadCode,CostHeadName,CostLevel,CalculationMode,UnitType) VALUES('HALL','Training Hall','Batch','Automatic','Day');
IF NOT EXISTS (SELECT 1 FROM dbo.FinanceCostHeadMaster WHERE CostHeadCode='STUDY_MATERIAL')
INSERT INTO dbo.FinanceCostHeadMaster(CostHeadCode,CostHeadName,CostLevel,CalculationMode,UnitType) VALUES('STUDY_MATERIAL','Study Material','Batch','Automatic','Trainee');
IF NOT EXISTS (SELECT 1 FROM dbo.FinanceCostHeadMaster WHERE CostHeadCode='TRANSPORT')
INSERT INTO dbo.FinanceCostHeadMaster(CostHeadCode,CostHeadName,CostLevel,CalculationMode,UnitType) VALUES('TRANSPORT','Transportation','Batch','Manual','Fixed');
IF NOT EXISTS (SELECT 1 FROM dbo.FinanceCostHeadMaster WHERE CostHeadCode='MISC')
INSERT INTO dbo.FinanceCostHeadMaster(CostHeadCode,CostHeadName,CostLevel,CalculationMode,UnitType) VALUES('MISC','Miscellaneous','Batch','Manual','Fixed');
