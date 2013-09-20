declare @ExecSQL nvarchar(max)
set @ExecSQL = REPLACE('

INSERT INTO [dbo].[ActivityCategory]
	([GUID]
	,[Name]
	,[Description]
	,[MetaTags]
	,[AuthGroupID]
	,[SoftDelete]
	,[InsertedByUserAlias]
	,[InsertedDateTime]
	,[UpdatedByUserAlias]
	,[UpdatedDateTime])
SELECT A1.[GUID]
	,A1.[Name]
	,A1.[Description]
	,A1.[MetaTags]
	,AG.[Id]
	,A1.[SoftDelete]
	,A1.[InsertedByUserAlias]
	,A1.[InsertedDateTime]
	,A1.[UpdatedByUserAlias]
	,A1.[UpdatedDateTime] 
	FROM [CommonWorkflowAssetStoreOASP5].[dbo].[ActivityCategory] A1 
	LEFT JOIN [dbo].[ActivityCategory] A ON A.[GUID] = A1.[GUID] 
	join [CommonWorkflowAssetStoreOASP5].[dbo].[AuthorizationGroup] TAG on A1.[AuthGroupId] = TAG.[Id]
	join [dbo].[AuthorizationGroup] AG on TAG.[Name] = AG.[Name]
	WHERE A.Id IS NULL

INSERT INTO [dbo].[ActivityCategory]
	([GUID]
	,[Name]
	,[Description]
	,[MetaTags]
	,[AuthGroupID]
	,[SoftDelete]
	,[InsertedByUserAlias]
	,[InsertedDateTime]
	,[UpdatedByUserAlias]
	,[UpdatedDateTime])
SELECT NEWID()
	,A1.[Name]
	,A1.[Description]
	,A1.[MetaTags]
	,AG.[Id]
	,A1.[SoftDelete]
	,A1.[InsertedByUserAlias]
	,A1.[InsertedDateTime]
	,A1.[UpdatedByUserAlias]
	,A1.[UpdatedDateTime] 
	FROM [@DBName].[dbo].[ActivityCategory] A1 
	LEFT JOIN [dbo].[ActivityCategory] A ON A.Name = A1.Name
	join [@DBName].[dbo].[AuthorizationGroup] TAG on A1.[AuthGroupId] = TAG.[Id]
	join [dbo].[AuthorizationGroup] AG on TAG.[Name] = AG.[Name]
	WHERE A.Id IS NULL

', '@DBName', $(DBName))
Exec(@ExecSQL)
GO