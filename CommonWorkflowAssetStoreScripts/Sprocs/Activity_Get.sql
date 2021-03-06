/****** Object:  StoredProcedure [dbo].[Activity_Get]    Script Date: 08/03/2012 16:22:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/**************************************************************************
// Product:  CommonWF
// FileName: Activity_Get.sql
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   Activity_Get                                    *
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
**  11/22/2010     v-stska             Original implementation
**  12/16/2010     v-stska             Modify for NEW3 db & get all	
**  12/21/2010     v-stska             Eliminate dynamic SQL
**  2/13/2011      v-stska             Add @OutError logic
**  6/21/2011      v-stska             Make @InName 255 (From 50)
**  11-Nov-2011	   v-richt             Bug#86713 - Change error codes to positive numbers
**  01-Mar-2012    v-luval			   Added code to return also the InsertedByUserAlias in the query
** *************************************************************************/
create PROCEDURE [dbo].[Activity_Get]
		@inCaller			nvarchar(50),
		@inCallerversion	nvarchar (50),
		@InId				bigint,
		@InGuid	varchar(50)   ,
		@InName				nvarchar(255),
		@InVersion			nvarchar(25),
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
          ,@Guid1              [nvarchar] (36)
 
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
	
	BEGIN TRY
		DECLARE @InNameExists bit
		DECLARE @InVersionExists bit
		
		IF (@InId < 0)
		BEGIN
			SET @outErrorString = 'Invalid Parameter Value (@InId)'
			RETURN 55123
		END
		ELSE
		IF(@InId > 0)
		BEGIN
			DECLARE @id bigint
			DECLARE @SoftDelete bit
			SELECT @id = ID, @SoftDelete = SoftDelete
			FROM  [dbo].[Activity]
			WHERE Id = @Inid
			
			IF (@id IS NULL)
			BEGIN
				SET @outErrorString = 'Invalid @InId attempting to perform a GET on table'
				RETURN 55040
			END
			ELSE
			BEGIN
				IF (@SoftDelete = 1)
				BEGIN
					SET @outErrorString = 'Invalid @InId attempting to perform a GET on table that is marked soft delete'
					RETURN 55044
				END
			END
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
			WHERE sa.Id = @InId
				AND sa.SoftDelete = 0
				
		END
		ELSE
			IF (@InGUID <> '00000000-0000-0000-0000-000000000000')
		BEGIN
			SELECT @id = ID
			FROM [dbo].[Activity]
			WHERE @InGuid = guid
			
			IF (@id IS NULL)
			BEGIN
				SET @outErrorString = 'Invalid @InGUID attempting to perform a GET on table'
				RETURN 55041
			END
			
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
					al.AuthGroupID,
					ag.Name AS AuthGroupName,
					al.VersionNumber AS ActivityLibraryVersion, 
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
			WHERE sa.[GUID] = @InGuid
				AND sa.SoftDelete = 0
				
		END
		ELSE
		BEGIN					
			IF (@InName IS NULL or @InName = '')
				SET @InNameExists = 0
			ELSE
				SET @InNameExists = 1
				
			IF (@InVersion IS NULL or @InVersion = '')
				SET @InVersionExists = 0
			ELSE
				SET @InVersionExists = 1
				
			SELECT @id = ID
			FROM [dbo].[Activity]
			WHERE Name = @InName AND  [Version] = @InVersion
			
			IF (@InNameExists = 0 AND @InVersionExists = 0) OR (@id IS NULL)
			BEGIN
				SET @outErrorString = 'Invalid @Name/@Version attempting to perform a GET on table'
				RETURN 55047
			END
			
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
			WHERE (@InName = sa.Name OR @InNameExists <> 1) AND
				(@InVersion = sa.[Version] OR @InVersionExists <> 1)
				AND sa.SoftDelete = 0 
		END
	END TRY
	BEGIN CATCH
		/*
           Available error values from CATCH
           ERROR_NUMBER() ,ERROR_SEVERITY() ,ERROR_STATE() ,ERROR_PROCEDURE() ,ERROR_LINE() ,ERROR_MESSAGE()
        */
		SELECT @error    = @@ERROR
			 ,@rowcount = @@ROWCOUNT
		IF @error <> 0
		BEGIN
		  SET @Guid1         = NEWID();
		  SET @rc           = 56099
		  SET @step         = ERROR_LINE()
		  SET @ErrorMessage = ERROR_MESSAGE()
		  SET @SeverityCode = ERROR_SEVERITY()
		  SET @Error         = ERROR_NUMBER()
		  --IF @@TRANCOUNT <> 0
		  --BEGIN
			 --ROLLBACK TRAN
		  --END
		  EXECUTE @rc2 = [dbo].[Error_Raise]
				 @inCaller           = @inCaller        --calling object
				,@inCallerVersion    = @inCallerVersion --calling object version
				,@ErrorGuid          = @Guid1
				,@inMethodName       = @cObjectName     --current object
				,@inMethodStep       = @step
				,@inErrorNumber      = @Error
				,@inRowsAffected     = @rowcount
				,@inSeverityCode     = @SeverityCode
				,@inErrorMessage     = @ErrorMessage
		  SET @outErrorString = @ErrorMessage
		  RETURN @Error
		 END
	END CATCH
   RETURN @rc

END
