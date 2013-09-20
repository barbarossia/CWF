SET IDENTITY_INSERT [dbo].[Role] ON
GO

if (select 1 from Role where Name ='CWF Tenant Viewer') is null
begin
INSERT INTO [dbo].[Role]([Id],[Name],[SoftDelete],[InsertedByUserAlias],[InsertedDateTime],[UpdatedByUserAlias],[UpdatedDateTime])
  VALUES (1, 'CWF Tenant Viewer', 0, 'setup', GETDATE(), 'setup', GETDATE())
end

if (select 1 from Role where Name ='CWF Tenant Author') is null
begin
INSERT INTO [dbo].[Role]([Id],[Name],[SoftDelete],[InsertedByUserAlias],[InsertedDateTime],[UpdatedByUserAlias],[UpdatedDateTime])
  VALUES (2, 'CWF Tenant Author', 0, 'setup', GETDATE(), 'setup', GETDATE())
end

if (select 1 from Role where Name ='CWF Tenant Admin') is null
begin
INSERT INTO [dbo].[Role]([Id],[Name],[SoftDelete],[InsertedByUserAlias],[InsertedDateTime],[UpdatedByUserAlias],[UpdatedDateTime])
  VALUES (3, 'CWF Tenant Admin', 0, 'setup', GETDATE(), 'setup', GETDATE())
end

if (select 1 from Role where Name ='CWF Tenant Env Admin') is null
begin
INSERT INTO [dbo].[Role]([Id],[Name],[SoftDelete],[InsertedByUserAlias],[InsertedDateTime],[UpdatedByUserAlias],[UpdatedDateTime])
  VALUES (4, 'CWF Tenant Env Admin', 0, 'setup', GETDATE(), 'setup', GETDATE())
end

if (select 1 from Role where Name ='CWF Admin') is null
begin
INSERT INTO [dbo].[Role]([Id],[Name],[SoftDelete],[InsertedByUserAlias],[InsertedDateTime],[UpdatedByUserAlias],[UpdatedDateTime])
  VALUES (5, 'CWF Admin', 0, 'setup', GETDATE(), 'setup', GETDATE())
end

if (select 1 from Role where Name ='CWF Tenant Stage Author') is null
begin
INSERT INTO [dbo].[Role]([Id],[Name],[SoftDelete],[InsertedByUserAlias],[InsertedDateTime],[UpdatedByUserAlias],[UpdatedDateTime])
  VALUES (6, 'CWF Tenant Stage Author', 0, 'setup', GETDATE(), 'setup', GETDATE())
end

GO
SET IDENTITY_INSERT [dbo].[Role] OFF
GO