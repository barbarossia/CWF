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
// FileName: PublishWorkflow_GetByWorkflowType.sql
// File description: Gets specific dependency head.
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* ********************************************************************
**    Name:   PublishWorkflow_GetByWorkflowType                     *
**    Desc:   Gets all Dependency Heads.                              *
**    Auth:   v-stska                                                 *
**    Date:   5/9/2011                                                *
**                                                                    *
***********************************************************************
**   sproc logic flow: <Optional if complex> 
**   Parameter definition if complex
***********************************************************************
**                  CHANGE HISTORY
***********************************************************************
**	Date:        Author:             Description:
** ____________    ________________    ______________________________
**  5/9/2011       v-stska             Original implementation
**  11-Nov-2011    v-richt             Bug#86713 - Change error codes to positive numbers
** ********************************************************************/
CREATE PROCEDURE [dbo].[PublishWorkflow_GetByWorkflowType]
		@InCaller nvarchar(50),
		@InCallerversion nvarchar (50),
		@InWorkflowType nvarchar (50),
		@OutErrorString nvarchar (300) OUT
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
	
	BEGIN TRY
		IF (@inCaller IS NULL OR @inCaller = '')
		BEGIN
			SET @OutErrorString = 'Invalid Parameter Value (@inCaller)'
			RETURN 55100
		END
		IF (@inCallerversion IS NULL OR @inCallerversion = '')
		BEGIN
			SET @OutErrorString = 'Invalid Parameter Value (@inCallerversion)'
			RETURN 55101
		END
		IF (@InWorkflowType IS NULL OR @InWorkflowType = '')
		BEGIN
			SET @OutErrorString = 'Invalid Parameter Value (@InWorkflowTypeName)'
			RETURN 55117
		END
		
		DECLARE @TempID bigint
		DECLARE @PublishingWorkflowID bigint
		SELECT @TempID = ID, @PublishingWorkflowID = PublishingWorkflowId
		FROM WorkflowType wt
		WHERE wt.Name = @InWorkflowType
			  
		IF (@TempID IS NULL)
		BEGIN
			SET @OutErrorString = 'Invalid Parameter Value (@InWorkflowTypeName)'
			RETURN 55117
		END
		
		IF (@PublishingWorkflowID IS NULL)
		BEGIN
			SET @OutErrorString = 'Workflow type does not have publishing workflow assigned'
			RETURN 55175
		END
		
		SELECT wft.Id, 
			   wft.Name AS WorkFlowTypeName,
			   wft.publishingWorkFlowId,
			   sa1.Name AS publishingWorkFlowName,
			   sa1.XAML AS XAML,
			   sa1.ActivityLibraryId,
			   al.Name AS ActivityLibraryName,
			   al.Executable as DLL
		FROM WorkflowType wft
		LEFT JOIN Activity sa1 ON sa1.Id = wft.publishingWorkFlowId
		LEFT JOIN ActivityLibrary al ON sa1.ActivityLibraryId = al.Id
		WHERE @TempID = wft.id
		
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
		  IF @@TRANCOUNT <> 0
		  BEGIN
			 ROLLBACK TRAN
		  END
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
			RETURN @Error
		END
	END CATCH
   RETURN @rc

END
GO


