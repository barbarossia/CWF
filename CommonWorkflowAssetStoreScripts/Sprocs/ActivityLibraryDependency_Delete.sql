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
// FileName: ActivityLibraryDependency_Delete.sql
// File description: Soft Deletes a row in the 
//                   mtblActivityLibraryDependenciesDelete table.
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   ActivityLibraryDependency_Delete                     *
**    Auth:   v-stska                                                      *
**    Date:   11/18/2010                                                   *
**                                                                         *
****************************************************************************
**   sproc logic flow: <Optional if complex> 
**   Parameter definition if complex
****************************************************************************
**                  CHANGE HISTORY
****************************************************************************
**	Date:        Author:             Description:
** ____________    ________________    ____________________________________
**  2/8/2010       v-stska             Original implementation
**  5/31/2011      v-stska             Add usage counter
**  11-Nov-2011    v-richt             Bug#86713 - Change error codes to positive numbers
** *************************************************************************/
CREATE PROCEDURE [dbo].[ActivityLibraryDependency_Delete]
        @InCaller nvarchar(50),
        @InCallerversion nvarchar (50),
        @inActivityLibraryName nvarchar (255),
        @inActivityLibraryVersionNumber nvarchar (50),
        @inActivityLibraryDependentName nvarchar (255),
        @inActivityLibraryDependentVersionNumber nvarchar (50),
        @outErrorString nvarchar (300)OUTPUT
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

    DECLARE @Id bigint
    DECLARE @SoftDelete bit
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
    IF (@inActivityLibraryName IS NULL OR @inActivityLibraryName = '')
    BEGIN
        SET @outErrorString = 'Invalid Parameter Value (@inActivityLibraryName)'
        RETURN 55128
    END
    IF (@inActivityLibraryVersionNumber IS NULL OR @inActivityLibraryVersionNumber = '')
    BEGIN
        SET @outErrorString = 'Invalid Parameter Value (@inActivityLibraryVersionNumber)'
        RETURN 55129
    END
    IF (@inActivityLibraryDependentName IS NULL OR @inActivityLibraryDependentName = '')
    BEGIN
        SET @outErrorString = 'Invalid Parameter Value (@inActivityLibraryDependentName)'
        RETURN 55130
    END
    IF (@inActivityLibraryDependentVersionNumber IS NULL OR @inActivityLibraryDependentVersionNumber = '')
    BEGIN
        SET @outErrorString = 'Invalid Parameter Value (@inActivityLibraryDependentVersionNumber)'
        RETURN 55131
    END
    
    DECLARE @ActivityLibraryId bigint
    SELECT @ActivityLibraryId = ID
    FROM [dbo].[ActivityLibrary]
    WHERE @inActivityLibraryName = name AND @inActivityLibraryVersionNumber = VersionNumber
    -- If found, change this from an insert to an update
    If (@ActivityLibraryId IS NULL)
    BEGIN
        SET @outErrorString = 'Invalid Parameter Value (@inActivityLibraryName/@inActivityLibraryVersionNumber)'
        RETURN 55132
    END
    
    DECLARE @ActivityLibraryDependentId bigint
    SELECT @ActivityLibraryDependentId = ID
    FROM [dbo].[ActivityLibrary]
    WHERE @inActivityLibraryDependentName = name AND @inActivityLibraryDependentVersionNumber = VersionNumber
    -- If found, change this from an insert to an update
    If (@ActivityLibraryDependentId IS NULL)
    BEGIN
        SET @outErrorString = 'Invalid Parameter Value (@inActivityLibraryDependentName/@inActivityLibraryDependentVersionNumber)'
        RETURN 55133
    END
    
    DECLARE @TEMPUsageCount bigint
    SELECT  @Id = Id, @SoftDelete = SoftDelete, @TEMPUsageCount = UsageCount
    FROM ActivityLibraryDependency ald
    WHERE ald.DependentActivityLibraryId = @ActivityLibraryDependentId AND 
            ald.ActivityLibraryID = @ActivityLibraryId
    
    -- Delete entry
    If (@TEMPUsageCount = 1)
    BEGIN
        DELETE ActivityLibraryDependency
        WHERE @Id = Id
    END
    ELSE
    BEGIN
        UPDATE ActivityLibraryDependency
        SET UsageCount = UsageCount - 1
        WHERE @Id = Id
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


