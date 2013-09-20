IF NOT EXISTS(SELECT name FROM sys.columns WHERE Name = 'OriginalId'
               AND object_id = OBJECT_ID('WorkflowType'))
ALTER TABLE [WorkflowType] ADD [OriginalId] BIGINT NULL
GO

