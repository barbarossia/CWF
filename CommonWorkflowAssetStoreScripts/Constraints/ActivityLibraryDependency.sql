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
**      Name: [ActivityLibraryDependency]          ActivityLibraryDependency.sql
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

   PRINT '    Dropping CONSTRAINTS FROM [ActivityLibraryDependency]'
 
   IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ActivityLibraryID]') AND parent_object_id = OBJECT_ID(N'[dbo].[ActivityLibraryDependency]'))
   ALTER TABLE [dbo].[ActivityLibraryDependency] DROP CONSTRAINT [FK_ActivityLibraryID]

   IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DependentActivityLibraryId]') AND parent_object_id = OBJECT_ID(N'[dbo].[ActivityLibraryDependency]'))
   ALTER TABLE [dbo].[ActivityLibraryDependency] DROP CONSTRAINT [FK_DependentActivityLibraryId]

   IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__ActivityLibraryDependency__Usage]') AND type = 'D')
   BEGIN
      ALTER TABLE [dbo].[ActivityLibraryDependency] DROP CONSTRAINT [DF__ActivityLibraryDependency__Usage]
   END

   PRINT '    Adding CONSTRAINTS To [ActivityLibraryDependency]'

   ALTER TABLE [dbo].[ActivityLibraryDependency]  WITH CHECK ADD  CONSTRAINT [FK_ActivityLibraryID] FOREIGN KEY([ActivityLibraryID])
   REFERENCES [dbo].[ActivityLibrary] ([Id])

   ALTER TABLE [dbo].[ActivityLibraryDependency] CHECK CONSTRAINT [FK_ActivityLibraryID]

   ALTER TABLE [dbo].[ActivityLibraryDependency]  WITH CHECK ADD  CONSTRAINT [FK_DependentActivityLibraryId] FOREIGN KEY([DependentActivityLibraryId])
   REFERENCES [dbo].[ActivityLibrary] ([Id])

   ALTER TABLE [dbo].[ActivityLibraryDependency] CHECK CONSTRAINT [FK_DependentActivityLibraryId]

   ALTER TABLE [dbo].[ActivityLibraryDependency] ADD  DEFAULT ((1)) FOR [UsageCount]