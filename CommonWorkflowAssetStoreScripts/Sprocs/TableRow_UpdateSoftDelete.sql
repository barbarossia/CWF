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

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TableRow_UpdateSoftDelete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[TableRow_UpdateSoftDelete]
GO


/**************************************************************************
// Product:  CommonWF
// FileName: TableRow_UpdateSoftDelete.sql
// File description: Set softdelete value back to 0.
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
-- Author:		<v-ertang>
-- Create date: <2-22-2011>
-- Description:	<Set softdelete value back to 0>
****************************************************************************
**   sproc logic flow: <Optional if complex> 
**   Parameter definition if complex
****************************************************************************
**                  CHANGE HISTORY
****************************************************************************
**	Date:        Author:             Description:
** ____________    ________________    ____________________________________
**  2-22-2011     v-ertang             Original implementation
**  11-Nov-2011   v-richt              Bug#86713 - Change error codes to positive numbers
** *************************************************************************/
CREATE PROCEDURE [dbo].[TableRow_UpdateSoftDelete] 
	@InId bigint,
	@tableName varchar(50)
AS
BEGIN
    if(@tableName = 'Application')
    BEGIN
		Update [dbo].[Application]
		Set SoftDelete = 0
		where Id = @InId
	END
	else if(@tableName = 'ActivityCategory')
	BEGIN
		Update [dbo].[ActivityCategory]
		Set SoftDelete = 0
		where Id = @InId
	END
	else if(@tableName = 'AuthorizationGroup')
	BEGIN
		Update [dbo].[AuthorizationGroup]
		Set SoftDelete = 0
		where Id = @InId
	END
	else if(@tableName = 'ToolBoxTabName')
	BEGIN
		Update [dbo].[ToolBoxTabName]
		Set SoftDelete = 0
		where Id = @InId
	END
	else if(@tableName = 'Activity')
	BEGIN
		Update [dbo].[Activity]
		Set SoftDelete = 0
		where Id = @InId
	END
	else if(@tableName = 'StatusCode')
	BEGIN
		Update [dbo].[StatusCode]
		Set SoftDelete = 0
		where Code = @InId
	END
	else if(@tableName = 'ActivityLibrary')
	BEGIN
		Update [dbo].[ActivityLibrary]
		Set SoftDelete = 0
		where Id = @InId
	END
	else if(@tableName = 'ActivityLibraryDependency')
	BEGIN
		Update [dbo].[ActivityLibraryDependency]
		Set SoftDelete = 0
		where Id = @InId
	END
	else
	BEGIN
		RETURN 55001
	END
END
GO





