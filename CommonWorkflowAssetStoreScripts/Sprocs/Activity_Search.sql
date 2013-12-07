/****** Object:  StoredProcedure [dbo].[Activity_Search]    Script Date: 06/05/2013 00:44:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Activity_Search]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Activity_Search]
GO

/****** Object:  StoredProcedure [dbo].[Activity_Search]    Script Date: 06/05/2013 00:44:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO


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
        @InAuthGroupName	[dbo].[AuthGroupNameTableType] READONLY ,
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
		@FilterByCreator bit,
		@InEnvironments	[dbo].[EnvironmentTableType] READONLY ,
		@outErrorString nvarchar (300)OUTPUT
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @cObjectName       [sysname]
	SELECT  @cObjectName       = OBJECT_NAME(@@PROCID)
    SET @outErrorString = ''
    
    DECLARE @Return_Value int
    EXEC @Return_Value = dbo.ValidateSPPermission 
		@InSPName = @cObjectName,
		@InAuthGroupName = @InAuthGroupName,
		@InEnvironments = @InEnvironments,
		@OutErrorString =  @OutErrorString output
	IF (@Return_Value > 0)
	BEGIN		    
        RETURN @Return_Value
	END

	BEGIN TRY
	    
			IF (@SortColumn IS NULL OR @SortColumn = '')
			SET @SortColumn = 'id'

			IF (@PageSize IS NULL OR @PageSize < 1)
			SET @PageSize = 10

			IF (@PageNumber IS NULL OR @PageNumber < 1)
			SET @PageNumber = 1

			DECLARE @StartIndex int
			IF @PageNumber > 1 
				SET @StartIndex = @PageSize * (@PageNumber - 1) + 1
			ELSE
				SET @StartIndex = 1

			IF (@SearchText IS NOT NULL AND @SearchText != '')
				SET @SearchText = '%' + Replace(@SearchText,'_', '[_]') + '%'
			
			declare @Environment table (Id int)
			insert into @Environment(Id)
			select E.Id from Environment E
			join @InEnvironments IE on E.[Name] = IE.[Name]

			DECLARE @TotalCount INT

			IF ((@SearchText IS NOT NULL AND @SearchText != '') AND
				((@FilterByName IS NOT NULL AND @FilterByName != 0) OR
				 (@FilterByDescription IS NOT NULL AND @FilterByDescription != 0) OR
				 (@FilterByTags IS NOT NULL AND @FilterByTags != 0) OR
				 (@FilterByType IS NOT NULL AND @FilterByType != 0) OR
				 (@FilterByVersion IS NOT NULL AND @FilterByVersion != 0) OR
				 (@FilterByCreator IS NOT NULL AND @FilterByCreator != 0)))				
			BEGIN
				;WITH cteCount(Id) AS 
				(
				SELECT DISTINCT sa.Id
				FROM [dbo].[Activity] sa
				LEFT JOIN ActivityLibrary al ON sa.ActivityLibraryId = al.Id
				JOIN WorkflowType wft ON sa.WorkflowTypeId = wft.Id
				LEFT JOIN TaskActivity ta ON sa.Id = ta.ActivityId
				JOIN @Environment E ON sa.Environment = E.Id
				WHERE sa.SoftDelete = 0 AND sa.XAML IS NOT NULL 
				AND ta.ActivityId IS NULL
				AND (@FilterOlder IS NULL OR @FilterOlder = 0 OR not exists 
				(SELECT 1 FROM Activity where Name = sa.Name and Environment = sa.Environment and SoftDelete = 0  and Id > sa.Id )) AND
				(
				(@FilterByName IS NOT NULL AND @FilterByName != 0 AND sa.Name LIKE @SearchText) OR
				(@FilterByDescription IS NOT NULL AND @FilterByDescription != 0 AND sa.[Description] LIKE @SearchText) OR
				(@FilterByTags IS NOT NULL AND @FilterByTags != 0 AND sa.MetaTags LIKE @SearchText) OR 
				(@FilterByCreator IS NOT NULL AND @FilterByCreator != 0 AND sa.InsertedByUserAlias LIKE @SearchText) OR
				(@FilterByVersion IS NOT NULL AND @FilterByVersion != 0 AND sa.[Version] LIKE @SearchText) OR
				(@FilterByType IS NOT NULL AND @FilterByType != 0 AND wft.[Name] LIKE @SearchText))			
				)

				SELECT @TotalCount= COUNT(Id) FROM cteCount
				IF @TotalCount = 0
				BEGIN 				
					RETURN
				END
				ELSE
				BEGIN
					;WITH cteSearch(Id,
					[GUID],
					Name,
					[Description],	
					MetaTags, 
					[Version], 
					WorkFlowTypeName, 
					InsertedByUserAlias,
					InsertedDateTime,
					Environment) AS
					(	SELECT sa.Id, 
						sa.[GUID], 
						sa.Name,
						sa.[Description], 
						sa.MetaTags, 
						sa.[Version], 
						wft.Name as WorkFlowTypeName, 
						sa.InsertedByUserAlias,
						sa.InsertedDateTime,
						sa.Environment
				FROM [dbo].[Activity] sa
				LEFT JOIN ActivityLibrary al ON sa.ActivityLibraryId = al.Id
				JOIN WorkflowType wft ON sa.WorkflowTypeId = wft.Id
				LEFT JOIN TaskActivity ta ON sa.Id = ta.ActivityId
				JOIN @Environment E ON sa.Environment = E.Id
				WHERE sa.SoftDelete = 0 AND sa.XAML IS NOT NULL
				AND ta.ActivityId IS NULL
				AND (@FilterOlder IS NULL OR @FilterOlder = 0 OR not exists 
				(SELECT 1 FROM Activity where Name = sa.Name and Environment = sa.Environment and SoftDelete = 0  and Id > sa.Id )) AND
				(
				(@FilterByName IS NOT NULL AND @FilterByName != 0 AND sa.Name LIKE @SearchText) OR
				(@FilterByDescription IS NOT NULL AND @FilterByDescription != 0 AND sa.[Description] LIKE @SearchText) OR
				(@FilterByTags IS NOT NULL AND @FilterByTags != 0 AND sa.MetaTags LIKE @SearchText) OR 
				(@FilterByCreator IS NOT NULL AND @FilterByCreator != 0 AND sa.InsertedByUserAlias LIKE @SearchText) OR
				(@FilterByVersion IS NOT NULL AND @FilterByVersion != 0 AND sa.[Version] LIKE @SearchText) OR
				(@FilterByType IS NOT NULL AND @FilterByType != 0 AND wft.[Name] LIKE @SearchText))
				),
				ctePage(
					RowNumber,
					Id,
					[GUID],
					Name,
					[Description],	
					MetaTags, 
					[Version], 
					WorkFlowTypeName, 
					InsertedByUserAlias,
					InsertedDateTime,
					Environment) AS
					(
					SELECT ROW_NUMBER() OVER 
					(ORDER BY							
							CASE WHEN @SortColumn = 'name' AND @SortAscending = 1 THEN Name END ASC,
							CASE WHEN @SortColumn = 'name' THEN Name END DESC,
							CASE WHEN @SortColumn = 'version' AND @SortAscending = 1 THEN [Version] END ASC,
							CASE WHEN @SortColumn = 'version' THEN [Version] END DESC,
							CASE WHEN @SortColumn = 'workflowtypename' AND @SortAscending = 1 THEN WorkFlowTypeName END ASC,
							CASE WHEN @SortColumn = 'workflowtypename' THEN WorkFlowTypeName END DESC,
							CASE WHEN @SortColumn = 'description'  AND @SortAscending = 1  THEN [Description] END ASC,
							CASE WHEN @SortColumn = 'description' THEN [Description] END,
							CASE WHEN @SortColumn = 'ininsertedbyuseralias'  AND @SortAscending = 1  THEN [InsertedByUserAlias] END ASC,
							CASE WHEN @SortColumn = 'ininsertedbyuseralias' THEN [InsertedByUserAlias] END DESC,
							CASE WHEN @SortColumn = 'metatags'  AND @SortAscending = 1 THEN [MetaTags] END ASC,
							CASE WHEN @SortColumn = 'metatags' THEN [MetaTags] END DESC,
							CASE WHEN @SortColumn = 'id'  AND @SortAscending = 1 THEN [Id] END ASC, 
							CASE WHEN @SortColumn = 'id' THEN [Id] END DESC,
							CASE WHEN @SortColumn = 'environment' AND @SortAscending = 1 THEN [Environment] END ASC, 
							CASE WHEN @SortColumn = 'environment' THEN [Environment] END DESC) AS [RowNumber],
						s.Id, 
						s.[GUID], 
						s.Name,
						s.[Description], 
						s.MetaTags, 
						s.[Version], 
						s.WorkFlowTypeName, 
						s.InsertedByUserAlias,
						s.InsertedDateTime,
						s.Environment
				FROM cteSearch s
				)
				SELECT s.Id, 
						s.[GUID], 
						s.Name,
						s.[Description], 
						s.MetaTags, 
						s.[Version], 
						s.WorkFlowTypeName, 
						s.InsertedByUserAlias,
						s.InsertedDateTime,
						E.[Name] AS Environment
				FROM ctePage s
				JOIN Environment E ON s.Environment = E.Id
				WHERE RowNumber BETWEEN @StartIndex AND ((@StartIndex + @PageSize) -1)		
					SELECT @TotalCount AS Total
				END													
			END
		ELSE
		BEGIN
			;WITH cteCount(Id) AS 
			(
			SELECT DISTINCT sa.Id
			FROM [dbo].[Activity] sa
			LEFT JOIN ActivityLibrary al ON sa.ActivityLibraryId = al.Id
			JOIN WorkflowType wft ON sa.WorkflowTypeId = wft.Id
			LEFT JOIN TaskActivity ta ON sa.Id = ta.ActivityId
			JOIN @Environment E ON sa.Environment = E.Id
			WHERE sa.SoftDelete = 0 AND sa.XAML IS NOT NULL 
			AND ta.ActivityId IS NULL
			AND (@FilterOlder IS NULL OR @FilterOlder = 0 OR not exists 
				(SELECT 1 FROM Activity where Name = sa.Name and Environment = sa.Environment and SoftDelete = 0  and Id > sa.Id ))			
			)

			SELECT @TotalCount= COUNT(Id) FROM cteCount
			IF @TotalCount = 0
			BEGIN 				
				RETURN
			END
			ELSE
			BEGIN
				;WITH cteSearch(Id,
				[GUID],
				Name,
				[Description],	
				MetaTags, 
				[Version], 
				WorkFlowTypeName, 
				InsertedByUserAlias,
				InsertedDateTime,
				Environment) AS
				(	SELECT sa.Id, 
					sa.[GUID], 
					sa.Name,
					sa.[Description], 
					sa.MetaTags, 
					sa.[Version], 
					wft.Name as WorkFlowTypeName, 
					sa.InsertedByUserAlias,
					sa.InsertedDateTime,
					sa.Environment
			FROM [dbo].[Activity] sa
			LEFT JOIN ActivityLibrary al ON sa.ActivityLibraryId = al.Id
			JOIN WorkflowType wft ON sa.WorkflowTypeId = wft.Id
			LEFT JOIN TaskActivity ta ON sa.Id = ta.ActivityId
			JOIN @Environment E ON sa.Environment = E.Id
			WHERE sa.SoftDelete = 0 AND sa.XAML IS NOT NULL
			AND ta.ActivityId IS NULL
			AND (@FilterOlder IS NULL OR @FilterOlder = 0 OR not exists 
				(SELECT 1 FROM Activity where Name = sa.Name and Environment = sa.Environment and SoftDelete = 0  and Id > sa.Id ))
			),
			ctePage(
				RowNumber,
				Id,
				[GUID],
				Name,
				[Description],	
				MetaTags, 
				[Version], 
				WorkFlowTypeName, 
				InsertedByUserAlias,
				InsertedDateTime,
				Environment) AS
				(
				SELECT ROW_NUMBER() OVER 
				(ORDER BY							
							CASE WHEN @SortColumn = 'name' AND @SortAscending = 1 THEN Name END ASC,
							CASE WHEN @SortColumn = 'name' THEN Name END DESC,
							CASE WHEN @SortColumn = 'version' AND @SortAscending = 1 THEN [Version] END ASC,
							CASE WHEN @SortColumn = 'version' THEN [Version] END DESC,
							CASE WHEN @SortColumn = 'workflowtypename' AND @SortAscending = 1 THEN WorkFlowTypeName END ASC,
							CASE WHEN @SortColumn = 'workflowtypename' THEN WorkFlowTypeName END DESC,
							CASE WHEN @SortColumn = 'description'  AND @SortAscending = 1  THEN [Description] END ASC,
							CASE WHEN @SortColumn = 'description' THEN [Description] END,
							CASE WHEN @SortColumn = 'ininsertedbyuseralias'  AND @SortAscending = 1  THEN [InsertedByUserAlias] END ASC,
							CASE WHEN @SortColumn = 'ininsertedbyuseralias' THEN [InsertedByUserAlias] END DESC,
							CASE WHEN @SortColumn = 'metatags'  AND @SortAscending = 1 THEN [MetaTags] END ASC,
							CASE WHEN @SortColumn = 'metatags' THEN [MetaTags] END DESC,
							CASE WHEN @SortColumn = 'id'  AND @SortAscending = 1 THEN [Id] END ASC, 
							CASE WHEN @SortColumn = 'id' THEN [Id] END DESC,
							CASE WHEN @SortColumn = 'environment' AND @SortAscending = 1 THEN [Environment] END ASC, 
							CASE WHEN @SortColumn = 'environment' THEN [Environment] END DESC) AS [RowNumber],
					s.Id, 
					s.[GUID], 
					s.Name,
					s.[Description], 
					s.MetaTags, 
					s.[Version], 
					s.WorkFlowTypeName, 
					s.InsertedByUserAlias,
					s.InsertedDateTime,
					s.Environment
			FROM cteSearch s
			)
			SELECT s.Id, 
					s.[GUID], 
					s.Name,
					s.[Description], 
					s.MetaTags, 
					s.[Version], 
					s.WorkFlowTypeName, 
					s.InsertedByUserAlias,
					s.InsertedDateTime,
					E.[Name] AS Environment
			FROM ctePage s
			JOIN Environment E ON s.Environment = E.Id
			WHERE RowNumber BETWEEN @StartIndex AND ((@StartIndex + @PageSize) -1)		
				SELECT @TotalCount AS Total
			END
		END
	END TRY
	BEGIN CATCH
		EXECUTE [dbo].Error_Handle
	END CATCH
END


GO

GRANT EXECUTE ON [dbo].[Activity_Search] TO [MarketplaceService];
GO


