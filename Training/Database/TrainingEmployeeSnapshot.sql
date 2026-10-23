/*
    Historical trainee designation/posting snapshot.
    Run once against the Training database before deploying the report code.
    Snapshots are captured when TrainingDetails enters a completed TrainingStatus or WorkflowStatus.
    Existing completed records are deliberately NOT backfilled from current employee data,
    because current data cannot prove the employee's historical designation/posting.
*/
IF OBJECT_ID('dbo.TrainingEmployeeSnapshot','U') IS NULL
BEGIN
    CREATE TABLE dbo.TrainingEmployeeSnapshot
    (
        TrainingID varchar(50) NOT NULL,
        EmpID varchar(50) NOT NULL,
        EmpName nvarchar(200) NULL,
        EmpDesignation nvarchar(200) NULL,
        EmpCompany nvarchar(200) NULL,
        EmpPostingPlace nvarchar(300) NULL,
        DetailPostingPlace nvarchar(300) NULL,
        EmpPostingDepartment nvarchar(300) NULL,
        AreaBoardZone nvarchar(200) NULL,
        Circle nvarchar(200) NULL,
        Division nvarchar(200) NULL,
        Subdivision nvarchar(200) NULL,
        Section nvarchar(200) NULL,
        SnapshotCapturedOn datetime NOT NULL CONSTRAINT DF_TrainingEmployeeSnapshot_CapturedOn DEFAULT(GETDATE()),
        SnapshotSource varchar(50) NOT NULL CONSTRAINT DF_TrainingEmployeeSnapshot_Source DEFAULT('TrainingCompleted'),
        CONSTRAINT PK_TrainingEmployeeSnapshot PRIMARY KEY (TrainingID, EmpID)
    );
END
GO

IF OBJECT_ID('dbo.trg_TrainingDetails_CaptureEmployeeSnapshot','TR') IS NOT NULL
    DROP TRIGGER dbo.trg_TrainingDetails_CaptureEmployeeSnapshot;
GO

CREATE TRIGGER dbo.trg_TrainingDetails_CaptureEmployeeSnapshot
ON dbo.TrainingDetails
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.TrainingEmployeeSnapshot
    (
        TrainingID,EmpID,EmpName,EmpDesignation,EmpCompany,EmpPostingPlace,
        DetailPostingPlace,EmpPostingDepartment,AreaBoardZone,Circle,Division,
        Subdivision,Section,SnapshotCapturedOn,SnapshotSource
    )
    SELECT
        I.TrainingID,
        TA.EmpID,
        E.EmpName,
        E.EmpDesignation,
        E.EmpCompany,
        E.EmpPostingPlace,
        P.EmpPostingPlace,
        P.EmpPostingDepartment,
        P.AreaBoardZone,
        P.Circle,
        P.Division,
        P.Subdivision,
        P.Section,
        GETDATE(),
        'TrainingCompleted'
    FROM inserted I
    INNER JOIN deleted D ON D.TrainingID=I.TrainingID
    INNER JOIN dbo.TrainingAssignment TA ON TA.TrainingID=I.TrainingID
    INNER JOIN dbo.EmpBasicMaster E ON E.EmpID=TA.EmpID
    OUTER APPLY
    (
        SELECT TOP 1
            EP.EmpPostingPlace,
            EP.EmpPostingDepartment,
            EP.AreaBoardZone,
            EP.Circle,
            EP.Division,
            EP.Subdivision,
            EP.Section
        FROM dbo.EmpPostingDetails EP
        WHERE EP.EmpID=TA.EmpID
        ORDER BY EP.ID DESC
    ) P
    WHERE (ISNULL(I.TrainingStatus,'') IN ('Completed','TrainingCompleted') OR ISNULL(I.WorkflowStatus,'')='ABCDEFGHIJ')
      AND NOT (ISNULL(D.TrainingStatus,'') IN ('Completed','TrainingCompleted') OR ISNULL(D.WorkflowStatus,'')='ABCDEFGHIJ')
      AND ISNULL(TA.Cancelled,0)=0
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.TrainingEmployeeSnapshot S
          WHERE S.TrainingID=I.TrainingID
            AND S.EmpID=TA.EmpID
      );
END
GO
