declare @ExecSQL nvarchar(max)
set @ExecSQL = REPLACE(REPLACE('

declare @EnvironmentId int
select @EnvironmentId = Id from [dbo].[Environment] where [Name] = ''@Evr''

INSERT INTO [dbo].[ActivityLibrary]
	([OriginalId]
	,[GUID]
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
	,[Environment])
SELECT A.[Id]
	,A.[GUID]
	,A.[Name]
	,AG.[Id] AS [AuthGroupId]
	,A.[Category]
	,A.[Executable]
	,A.[HasActivities]
	,A.[Description]
	,A.[ImportedBy]
	,A.[VersionNumber]
	,A.[Status]
	,A.[MetaTags]
	,A.[SoftDelete]
	,A.[InsertedByUserAlias]
	,A.[InsertedDateTime]
	,A.[UpdatedByUserAlias]
	,A.[UpdatedDateTime]
	,AC.[Id] AS [CategoryId]
	,A.[FriendlyName]
	,A.[ReleaseNotes]
	,@EnvironmentId  as [Environment] 
	FROM [@DBName].[dbo].[ActivityLibrary] A
	left join [@DBName].[dbo].[ActivityCategory] TAC on A.[CategoryId] = TAC.[Id]
	join [@DBName].[dbo].[AuthorizationGroup] TAG on A.[AuthGroupId] = TAG.[Id]
	left join [dbo].[ActivityCategory] AC on TAC.[Name] = AC.[Name]
	join [dbo].[AuthorizationGroup] AG on TAG.[Name] = AG.[Name]

', '@Evr', $(Evr)),'@DBName', $(DBName))
Exec(@ExecSQL)
GO