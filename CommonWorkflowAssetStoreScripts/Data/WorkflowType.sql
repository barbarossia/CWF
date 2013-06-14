DECLARE @Guid uniqueidentifier
DECLARE @Name nvarchar(30)
DECLARE @PublishingWorkFlowId bigint
DECLARE @WorkflowTemplateId bigint
DECLARE @HandleVariableId bigint
DECLARE @PageViewVariableId bigint
DECLARE @AuthgroupId bigint
DECLARE @SelectionWorkflowId bigint

DECLARE @InsertedByUserAlias  [nvarchar](50)
DECLARE @InsertedDateTime [datetime] 
DECLARE @UpdatedByUserAlias [nvarchar](50) 
DECLARE @UpdatedDateTime [datetime] 

SET @InsertedByUserAlias = 'setup'
SET @InsertedDateTime = GETDATE()
SET @UpdatedByUserAlias = 'setup'
SET @UpdatedDateTime = GETDATE()

SET @Guid = '0EEC11A1-DBD0-4DC7-8F7E-041E7BCB543A'
SET @Name = 'OAS Page'
SET @AuthgroupId = 2
INSERT INTO [dbo].[WorkflowType]
(GUID, Name, PublishingWorkflowId, WorkflowTemplateId, HandleVariableId, PageViewVariableId, AuthGroupId, SelectionWorkflowId, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES (@Guid, @Name, @PublishingWorkFlowId, @WorkflowTemplateId, @HandleVariableId, @PageViewVariableId, @AuthgroupId, @SelectionWorkflowId, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)

SET @Guid = '000DCFEB-7266-4D64-B0A6-2717B48403D5'
SET @Name = 'Template'
SET @AuthgroupId = 2
INSERT INTO [dbo].[WorkflowType]
(GUID, Name, PublishingWorkflowId, WorkflowTemplateId, HandleVariableId, PageViewVariableId, AuthGroupId, SelectionWorkflowId, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES (@Guid, @Name, @PublishingWorkFlowId, @WorkflowTemplateId, @HandleVariableId, @PageViewVariableId, @AuthgroupId, @SelectionWorkflowId, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)

SET @Guid = 'EA61F5E7-016C-4F39-AC31-D4836EFCF781'
SET @Name = 'Custom Activity'
SET @AuthgroupId = 2
INSERT INTO [dbo].[WorkflowType]
(GUID, Name, PublishingWorkflowId, WorkflowTemplateId, HandleVariableId, PageViewVariableId, AuthGroupId, SelectionWorkflowId, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES (@Guid, @Name, @PublishingWorkFlowId, @WorkflowTemplateId, @HandleVariableId, @PageViewVariableId, @AuthgroupId, @SelectionWorkflowId, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)

SET @Guid = '4C1931FD-D8E5-4B57-A6C0-1A792CD80226'
SET @Name = 'Publishing Workflow'
SET @AuthgroupId = 1
INSERT INTO [dbo].[WorkflowType]
(GUID, Name, PublishingWorkflowId, WorkflowTemplateId, HandleVariableId, PageViewVariableId, AuthGroupId, SelectionWorkflowId, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime)
VALUES (@Guid, @Name, @PublishingWorkFlowId, @WorkflowTemplateId, @HandleVariableId, @PageViewVariableId, @AuthgroupId, @SelectionWorkflowId, 0, @InsertedByUserAlias, @InsertedDateTime, @UpdatedByUserAlias, @UpdatedDateTime)

