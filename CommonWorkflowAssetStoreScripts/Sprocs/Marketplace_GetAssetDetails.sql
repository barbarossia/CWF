/****** Object:  StoredProcedure [dbo].[Marketplace_GetAssetDetails]    Script Date: 06/06/2012 15:34:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Marketplace_GetAssetDetails]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Marketplace_GetAssetDetails]
GO


 /**************************************************************************
// Product:  CommonWF
// FileName: Marketplace_GetAssetDetails.sql
// File description: Get a row(s) in etblActivityLibraries.
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   Marketplace_GetAssetDetails                                  
**    Auth:   v-bobzh                                                      
**    Date:   05/31/2012                                                   
**                                                                         
****************************************************************************
**   sproc logic flow: <Optional if complex> 
**   Parameter definition if complex
****************************************************************************
**                  CHANGE HISTORY
****************************************************************************
**	Date:        Author:             Description:
** ____________    ________________    ____________________________________
**  05/31/2012      v-bobzh             Original implementation
** *************************************************************************/
CREATE PROCEDURE [dbo].[Marketplace_GetAssetDetails]
		@Id bigint,
		@AssetType tinyint
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY
		IF @AssetType = 2  -- 2 means Project
		BEGIN				
			SELECT DISTINCT al.[Id], 
				al.[Name], 
				ac.[Name] AS CategoryName,   
				al.[Description], 
				al.[MetaTags],
				icon.Name AS ThumbnailUrl
			FROM [dbo].[ActivityLibrary] al
				LEFT JOIN [dbo].[Activity] sa ON al.Id = sa.ActivityLibraryId
				LEFT JOIN [dbo].[ActivityCategory] ac ON sa.CategoryId = ac.Id
				LEFT JOIN [dbo].[Icon] icon ON sa.IconsId = icon.Id
			WHERE al.Id = @Id
				AND al.SoftDelete = 0			
		END
		ELSE IF @AssetType = 1 --1 means Activities
		BEGIN		
			SELECT DISTINCT al.[Id], 
				al.[Name], 
				ac.[Name] AS CategoryName,   
				al.[Description], 
				al.[MetaTags],
				icon.Name AS ThumbnailUrl
			FROM [dbo].[ActivityLibrary] al
				LEFT JOIN [dbo].[Activity] sa ON al.Id = sa.ActivityLibraryId
				LEFT JOIN [dbo].[ActivityCategory] ac ON sa.CategoryId = ac.Id
				LEFT JOIN [dbo].[Icon] icon ON sa.IconsId = icon.Id
			WHERE al.Id = @Id
				AND al.SoftDelete = 0	
					
			SELECT 
				sa.Id AS ActivityId,
				sa.Name AS ActivityName,
				sa.[Version] AS [Version]
			FROM [dbo].[ActivityLibrary] al
				JOIN [dbo].[Activity] sa ON al.Id = sa.ActivityLibraryId
			WHERE al.Id = @Id
				AND al.SoftDelete = 0
		END		
	END TRY
	BEGIN CATCH
		EXECUTE [dbo].Error_Handle 
	END CATCH
END


GO


