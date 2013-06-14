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
// FileName: ToolboxTabName_Get.sql
// File description: Get a list of toolbox items.
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   ToolboxTabName_Get                                     *
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
**  11/23/2010      v-stska             Original implementation
**  12/21/2010      v-stska             Update for NEW3PrototypeAssetStore
**  2/13/2011       v-stska             Add @OutError logic
**  11-Nov-2011     v-richt             Bug#86713 - Change error codes to positive numbers
** *************************************************************************/
CREATE PROCEDURE [dbo].[ToolboxTabName_Get]
		@inCaller nvarchar(50),
		@inCallerversion nvarchar (50),
		@InName nvarchar (50),
		@InId bigint,
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
	
	DECLARE @TEMPID bigint
	DECLARE @TEMPname nvarchar (50)
	DECLARE @InIdExists bit
	DECLARE @InNameExists bit
	SET @InIdExists = 0
	SET @InNameExists = 0
	
	BEGIN TRY
	IF (@InId < 0)
	BEGIN
		SET @outErrorString = 'Invalid Parameter Value (@InId)'
		RETURN 55123
	END
	ELSE
	IF(@InId > 0)
	BEGIN
		-- Test for valid @InId
		DECLARE @SoftDelete bit
		SELECT @TEMPID = [Id], @SoftDelete = SoftDelete
		FROM [dbo].[ToolBoxTabName]
		WHERE Id = @InId
		IF (@TEMPID IS NULL)
		BEGIN
			SET @outErrorString = 'Invalid @InId attempting to perform a GET on table'
			RETURN 55040
		END
		ELSE
		IF (@SoftDelete = 1)
		BEGIN
			SET @outErrorString = 'Invalid @InId attempting to perform a GET on table that is marked soft delete'
			RETURN 55044
		END
		ELSE
			SET @InIdExists = 1
	END
	ELSE
	IF (@InName IS NOT NULL AND @InName <> '')
	BEGIN
		-- Test for valid @Name
		SELECT @TEMPname = [name], @TEMPID = Id
		FROM [dbo].[ToolBoxTabName]
		WHERE @InName = [Name]
		IF (@TEMPname IS NULL)
		BEGIN
			SET @outErrorString = 'Invalid @InName attempting to perform a GET on table'
			RETURN 55042
		END
		ELSE
			SET @InNameExists = 1
	END
		SELECT			
			tb.Name,
			tb.Id
		FROM			
			[ToolBoxTabName] tb
		WHERE (@InName = tb.Name OR @InNameExists <> 1) AND
			  (@InId = tb.Id OR @InIdExists <> 1) AND 
			  tb.SoftDelete = 0
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