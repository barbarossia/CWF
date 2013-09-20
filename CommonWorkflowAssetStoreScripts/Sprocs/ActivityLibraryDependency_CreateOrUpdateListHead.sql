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

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ActivityLibraryDependency_CreateOrUpdateListHead]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ActivityLibraryDependency_CreateOrUpdateListHead]
GO

/**************************************************************************
// Product:  CommonWF
// FileName: ActivityLibraryDependency_CreateOrUpdateListHead.sql
// File description: Save a new Activitylibrary.
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   ActivityLibraryDependency_CreateOrUpdateListHead     *
**    Desc:   Save or Update mtblActivityLibraryDependenciesCreateOrUpdate *
**    Auth:   v-stska                                                      *
**    Date:   5/15/2011                                                    *
**                                                                         *
****************************************************************************
**   sproc logic flow: <Optional if complex> 
**   Parameter definition if complex
****************************************************************************
**                  CHANGE HISTORY
****************************************************************************
**	Date:        Author:             Description:
** ____________    ________________    ____________________________________
**  2/7/2011       v-stska             Original implementation
**  11-Nov-2011    v-richt             Bug#86713 - Change error codes to positive numbers
** *************************************************************************/
CREATE PROCEDURE [dbo].[ActivityLibraryDependency_CreateOrUpdateListHead]
		@inCaller nvarchar(50),
		@inCallerversion nvarchar (50),
		@inActivityLibraryName nvarchar (255),
		@inActivityLibraryVersionNumber nvarchar (50),
		@inInsertedByUserAlias nvarchar(50),
		@inUpdatedByUserAlias nvarchar(50),
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
 
   	SELECT   @rc                = 0
          	,@error             = 0
          	,@rowcount          = 0
          	,@step              = 0
          	,@cObjectName       = OBJECT_NAME(@@PROCID)
	--BEGIN TRANSACTION

	DECLARE @InInsertedDateTime datetime
	SET @InInsertedDateTime = GETDATE()
	DECLARE @inUpdatedDateTime datetime
	SET @inUpdatedDateTime = GETDATE()
	
	BEGIN TRY
		SET @outErrorString = ''
		-- Check the input parameters
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
		IF (@inActivityLibraryName IS NULL OR @inActivityLibraryName = '')
		BEGIN
			SET @outErrorString = 'Invalid Parameter Value (@inActivityLibraryName)'
			RETURN 55128
		END
		IF (@inActivityLibraryVersionNumber IS NULL OR @inActivityLibraryVersionNumber = '')
		BEGIN
			SET @outErrorString = 'Invalid Parameter Value (@inActivityLibraryVersionNumber)'
			RETURN 55129
		END
		
		-- check if both activity libraries exist and get thier PKs
		DECLARE @ActivityLibraryId bigint
		SELECT @ActivityLibraryId = ID
		FROM [dbo].[ActivityLibrary]
		WHERE @inActivityLibraryName = name AND @inActivityLibraryVersionNumber = VersionNumber
		
		-- Bad parameter
		If (@ActivityLibraryId IS NULL)
		BEGIN
			SET @outErrorString = 'Invalid Parameter Value (@inActivityLibraryName/@inActivityLibraryVersionNumber)'
			RETURN 55132
		END

		-- check for a soft delete
		DECLARE @mtblID bigint
		DECLARE @mtblSoftDelete bit
		SELECT @mtblID = ID, @mtblSoftDelete = SoftDelete
		FROM dbo.ActivityLibraryDependencyHead
		WHERE TreeHead = @ActivityLibraryId 
		
		DECLARE @InId bigint
		IF (@mtblID IS NOT NULL AND @mtblID <> 0)
		BEGIN
			SET @InId = @mtblID
		END

		-- This is an update
		IF (@InId > 0)
		BEGIN
		-- Check updated by user alias parameter
		IF (@InUpdatedByUserAlias IS NULL OR @InUpdatedByUserAlias = '')
			BEGIN
				SET @outErrorString = 'Invalid Parameter Value (@InUpdatedByUserAlias))'
				RETURN 55103
			END
		UPDATE ActivityLibraryDependencyHead
		SET TreeHead = @ActivityLibraryId
			,SoftDelete = 0
			,InsertedByUserAlias = COALESCE(@inInsertedByUserAlias, InsertedByUserAlias)
			,InsertedDateTime = @InInsertedDateTime
			,UpdatedByUserAlias = COALESCE(@inUpdatedByUserAlias, UpdatedByUserAlias)
			,UpdatedDateTime = @inUpdatedDateTime
		WHERE @InId = ID
			
		 END
		 ELSE
		 BEGIN
			IF (@inInsertedByUserAlias IS NULL OR @inInsertedByUserAlias = '')
			BEGIN
				SET @outErrorString = 'Invalid Parameter Value (@inInsertedByUserAlias))'
				RETURN 55102
			END
						
			INSERT INTO ActivityLibraryDependencyHead
					(TreeHead, IntersectingTableLinkID, SoftDelete, InsertedByUserAlias, 
					InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
			VALUES(@ActivityLibraryId, null, 0, @inInsertedByUserAlias, 
					@InInsertedDateTime, @inUpdatedByUserAlias, @inUpdatedDateTime)
					
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
		
		  
		  SET @Guid         = NEWID();
		  SET @rc           = 56099
		  SET @step         = ERROR_LINE()
		  SET @ErrorMessage = ERROR_MESSAGE()
		  SET @SeverityCode = ERROR_SEVERITY()
		  SET @Error         = ERROR_NUMBER()
	
		  EXECUTE @rc2 = [dbo].[Error_Raise]
				 @inCaller           = @inCaller        --calling object
				,@inCallerVersion    = @inCallerVersion --calling object version
				,@ErrorGuid          = @Guid
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



