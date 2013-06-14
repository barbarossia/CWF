
/**************************************************************************
// Product:  CommonWF
// FileName: ActivityLibrary_GetWoExecutable.sql
// File description: Get the XAML based on Name & version.
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   ActivityLibrary_GetWoExecutable                            *
**    Desc:   Gets all ActivityLibraries without the Xaml.                 *
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
**  10/28/2010     v-stska             Original implementation
**  11/2/2010      v-stska             Add BEGIN TRY/CATCH error
**                                     handling for writing to etblErrorLog
**  2/13/2011      v-stska             Add OutErrorString
**  11-Nov-2011	   v-richt             Bug#86713 - Change error codes to positive numbers
** *************************************************************************/
CREATE PROCEDURE [dbo].[ActivityLibrary_GetWithoutExecutable]
        @inCaller nvarchar(50),
        @inCallerversion nvarchar (50),
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
            
        SELECT        
            al.Id,
            al.[GUID],
            al.Name, 
            al.AuthGroupId,
            ag.Name AS AuthGroupName, 
            al.Category,
            al.HasActivities, 
            al.[Description], 
            al.ImportedBy, 
            al.VersionNumber, 
            al.[Status], 
            al.MetaTags,
            al.[FriendlyName],
            al.[ReleaseNotes]
        FROM            
            ActivityLibrary al
            JOIN AuthorizationGroup ag ON al.AuthGroupId = ag.Id
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