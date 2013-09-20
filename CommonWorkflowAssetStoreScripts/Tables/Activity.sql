SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Activity]') AND type in (N'U'))

CREATE TABLE [dbo].[Activity](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
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
	[Environment] [int] NULL,
 CONSTRAINT [PK_Activity] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [DF_ActivityUniqueGUID] UNIQUE NONCLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [DF_ActivityUniqueName] UNIQUE NONCLUSTERED 
(
	[Name] ASC,
	[Version] ASC,
	[ShortName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO



