/****** Object:  Table [dbo].[ActivityLibrary]    Script Date: 06/12/2011 06:33:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ActivityLibrary]') AND type in (N'U'))

CREATE TABLE [dbo].[ActivityLibrary](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
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
	[FriendlyName]		  NVARCHAR(50)		NULL,
	[ReleaseNotes]		  NVARCHAR(250)		NULL,
	[Environment] [int] NULL,
 CONSTRAINT [PK_ActivityLibrary] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [DF_ActivityLibraryUniqueGUID] UNIQUE NONCLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [DF_ActivityLibraryUniqueNameVersion] UNIQUE NONCLUSTERED 
(
	[Name] ASC,
	[VersionNumber] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

