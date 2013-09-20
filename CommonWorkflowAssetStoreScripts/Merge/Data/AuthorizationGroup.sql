declare @ExecSQL nvarchar(max)
set @ExecSQL = REPLACE('

INSERT INTO [dbo].[AuthorizationGroup]
       ([Guid]
       ,[Name]
       ,[AuthoringToolLevel]
       ,[SoftDelete]
       ,[InsertedByUserAlias]
       ,[InsertedDateTime]
       ,[UpdatedByUserAlias]
       ,[UpdatedDateTime])
SELECT A1.[Guid]
       ,A1.[Name]
       ,A1.[AuthoringToolLevel]
       ,A1.[SoftDelete]
       ,A1.[InsertedByUserAlias]
       ,A1.[InsertedDateTime]
       ,A1.[UpdatedByUserAlias]
       ,A1.[UpdatedDateTime]
	FROM [@DBName].[dbo].[AuthorizationGroup] A1 LEFT JOIN [dbo].[AuthorizationGroup] A 
	ON A.[Guid] = A1.[Guid]
	WHERE A.Id IS NULL

INSERT INTO [dbo].[AuthorizationGroup]
       ([Guid]
       ,[Name]
       ,[AuthoringToolLevel]
       ,[SoftDelete]
       ,[InsertedByUserAlias]
       ,[InsertedDateTime]
       ,[UpdatedByUserAlias]
       ,[UpdatedDateTime])
SELECT NEWID()
       ,A1.[Name]
       ,A1.[AuthoringToolLevel]
       ,A1.[SoftDelete]
       ,A1.[InsertedByUserAlias]
       ,A1.[InsertedDateTime]
       ,A1.[UpdatedByUserAlias]
       ,A1.[UpdatedDateTime]
	FROM [@DBName].[dbo].[AuthorizationGroup] A1 LEFT JOIN [dbo].[AuthorizationGroup] A 
	ON A.[Name] = A1.[Name]
	WHERE A.Id IS NULL

', '@DBName', $(DBName))
Exec(@ExecSQL)
GO