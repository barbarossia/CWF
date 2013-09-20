SET IDENTITY_INSERT [dbo].[Environment] ON

INSERT [dbo].[Environment] ([Id], [Name],[SoftDelete],[InsertedByUserAlias],[InsertedDateTime],[UpdatedByUserAlias],[UpdatedDateTime]) VALUES (1, N'dev', 0, 'setup', GETDATE(), 'setup', GETDATE())
INSERT [dbo].[Environment] ([Id], [Name],[SoftDelete],[InsertedByUserAlias],[InsertedDateTime],[UpdatedByUserAlias],[UpdatedDateTime]) VALUES (2, N'test', 0, 'setup', GETDATE(), 'setup', GETDATE())
INSERT [dbo].[Environment] ([Id], [Name],[SoftDelete],[InsertedByUserAlias],[InsertedDateTime],[UpdatedByUserAlias],[UpdatedDateTime]) VALUES (3, N'stage', 1, 'setup', GETDATE(), 'setup', GETDATE())
INSERT [dbo].[Environment] ([Id], [Name],[SoftDelete],[InsertedByUserAlias],[InsertedDateTime],[UpdatedByUserAlias],[UpdatedDateTime]) VALUES (4, N'prod', 1, 'setup', GETDATE(), 'setup', GETDATE())

SET IDENTITY_INSERT [dbo].[Environment] OFF

GO

