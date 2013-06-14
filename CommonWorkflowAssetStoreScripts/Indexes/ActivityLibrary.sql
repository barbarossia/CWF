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
**      Name: [ActivityLibrary]          
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
PRINT '    Dropping INDEX FROM [ActivityLibrary]'
IF  EXISTS (SELECT * FROM sys.fulltext_indexes fti WHERE fti.object_id = OBJECT_ID(N'[dbo].[ActivityLibrary]'))
ALTER FULLTEXT INDEX ON [dbo].[ActivityLibrary] DISABLE
GO

/****** Object:  FullTextIndex     Script Date: 05/31/2012 15:58:18 ******/
IF  EXISTS (SELECT * FROM sys.fulltext_indexes fti WHERE fti.object_id = OBJECT_ID(N'[dbo].[ActivityLibrary]'))
DROP FULLTEXT INDEX ON [dbo].[ActivityLibrary]
GO

IF  EXISTS (SELECT * FROM sysfulltextcatalogs ftc WHERE ftc.name = N'ActivityLibrary_catalog')
DROP FULLTEXT CATALOG [ActivityLibrary_catalog]
GO

PRINT '    Adding INDEX To [ActivityLibrary]'
CREATE FULLTEXT CATALOG ActivityLibrary_catalog;
GO
CREATE FULLTEXT INDEX ON ActivityLibrary
 ( 
  [Description]
     Language 1033,
  Name
     Language 1033,
  MetaTags 
     Language 1033     
 ) 
  KEY INDEX PK_ActivityLibrary 
      ON ActivityLibrary_catalog; 
GO