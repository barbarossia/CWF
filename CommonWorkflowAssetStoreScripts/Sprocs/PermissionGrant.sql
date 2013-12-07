-- ================GRANT PERMISSION TO WEB SERVICE ROLE [BEGIN]==============================
PRINT 'Permission to [MarketplaceService] role - granting'
GRANT EXECUTE ON [dbo].[Activity_ChangeAuthor] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Activity_Copy] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Activity_Move] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Activity_DeleteByNameAndVersion] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Activity_CreateOrUpdate] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Activity_Delete] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Activity_Exists] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Activity_Get] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Activity_GetByActivityLibrary] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Activity_GetByCategoryNameWithoutExecutable] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Activity_GetByName] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Activity_OverrideLock] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Activity_SetLock] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Activity_UpdateLock] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Activity_Search] TO [MarketplaceService];

GRANT EXECUTE ON [dbo].[ActivityCategory_CreateOrUpdate] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[ActivityCategory_Delete] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[ActivityCategory_Get] TO [MarketplaceService];

GRANT EXECUTE ON [dbo].[ActivityLibrary_GetMissing] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[ActivityLibrary_Delete] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[ActivityLibrary_Get] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[ActivityLibrary_CreateOrUpdate] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[ActivityLibrary_Exists] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[ActivityLibrary_GetWithoutExecutable] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[ActivityLibrary_RecursionCheck] TO [MarketplaceService];

GRANT EXECUTE ON [dbo].[ActivityLibraryDependency_CreateOrUpdate] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[ActivityLibraryDependency_Delete] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[ActivityLibraryDependency_Get] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[ActivityLibraryDependency_CreateOrUpdateListHead] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[ActivityLibraryDependency_GetTree] TO [MarketplaceService];

GRANT EXECUTE ON [dbo].[Application_CreateOrUpdate] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Application_Delete] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Application_Get] TO [MarketplaceService];

GRANT EXECUTE ON [dbo].[AuthorizationGroup_CreateOrUpdate] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[AuthorizationGroup_Delete] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[AuthorizationGroup_Get] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[AuthorizationGroup_EnableOrDisable] TO [MarketplaceService];

GRANT EXECUTE ON [dbo].[PublishWorkflow_GetByNameVersion] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[PublishWorkflow_GetByWorkflowType] TO [MarketplaceService];

GRANT EXECUTE ON [dbo].[StatusCode_CreateOrUpdate] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[StatusCode_Delete] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[StatusCode_Get] TO [MarketplaceService];

GRANT EXECUTE ON [dbo].[TableRow_DeleteById] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[TableRow_UpdateSoftDelete] TO [MarketplaceService];

GRANT EXECUTE ON [dbo].[ToolBoxTabName_CreateOrUpdate] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[ToolBoxTabName_Delete] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[ToolboxTabName_Get] TO [MarketplaceService];

GRANT EXECUTE ON [dbo].[WorkflowType_CreateOrUpdate] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[WorkflowType_Get] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[WorkflowType_Search] TO [MarketplaceService];

GRANT EXECUTE ON TYPE::[dbo].[ActivityLibraryTableType] to [MarketplaceService];
GRANT EXECUTE ON TYPE::[dbo].[AuthGroupNameTableType] to [MarketplaceService];
GRANT EXECUTE ON TYPE::[dbo].[EnvironmentTableType] to [MarketplaceService];

GRANT EXECUTE ON [dbo].[Marketplace_Search] to [MarketplaceService];
GRANT EXECUTE ON [dbo].[Marketplace_SearchAllItems] to [MarketplaceService];
GRANT EXECUTE ON [dbo].[Marketplace_SearchExecutableItems] to [MarketplaceService];
GRANT EXECUTE ON [dbo].[Marketplace_SearchXamlItems] to [MarketplaceService];
GRANT EXECUTE ON [dbo].[Marketplace_GetAssetDetails] to [MarketplaceService];

GRANT EXECUTE ON [dbo].[TaskActivity_CreateOrUpdate] to [MarketplaceService];
GRANT EXECUTE ON [dbo].[TaskActivity_Get] to [MarketplaceService];
GRANT EXECUTE ON [dbo].[TaskActivity_Search] to [MarketplaceService];
GRANT EXECUTE ON [dbo].[TaskActivity_UpdateStatus] to [MarketplaceService];

GRANT EXECUTE ON [dbo].[Permission_Get] to [MarketplaceService];
GRANT EXECUTE ON [dbo].[ValidateEnvironmentMove] to [MarketplaceService];
GRANT EXECUTE ON [dbo].[ValidateSPPermission] to [MarketplaceService];


-- GRANT CONNECT  TO [MarketplaceService];
PRINT 'Permission to [MarketplaceService] role - granted'
PRINT '--------------------------------------'
-- ================GRANT PERMISSION TO WEB SERVICE ROLE [END]================================