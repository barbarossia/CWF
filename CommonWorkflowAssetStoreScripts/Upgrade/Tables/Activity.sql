
IF NOT EXISTS(SELECT name FROM sys.columns WHERE Name = 'Environment'
               AND object_id = OBJECT_ID('Activity'))
ALTER TABLE [Activity] ADD [Environment] INT NULL
GO
