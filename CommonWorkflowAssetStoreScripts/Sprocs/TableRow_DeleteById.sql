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

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TableRow_DeleteById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[TableRow_DeleteById]
GO

/**************************************************************************
// Product:  CommonWF
// FileName: TableRow_DeleteById.sql
// File description: Deletes a row in the table inputted.
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
-- Author:		<v-ertang>
-- Create date: <2-28-2011>
-- Description:	<Delete row with this id>
****************************************************************************
**   sproc logic flow: <Optional if complex> 
**   Parameter definition if complex
****************************************************************************
**                  CHANGE HISTORY
****************************************************************************
**	Date:        Author:             Description:
** ____________    ________________    ____________________________________
**  2-28-2011      v-ertang            Original implementation
**  11-Nov-2011    v-richt             Bug#86713 - Change error codes to positive numbers
** *************************************************************************/
CREATE PROCEDURE [dbo].[TableRow_DeleteById] 
	@InId BigInt,
	@tableName varchar(50)
AS
BEGIN 
	if(@tableName = 'Application')
	BEGIN
		DELETE FROM [dbo].[Application]
		WHERE Id = @InId
	END
	else if(@tableName = 'ActivityCategory')
	BEGIN
		DELETE FROM [dbo].[ActivityCategory]
		WHERE Id = @InId
	END
	else if(@tableName = 'AuthorizationGroup')
	BEGIN
		DELETE FROM [dbo].[AuthorizationGroup]
		WHERE Id = @InId
	END
	else if(@tableName = 'ToolBoxTabName')
	BEGIN
		DELETE FROM [dbo].[ToolBoxTabName]
		WHERE Id = @InId
	END
	else if(@tableName = 'Activity')
	BEGIN
		DELETE FROM [dbo].[Activity]
		WHERE Id = @InId
	END
	else if(@tableName = 'StatusCode')
	BEGIN
		DELETE FROM [dbo].[StatusCode]
		WHERE Code = @InId
	END
	else if(@tableName = 'ActivityLibrary')
	BEGIN
		DELETE FROM [dbo].[ActivityLibrary]
		WHERE Id = @InId
	END
	else if(@tableName = 'ActivityLibraryDependency')
	BEGIN
		DELETE FROM [dbo].[ActivityLibraryDependency]
		WHERE Id = @InId
	END
	else
	BEGIN
		RETURN 55001
	END
END
GO

