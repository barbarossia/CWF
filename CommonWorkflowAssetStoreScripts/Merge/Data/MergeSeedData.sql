declare @ExecSQL nvarchar(max)
set @ExecSQL = REPLACE('

DELETE Temp_ActivityLibrary
DELETE Temp_Activity
DELETE Temp_WorkflowType

DECLARE @Id bigint
DECLARE @originalId bigint
DECLARE @ActivityLibraryId bigint
DECLARE @DependentActivityLibraryId bigint
DECLARE @PublishingWorkflowId bigint
DECLARE @WorkflowTemplateId bigint
DECLARE @WorkflowTypeId bigint
DECLARE @Name nvarchar(255)
DECLARE @Version nvarchar(50)

-- WorkflowType

DECLARE WorkflowType_cursor CURSOR FOR 
SELECT Id, Name from [dbo].[Ori_WorkflowType] 
OPEN WorkflowType_cursor
FETCH NEXT
FROM WorkflowType_cursor INTO @originalId, @Name
WHILE @@FETCH_STATUS = 0
BEGIN
	SET @Id = NULL
	SELECT @Id = id FROM [dbo].[WorkflowType] WHERE [Name] = @Name AND [Environment] = @Environment
	IF @Id IS NULL
	BEGIN
		INSERT [dbo].[WorkflowType] ([GUID], [Name], [PublishingWorkflowId], [WorkflowTemplateId], [HANDleVariableId], [PageViewVariableId], [AuthGroupId], [SELECTionWorkflowId], [SoftDelete], [InsertedByUserAlias], [InsertedDateTime], [UpdatedByUserAlias], [UpdatedDateTime], [Environment]) 
		SELECT [GUID]
			  ,[Name]
			  ,NULL
			  ,NULL
			  ,[HandleVariableId]
			  ,[PageViewVariableId]
			  ,[AuthGroupId]
			  ,[SelectionWorkflowId]
			  ,[SoftDelete]
			  ,[InsertedByUserAlias]
			  ,[InsertedDateTime]
			  ,[UpdatedByUserAlias]
			  ,[UpdatedDateTime]
			  ,[Environment]
		  FROM [dbo].[Ori_WorkflowType]
		  WHERE [Name] = @Name AND [Environment] = @Environment
		SELECT @Id = SCOPE_IDENTITY(); 
		
		SELECT @PublishingWorkflowId = [PublishingWorkflowId], @WorkflowTemplateId = [WorkflowTemplateId]
		FROM [dbo].[Ori_WorkflowType]
		WHERE [Name] = @Name AND [Environment] = @Environment
	END
	ELSE
	BEGIN
		SET @PublishingWorkflowId = NULL
		SET @WorkflowTemplateId = NULL
	END
	INSERT INTO Temp_WorkflowType (Id, [OriginalId], PublishingWorkflowId, WorkflowTemplateId) VALUES (@id, @originalId, @PublishingWorkflowId, @WorkflowTemplateId)

	FETCH NEXT
	FROM WorkflowType_cursor INTO @originalId, @Name
END
CLOSE WorkflowType_cursor
DEALLOCATE WorkflowType_cursor

-- WorkflowType End


-- ActivityLibrary

DECLARE ActivityLibrary_cursor CURSOR FOR 
SELECT Id, Name, VersionNumber from [dbo].[Ori_ActivityLibrary] 
OPEN ActivityLibrary_cursor
FETCH NEXT
FROM ActivityLibrary_cursor INTO @originalId, @Name, @Version
WHILE @@FETCH_STATUS = 0
BEGIN
	SET @Id = NULL
	SELECT @Id = id FROM [dbo].[ActivityLibrary] WHERE [Name] = @Name AND [VersionNumber] = @Version AND [Environment] = @Environment
	IF @Id IS NULL
	BEGIN
		INSERT [dbo].[ActivityLibrary] ([GUID], [Name], [AuthGroupId], [Category], [Executable], [HasActivities], [Description], [ImportedBy], [VersionNumber], [Status], [MetaTags], [SoftDelete], [InsertedByUserAlias], [InsertedDateTime], [UpdatedByUserAlias], [UpdatedDateTime], [CategoryId], [FriendlyName], [ReleaseNotes], [Environment]) 
		SELECT [GUID]
		,[Name]
		,[AuthGroupId]
		,[Category]
		,[Executable]
		,[HasActivities]
		,[Description]
		,[ImportedBy]
		,[VersionNumber]
		,[Status]
		,[MetaTags]
		,[SoftDelete]
		,[InsertedByUserAlias]
		,[InsertedDateTime]
		,[UpdatedByUserAlias]
		,[UpdatedDateTime]
		,[CategoryId]
		,[FriendlyName]
		,[ReleaseNotes]
		,[Environment]
		FROM [dbo].[Ori_ActivityLibrary]
		WHERE [Name] = @Name AND [VersionNumber] = @Version AND [Environment] = @Environment
		SELECT @Id = SCOPE_IDENTITY(); 
	END
	INSERT INTO Temp_ActivityLibrary (Id, [OriginalId]) VALUES (@id, @originalId)

	FETCH NEXT
	FROM ActivityLibrary_cursor INTO @originalId, @Name, @Version
END
CLOSE ActivityLibrary_cursor
DEALLOCATE ActivityLibrary_cursor

-- ActivityLibrary End


-- ActivityLibraryDependency
	INSERT [dbo].[ActivityLibraryDependency] ([ActivityLibraryID], [DependentActivityLibraryId], [SoftDelete], [InsertedByUserAlias], [InsertedDateTime], [UpdatedByUserAlias], [UpdatedDateTime], [UsageCount]) 
	SELECT t1.[Id]
		  ,t2.[Id]
		  ,[SoftDelete]
		  ,[InsertedByUserAlias]
		  ,[InsertedDateTime]
		  ,[UpdatedByUserAlias]
		  ,[UpdatedDateTime]
		  ,[UsageCount]
	  FROM [dbo].[Ori_ActivityLibraryDependency] o 
	  JOIN Temp_ActivityLibrary t1 ON o.[ActivityLibraryID]= t1.[OriginalId]
	  JOIN Temp_ActivityLibrary t2 ON o.[DependentActivityLibraryId]= t2.[OriginalId]

-- ActivityLibraryDependency End

-- Activity

DECLARE Activity_cursor CURSOR FOR 
SELECT o.Id, o.[Name], o.[Version], ta.Id AS ActivityLibraryId, tw.Id AS WorkflowTypeId FROM [dbo].[Ori_Activity] o
JOIN Temp_ActivityLibrary ta on o.ActivityLibraryId = ta.[OriginalId]
JOIN Temp_WorkflowType tw on o.WorkflowTypeId = tw.[OriginalId]
OPEN Activity_cursor
FETCH NEXT
FROM Activity_cursor INTO @originalId, @Name, @Version, @ActivityLibraryId, @WorkflowTypeId   
WHILE @@FETCH_STATUS = 0
BEGIN
	SET @Id = NULL
	SELECT @Id = id FROM [dbo].[Activity] WHERE [Name] = @Name AND [Version] =@Version AND [Environment] = @Environment
	IF @Id IS NULL
	BEGIN
		INSERT [dbo].[Activity] ([GUID], [Name], [Description], [MetaTags], [IconsId], [IsSwitch], [IsService], [ActivityLibraryId], [IsUxActivity], [CategoryId], [ToolBoxTab], [IsToolBoxActivity], [Version], [StatusId], [WorkflowTypeId], [Locked], [LockedBy], [IsCodeBeside], [XAML], [DeveloperNotes], [BaSEType], [Namespace], [SoftDelete], [InsertedByUserAlias], [InsertedDateTime], [UpdatedByUserAlias], [UpdatedDateTime], [Url], [ShortName], [Environment]) 
		SELECT [GUID]
		,[Name]
		,[Description]
		,[MetaTags]
		,[IconsId]
		,[IsSwitch]
		,[IsService]
		,@ActivityLibraryId
		,[IsUxActivity]
		,[CategoryId]
		,[ToolBoxTab]
		,[IsToolBoxActivity]
		,[Version]
		,[StatusId]
		,@WorkflowTypeId
		,[Locked]
		,[LockedBy]
		,[IsCodeBeside]
		,[XAML]
		,[DeveloperNotes]
		,[BaseType]
		,[Namespace]
		,[SoftDelete]
		,[InsertedByUserAlias]
		,[InsertedDateTime]
		,[UpdatedByUserAlias]
		,[UpdatedDateTime]
		,[Url]
		,[ShortName]
		,[Environment]
		FROM [dbo].[Ori_Activity]
		WHERE [Name] = @Name AND [Version] =@Version AND [Environment] = @Environment
		SELECT @Id = SCOPE_IDENTITY(); 
	END
	INSERT INTO Temp_Activity (Id, [OriginalId]) VALUES (@id, @originalId)
	FETCH NEXT
	FROM Activity_cursor INTO @originalId, @Name, @Version, @ActivityLibraryId, @WorkflowTypeId   
END
CLOSE Activity_cursor
DEALLOCATE Activity_cursor

-- Activity End

-- WorkflowType Update

DECLARE Update_WorkflowType_cursor CURSOR FOR 
SELECT  w.Name, t.PublishingWorkflowId, t.WorkflowTemplateId FROM Temp_WorkflowType t
JOIN [dbo].[WorkflowType] w ON w.Id = t.Id
WHERE w.[Environment] = @Environment
OPEN Update_WorkflowType_cursor
FETCH NEXT
FROM Update_WorkflowType_cursor INTO @Name, @PublishingWorkflowId, @WorkflowTemplateId
WHILE @@FETCH_STATUS = 0
BEGIN
	IF @PublishingWorkflowId IS NOT NULL
	BEGIN
		SELECT @id = Id FROM Temp_Activity WHERE OriginalId = @PublishingWorkflowId
		UPDATE [dbo].[WorkflowType]
		SET PublishingWorkflowId = @id
		WHERE Name= @Name AND [Environment] = @Environment
	END
	IF @WorkflowTemplateId IS NOT NULL
	BEGIN
		SELECT @id = Id FROM Temp_Activity WHERE OriginalId = @WorkflowTemplateId
		UPDATE [dbo].[WorkflowType]
		SET WorkflowTemplateId = @id
		WHERE Name= @Name AND [Environment] = @Environment
	END
	FETCH NEXT
	FROM Update_WorkflowType_cursor INTO @Name, @PublishingWorkflowId, @WorkflowTemplateId
END
CLOSE Update_WorkflowType_cursor
DEALLOCATE Update_WorkflowType_cursor

-- WorkflowType Update End

', '@Environment', $(Env))
Exec(@ExecSQL)
