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
// FileName: StatusCode_CreateOrUpdate.sql
// File description: Create/update an entry in the ltblStatusCode table.
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   StatusCode_CreateOrUpdate                              *
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
**  11/21/2010     v-stska             Add inserted, updated, alias & dt	
**  11/23/2010     v-stska             Update logic	
**  12/12/2010     v-stska             Update to NEW3PrototypeAsetStore
**  1/5/2011       v-stska             Change Insert to Update if row exists
**  2/13/2011      v-stska             Add @OutError logic
**  11-Nov-2011	   v-richt             Bug#86713 - Change error codes to positive numbers
** *************************************************************************/
CREATE PROCEDURE [dbo].[StatusCode_CreateOrUpdate]
		@inCaller nvarchar(50),
		@inCallerversion nvarchar (50),
		@InCode int,
		@InName nvarchar(50),
		@InDescription nvarchar(250),
		@InShowInProduction bit,
		@InLockForChanges bit,
		@InIsDeleted bit,
		@InIsEligibleForCleanUp bit,
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
		IF (@InCode < 0)
		BEGIN
			SET @outErrorString = 'Invalid Parameter Value (@InCode)'
			RETURN 55127
		END
		DECLARE @Code int
		SELECT @Code = code
		FROM [dbo].[StatusCode]
		WHERE Name = @InName
		
	IF (@Code IS NULL)
		BEGIN
		IF (@InName IS NULL OR @InName = '')
		BEGIN
			SET @outErrorString = 'Invalid Parameter Value (@InName)'
			RETURN 55106
		END
		IF (@InDescription IS NULL OR @InDescription = '')
		BEGIN
			SET @outErrorString = 'Invalid Parameter Value (@InDescription)'
			RETURN 55107
		END
		IF (@InShowInProduction IS NULL)
		BEGIN
			SET @outErrorString = 'Invalid Parameter Value (@InShowInProduction)'
			RETURN 55159
		END
		IF (@InLockForChanges IS NULL)
		BEGIN
			SET @outErrorString = 'Invalid Parameter Value (@InLockForChanges)'
			RETURN 55160
		END
		IF (@InInsertedByUserAlias IS NULL OR @InInsertedByUserAlias = '')
		BEGIN
			SET @outErrorString = 'Invalid Parameter Value (@InInsertedByUserAlias)'
			RETURN 55102
		END
		IF (@InUpdatedByUserAlias IS NULL OR @InUpdatedByUserAlias = '')
			SET @InUpdatedByUserAlias = @InInsertedByUserAlias

		INSERT INTO [dbo].[StatusCode]
			(Code, Name, [Description], ShowInProduction, LockForChanges, IsDeleted, IsEligibleForCleanUp, SoftDelete,
			InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
			VALUES(@InCode, @InName, @InDescription, @InShowInProduction, @InLockForChanges, @InIsDeleted, @InIsEligibleForCleanUp, 0,
			@InInsertedByUserAlias, @InInsertedDateTime, @InUpdatedByUserAlias, @InUpdatedDateTime)
		 --COMMIT TRANSACTION
		 END
		 ELSE
		 BEGIN
			-- This is an UPDATE
			-- Test for valid @InId
			DECLARE @TEMPID bigint
			SELECT @TEMPID = [Code]
			FROM [dbo].[StatusCode]
			WHERE Code = @InCode
			IF (@TEMPID IS NULL)
			BEGIN
				SET @outErrorString = 'Invalid @InId attempting to perform an UPDATE on table'
				RETURN 55060
			END
				
			UPDATE [dbo].[StatusCode]
			SET [Description] = Coalesce(@InDescription, [Description]),
				ShowInProduction = Coalesce(@InShowInProduction, ShowInProduction),
				LockForChanges = Coalesce(@InLockForChanges, LockForChanges),
				IsDeleted = Coalesce(@InIsDeleted, IsDeleted),
				IsEligibleForCleanUp = Coalesce(@InIsEligibleForCleanUp, IsEligibleForCleanUp),
				SoftDelete = 0,
				updatedByUserAlias = @InUpdatedByUserAlias,
				UpdatedDateTime = GETDATE()
			WHERE Code = @InCode
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

