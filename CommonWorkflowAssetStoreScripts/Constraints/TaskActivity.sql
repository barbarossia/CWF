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
**      Name: [TaskActivity]          TaskActivity.alt
**      Desc:
**
**      Auth: REDMOND\v-kason
**
**      Date: 03/06/2013 7:15 AM
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
   PRINT '    Dropping CONSTRAINTS FROM [TaskActivity]'

   IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ChildActivity_TaskActivity]') AND parent_object_id = OBJECT_ID(N'[dbo].[TaskActivity]'))
   ALTER TABLE [dbo].[TaskActivity] DROP CONSTRAINT [FK_ChildActivity_TaskActivity]

   PRINT '    Adding CONSTRAINTS To [TaskActivity]'

   alter table [dbo].[TaskActivity] with check add constraint [FK_ChildActivity_TaskActivity] Foreign key([ActivityId])
   references [dbo].[Activity]([Id])
