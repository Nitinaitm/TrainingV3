/*
    Employee Role migration — SQL Server 2012 compatible.
    Run in the Training database before publishing the Role-enabled application.
    If also applying TrainingEmployeeSnapshot.sql, run that script first and do not rerun it after this migration.

    This script is additive and intentionally uses no foreign-key constraints.
    Historical role rows are captured only when a training transitions to final
    completion. Existing completed trainings are not backfilled from today's roles.
*/
IF OBJECT_ID('dbo.RoleMaster','U') IS NULL
BEGIN
    CREATE TABLE dbo.RoleMaster
    (
        RoleID int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        RoleName nvarchar(150) NOT NULL,
        IsActive bit NOT NULL CONSTRAINT DF_RoleMaster_IsActive DEFAULT (1),
        CreatedOn datetime NOT NULL CONSTRAINT DF_RoleMaster_CreatedOn DEFAULT (GETDATE())
    );
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.RoleMaster') AND name='UX_RoleMaster_RoleName')
BEGIN
    CREATE UNIQUE INDEX UX_RoleMaster_RoleName ON dbo.RoleMaster(RoleName);
END
GO
IF OBJECT_ID('dbo.EmpRoleMapping','U') IS NULL
BEGIN
    CREATE TABLE dbo.EmpRoleMapping
    (
        EmpID varchar(50) NOT NULL,
        RoleID int NOT NULL,
        CreatedOn datetime NOT NULL CONSTRAINT DF_EmpRoleMapping_CreatedOn DEFAULT (GETDATE()),
        CreatedBy nvarchar(100) NULL,
        CONSTRAINT PK_EmpRoleMapping PRIMARY KEY (EmpID,RoleID)
    );
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.EmpRoleMapping') AND name='IX_EmpRoleMapping_RoleID_EmpID')
BEGIN
    CREATE INDEX IX_EmpRoleMapping_RoleID_EmpID ON dbo.EmpRoleMapping(RoleID,EmpID);
END
GO
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
        Section nvarchar(300) NULL,
        EmployeeRoles nvarchar(max) NULL,
        SnapshotCapturedOn datetime NOT NULL CONSTRAINT DF_TrainingEmployeeSnapshot_CapturedOn DEFAULT(GETDATE()),
        SnapshotSource varchar(50) NOT NULL CONSTRAINT DF_TrainingEmployeeSnapshot_Source DEFAULT('TrainingCompleted'),
        CONSTRAINT PK_TrainingEmployeeSnapshot PRIMARY KEY (TrainingID, EmpID)
    );
END
GO
IF COL_LENGTH('dbo.TrainingEmployeeSnapshot','EmployeeRoles') IS NULL
BEGIN
    ALTER TABLE dbo.TrainingEmployeeSnapshot ADD EmployeeRoles nvarchar(max) NULL;
