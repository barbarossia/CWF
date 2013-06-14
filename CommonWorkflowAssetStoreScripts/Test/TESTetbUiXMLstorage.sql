USE [Argus]

DECLARE @RunTimeStamp					[DateTime]
		,@NotificationsLastScanData		[xml]
		,@LogData						[xml]
		,@NotificationData				[xml]
		,@AlertsData					[xml]

SET @RunTimeStamp = GETDATE()
SET @NotificationsLastScanData	= '<Root>@NotificationsLastScanData</Root>'
/*
 * Run a note that the ScraperKey is the same for for all 5 entries
 * Comment the @LogData Set statement and run. Note the error msg, Errorlog entry
 *     And no additional entry made in the 5 files
 */
--SET @LogData					= '<Root>@LogData</Root>'
SET @NotificationData			= '<Root>@NotificationData</Root>'
SET @AlertsData				= '<Root>@AlertsData</Root>'
	
EXEC [dbo].[petblScraperDataCreate] @RunTimeStamp = @RunTimeStamp
		,@NotificationsLastScanData	= 	@NotificationsLastScanData
		,@LogData					=	@LogData
		,@NotificationData			=	@NotificationData
		,@AlertsData				=	@AlertsData


SELECT *
FROM etblScraperData

SELECT *
FROM etblScraperAlerts
SELECT *
FROM etblScraperLogs
SELECT *
FROM etblScraperNotifications
SELECT *
FROM etblScraperLastScanData
SELECT *
FROM ErrtblErrorLog