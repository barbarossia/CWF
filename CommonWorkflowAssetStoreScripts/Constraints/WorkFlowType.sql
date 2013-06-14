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
**      Name: [WorkflowType]          WorkflowType.alt
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

   PRINT '    Dropping CONSTRAINTS FROM [WorkflowType]'
 
   IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkflowType_AuthorizationGroup]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkflowType]'))
   ALTER TABLE [dbo].[WorkflowType] DROP CONSTRAINT [FK_WorkflowType_AuthorizationGroup]

   PRINT '    Adding CONSTRAINTS To [WorkflowType]'

   ALTER TABLE [dbo].[WorkflowType]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowType_AuthorizationGroup] FOREIGN KEY([AuthGroupId])
   REFERENCES [dbo].[AuthorizationGroup] ([Id])

   ALTER TABLE [dbo].[WorkflowType] CHECK CONSTRAINT [FK_WorkflowType_AuthorizationGroup]

   ALTER TABLE [dbo].[WorkflowType]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowType_PublishingWorkflowId] FOREIGN KEY([PublishingWorkflowId])
   REFERENCES [dbo].[Activity] ([Id])


   ALTER TABLE [dbo].[WorkflowType] CHECK CONSTRAINT [FK_WorkflowType_PublishingWorkflowId]


   ALTER TABLE [dbo].[WorkflowType]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowType_SelectionWorkflowId] FOREIGN KEY([SelectionWorkflowId])
   REFERENCES [dbo].[Activity] ([Id])


   ALTER TABLE [dbo].[WorkflowType] CHECK CONSTRAINT [FK_WorkflowType_SelectionWorkflowId]


   ALTER TABLE [dbo].[WorkflowType]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowType_WorkflowTemplateId] FOREIGN KEY([WorkflowTemplateId])
   REFERENCES [dbo].[Activity] ([Id])


   ALTER TABLE [dbo].[WorkflowType] CHECK CONSTRAINT [FK_WorkflowType_WorkflowTemplateId]


