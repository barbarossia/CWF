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

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ActivityCategory_CreateOrUpdate]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ActivityCategory_CreateOrUpdate]
GO

/**************************************************************************
// Product:  CommonWF
// FileName: ActivityCategory_CreateOrUpdate.sql
// File description: Create/update a row on ltblActivityCategory table.
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   ActivityCategory_CreateOrUpdate                        *
**    Auth:   v-stska                                                      *
**    Date:   11/17/2010                                                   *
**                                                                         *
****************************************************************************
**   sproc logic flow: <Optional if complex> 
**   Parameter definition if complex
****************************************************************************
**                  CHANGE HISTORY
****************************************************************************
**	Date:        Author:             Description:
** ____________    ________________    ____________________________________
**  11/17/2010     v-stska             Original implementation
**  11/21/2010     v-stska             Add inserted, updated, alias & d
**  12/6/2010      v-stska             Eliminate inserted/Update DT parms
**  12/12/2010     v-stska             Update to NEW3PrototypeAssetStore
**  12/22/2010     v-stska             Check AuthGroupName for valid entry
**  1/5/2011       v-stska             Change Insert to Update if row exists
**  2/13/2011      v-stska             Add @OutError logic
**  11-Nov-2011	   v-richt             Bug#86713 - Change error codes to positive numbers
** *************************************************************************/
CREATE PROCEDURE [dbo].[ActivityCategory_CreateOrUpdate]
		@inCaller nvarchar(50),
		@inCallerversion nvarchar (50),
		@InId bigint,
		@InGUID uniqueidentifier,
		@InName nvarchar(50),
		@InDescription nvarchar(250),
		@InMetaTags nvarchar (max),
		@InAuthGroupName nvarchar (50),
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
 
   	SELECT   @rc                = 0
          	,@error             = 0
          	,@rowcount          = 0
          	,@step              = 0
          	,@cObjectName       = OBJECT_NAME(@@PROCID)
	--BEGIN TRANSACTION

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
	IF (@InId < 0)
	BEGIN
		SET @outErrorString = 'Invalid Parameter Value (@InId)'
		RETURN 55123
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
		IF (@InId > 0)
		BEGIN
			SELECT @CHECKId = ID
			FROM [dbo].[ActivityCategory]
			WHERE @InId = Id
			-- If found, change this from an insert to an update
			If (@CHECKId > 0)
				SET @InId = @CHECKId
			ELSE
			BEGIN
				SET @outErrorString = 'Invalid @InId attempting to perform an UPDATE on table'
				RETURN 55060
			END
		END
	IF (@InId IS NULL OR @InId = 0)
	BEGIN	
		-- Insert
		IF (@InGUID IS NULL)
			BEGIN
				SET @outErrorString = 'Invalid Parameter Value (@InGUID)'
				RETURN 55105
			END
		IF (@InInsertedByUserAlias IS NULL OR @InInsertedByUserAlias = '')
			BEGIN
				SET @outErrorString = 'Invalid Parameter Value (@InInsertedByUserAlias)'
				RETURN 55102
			END
		IF (@InUpdatedByUserAlias IS NULL OR @InUpdatedByUserAlias = '')
			BEGIN
				SET @InUpdatedByUserAlias = @InInsertedByUserAlias
			END
		IF (@InDescription IS NULL OR @InDescription = '')
			BEGIN
				SET @outErrorString = 'Invalid Parameter Value (@InDescription)'
				RETURN 55107
			END
		IF (@InName IS NULL OR @InName = '')
			BEGIN
				SET @outErrorString = 'Invalid Parameter Value (@InName)'
				RETURN 55106
			END
		IF (@InMetaTags IS NULL OR @InMetaTags = '')
			BEGIN
				SET @outErrorString = 'Invalid Parameter Value (@InMetaTags)'
				RETURN 55124
			END
		IF (@InAuthGroupName IS NULL OR @InAuthGroupName = '')
			BEGIN
				SET @outErrorString = 'Invalid Parameter Value (@InAuthGroupName)'
				RETURN 55118
			END
			
		--Check @InAuthGroupName for validity
		DECLARE @AuthGroupId bigint
		
		SELECT @AuthGroupId = Id
		FROM [dbo].[AuthorizationGroup] ag
		WHERE ag.Name = @InAuthGroupName
		
		IF (@AuthGroupId IS NULL)
		BEGIN
			SET @outErrorString = 'Invalid Parameter Value (@InAuthGroupName)'
			RETURN 55118
		END
		
		INSERT INTO [dbo].[ActivityCategory]
			([GUID], Name, [Description], MetaTags, AuthGroupID, SoftDelete,
			InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
			VALUES (@InGUID, @InName, @InDescription, @InMetaTags, @AuthGroupId, 0,
			@InInsertedByUserAlias, @InInsertedDateTime, @InUpdatedByUserAlias, @InUpdatedDateTime)
	END 
	ELSE
	BEGIN
		-- This is an UPDATE
		-- Test for valid @InId
		DECLARE @TEMPID bigint
		SELECT @TEMPID = [Id]
		FROM [dbo].[ActivityCategory]
		WHERE Id = @InId
		IF (@TEMPID IS NULL)
			RETURN 55060
			
		SET @InUpdatedDateTime = GETDATE();
		
		DECLARE @AuthGroupId1 bigint
		IF(@InAuthGroupName IS NOT NULL )
		BEGIN
			SELECT @AuthGroupId1 = Id
			FROM [dbo].[AuthorizationGroup] ag
			WHERE ag.Name = @InAuthGroupName
		END
		
		IF (@AuthGroupId1 IS NULL)
		BEGIN
			SET @outErrorString = 'Invalid Parameter Value (@InAuthGroupName)'
			RETURN 55118
		END
		
		IF (@InUpdatedByUserAlias IS NULL OR @InUpdatedByUserAlias = '')
		BEGIN
			SET @outErrorString = 'Invalid Parameter Value (@InUpdatedByUserAlias)'
			RETURN 55103
		END
		
		UPDATE [dbo].[ActivityCategory]
		SET 
			[GUID] = Coalesce(@InGUID, [GUID]),
			Name = Coalesce(@InName, Name),
			[Description] = Coalesce(@InDescription, [Description]),
			MetaTags = Coalesce(@InMetaTags, MetaTags),
			AuthGroupID = Coalesce(@AuthGroupId1, AuthGroupID),
			SoftDelete = 0,
			UpdatedByUserAlias = @InUpdatedByUserAlias,
			UpdatedDateTime = @InUpdatedDateTime
		WHERE Id=@InId
	END
		 --COMMIT TRANSACTION
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


