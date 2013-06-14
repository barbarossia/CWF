 /**************************************************************************
// Product:  CommonWF
// FileName: Activity_Search.sql
// File description: Search row(s) in Activity.
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   Activity_Search                                 
**    Auth:   v-luval                                                      
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
**  04/17/2012      v-laval            Original implementation
** *************************************************************************/
CREATE PROCEDURE [dbo].[Activity_Search]
		@SearchText nvarchar(250),
		@SortColumn varchar(50),
		@SortAscending bit,
		@PageSize int,
		@PageNumber int,
		@FilterOlder bit,
		@FilterByName bit,
		@FilterByDescription bit,
		@FilterByTags bit,
		@FilterByType bit,
		@FilterByVersion bit,
		@FilterByCreator bit
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
				SET @StartIndex = @PageSize * (@PageNumber - 1)
			ELSE
				SET @StartIndex = 1

			IF (@SearchText IS NOT NULL AND @SearchText != '')
				SET @SearchText = '%' + @SearchText + '%'
            
			CREATE TABLE #searchactivityresults 
			( 
				RowNumber bigint, 
				Id int, 
				[GUID] UniqueIdentifier, 
				Name nvarchar(255),
				[Description] nvarchar(250), 
				MetaTags nvarchar(max), 
				[Version] nvarchar(25), 
				WorkFlowTypeName nvarchar(50), 
				InsertedByUserAlias nvarchar(50),
				InsertedDateTime datetime
			)

			IF ((@SearchText IS NOT NULL AND @SearchText != '') AND
				((@FilterByName IS NOT NULL AND @FilterByName != 0) OR
				 (@FilterByDescription IS NOT NULL AND @FilterByDescription != 0) OR
				 (@FilterByTags IS NOT NULL AND @FilterByTags != 0) OR
				 (@FilterByType IS NOT NULL AND @FilterByType != 0) OR
				 (@FilterByVersion IS NOT NULL AND @FilterByVersion != 0) OR
				 (@FilterByCreator IS NOT NULL AND @FilterByCreator != 0)))				
			BEGIN
			
			INSERT INTO #searchactivityresults
			SELECT ROW_NUMBER() OVER 
			(ORDER BY							
						CASE WHEN @SortColumn = 'name' AND @SortAscending = 1 THEN sa.Name END ASC,
						CASE WHEN @SortColumn = 'name' THEN sa.Name END DESC,
						CASE WHEN @SortColumn = 'version' AND @SortAscending = 1 THEN [Version] END ASC,
						CASE WHEN @SortColumn = 'version' THEN [Version] END DESC,
						CASE WHEN @SortColumn = 'workflowtypename' AND @SortAscending = 1 THEN wft.[Name] END ASC,
						CASE WHEN @SortColumn = 'workflowtypename' THEN wft.[Name] END DESC,
						CASE WHEN @SortColumn = 'description'  AND @SortAscending = 1  THEN sa.[Description] END ASC,
						CASE WHEN @SortColumn = 'description' THEN sa.[Description] END,
						CASE WHEN @SortColumn = 'createdby'  AND @SortAscending = 1  THEN sa.[InsertedByUserAlias] END ASC,
						CASE WHEN @SortColumn = 'createdby' THEN sa.[InsertedByUserAlias] END DESC,
						CASE WHEN @SortColumn = 'tags'  AND @SortAscending = 1 THEN sa.[MetaTags] END ASC,
						CASE WHEN @SortColumn = 'tags' THEN sa.[MetaTags] END DESC,
						CASE WHEN @SortColumn = 'id'  AND @SortAscending = 1 THEN sa.[Id] END ASC, 
						CASE WHEN @SortColumn = 'id' THEN sa.[Id] END DESC) AS [RowNumber],
					sa.Id, 
					sa.[GUID], 
					sa.Name,
					sa.[Description], 
					sa.MetaTags, 
					sa.[Version], 
					wft.Name as WorkFlowTypeName, 
					sa.InsertedByUserAlias,
					sa.InsertedDateTime
			FROM [dbo].[Activity] sa
			LEFT JOIN ActivityLibrary al ON sa.ActivityLibraryId = al.Id
			JOIN WorkflowType wft ON sa.WorkflowTypeId = wft.Id
			LEFT JOIN TaskActivity ta ON sa.Id = ta.ActivityId	
			WHERE sa.SoftDelete = 0 AND sa.XAML IS NOT NULL AND sa.XAML != '' 
			AND ta.ActivityId IS NULL
			AND (@FilterOlder IS NULL OR @FilterOlder = 0 OR sa.Id IN (SELECT MAX(Id) as Id FROM Activity GROUP BY Activity.Name)) AND
			(
			(@FilterByName IS NOT NULL AND @FilterByName != 0 AND sa.Name LIKE @SearchText) OR
			(@FilterByDescription IS NOT NULL AND @FilterByDescription != 0 AND sa.[Description] LIKE @SearchText) OR
			(@FilterByTags IS NOT NULL AND @FilterByTags != 0 AND sa.MetaTags LIKE @SearchText) OR 
			(@FilterByCreator IS NOT NULL AND @FilterByCreator != 0 AND sa.InsertedByUserAlias LIKE @SearchText) OR
			(@FilterByVersion IS NOT NULL AND @FilterByVersion != 0 AND sa.[Version] LIKE @SearchText) OR
			(@FilterByType IS NOT NULL AND @FilterByType != 0 AND wft.[Name] LIKE @SearchText))		
					
			END
			
			ELSE
			
			BEGIN
			INSERT INTO #searchactivityresults
			SELECT ROW_NUMBER() OVER 
			(ORDER BY							
						CASE WHEN @SortColumn = 'name' AND @SortAscending = 1 THEN sa.Name END ASC,
						CASE WHEN @SortColumn = 'name' THEN sa.Name END DESC,
						CASE WHEN @SortColumn = 'version' AND @SortAscending = 1 THEN [Version] END ASC,
						CASE WHEN @SortColumn = 'version' THEN [Version] END DESC,
						CASE WHEN @SortColumn = 'workflowtypename' AND @SortAscending = 1 THEN wft.[Name] END ASC,
						CASE WHEN @SortColumn = 'workflowtypename' THEN wft.[Name] END DESC,
						CASE WHEN @SortColumn = 'description'  AND @SortAscending = 1  THEN sa.[Description] END ASC,
						CASE WHEN @SortColumn = 'description' THEN sa.[Description] END,
						CASE WHEN @SortColumn = 'createdby'  AND @SortAscending = 1  THEN sa.[InsertedByUserAlias] END ASC,
						CASE WHEN @SortColumn = 'createdby' THEN sa.[InsertedByUserAlias] END DESC,
						CASE WHEN @SortColumn = 'tags'  AND @SortAscending = 1 THEN sa.[MetaTags] END ASC,
						CASE WHEN @SortColumn = 'tags' THEN sa.[MetaTags] END DESC,
						CASE WHEN @SortColumn = 'id'  AND @SortAscending = 1 THEN sa.[Id] END ASC, 
						CASE WHEN @SortColumn = 'id' THEN sa.[Id] END DESC) AS [RowNumber],
					sa.Id, 
					sa.[GUID], 
					sa.Name,
					sa.[Description], 
					sa.MetaTags, 
					sa.[Version], 
					wft.Name as WorkFlowTypeName, 
					sa.InsertedByUserAlias,
					sa.InsertedDateTime
			FROM [dbo].[Activity] sa
			LEFT JOIN ActivityLibrary al ON sa.ActivityLibraryId = al.Id	
			JOIN WorkflowType wft ON sa.WorkflowTypeId = wft.Id	
			LEFT JOIN TaskActivity ta ON sa.Id = ta.ActivityId	
			WHERE sa.SoftDelete = 0 AND sa.XAML IS NOT NULL AND sa.XAML != '' 
			AND ta.ActivityId IS NULL
			AND (@FilterOlder IS NULL OR @FilterOlder = 0 OR sa.Id IN (SELECT MAX(Id) as Id FROM Activity GROUP BY Activity.Name))			
			END
			
			SELECT *
			FROM #searchactivityresults
			WHERE RowNumber BETWEEN @StartIndex AND ((@StartIndex + @PageSize) -1)

			SELECT COUNT(*) AS Total
			FROM #searchactivityresults

			DROP TABLE #searchactivityresults									
			
	END TRY
	BEGIN CATCH
		EXECUTE [dbo].Error_Handle
	END CATCH
END

GO


