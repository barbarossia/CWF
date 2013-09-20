IF NOT EXISTS(SELECT name FROM sys.columns WHERE Name = 'Environment'
               AND object_id = OBJECT_ID('ActivityLibrary'))
ALTER TABLE [ActivityLibrary] ADD [Environment] INT NULL
GO
