IF NOT EXISTS(SELECT name FROM sys.columns WHERE Name = 'OriginalId'
               AND object_id = OBJECT_ID('ActivityLibrary'))
ALTER TABLE [ActivityLibrary] ADD [OriginalId] BIGINT NULL
GO
