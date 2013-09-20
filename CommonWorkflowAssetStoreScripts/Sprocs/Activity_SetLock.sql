/****** Object:  StoredProcedure [dbo].[Activity_SetLock]    Script Date: 05/17/2013 01:08:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Activity_SetLock]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Activity_SetLock]
GO

/**************************************************************************
// Product:  CommonWF
// FileName:ps_etblStoreActivities_UpdateLock.sql
// File description: Checks for the existance of a library.
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   ps_etblStoreActivities_UpdateLock                            *
**    Desc:   Update the activity locked and lockedby feild.                                  *
**    Auth:   v-bobzh                                                      *
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
**  11/May/2012      v-bobzh             Original implementation
**  4/June/2012      v-ery               Add @InLockedTime parameter
** 11/June/2012      v-ery               Support unlocking
** *************************************************************************/
CREATE PROCEDURE [dbo].[Activity_SetLock]	
        @InCaller nvarchar(50),
        @InCallerversion nvarchar (50),	
        @InName nvarchar(255),
        @InVersion nvarchar(25),
        @InEnvironmentName	nvarchar(50) ,
        @InOperatorUserAlias nvarchar(50),
        @InLocked bit,
        @InLockedTime datetime2,
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
    DECLARE @Id bigint
    		
	SELECT @Id = sa.ID
	FROM [dbo].[Activity] sa
	join [dbo].[Environment] E on sa.Environment = E.Id
	WHERE sa.Name = @InName AND  sa.[Version] = @InVersion AND E.Name = @InEnvironmentName AND sa.SoftDelete= 0
	
	IF (@Id IS NULL)
	BEGIN
		SET @outErrorString = 'Invalid @Id attempting to perform a GET on table'
		RETURN 55040
	END
	ELSE
	BEGIN
		DECLARE @currentLocked BIT
		DECLARE @currentLockedBy NVARCHAR(50)

		SELECT @currentLocked = Locked, @currentLockedBy = LockedBy
		FROM [dbo].[Activity]
		WHERE Id = @Id

        -- users can overwrite the lock,
        -- but they cannot unlock an item locked by someone else
		IF (@InLocked = 0 AND @currentLocked = 1 AND @currentLockedBy <> @InOperatorUserAlias)
		BEGIN
			SET @outErrorString = 'Lock has been changed by ' + @currentLockedBy
			RETURN 55066
		END	
		
		IF @InLocked = 1
		BEGIN
			UPDATE [dbo].[Activity]
			SET Locked = @InLocked,
				LockedBy = @InOperatorUserAlias,
				UpdatedDateTime = @InLockedTime,
				UpdatedByUserAlias = @InOperatorUserAlias
			WHERE Id = @Id
		END
		ELSE
		BEGIN
			UPDATE [dbo].[Activity]
			SET Locked = @InLocked,
				LockedBy = NULL
			WHERE Id = @Id		
		END
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



GO


