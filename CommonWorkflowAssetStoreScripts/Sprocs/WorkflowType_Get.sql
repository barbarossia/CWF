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

/**************************************************************************
// Product:  CommonWF
// FileName: WorkflowType_Get.sql
// File description: Get a row in etblWorkflowType.
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   WorkflowType_Get                                       *
**    Auth:   v-stska                                                      *
**    Date:   10/27/2010                                                   *
**                                                                         *
****************************************************************************
**   sproc logic flow: <Optional if complex> 
**   Parameter definition if complex
****************************************************************************
**                  CHANGE HISTORY
****************************************************************************
**	Date:        Author:             Description:
** ____________    ________________    ____________________________________
**  11/23/2010      v-stska             Original implementation
**  12/12/2010      v-stska             Update to NEW3PrototypeAssetStore
**  2/13/2011       v-stska             Add @OutError logic
**  11-Nov-2011	    v-richt             Bug#86713 - Change error codes to positive numbers
**  03/27/2012	    v-sanja             Exception handling refinements, move validations to business layer.
** *************************************************************************/
CREATE PROCEDURE [dbo].[WorkflowType_Get]
        @Id BIGINT,
        @Name VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON	
    
    BEGIN TRY
        IF (@Id != 0)
        BEGIN
            SELECT wft.[Id]
                  ,wft.[GUID]
                  ,wft.[Name]
                  ,wft.[PublishingWorkflowId]
                  ,sa1.[Name] AS PublishingWorkflowName
				  ,sa1.[Version] AS PublishingWorkflowVersion
                  ,wft.[WorkflowTemplateId]
                  ,sa2.[Name] AS WorkflowTemplateName
                  ,wft.[HandleVariableId]
                  ,wft.[PageViewVariableId]
                  ,wft.[AuthGroupId]
                  ,ag.[Name] AS AuthGroupName
                  ,wft.[SelectionWorkflowId]
              FROM [dbo].[WorkflowType] wft
              JOIN AuthorizationGroup ag ON wft.AuthGroupId = ag.Id
              LEFT JOIN Activity sa1 ON wft.PublishingWorkflowId = sa1.id
              LEFT JOIN Activity sa2 ON wft.WorkflowTemplateId = sa2.Id
              WHERE wft.Id = @Id
                AND wft.SoftDelete = 0
        END
        ELSE
        IF (@Name IS NOT NULL AND @Name != '')
        BEGIN				
            SELECT wft.[Id]
                  ,wft.[GUID]
                  ,wft.[Name]
                  ,wft.[PublishingWorkflowId]
                  ,sa1.[Name] AS PublishingWorkflowName
				  ,sa1.[Version] AS PublishingWorkflowVersion
                  ,wft.[WorkflowTemplateId]
                  ,sa2.[Name] AS WorkflowTemplateName
                  ,wft.[HandleVariableId]
                  ,wft.[PageViewVariableId]
                  ,wft.[AuthGroupId]
                  ,ag.[Name] AS AuthGroupName
                  ,wft.[SelectionWorkflowId]
              FROM [dbo].[WorkflowType] wft
              JOIN AuthorizationGroup ag ON wft.AuthGroupId = ag.Id
              LEFT JOIN Activity sa1 on wft.PublishingWorkflowId = sa1.id
              LEFT JOIN Activity sa2 ON wft.WorkflowTemplateId = sa2.Id
              WHERE wft.name = @Name
                AND wft.SoftDelete = 0
        END
        ELSE
        BEGIN
            SELECT wft.[Id]
                  ,wft.[GUID]
                  ,wft.[Name]
                  ,wft.[PublishingWorkflowId]
                  ,sa1.[Name] AS PublishingWorkflowName
				  ,sa1.[Version] AS PublishingWorkflowVersion
                  ,wft.[WorkflowTemplateId]
                  ,sa2.[Name] AS WorkflowTemplateName
                  ,wft.[HandleVariableId]
                  ,wft.[PageViewVariableId]
                  ,wft.[AuthGroupId]
                  ,ag.[Name] AS AuthGroupName
                  ,wft.[SelectionWorkflowId]
              FROM [dbo].[WorkflowType] wft
              JOIN AuthorizationGroup ag ON wft.AuthGroupId = ag.Id
              LEFT JOIN Activity sa1 on wft.PublishingWorkflowId = sa1.id
              LEFT JOIN Activity sa2 ON wft.WorkflowTemplateId = sa2.Id
              WHERE wft.SoftDelete = 0
        END
    END TRY
    BEGIN CATCH
        EXECUTE [dbo].Error_Handle 
    END CATCH
END
GO
