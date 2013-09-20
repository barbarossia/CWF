declare @ExecSQL nvarchar(max)
set @ExecSQL = REPLACE('

INSERT INTO [dbo].[ActivityLibraryDependencyHead]
       ([TreeHead]
       ,[IntersectingTableLinkID]
       ,[SoftDelete]
       ,[InsertedByUserAlias]
       ,[InsertedDateTime]
       ,[UpdatedByUserAlias]
       ,[UpdatedDateTime])
SELECT AL.[Id]
	,A.[IntersectingTableLinkID]
	,A.[SoftDelete]
	,A.[InsertedByUserAlias]
	,A.[InsertedDateTime]
	,A.[UpdatedByUserAlias]
	,A.[UpdatedDateTime]
	FROM [@DBName].[dbo].[ActivityLibraryDependencyHead] A
	join [dbo].[ActivityLibrary] AL on A.[TreeHead] = AL.[OriginalId]
', '@DBName', $(DBName))
Exec(@ExecSQL)
GO