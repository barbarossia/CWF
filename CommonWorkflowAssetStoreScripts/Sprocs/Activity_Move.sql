IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Activity_Move]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Activity_Move]
GO

create PROCEDURE [dbo].[Activity_Move]
		@inCaller			nvarchar(50),
		@inCallerversion	nvarchar (50),
        	@InAuthGroupName	[dbo].[AuthGroupNameTableType] READONLY ,
		@InName				nvarchar(255),
		@InVersion			nvarchar(25),
		@InEnvironment nvarchar(50) ,
		@InEnvironmentTarget nvarchar(50) ,
		@InWorkflowTypeID bigint,
        	@InOperatorUserAlias nvarchar(50),
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
    
DECLARE @EnvironmentID INT
    SELECT @EnvironmentID = ID 
    FROM Environment
    WHERE [Name] = @InEnvironmentTarget
    IF (@Environmentid IS NULL)
    BEGIN
        SET @outErrorString = 'Invalid Parameter Value (@InEnvironmentTarget)'
        RETURN 55104
    END

DECLARE @currentEnviornmentID INT
DECLARE @currentEnviornment nvarchar(50)
    SELECT @currentEnviornmentID = ID, @currentEnviornment = [Name] 
    FROM Environment
    WHERE [Name] = @InEnvironment
    IF (@currentEnviornmentID IS NULL)
    BEGIN
        SET @outErrorString = 'Invalid Parameter Value (@InEnvironment)'
        RETURN 55104
    END

	DECLARE @CheckID bigint
 	SELECT @CheckID = ID 
    FROM WorkflowType
    WHERE [Id] = @InWorkflowTypeID and Environment = @EnvironmentID
    IF (@CheckID IS NULL)
    BEGIN
        SET @outErrorString = 'Invalid Parameter Value (@InWorkflowTypeID)'
        RETURN 55105
    END

	DECLARE @Return_Value int
	DECLARE @InEnvironments [dbo].[EnvironmentTableType]
	INSERT @InEnvironments (Name) Values (@InEnvironmentTarget)
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
	WHERE sa.Name = @InName AND  sa.[Version] = @InVersion AND sa.[Environment] = @currentEnviornmentID AND sa.SoftDelete= 0
	
	IF (@Id IS NULL)
	BEGIN
		SET @outErrorString = 'Invalid @Id attempting to perform a GET on table'
		RETURN 55040
	END

    	EXEC @Return_Value = dbo.ValidateEnvironmentMove 
		@CurrentEnvironmentName = @currentEnviornment,
		@ToEnvironmentName = @InEnvironmentTarget,
		@OutErrorString =  @OutErrorString output
	IF (@Return_Value > 0)
	BEGIN		    
        RETURN @Return_Value
	END

	DECLARE @inUpdatedDateTime datetime
	SET @inUpdatedDateTime = GETutcDATE()
	UPDATE [dbo].[Activity]
		SET Environment = @EnvironmentID,
			WorkflowTypeId = @InWorkflowTypeID,
			UpdatedDateTime = @inUpdatedDateTime,
			UpdatedByUserAlias = @InOperatorUserAlias
		WHERE Id = @Id

	UPDATE [dbo].[ActivityLibrary]
		SET Environment = @EnvironmentID,
			UpdatedDateTime = @inUpdatedDateTime,
			UpdatedByUserAlias = @InOperatorUserAlias
		from [ActivityLibrary] al join Activity sa on al.Id = sa.ActivityLibraryid
		WHERE sa.Id = @Id
					
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