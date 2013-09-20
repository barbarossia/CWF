/****** Object:  StoredProcedure [dbo].[Marketplace_Search]    Script Date: 05/16/2013 01:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Marketplace_Search]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Marketplace_Search]
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
		@InAuthGroupName	[dbo].[AuthGroupNameTableType] READONLY ,
		@SearchText nvarchar(250),
		@AssetType tinyint,
		@GetTemplates bit = null,
		@GetPublishingWorkflows bit =null,
		@PageSize int,
		@PageNumber int,
		@SortColumn varchar(50),
		@SortAscending bit,
		@FilterOlder bit,
		@outErrorString nvarchar (300)OUTPUT
AS
BEGIN
	SET NOCOUNT ON	
	
	DECLARE @cObjectName       [sysname]
	SELECT  @cObjectName       = OBJECT_NAME(@@PROCID)
    SET @outErrorString = ''

	DECLARE @Return_Value int
	DECLARE @InEnvironments [dbo].[EnvironmentTableType]
	INSERT @InEnvironments (Name) SELECT [Name] FROM Environment
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


