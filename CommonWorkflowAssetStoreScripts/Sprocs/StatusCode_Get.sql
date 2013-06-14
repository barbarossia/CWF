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
// FileName: StatusCode_Get.sql
// File description: Get a row in ltblStatusCodes.
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   StatusCode_Get                                        *
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
**  12/12/2010      v-stska             Update to NEW3PrototypeAssetStore
**  2/13/2011       v-stska             Add @OutError logic
**  11-Nov-2011     v-richt             Bug#86713 - Change error codes to positive numbers
** *************************************************************************/
CREATE PROCEDURE [dbo].[StatusCode_Get]
		@inCaller nvarchar(50),
		@inCallerversion nvarchar (50),
		@InCode bigint,
        @InName varchar(50),
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
	DECLARE @TEMPID bigint
	DECLARE @TEMPname nvarchar (50)
	DECLARE @InIdExists bit
	DECLARE @InNameExists bit
	SET @InIdExists = 0
	SET @InNameExists = 0
	
	BEGIN TRY
		IF (@InCode < 0)
		BEGIN
			SET @outErrorString = 'Invalid Parameter Value (@InCode)'
			RETURN 55127
		END
		ELSE
		IF (@InCode > 0)
		BEGIN
			-- Test for valid @InId
			DECLARE @SoftDelete bit
			SELECT @TEMPID = [code], @SoftDelete = SoftDelete
			FROM [dbo].[StatusCode]
			WHERE Code = @InCode
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
			SELECT @TEMPname = [name], @TEMPID = @InCode
			FROM [dbo].[StatusCode]
			WHERE @InName = [Name]
			IF (@TEMPname IS NULL)
			BEGIN
				SET @outErrorString = 'Invalid @InName attempting to perform a GET on table'
				RETURN 55042
			END
			ELSE
				SET @InNameExists = 1
		END
		
		SELECT sc.[Code]
				  ,sc.[Name]
				  ,sc.[Description]
				  ,sc.[ShowInProduction]
				  ,sc.[LockForChanges]
				  ,sc.[IsDeleted]
				  ,sc.[IsEligibleForCleanUp]
			  FROM [dbo].[StatusCode] sc
			  WHERE (@InName = sc.Name OR @InNameExists <> 1) AND
					(@InCode = sc.Code OR @InIdExists <> 1) 
					 AND sc.SoftDelete = 0
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

