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
**      Name: [ActivityLibrary]          ActivityLibrary.sql
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
   PRINT '    Dropping CONSTRAINTS FROM [ActivityLibrary]'
 
   IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__ActivityLibrary__ActivityCategory]') AND parent_object_id = OBJECT_ID(N'[dbo].[ActivityLibrary]'))
   ALTER TABLE [dbo].[ActivityLibrary] DROP CONSTRAINT [FK__ActivityLibrary__ActivityCategory]

   IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ActivityLibrary_AuthorizationGroup]') AND parent_object_id = OBJECT_ID(N'[dbo].[ActivityLibrary]'))
   ALTER TABLE [dbo].[ActivityLibrary] DROP CONSTRAINT [FK_ActivityLibrary_AuthorizationGroup]

   IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ActivityLibrary_Status]') AND parent_object_id = OBJECT_ID(N'[dbo].[ActivityLibrary]'))
   ALTER TABLE [dbo].[ActivityLibrary] DROP CONSTRAINT [FK_ActivityLibrary_Status]

   IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ActivityLibrary_Environment]') AND parent_object_id = OBJECT_ID(N'[dbo].[ActivityLibrary]'))
   ALTER TABLE [dbo].[ActivityLibrary] DROP CONSTRAINT [FK_ActivityLibrary_Environment]

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ActivityLibraryUniqueGUID]'))
BEGIN
ALTER TABLE [dbo].[ActivityLibrary] DROP CONSTRAINT [DF_ActivityLibraryUniqueGUID]
END

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ActivityLibraryUniqueNameVersion]'))
BEGIN
ALTER TABLE [dbo].[ActivityLibrary] DROP CONSTRAINT [DF_ActivityLibraryUniqueNameVersion]
END

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ActivityLibrary_ContainsDesignerActivities]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ActivityLibrary] DROP CONSTRAINT [DF_ActivityLibrary_ContainsDesignerActivities]
END


   PRINT '    Adding CONSTRAINTS To [ActivityLibrary]'

   ALTER TABLE [dbo].[ActivityLibrary]  WITH CHECK ADD FOREIGN KEY([CategoryId])
   REFERENCES [dbo].[ActivityCategory] ([Id])

   ALTER TABLE [dbo].[ActivityLibrary]  WITH CHECK ADD  CONSTRAINT [FK_ActivityLibrary_AuthorizationGroup] FOREIGN KEY([AuthGroupId])
   REFERENCES [dbo].[AuthorizationGroup] ([Id])

   ALTER TABLE [dbo].[ActivityLibrary] CHECK CONSTRAINT [FK_ActivityLibrary_AuthorizationGroup]

   ALTER TABLE [dbo].[ActivityLibrary]  WITH CHECK ADD  CONSTRAINT [FK_ActivityLibrary_Status] FOREIGN KEY([Status])
   REFERENCES [dbo].[StatusCode] ([Code])

   ALTER TABLE [dbo].[ActivityLibrary] CHECK CONSTRAINT [FK_ActivityLibrary_Status]

   ALTER TABLE [dbo].[ActivityLibrary] ADD  CONSTRAINT [DF_ActivityLibrary_ContainsDesignerActivities]  DEFAULT ((1)) FOR [HasActivities]

   ALTER TABLE [dbo].[ActivityLibrary]  WITH CHECK ADD  CONSTRAINT [FK_ActivityLibrary_Environment] FOREIGN KEY([Environment])
   REFERENCES [dbo].[Environment] ([Id])

   ALTER TABLE [dbo].[ActivityLibrary] CHECK CONSTRAINT [FK_ActivityLibrary_Environment]

ALTER TABLE [dbo].[ActivityLibrary] ADD  CONSTRAINT [DF_ActivityLibraryUniqueGUID] UNIQUE NONCLUSTERED 
(
	[GUID] ASC,
	[Environment] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

ALTER TABLE [dbo].[ActivityLibrary] ADD  CONSTRAINT [DF_ActivityLibraryUniqueNameVersion] UNIQUE NONCLUSTERED 
(
	[Name] ASC,
	[VersionNumber] ASC,
	[Environment] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]



