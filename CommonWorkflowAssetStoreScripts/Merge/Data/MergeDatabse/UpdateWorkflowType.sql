declare @ExecSQL nvarchar(max)
set @ExecSQL = REPLACE('

update Activity
set WorkflowTypeId = W.Id
FROM [dbo].[Activity] A 
join [@DBName].[dbo].[Activity] TA on A.[OriginalId] = TA.[Id]
join [dbo].[WorkflowType] W on TA.[WorkflowTypeId] = W.[OriginalId]

', '@DBName', $(DBName))
Exec(@ExecSQL)
GO
