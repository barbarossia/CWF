/****** Object:  StoredProcedure [dbo].[ActivityLibrary_CreateOrUpdate]    Script Date: 07/31/2012 13:29:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

/**************************************************************************
// Product:  CommonWF
// FileName: ActivityLibrary_CreateOrUpdate.sql
// File description: Save a new Activitylibrary.
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   ActivityLibrary_CreateOrUpdate                        *
**    Desc:   Save or Update a new activitylibrary.                        *
**    Auth:   v-stska                                                      *
**    Date:   10/27/2010                                                   *
**                                                                         *
****************************************************************************
**   sproc logic flow: <Optional if complex> 
**   Parameter definition if complex
****************************************************************************
**                  CHANGE HISTORY
****************************************************************************
**	Date:        Author:             Description:
** ____________    ________________    ____________________________________
**  11/8/2010      v-stska             Original implementation
**  11/21/2010     v-stska             integrate Create & Update
**  1/5/2010       v-stska             Change Insert to Update if soft deleted
**  11-Nov-2011	   v-richt             Bug#86713 - Change error codes to positive numbers
**  03/08/2012     v-sanja             Provide default activity category name when null.  
**  27-Mar-2012	   v-richt			   Change the way Status Code is passed in/determined.                                     
** *************************************************************************/
create PROCEDURE [dbo].[ActivityLibrary_CreateOrUpdate]
        @inCaller nvarchar(50),
        @inCallerversion nvarchar (50),
        @InId bigint,
        @InGUID varchar (50),
        @inName nvarchar (255),
        @inAuthGroup nvarchar(50),
        @inCategoryName varchar(50),
        @InCategoryGUID varchar(50),
        @inExecutable varbinary(max),
        @inHasActivities bit,
        @inDescription nvarchar(250),
        @inImportedBy nvarchar (50),
        @inVersionNumber nvarchar (50),
        @inMetaTags nvarchar (250),
        @inInsertedByUserAlias nvarchar(50),
        @inUpdatedByUserAlias nvarchar(50),
        @inFriendlyName nvarchar(50),
        @inReleaseNotes nvarchar(250),
        @inStatusName varchar(50),
        @outErrorString nvarchar (300)OUTPUT
       
        
