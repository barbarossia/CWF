IF NOT EXISTS(SELECT name FROM sys.columns WHERE Name = 'Environment'
               AND object_id = OBJECT_ID('WorkflowType'))
ALTER TABLE [WorkflowType] ADD [Environment] INT NULL
GO
