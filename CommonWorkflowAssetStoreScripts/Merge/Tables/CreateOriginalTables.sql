
CREATE TABLE [dbo].[Ori_WorkflowType](
	[Id] [bigint]  NOT NULL,
	[GUID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[PublishingWorkflowId] [bigint] NULL,
	[WorkflowTemplateId] [bigint] NULL,
	[HandleVariableId] [bigint] NULL,
	[PageViewVariableId] [bigint] NULL,
	[AuthGroupId] [bigint] NOT NULL,
	[SelectionWorkflowId] [bigint] NULL,
	[SoftDelete] [bit] NOT NULL,
	[InsertedByUserAlias] [nvarchar](50) NOT NULL,
	[InsertedDateTime] [datetime] NOT NULL,
	[UpdatedByUserAlias] [nvarchar](50) NOT NULL,
	[UpdatedDateTime] [datetime] NOT NULL,
	[Timestamp] [timestamp] NOT NULL,
	[Environment] [int] NOT NULL
)
GO

CREATE TABLE [dbo].[Ori_ActivityLibrary](
	[Id] [bigint] NOT NULL,
	[GUID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[AuthGroupId] [bigint] NOT NULL,
	[Category] [uniqueidentifier] NULL,
	[Executable] [varbinary](max) NULL,
	[HasActivities] [bit] NULL,
	[Description] [nvarchar](250) NULL,
	[ImportedBy] [nvarchar](50) NULL,
	[VersionNumber] [nvarchar](50) NOT NULL,
	[Status] [bigint] NOT NULL,
	[MetaTags] [nvarchar](250) NULL,
	[SoftDelete] [bit] NOT NULL,
	[InsertedByUserAlias] [nvarchar](50) NOT NULL,
	[InsertedDateTime] [datetime] NOT NULL,
	[UpdatedByUserAlias] [nvarchar](50) NOT NULL,
	[UpdatedDateTime] [datetime] NOT NULL,
	[Timestamp] [timestamp] NOT NULL,
	[CategoryId] [bigint] NULL,
	[FriendlyName] [nvarchar](50) NULL,
	[ReleaseNotes] [nvarchar](250) NULL,
	[Environment] [int] NOT NULL
)
GO

CREATE TABLE [dbo].[Ori_ActivityLibraryDependency](
	[Id] [bigint] NOT NULL,
	[ActivityLibraryID] [bigint] NOT NULL,
	[DependentActivityLibraryId] [bigint] NOT NULL,
	[SoftDelete] [bit] NOT NULL,
	[InsertedByUserAlias] [nvarchar](50) NOT NULL,
	[InsertedDateTime] [datetime] NOT NULL,
	[UpdatedByUserAlias] [nvarchar](50) NOT NULL,
	[UpdatedDateTime] [datetime] NOT NULL,
	[Timestamp] [timestamp] NOT NULL,
	[UsageCount] [bigint] NOT NULL
)
GO

CREATE TABLE [dbo].[Ori_Activity](
	[Id] [bigint] NOT NULL,
	[GUID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](250) NULL,
	[MetaTags] [nvarchar](max) NULL,
	[IconsId] [bigint] NULL,
	[IsSwitch] [bit] NOT NULL,
	[IsService] [bit] NOT NULL,
	[ActivityLibraryId] [bigint] NULL,
	[IsUxActivity] [bit] NOT NULL,
	[CategoryId] [bigint] NOT NULL,
	[ToolBoxTab] [bigint] NULL,
	[IsToolBoxActivity] [bit] NOT NULL,
	[Version] [nvarchar](25) NOT NULL,
	[StatusId] [bigint] NOT NULL,
	[WorkflowTypeId] [bigint] NULL,
	[Locked] [bit] NULL,
	[LockedBy] [nvarchar](50) NULL,
	[IsCodeBeside] [bit] NULL,
	[XAML] [nvarchar](max) NULL,
	[DeveloperNotes] [nvarchar](250) NULL,
	[BaseType] [nvarchar](50) NULL,
	[Namespace] [nvarchar](250) NULL,
	[SoftDelete] [bit] NOT NULL,
	[InsertedByUserAlias] [nvarchar](50) NOT NULL,
	[InsertedDateTime] [datetime] NOT NULL,
	[UpdatedByUserAlias] [nvarchar](50) NOT NULL,
	[UpdatedDateTime] [datetime] NOT NULL,
	[Timestamp] [timestamp] NOT NULL,
	[Url] [nvarchar](150) NULL,
	[ShortName] [nvarchar](50) NOT NULL,
	[Environment] [int] NOT NULL
)
GO

CREATE TABLE Temp_ActivityLibrary([Id] [bigint] NOT NULL, [OriginalId] [bigint] NOT NULL)
CREATE TABLE Temp_Activity([Id] [bigint] NOT NULL, [OriginalId] [bigint] NOT NULL)
CREATE TABLE Temp_WorkflowType([Id] [bigint] NOT NULL, [OriginalId] [bigint] NOT NULL, [PublishingWorkflowId] [bigint] NULL, [WorkflowTemplateId] [bigint] NULL)
GO
