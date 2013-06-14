/****** Object:  Table [dbo].[ActivityLibraryDependencyHead]    Script Date: 06/12/2011 06:36:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ActivityLibraryDependencyHead](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TreeHead] [bigint] NOT NULL,
	[IntersectingTableLinkID] [bigint] NULL,
	[SoftDelete] [bit] NOT NULL,
	[InsertedByUserAlias] [nvarchar](50) NOT NULL,
	[InsertedDateTime] [datetime] NOT NULL,
	[UpdatedByUserAlias] [nvarchar](50) NOT NULL,
	[UpdatedDateTime] [datetime] NOT NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_ActivityLibraryDependencyHead] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


