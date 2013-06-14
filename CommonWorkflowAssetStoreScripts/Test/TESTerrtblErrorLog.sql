USE [Argus]

DECLARE @RunTimeStamp					[DateTime]
		,@NotificationsLastScanData		[xml]
		,@LogData						[xml]
		,@NotificationData				[xml]
		,@AlertsData					[xml]

SET @RunTimeStamp = GETDATE()
SET @NotificationsLastScanData	= '<Root>@NotificationsLastScanData</Root>'

/*
 * case #1 - just run this script and see an error log failure entry
 * Eliminate the two comments in the set statements and run again
 *     The entry in the log shows a valid call
*/
--SET @LogData					= '<Root>@LogData</Root>'
--SET @NotificationData			= '<Root>@NotificationData</Root>'
SET @AlertsData				= '<Root>@AlertsData</Root>'
	
EXEC [dbo].[petblScraperDataCreate] @RunTimeStamp = @RunTimeStamp
		,@NotificationsLastScanData	= 	@NotificationsLastScanData
		,@LogData					=	@LogData
		,@NotificationData			=	@NotificationData
		,@AlertsData				=	@AlertsData


SELECT *
FROM ErrtblErrorLog