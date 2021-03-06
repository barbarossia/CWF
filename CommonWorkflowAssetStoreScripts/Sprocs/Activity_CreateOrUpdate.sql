/****** Object:  StoredProcedure [dbo].[Activity_CreateOrUpdate]    Script Date: 07/31/2012 13:28:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/**************************************************************************
// Product:  CommonWF
// FileName: Activity_CreateOrUpdate.sql
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   Activity_CreateOrUpdate                         *
**    Desc:   Create/update an entry in storeActivities.                   *
**    Auth:   v-stska                                                      *
**    Date:   11/8/2010                                                    *
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
**  11/15/2010     v-stska             changes for NEW2 identity model
**  11/21/2010     v-stska             Add inserted, updated, alias & dt	
**  11-Nov-2011	   v-richt             Bug#86713 - Change error codes to positive numbers
**  9 Nov 2011     v-richt             Bug#76211, #27574 -  [Authoring Tool] Creating a 
                                       wkflw with name too long and saving to server,
                                       open from the AT UI and open workflow throws exception
**  03/08/2012     v-sanja             Eliminated the paramters BaseType, DefaultRender, DeveloperNotes, IconsName since they 
                                       are no longer being used.  Provided default values instead.
                                       Provide default activity category name when null.
                                       Also eliminated IsSwitch, IsUxActivity, IsToolboxActivity.
** 03/20/2012		v-richt			   Fix for bug #154910, ' WorkFlow with Empty Or WhiteSpace Status should not be saved. '
** 03/21/2012		v-luval			   Make the workflowType nullable, as people can create workflows from scratch and not from a template.
** 03/21/2012		v-sanja			   Got rid of DefaultRender and ToolboxName columns.
** 03/23/2012		v-sanja			   Added back developernotes parameter.
**  27-Mar-2012	   v-richt			   Change the way Status Code is passed in/determined.   
**  17/May/2012    v-bobzh             Return the insert or update record      
** *************************************************************************/
create PROCEDURE [dbo].[Activity_CreateOrUpdate]
        @inCaller			nvarchar(50),
        @inCallerversion	nvarchar (50),
        @InId				bigint,
        @InGUID				varchar(50)   ,
        @InName				nvarchar(255) ,
        @inShortName		nvarchar (50) ,
        @InDescription		nvarchar(250) ,
        @InAuthGroupName	nvarchar(50) ,
        @InMetaTags			nvarchar(max) ,
        @InIsService		bit ,
        @InActivityLibraryName nvarchar(255) ,
        @InActivityLibraryVersion nvarchar (50),
        @InCategoryName		nvarchar(50) ,
        @InVersion			nvarchar(25) ,
        @InStatusName		nvarchar(50),
        @InWorkflowTypeName	nvarchar(50) ,
        @InLocked			bit ,
        @InLockedBy			nvarchar(50) ,
        @InIsCodeBeside		bit ,
        @InXAML				nvarchar(max) ,
        @InDeveloperNotes	nvarchar(250) ,
        @InNamespace		nvarchar(250) ,
        @InInsertedByUserAlias nvarchar(50),
        @InUpdatedByUserAlias nvarchar(50),
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
          
    DECLARE @StatusID bigint
    DECLARE @InInsertedDateTime datetime
    DECLARE @inUpdatedDateTime datetime
    DECLARE @CHECKId bigint
    DECLARE @IconId bigint
    DECLARE @ToolBoxTabId bigint
    DECLARE @ActivityLibraryID bigint
    DECLARE @CategoryID bigint
    DECLARE @WorkflowID bigint
    DECLARE @AuthGroupID bigint
    DECLARE @Id bigint
 
    SELECT   @rc                = 0
            ,@error             = 0
            ,@rowcount          = 0
            ,@step              = 0
            ,@cObjectName       = OBJECT_NAME(@@PROCID)
            
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

    SET @InInsertedDateTime = GETutcDATE()
    SET @inUpdatedDateTime = GETutcDATE()

        /* check to see if this needs to be a real insert or an update
           It could have been soft deleted and an insert will fail on
           a unique Name/versionNumber constraint */

    SELECT @CHECKId = ID
    FROM [dbo].[Activity]
    WHERE @inName = name AND @inVersion = [Version]
        -- If found, change this from an insert to an update
    If (@CHECKId > 0)
        SET @InId = @CHECKId
            
    -- Sanjeewa: 03/08/2012 - Set default value for category name.
    IF (@InCategoryName IS NULL OR @InCategoryName = '')
    BEGIN        
        SET @InCategoryName = 'OAS Basic Controls';
    END        
        
    IF (@InId IS NULL OR @InId = 0)
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
        IF (@InShortName IS NULL OR @InShortName = '')
        BEGIN
            SET @outErrorString = 'Invalid Parameter Value (@InShortName)'
            RETURN 55134
        END       
        IF (@InIsService IS NULL)
        BEGIN
            SET @outErrorString = 'Invalid Parameter Value (@InIsService)'
            RETURN 55110
        END
        IF ((@InActivityLibraryVersion IS NULL OR @InActivityLibraryVersion = '') AND NOT (@InActivityLibraryName IS NULL OR @InActivityLibraryName = ''))
        BEGIN
            SET @outErrorString = 'Invalid Parameter Value (@InActivityLibraryVersion)'
            RETURN 55112
        END
        IF (@InVersion IS NULL OR @InVersion = '')
        BEGIN
            SET @outErrorString = 'Invalid Parameter Value (@InVersion)'
            RETURN 55104
        END

        IF (@InInsertedByUserAlias IS NULL OR @InInsertedByUserAlias = '')
        BEGIN
            SET @outErrorString = 'Invalid Parameter Value (@InInsertedByUserAlias)'
            RETURN 55102
        END
        IF (@InUpdatedByUserAlias IS NULL OR @InUpdatedByUserAlias = '')
            SET @InUpdatedByUserAlias = @InInsertedByUserAlias
        IF (@InAuthGroupName IS NULL OR @InAuthGroupName = '')
        BEGIN
            SET @outErrorString = 'Invalid Parameter Value (@InAuthGroupName)'
            RETURN 55118
        END
        -- get the identities

        SET @IconId = 1; -- Sanjeewa 03/08/2012 - Assigned the default value, this field is not used by query service anymore
        SET @ToolBoxTabId = 1; -- Sanjeewa 03/08/2012 - Assigned the default value, this field is not used by query service anymore
          
        SELECT @ActivityLibraryID = ID
        FROM [dbo].[ActivityLibrary] al
        WHERE al.Name = @InActivityLibraryName AND
              al.VersionNumber = @InActivityLibraryVersion
        
      
        SELECT @CategoryID = ID
        FROM [dbo].[ActivityCategory] al
        WHERE al.Name = @InCategoryName
        IF (@CategoryID IS NULL)
        BEGIN
            SET @outErrorString = 'Invalid Parameter Value (@InCategoryName) - not in [dbo].[ActivityCategory]'
            RETURN 55115
        END

        SELECT @WorkflowID = ID
        FROM [dbo].[WorkflowType] wt
        WHERE wt.Name = @inWorkflowTypeName        
     
        SELECT @AuthGroupID = Id
        FROM [dbo].[AuthorizationGroup] ag
        WHERE ag.Name= @InAuthGroupName
        IF (@AuthGroupID IS NULL)
        BEGIN
            SET @outErrorString = 'Invalid Parameter Value (@InAuthGroup) - not in [dbo].[AuthorizationGroup]'
            RETURN 55118
        END


        SELECT @StatusID = Code
        FROM [dbo].[StatusCode] sc
        WHERE sc.Name = @inStatusName


        -- Sanjeewa 03/08/2012 - Assigned the default value for DeveloperNotes, BaseType.These fields are not used by query service anymore    
        -- Also set default values for IsSwitch, IsToolboxActivity, IsUxActivity as 1.
        -- Set Locked = 0, LockedBy = ''.  Not used.  
        INSERT INTO Activity
            ([GUID], Name, ShortName, [Description], MetaTags, IconsId, IsSwitch, IsService, ActivityLibraryId, 
            IsUxActivity, CategoryId, ToolboxTab, IsToolBoxActivity, [Version], StatusId, 
            WorkflowTypeId, Locked, LockedBy, IsCodeBeside, XAML, DeveloperNotes, BaseType, [Namespace], SoftDelete,
                InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
            VALUES(@InGUID, @InName, @inShortName,  @InDescription, @InMetaTags, @IconId, 1, @InIsService, @ActivityLibraryID, 
                    1, @CategoryID, @ToolboxTabId, 1, @InVersion, @StatusId, 
                    @WorkflowID, @InLocked, @InLockedBy, @InIsCodeBeside, @InXAML, @InDeveloperNotes, '', @InNamespace, 0, 
                    @InInsertedByUserAlias, @InInsertedDateTime, @InUpdatedByUserAlias, @InUpdatedDateTime)
        SELECT @Id = SCOPE_IDENTITY(); 
		SELECT sa.Id, 
			sa.[GUID], 
			sa.Name,
			sa.ShortName, 
			sa.[Description], 
			sa.MetaTags, 
			sa.IconsId, 
			ic.[Name] AS iconsName,
			sa.IsSwitch, 
			sa.IsService, 
			sa.ActivityLibraryId, 
			al.Name AS ActivityLibraryName,
			al.VersionNumber AS ActivityLibraryVersion,
			al.AuthGroupID,
			ag.Name AS AuthGroupName, 
			sa.IsUxActivity, 
			sa.CategoryId, 
			ac.Name as ActivityCategoryName, 
			tbtn.Name as ToolBoxtabName, 
			sa.ToolBoxtab, 
			sa.IsToolBoxActivity, 
			sa.[Version], 
			sa.StatusId, 
			sc.Name AS StatusCodeName, 
			sa.WorkflowTypeId,
			wft.Name as WorkFlowTypeName, 
			sa.Locked, 
			sa.LockedBy, 
			sa.IsCodeBeside, 
			sa.XAML, 
			sa.DeveloperNotes, 
			sa.BaseType, 
			sa.[Namespace],
			sa.InsertedByUserAlias,
			sa.InsertedDateTime,
			sa.UpdatedByUserAlias,
			sa.UpdatedDateTime
			FROM [dbo].[Activity] sa
			LEFT JOIN ActivityLibrary al ON sa.ActivityLibraryId = al.Id
			JOIN ActivityCategory ac ON sa.CategoryId = ac.Id
			LEFT JOIN ToolBoxTabName tbtn ON sa.ToolBoxTab = tbtn.Id
			JOIN StatusCode sc ON sa.StatusId = sc.Code
			JOIN WorkflowType wft ON sa.WorkflowTypeId = wft.Id
			LEFT JOIN Icon ic ON sa.IconsId = ic.Id 
			LEFT JOIN AuthorizationGroup ag ON ag.Id = al.AuthGroupId
			WHERE sa.Id = @Id
			AND sa.SoftDelete = 0

     END
     ELSE
     BEGIN
        -- This is an UPDATE

        DECLARE @TEMPID bigint      
        DECLARE @ActivityLibraryID1 bigint
        DECLARE @CategoryID1 bigint 
        DECLARE @WorkflowID1 bigint
        DECLARE @StatusID1 bigint
        DECLARE @AuthgroupID1 bigint
        
        -- Test for valid @InId
        SELECT @TEMPID = [Id]
        FROM [dbo].[Activity]
        WHERE Id = @InId
        IF (@TEMPID IS NULL)
        BEGIN
            SET @outErrorString = 'Invalid @InId attempting to perform an UPDATE on table'
            RETURN 55060
        END
              
        
        --FK to ActivityLibrary
        IF (@InActivityLibraryName IS  NOT NULL AND @InActivityLibraryVersion IS NOT NULL)
        BEGIN
            SELECT @ActivityLibraryID1 = ID
            FROM [dbo].[ActivityLibrary] sa
            WHERE sa.Name = @InActivityLibraryName AND
                  sa.VersionNumber = @InActivityLibraryVersion
        END
        
        --FK to CategoryName       
        IF (@InCategoryName IS NOT NULL)
        BEGIN
            SELECT @CategoryID1 = ID
            FROM [dbo].[ActivityCategory] ac
            WHERE ac.Name = @InCategoryName
        END 
        
        --FK to Workflow
        IF (@InWorkflowTypeName IS NOT NULL)
        BEGIN
            SELECT @WorkflowId1 = ID
            FROM [dbo].[ActivityCategory] ac
            WHERE ac.Name = @InWorkflowTypeName
        END
       
        --FK to Status
        IF (@InStatusName IS NOT NULL)
        BEGIN
            SELECT @StatusID1 = Code
			FROM [dbo].[StatusCode] sc
			WHERE sc.Name = @inStatusName
        END
        
        --FK to AuthGroup
        IF (@InAuthGroupName IS NOT NULL)
        BEGIN
            SELECT @AuthGroupId = Id
            FROM [dbo].[Activity] sa
            WHERE sa.Name = @InAuthGroupName
        END
        
        -- if guid is empty guid, make it null on an update
        IF (@InGUID = '00000000-0000-0000-0000-000000000000')
        BEGIN
            SET @InGUID = NULL
        END
        
        UPDATE [Dbo].[Activity]
        SET 
            [GUID] = Coalesce(@InGUID, [GUID]),
            Name = Coalesce(@InName, name),
            ShortName = Coalesce(@inShortName, ShortName),
            [Description] = Coalesce(@InDescription, [Description]),
            MetaTags = Coalesce(@InMetaTags, MetaTags),
            IconsId = 1, -- Sanjeewa 03/08/2012 - Assigned the default value, this field is not used by query service anymore
            IsSwitch = 1, -- Sanjeewa 03/08/2012 - Assigned the default value, this field is not used by query service anymore
            IsService = Coalesce(@InIsService, IsService),
            ActivityLibraryId = Coalesce(@ActivityLibraryID1, ActivityLibraryId),
            IsUxActivity = 1, -- Sanjeewa 03/08/2012 - Assigned the default value, this field is not used by query service anymore
            CategoryId = Coalesce(@CategoryID1, CategoryId),
            ToolboxTab = 1,-- Sanjeewa 03/08/2012 - Assigned the default value, this field is not used by query service anymore
            IsToolBoxActivity = 1, -- Sanjeewa 03/08/2012 - Assigned the default value, this field is not used by query service anymore
            [Version] = Coalesce(@InVersion, [Version]),
            StatusId = Coalesce(@StatusId1, StatusId),
            WorkflowTypeId = Coalesce(@WorkflowID1, WorkflowTypeId),
            Locked = Coalesce(@InLocked, Locked),
            LockedBy = Coalesce(@InLockedBy, LockedBy),
            IsCodeBeside = Coalesce(@InIsCodeBeside, IsCodeBeside),
            XAML = Coalesce(@InXAML, XAML),
            DeveloperNotes = @InDeveloperNotes,
            BaseType = '', -- Sanjeewa 03/08/2012 - Assigned the default value, this field is not used by query service anymore
            [Namespace] = Coalesce(@InNamespace, [Namespace]),
            SoftDelete = 0,
            InsertedByUserAlias = Coalesce(@InInsertedByUserAlias, InsertedByUserAlias),
            InsertedDateTime = Coalesce(@InInsertedDateTime, InsertedDateTime),
            UpdatedByUserAlias = Coalesce(@InUpdatedByUserAlias, UpdatedByUserAlias),
            UpdatedDateTime = Coalesce(@InUpdatedDateTime, GETutcDATE())
        WHERE Id = @InID
        SELECT @Id = @InID; 
		SELECT sa.Id, 
			sa.[GUID], 
			sa.Name,
			sa.ShortName, 
			sa.[Description], 
			sa.MetaTags, 
			sa.IconsId, 
			ic.[Name] AS iconsName,
			sa.IsSwitch, 
			sa.IsService, 
			sa.ActivityLibraryId, 
			al.Name AS ActivityLibraryName,
			al.VersionNumber AS ActivityLibraryVersion,
			al.AuthGroupID,
			ag.Name AS AuthGroupName, 
			sa.IsUxActivity, 
			sa.CategoryId, 
			ac.Name as ActivityCategoryName, 
			tbtn.Name as ToolBoxtabName, 
			sa.ToolBoxtab, 
			sa.IsToolBoxActivity, 
			sa.[Version], 
			sa.StatusId, 
			sc.Name AS StatusCodeName, 
			sa.WorkflowTypeId,
			wft.Name as WorkFlowTypeName, 
			sa.Locked, 
			sa.LockedBy, 
			sa.IsCodeBeside, 
			sa.XAML, 
			sa.DeveloperNotes, 
			sa.BaseType, 
			sa.[Namespace],
			sa.InsertedByUserAlias,
			sa.InsertedDateTime,
			sa.UpdatedByUserAlias,
			sa.UpdatedDateTime
			FROM [dbo].[Activity] sa
			LEFT JOIN ActivityLibrary al ON sa.ActivityLibraryId = al.Id
			JOIN ActivityCategory ac ON sa.CategoryId = ac.Id
			LEFT JOIN ToolBoxTabName tbtn ON sa.ToolBoxTab = tbtn.Id
			JOIN StatusCode sc ON sa.StatusId = sc.Code
			JOIN WorkflowType wft ON sa.WorkflowTypeId = wft.Id
			LEFT JOIN Icon ic ON sa.IconsId = ic.Id 
			LEFT JOIN AuthorizationGroup ag ON ag.Id = al.AuthGroupId
			WHERE sa.Id = @Id
			AND sa.SoftDelete = 0
     END
  
   RETURN @rc
END
