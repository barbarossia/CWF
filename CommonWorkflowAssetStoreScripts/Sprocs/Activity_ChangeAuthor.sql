IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Activity_ChangeAuthor]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Activity_ChangeAuthor]
GO

CREATE PROCEDURE [dbo].[Activity_ChangeAuthor]		
        @InCaller nvarchar(50),
        @InCallerversion nvarchar (50),
        @InAuthGroupName	[dbo].[AuthGroupNameTableType] READONLY ,
        @InEnvironmentName	nvarchar(50) ,
        @InName nvarchar(255),
        @InVersion nvarchar(25),
	@InAuthorAlias nvarchar(50),
        @InOperatorUserAlias nvarchar(50),
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
    IF (@InAuthorAlias IS NULL OR @InName = '')
    BEGIN
        SET @outErrorString = 'Invalid Parameter Value (@InAuthorAlias)'
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

	SELECT @Id = sa.ID
	FROM [dbo].[Activity] sa
	join [dbo].[Environment] E on sa.[Environment] = E.[Id]
	WHERE sa.Name = @InName AND  sa.[Version] = @InVersion AND E.[Name]= @InEnvironmentName AND sa.SoftDelete= 0
	
	IF (@Id IS NULL)
	BEGIN
		SET @outErrorString = 'Invalid @Id attempting to perform a GET on table'
		RETURN 55040
	END

	DECLARE @currentCreateBy NVARCHAR(50)
	SELECT @currentCreateBy = InsertedByUserAlias
		FROM [dbo].[Activity]
		WHERE Id = @Id
	IF (@currentCreateBy = @InAuthorAlias)
		BEGIN
			SET @outErrorString = 'Create by user as same as' + @currentCreateBy
			RETURN 55066
		END	

	UPDATE [dbo].[Activity]
		SET InsertedByUserAlias = @InAuthorAlias,
			UpdatedDateTime = GETUTCDATE(),
			UpdatedByUserAlias = @InOperatorUserAlias
		WHERE Id = @Id
					
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