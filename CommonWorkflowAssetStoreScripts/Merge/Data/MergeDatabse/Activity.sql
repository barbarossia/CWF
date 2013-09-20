declare @ExecSQL nvarchar(max)
set @ExecSQL = REPLACE('

INSERT INTO [dbo].[Activity]
	([OriginalId]
	,[GUID]
	,[Name]
	,[Description]
	,[MetaTags]
	,[IconsId]
	,[IsSwitch]
	,[IsService]
	,[ActivityLibraryId]
	,[IsUxActivity]
	,[CategoryId]
	,[ToolBoxTab]
	,[IsToolBoxActivity]
	,[Version]
	,[StatusId]
	,[WorkflowTypeId]
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
	,[Environment])
SELECT A.[Id]
	,A.[GUID]
	,A.[Name]
	,A.[Description]
	,A.[MetaTags]
	,A.[IconsId]
	,A.[IsSwitch]
	,A.[IsService]
	,AL.[Id] AS [ActivityLibraryId]
	,A.[IsUxActivity]
	,AC.[Id] AS [CategoryId]
	,A.[ToolBoxTab]
	,A.[IsToolBoxActivity]
	,A.[Version]
	,A.[StatusId]
	,NULL AS [WorkflowTypeId]
	,A.[Locked]
	,A.[LockedBy]
	,A.[IsCodeBeside]
	,A.[XAML]
	,A.[DeveloperNotes]
	,A.[BaseType]
	,A.[Namespace]
	,A.[SoftDelete]
	,A.[InsertedByUserAlias]
	,A.[InsertedDateTime]
	,A.[UpdatedByUserAlias]
	,A.[UpdatedDateTime]
	,A.[Url]
	,A.[ShortName]
	,A.[Environment] 
	FROM [@DBName].[dbo].[Activity] A 
	join [@DBName].[dbo].[ActivityCategory] TAC on A.[CategoryId] = TAC.[Id]
	join [dbo].[ActivityCategory] AC on AC.[Name] = TAC.[Name]
	left join [dbo].[ActivityLibrary] AL on A.[ActivityLibraryId] = AL.[OriginalId]

', '@DBName', $(DBName))
Exec(@ExecSQL)
GO