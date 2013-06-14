DECLARE @Code bigint
DECLARE @Name nvarchar (50)
DECLARE @Description nvarchar (250)
DECLARE @ShowInProduction bit
DECLARE @LockForChanges bit
DECLARE @IsDeleted bit
DECLARE @IsEligibleForCleanUp bit

DECLARE @InsertedByUserAlias  [nvarchar](50)
DECLARE @InsertedDateTime [datetime] 
DECLARE @UpdatedByUserAlias [nvarchar](50) 
DECLARE @UpdatedDateTime [datetime] 

SET @ShowInProduction = 0
SET @LockForChanges = 0
SET @IsDeleted = 0
SET @IsEligibleForCleanUp = 0
SET @InsertedByUserAlias = 'setup'
SET @InsertedDateTime = GETDATE()
SET @UpdatedByUserAlias = 'setup'
SET @UpdatedDateTime = GETDATE()


SET @Code = 1000
SET @Name = 'Private'
SET @Description = 'Private/not visible in the marketplace activity/workflow'

INSERT INTO [dbo].[StatusCode]
(Code, Name, Description, ShowInProduction, LockForChanges, IsDeleted, IsEligibleForCleanUp, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES (@Code, @Name, @Description, @ShowInProduction, @LockForChanges, @IsDeleted, @IsEligibleForCleanUp, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)

SET @Code = 1010
SET @Name = 'Public'
SET @Description = 'Public/marketplace visible activity/workflow'

INSERT INTO [dbo].[StatusCode]
(Code, Name, Description, ShowInProduction, LockForChanges, IsDeleted, IsEligibleForCleanUp, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES (@Code, @Name, @Description, @ShowInProduction, @LockForChanges, @IsDeleted, @IsEligibleForCleanUp, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)

SET @Code = 1020
SET @Name = 'Retired'
SET @Description = 'activity/workflow that is no longer accessible in the marketplace'

INSERT INTO [dbo].[StatusCode]
(Code, Name, Description, ShowInProduction, LockForChanges, IsDeleted, IsEligibleForCleanUp, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES (@Code, @Name, @Description, @ShowInProduction, @LockForChanges, @IsDeleted, @IsEligibleForCleanUp, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)


