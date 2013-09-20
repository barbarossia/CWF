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
BEGIN
ALTER TABLE [dbo].[WorkflowType] DROP CONSTRAINT [FK_WorkflowType_AuthorizationGroup]
END
   
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkflowType_Environment]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkflowType]'))
   ALTER TABLE [dbo].[WorkflowType] DROP CONSTRAINT [FK_WorkflowType_Environment]

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkflowType_PublishingWorkflowId]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkflowType]'))
   ALTER TABLE [dbo].[WorkflowType] DROP CONSTRAINT [FK_WorkflowType_PublishingWorkflowId]

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkflowType_SelectionWorkflowId]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkflowType]'))
   ALTER TABLE [dbo].[WorkflowType] DROP CONSTRAINT [FK_WorkflowType_SelectionWorkflowId]

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkflowType_WorkflowTemplateId]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkflowType]'))
   ALTER TABLE [dbo].[WorkflowType] DROP CONSTRAINT [FK_WorkflowType_WorkflowTemplateId]

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_WorkflowType_Name]'))
BEGIN
ALTER TABLE [dbo].[WorkflowType] DROP CONSTRAINT [DF_WorkflowType_Name]
END

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_WorkflowTypeUniqueGUID]'))
BEGIN
ALTER TABLE [dbo].[WorkflowType] DROP CONSTRAINT [DF_WorkflowTypeUniqueGUID]
END


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

   ALTER TABLE [dbo].[WorkflowType]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowType_Environment] FOREIGN KEY([Environment])
   REFERENCES [dbo].[Environment] ([Id])

   ALTER TABLE [dbo].[WorkflowType] CHECK CONSTRAINT [FK_WorkflowType_Environment]

ALTER TABLE [dbo].[WorkflowType] ADD  CONSTRAINT [DF_WorkflowType_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC,
	[Environment] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

ALTER TABLE [dbo].[WorkflowType] ADD  CONSTRAINT [DF_WorkflowTypeUniqueGUID] UNIQUE NONCLUSTERED 
(
	[GUID] ASC,
	[Environment] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]


