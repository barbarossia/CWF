declare @ExecSQL nvarchar(max)
set @ExecSQL = REPLACE('

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[@DBName].[dbo].[TaskActivity]'') AND type in (N''U''))
BEGIN
INSERT INTO [dbo].[TaskActivity]
       ([GUID]
       ,[ActivityId]
       ,[AssignedTo]
       ,[Status])
SELECT T.[GUID]
	,A.[Id] AS [ActivityId]
	,T.[AssignedTo]
	,T.[Status]
	FROM [@DBName].[dbo].[TaskActivity] T
	join [dbo].[Activity] A on T.[ActivityId] = A.[OriginalId]
END
', '@DBName', $(DBName))
Exec(@ExecSQL)
GO