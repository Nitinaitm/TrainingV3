/* Run once on the Training database before publishing the communication updates.
   SQL Server 2012 compatible. Existing records remain valid and default to the
   entire batch/training duration when no explicit window was stored. */
IF OBJECT_ID('dbo.Announcement','U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Announcement','TargetTrainingID') IS NULL ALTER TABLE dbo.Announcement ADD TargetTrainingID NVARCHAR(50) NULL;
    IF COL_LENGTH('dbo.Announcement','TargetSessionID') IS NULL ALTER TABLE dbo.Announcement ADD TargetSessionID NVARCHAR(50) NULL;
    IF COL_LENGTH('dbo.Announcement','AvailabilityMode') IS NULL ALTER TABLE dbo.Announcement ADD AvailabilityMode NVARCHAR(20) NOT NULL CONSTRAINT DF_Announcement_AvailabilityMode DEFAULT ('Batch');
    IF COL_LENGTH('dbo.Announcement','EffectiveFrom') IS NULL ALTER TABLE dbo.Announcement ADD EffectiveFrom DATETIME NULL;
    IF COL_LENGTH('dbo.Announcement','EffectiveTo') IS NULL ALTER TABLE dbo.Announcement ADD EffectiveTo DATETIME NULL;
END;
IF OBJECT_ID('dbo.Notification','U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Notification','Title') IS NULL ALTER TABLE dbo.Notification ADD Title NVARCHAR(250) NULL;
    IF COL_LENGTH('dbo.Notification','Audience') IS NULL ALTER TABLE dbo.Notification ADD Audience NVARCHAR(20) NULL;
    IF COL_LENGTH('dbo.Notification','TargetTrainingID') IS NULL ALTER TABLE dbo.Notification ADD TargetTrainingID NVARCHAR(50) NULL;
    IF COL_LENGTH('dbo.Notification','TargetSessionID') IS NULL ALTER TABLE dbo.Notification ADD TargetSessionID NVARCHAR(50) NULL;
    IF COL_LENGTH('dbo.Notification','AvailabilityMode') IS NULL ALTER TABLE dbo.Notification ADD AvailabilityMode NVARCHAR(20) NOT NULL CONSTRAINT DF_Notification_AvailabilityMode DEFAULT ('Batch');
    IF COL_LENGTH('dbo.Notification','EffectiveFrom') IS NULL ALTER TABLE dbo.Notification ADD EffectiveFrom DATETIME NULL;
    IF COL_LENGTH('dbo.Notification','EffectiveTo') IS NULL ALTER TABLE dbo.Notification ADD EffectiveTo DATETIME NULL;
END;
