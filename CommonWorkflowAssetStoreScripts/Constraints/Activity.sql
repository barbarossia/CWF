SET ANSI_DEFAULTS ON
SET CURSOR_CLOSE_ON_COMMIT OFF
SET IMPLICIT_TRANSACTIONS OFF
SET ARITHABORT ON
SET CONCAT_NULL_YIELDS_NULL ON
SET NUMERIC_ROUNDABORT OFF
SET QUOTED_IDENTIFIER ON
 
SET DATEFORMAT ymd
SET LOCK_TIMEOUT -1
SET NOCOUNT ON
SET ROWCOUNT 0
SET TEXTSIZE 0
GO
 
/**********************************************************************
**      Name: [Activity]          Activity.alt
**      Desc:
**
**      Auth: REDMOND\v-stska
**
**      Date: 06/13/2011 7:15 AM
**
***********************************************************************
**
**              CHANGE HISTORY
***********************************************************************
**   Date:      Author:      Description:
**   ________   __________   __________________________________________
**
**
***********************************************************************/
   PRINT '    Dropping CONSTRAINTS FROM [Activity]'
 
   IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Activity_ActivityCategory]') AND parent_object_id = OBJECT_ID(N'[dbo].[Activity]'))
   ALTER TABLE [dbo].[Activity] DROP CONSTRAINT [FK_Activity_ActivityCategory]

   IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Activity_ActivityLibrary]') AND parent_object_id = OBJECT_ID(N'[dbo].[Activity]'))
   ALTER TABLE [dbo].[Activity] DROP CONSTRAINT [FK_Activity_ActivityLibrary]

   IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Activity_Icon]') AND parent_object_id = OBJECT_ID(N'[dbo].[Activity]'))
   ALTER TABLE [dbo].[Activity] DROP CONSTRAINT [FK_Activity_Icon]

   IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Activity_StatusCode]') AND parent_object_id = OBJECT_ID(N'[dbo].[Activity]'))
   ALTER TABLE [dbo].[Activity] DROP CONSTRAINT [FK_Activity_StatusCode]

   IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Activity_WorkflowType]') AND parent_object_id = OBJECT_ID(N'[dbo].[Activity]'))
   ALTER TABLE [dbo].[Activity] DROP CONSTRAINT [FK_Activity_WorkflowType]

   IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Activity_IsUxActivity]') AND type = 'D')
   BEGIN
      ALTER TABLE [dbo].[Activity] DROP CONSTRAINT [DF_Activity_IsUxActivity]
   END

   IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Activity_Environment]') AND parent_object_id = OBJECT_ID(N'[dbo].[Activity]'))
   ALTER TABLE [dbo].[Activity] DROP CONSTRAINT [FK_Activity_Environment]

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ActivityUniqueGUID]'))
BEGIN
ALTER TABLE [dbo].[Activity] DROP CONSTRAINT [DF_ActivityUniqueGUID]
END

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ActivityUniqueName]'))
BEGIN
ALTER TABLE [dbo].[Activity] DROP CONSTRAINT [DF_ActivityUniqueName]
END
   
   PRINT '    Adding CONSTRAINTS To [Activity]'

   ALTER TABLE [dbo].[Activity]  WITH CHECK ADD  CONSTRAINT [FK_Activity_ActivityCategory] FOREIGN KEY([CategoryId])
   REFERENCES [dbo].[ActivityCategory] ([Id])

   ALTER TABLE [dbo].[Activity] CHECK CONSTRAINT [FK_Activity_ActivityCategory]

   ALTER TABLE [dbo].[Activity]  WITH CHECK ADD  CONSTRAINT [FK_Activity_ActivityLibrary] FOREIGN KEY([ActivityLibraryId])
   REFERENCES [dbo].[ActivityLibrary] ([Id])

   ALTER TABLE [dbo].[Activity] CHECK CONSTRAINT [FK_Activity_ActivityLibrary]

   ALTER TABLE [dbo].[Activity]  WITH CHECK ADD  CONSTRAINT [FK_Activity_Icon] FOREIGN KEY([IconsId])
   REFERENCES [dbo].[Icon] ([Id])

   ALTER TABLE [dbo].[Activity] CHECK CONSTRAINT [FK_Activity_Icon]

   ALTER TABLE [dbo].[Activity]  WITH CHECK ADD  CONSTRAINT [FK_Activity_StatusCode] FOREIGN KEY([StatusId])
   REFERENCES [dbo].[StatusCode] ([Code])

   ALTER TABLE [dbo].[Activity] CHECK CONSTRAINT [FK_Activity_StatusCode]

   ALTER TABLE [dbo].[Activity]  WITH CHECK ADD  CONSTRAINT [FK_Activity_WorkflowType] FOREIGN KEY([WorkflowTypeId])
   REFERENCES [dbo].[WorkflowType] ([Id])

   ALTER TABLE [dbo].[Activity] CHECK CONSTRAINT [FK_Activity_WorkflowType]

   ALTER TABLE [dbo].[Activity]  WITH CHECK ADD  CONSTRAINT [FK_Activity_Environment] FOREIGN KEY([Environment])
   REFERENCES [dbo].[Environment] ([Id])

   ALTER TABLE [dbo].[Activity] CHECK CONSTRAINT [FK_Activity_Environment]

   ALTER TABLE [dbo].[Activity] ADD  CONSTRAINT [DF_Activity_IsUxActivity]  DEFAULT ((0)) FOR [IsUxActivity]

ALTER TABLE [dbo].[Activity] ADD  CONSTRAINT [DF_ActivityUniqueGUID] UNIQUE NONCLUSTERED 
(
	[GUID] ASC,
	[Environment] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

ALTER TABLE [dbo].[Activity] ADD  CONSTRAINT [DF_ActivityUniqueName] UNIQUE NONCLUSTERED 
(
	[Name] ASC,
	[Version] ASC,
	[ShortName] ASC,
	[Environment] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]


