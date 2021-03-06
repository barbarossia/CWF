/****** Object:  StoredProcedure [dbo].[Activity_GetByName]    Script Date: 07/05/2012 10:54:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/**************************************************************************
// Product:  CommonWF
// FileName: Activity_GetByName.sql
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/**************************************************************************
**    Name:   Activity_GetByName                                    
**    Desc:   Create/update an entry in storeActivities.                   
**    Auth:   v-stska                                                      
**    Date:   11/8/2010                                                    
**                                                                         
****************************************************************************
**   sproc logic flow: <Optional if complex> 
**   Parameter definition if complex
****************************************************************************
**                  CHANGE HISTORY
****************************************************************************
**	Date:        Author:             Description:
** ____________  ________________    ____________________________________
**  25-Jan-2012   v-richt            Initial creation. Initial purpose is to 
**                                   support Version Control
** *************************************************************************/
create PROCEDURE [dbo].[Activity_GetByName]
		@Name nvarchar(255),
		@UserName varchar(50)
AS
BEGIN
    SET NOCOUNT ON

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
			al.VersionNumber AS ActivityLibraryVersion,
			al.AuthGroupID,
			ag.Name AS AuthGroupName, 
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
			sa.[Namespace],
			sa.InsertedByUserAlias,
			sa.InsertedDateTime
	FROM [dbo].[Activity] sa
	LEFT JOIN ActivityLibrary al ON sa.ActivityLibraryId = al.Id
	JOIN ActivityCategory ac ON sa.CategoryId = ac.Id
	LEFT JOIN ToolBoxTabName tbtn ON sa.ToolBoxTab = tbtn.Id
	JOIN StatusCode sc ON sa.StatusId = sc.Code
	JOIN WorkflowType wft ON sa.WorkflowTypeId = wft.Id
	LEFT JOIN Icon ic ON sa.IconsId = ic.Id 
	LEFT JOIN AuthorizationGroup ag ON ag.Id = al.AuthGroupId
	WHERE 1=1
		 AND sa.SoftDelete = 0
		 AND (sa.Name = @Name
		      OR sa.ShortName = @Name
			  OR al.Name = @Name)
	ORDER BY sa.InsertedDateTime DESC
END