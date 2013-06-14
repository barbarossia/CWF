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
**      Name: [Activity]          Activity.alt
**      Desc:
**
**      Auth: REDMOND\v-bobzh
**
**      Date: 05/31/2012 
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
PRINT '    Dropping INDEX FROM [Activity]'
IF  EXISTS (SELECT * FROM sys.fulltext_indexes fti WHERE fti.object_id = OBJECT_ID(N'[dbo].[Activity]'))
ALTER FULLTEXT INDEX ON [dbo].[Activity] DISABLE
GO

/****** Object:  FullTextIndex     Script Date: 05/31/2012 15:58:18 ******/
IF  EXISTS (SELECT * FROM sys.fulltext_indexes fti WHERE fti.object_id = OBJECT_ID(N'[dbo].[Activity]'))
DROP FULLTEXT INDEX ON [dbo].[Activity]

GO

IF  EXISTS (SELECT * FROM sysfulltextcatalogs ftc WHERE ftc.name = N'Activity_catalog')
DROP FULLTEXT CATALOG [Activity_catalog]
GO

PRINT '    Adding INDEX To [Activity]'
CREATE FULLTEXT CATALOG Activity_catalog;
GO
CREATE FULLTEXT INDEX ON Activity
 ( 
  ShortName
     Language 1033
 ) 
  KEY INDEX PK_Activity 
      ON Activity_catalog; 
GO