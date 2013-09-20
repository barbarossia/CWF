-- disable all constraints
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ActivityUniqueGUID]'))
BEGIN
ALTER TABLE [dbo].[Activity] DROP CONSTRAINT [DF_ActivityUniqueGUID]
END

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ActivityUniqueName]'))
BEGIN
ALTER TABLE [dbo].[Activity] DROP CONSTRAINT [DF_ActivityUniqueName]
END

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ActivityLibraryUniqueGUID]'))
BEGIN
ALTER TABLE [dbo].[ActivityLibrary] DROP CONSTRAINT [DF_ActivityLibraryUniqueGUID]
END

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ActivityLibraryUniqueNameVersion]'))
BEGIN
ALTER TABLE [dbo].[ActivityLibrary] DROP CONSTRAINT [DF_ActivityLibraryUniqueNameVersion]
END

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_WorkflowType_Name]'))
BEGIN
ALTER TABLE [dbo].[WorkflowType] DROP CONSTRAINT [DF_WorkflowType_Name]
END

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_WorkflowTypeUniqueGUID]'))
BEGIN
ALTER TABLE [dbo].[WorkflowType] DROP CONSTRAINT [DF_WorkflowTypeUniqueGUID]
END

Go
-- insert temp table

SELECT [Name]
      ,[AuthGroupId]
      ,[Category]
      ,[Executable]
      ,[HasActivities]
      ,[Description]
      ,[ImportedBy]
      ,[VersionNumber]
      ,[Status]
      ,[MetaTags]
      ,[SoftDelete]
      ,[InsertedByUserAlias]
      ,[InsertedDateTime]
      ,[UpdatedByUserAlias]
      ,[UpdatedDateTime]
      ,[Timestamp]
      ,[CategoryId]
      ,[FriendlyName]
      ,[ReleaseNotes] 
into #T_ActivityLibrary
from ActivityLibrary
      
SELECT AL1.Name AS [ActivityLibraryName]
	,AL1.VersionNumber AS [ActivityLibraryVersion]
      ,AL2.Name AS [DependentActivityLibraryName]
      ,AL2.VersionNumber AS [DependentActivityLibraryVersion]
      ,D.[SoftDelete]
      ,D.[InsertedByUserAlias]
      ,D.[InsertedDateTime]
      ,D.[UpdatedByUserAlias]
      ,D.[UpdatedDateTime]
      ,D.[Timestamp]
      ,D.[UsageCount]
into #T_ActivityLibraryDependency
from ActivityLibraryDependency D 
JOIN ActivityLibrary AL1 ON D.ActivityLibraryID = AL1.Id
JOIN ActivityLibrary AL2 ON D.DependentActivityLibraryId = AL2.Id


SELECT [Name]
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
      ,[Timestamp]
into #T_WorkflowType
from WorkflowType


SELECT A.[Name]
      ,A.[Description]
      ,A.[MetaTags]
      ,A.[IconsId]
      ,A.[IsSwitch]
      ,A.[IsService]
      ,AL.Name AS [ActivityLibraryName]
      ,AL.VersionNumber AS [ActivityLibraryVersion]
      ,A.[IsUxActivity]
      ,A.[CategoryId]
      ,A.[ToolBoxTab]
      ,A.[IsToolBoxActivity]
      ,A.[Version]
      ,A.[StatusId]
      ,W.[Name] AS WorkflowTypeName
      ,A.[Locked]
      ,A.[LockedBy]
      ,A.[IsCodeBeside]
      ,A.[XAML]
      ,A.[DeveloperNotes]
      ,A.[BaseType]
      ,A.[Namespace]
      ,A.[SoftDelete]
      ,A.[InsertedByUserAlias]
      ,A.[InsertedDateTime]
      ,A.[UpdatedByUserAlias]
      ,A.[UpdatedDateTime]
      ,A.[Timestamp]
      ,A.[Url]
      ,A.[ShortName]
into #T_Activity
from Activity A 
JOIN ActivityLibrary AL ON A.ActivityLibraryId = AL.Id
JOIN WorkflowType W ON A.WorkflowTypeId = W.Id

-- delete template data
delete ActivityLibraryDependency

update WorkflowType
set PublishingWorkflowId = null, WorkflowTemplateId = null, SelectionWorkflowId = null

delete Activity

delete WorkflowType

delete ActivityLibrary

-- insert new template data

