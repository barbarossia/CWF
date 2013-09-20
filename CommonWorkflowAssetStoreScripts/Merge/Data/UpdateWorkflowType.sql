declare @ExecSQL nvarchar(max)
set @ExecSQL = REPLACE(REPLACE('

declare @EnvironmentId int
select @EnvironmentId = Id from [dbo].[Environment] where [Name] = ''@Evr''

update Activity
set WorkflowTypeId = W.Id
FROM [dbo].[Activity] A 
join [@DBName].[dbo].[Activity] TA on A.[OriginalId] = TA.[Id] and A.Environment = @EnvironmentId
join [dbo].[WorkflowType] W on TA.[WorkflowTypeId] = W.[OriginalId] and W.Environment = @EnvironmentId

', '@Evr', $(Evr)),'@DBName', $(DBName))
Exec(@ExecSQL)
GO
