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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/**************************************************************************
// Product:  CommonWF
// FileName: AuthorizationGroup_CreateOrUpdate.sql
// File description: Create/update a row in the ltblAuthGroups.
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   AuthorizationGroup_CreateOrUpdate                              *
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
**  11/17/2010     v-stska             Original implementation
**  11/21/2010     v-stska             Add inserted, updated, alias & d
**  11/23/2010     v-stska             Integrate with Update
**  1/5/2011       v-stska             change Insert to Update if row exists
**  2/13/2011      v-stska             Add @OutError logic
**  11-Nov-2011	   v-richt             Bug#86713 - Change error codes to positive numbers
** *************************************************************************/
CREATE PROCEDURE [dbo].[AuthorizationGroup_CreateOrUpdate]
		@inCaller nvarchar(50),
		@inCallerversion nvarchar (50),
		@InId bigint,
		@InGUID uniqueidentifier,
		@InName nvarchar(50),
		@InAuthoringToolLevel int,
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
	DECLARE @InInsertedDateTime datetime
	SET @InInsertedDateTime = GETDATE()
	DECLARE @inUpdatedDateTime datetime
	SET @inUpdatedDateTime = GETDATE()
	BEGIN TRY
		IF (@InId < 0)
		BEGIN
			SET @outErrorString = 'Invalid Parameter Value (@InId)'
			RETURN 55123
		END
	    /* check to see if this needs to be a real insert or an update
		   It could have been soft deleted and an insert will fail on
		   a unique Name/versionNumber constraint */
		DECLARE @CHECKId bigint
		SELECT @CHECKId = ID
		FROM [dbo].[AuthorizationGroup]
		WHERE @inName = name 
		-- If found, change this from an insert to an update
		If (@CHECKId > 0)
			SET @InId = @CHECKId
	IF (@InId = 0 OR @InId IS NULL)
		BEGIN
			IF (@InGuid IS NULL)
			BEGIN
				SET @outErrorString = 'Invalid Parameter Value (@InGUID)'
				RETURN 55105
			END
			IF (@InName IS NULL OR @InName = '')
			BEGIN
				SET @outErrorString = 'Invalid Parameter Value (@InName)'
				RETURN 55106
			END
			IF (@InAuthoringToolLevel IS NULL)
			BEGIN
				SET @outErrorString = 'Invalid Parameter Value (@InAuthoringToolLevel)'
				RETURN 55156
			END
			IF (@InInsertedByUserAlias IS NULL OR @InInsertedByUserAlias = '')
			BEGIN
				SET @outErrorString = 'Invalid Parameter Value (@InInsertedByUserAlias)'
				RETURN 55102
			END
			SET @InUpdatedByUserAlias = @InInsertedByUserAlias
				
			INSERT INTO [dbo].[AuthorizationGroup]
				([GUID], Name, AuthoringToolLevel,SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
				VALUES(@InGUID, @InName, @InAuthoringToolLevel, 0, @InInsertedByUserAlias, @InInsertedDateTime, @InUpdatedByUserAlias, @InUpdatedDateTime)
			 --COMMIT TRANSACTION
		 END
		 ELSE
		 BEGIN
			-- This is an UPDATE
			-- Test for valid @InId
			DECLARE @TEMPID bigint
			SELECT @TEMPID = [Id]
			FROM [dbo].[AuthorizationGroup]
			WHERE Id = @InId
			IF (@TEMPID IS NULL)
			BEGIN
				SET @outErrorString = 'Invalid @InId attempting to perform an UPDATE on table'
				RETURN 55060
			END
				
			UPDATE [dbo].[AuthorizationGroup]
				SET 
					AuthoringToolLevel = Coalesce(@InAuthoringToolLevel, AuthoringToolLevel),
					InsertedByUserAlias = InsertedByUserAlias,
					InsertedDateTime = InsertedDateTime,
					SoftDelete = 0,
					UpdatedByUserAlias = Coalesce(@InUpdatedByUserAlias, UpdatedByUserAlias),
					UpdatedDateTime = @inUpdatedDateTime
			WHERE Id = @InId
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


