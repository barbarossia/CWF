IF NOT EXISTS(SELECT name FROM sys.columns WHERE Name = 'RoleId'
               AND object_id = OBJECT_ID('AuthorizationGroup'))
ALTER TABLE [AuthorizationGroup] ADD [RoleId] INT NULL
GO

IF NOT EXISTS(SELECT name FROM sys.columns WHERE Name = 'Enabled'
               AND object_id = OBJECT_ID('AuthorizationGroup'))
ALTER TABLE [AuthorizationGroup] ADD [Enabled] BIT NOT NULL DEFAULT 0
GO
