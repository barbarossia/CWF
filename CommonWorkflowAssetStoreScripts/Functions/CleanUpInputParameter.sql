
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


/**********************************************************************
**      Name: [CleanUpInputParameter]    CleanUpInputParameter.sql
**      Desc:
**
**      Auth: 
**
**      Date: 
**
***********************************************************************
**
**              CHANGE HISTORY
***********************************************************************
**   Date:      Author:      Description:
**   ________   __________   __________________________________________
**
**
***********************************************************************/

RAISERROR(N'', 0, 1) WITH NOWAIT

IF ( OBJECT_ID(N'[dbo].[CleanUpInputParameter]')IS NULL )
BEGIN
   RAISERROR(N'Creating User-Defined Inline Table-valued Function:  [dbo].[CleanUpInputParameter]', 0, 1) WITH NOWAIT
END
ELSE
BEGIN
   IF ( OBJECTPROPERTY(OBJECT_ID(N'[dbo].[CleanUpInputParameter]'), N'IsScalarFunction') = 0 )
   BEGIN
      RAISERROR(N'The name [dbo].[CleanUpInputParameter] is currently being used by another type of object!', 16, 127) WITH NOWAIT
   END
   ELSE
   BEGIN
      RAISERROR(N'Dropping and recreating User-Defined Inline Table-valued Function:  [dbo].[CleanUpInputParameter]', 0, 1) WITH NOWAIT
      DROP FUNCTION [dbo].[CleanUpInputParameter]
   END
END
GO

/* ********************************************************************
**    Name:   fCleanUpInputParameter                                  *
**    Desc:   Cleans up string.                                       *
**    Auth:   v-stska                                                 *
**    Date:   10/31/2010                                              *
**                                                                    *
***********************************************************************
**   sproc logic flow: <Optional if complex> 
**   Parameter definition if complex
***********************************************************************
**                  CHANGE HISTORY
***********************************************************************
**	Date:        Author:             Description:
** ____________    ________________    ______________________________
**  4/25/2008      v-tibyer             Original implementation
** 10/31/2010      v-stska				Modification
**
** ********************************************************************/
CREATE FUNCTION [dbo].[CleanUpInputParameter]
   (@InputString   AS [nvarchar](MAX) = NULL
   )
RETURNS [nvarchar](MAX)
AS
BEGIN
    SELECT @InputString = replace(@InputString,'--','')
    WHILE LEN(@InputString) > LEN(REPLACE(@InputString , '; ', ';'))
       OR LEN(@InputString) > LEN(REPLACE(@InputString , ' ;', ';'))
    BEGIN
		SELECT @InputString = REPLACE(@InputString , '; ', ';')
		SELECT @InputString = REPLACE(@InputString , ' ;', ';')
    END 
    RETURN(@InputString)
END
	
GO


IF ( ISNULL(OBJECTPROPERTY(OBJECT_ID(N'[dbo].[CleanUpInputParameter]'), N'IsScalarFunction'), 0) = 0 )
BEGIN
   RAISERROR(N'Unable to create User-Defined Inline Table-valued Function [dbo].[CleanUpInputParameter]!', 16, 127) WITH NOWAIT
END
GO

