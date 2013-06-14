
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
// FileName: Activity_GetByActivityLibrary.sql
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   Activity_GetByActivityLibrary                   *
**    Desc:   Get an entry(s) in storeActivities by the ActivityLibrary.   *
**    Auth:   v-stska                                                      *
**    Date:   11/8/2010                                                    *
**                                                                         *
****************************************************************************
**   sproc logic flow: <Optional if complex> 
**   Parameter definition if complex
****************************************************************************
**                  CHANGE HISTORY
****************************************************************************
**	Date:        Author:             Description:
** ____________    ________________    ____________________________________
**  12/20/2010     v-stska             Original implementation
**  5/29/2011      v-stska             Fix null AL & SA tables
**  11-Nov-2011	   v-richt             Bug#86713 - Change error codes to positive numbers
**  03/28/2012	   v-sanja             Exception handling refinements, move validations to business layer.
** *************************************************************************/
CREATE PROCEDURE [dbo].[Activity_GetByActivityLibrary]
		@Id BIGINT,
		@Name NVARCHAR(255),
		@Version NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON
    	
	BEGIN TRY		
		--When Id is not specified, determine ID from Name and Version.  
		--Negative ID is considered a user specified ID and it will ultimately yield no results.
		IF (@Id = 0) 
		BEGIN
			SELECT @Id = Id
			FROM [dbo].[ActivityLibrary]
			WHERE Name = @Name AND VersionNumber = @Version AND SoftDelete = 0		
		END	
		
		SELECT sa.Id, 
					sa.[GUID], 
					sa.Name,
					sa.ShortName,
					sa.[Description], 
					sa.MetaTags, 
					sa.IconsId, 
					ic.[Name] AS iconsName,
					sa.IsSwitch, 
					sa.IsService, 
					sa.ActivityLibraryId, 
					al.Name AS ActivityLibraryName, 
					sa.IsUxActivity, 
					sa.CategoryId, 
					ac.Name as ActivityCategoryName, 
					tbtn.Name as ToolBoxtabName, 
					sa.ToolBoxtab, 
					sa.IsToolBoxActivity, 
					sa.[Version], 
					sa.StatusId, 
					sc.Name AS StatusCodeName, 
					sa.WorkflowTypeId,
					wft.Name as WorkFlowTypeName, 
					sa.Locked, 
					sa.LockedBy, 
					sa.IsCodeBeside, 
					sa.XAML, 
					sa.DeveloperNotes, 
					sa.BaseType, 
					sa.[Namespace]
			FROM [dbo].[Activity] sa
			JOIN ActivityLibrary al ON sa.ActivityLibraryId = al.Id
			JOIN ActivityCategory ac ON sa.CategoryId = ac.Id
			JOIN ToolboxTabName tbtn ON sa.ToolBoxTab = tbtn.Id
			JOIN StatusCode sc ON sa.StatusId = sc.Code
			JOIN WorkflowType wft ON sa.WorkflowTypeId = wft.Id
			JOIN Icon ic ON sa.IconsId = ic.Id 
			WHERE sa.ActivityLibraryId = @Id
				AND sa.SoftDelete = 0
	END TRY
	BEGIN CATCH
		EXECUTE [dbo].Error_Handle 
	END CATCH
END
GO
