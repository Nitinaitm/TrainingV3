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
CREATE INDEX IX_AuditTrail_TableRecord ON dbo.AuditTrail(TableName,RecordID,ChangedOn DESC);
GO
CREATE OR ALTER TRIGGER dbo.trg_Audit_TrainingDetails ON dbo.TrainingDetails AFTER INSERT,UPDATE,DELETE AS
BEGIN
SET NOCOUNT ON;
INSERT dbo.AuditTrail(TableName,RecordID,ActionType,OldData,NewData,ChangedBy,ChangedRole,IPAddress,SessionID,SQLLogin)
SELECT 'TrainingDetails',CONVERT(nvarchar(150),COALESCE(i.ID,d.ID)),CASE WHEN i.ID IS NULL THEN 'DELETE' WHEN d.ID IS NULL THEN 'INSERT' ELSE 'UPDATE' END,o.OldData,n.NewData,CONVERT(nvarchar(100),SESSION_CONTEXT(N'UserID')),CONVERT(nvarchar(50),SESSION_CONTEXT(N'UserRole')),CONVERT(varchar(100),SESSION_CONTEXT(N'IPAddress')),CONVERT(nvarchar(100),SESSION_CONTEXT(N'SessionID')),ORIGINAL_LOGIN()
FROM inserted i FULL OUTER JOIN deleted d ON i.ID=d.ID
OUTER APPLY(SELECT d.* FOR JSON PATH,WITHOUT_ARRAY_WRAPPER)o(OldData)
OUTER APPLY(SELECT i.* FOR JSON PATH,WITHOUT_ARRAY_WRAPPER)n(NewData);
END
GO
CREATE OR ALTER TRIGGER dbo.trg_Audit_TrainingAssignment ON dbo.TrainingAssignment AFTER INSERT,UPDATE,DELETE AS
BEGIN
SET NOCOUNT ON;
INSERT dbo.AuditTrail(TableName,RecordID,ActionType,OldData,NewData,ChangedBy,ChangedRole,IPAddress,SessionID,SQLLogin)
SELECT 'TrainingAssignment',CONVERT(nvarchar(150),COALESCE(i.ID,d.ID)),CASE WHEN i.ID IS NULL THEN 'DELETE' WHEN d.ID IS NULL THEN 'INSERT' ELSE 'UPDATE' END,o.OldData,n.NewData,CONVERT(nvarchar(100),SESSION_CONTEXT(N'UserID')),CONVERT(nvarchar(50),SESSION_CONTEXT(N'UserRole')),CONVERT(varchar(100),SESSION_CONTEXT(N'IPAddress')),CONVERT(nvarchar(100),SESSION_CONTEXT(N'SessionID')),ORIGINAL_LOGIN()
FROM inserted i FULL OUTER JOIN deleted d ON i.ID=d.ID
OUTER APPLY(SELECT d.* FOR JSON PATH,WITHOUT_ARRAY_WRAPPER)o(OldData)
OUTER APPLY(SELECT i.* FOR JSON PATH,WITHOUT_ARRAY_WRAPPER)n(NewData);
END
GO
CREATE OR ALTER TRIGGER dbo.trg_Audit_SessionMaster ON dbo.SessionMaster AFTER INSERT,UPDATE,DELETE AS
BEGIN
SET NOCOUNT ON;
INSERT dbo.AuditTrail(TableName,RecordID,ActionType,OldData,NewData,ChangedBy,ChangedRole,IPAddress,SessionID,SQLLogin)
SELECT 'SessionMaster',CONVERT(nvarchar(150),COALESCE(i.ID,d.ID)),CASE WHEN i.ID IS NULL THEN 'DELETE' WHEN d.ID IS NULL THEN 'INSERT' ELSE 'UPDATE' END,o.OldData,n.NewData,CONVERT(nvarchar(100),SESSION_CONTEXT(N'UserID')),CONVERT(nvarchar(50),SESSION_CONTEXT(N'UserRole')),CONVERT(varchar(100),SESSION_CONTEXT(N'IPAddress')),CONVERT(nvarchar(100),SESSION_CONTEXT(N'SessionID')),ORIGINAL_LOGIN()
FROM inserted i FULL OUTER JOIN deleted d ON i.ID=d.ID
OUTER APPLY(SELECT d.* FOR JSON PATH,WITHOUT_ARRAY_WRAPPER)o(OldData)
OUTER APPLY(SELECT i.* FOR JSON PATH,WITHOUT_ARRAY_WRAPPER)n(NewData);
END
GO
CREATE OR ALTER TRIGGER dbo.trg_Audit_SessionAttendance ON dbo.SessionAttendance AFTER INSERT,UPDATE,DELETE AS
BEGIN
SET NOCOUNT ON;
INSERT dbo.AuditTrail(TableName,RecordID,ActionType,OldData,NewData,ChangedBy,ChangedRole,IPAddress,SessionID,SQLLogin)
SELECT 'SessionAttendance',CONVERT(nvarchar(150),COALESCE(i.ID,d.ID)),CASE WHEN i.ID IS NULL THEN 'DELETE' WHEN d.ID IS NULL THEN 'INSERT' ELSE 'UPDATE' END,o.OldData,n.NewData,CONVERT(nvarchar(100),SESSION_CONTEXT(N'UserID')),CONVERT(nvarchar(50),SESSION_CONTEXT(N'UserRole')),CONVERT(varchar(100),SESSION_CONTEXT(N'IPAddress')),CONVERT(nvarchar(100),SESSION_CONTEXT(N'SessionID')),ORIGINAL_LOGIN()
FROM inserted i FULL OUTER JOIN deleted d ON i.ID=d.ID
OUTER APPLY(SELECT d.* FOR JSON PATH,WITHOUT_ARRAY_WRAPPER)o(OldData)
OUTER APPLY(SELECT i.* FOR JSON PATH,WITHOUT_ARRAY_WRAPPER)n(NewData);
END
GO
CREATE OR ALTER TRIGGER dbo.trg_Audit_TestMaster ON dbo.TestMaster AFTER INSERT,UPDATE,DELETE AS
BEGIN
SET NOCOUNT ON;
INSERT dbo.AuditTrail(TableName,RecordID,ActionType,OldData,NewData,ChangedBy,ChangedRole,IPAddress,SessionID,SQLLogin)
SELECT 'TestMaster',CONVERT(nvarchar(150),COALESCE(i.ID,d.ID)),CASE WHEN i.ID IS NULL THEN 'DELETE' WHEN d.ID IS NULL THEN 'INSERT' ELSE 'UPDATE' END,o.OldData,n.NewData,CONVERT(nvarchar(100),SESSION_CONTEXT(N'UserID')),CONVERT(nvarchar(50),SESSION_CONTEXT(N'UserRole')),CONVERT(varchar(100),SESSION_CONTEXT(N'IPAddress')),CONVERT(nvarchar(100),SESSION_CONTEXT(N'SessionID')),ORIGINAL_LOGIN()
FROM inserted i FULL OUTER JOIN deleted d ON i.ID=d.ID
OUTER APPLY(SELECT d.* FOR JSON PATH,WITHOUT_ARRAY_WRAPPER)o(OldData)
OUTER APPLY(SELECT i.* FOR JSON PATH,WITHOUT_ARRAY_WRAPPER)n(NewData);
END
GO
CREATE OR ALTER TRIGGER dbo.trg_Audit_TrainingCertificate ON dbo.TrainingCertificate AFTER INSERT,UPDATE,DELETE AS
BEGIN
SET NOCOUNT ON;
INSERT dbo.AuditTrail(TableName,RecordID,ActionType,OldData,NewData,ChangedBy,ChangedRole,IPAddress,SessionID,SQLLogin)
SELECT 'TrainingCertificate',CONVERT(nvarchar(150),COALESCE(i.ID,d.ID)),CASE WHEN i.ID IS NULL THEN 'DELETE' WHEN d.ID IS NULL THEN 'INSERT' ELSE 'UPDATE' END,o.OldData,n.NewData,CONVERT(nvarchar(100),SESSION_CONTEXT(N'UserID')),CONVERT(nvarchar(50),SESSION_CONTEXT(N'UserRole')),CONVERT(varchar(100),SESSION_CONTEXT(N'IPAddress')),CONVERT(nvarchar(100),SESSION_CONTEXT(N'SessionID')),ORIGINAL_LOGIN()
FROM inserted i FULL OUTER JOIN deleted d ON i.ID=d.ID
OUTER APPLY(SELECT d.* FOR JSON PATH,WITHOUT_ARRAY_WRAPPER)o(OldData)
OUTER APPLY(SELECT i.* FOR JSON PATH,WITHOUT_ARRAY_WRAPPER)n(NewData);
END
GO
CREATE OR ALTER TRIGGER dbo.trg_Audit_ManagerMaster ON dbo.ManagerMaster AFTER INSERT,UPDATE,DELETE AS
BEGIN
SET NOCOUNT ON;
INSERT dbo.AuditTrail(TableName,RecordID,ActionType,OldData,NewData,ChangedBy,ChangedRole,IPAddress,SessionID,SQLLogin)
SELECT 'ManagerMaster',CONVERT(nvarchar(150),COALESCE(i.ID,d.ID)),CASE WHEN i.ID IS NULL THEN 'DELETE' WHEN d.ID IS NULL THEN 'INSERT' ELSE 'UPDATE' END,o.OldData,n.NewData,CONVERT(nvarchar(100),SESSION_CONTEXT(N'UserID')),CONVERT(nvarchar(50),SESSION_CONTEXT(N'UserRole')),CONVERT(varchar(100),SESSION_CONTEXT(N'IPAddress')),CONVERT(nvarchar(100),SESSION_CONTEXT(N'SessionID')),ORIGINAL_LOGIN()
FROM inserted i FULL OUTER JOIN deleted d ON i.ID=d.ID
OUTER APPLY(SELECT d.* FOR JSON PATH,WITHOUT_ARRAY_WRAPPER)o(OldData)
OUTER APPLY(SELECT i.* FOR JSON PATH,WITHOUT_ARRAY_WRAPPER)n(NewData);
END
GO