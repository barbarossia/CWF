/****** Object:  StoredProcedure [dbo].[Marketplace_Search]    Script Date: 06/06/2012 15:34:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



/**************************************************************************
// Product:  CommonWF
// FileName: Marketplace_Search.sql
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   Marketplace_Search                                            *
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
CREATE PROCEDURE [dbo].[Marketplace_Search]
		@SearchText nvarchar(250),
		@AssetType tinyint,
		@GetTemplates bit = null,
		@GetPublishingWorkflows bit =null,
		@PageSize int,
		@PageNumber int,
		@SortColumn varchar(50),
		@SortAscending bit,
		@FilterOlder bit
AS
BEGIN
	SET NOCOUNT ON	
	BEGIN TRY	
		IF @AssetType = 0
			EXEC  Marketplace_SearchAllItems @SearchText, @GetTemplates, @GetPublishingWorkflows, @PageSize, @PageNumber, @SortColumn, @SortAscending, @FilterOlder
		ELSE IF @AssetType= 1	
			EXEC  Marketplace_SearchExecutableItems @SearchText, @PageSize, @PageNumber, @SortColumn, @SortAscending, @FilterOlder
		ELSE IF @AssetType= 2
			EXEC  Marketplace_SearchXamlItems @SearchText, @GetTemplates, @GetPublishingWorkflows, @PageSize, @PageNumber, @SortColumn, @SortAscending, @FilterOlder
	END TRY
	BEGIN CATCH
	EXECUTE [dbo].Error_Handle
	END CATCH		
END


GO


