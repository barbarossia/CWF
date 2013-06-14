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
**      Name: [ActivityCategory]          ActivityCategory.sql
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

   PRINT '    Dropping CONSTRAINTS FROM [ActivityCategory]'
 
   IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ActivityCategory_AuthorizationGroup]') AND parent_object_id = OBJECT_ID(N'[dbo].[ActivityCategory]'))
   ALTER TABLE [dbo].[ActivityCategory] DROP CONSTRAINT [FK_ActivityCategory_AuthorizationGroup]

   PRINT '    Adding CONSTRAINTS To [ActivityCategory]'

   ALTER TABLE [dbo].[ActivityCategory]  WITH CHECK ADD  CONSTRAINT [FK_ActivityCategory_AuthorizationGroup] FOREIGN KEY([AuthGroupID])
   REFERENCES [dbo].[AuthorizationGroup] ([Id])

   ALTER TABLE [dbo].[ActivityCategory] CHECK CONSTRAINT [FK_ActivityCategory_AuthorizationGroup]