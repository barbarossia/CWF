 /**************************************************************************
// Product:  CommonWF
// FileName: WorkflowType_Search.sql
// File description: Search row(s) in WorkflowType.
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   [WorkflowType_Search]                        
**    Auth:   v-kason                                                      
**    Date:   04/17/2012                                                   
**                                                                         
****************************************************************************
**   sproc logic flow: <Optional if complex> 
**   Parameter definition if complex
****************************************************************************
**                  CHANGE HISTORY
****************************************************************************
**	Date:        Author:             Description:
** ____________    ________________    ____________________________________
**  11/20/2012      v-kason            Original implementation
** *************************************************************************/
CREATE PROCEDURE [dbo].[WorkflowType_Search]
		@SearchText nvarchar(250),
		@SortColumn varchar(50),
		@SortAscending bit,
		@PageSize int,
		@PageNumber int
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY
			
			IF (@SortColumn IS NULL OR @SortColumn = '')
			SET @SortColumn = 'id'

			IF (@PageSize IS NULL OR @PageSize < 1)
			SET @PageSize = 10

			IF (@PageNumber IS NULL OR @PageNumber < 1)
			SET @PageNumber = 1

			DECLARE @StartIndex int
			IF @PageNumber > 1 
				SET @StartIndex = @PageSize * (@PageNumber - 1)+1
			ELSE
				SET @StartIndex = 1

			IF (@SearchText IS NOT NULL AND @SearchText != '')
				SET @SearchText = '%' + @SearchText + '%'
            
			CREATE TABLE #SearchWorkflowTypeResults 
			( 
				RowNumber bigint, 
				Id int, 
				[GUID] UniqueIdentifier, 
				Name nvarchar(255),
				PublishingWorkflowId bigint,
				PublishingWorkflowName nvarchar(250),
				PublishingWorkflowVersion nvarchar(25), 
				WorkflowTemplateId bigint,
				WorkflowTemplateName nvarchar(250), 
				WorkflowTemplateVersion nvarchar(25),
				SelectionWorkflowId bigint,
				AuthGroupId bigint,
				AuthGroupName nvarchar(255),
				InsertedByUserAlias nvarchar(50),
				InsertedDateTime datetime,
				WorkflowsCount bigint
			)

			IF (@SearchText IS NOT NULL AND @SearchText != '')				
			BEGIN
			
			INSERT INTO #SearchWorkflowTypeResults
			SELECT ROW_NUMBER() OVER 
			(ORDER BY							
						CASE WHEN @SortColumn = 'Name' AND @SortAscending = 1 THEN wf.Name END ASC,
						CASE WHEN @SortColumn = 'Name' THEN wf.Name END DESC,
						CASE WHEN @SortColumn = 'AuthGroupName' AND @SortAscending = 1 THEN ag.Name END ASC,
						CASE WHEN @SortColumn = 'AuthGroupName' THEN ag.Name END DESC,
						CASE WHEN @SortColumn = 'PublishingWorkflow' AND @SortAscending = 1 THEN a1.[Name] END ASC,
						CASE WHEN @SortColumn = 'PublishingWorkflow' THEN a1.[Name] END DESC,
						CASE WHEN @SortColumn = 'TemplateWorkflow'  AND @SortAscending = 1  THEN a2.[Name] END ASC,
						CASE WHEN @SortColumn = 'TemplateWorkflow' THEN a2.[Name] END,
						CASE WHEN @SortColumn = 'CreatedBy'  AND @SortAscending = 1  THEN wf.[InsertedByUserAlias] END ASC,
						CASE WHEN @SortColumn = 'CreatedBy' THEN wf.[InsertedByUserAlias] END DESC,
						CASE WHEN @SortColumn = 'Id'  AND @SortAscending = 1 THEN wf.Id END ASC,
						CASE WHEN @SortColumn = 'Id' THEN wf.Id END DESC
			            ) AS [RowNumber],
					wf.Id, 
					wf.[GUID], 
					wf.Name,
					wf.PublishingWorkflowId,
					a1.Name as PublishingWorkflowName,
					a1.[Version] as PublishingWorkflowVersion,
					wf.WorkflowTemplateId,
					a2.Name as WorkflowTemplateName,
					a2.[Version] as WorkflowTemplateVersion,
					wf.SelectionWorkflowId,
					wf.AuthGroupId,
					ag.Name as AuthGroupName,
					wf.InsertedByUserAlias,
					wf.InsertedDateTime,
					(select COUNT(1) from Activity where WorkflowTypeId=wf.id) as WorkflowsCount 
			FROM [dbo].WorkflowType wf
			LEFT JOIN Activity a1 ON wf.PublishingWorkflowId = a1.Id
			left join Activity a2 on wf.WorkflowTemplateId = a2.Id
			left join AuthorizationGroup ag on wf.AuthGroupId = ag.Id
			WHERE wf.SoftDelete = 0 AND wf.Name like @SearchText
			END
			
			ELSE
			
			BEGIN
			INSERT INTO #SearchWorkflowTypeResults
			SELECT ROW_NUMBER() OVER 
			(ORDER BY							
						CASE WHEN @SortColumn = 'Name' AND @SortAscending = 1 THEN wf.Name END ASC,
						CASE WHEN @SortColumn = 'Name' THEN wf.Name END DESC,
						CASE WHEN @SortColumn = 'AuthGroupName' AND @SortAscending = 1 THEN ag.Name END ASC,
						CASE WHEN @SortColumn = 'AuthGroupName' THEN ag.Name END DESC,
						CASE WHEN @SortColumn = 'PublishingWorkflow' AND @SortAscending = 1 THEN a1.[Name] END ASC,
						CASE WHEN @SortColumn = 'PublishingWorkflow' THEN a1.[Name] END DESC,
						CASE WHEN @SortColumn = 'TemplateWorkflow'  AND @SortAscending = 1  THEN a2.[Name] END ASC,
						CASE WHEN @SortColumn = 'TemplateWorkflow' THEN a2.[Name] END,
						CASE WHEN @SortColumn = 'CreatedBy'  AND @SortAscending = 1  THEN wf.[InsertedByUserAlias] END ASC,
						CASE WHEN @SortColumn = 'CreatedBy' THEN wf.[InsertedByUserAlias] END DESC,
						CASE WHEN @SortColumn = 'Id'  AND @SortAscending = 1 THEN wf.Id END ASC,
						CASE WHEN @SortColumn = 'Id' THEN wf.Id END DESC
			        ) AS [RowNumber],
					wf.Id, 
					wf.[GUID], 
					wf.Name,
					wf.PublishingWorkflowId,
					a1.Name as PublishingWorkflowName,
					a1.[Version] as PublishingWorkflowVersion,
					wf.WorkflowTemplateId,
					a2.Name as WorkflowTemplateName,
					a2.[Version] as WorkflowTemplateVersion,
					wf.SelectionWorkflowId,
					wf.AuthGroupId,
					ag.Name as AuthGroupName,
					wf.InsertedByUserAlias,
					wf.InsertedDateTime,
					(select COUNT(1) from Activity where WorkflowTypeId=wf.id) as WorkflowsCount 
					
			FROM [dbo].WorkflowType wf
			LEFT JOIN Activity a1 ON wf.PublishingWorkflowId = a1.Id
			left join Activity a2 on wf.WorkflowTemplateId = a2.Id
			left join AuthorizationGroup ag on wf.AuthGroupId = ag.Id
			WHERE wf.SoftDelete = 0
			END
			
			SELECT Id,[GUID],Name,PublishingWorkflowId, PublishingWorkflowName, PublishingWorkflowVersion, WorkflowTemplateId, WorkflowTemplateName, WorkflowTemplateVersion
			,AuthGroupId,AuthGroupName,InsertedByUserAlias,InsertedDateTime,WorkflowsCount
			FROM #SearchWorkflowTypeResults
			WHERE RowNumber BETWEEN @StartIndex AND ((@StartIndex + @PageSize) -1)
			order by rownumber	
			select count(*) as Total from #SearchWorkflowTypeResults
			
			DROP TABLE #SearchWorkflowTypeResults									
			
	END TRY
	BEGIN CATCH
		EXECUTE [dbo].Error_Handle
	END CATCH
END

GO


