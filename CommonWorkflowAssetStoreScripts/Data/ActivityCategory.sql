DECLARE @Guid [uniqueidentifier] 
DECLARE @Name [nvarchar](50)
DECLARE @Description [nvarchar](250)
DECLARE @Metatags [nvarchar] (max)
DECLARE @AuthgroupId bigint

DECLARE @InsertedByUserAlias  [nvarchar](50)
DECLARE @InsertedDateTime [datetime] 
DECLARE @UpdatedByUserAlias [nvarchar](50) 
DECLARE @UpdatedDateTime [datetime] 

SET @InsertedByUserAlias = 'setup'
SET @InsertedDateTime = GETDATE()
SET @UpdatedByUserAlias = 'setup'
SET @UpdatedDateTime = GETDATE()

SET @Guid = '00000000-0000-0000-0000-000000000000'
SET @Name = 'Unassigned'
SET @Description = 'Unassigned'
SET @Metatags = 'unassigned'
SET @AuthgroupId = 2
INSERT INTO [dbo].[ActivityCategory]
([GUID], Name, [Description], MetaTags, AuthGroupID, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES (@Guid, @Name, @Description, @Metatags, @AuthgroupId, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)

SET @Guid = '424106EC-1A17-4AAB-B8B2-3C7D291851DC'
SET @Name = 'Administration'
SET @Description = 'Items belonging to administrative group'
SET @Metatags = 'Admin;administration'
SET @AuthgroupId = 3
INSERT INTO [dbo].[ActivityCategory]
([GUID], Name, [Description], MetaTags, AuthGroupID, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES (@Guid, @Name, @Description, @Metatags, @AuthgroupId, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)

SET @Guid = 'E0EDF9D0-529E-4C7D-9A86-45D4F233FFD4'
SET @Name = 'OAS Basic Controls'
SET @Description = 'Basic Controls'
SET @Metatags = 'Basic;Controls'
SET @AuthgroupId = 2
INSERT INTO [dbo].[ActivityCategory]
([GUID], Name, [Description], MetaTags, AuthGroupID, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES (@Guid, @Name, @Description, @Metatags, @AuthgroupId, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)

SET @Guid = 'D69AB639-0690-43B9-8530-5651A92503CA'
SET @Name = 'Developer Toolbox'
SET @Description = 'Standard WF4 Activities restricted to developers'
SET @Metatags = 'WF4;Developers'
SET @AuthgroupId = 1
INSERT INTO [dbo].[ActivityCategory]
([GUID], Name, [Description], MetaTags, AuthGroupID, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES (@Guid, @Name, @Description, @Metatags, @AuthgroupId, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)

SET @Guid = 'C6AE5EC7-11D6-4C5D-A79A-C249BEBCD2EB'
SET @Name = 'Pages'
SET @Description = 'Page Activities'
SET @Metatags = 'Pages'
SET @AuthgroupId = 2
INSERT INTO [dbo].[ActivityCategory]
([GUID], Name, [Description], MetaTags, AuthGroupID, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES (@Guid, @Name, @Description, @Metatags, @AuthgroupId, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)

SET @Guid = 'A94878AD-77C9-4B46-B785-C93DEB342E4C'
SET @Name = 'Specific'
SET @Description = 'Specific'
SET @Metatags = 'Specific'
SET @AuthgroupId = 2
INSERT INTO [dbo].[ActivityCategory]
([GUID], Name, [Description], MetaTags, AuthGroupID, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES (@Guid, @Name, @Description, @Metatags, @AuthgroupId, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)

SET @Guid = '919E4F46-10C9-4BBC-B4AE-EB78D098A907'
SET @Name = 'Generic Activities '
SET @Description = 'Generic Common Workflow Framework Activities'
SET @Metatags = 'Generic'
SET @AuthgroupId = 2
INSERT INTO [dbo].[ActivityCategory]
([GUID], Name, [Description], MetaTags, AuthGroupID, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES (@Guid, @Name, @Description, @Metatags, @AuthgroupId, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)

SET @Guid = '440BC0F2-E8F8-4E77-9C71-F2822112D752'
SET @Name = 'Publishing Workflow'
SET @Description = 'Publishing workflows'
SET @Metatags = 'Publishers'
SET @AuthgroupId = 2
INSERT INTO [dbo].[ActivityCategory]
([GUID], Name, [Description], MetaTags, AuthGroupID, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES (@Guid, @Name, @Description, @Metatags, @AuthgroupId, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)

SET @Guid = '42B8808E-9832-46AA-9C40-78B015325CE6'
SET @Name = 'Business'
SET @Description = 'Business Users'
SET @Metatags = 'Business'
SET @AuthgroupId = 2
INSERT INTO [dbo].[ActivityCategory]
([GUID], Name, [Description], MetaTags, AuthGroupID, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES (@Guid, @Name, @Description, @Metatags, @AuthgroupId, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)