END
GO
IF OBJECT_ID('dbo.TrainingEmployeeSnapshotRole','U') IS NULL
BEGIN
    CREATE TABLE dbo.TrainingEmployeeSnapshotRole
    (
        TrainingID varchar(50) NOT NULL,
        EmpID varchar(50) NOT NULL,
        RoleID int NOT NULL,
        RoleName nvarchar(150) NOT NULL,
        SnapshotCapturedOn datetime NOT NULL CONSTRAINT DF_TrainingEmployeeSnapshotRole_CapturedOn DEFAULT (GETDATE()),
        CONSTRAINT PK_TrainingEmployeeSnapshotRole PRIMARY KEY (TrainingID,EmpID,RoleID)
    );
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.TrainingEmployeeSnapshotRole') AND name='IX_TrainingEmployeeSnapshotRole_Role')
BEGIN
    CREATE INDEX IX_TrainingEmployeeSnapshotRole_Role ON dbo.TrainingEmployeeSnapshotRole(RoleID,TrainingID,EmpID);
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
        Subdivision,Section,EmployeeRoles,SnapshotCapturedOn,SnapshotSource
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
        ISNULL(R.EmployeeRoles,''),
        GETDATE(),
        'TrainingCompleted'
    FROM inserted I
    INNER JOIN deleted D ON D.TrainingID=I.TrainingID
    INNER JOIN dbo.TrainingAssignment TA ON TA.TrainingID=I.TrainingID
    INNER JOIN dbo.EmpBasicMaster E ON E.EmpID=TA.EmpID
    OUTER APPLY
    (
        SELECT TOP 1 EP.EmpPostingPlace,EP.EmpPostingDepartment,EP.AreaBoardZone,EP.Circle,EP.Division,EP.Subdivision,EP.Section
        FROM dbo.EmpPostingDetails EP
        WHERE EP.EmpID=TA.EmpID
        ORDER BY EP.ID DESC
    ) P
    OUTER APPLY
    (
        SELECT STUFF((SELECT N', '+RM.RoleName
                      FROM dbo.EmpRoleMapping ERM
                      INNER JOIN dbo.RoleMaster RM ON RM.RoleID=ERM.RoleID
                      WHERE ERM.EmpID=TA.EmpID
                      ORDER BY RM.RoleName
                      FOR XML PATH(''),TYPE).value('.','nvarchar(max)'),1,2,'') AS EmployeeRoles
    ) R
    WHERE (ISNULL(I.TrainingStatus,'') IN ('Completed','TrainingCompleted') OR ISNULL(I.WorkflowStatus,'')='ABCDEFGHIJ')
      AND NOT (ISNULL(D.TrainingStatus,'') IN ('Completed','TrainingCompleted') OR ISNULL(D.WorkflowStatus,'')='ABCDEFGHIJ')
      AND ISNULL(TA.Cancelled,0)=0
      AND NOT EXISTS (SELECT 1 FROM dbo.TrainingEmployeeSnapshot S WHERE S.TrainingID=I.TrainingID AND S.EmpID=TA.EmpID);

    INSERT INTO dbo.TrainingEmployeeSnapshotRole(TrainingID,EmpID,RoleID,RoleName,SnapshotCapturedOn)
    SELECT I.TrainingID,TA.EmpID,ERM.RoleID,RM.RoleName,GETDATE()
    FROM inserted I
    INNER JOIN deleted D ON D.TrainingID=I.TrainingID
    INNER JOIN dbo.TrainingAssignment TA ON TA.TrainingID=I.TrainingID
    INNER JOIN dbo.EmpRoleMapping ERM ON ERM.EmpID=TA.EmpID
    INNER JOIN dbo.RoleMaster RM ON RM.RoleID=ERM.RoleID
    WHERE (ISNULL(I.TrainingStatus,'') IN ('Completed','TrainingCompleted') OR ISNULL(I.WorkflowStatus,'')='ABCDEFGHIJ')
      AND NOT (ISNULL(D.TrainingStatus,'') IN ('Completed','TrainingCompleted') OR ISNULL(D.WorkflowStatus,'')='ABCDEFGHIJ')
      AND ISNULL(TA.Cancelled,0)=0
      AND EXISTS (SELECT 1 FROM dbo.TrainingEmployeeSnapshot S WHERE S.TrainingID=I.TrainingID AND S.EmpID=TA.EmpID)
      AND NOT EXISTS (SELECT 1 FROM dbo.TrainingEmployeeSnapshotRole SR WHERE SR.TrainingID=I.TrainingID AND SR.EmpID=TA.EmpID AND SR.RoleID=ERM.RoleID);
END
GO

/* Verify schema and role mappings after applying the migration. */
SELECT RoleID,RoleName,IsActive FROM dbo.RoleMaster ORDER BY RoleName;
SELECT EmpID,RoleID,CreatedOn,CreatedBy FROM dbo.EmpRoleMapping ORDER BY EmpID,RoleID;
SELECT TrainingID,EmpID,EmployeeRoles,SnapshotSource FROM dbo.TrainingEmployeeSnapshot WHERE EmployeeRoles IS NOT NULL;
