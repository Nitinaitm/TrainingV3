/*
    Training3 - SQL Server 2012 Audit Context Compatibility

    Run this ONCE against the LOCAL Training3 database after the per-table
    audit triggers have been created.

    Purpose:
    - Replace SQL Server 2016+ SESSION_CONTEXT usage with SQL Server 2012
      compatible CONTEXT_INFO().
    - Keep UserID, UserRole, IPAddress and ASP.NET SessionID available to
      existing AuditTrail and <Table>_Audit triggers.
    - After this migration, Generate Scripts from the local database can be
      used repeatedly for SQL Server 2012 production deployment.

    Context layout (117 bytes):
    1-20   UserID
    21-40  UserRole
    41-85  IPAddress
    86-117 SessionID
*/

USE [Training3];
GO

IF OBJECT_ID(N'dbo.fn_AuditUserID', N'FN') IS NULL
BEGIN
    EXEC(N'CREATE FUNCTION dbo.fn_AuditUserID()
    RETURNS nvarchar(100)
    AS
    BEGIN
        RETURN NULL;
    END');
END
GO

ALTER FUNCTION dbo.fn_AuditUserID()
RETURNS nvarchar(100)
AS
BEGIN
    RETURN LTRIM(RTRIM(CONVERT(nvarchar(100), SUBSTRING(CONVERT(varchar(128), CONTEXT_INFO()), 1, 20))));
END
GO

IF OBJECT_ID(N'dbo.fn_AuditUserRole', N'FN') IS NULL
BEGIN
    EXEC(N'CREATE FUNCTION dbo.fn_AuditUserRole()
    RETURNS nvarchar(50)
    AS
    BEGIN
        RETURN NULL;
    END');
END
GO

ALTER FUNCTION dbo.fn_AuditUserRole()
RETURNS nvarchar(50)
AS
BEGIN
    RETURN LTRIM(RTRIM(CONVERT(nvarchar(50), SUBSTRING(CONVERT(varchar(128), CONTEXT_INFO()), 21, 20))));
END
GO

IF OBJECT_ID(N'dbo.fn_AuditIPAddress', N'FN') IS NULL
BEGIN
    EXEC(N'CREATE FUNCTION dbo.fn_AuditIPAddress()
    RETURNS varchar(100)
    AS
    BEGIN
        RETURN NULL;
    END');
END
GO

ALTER FUNCTION dbo.fn_AuditIPAddress()
RETURNS varchar(100)
AS
BEGIN
    RETURN LTRIM(RTRIM(CONVERT(varchar(100), SUBSTRING(CONVERT(varchar(128), CONTEXT_INFO()), 41, 45))));
END
GO

IF OBJECT_ID(N'dbo.fn_AuditSessionID', N'FN') IS NULL
BEGIN
    EXEC(N'CREATE FUNCTION dbo.fn_AuditSessionID()
    RETURNS nvarchar(100)
    AS
    BEGIN
        RETURN NULL;
    END');
END
GO

ALTER FUNCTION dbo.fn_AuditSessionID()
RETURNS nvarchar(100)
AS
BEGIN
    RETURN LTRIM(RTRIM(CONVERT(nvarchar(100), SUBSTRING(CONVERT(varchar(128), CONTEXT_INFO()), 86, 32))));
END
GO

DECLARE @TriggerName sysname;
DECLARE @SchemaName sysname;
DECLARE @Definition nvarchar(max);
DECLARE @AlterDefinition nvarchar(max);
DECLARE @Sql nvarchar(max);

DECLARE TriggerCursor CURSOR LOCAL FAST_FORWARD FOR
SELECT
    tr.name,
    sc.name,
    sm.definition
FROM sys.triggers tr
INNER JOIN sys.tables tb ON tb.object_id = tr.parent_id
INNER JOIN sys.schemas sc ON sc.schema_id = tb.schema_id
INNER JOIN sys.sql_modules sm ON sm.object_id = tr.object_id
WHERE tr.parent_class = 1
  AND ISNULL(sm.definition, N'') LIKE N'%SESSION_CONTEXT%';

OPEN TriggerCursor;

FETCH NEXT FROM TriggerCursor INTO @TriggerName, @SchemaName, @Definition;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @AlterDefinition = REPLACE(@Definition, N'SESSION_CONTEXT(N''UserID'')', N'dbo.fn_AuditUserID()');
    SET @AlterDefinition = REPLACE(@AlterDefinition, N'SESSION_CONTEXT(N''UserRole'')', N'dbo.fn_AuditUserRole()');
    SET @AlterDefinition = REPLACE(@AlterDefinition, N'SESSION_CONTEXT(N''IPAddress'')', N'dbo.fn_AuditIPAddress()');
    SET @AlterDefinition = REPLACE(@AlterDefinition, N'SESSION_CONTEXT(N''SessionID'')', N'dbo.fn_AuditSessionID()');

    IF CHARINDEX(N'CREATE', @AlterDefinition) > 0
    BEGIN
        SET @AlterDefinition = STUFF(@AlterDefinition, CHARINDEX(N'CREATE', @AlterDefinition), 6, N'ALTER');
        SET @Sql = @AlterDefinition;
        EXEC sp_executesql @Sql;
    END;

    FETCH NEXT FROM TriggerCursor INTO @TriggerName, @SchemaName, @Definition;
END;

CLOSE TriggerCursor;
DEALLOCATE TriggerCursor;
GO

SELECT
    tr.name AS TriggerName,
    tb.name AS TableName,
    CASE
        WHEN sm.definition LIKE N'%SESSION_CONTEXT%' THEN 'NOT CONVERTED'
        ELSE 'SQL 2012 COMPATIBLE'
    END AS AuditContextStatus
FROM sys.triggers tr
INNER JOIN sys.tables tb ON tb.object_id = tr.parent_id
LEFT JOIN sys.sql_modules sm ON sm.object_id = tr.object_id
WHERE tr.parent_class = 1
ORDER BY tb.name, tr.name;
GO
