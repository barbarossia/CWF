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

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ActivityLibrary_Get]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ActivityLibrary_Get]
GO

 /**************************************************************************
// Product:  CommonWF
// FileName: ActivityLibrary_Get.sql
// File description: Get a row(s) in etblActivityLibraries.
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   ActivityLibrary_Get                                  
**    Auth:   v-stska                                                      
**    Date:   10/27/2010                                                   
**                                                                         
****************************************************************************
**   sproc logic flow: <Optional if complex> 
**   Parameter definition if complex
****************************************************************************
**                  CHANGE HISTORY
****************************************************************************
**	Date:        Author:             Description:
** ____________    ________________    ____________________________________
**  11/23/2010      v-stska             Original implementation
**  12/21/2010      v-stska             allow name version
**  12/22/2010      v-stska             Eliminate dynamic SQL
**  05/29/2011      v-stska             Fix count() = 0 return
**  11/11/2011	    v-richt             Bug#86713 - Change error codes to positive numbers
**  02/17/2012		v-sanja				Eliminated validation code, moved them to the business layer.
** *************************************************************************/
CREATE PROCEDURE [dbo].[ActivityLibrary_Get]
		@inCaller nvarchar(50),
		@inCallerversion nvarchar (50),
		@InId bigint,
		@InGUID varchar(50),
		@InName varchar(255),
		@InVersion nvarchar(50)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY
		IF(@InId <> 0) -- 0 means user is not intending to search by ID.  Any other value means to prioritize the search based on ID.
		BEGIN				
			SELECT al.[Id], al.[GUID], al.[Name], al.[AuthGroupId], ag.Name AS AuthGroupName, al.[Category], al.[CategoryId], 
				ac.[Name] AS CategoryName, al.[Executable], al.[HasActivities], al.[Description], al.[ImportedBy], 
				al.[VersionNumber], al.[Status], sc.Name as StatusName, al.[MetaTags], al.[FriendlyName], al.[ReleaseNotes], E.[Name] AS Environment
			FROM [dbo].[ActivityLibrary] al
				JOIN [dbo].[AuthorizationGroup] ag ON ag.Id = al.AuthGroupId
				JOIN [dbo].[StatusCode] sc on al.Status = sc.Code
				JOIN Environment E ON al.Environment = E.Id
				LEFT JOIN [dbo].[ActivityCategory] ac on al.CategoryId = ac.Id
			WHERE al.Id = @InId
				AND al.SoftDelete = 0
		END
		ELSE
		IF (@InGUID <> '00000000-0000-0000-0000-000000000000')
		BEGIN	
			SELECT al.[Id], al.[GUID], al.[Name], al.[AuthGroupId], ag.Name AS AuthGroupName, al.[Category], al.[CategoryId], 
				ac.[Name] AS CategoryName, al.[Executable], al.[HasActivities], al.[Description], al.[ImportedBy],
				al.[VersionNumber], al.[Status], sc.Name as StatusName, al.[MetaTags], al.[FriendlyName], al.[ReleaseNotes], E.[Name] AS Environment
			FROM [dbo].[ActivityLibrary] al
				JOIN [dbo].[AuthorizationGroup] ag ON ag.Id = al.AuthGroupId
				JOIN [dbo].[StatusCode] sc on al.Status = sc.Code
				JOIN Environment E ON al.Environment = E.Id 
				LEFT JOIN [dbo].[ActivityCategory] ac on al.CategoryId = ac.Id
			WHERE al.GUID = @InGUID
				AND al.SoftDelete = 0
		END
		ELSE
		BEGIN
			DECLARE @InNameExists BIT, @InVersionExists BIT
			
			IF (@InName IS NULL or @InName = '')
				SET @InNameExists = 0
			ELSE
				SET @InNameExists = 1
				
			IF (@InVersion IS NULL or @InVersion = '')
				SET @InVersionExists = 0
			ELSE
				SET @InVersionExists = 1
	
			-- Find by name & version if both are provided.  Find by either name or version if only
			-- one of them is provided.  Return all activity libraries if none is provided.
			SELECT al.[Id], al.[GUID], al.[Name], al.[AuthGroupId], ag.Name AS AuthGroupName, al.[Category], al.[CategoryId], 
				ac.[Name] AS CategoryName, al.[Executable], al.[HasActivities], al.[Description], al.[ImportedBy],
				al.[VersionNumber], al.[Status], sc.Name as StatusName, al.[MetaTags], al.[FriendlyName], al.[ReleaseNotes], E.[Name] AS Environment
			FROM [dbo].[ActivityLibrary] al
				JOIN [dbo].[AuthorizationGroup] ag ON ag.Id = al.AuthGroupId
				JOIN [dbo].[StatusCode] sc on al.Status = sc.Code
				JOIN Environment E ON al.Environment = E.Id 
				LEFT JOIN [dbo].[ActivityCategory] ac on al.CategoryId = ac.Id
			WHERE (@InName = al.Name OR @InNameExists <> 1) AND
				(@InVersion = al.VersionNumber OR @InVersionExists <> 1)
					AND al.SoftDelete = 0
		END
	END TRY
	BEGIN CATCH
		EXECUTE [dbo].Error_Handle 
	END CATCH
END
GO
