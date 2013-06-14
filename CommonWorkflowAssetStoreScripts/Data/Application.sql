DECLARE @GUID [uniqueidentifier]
DECLARE @Name [nvarchar](50)
DECLARE @Description [nvarchar](250)
DECLARE @SoftDelete bit 
DECLARE @InsertedByUserAlias  [nvarchar](50)
DECLARE @InsertedDateTime [datetime] 
DECLARE @UpdatedByUserAlias [nvarchar](50) 
DECLARE @UpdatedDateTime [datetime] 

SET @InsertedByUserAlias = 'setup'
SET @InsertedDateTime = GETDATE()
SET @UpdatedByUserAlias = 'setup'
SET @UpdatedDateTime = GETDATE()
SET @SoftDelete = 0

SET @GUID = '78AE4A0F-B05C-42C7-A545-0C06BD7A9FD4'
SET @Name = 'BPOSD'
SET @Description = 'Bpos-D'
INSERT INTO [dbo].[Application]
(GUID, Name, Description, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES(@GUID, @Name, @Description, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)

SET @GUID = 'CC4EC9B2-0975-434B-A719-179BD85BD6F5'
SET @Name = 'ALL'
SET @Description = 'Workflows and Activities which will work with all applications (e.g. a generic if statement)'
INSERT INTO [dbo].[Application]
(GUID, Name, Description, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES(@GUID, @Name, @Description, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)

SET @GUID = '24F082B9-407E-4207-BE30-44836FF129DE'
SET @Name = 'MPO'
SET @Description = 'MPO'
INSERT INTO [dbo].[Application]
(GUID, Name, Description, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES(@GUID, @Name, @Description, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)

SET @GUID = '87107E43-5DBC-481D-8C32-4D512544A1E2'
SET @Name = 'Framework'
SET @Description = 'Common Workflow Framework'
INSERT INTO [dbo].[Application]
(GUID, Name, Description, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES(@GUID, @Name, @Description, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)

SET @GUID = '72ED0A02-A262-4A8B-8779-75732019CDA2'
SET @Name = 'OAS'
SET @Description = 'Dummy entry for the OAS system'
INSERT INTO [dbo].[Application]
(GUID, Name, Description, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES(@GUID, @Name, @Description, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)

SET @GUID = 'D2EFB17B-4470-49C5-9B91-8E13D2EBB371'
SET @Name = 'EHS'
SET @Description = 'EHS'
INSERT INTO [dbo].[Application]
(GUID, Name, Description, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES(@GUID, @Name, @Description, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)

SET @GUID = '0A1962BA-B6AB-4726-AAFB-ACF599ED42EB'
SET @Name = 'CCF'
SET @Description = 'Test CCF Application'
INSERT INTO [dbo].[Application]
(GUID, Name, Description, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES(@GUID, @Name, @Description, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)

SET @GUID = '1ECB6F8F-04D2-4F33-8127-B895FC923D7F'
SET @Name = 'OSS'
SET @Description = 'OSS'
INSERT INTO [dbo].[Application]
(GUID, Name, Description, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES(@GUID, @Name, @Description, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)

SET @GUID = '8D38C13C-BE72-4AE7-9030-BBCDE3872554'
SET @Name = 'OASP'
SET @Description = 'Another Dummy Entry'
INSERT INTO [dbo].[Application]
(GUID, Name, Description, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES(@GUID, @Name, @Description, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)

SET @GUID = 'AB2663A6-8F87-4A28-9219-D28208C1E066'
SET @Name = 'TestApp'
SET @Description = 'Test Workflow System Application'
INSERT INTO [dbo].[Application]
(GUID, Name, Description, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES(@GUID, @Name, @Description, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)

SET @GUID = '61F4B8E8-7459-45AC-AFDF-D6D7C5AE54B4'
SET @Name = 'BPOSS'
SET @Description = 'Bpos-S'
INSERT INTO [dbo].[Application]
(GUID, Name, Description, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES(@GUID, @Name, @Description, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)


