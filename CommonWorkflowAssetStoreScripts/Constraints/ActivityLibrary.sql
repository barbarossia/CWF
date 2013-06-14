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