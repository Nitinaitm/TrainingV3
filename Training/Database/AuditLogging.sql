IF OBJECT_ID('dbo.UserLoginHistory','U') IS NULL
BEGIN
CREATE TABLE dbo.UserLoginHistory(LoginHistoryID bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,UserID nvarchar(100) NOT NULL,UserRole nvarchar(50) NULL,LoginTime datetime NOT NULL CONSTRAINT DF_UserLoginHistory_LoginTime DEFAULT(GETDATE()),LogoutTime datetime NULL,LoginStatus varchar(20) NOT NULL,FailureReason nvarchar(500) NULL,IPAddress varchar(100) NULL,UserAgent nvarchar(500) NULL,SessionID nvarchar(100) NULL);
END
GO
CREATE INDEX IX_UserLoginHistory_UserID_LoginTime ON dbo.UserLoginHistory(UserID,LoginTime DESC);
GO
IF OBJECT_ID('dbo.UserActivityLog','U') IS NULL
BEGIN
CREATE TABLE dbo.UserActivityLog(ActivityID bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,UserID nvarchar(100) NOT NULL,UserRole nvarchar(50) NULL,ActionType varchar(30) NOT NULL,Module nvarchar(100) NULL,PageName nvarchar(200) NULL,RecordType nvarchar(100) NULL,RecordID nvarchar(150) NULL,Description nvarchar(1000) NULL,ActivityTime datetime NOT NULL CONSTRAINT DF_UserActivityLog_ActivityTime DEFAULT(GETDATE()),IPAddress varchar(100) NULL,SessionID nvarchar(100) NULL);
END
GO
CREATE INDEX IX_UserActivityLog_UserTime ON dbo.UserActivityLog(UserID,ActivityTime DESC);
GO
IF OBJECT_ID('dbo.AuditTrail','U') IS NULL
BEGIN
CREATE TABLE dbo.AuditTrail(AuditID bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,TableName sysname NOT NULL,RecordID nvarchar(150) NULL,ActionType varchar(10) NOT NULL,OldData nvarchar(max) NULL,NewData nvarchar(max) NULL,ChangedBy nvarchar(100) NULL,ChangedRole nvarchar(50) NULL,ChangedOn datetime NOT NULL CONSTRAINT DF_AuditTrail_ChangedOn DEFAULT(GETDATE()),IPAddress varchar(100) NULL,SessionID nvarchar(100) NULL,SQLLogin nvarchar(128) NULL);
END
GO
GO
IF OBJECT_ID(N'dbo.fn_AuditUserID',N'FN') IS NULL
    EXEC(N'CREATE FUNCTION dbo.fn_AuditUserID() RETURNS nvarchar(100) AS BEGIN RETURN NULL; END');
GO
ALTER FUNCTION dbo.fn_AuditUserID()
RETURNS nvarchar(100)
AS
BEGIN
    RETURN LTRIM(RTRIM(CONVERT(nvarchar(100),SUBSTRING(CONVERT(varchar(128),CONTEXT_INFO()),1,20))));
END
GO
IF OBJECT_ID(N'dbo.fn_AuditUserRole',N'FN') IS NULL
    EXEC(N'CREATE FUNCTION dbo.fn_AuditUserRole() RETURNS nvarchar(50) AS BEGIN RETURN NULL; END');
GO
ALTER FUNCTION dbo.fn_AuditUserRole()
RETURNS nvarchar(50)
AS
BEGIN
    RETURN LTRIM(RTRIM(CONVERT(nvarchar(50),SUBSTRING(CONVERT(varchar(128),CONTEXT_INFO()),21,20))));
END
GO
IF OBJECT_ID(N'dbo.fn_AuditIPAddress',N'FN') IS NULL
    EXEC(N'CREATE FUNCTION dbo.fn_AuditIPAddress() RETURNS varchar(100) AS BEGIN RETURN NULL; END');
GO
ALTER FUNCTION dbo.fn_AuditIPAddress()
RETURNS varchar(100)
AS
BEGIN
    RETURN LTRIM(RTRIM(CONVERT(varchar(100),SUBSTRING(CONVERT(varchar(128),CONTEXT_INFO()),41,45))));
END
GO
IF OBJECT_ID(N'dbo.fn_AuditSessionID',N'FN') IS NULL
    EXEC(N'CREATE FUNCTION dbo.fn_AuditSessionID() RETURNS nvarchar(100) AS BEGIN RETURN NULL; END');
