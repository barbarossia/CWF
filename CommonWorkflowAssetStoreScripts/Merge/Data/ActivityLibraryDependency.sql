declare @ExecSQL nvarchar(max)
set @ExecSQL = REPLACE('

INSERT INTO [dbo].[ActivityLibraryDependency]
	([ActivityLibraryID]
	,[DependentActivityLibraryId]
	,[SoftDelete]
	,[InsertedByUserAlias]
	,[InsertedDateTime]
	,[UpdatedByUserAlias]
	,[UpdatedDateTime]
	,[UsageCount])
SELECT AL1.[Id]
	,AL2.[Id]
	,A.[SoftDelete]
	,A.[InsertedByUserAlias]
	,A.[InsertedDateTime]
	,A.[UpdatedByUserAlias]
	,A.[UpdatedDateTime]
	,A.[UsageCount]
	FROM [@DBName].[dbo].[ActivityLibraryDependency] A
	join [dbo].[ActivityLibrary] AL1 on A.[ActivityLibraryID] = AL1.[OriginalId]
	join [dbo].[ActivityLibrary] AL2 on A.[DependentActivityLibraryId] = AL2.[OriginalId]

', '@DBName', $(DBName))
Exec(@ExecSQL)
GO