declare @ExecSQL nvarchar(max)
set @ExecSQL = REPLACE(REPLACE('

declare @EnvironmentId int
select @EnvironmentId = Id from [dbo].[Environment] where [Name] = ''@Evr''

INSERT INTO [dbo].[WorkflowType]
           ([OriginalId]
           ,[GUID]
           ,[Name]
           ,[PublishingWorkflowId]
           ,[WorkflowTemplateId]
           ,[HandleVariableId]
           ,[PageViewVariableId]
           ,[AuthGroupId]
           ,[SelectionWorkflowId]
           ,[SoftDelete]
           ,[InsertedByUserAlias]
           ,[InsertedDateTime]
           ,[UpdatedByUserAlias]
           ,[UpdatedDateTime]
           ,[Environment])
SELECT W.[Id]
       ,W.[GUID]
       ,W.[Name]
       ,A1.[Id] AS [PublishingWorkflowId]
       ,A2.[Id] AS [WorkflowTemplateId]
       ,W.[HandleVariableId]
       ,W.[PageViewVariableId]
       ,AG.[Id] AS [AuthGroupId]
       ,A3.[Id] AS [SelectionWorkflowId]
       ,W.[SoftDelete]
       ,W.[InsertedByUserAlias]
       ,W.[InsertedDateTime]
       ,W.[UpdatedByUserAlias]
       ,W.[UpdatedDateTime]
	,@EnvironmentId  as [Environment] 
	FROM [@DBName].[dbo].[WorkflowType] W 
	join [@DBName].[dbo].[AuthorizationGroup] TAG on W.[AuthGroupId] = TAG.[Id]
	join [dbo].[AuthorizationGroup] AG on TAG.[Name] = AG.[Name]
	left join [dbo].[Activity] A1 on W.[PublishingWorkflowId] = A1.[OriginalId] and A1.[Environment] = @EnvironmentId
	left join [dbo].[Activity] A2 on W.[WorkflowTemplateId] = A2.[OriginalId] and A2.[Environment] = @EnvironmentId
	left join [dbo].[Activity] A3 on W.[SelectionWorkflowId] = A3.[OriginalId] and A3.[Environment] = @EnvironmentId

', '@Evr', $(Evr)),'@DBName', $(DBName))
Exec(@ExecSQL)
GO