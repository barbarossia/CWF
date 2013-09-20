IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Activity_Copy]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Activity_Copy]
GO

create PROCEDURE [dbo].[Activity_Copy]
		@inCaller		nvarchar(50),
		@inCallerversion	nvarchar (50),
        	@InAuthGroupName	[dbo].[AuthGroupNameTableType] READONLY ,
		@InName				nvarchar(255),
		@InVersion			nvarchar(25),
		@InNewVersion			nvarchar(25),
		@InEnvironment nvarchar(50) ,
		@InEnvironmentTarget nvarchar(50) ,
		@InWorkflowTypeID bigint,
		@InInsertedByUserAlias nvarchar(50),
        	@InUpdatedByUserAlias nvarchar(50),
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
    IF (@InNewVersion IS NULL OR @InNewVersion = '')
    BEGIN
        SET @outErrorString = 'Invalid Parameter Value (@InNewVersion)'
        RETURN 55125
    END
    IF (@InEnvironmentTarget IS NULL OR @InEnvironmentTarget = '')
    BEGIN
        SET @outErrorString = 'Invalid Parameter Value (@InEnvironmentTarget)'
        RETURN 55126
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

	DECLARE @EnvironmentID INT
	SELECT @EnvironmentID = ID 
	FROM Environment
	WHERE [Name] = @InEnvironmentTarget
	IF (@Environmentid IS NULL)
	BEGIN
		SET @outErrorString = 'Invalid Parameter Value (@InEnvironmentTarget)'
		RETURN 55104
	END

	DECLARE @currentEnviornment INT
	SELECT @currentEnviornment = ID 
	FROM Environment
	WHERE [Name] = @InEnvironment
	IF (@currentEnviornment  IS NULL)
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

	DECLARE @Id bigint
	DECLARE @ActivityLibraryID bigint

	SELECT @Id = sa.ID, @ActivityLibraryID = ActivityLibraryID
	FROM [dbo].[Activity] sa
	WHERE sa.Name = @InName AND  sa.[Version] = @InVersion AND sa.[Environment] = @currentEnviornment AND sa.SoftDelete= 0
	
	IF (@Id IS NULL)
	BEGIN
		SET @outErrorString = 'Invalid @Id attempting to perform a GET on table'
		RETURN 55040
	END

	IF (@currentEnviornment = @EnvironmentID)
	BEGIN
		SET @outErrorString = 'Invalid Parameter Value (@InEnvironmentTarget)'
		RETURN 55041
	END

	SELECT @CheckID = ID 
	FROM ActivityLibrary
	WHERE [Id] = @ActivityLibraryID and Environment = @currentEnviornment
	IF (@CheckID IS NULL)
	BEGIN
		SET @outErrorString = 'Invalid Parameter Value (@ActivityLibraryID)'
		RETURN 55042
	END
    	
	DECLARE @InInsertedDateTime datetime
	DECLARE @InUpdatedDateTime datetime
	DECLARE @NewActivityLibraryID bigint
	DECLARE @NewActivityID bigint
	SET @InInsertedDateTime = GETutcDATE()
	SET @InUpdatedDateTime = GETutcDATE()

	INSERT INTO ActivityLibrary
                ([GUID], Name, AuthGroupId, Category, CategoryId, [Executable], HasActivities, [Description], ImportedBy, VersionNumber, [Status], MetaTags, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime, FriendlyName,ReleaseNotes, Environment)
	select NEWID(), @InName, AuthGroupId, Category, CategoryId, [Executable], HasActivities, [Description], ImportedBy, @InNewVersion, [Status], MetaTags, 0, @InInsertedByUserAlias, @InInsertedDateTime, @InUpdatedByUserAlias, @InUpdatedDateTime,FriendlyName,ReleaseNotes, @EnvironmentID from ActivityLibrary
	where Id = @ActivityLibraryID
	SELECT @NewActivityLibraryID = SCOPE_IDENTITY(); 

	INSERT INTO Activity
            ([GUID], Name, ShortName, [Description], MetaTags, IconsId, IsSwitch, IsService, ActivityLibraryId, 
            IsUxActivity, CategoryId, ToolboxTab, IsToolBoxActivity, [Version], StatusId, 
            WorkflowTypeId, Locked, LockedBy, IsCodeBeside, XAML, DeveloperNotes, BaseType, [Namespace], SoftDelete,
                InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime, Environment)
        select NEWID(), @InName, ShortName, [Description], MetaTags, IconsId, IsSwitch, IsService, @NewActivityLibraryID, 
                    IsUxActivity, CategoryId, ToolboxTab, IsToolBoxActivity, @InNewVersion, StatusId, 
                    @InWorkflowTypeID, Locked, LockedBy, IsCodeBeside, XAML, DeveloperNotes, BaseType, [Namespace], 0, 
                    @InInsertedByUserAlias, @InInsertedDateTime, @InUpdatedByUserAlias, @InUpdatedDateTime, @EnvironmentID
	from Activity
	where Id = @Id
	SELECT @NewActivityID = SCOPE_IDENTITY(); 

	INSERT INTO ActivityLibraryDependencyHead
		(TreeHead, IntersectingTableLinkID, SoftDelete, InsertedByUserAlias, 
		InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
	VALUES(@NewActivityLibraryID, null, 0, @InInsertedByUserAlias, 
		@InInsertedDateTime, @InUpdatedByUserAlias, @InUpdatedDateTime)

	INSERT INTO [ActivityLibraryDependency]
		(ActivityLibraryID, DependentActivityLibraryId, SoftDelete, InsertedByUserAlias, 
		InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime, UsageCount)
	select @NewActivityLibraryID, DependentActivityLibraryId, 0, @InInsertedByUserAlias, 
		@InInsertedDateTime, @InUpdatedByUserAlias, @InUpdatedDateTime, UsageCount
	from [ActivityLibraryDependency]
	where ActivityLibraryID = @ActivityLibraryID

				SELECT sa.Id, 
					sa.[GUID], 
					sa.Name,
					sa.ShortName, 
					sa.[Description], 
					sa.MetaTags, 
					sa.IconsId, 
					ic.[Name] AS iconsName,
					sa.IsSwitch, 
					sa.IsService, 
					sa.ActivityLibraryId, 
					al.Name AS ActivityLibraryName,
					al.VersionNumber AS ActivityLibraryVersion,
					al.AuthGroupID,
					ag.Name AS AuthGroupName, 
					sa.IsUxActivity, 
					sa.CategoryId, 
					ac.Name as ActivityCategoryName, 
					tbtn.Name as ToolBoxtabName, 
					sa.ToolBoxtab, 
					sa.IsToolBoxActivity, 
					sa.[Version], 
					sa.StatusId, 
					sc.Name AS StatusCodeName, 
					sa.WorkflowTypeId,
					wft.Name as WorkFlowTypeName, 
					sa.Locked, 
					sa.LockedBy, 
					sa.IsCodeBeside, 
					sa.XAML, 
					sa.DeveloperNotes, 
					sa.BaseType, 
					sa.[Namespace],
					sa.InsertedByUserAlias,
					sa.InsertedDateTime,
					sa.UpdatedByUserAlias,
					sa.UpdatedDateTime,
					E.[Name] AS Environment 
			FROM [dbo].[Activity] sa
			LEFT JOIN ActivityLibrary al ON sa.ActivityLibraryId = al.Id
			JOIN ActivityCategory ac ON sa.CategoryId = ac.Id
			LEFT JOIN ToolBoxTabName tbtn ON sa.ToolBoxTab = tbtn.Id
			JOIN StatusCode sc ON sa.StatusId = sc.Code
			JOIN WorkflowType wft ON sa.WorkflowTypeId = wft.Id
			LEFT JOIN Icon ic ON sa.IconsId = ic.Id 
			LEFT JOIN AuthorizationGroup ag ON ag.Id = al.AuthGroupId
			JOIN Environment E ON sa.Environment = E.Id
			WHERE sa.Id = @NewActivityID
				AND sa.SoftDelete = 0
					
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