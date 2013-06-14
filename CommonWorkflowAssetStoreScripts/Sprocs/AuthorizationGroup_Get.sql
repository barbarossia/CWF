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
// FileName: AuthorizationGroup_Get.sql
// File description: Get a row in ltblAuthGroups.
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   AuthorizationGroup_Get                                         *
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
**  11-Nov-2011	    v-richt             Bug#86713 - Change error codes to positive numbers
** *************************************************************************/
CREATE PROCEDURE [dbo].[AuthorizationGroup_Get]
		@inCaller nvarchar(50),
		@inCallerversion nvarchar (50),
		@InId bigint,
        @InName varchar(50),
        @InGuid nvarchar (50),
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
	DECLARE @ID bigint
	DECLARE @Name nvarchar (50)
	DECLARE @Guid nvarchar (50)
	DECLARE @InIdExists bit
	DECLARE @InNameExists bit
	DECLARE @InGuidExists bit
	SET @InIdExists = 0
	SET @InNameExists = 0
	SET @InGuidExists = 0
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
			SELECT @ID = ID, @SoftDelete = SoftDelete
			FROM [dbo].[AuthorizationGroup]
			WHERE ID = @InId
			IF (@ID IS NULL)
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
			SELECT @Name= [name]
			FROM [dbo].[AuthorizationGroup]
			WHERE @InName = [Name]
			IF (@Name IS NULL)
			BEGIN
				SET @outErrorString = 'Invalid @InName attempting to perform a GET on table'
				RETURN 55042
			END
			ELSE
			SET @InNameExists = 1
		END	
		ELSE
		IF (@InGuid <> '00000000-0000-0000-0000-000000000000')
		BEGIN
		-- Test for valid @Name
			SELECT @Guid= [Guid], @ID = Id
			FROM [dbo].[AuthorizationGroup]
			WHERE @InGuid = [Guid]
			IF (@Guid IS NULL)
			BEGIN
				SET @outErrorString = 'Invalid @InGUID attempting to perform a GET on table'
				RETURN 55041
			END
			ELSE
			SET @InGuidExists = 1
		END
			SELECT ag.[Id]
				  ,ag.[Guid]
				  ,ag.[Name]
				  ,ag.[AuthoringToolLevel]
			  FROM [dbo].[AuthorizationGroup] ag
			  WHERE (@InName = ag.Name OR @InNameExists <> 1) AND
					(@InId = ag.Id OR @InIdExists <> 1) AND
					(@InGuid = ag.[Guid] OR @InGuidExists <> 1) AND
					 ag.SoftDelete = 0

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


