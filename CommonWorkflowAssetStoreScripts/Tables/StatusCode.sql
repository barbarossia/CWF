/****** Object:  Table [dbo].[StatusCode]    Script Date: 06/12/2011 06:51:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[StatusCode](
	[Code] [bigint] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](250) NOT NULL,
	[ShowInProduction] [bit] NOT NULL,
	[LockForChanges] [bit] NOT NULL,
	[IsDeleted] [bit] NULL,
	[IsEligibleForCleanUp] [bit] NULL,
	[SoftDelete] [bit] NOT NULL,
	[InsertedByUserAlias] [nvarchar](50) NOT NULL,
	[InsertedDateTime] [datetime] NOT NULL,
	[UpdatedByUserAlias] [nvarchar](50) NOT NULL,
	[UpdatedDateTime] [datetime] NOT NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_StatusCode] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [StatusCodeUniqueName] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