--WITH ENCRYPTION
AS
BEGIN
    SET NOCOUNT ON

    DECLARE 
           @rc                [int]
          ,@rc2               [int]
          ,@error             [int]
          ,@rowcount          [int]
          ,@step              [int]
          ,@cObjectName       [sysname]
          ,@ErrorMessage      [nvarchar](2048)
          ,@SeverityCode      [nvarchar] (50)
          ,@Guid              [nvarchar] (36)
          ,@StatusID		  int
 
    SELECT   @rc                = 0
            ,@error             = 0
            ,@rowcount          = 0
            ,@step              = 0
            ,@cObjectName       = OBJECT_NAME(@@PROCID)
            
            
    --BEGIN TRANSACTION
    SET @outErrorString = ''
    -- Check the input variables
    IF (@inCaller IS NULL OR @inCaller = '')
    BEGIN
        SET @outErrorString = 'Invalid Parameter Value (@inCaller)'
        RETURN 55100
    END
    IF (@inCallerversion IS NULL OR @inCallerversion = '')
    BEGIN
        SET @outErrorString = 'Invalid Parameter Value (@inCallerversion)'
        RETURN 55101
    END
    IF (@InInsertedByUserAlias IS NULL OR @InInsertedByUserAlias = '')
    BEGIN
        SET @outErrorString = 'Invalid Parameter Value (@InInsertedByUserAlias)'
        RETURN 55102
    END

    IF (@InUpdatedByUserAlias IS NULL OR @InUpdatedByUserAlias = '')
    BEGIN
        SET @outErrorString = 'Invalid Parameter Value (@InUpdatedByUserAlias)'
        RETURN 55103
    END

    -- Sanjeewa: 03/08/2012 - Set default value for category name.
    IF (@inCategoryName IS NULL OR @inCategoryName = '')
    BEGIN        
        SET @inCategoryName = 'OAS Basic Controls';
    END   
    
    DECLARE @InInsertedDateTime datetime
    SET @InInsertedDateTime = GETDATE()
    DECLARE @inUpdatedDateTime datetime
    SET @inUpdatedDateTime = GETDATE()
    
    BEGIN TRY
        /* check to see if this needs to be a real insert or an update
           It could have been soft deleted and an insert will fail on
           a unique Name/versionNumber constraint */
        DECLARE @CHECKId bigint
        SELECT @CHECKId = ID
        FROM [dbo].[ActivityLibrary]
        WHERE @inName = name AND @inVersionNumber = VersionNumber
        -- If found, change this from an insert to an update
        If (@CHECKId > 0)
            SET @InId = @CHECKId

        SELECT @StatusID = Code
        FROM [dbo].[StatusCode] sc
        WHERE sc.Name = @inStatusName

        -- Insert
        IF (@InId IS Null OR @InId = 0)
        BEGIN
            IF (@InGUID IS NULL)
            BEGIN
                SET @outErrorString = 'Invalid Parameter Value (@InGUID)'
                RETURN 55105
            END
            IF (@InName IS NULL OR @InName = '')
            BEGIN
                SET @outErrorString = 'Invalid Parameter Value (@InName)'
                RETURN 55106
            END
            IF (@inVersionNumber IS NULL OR @inVersionNumber = '')
            BEGIN
                SET @outErrorString = 'Invalid Parameter Value (@inVersionNumber)'
                RETURN 55104
            END
            IF (@InAuthGroup IS NULL OR @InAuthGroup = '')
            BEGIN
                SET @outErrorString = 'Invalid Parameter Value (@InAuthGroup)'
                RETURN 55070
            END
            IF (@InUpdatedByUserAlias IS NULL OR @InUpdatedByUserAlias = '')
                SET @InUpdatedByUserAlias = @InInsertedByUserAlias
                
            -- Find AuthGroupID
            DECLARE @AuthGroupID bigint
            SELECT @AuthGroupID = ID
            FROM [dbo].[AuthorizationGroup] ag
            WHERE ag.Name = @inAuthGroup
            
            IF (@AuthGroupID IS NULL)
            BEGIN
                SET @outErrorString = '@InAuthGroup - Not in [AuthorizationGroup]'
                RETURN 55118
            END
                
            -- Find ActivityCategoryID
            DECLARE @ActivityCategoryID bigint
            SELECT @ActivityCategoryID = ID
            FROM [dbo].[ActivityCategory] ag
            WHERE ag.Name = @inCategoryName
            
            IF (@ActivityCategoryID IS NULL)
            BEGIN
                SET @outErrorString = 'Invalid Parameter Value (@inCategory - Not in [ActivityCategory])'
                RETURN 55115
            END

            SET @ActivityCategoryID = NULL

            INSERT INTO ActivityLibrary
                ([GUID], Name, AuthGroupId, Category, CategoryId, [Executable], HasActivities, [Description], ImportedBy, VersionNumber, [Status], MetaTags, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime, FriendlyName,ReleaseNotes)
                VALUES(@InGUID, @InName, @AuthGroupID, @InCategoryGUID, @ActivityCategoryID, @InExecutable, @InHasActivities, @InDescription, @InImportedBy, @InVersionNumber, @StatusID, @InMetaTags, 0, @InInsertedByUserAlias, @InInsertedDateTime, @InUpdatedByUserAlias, @InUpdatedDateTime,@inFriendlyName,@inReleaseNotes)
             --COMMIT TRANSACTION
         END
         ELSE
         BEGIN
            IF (@InUpdatedByUserAlias IS NULL OR @InUpdatedByUserAlias = '')
            BEGIN
                SET @outErrorString = 'Invalid Parameter Value (@InUpdatedByUserAlias))'
                RETURN 55103
            END
            -- Test for valid @InId
            DECLARE @TEMPID bigint
            SELECT @TEMPID = [Id]
            FROM [dbo].[ActivityLibrary]
            WHERE Id = @InId
            IF (@TEMPID IS NULL)
            BEGIN
                SET @outErrorString = 'Invalid @InId attempting to perform an UPDATE on table'
                RETURN 55060
            END
            
            IF (@inAuthGroup IS NOT NULL OR @inAuthGroup != '')
            BEGIN
            -- Find AuthGroupID
                SELECT @AuthGroupID = ID
                FROM [dbo].[AuthorizationGroup] ag
                WHERE ag.Name = @inAuthGroup
                
                IF (@AuthGroupID IS NULL)
                BEGIN
                    SET @outErrorString = '@InAuthGroup - Not in [AuthorizationGroup]'
                    RETURN 55070
                END
            END
            UPDATE ActivityLibrary
            SET 
                Name = Coalesce(@InName, Name),
                [GUID] = Coalesce(@InGUID, [GUID]),
                AuthGroupId = Coalesce(@AuthGroupID, AuthGroupId),
                Category = Coalesce(@InCategoryGUID, Category),
                [Executable] = Coalesce(@InExecutable, [Executable]),
                HasActivities = Coalesce(@InHasActivities, HasActivities),
                [Description] = Coalesce(@InDescription, [Description]),
                ImportedBy = Coalesce(@InImportedBy, ImportedBy),
                VersionNumber = Coalesce(@InVersionNumber, VersionNumber),
                [Status] = Coalesce(@StatusID, [Status]),
                SoftDelete = 0,
                MetaTags = Coalesce(@InMetaTags, MetaTags),
                UpdatedByUserAlias = Coalesce(@InUpdatedByUserAlias, @InInsertedByUserAlias),
                UpdatedDateTime = Coalesce(@InUpdatedDateTime, GETDATE()),
                FriendlyName = Coalesce(@InFriendlyName, FriendlyName),
                ReleaseNotes = Coalesce(@InReleaseNotes, ReleaseNotes)
            WHERE Id = @InId
         END
    END TRY
    BEGIN CATCH
       EXECUTE [dbo].Error_Handle 
    END CATCH
   RETURN @rc
END
