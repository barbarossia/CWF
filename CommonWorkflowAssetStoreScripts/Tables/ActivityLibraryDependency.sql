/****** Object:  Table [dbo].[ActivityLibraryDependency]    Script Date: 06/12/2011 06:55:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ActivityLibraryDependency](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ActivityLibraryID] [bigint] NOT NULL,
	[DependentActivityLibraryId] [bigint] NOT NULL,
	[SoftDelete] [bit] NOT NULL,
	[InsertedByUserAlias] [nvarchar](50) NOT NULL,
	[InsertedDateTime] [datetime] NOT NULL,
	[UpdatedByUserAlias] [nvarchar](50) NOT NULL,
	[UpdatedDateTime] [datetime] NOT NULL,
	[Timestamp] [timestamp] NOT NULL,
	[UsageCount] [bigint] NOT NULL,
 CONSTRAINT [PK_ActivityLibraryDependency] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

