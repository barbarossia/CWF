/****** Object:  StoredProcedure [dbo].[TaskActivity_UpdateStatus]    Script Date: 05/16/2013 01:49:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TaskActivity_UpdateStatus]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[TaskActivity_UpdateStatus]
GO


/**************************************************************************
// Product:  CommonWF
// FileName:ps_etblTaskActivity_UpdateStatus.sql
// File description: Checks for the existance of a library.
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   ps_etblTaskActivity_UpdateStatus                          *
**    Desc:   Update the TaskActivity Status.                                  *
**    Auth:   v-kason                                                      *
**    Date:   5/11/2012                                                   *
**                                                                         *
****************************************************************************
**   sproc logic flow: <Optional if complex> 
**   Parameter definition if complex
****************************************************************************
**                  CHANGE HISTORY
****************************************************************************
**	Date:        Author:             Description:
** ____________    ________________    ____________________________________
**  26/March/2013     v-kason            Original implementation
** *************************************************************************/
CREATE PROCEDURE [dbo].[TaskActivity_UpdateStatus]		
        @InCaller nvarchar(50),
        @InCallerversion nvarchar (50),
        @InAuthGroupName	[dbo].[AuthGroupNameTableType] READONLY ,
        @InEnvironmentName	nvarchar(50) ,
        @InTaskActivityGUID nvarchar(50),
        @InId bigint,
        @InStatus nvarchar(50),
        @outErrorString nvarchar (300) OUTPUT
AS
BEGIN TRY
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
            
    SET NOCOUNT ON

    SET @outErrorString = ''
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
	
	IF (@InStatus IS NULL OR @InStatus = '')			
    BEGIN
        SET @outErrorString = 'Invalid Parameter Value (@InStatus)'
        RETURN 55101
    END	
    
  	IF (@InEnvironmentName IS NULL OR @InEnvironmentName = '')
    BEGIN
        SET @outErrorString = 'Invalid Parameter Value (@InEnvironmentName)'
        RETURN 55126
    END

DECLARE @Return_Value int
DECLARE @InEnvironments [dbo].[EnvironmentTableType]
INSERT @InEnvironments (Name) Values (@InEnvironmentName)
EXEC @Return_Value = dbo.ValidateSPPermission 
	@InSPName = @cObjectName,
	@InAuthGroupName = @InAuthGroupName,
	@InEnvironments = @InEnvironments,
	@OutErrorString =  @OutErrorString output
IF (@Return_Value > 0)
BEGIN		    
	RETURN @Return_Value
END


	DECLARE @id bigint
	DECLARE @activityId bigint
    DECLARE @softDelete bit
	IF (@InId > 0)
	BEGIN
		SELECT @id = Id,@activityId = ActivityId
		FROM [dbo].[TaskActivity]
		WHERE Id = @InId

		SELECT @softDelete = SoftDelete
		FROM [dbo].[Activity]
		WHERE Id = @activityId

		IF ( @id IS NULL OR @activityId IS NULL)
			BEGIN
				SET @outErrorString = 'Invalid @InId attempting to perform a GET on table'
				RETURN 55040
			END

		IF (@softDelete = 1)
			BEGIN
				SET @outErrorString = 'Invalid @InId attempting to perform a GET on table that is marked soft delete'
				RETURN 55041
			END

	END
	ELSE IF(@InTaskActivityGUID <> '00000000-0000-0000-0000-000000000000')
	BEGIN
			SELECT @id = Id,@activityId = Id
			FROM [dbo].[TaskActivity]
			WHERE [Guid] = @InTaskActivityGUID
			AND Id IN (SELECT MAX(Id) AS Id From TaskActivity GROUP BY [Guid])

			SELECT @softDelete = SoftDelete
			FROM [dbo].[Activity]
			WHERE Id = @activityId

			IF (@id = 0 OR @id Is NULL)
			BEGIN
				SET @outErrorString = 'Invalid @InTaskActivityGUID attempting to perform a GET on table'
				RETURN 55042
			END
			IF (@softDelete = 1)
			BEGIN
				SET @outErrorString = 'Invalid @InTaskActivityGUID attempting to perform a GET on table that is marked soft delete'
				RETURN 55043
			END
	END
	IF (@id IS NULL OR @id = 0)
	BEGIN
		SET @outErrorString = 'Invalid @id attempting to set status on table'
		RETURN 55045
	END

	UPDATE [dbo].[TaskActivity]
	SET [Status] = @InStatus
	WHERE Id = @id
				
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
          --IF @@TRANCOUNT <> 0
          --BEGIN
             --ROLLBACK TRAN
          --END
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

