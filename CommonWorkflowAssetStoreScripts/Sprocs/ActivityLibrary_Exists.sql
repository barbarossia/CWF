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
// FileName:ActivityLibrary_Exists.sql
// File description: Checks for the existance of a library.
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   ActivityLibrary_Exists                             *
**    Desc:   Save a new activitylibrary.                                  *
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
**  11/8/2010      v-stska             Original implementation
**  11/15/2010     v-stska             convert from NEW to NEW2 model
**  12/3/2010      v-stska             Use varchar for guids internally
**  12/21/2010     v-stska             Update returned columns
**  2/13/2011      v-stska             Add @outErrorString as parameter
**  11-Nov-2011	   v-richt             Bug#86713 - Change error codes to positive numbers
** *************************************************************************/
CREATE PROCEDURE [dbo].[ActivityLibrary_Exists]
		@inCaller        nvarchar(50),
		@inCallerversion nvarchar (50),
		@inName            nvarchar (255),
		@inVersionNumber   nvarchar (50),
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
	SET @outErrorString = ''
	BEGIN TRY
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
		IF (@InName IS NULL OR @InName = '')
		BEGIN
			SET @outErrorString = 'Invalid Parameter Value (@InName)'
			RETURN 55106
		END
		IF (@InVersionNumber IS NULL OR @InVersionNumber = '')
		BEGIN
			SET @outErrorString = 'Invalid Parameter Value (@InVersionNumber)'
			RETURN 55104
		END
		
		DECLARE @id BIGINT
		SELECT @id = ID
		FROM ActivityLibrary
		WHERE @inName = Name AND @inVersionNumber = VersionNumber
		
		IF (@id IS NULL)
		BEGIN
			SET @outErrorString = 'Invalid Parameter Value (@inActivityLibraryName/@inActivityLibraryVersionNumber)'
			RETURN 55132
		END
		ELSE
		BEGIN
			SELECT Guid
			FROM ActivityLibrary
			WHERE @inName = Name AND @inVersionNumber = VersionNumber
		
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


