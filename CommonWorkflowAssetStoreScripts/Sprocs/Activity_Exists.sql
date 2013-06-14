
/****** Object:  StoredProcedure [dbo].[Activity_Exists]    Script Date: 4/25/2012 6:19:07 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/**************************************************************************
// Product:  CommonWF
// FileName:Activity_Exists.sql
// File description: Checks for the existance of an activity.
//
// Copyright 2012 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   ActivityLibrary_Exists                             *
**    Desc:   Checks for the existance of an activity.                                  *
**    Auth:   v-luval                                                      *
**    Date:   04/17/2012                                                   *
**                                                                         *
****************************************************************************
**   sproc logic flow: <Optional if complex> 
**   Parameter definition if complex
****************************************************************************
**                  CHANGE HISTORY
****************************************************************************
**	Date:        Author:             Description:
** ____________    ________________    ____________________________________
**  04/17/2012      v-stska             Original implementation
** *************************************************************************/
CREATE PROCEDURE [dbo].[Activity_Exists]		
		@InName varchar(255),
		@InVersion nvarchar(50)
--WITH ENCRYPTION
AS
BEGIN
    SET NOCOUNT ON
		   
	BEGIN TRY
			SELECT [Activity].[Id]
			FROM Activity
			WHERE @inName = Name AND @InVersion = [Version]				 
	END TRY
	BEGIN CATCH
		EXECUTE [dbo].Error_Handle 
	END CATCH
END


GO