insert into WorkflowType([GUID]
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
SELECT NEWID()
	,W.[Name]
      ,NULL AS [PublishingWorkflowId]
      ,NULL AS [WorkflowTemplateId]
      ,W.[HandleVariableId]
      ,W.[PageViewVariableId]
      ,W.[AuthGroupId]
      ,NULL AS [SelectionWorkflowId]
      ,W.[SoftDelete]
      ,W.[InsertedByUserAlias]
      ,W.[InsertedDateTime]
      ,W.[UpdatedByUserAlias]
      ,W.[UpdatedDateTime]
      ,E.[Id] AS Environment
from #T_WorkflowType W, Environment E

insert into ActivityLibrary(
      [GUID]
      ,[Name]
      ,[AuthGroupId]
      ,[Category]
      ,[Executable]
      ,[HasActivities]
      ,[Description]
      ,[ImportedBy]
      ,[VersionNumber]
      ,[Status]
      ,[MetaTags]
      ,[SoftDelete]
      ,[InsertedByUserAlias]
      ,[InsertedDateTime]
      ,[UpdatedByUserAlias]
      ,[UpdatedDateTime]
      ,[CategoryId]
      ,[FriendlyName]
      ,[ReleaseNotes]
      ,[Environment])
SELECT NEWID()
      ,AL.[Name]
      ,AL.[AuthGroupId]
      ,AL.[Category]
      ,AL.[Executable]
      ,AL.[HasActivities]
      ,AL.[Description]
      ,AL.[ImportedBy]
      ,AL.[VersionNumber]
      ,AL.[Status]
      ,AL.[MetaTags]
      ,AL.[SoftDelete]
      ,AL.[InsertedByUserAlias]
      ,AL.[InsertedDateTime]
      ,AL.[UpdatedByUserAlias]
      ,AL.[UpdatedDateTime]
      ,AL.[CategoryId]
      ,AL.[FriendlyName]
      ,AL.[ReleaseNotes]
      ,E.[Id] AS Environment
from #T_ActivityLibrary AL, Environment E

insert into ActivityLibraryDependency(
      [ActivityLibraryID]
      ,[DependentActivityLibraryId]
      ,[SoftDelete]
      ,[InsertedByUserAlias]
      ,[InsertedDateTime]
      ,[UpdatedByUserAlias]
      ,[UpdatedDateTime]
      ,[UsageCount])
SELECT AL1.Id AS [ActivityLibraryId]
      ,AL2.Id AS [DependentActivityLibraryId]
      ,D.[SoftDelete]
      ,D.[InsertedByUserAlias]
      ,D.[InsertedDateTime]
      ,D.[UpdatedByUserAlias]
      ,D.[UpdatedDateTime]
      ,D.[UsageCount]
from #T_ActivityLibraryDependency D 
JOIN ActivityLibrary AL1 ON D.[ActivityLibraryName] = AL1.[Name] and d.[ActivityLibraryVersion] = al1.[VersionNumber]
JOIN ActivityLibrary AL2 ON D.[DependentActivityLibraryName] = AL2.[Name] and d.[DependentActivityLibraryVersion] = al2.[VersionNumber]
where al1.Environment = al2.Environment


insert into Activity(
      [GUID]
      ,[Name]
      ,[Description]
      ,[MetaTags]
      ,[IconsId]
      ,[IsSwitch]
      ,[IsService]
      ,[ActivityLibraryId]
      ,[IsUxActivity]
      ,[CategoryId]
      ,[ToolBoxTab]
      ,[IsToolBoxActivity]
      ,[Version]
      ,[StatusId]
      ,[WorkflowTypeId]
      ,[Locked]
      ,[LockedBy]
      ,[IsCodeBeside]
      ,[XAML]
      ,[DeveloperNotes]
      ,[BaseType]
      ,[Namespace]
      ,[SoftDelete]
      ,[InsertedByUserAlias]
      ,[InsertedDateTime]
      ,[UpdatedByUserAlias]
      ,[UpdatedDateTime]
      ,[Url]
      ,[ShortName]
      ,[Environment])
SELECT  NEWID()
	,A.[Name]
      ,A.[Description]
      ,A.[MetaTags]
      ,A.[IconsId]
      ,A.[IsSwitch]
      ,A.[IsService]
      ,AL.Id AS [ActivityLibraryId]
      ,A.[IsUxActivity]
      ,A.[CategoryId]
      ,A.[ToolBoxTab]
      ,A.[IsToolBoxActivity]
      ,A.[Version]
      ,A.[StatusId]
      ,NULL AS [WorkflowTypeId]
      ,A.[Locked]
      ,A.[LockedBy]
      ,A.[IsCodeBeside]
      ,A.[XAML]
      ,A.[DeveloperNotes]
      ,A.[BaseType]
      ,A.[Namespace]
      ,A.[SoftDelete]
      ,A.[InsertedByUserAlias]
      ,A.[InsertedDateTime]
      ,A.[UpdatedByUserAlias]
      ,A.[UpdatedDateTime]
      ,A.[Url]
      ,A.[ShortName]
      ,AL.Environment 
from #T_Activity A
JOIN ActivityLibrary AL ON A.[ActivityLibraryName] = AL.Name AND A.[ActivityLibraryVersion] = AL.VersionNumber


update [dbo].[WorkflowType]
set WorkflowTemplateId = A.Id
from WorkflowType W
join Activity A on w.Environment = a.Environment
where W.Name='Page'and a.Name = 'PageTemplate'

update [dbo].[WorkflowType]
set PublishingWorkflowId = A.Id
from WorkflowType W
join Activity A on w.Environment = a.Environment
where W.Name='Workflow' and a.Name = 'PublishingWorkflow'

update [dbo].[WorkflowType]
set WorkflowTemplateId = A.Id
from WorkflowType W
join Activity A on w.Environment = a.Environment
where W.Name='Workflow' and a.Name = 'WorkflowTemplate'


UPDATE Activity
SET WorkflowTypeId = W.ID
from Activity A
join WorkflowType w on a.Environment = w.Environment 
where a.Name = 'PageTemplate' and w.Name = 'Workflow'

UPDATE Activity
SET WorkflowTypeId = W.ID
from Activity A
join WorkflowType w on a.Environment = w.Environment 
where w.Name = 'Metadata' and a.Name <> 'PageTemplate'


drop table #T_Activity
drop table #T_ActivityLibrary
drop table #T_ActivityLibraryDependency
drop table #T_WorkflowType



