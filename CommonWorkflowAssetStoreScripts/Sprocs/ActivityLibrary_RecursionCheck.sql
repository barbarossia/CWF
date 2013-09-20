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

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ActivityLibrary_RecursionCheck]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ActivityLibrary_RecursionCheck]
GO

/**************************************************************************
// Product:  CommonWF
// FileName: ActivityLibrary_RecursionCheck.sql
// File description: Check if the m,n entry for the Activity Dependency 
//                   list will be recursive.
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   ActivityLibrary_RecursionCheck                       *
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
**   4/6/2010      v-stska             Original implementation
**  11-Nov-2011    v-richt             Bug#86713 - Change error codes to positive numbers
** *************************************************************************/
CREATE PROCEDURE [dbo].[ActivityLibrary_RecursionCheck]
		@inCaller nvarchar(50),
		@inCallerversion nvarchar (50),
		@inActivityLibraryId bigint,
		@inActivityLibraryDependentId bigInt,
		@inBaseStartM bigint,
		@inLoopCount bigint,
		@outErrorString nvarchar (300)OUTPUT

--WITH ENCRYPTION
AS
BEGIN
    SET NOCOUNT ON

	DECLARE @Table table (Id bigint IDENTITY(1,1),
					  ActivityLibraryID bigint,
					  DependentActivityLibraryId bigint)
	DECLARE @StartM bigint
	DECLARE @StartN bigint
	DECLARE @TempM bigint
	DECLARE @TempN bigint
	DECLARE @NextM bigint

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
		IF (@inActivityLibraryId IS NULL OR @inActivityLibraryId = '')
		BEGIN
			SET @outErrorString = 'Invalid Parameter Value (@inActivityLibraryName)'
			RETURN 55128
		END
		IF (@inActivityLibraryDependentId IS NULL OR @inActivityLibraryDependentId = '')
		BEGIN
			SET @outErrorString = 'Invalid Parameter Value (@inActivityLibraryVersionNumber)'
			RETURN 55129
		END
		
		-- check if both activity libraries exist and get their PKs
		DECLARE @ActivityLibraryId bigint
		SELECT @ActivityLibraryId = ID
		FROM [dbo].[ActivityLibrary] al
		WHERE @inActivityLibraryId = al.Id
		
		-- Bad parameter
		If (@ActivityLibraryId IS NULL)
		BEGIN
			SET @outErrorString = 'Invalid Parameter Value (@inActivityLibraryName/@inActivityLibraryVersionNumber)'
			RETURN 55132
		END
		
		DECLARE @ActivityLibraryDependentId bigint
		SELECT @ActivityLibraryDependentId = ID
		FROM [dbo].[ActivityLibrary] al
		WHERE @inActivityLibraryDependentId = al.Id
		
		-- Bad parameter
		If (@ActivityLibraryDependentId IS NULL)
		BEGIN
			SET @outErrorString = 'Invalid Parameter Value (@inActivityLibraryDependentName/@inActivityLibraryDependentVersionNumber)'
			RETURN 55133
		END

		IF(@inActivityLibraryId = @inBaseStartM)
		BEGIN
			SET @outErrorString = 'Found a recursion match'
			RETURN 55001
		END
		
		-- Load the @Table with the Dependency that we are looking for
		SET @StartM = @inActivityLibraryId
		SET @StartN =  @inActivityLibraryDependentId
		
		INSERT INTO @Table (ActivityLibraryID, DependentActivityLibraryId)
		SELECT ActivityLibraryID, DependentActivityLibraryId
		FROM ActivityLibraryDependency
		WHERE DependentActivityLibraryId = @StartM
	
		-- Go through the entire table with recursive calls
		DECLARE @Total bigint
		DECLARE @Ix bigint
		DECLARE @RESULTS int
		DECLARE @TotalLoops int
		SET @TotalLoops = @inLoopCount + 1
		
		SELECT @Total = COUNT(Id)
		FROM @Table

		SET @Ix = 1

		WHILE @Ix <= @Total
		BEGIN
			SELECT @TempM = ActivityLibraryID, @TempN = DependentActivityLibraryId
			FROM @Table
			WHERE Id = @Ix

			IF (@TempM = @inBaseStartM)
			BEGIN
				-- found a recursion
				SET @outErrorString = 'Found a recursion match'
				RETURN 55001
			END
			
			IF (@TotalLoops <= 100)
			BEGIN
				EXEC @RESULTS = ActivityLibrary_RecursionCheck
								@inCaller = @inCaller,
								@inCallerversion = @inCallerversion,
								@inActivityLibraryId = @TempM,
								@inActivityLibraryDependentId = @TempN,
								@inBaseStartM = @inBaseStartM,
								@inLoopCount = @TotalLoops,
								@outErrorString = @outErrorString
							
				IF (@RESULTS = 1)
				BEGIN
					-- found a recursion
					SET @outErrorString = 'Found a recursion match'
					RETURN 55001
				END
				IF (@RESULTS = 2)
				BEGIN
					-- loop count exceeded
					SET @outErrorString = 'Loop count exceeded'
					RETURN 55002
				END
			END
			ELSE
			BEGIN
				SET @outErrorString = 'Loop count exceeded'
				RETURN 55002
			END

			SET @Ix = @Ix + 1
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


