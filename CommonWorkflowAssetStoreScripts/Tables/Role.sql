SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Role]') AND type in (N'U'))

CREATE TABLE [dbo].[Role](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[SoftDelete] [bit] NOT NULL,
	[InsertedByUserAlias] [nvarchar](50) NOT NULL,
	[InsertedDateTime] [datetime] NOT NULL,
	[UpdatedByUserAlias] [nvarchar](50) NOT NULL,
	[UpdatedDateTime] [datetime] NOT NULL,
	[Timestamp] [timestamp] NOT NULL,
CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


