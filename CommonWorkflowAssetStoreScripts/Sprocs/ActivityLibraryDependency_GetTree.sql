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
// FileName:ActivityLibraryDependency_GetTree.sql
// File description: Get a row in mtblActivityLibraryDependenciesGet.
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   ActivityLibraryDependency_GetTree                        *
**    Auth:   v-stska                                                      *
**    Date:   10/27/2010                                                   *
**                                                                         *
****************************************************************************
**   sproc logic flow: <Optional if complex> 
**   Parameter definition if complex
****************************************************************************
**                  CHANGE HISTORY
****************************************************************************
**	Date:        Author:             Description:
** ____________    ________________    ____________________________________
**   2/7/2011      v-stska             Original implementation
**  11-Nov-2011    v-richt             Bug#86713 - Change error codes to positive numbers
**  03/27/2012	   v-sanja             Exception handling refinements, move validations to business layer.
** *************************************************************************/
CREATE PROCEDURE [dbo].[ActivityLibraryDependency_GetTree]
        @Name varchar(255),
        @Version nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON 
    
    BEGIN TRY       
        DECLARE @TempId bigint
        SELECT @TempId = ID
        FROM dbo.ActivityLibrary
        WHERE Name = @Name AND VersionNumber = @Version AND SoftDelete = 0;
        
        WITH cte (Id, ActivityLibraryID, DependentActivityLibraryId)
        AS
        (
            SELECT Id, ActivityLibraryID, DependentActivityLibraryId
            FROM dbo.ActivityLibraryDependency
            WHERE ActivityLibraryID  = @TEMPID AND SoftDelete = 0
          UNION ALL
            SELECT  e.Id, e.ActivityLibraryID, e.DependentActivityLibraryId
            FROM dbo.ActivityLibraryDependency AS e
            JOIN cte ON e.ActivityLibraryID = cte.DependentActivityLibraryId
        )
        SELECT DISTINCT Id, ActivityLibraryID, DependentActivityLibraryId
        FROM cte;
    END TRY
    BEGIN CATCH
        EXECUTE [dbo].Error_Handle 
    END CATCH
END
GO


