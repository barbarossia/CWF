
/****** Object:  StoredProcedure [dbo].[Activity_Exists]    Script Date: 4/25/2012 6:19:07 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Activity_Exists]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Activity_Exists]
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
		@InVersion nvarchar(50),
		@InEnvironmentName nvarchar(50)
--WITH ENCRYPTION
AS
BEGIN
    SET NOCOUNT ON
		   
	BEGIN TRY
			SELECT sa.[Id]
			FROM Activity sa
			join Environment E on sa.Environment = E.Id
			WHERE @inName = sa.Name AND @InVersion = sa.[Version] AND @InEnvironmentName = E.[Name]			 
	END TRY
	BEGIN CATCH
		EXECUTE [dbo].Error_Handle 
	END CATCH
END


GO


