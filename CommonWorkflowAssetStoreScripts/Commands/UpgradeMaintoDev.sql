
IF NOT EXISTS (SELECT *
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = '[dbo].etblActivityLibraries'
AND COLUMN_NAME = 'FriendlyName')
ALTER TABLE [dbo].etblActivityLibraries ADD FriendlyName nvarchar(50) NULL
go

IF NOT EXISTS (SELECT *
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = '[dbo].etblActivityLibraries'
AND COLUMN_NAME = 'ReleaseNotes')
ALTER TABLE [dbo].etblActivityLibraries ADD ReleaseNotes nvarchar(250) NULL
go

EXEC SP_RENAME 'etblStoreActivities','Activity'
EXEC SP_RENAME 'ltblActivityCategory','ActivityCategory'
EXEC SP_RENAME 'etblActivityLibraries','ActivityLibrary'
EXEC SP_RENAME 'mtblActivityLibraryDependencies','ActivityLibraryDependency'
EXEC SP_RENAME 'etblActivityLibrariesDependencyHeads','ActivityLibraryDependencyHead'
EXEC SP_RENAME 'ltblApplications','Application'
EXEC SP_RENAME 'ltblAuthGroups','AuthorizationGroup'
EXEC SP_RENAME 'ltblIcons','Icon'
EXEC SP_RENAME 'ltblStatusCodes','StatusCode'
EXEC SP_RENAME 'ltblToolBoxTabName','ToolBoxTabName'
EXEC SP_RENAME 'etblWorkflowType','WorkflowType'




