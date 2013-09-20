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
