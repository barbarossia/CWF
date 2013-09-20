/****** Object:  StoredProcedure [dbo].[Marketplace_SearchExecutableItems]    Script Date: 07/31/2012 13:36:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Marketplace_SearchExecutableItems]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Marketplace_SearchExecutableItems]
GO


/**************************************************************************
// Product:  CommonWF
// FileName: Marketplace_SearchExecutableItems.sql
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   Marketplace_SearchExecutableItems                            *
**    Desc:   Search marketplace.                                          *
**    Auth:   v-bobzh                                                      *
**    Date:   05/31/2012                                                    *
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
create PROCEDURE [dbo].[Marketplace_SearchExecutableItems]
		@SearchText NVARCHAR(250),
		@PageSize INT,
		@PageNumber INT,
		@SortColumn VARCHAR(50),
		@SortAscending BIT,
		@IsNewest bit
AS
BEGIN
	SET NOCOUNT ON	
	BEGIN TRY	

		DECLARE @StartIndex INT
		IF @PageNumber > 1 
		SET @StartIndex = @PageSize * (@PageNumber - 1)
		ELSE
		SET @StartIndex = 1	
		
		DECLARE @TotalCount INT
		DECLARE @PageCount INT

		IF ISNULL(@SearchText,'') = ''		
			SET @SearchText	=''

		IF (@SearchText != '')
			SET @SearchText = '%' + @SearchText + '%'		
			
		;WITH cteCount (Id) AS
        (	
			SELECT DISTINCT al.Id
			FROM ActivityLibrary al
				LEFT JOIN Activity sa ON sa.ActivityLibraryId = al.Id
				JOIN StatusCode sc ON al.[Status] = sc.Code AND sc.Name = 'Public'
			WHERE al.[Executable] IS NOT NULL 
				AND ((@SearchText = '') 
					OR (al.Name LIKE @SearchText OR 
					    al.[Description] LIKE @SearchText OR 
					    al.MetaTags LIKE @SearchText OR 
					    sa.ShortName LIKE @SearchText))
				AND (@IsNewest = 0 OR not exists (SELECT 1 FROM ActivityLibrary JOIN StatusCode ON ActivityLibrary.[Status] = StatusCode.Code AND StatusCode.Name = 'Public' and ActivityLibrary.Name = al.Name and ActivityLibrary.Environment = al.Environment and ActivityLibrary.Id > al.Id))
				AND al.SoftDelete = 0
		)
		SELECT @TotalCount = COUNT(*) FROM cteCount
		IF @TotalCount > 0 
		BEGIN
			SET  @PageCount = @TotalCount / @PageSize + 1
			IF @PageNumber > @PageCount
			BEGIN
				SET @PageNumber = @PageCount
				SET @StartIndex = @PageSize * (@PageNumber - 1)
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
			WHERE al.[Executable] IS NOT NULL 
				AND ((@SearchText = '') 
					OR (al.Name LIKE @SearchText OR 
					    al.[Description] LIKE @SearchText OR 
					    al.MetaTags LIKE @SearchText OR 
					    sa.ShortName LIKE @SearchText))
				AND (@IsNewest = 0 OR not exists (SELECT 1 FROM ActivityLibrary JOIN StatusCode ON ActivityLibrary.[Status] = StatusCode.Code AND StatusCode.Name = 'Public' and ActivityLibrary.Name = al.Name and ActivityLibrary.Environment = al.Environment and ActivityLibrary.Id > al.Id))
				AND al.SoftDelete = 0
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
							CASE WHEN UPPER(@SortColumn) = 'VERSION' AND @SortAscending = 1  THEN CASE WHEN ISNUMERIC(parsename([version], 4)) = 1 THEN CAST(parsename([version], 4) AS INT) ELSE 0 END END ASC,
							CASE WHEN UPPER(@SortColumn) = 'VERSION' AND @SortAscending = 1  THEN CASE WHEN ISNUMERIC(parsename([version], 3)) = 1 THEN CAST(parsename([version], 3) AS INT) ELSE 0 END END ASC,
							CASE WHEN UPPER(@SortColumn) = 'VERSION' AND @SortAscending = 1  THEN CASE WHEN ISNUMERIC(parsename([version], 3)) = 1 THEN CAST(parsename([version], 2) AS INT) ELSE 0 END END ASC,
							CASE WHEN UPPER(@SortColumn) = 'VERSION' AND @SortAscending = 1  THEN CASE WHEN ISNUMERIC(parsename([version], 1)) = 1 THEN CAST(parsename([version], 1) AS INT) ELSE 0 END END ASC,
							CASE WHEN UPPER(@SortColumn) = 'VERSION' THEN CASE WHEN ISNUMERIC(parsename([version], 4)) = 1 THEN CAST(parsename([version], 4) AS INT) ELSE 0 END END DESC,
							CASE WHEN UPPER(@SortColumn) = 'VERSION' THEN CASE WHEN ISNUMERIC(parsename([version], 3)) = 1 THEN CAST(parsename([version], 3) AS INT) ELSE 0 END END DESC,
							CASE WHEN UPPER(@SortColumn) = 'VERSION' THEN CASE WHEN ISNUMERIC(parsename([version], 2)) = 1 THEN CAST(parsename([version], 2) AS INT) ELSE 0 END END DESC,
							CASE WHEN UPPER(@SortColumn) = 'VERSION' THEN CASE WHEN ISNUMERIC(parsename([version], 1)) = 1 THEN CAST(parsename([version], 1) AS INT) ELSE 0 END END DESC,
							CASE WHEN UPPER(@SortColumn) = 'UPDATEDDATE' AND @SortAscending = 1 THEN UpdatedDateTime END ASC,
							CASE WHEN UPPER(@SortColumn) = 'UPDATEDDATE' THEN UpdatedDateTime END DESC)
		FROM cteSearch)		
		SELECT  ctePage.Id, 
				ctePage.Name, 
				al.InsertedByUserAlias, 
				al.UpdatedByUserAlias, 
				al.InsertedDateTime, 
				ctePage.UpdatedDateTime, 
				ctePage.[Version], 
				'Executable' AS AssetType, 
				NULL AS IsTemplate, 
				NULL AS IsPublishingWorkflow,
				E.[Name] AS Environment
			FROM ctePage INNER JOIN ActivityLibrary al ON ctePage.Id = al.Id
			JOIN Environment E ON al.Environment = E.Id 
		WHERE ctePage.RowNumber BETWEEN @StartIndex AND ((@StartIndex + @PageSize) -1)
		SELECT @PageCount AS [PageCount], @PageNumber AS PageNumber
	
	END TRY
	BEGIN CATCH
	EXECUTE [dbo].Error_Handle
	END CATCH		
END
