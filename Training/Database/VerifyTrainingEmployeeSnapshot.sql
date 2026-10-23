/*
    Read-only verification for TrainingEmployeeSnapshot.
    Run this after TrainingEmployeeSnapshot.sql against the same Training database.
    This script does not insert, update, or delete application data.
*/

SELECT
    CASE WHEN OBJECT_ID('dbo.TrainingEmployeeSnapshot','U') IS NULL THEN 'MISSING' ELSE 'OK' END AS SnapshotTableStatus,
    CASE WHEN OBJECT_ID('dbo.trg_TrainingDetails_CaptureEmployeeSnapshot','TR') IS NULL THEN 'MISSING' ELSE 'PRESENT' END AS SnapshotTriggerStatus;

SELECT
    name AS TriggerName,
    is_disabled AS IsDisabled
FROM sys.triggers
WHERE object_id = OBJECT_ID('dbo.trg_TrainingDetails_CaptureEmployeeSnapshot');

SELECT
    ISNULL(TD.TrainingStatus,'') AS TrainingStatus,
    ISNULL(TD.WorkflowStatus,'') AS WorkflowStatus,
    COUNT(*) AS TrainingCount
FROM dbo.TrainingDetails TD
WHERE ISNULL(TD.TrainingStatus,'') IN ('Completed','TrainingCompleted')
   OR ISNULL(TD.WorkflowStatus,'') = 'ABCDEFGHIJ'
GROUP BY ISNULL(TD.TrainingStatus,''), ISNULL(TD.WorkflowStatus,'')
ORDER BY TrainingStatus, WorkflowStatus;

SELECT
    TD.TrainingID,
    TD.TrainingStatus,
    TD.WorkflowStatus,
    TA.EmpID,
    CASE WHEN E.EmpID IS NULL THEN 'Employee missing from EmpBasicMaster' ELSE 'Employee found' END AS EmployeeMasterStatus,
    CASE WHEN S.EmpID IS NULL THEN 'SNAPSHOT MISSING' ELSE 'Snapshot present' END AS SnapshotStatus
FROM dbo.TrainingDetails TD
INNER JOIN dbo.TrainingAssignment TA
    ON TA.TrainingID = TD.TrainingID
   AND ISNULL(TA.Cancelled,0) = 0
LEFT JOIN dbo.EmpBasicMaster E
    ON E.EmpID = TA.EmpID
LEFT JOIN dbo.TrainingEmployeeSnapshot S
    ON S.TrainingID = TA.TrainingID
   AND S.EmpID = TA.EmpID
WHERE
    (
        ISNULL(TD.TrainingStatus,'') IN ('Completed','TrainingCompleted')
        OR ISNULL(TD.WorkflowStatus,'') = 'ABCDEFGHIJ'
    )
    AND S.EmpID IS NULL
ORDER BY TD.TrainingID, TA.EmpID;

SELECT
    COUNT(*) AS SnapshotRowCount,
    ISNULL(SUM(CASE WHEN ISNULL(EmpDesignation,'') = '' THEN 1 ELSE 0 END),0) AS MissingDesignationCount,
    ISNULL(SUM(CASE WHEN ISNULL(EmpCompany,'') = '' THEN 1 ELSE 0 END),0) AS MissingCompanyCount,
    ISNULL(SUM(CASE WHEN ISNULL(EmpPostingPlace,'') = '' THEN 1 ELSE 0 END),0) AS MissingMasterPostingCount,
    ISNULL(SUM(CASE WHEN ISNULL(DetailPostingPlace,'') = '' THEN 1 ELSE 0 END),0) AS MissingDetailedPostingCount
FROM dbo.TrainingEmployeeSnapshot;

GO

SELECT
    CASE WHEN OBJECT_ID('dbo.RoleMaster','U') IS NOT NULL THEN 'OK' ELSE 'MISSING' END AS RoleMasterStatus,
    CASE WHEN OBJECT_ID('dbo.EmpRoleMapping','U') IS NOT NULL THEN 'OK' ELSE 'MISSING' END AS EmployeeRoleMappingStatus,
    CASE WHEN COL_LENGTH('dbo.TrainingEmployeeSnapshot','EmployeeRoles') IS NOT NULL THEN 'OK' ELSE 'MISSING' END AS SnapshotRoleNamesColumnStatus,
    CASE WHEN COL_LENGTH('dbo.TrainingEmployeeSnapshot','EmployeeRoleIDs') IS NOT NULL THEN 'OK' ELSE 'MISSING' END AS SnapshotRoleIDsColumnStatus;
GO

SELECT
    COUNT(*) AS SnapshotRowCount,
    ISNULL(SUM(CASE WHEN ISNULL(EmployeeRoles,'')='' THEN 1 ELSE 0 END),0) AS SnapshotRowsWithoutRoleNames,
    ISNULL(SUM(CASE WHEN ISNULL(EmployeeRoleIDs,'')='' THEN 1 ELSE 0 END),0) AS SnapshotRowsWithoutRoleIDs
FROM dbo.TrainingEmployeeSnapshot;
GO
