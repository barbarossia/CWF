SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TaskActivity](

 [Id] [bigint] Identity(1,1) NOT NULL,
 [GUID] [uniqueidentifier] not null,
 [ActivityId] [bigint] not null,
 [AssignedTo] [nvarchar](100) null,
 [Status][nvarchar](50) null,

 CONSTRAINT [PK_TaskActivity] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],

) ON [PRIMARY]

GO