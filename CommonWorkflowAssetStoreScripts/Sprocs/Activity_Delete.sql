/****** Object:  StoredProcedure [dbo].[Activity_Delete]    Script Date: 05/16/2013 01:44:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Activity_Delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Activity_Delete]
GO

 /**************************************************************************
// Product:  CommonWF
// FileName: Activity_Delete.sql
// File description: Soft Deletes a row in the etblStatusCode table.
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   Activity_Delete                                *
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
**  11-Nov-2011	   v-richt             Bug#86713 - Change error codes to positive numbers
** *************************************************************************/
CREATE PROCEDURE [dbo].[Activity_Delete]
        @InCaller nvarchar(50),
        @InCallerversion nvarchar (50),
        @InAuthGroupName	[dbo].[AuthGroupNameTableType] READONLY ,
        @InEnvironmentName	nvarchar(50) ,
        @InName nvarchar (255),
        @InVersion nvarchar (50),
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
    
    SET @outErrorString = ''
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
    IF (@InName IS NULL OR @InName = '') AND (@InVersion IS NULL OR @InVersion = '')
    BEGIN
        SET @outErrorString = 'Invalid Parameter Value (@InName/@inVersion)'
        RETURN 55125
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

	declare @ActivityLibraryID bigint
	DECLARE @NewVersion NVARCHAR(50)
	DECLARE @MaxVersion NVARCHAR(50)
	DECLARE @NewName nvarchar (255)
	SET @NewVersion = '0.0.0.0'
	SET @NewName = LEFT(@InName + '-' + Replace(convert(varchar(50), NEWID()), '-', ''), 255)

	SELECT  @Id = sa.Id, @ActivityLibraryID = sa.ActivityLibraryId
	FROM [dbo].[Activity] sa
	JOIN [dbo].[Environment] E ON sa.Environment = E.Id
	WHERE sa.Name = @InName AND
		sa.Version = @InVersion AND
		E.Name = @InEnvironmentName

	SELECT @MaxVersion = Version FROM [dbo].[Activity] 
	WHERE name = @InName AND SoftDelete = 1
    	GROUP BY Version 
	
	IF (@MaxVersion IS NOT NULL)
	BEGIN
		DECLARE @Max INT
		DECLARE @Min INT
		DECLARE @build INT
		DECLARE @revision INT
		
		SELECT @revision = CAST(PARSENAME(@MaxVersion, 1) AS INT)
		SELECT @build = CAST(PARSENAME(@MaxVersion, 2) AS INT)
		SELECT @Min = CAST(PARSENAME(@MaxVersion, 3) AS INT)
		SELECT @Max = CAST(PARSENAME(@MaxVersion, 4) AS INT)
		SET @revision = @revision + 1
		SELECT @NewVersion = CAST(@Max AS NVARCHAR(50)) + '.' + 
					CAST(@Min AS NVARCHAR(50)) + '.' + 
					CAST(@build AS NVARCHAR(50)) + '.' + 
					CAST(@revision AS NVARCHAR(50))
	END	
		
	BEGIN TRAN
		UPDATE [dbo].[Activity]
		SET SoftDelete = 1, [Version] = @NewVersion, [Name] = @NewName, [ShortName] = LEFT(@NewName, 50)
		WHERE Id = @Id

		UPDATE [dbo].[ActivityLibrary]
		SET SoftDelete = 1, [VersionNumber] = @NewVersion, [Name] = @NewName
		WHERE Id = @ActivityLibraryID
	COMMIT TRAN

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

GRANT EXECUTE ON [dbo].[Activity_Delete] TO [MarketplaceService];
GO
