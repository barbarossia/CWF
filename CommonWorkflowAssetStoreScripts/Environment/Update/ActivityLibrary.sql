declare @ExecSQL nvarchar(max)
set @ExecSQL = REPLACE('

declare @EnvironmentId int
select @EnvironmentId = Id from [dbo].[Environment] where [Name] = ''@Evr''

UPDATE [dbo].[ActivityLibrary]
SET [Environment] = @EnvironmentId

', '@Evr', $(Evr))
Exec(@ExecSQL)
GO