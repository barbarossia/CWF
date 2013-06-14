DECLARE @Guid [uniqueidentifier] 
DECLARE @Name [nvarchar](50)

DECLARE @AuthoringToolLevel [int]
DECLARE @InsertedByUserAlias  [nvarchar](50)
DECLARE @InsertedDateTime [datetime] 
DECLARE @UpdatedByUserAlias [nvarchar](50) 
DECLARE @UpdatedDateTime [datetime] 

SET @InsertedByUserAlias = 'setup'
SET @InsertedDateTime = GETDATE()
SET @UpdatedByUserAlias = 'setup'
SET @UpdatedDateTime = GETDATE()
SET @AuthoringToolLevel = 0

SET @Name = 'pqocwfdevuser'
SET @Guid = '5089D7CB-C404-419D-B153-0C3CD989FE01'
INSERT INTO [dbo].[AuthorizationGroup]
(Guid, Name, AuthoringToolLevel, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES (@Guid, @Name, @AuthoringToolLevel, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)

SET @Name = 'pqocwfauthors'
SET @Guid = 'AF0DDD15-255C-4A55-997D-45FF1AAD9B95'
INSERT INTO [dbo].[AuthorizationGroup]
(Guid, Name, AuthoringToolLevel, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES (@Guid, @Name, @AuthoringToolLevel, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)

SET @Name = 'pqocwfadmin'
SET @Guid = '6795B3DE-9E6D-4194-A5FD-63A9A17A3FCD'
INSERT INTO [dbo].[AuthorizationGroup]
(Guid, Name, AuthoringToolLevel, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES (@Guid, @Name, @AuthoringToolLevel, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)

SET @Name = 'Unassigned'
SET @Guid = 'EB09CE89-E7C8-4B62-9D1E-D5460444B276'
INSERT INTO [dbo].[AuthorizationGroup]
(Guid, Name, AuthoringToolLevel, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES (@Guid, @Name, @AuthoringToolLevel, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)
