IF OBJECT_ID('dbo.Announcement','U') IS NULL
BEGIN
CREATE TABLE dbo.Announcement
(
    AnnouncementID NVARCHAR(50) NOT NULL CONSTRAINT PK_Announcement PRIMARY KEY,
    TrainerID NVARCHAR(100) NOT NULL,
    Title NVARCHAR(250) NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    Audience NVARCHAR(50) NULL,
    CreatedOn DATETIME NOT NULL CONSTRAINT DF_Announcement_CreatedOn DEFAULT(GETDATE()),
    IsActive BIT NOT NULL CONSTRAINT DF_Announcement_IsActive DEFAULT(1)
);
END;

IF OBJECT_ID('dbo.Notification','U') IS NULL
BEGIN
CREATE TABLE dbo.Notification
(
    NotificationID NVARCHAR(50) NOT NULL CONSTRAINT PK_Notification PRIMARY KEY,
    TrainerID NVARCHAR(100) NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    IsRead BIT NOT NULL CONSTRAINT DF_Notification_IsRead DEFAULT(0),
    CreatedOn DATETIME NOT NULL CONSTRAINT DF_Notification_CreatedOn DEFAULT(GETDATE())
);
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_Announcement_TrainerID_CreatedOn' AND object_id=OBJECT_ID('dbo.Announcement'))
CREATE INDEX IX_Announcement_TrainerID_CreatedOn ON dbo.Announcement(TrainerID,CreatedOn DESC);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_Notification_TrainerID_CreatedOn' AND object_id=OBJECT_ID('dbo.Notification'))
CREATE INDEX IX_Notification_TrainerID_CreatedOn ON dbo.Notification(TrainerID,CreatedOn DESC);