/****** Object:  StoredProcedure [dbo].[Marketplace_SearchXamlItems]    Script Date: 07/31/2012 13:37:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




/**************************************************************************
// Product:  CommonWF
// FileName: Marketplace_SearchXamlItems.sql
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   Marketplace_SearchXamlItems                                  *
**    Desc:   Search marketplace.                                          *
**    Auth:   v-bobzh                                                      *
**    Date:   05/31/2012                                                   *
**                                                                         *
****************************************************************************
**   sproc logic flow: <Optional if complex> 
**   Parameter definition if complex
****************************************************************************
**                  CHANGE HISTORY
****************************************************************************
**	Date:        Author:             Description:
** ____________    ________________    ____________________________________
**  05/31/2012     v-bobzh             Original implementation
** *************************************************************************/
create PROCEDURE [dbo].[Marketplace_SearchXamlItems]
		@SearchText nvarchar(250),
		@GetTemplates bit = null,
		@GetPublishingWorkflows bit =null,
		@PageSize int,
		@PageNumber int,
		@SortColumn varchar(50),
		@SortAscending bit,
		@IsNewest bit
AS
BEGIN
	SET NOCOUNT ON	
	BEGIN TRY	

		DECLARE @StartIndex int
		IF @PageNumber > 1 
			SET @StartIndex = @PageSize * (@PageNumber - 1) + 1
		ELSE
			SET @StartIndex = 1	
			
		IF ISNULL(@SearchText,'') = ''		
			SET @SearchText	='""'
		ELSE
			SET @SearchText = '"' + @SearchText + '*"'	
		
		DECLARE @TotalCount INT
		DECLARE @PageCount INT
		
		DECLARE @FilterType tinyint
		if (@GetTemplates IS NULL and @GetPublishingWorkflows IS NULL)
			set @FilterType=0
		if (@GetTemplates = 1 AND @GetPublishingWorkflows IS NULL)
			set @FilterType=1
		if (@GetTemplates IS NULL AND @GetPublishingWorkflows = 1)
			set @FilterType=2
		if (@GetTemplates = 0 AND @GetPublishingWorkflows = 0)
			set @FilterType=3
		
		;WITH cteCount (Id) AS
        (	
			SELECT DISTINCT al.Id
			FROM ActivityLibrary al
				LEFT JOIN Activity sa ON sa.ActivityLibraryId = al.Id
				JOIN StatusCode sc ON al.[Status] = sc.Code AND sc.Name = 'Public'
				LEFT JOIN [dbo].[WorkflowType] wft1 ON wft1.WorkflowTemplateId = sa.Id
				LEFT JOIN [dbo].[WorkflowType] wft2 ON wft2.PublishingWorkflowId = sa.Id
			WHERE sa.XAML IS NOT NULL
				AND (@SearchText = '""' 
					OR CONTAINS(al.Name, @SearchText) 
					OR CONTAINS(al.[Description], @SearchText)
					OR CONTAINS(al.MetaTags, @SearchText)
					OR CONTAINS(sa.ShortName, @SearchText))
				AND (NOT @FilterType = 0 OR 1 = 1)
				AND (NOT @FilterType = 1 OR wft1.WorkflowTemplateId IS NOT NULL)
				AND (NOT @FilterType = 2 OR wft2.PublishingWorkflowId IS NOT NULL)
				AND (NOT @FilterType = 3 
					OR (wft1.WorkflowTemplateId IS NULL AND wft2.PublishingWorkflowId IS NULL))
				AND (@IsNewest = 0 OR al.Id IN (SELECT MAX(Id) as Id FROM ActivityLibrary JOIN StatusCode ON ActivityLibrary.[Status] = StatusCode.Code AND StatusCode.Name = 'Public' GROUP BY ActivityLibrary.Name))
		)
		SELECT @TotalCount = COUNT(*) FROM cteCount
		IF @TotalCount > 0 
		BEGIN
			SET  @PageCount = (@TotalCount - 1) / @PageSize + 1
			IF @PageNumber > @PageCount
			BEGIN
				SET @PageNumber = @PageCount
				SET @StartIndex = @PageSize * (@PageNumber - 1) + 1
			END
		END
		ELSE
		BEGIN
			RETURN
		END
		
		;WITH cteSearch (Id, Name, UpdatedDateTime, [Version]) AS
        (	
			SELECT DISTINCT
				al.Id, 
				al.Name, 
				al.UpdatedDateTime, 
				al.VersionNumber AS [Version]
			FROM ActivityLibrary al
				LEFT JOIN Activity sa ON sa.ActivityLibraryId = al.Id
				JOIN StatusCode sc ON al.[Status] = sc.Code AND sc.Name = 'Public'
				LEFT JOIN [dbo].[WorkflowType] wft1 ON wft1.WorkflowTemplateId = sa.Id
				LEFT JOIN [dbo].[WorkflowType] wft2 ON wft2.PublishingWorkflowId = sa.Id
			WHERE sa.XAML IS NOT NULL
				AND (@SearchText = '""' 
					OR CONTAINS(al.Name, @SearchText) 
					OR CONTAINS(al.[Description], @SearchText)
					OR CONTAINS(al.MetaTags, @SearchText)
					OR CONTAINS(sa.ShortName, @SearchText))
				AND (NOT @FilterType = 0 OR 1 = 1)
				AND (NOT @FilterType = 1 OR wft1.WorkflowTemplateId IS NOT NULL)
				AND (NOT @FilterType = 2 OR wft2.PublishingWorkflowId IS NOT NULL)
				AND (NOT @FilterType = 3 
					OR (wft1.WorkflowTemplateId IS NULL AND wft2.PublishingWorkflowId IS NULL))
				AND (@IsNewest = 0 OR al.Id IN (SELECT MAX(Id) as Id FROM ActivityLibrary JOIN StatusCode ON ActivityLibrary.[Status] = StatusCode.Code AND StatusCode.Name = 'Public' GROUP BY ActivityLibrary.Name))
		),
		ctePage (Id, Name, UpdatedDateTime, [Version], RowNumber) AS
		(
			SELECT Id, 
				Name,
				UpdatedDateTime,
				[Version],
				row_number() OVER (ORDER BY							
					CASE WHEN UPPER(@SortColumn) = 'NAME' AND @SortAscending = 1 THEN Name END ASC,
					CASE WHEN UPPER(@SortColumn) = 'NAME' THEN Name END DESC,
					CASE WHEN UPPER(@SortColumn) = 'VERSION' AND @SortAscending = 1  THEN cast(parsename([Version], 4) as int) END ASC,
					CASE WHEN UPPER(@SortColumn) = 'VERSION' AND @SortAscending = 1  THEN cast(parsename([Version], 3) as int) END ASC,
					CASE WHEN UPPER(@SortColumn) = 'VERSION' AND @SortAscending = 1  THEN cast(parsename([Version], 2) as int) END ASC,
					CASE WHEN UPPER(@SortColumn) = 'VERSION' AND @SortAscending = 1  THEN cast(parsename([Version], 1) as int) END ASC,
					CASE WHEN UPPER(@SortColumn) = 'VERSION' THEN cast(parsename([Version], 4) as int) END DESC,
					CASE WHEN UPPER(@SortColumn) = 'VERSION' THEN cast(parsename([Version], 3) as int) END DESC,
					CASE WHEN UPPER(@SortColumn) = 'VERSION' THEN cast(parsename([Version], 2) as int) END DESC,
					CASE WHEN UPPER(@SortColumn) = 'VERSION' THEN cast(parsename([Version], 1) as int) END DESC,
					CASE WHEN UPPER(@SortColumn) = 'UPDATEDDATE' AND @SortAscending = 1 THEN UpdatedDateTime END ASC,
					CASE WHEN UPPER(@SortColumn) = 'UPDATEDDATE' THEN UpdatedDateTime END DESC)
			FROM cteSearch
		)	
	
		SELECT  ctePage.Id, 
				ctePage.Name, 
				al.InsertedByUserAlias, 
				al.UpdatedByUserAlias, 
				al.InsertedDateTime, 
				ctePage.UpdatedDateTime, 
				ctePage.[Version], 
				'XAML' AS AssetType, 
				@GetTemplates AS IsTemplate, 
				@GetPublishingWorkflows AS IsPublishingWorkflow
		FROM ctePage INNER JOIN ActivityLibrary al ON ctePage.Id=al.Id
		WHERE ctePage.RowNumber BETWEEN @StartIndex AND ((@StartIndex + @PageSize) -1)
		
		SELECT @PageCount AS [PageCount], @PageNumber AS PageNumber
		
	END TRY
	BEGIN CATCH
	EXECUTE [dbo].Error_Handle
	END CATCH		
END
