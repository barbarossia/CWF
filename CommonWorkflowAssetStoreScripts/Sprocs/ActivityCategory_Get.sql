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
// FileName: ActivityCategory_Get.sql
// File description: Get a row in ltblActivityCategory.
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   ActivityCategory_Get                                   *
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
**  11/23/2010      v-stska             Original implementation
**  12/12/2010      v-stska             Update to NEW3PrototypeAssetStore
**  2/13/2011       v-stska             Add @OutError logic
**  11-Nov-2011	    v-richt             Bug#86713 - Change error codes to positive numbers
**  03/27/2012	    v-sanja             Exception handling refinements, move validations to business layer.
** *************************************************************************/
CREATE PROCEDURE [dbo].[ActivityCategory_Get]
        @Id BIGINT,
        @Name VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON
    
    DECLARE @TempId BIGINT
    DECLARE @TempName nvarchar (50)
    DECLARE @IdExists bit
    DECLARE @NameExists bit
    SET @IdExists = 0
    SET @NameExists = 0

    BEGIN TRY
        IF (@Id != 0)
        BEGIN
            SELECT @TempId = ID
            FROM [dbo].[ActivityCategory]
            WHERE ID = @Id AND SoftDelete = 0
            
            IF (@TempId IS NOT NULL)
            BEGIN
                SET @IdExists = 1
            END
        END
        ELSE
        IF (@Name IS NOT NULL AND @Name != '')
        BEGIN
            SELECT @TempName = [Name]
            FROM [dbo].[ActivityCategory]
            WHERE @Name = [Name] AND SoftDelete = 0
            
            IF (@TempName IS NOT NULL)
            BEGIN
                SET @NameExists = 1
            END
        END

        SELECT ac.[Id]
            ,ac.[GUID]
            ,ac.[Name]
            ,ac.[Description]
            ,ac.[MetaTags]
            ,ac.[AuthGroupID]
            ,ag.[Name] AS AuthgroupName
        FROM [dbo].[ActivityCategory] ac
        JOIN [dbo].[AuthorizationGroup] ag ON ac.AuthGroupID = ag.Id
        WHERE (@Name = ac.Name OR @NameExists <> 1) AND
            (@Id = ac.Id OR @IdExists <> 1) 
             AND ac.SoftDelete = 0
        END TRY
    BEGIN CATCH
        EXECUTE [dbo].Error_Handle 
    END CATCH
END
GO