GO
ALTER FUNCTION dbo.fn_AuditSessionID()
RETURNS nvarchar(100)
AS
BEGIN
    RETURN LTRIM(RTRIM(CONVERT(nvarchar(100),SUBSTRING(CONVERT(varchar(128),CONTEXT_INFO()),86,32))));
END
GO
CREATE INDEX IX_AuditTrail_TableRecord ON dbo.AuditTrail(TableName,RecordID,ChangedOn DESC);
GO
IF OBJECT_ID(N'dbo.trg_Audit_EmpBasicMaster',N'TR') IS NOT NULL DROP TRIGGER dbo.trg_Audit_EmpBasicMaster;
CREATE TRIGGER dbo.trg_Audit_EmpBasicMaster ON dbo.EmpBasicMaster AFTER INSERT,UPDATE,DELETE AS
BEGIN
SET NOCOUNT ON;
INSERT dbo.AuditTrail(TableName,RecordID,ActionType,OldData,NewData,ChangedBy,ChangedRole,IPAddress,SessionID,SQLLogin)
SELECT 'EmpBasicMaster',CONVERT(nvarchar(150),COALESCE(i.ID,d.ID)),CASE WHEN i.ID IS NULL THEN 'DELETE' WHEN d.ID IS NULL THEN 'INSERT' ELSE 'UPDATE' END,o.OldData,n.NewData,dbo.fn_AuditUserID(),dbo.fn_AuditUserRole(),dbo.fn_AuditIPAddress(),dbo.fn_AuditSessionID(),ORIGINAL_LOGIN()
FROM inserted i FULL OUTER JOIN deleted d ON i.ID=d.ID
OUTER APPLY(SELECT CONVERT(nvarchar(max),(SELECT d.* FOR XML PATH('Row'),TYPE)) AS OldData)o
OUTER APPLY(SELECT CONVERT(nvarchar(max),(SELECT i.* FOR XML PATH('Row'),TYPE)) AS NewData)n;
END
GO
IF OBJECT_ID(N'dbo.trg_Audit_EmpPostingDetails',N'TR') IS NOT NULL DROP TRIGGER dbo.trg_Audit_EmpPostingDetails;
CREATE TRIGGER dbo.trg_Audit_EmpPostingDetails ON dbo.EmpPostingDetails AFTER INSERT,UPDATE,DELETE AS
BEGIN
SET NOCOUNT ON;
INSERT dbo.AuditTrail(TableName,RecordID,ActionType,OldData,NewData,ChangedBy,ChangedRole,IPAddress,SessionID,SQLLogin)
SELECT 'EmpPostingDetails',CONVERT(nvarchar(150),COALESCE(i.ID,d.ID)),CASE WHEN i.ID IS NULL THEN 'DELETE' WHEN d.ID IS NULL THEN 'INSERT' ELSE 'UPDATE' END,CONVERT(nvarchar(max),o.OldData),CONVERT(nvarchar(max),n.NewData),dbo.fn_AuditUserID(),dbo.fn_AuditUserRole(),dbo.fn_AuditIPAddress(),dbo.fn_AuditSessionID(),ORIGINAL_LOGIN()
FROM inserted i FULL OUTER JOIN deleted d ON i.ID=d.ID
OUTER APPLY(SELECT (SELECT d.* FOR XML PATH('Row'),TYPE) AS X)o(OldData)
OUTER APPLY(SELECT (SELECT i.* FOR XML PATH('Row'),TYPE) AS X)n(NewData);
END
GO
IF OBJECT_ID(N'dbo.trg_Audit_TrainingDetails',N'TR') IS NOT NULL DROP TRIGGER dbo.trg_Audit_TrainingDetails;
GO
CREATE TRIGGER dbo.trg_Audit_TrainingDetails ON dbo.TrainingDetails AFTER INSERT,UPDATE,DELETE AS
BEGIN
SET NOCOUNT ON;
INSERT dbo.AuditTrail(TableName,RecordID,ActionType,OldData,NewData,ChangedBy,ChangedRole,IPAddress,SessionID,SQLLogin)
SELECT 'TrainingDetails',CONVERT(nvarchar(150),COALESCE(i.ID,d.ID)),CASE WHEN i.ID IS NULL THEN 'DELETE' WHEN d.ID IS NULL THEN 'INSERT' ELSE 'UPDATE' END,CONVERT(nvarchar(max),o.OldData),CONVERT(nvarchar(max),n.NewData),dbo.fn_AuditUserID(),dbo.fn_AuditUserRole(),dbo.fn_AuditIPAddress(),dbo.fn_AuditSessionID(),ORIGINAL_LOGIN()
FROM inserted i FULL OUTER JOIN deleted d ON i.ID=d.ID
OUTER APPLY(SELECT (SELECT d.* FOR XML PATH('Row'),TYPE) AS X)o(OldData)
OUTER APPLY(SELECT (SELECT i.* FOR XML PATH('Row'),TYPE) AS X)n(NewData);
END
GO
IF OBJECT_ID(N'dbo.trg_Audit_TrainingAssignment',N'TR') IS NOT NULL DROP TRIGGER dbo.trg_Audit_TrainingAssignment;
CREATE TRIGGER dbo.trg_Audit_TrainingAssignment ON dbo.TrainingAssignment AFTER INSERT,UPDATE,DELETE AS
BEGIN
SET NOCOUNT ON;
INSERT dbo.AuditTrail(TableName,RecordID,ActionType,OldData,NewData,ChangedBy,ChangedRole,IPAddress,SessionID,SQLLogin)
SELECT 'TrainingAssignment',CONVERT(nvarchar(150),COALESCE(i.ID,d.ID)),CASE WHEN i.ID IS NULL THEN 'DELETE' WHEN d.ID IS NULL THEN 'INSERT' ELSE 'UPDATE' END,CONVERT(nvarchar(max),o.OldData),CONVERT(nvarchar(max),n.NewData),dbo.fn_AuditUserID(),dbo.fn_AuditUserRole(),dbo.fn_AuditIPAddress(),dbo.fn_AuditSessionID(),ORIGINAL_LOGIN()
FROM inserted i FULL OUTER JOIN deleted d ON i.ID=d.ID
OUTER APPLY(SELECT (SELECT d.* FOR XML PATH('Row'),TYPE) AS X)o(OldData)
OUTER APPLY(SELECT (SELECT i.* FOR XML PATH('Row'),TYPE) AS X)n(NewData);
END
GO
IF OBJECT_ID(N'dbo.trg_Audit_SessionMaster',N'TR') IS NOT NULL DROP TRIGGER dbo.trg_Audit_SessionMaster;
CREATE TRIGGER dbo.trg_Audit_SessionMaster ON dbo.SessionMaster AFTER INSERT,UPDATE,DELETE AS
BEGIN
SET NOCOUNT ON;
INSERT dbo.AuditTrail(TableName,RecordID,ActionType,OldData,NewData,ChangedBy,ChangedRole,IPAddress,SessionID,SQLLogin)
SELECT 'SessionMaster',CONVERT(nvarchar(150),COALESCE(i.ID,d.ID)),CASE WHEN i.ID IS NULL THEN 'DELETE' WHEN d.ID IS NULL THEN 'INSERT' ELSE 'UPDATE' END,CONVERT(nvarchar(max),o.OldData),CONVERT(nvarchar(max),n.NewData),dbo.fn_AuditUserID(),dbo.fn_AuditUserRole(),dbo.fn_AuditIPAddress(),dbo.fn_AuditSessionID(),ORIGINAL_LOGIN()
FROM inserted i FULL OUTER JOIN deleted d ON i.ID=d.ID
OUTER APPLY(SELECT (SELECT d.* FOR XML PATH('Row'),TYPE) AS X)o(OldData)
OUTER APPLY(SELECT (SELECT i.* FOR XML PATH('Row'),TYPE) AS X)n(NewData);
END
GO
IF OBJECT_ID(N'dbo.trg_Audit_SessionAttendance',N'TR') IS NOT NULL DROP TRIGGER dbo.trg_Audit_SessionAttendance;
CREATE TRIGGER dbo.trg_Audit_SessionAttendance ON dbo.SessionAttendance AFTER INSERT,UPDATE,DELETE AS
BEGIN
SET NOCOUNT ON;
INSERT dbo.AuditTrail(TableName,RecordID,ActionType,OldData,NewData,ChangedBy,ChangedRole,IPAddress,SessionID,SQLLogin)
SELECT 'SessionAttendance',CONVERT(nvarchar(150),COALESCE(i.ID,d.ID)),CASE WHEN i.ID IS NULL THEN 'DELETE' WHEN d.ID IS NULL THEN 'INSERT' ELSE 'UPDATE' END,CONVERT(nvarchar(max),o.OldData),CONVERT(nvarchar(max),n.NewData),dbo.fn_AuditUserID(),dbo.fn_AuditUserRole(),dbo.fn_AuditIPAddress(),dbo.fn_AuditSessionID(),ORIGINAL_LOGIN()
FROM inserted i FULL OUTER JOIN deleted d ON i.ID=d.ID
OUTER APPLY(SELECT (SELECT d.* FOR XML PATH('Row'),TYPE) AS X)o(OldData)
OUTER APPLY(SELECT (SELECT i.* FOR XML PATH('Row'),TYPE) AS X)n(NewData);
END
GO
IF OBJECT_ID(N'dbo.trg_Audit_TestMaster',N'TR') IS NOT NULL DROP TRIGGER dbo.trg_Audit_TestMaster;
CREATE TRIGGER dbo.trg_Audit_TestMaster ON dbo.TestMaster AFTER INSERT,UPDATE,DELETE AS
BEGIN
SET NOCOUNT ON;
INSERT dbo.AuditTrail(TableName,RecordID,ActionType,OldData,NewData,ChangedBy,ChangedRole,IPAddress,SessionID,SQLLogin)
SELECT 'TestMaster',CONVERT(nvarchar(150),COALESCE(i.ID,d.ID)),CASE WHEN i.ID IS NULL THEN 'DELETE' WHEN d.ID IS NULL THEN 'INSERT' ELSE 'UPDATE' END,CONVERT(nvarchar(max),o.OldData),CONVERT(nvarchar(max),n.NewData),dbo.fn_AuditUserID(),dbo.fn_AuditUserRole(),dbo.fn_AuditIPAddress(),dbo.fn_AuditSessionID(),ORIGINAL_LOGIN()
FROM inserted i FULL OUTER JOIN deleted d ON i.ID=d.ID
OUTER APPLY(SELECT (SELECT d.* FOR XML PATH('Row'),TYPE) AS X)o(OldData)
OUTER APPLY(SELECT (SELECT i.* FOR XML PATH('Row'),TYPE) AS X)n(NewData);
END
GO
IF OBJECT_ID(N'dbo.trg_Audit_TrainingCertificate',N'TR') IS NOT NULL DROP TRIGGER dbo.trg_Audit_TrainingCertificate;
CREATE TRIGGER dbo.trg_Audit_TrainingCertificate ON dbo.TrainingCertificate AFTER INSERT,UPDATE,DELETE AS
BEGIN
SET NOCOUNT ON;
INSERT dbo.AuditTrail(TableName,RecordID,ActionType,OldData,NewData,ChangedBy,ChangedRole,IPAddress,SessionID,SQLLogin)
SELECT 'TrainingCertificate',CONVERT(nvarchar(150),COALESCE(i.ID,d.ID)),CASE WHEN i.ID IS NULL THEN 'DELETE' WHEN d.ID IS NULL THEN 'INSERT' ELSE 'UPDATE' END,CONVERT(nvarchar(max),o.OldData),CONVERT(nvarchar(max),n.NewData),dbo.fn_AuditUserID(),dbo.fn_AuditUserRole(),dbo.fn_AuditIPAddress(),dbo.fn_AuditSessionID(),ORIGINAL_LOGIN()
FROM inserted i FULL OUTER JOIN deleted d ON i.ID=d.ID
OUTER APPLY(SELECT (SELECT d.* FOR XML PATH('Row'),TYPE) AS X)o(OldData)
OUTER APPLY(SELECT (SELECT i.* FOR XML PATH('Row'),TYPE) AS X)n(NewData);
END
GO
IF OBJECT_ID(N'dbo.trg_Audit_ManagerMaster',N'TR') IS NOT NULL DROP TRIGGER dbo.trg_Audit_ManagerMaster;
CREATE TRIGGER dbo.trg_Audit_ManagerMaster ON dbo.ManagerMaster AFTER INSERT,UPDATE,DELETE AS
BEGIN
SET NOCOUNT ON;
INSERT dbo.AuditTrail(TableName,RecordID,ActionType,OldData,NewData,ChangedBy,ChangedRole,IPAddress,SessionID,SQLLogin)
SELECT 'ManagerMaster',CONVERT(nvarchar(150),COALESCE(i.ID,d.ID)),CASE WHEN i.ID IS NULL THEN 'DELETE' WHEN d.ID IS NULL THEN 'INSERT' ELSE 'UPDATE' END,CONVERT(nvarchar(max),o.OldData),CONVERT(nvarchar(max),n.NewData),dbo.fn_AuditUserID(),dbo.fn_AuditUserRole(),dbo.fn_AuditIPAddress(),dbo.fn_AuditSessionID(),ORIGINAL_LOGIN()
FROM inserted i FULL OUTER JOIN deleted d ON i.ID=d.ID
OUTER APPLY(SELECT (SELECT d.* FOR XML PATH('Row'),TYPE) AS X)o(OldData)
OUTER APPLY(SELECT (SELECT i.* FOR XML PATH('Row'),TYPE) AS X)n(NewData);
END
GO