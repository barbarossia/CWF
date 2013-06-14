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
// FileName: Application_CreateOrUpdate.sql
// File description: Create/Update an ltblApplications row.
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   Application_CreateOrUpdate                            *
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
**  11/23/2010     v-stska             Combine with Update
**  12/6/2010      v-stska             Eliminate inserted/updated DT params
**  1/5/2011       v-stska             Change Insert to Update if row exists
**  2/13/2011      v-stska             Add @OutError logic
**  11-Nov-2011	   v-richt             Bug#86713 - Change error codes to positive numbers
** *************************************************************************/
CREATE PROCEDURE [dbo].[Application_CreateOrUpdate]
		@inCaller nvarchar(50),
		@inCallerversion nvarchar (50),
		@InId bigint,
		@InGuid varchar (50),
		@InName nvarchar(50),
		@InDescription nvarchar(250),
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
		IF (@InId IS NULL OR @InId = 0) AND (@inGuid IS NULL OR @InGuid = '00000000-0000-0000-0000-000000000000') AND (@InName IS NULL OR @InName = '')
		BEGIN
			SET @outErrorString = 'Invalid Parameter Value (@InId, @InName, @inGuid cannot all be null)'
			RETURN 55157
		END
	/* check to see if this needs to be a real insert or an update
		   It could have been soft deleted and an insert will fail on
		   a unique Name/versionNumber constraint */
		DECLARE @CHECKID bigint
		IF (@InId > 0)
		BEGIN
			SELECT @CHECKID = ID
			FROM [dbo].[Application]
			WHERE @InId = Id
			-- If found, change this from an insert to an update
			If (@CHECKID > 0)
				SET @InId = @CHECKID
			ELSE
			BEGIN
				SET @outErrorString = 'Invalid @InId attempting to perform an UPDATE on table'
				RETURN 55060
			END
		END
		ELSE
		IF (@inName IS NOT NULL or @inName <> '')
		BEGIN
			SET @CHECKID =  NULL
			SELECT @CHECKID = ID
			FROM [dbo].[Application]
			WHERE @InName = name
			-- If found, change this from an insert to an update
			If (@CHECKID IS NOT NULL)
				SET @InId = @CHECKID
		END
		ELSE
		IF(@InGuid IS NOT NULL OR @InGuid <> '00000000-0000-0000-0000-000000000000')
		BEGIN
			SET @CHECKID =  NULL
			SELECT @CHECKID = ID
			FROM [dbo].[Application]
			WHERE @InGuid = [GUID]
			-- If found, change this from an insert to an update
			If (@CHECKID IS NOT NULL)
				SET @InId = @CHECKID
		END
	IF (@InId = 0 OR @InId IS NULL)
		BEGIN
		-- insert
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
		IF (@InDescription IS NULL OR @InDescription = '')
		BEGIN
			SET @outErrorString = 'Invalid Parameter Value (@InDescription)'
			RETURN 55107
		END
		IF (@InInsertedByUserAlias IS NULL OR @InInsertedByUserAlias = '')
		BEGIN
			SET @outErrorString = 'Invalid Parameter Value (@InInsertedByUserAlias)'
			RETURN 55102
		END
		IF (@InUpdatedByUserAlias IS NULL OR @InUpdatedByUserAlias = '')
			SET @InUpdatedByUserAlias = @InInsertedByUserAlias
			
		INSERT INTO [dbo].[Application]
			(GUID, Name, [Description],SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
			VALUES(@InGuid, @InName, @InDescription, 0, @InInsertedByUserAlias, @InInsertedDateTime, @InUpdatedByUserAlias, @InUpdatedDateTime)
			
		 --COMMIT TRANSACTION
		 END
		 ELSE
		 BEGIN
			-- Test for valid @InId
			DECLARE @TEMPID bigint
			SELECT @TEMPID = [Id]
			FROM [dbo].[Application]
			WHERE Id = @InId
			IF (@TEMPID IS NULL)
			BEGIN
				SET @outErrorString = 'Invalid @InId attempting to perform an UPDATE on table'
				RETURN 55060
			END
			
			UPDATE [dbo].[Application]
				SET 
					[Description] = Coalesce(@InDescription, [Description]),
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


