DECLARE @Guid [uniqueidentifier] 
DECLARE @Name [nvarchar](50)

DECLARE @AuthoringToolLevel [int]
DECLARE @InsertedByUserAlias  [nvarchar](50)
DECLARE @InsertedDateTime [datetime] 
DECLARE @UpdatedByUserAlias [nvarchar](50) 
DECLARE @UpdatedDateTime [datetime] 
DECLARE @Role [int]
DECLARE @Enabled [bit]

SET @InsertedByUserAlias = 'setup'
SET @InsertedDateTime = GETDATE()
SET @UpdatedByUserAlias = 'setup'
SET @UpdatedDateTime = GETDATE()
SET @AuthoringToolLevel = 0
SET @Name = 'eco_cwf_admin'
SET @Guid = '5B53B2A1-CE70-4FB9-939D-6A19D6F50A1F'
SET @Role = 5
SET @Enabled = 1

IF NOT EXISTS (SELECT [Id] FROM [dbo].[AuthorizationGroup] WHERE [Name] = @Name) 
BEGIN
INSERT INTO [dbo].[AuthorizationGroup]
(Guid, Name, AuthoringToolLevel, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime, RoleId, Enabled)
VALUES (@Guid, @Name, @AuthoringToolLevel, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime, @Role, @Enabled)
END

SET @InsertedByUserAlias = 'setup'
SET @InsertedDateTime = GETDATE()
SET @UpdatedByUserAlias = 'setup'
SET @UpdatedDateTime = GETDATE()
SET @AuthoringToolLevel = 0
SET @Name = 'eco_cwf_tenant1admin'
SET @Guid = '1D3E4D5D-5BC8-42EE-96D6-B24BCFE696F2'
SET @Role = 4
SET @Enabled = 1

IF NOT EXISTS (SELECT [Id] FROM [dbo].[AuthorizationGroup] WHERE [Name] = @Name) 
BEGIN
INSERT INTO [dbo].[AuthorizationGroup]
(Guid, Name, AuthoringToolLevel, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime, RoleId, Enabled)
VALUES (@Guid, @Name, @AuthoringToolLevel, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime, @Role, @Enabled)
END

SET @InsertedByUserAlias = 'setup'
SET @InsertedDateTime = GETDATE()
SET @UpdatedByUserAlias = 'setup'
SET @UpdatedDateTime = GETDATE()
SET @AuthoringToolLevel = 0
SET @Name = 'eco_cwf_viewer1'
SET @Guid = '78E48566-65F1-4377-8BF1-A2C5A80FBA35'
SET @Role = 1
SET @Enabled = 1

IF NOT EXISTS (SELECT [Id] FROM [dbo].[AuthorizationGroup] WHERE [Name] = @Name) 
BEGIN
INSERT INTO [dbo].[AuthorizationGroup]
(Guid, Name, AuthoringToolLevel, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime, RoleId, Enabled)
VALUES (@Guid, @Name, @AuthoringToolLevel, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime, @Role, @Enabled)
END

SET @InsertedByUserAlias = 'setup'
SET @InsertedDateTime = GETDATE()
SET @UpdatedByUserAlias = 'setup'
SET @UpdatedDateTime = GETDATE()
SET @AuthoringToolLevel = 0
SET @Name = 'eco_cwftenantadmin1'
SET @Guid = '71D58E44-C468-4B2E-8A8A-DDD2B628EB61'
SET @Role = 3
SET @Enabled = 1

IF NOT EXISTS (SELECT [Id] FROM [dbo].[AuthorizationGroup] WHERE [Name] = @Name) 
BEGIN
INSERT INTO [dbo].[AuthorizationGroup]
(Guid, Name, AuthoringToolLevel, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime, RoleId, Enabled)
VALUES (@Guid, @Name, @AuthoringToolLevel, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime, @Role, @Enabled)
END

SET @InsertedByUserAlias = 'setup'
SET @InsertedDateTime = GETDATE()
SET @UpdatedByUserAlias = 'setup'
SET @UpdatedDateTime = GETDATE()
SET @AuthoringToolLevel = 0
SET @Name = 'eco_cwf_tenant1Auth'
SET @Guid = 'E165E3FA-C003-442E-9C2B-7BDDB62E1EA7'
SET @Role = 2
SET @Enabled = 1

IF NOT EXISTS (SELECT [Id] FROM [dbo].[AuthorizationGroup] WHERE [Name] = @Name) 
BEGIN
INSERT INTO [dbo].[AuthorizationGroup]
(Guid, Name, AuthoringToolLevel, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime, RoleId, Enabled)
VALUES (@Guid, @Name, @AuthoringToolLevel, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime, @Role, @Enabled)
END

SET @InsertedByUserAlias = 'setup'
SET @InsertedDateTime = GETDATE()
SET @UpdatedByUserAlias = 'setup'
SET @UpdatedDateTime = GETDATE()
SET @AuthoringToolLevel = 0
SET @Name = 'cwf_tenant_stg_auth'
SET @Guid = '7B29D27F-F654-437A-B646-1F77930B753A'
SET @Role = 6
SET @Enabled = 1

IF NOT EXISTS (SELECT [Id] FROM [dbo].[AuthorizationGroup] WHERE [Name] = @Name) 
BEGIN
INSERT INTO [dbo].[AuthorizationGroup]
(Guid, Name, AuthoringToolLevel, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime, RoleId, Enabled)
VALUES (@Guid, @Name, @AuthoringToolLevel, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime, @Role, @Enabled)
END
