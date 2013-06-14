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
// FileName: ToolBoxTabName_Delete.sql
// File description: Soft Deletes a row in the ltblToolBoxTabName table.
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   ToolBoxTabName_Delete                                  *
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
**  11/18/2010     v-stska             Original implementation
**  12/12/2010     v-stska             Update to NEW3PrototypeAsset DB
**  12/22/2010     v-stska             Add @InName parameter
**  2/13/2011      v-stska             Add @OutError logic
**  11-Nov-2011    v-richt             Bug#86713 - Change error codes to positive numbers
** *************************************************************************/
CREATE PROCEDURE [dbo].[ToolBoxTabName_Delete]
        @InCaller nvarchar(50),
        @InCallerversion nvarchar (50),
        @InId bigint,
        @InName nvarchar (50),
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
    DECLARE @Name nvarchar (50)
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
    IF (@InName IS NULL OR @InName = '') AND (@InId IS NULL OR @InId = 0)
    BEGIN
        SET @outErrorString = 'Invalid Parameter Value (@InId, and @InName" cannot all be null)'
        RETURN 55158
    END
    IF (@InId < 0)
    BEGIN
        SET @outErrorString = 'Invalid Parameter Value (@InId)'
        RETURN 55123
    END
    
    -- Check the @InId 
    If (@InId > 0)
    BEGIN
        SELECT  @Id = Id, @SoftDelete = SoftDelete, @Name = Name
        FROM [ToolBoxTabName] tbtn
        WHERE tbtn.Id = @InId
    END
    ELSE
    If (@InName IS NOT NULL OR @InName <> '')
    BEGIN
        SELECT  @Id = Id, @SoftDelete = SoftDelete, @Name = Name
        FROM [ToolBoxTabName] tbtn
        WHERE tbtn.Name = @InName
    END
    
    IF @Id > 0
    BEGIN

        UPDATE [ToolBoxTabName]
        SET SoftDelete = 1
        WHERE @Id =Id

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
GO