SET ANSI_DEFAULTS ON
SET CURSOR_CLOSE_ON_COMMIT OFF
SET IMPLICIT_TRANSACTIONS OFF
SET ARITHABORT ON
SET CONCAT_NULL_YIELDS_NULL ON
SET NUMERIC_ROUNDABORT OFF
SET QUOTED_IDENTIFIER ON
 
SET DATEFORMAT ymd
SET LOCK_TIMEOUT -1
SET NOCOUNT ON
SET ROWCOUNT 0
SET TEXTSIZE 0
GO

/**************************************************************************
// Product:  CommonWF
// FileName: Activity_GetByCategoryNameWoExecutable.sql
// File description: Get all StoreActivities rows of CategoryType. Do not
//                   include the XAML column.
//
// Copyright 2011 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   Activity_GetByCategoryNameWoExecutable.              *
**    Auth:   v-stska                                                      *
**    Date:   3/22/2011                                                    *
**                                                                         *
****************************************************************************
**   sproc logic flow: <Optional if complex> 
**   Parameter definition if complex
****************************************************************************
**                  CHANGE HISTORY
****************************************************************************
**	Date:        Author:             Description:
** ____________    ________________    ____________________________________
**  3/22/2011      v-stska             Original implementation
**  11-Nov-2011    v-richt             Bug#86713 - Change error codes to positive numbers
** *************************************************************************/
CREATE PROCEDURE [dbo].[Activity_GetByCategoryNameWithoutExecutable]
		@inCaller nvarchar(50),
		@inCallerversion nvarchar (50),
        @InCategoryName varchar(50),
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
          ,@Guid1             [nvarchar] (36)
 
   	SELECT   @rc                = 0
          	,@error             = 0
          	,@rowcount          = 0
          	,@step              = 0
          	,@cObjectName       = OBJECT_NAME(@@PROCID)
          	
	--BEGIN TRANSACTION
	BEGIN TRY
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
		IF (@InCategoryName IS NULL OR @InCategoryName = '')
		BEGIN
			SET @outErrorString = 'Invalid Parameter Value (@InCategoryName)'
			RETURN 55163
		END
	
		-- Test for valid @InCategoryName
		DECLARE @TEMPID bigint
		DECLARE @SoftDelete bit
		
		SELECT @TEMPID = ID, @SoftDelete = SoftDelete
		FROM WorkflowType
		WHERE @InCategoryName = name

		IF (@TEMPID IS NULL)
		BEGIN
			SET @outErrorString = 'Invalid @InCategoryName attempting to perform a GET on table'
			RETURN 55040
		END
		ELSE
		-- @InCategoryName is valid, however, is softdelete set
		IF (@SoftDelete = 1)
		BEGIN
			SET @outErrorString = 'Invalid @InCategoryName attempting to perform a GET on table that is marked soft delete'
			RETURN 55044
		END
		
		-- are there any rows available
		DECLARE @COUNT bigint
		
		SELECT @COUNT = COUNT(Id)
		FROM [dbo].[Activity] sa
		WHERE sa.WorkflowTypeId = @TEMPID
			AND sa.SoftDelete = 0 
		
		IF (@COUNT = 0)
		BEGIN
			SET @outErrorString = 'No StoreActivity entries with @InCategoryName FK while attempting to perform a GET on table StoreActivities'
			RETURN 55048
		END
		
		-- Get the rows for the CategoryName type that are not soft deleted
		SELECT  sa.[Id],
				sa.[GUID],
				sa.[Name],
				sa.[ShortName],
				sa.[Version],
				sa.[Description],
				sa.[MetaTags],
				ag.Id AS AuthGroupId,
				ag.Name AS AuthGroupName,
				sa.[IconsId],
				li.[Name] AS IconName,
				sa.[IsSwitch],
				sa.[IsService],
				sa.[ActivityLibraryId],
				al.[Name] AS ActivityLibraryName,
				al.[VersionNumber] AS ActivityLibraryVersion,
				sa.[IsUxActivity],
				sa.[CategoryId],
				ac.[Name] AS CategoryName,
				sa.[ToolBoxTab],
				sa.[IsToolBoxActivity],
				sa.[StatusId],
				sc.[Name] AS StatusCodeName,
				sa.[WorkflowTypeId],
				wt.[Name] AS WorkFlowTypeName,
				wt.GUID AS WorkFlowTypeGuid,
				sa.[Locked],
				sa.[LockedBy],
				sa.[IsCodeBeside],
				--sa.[XAML],
				sa.[DeveloperNotes],
				sa.[BaseType],
				sa.[Namespace],
				--sa.[SoftDelete],
				sa.[InsertedByUserAlias],
				sa.[InsertedDateTime],
				sa.[UpdatedByUserAlias],
				sa.[UpdatedDateTime]  
				--sa.[Timestamp],
				--sa.[Url],
		FROM [dbo].[Activity] sa
		LEFT JOIN Icon li ON sa.IconsId = li.Id
		LEFT JOIN ActivityLibrary al ON SA.ActivityLibraryId = al.Id
		LEFT JOIN ActivityCategory ac ON sa.CategoryId = ac.Id
		LEFT JOIN StatusCode sc ON sa.StatusId = sc.Code
		LEFT JOIN WorkflowType wt ON sa.WorkflowTypeId = wt.Id
		LEFT JOIN AuthorizationGroup ag ON wt.AuthGroupId = ag.Id
		WHERE sa.WorkflowTypeId = @TEMPID
			AND sa.SoftDelete = 0
			
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
		
		  -- error - could not Select from etblActivityLibraries
		  SET @Guid1         = NEWID();
		  SET @rc           = 56099
		  SET @step         = ERROR_LINE()
		  SET @ErrorMessage = ERROR_MESSAGE()
		  SET @SeverityCode = ERROR_SEVERITY()
		  SET @Error         = ERROR_NUMBER()

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
GO

