set server=%1
set database=%2

sqlcmd -S %server% -d %database% -E -b -i "ValidateSPPermission.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc ValidateSPPermission.sql"

sqlcmd -S %server% -d %database% -E -b -i "ValidateEnvironmentMove.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc ValidateEnvironmentMove"

sqlcmd -S %server% -d %database% -E -b -i "Error_Handle.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Error_Handle"

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibrary_GetMissing.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc ActivityLibrary_GetMissing"

sqlcmd -S %server% -d %database% -E -b -i "Activity_GetByName.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Activity_GetByName"

sqlcmd -S %server% -d %database% -E -b -i "TableRow_DeleteById.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc TableRow_DeleteById"

sqlcmd -S %server% -d %database% -E -b -i "TableRow_UpdateSoftDelete.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc TableRow_UpdateSoftDelete"

sqlcmd -S %server% -d %database% -E -b -i "Error_Raise.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Error_Raise"

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibrary_Get.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc ActivityLibrary_Get"

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibrary_CreateOrUpdate.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc ActivityLibrary_CreateOrUpdate"

sqlcmd -S %server% -d %database% -E -b -i "Activity_GetByActivityLibrary.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Activity_GetByActivityLibrary"

sqlcmd -S %server% -d %database% -E -b -i "Activity_CreateOrUpdate.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Activity_CreateOrUpdate"

sqlcmd -S %server% -d %database% -E -b -i "Activity_Delete.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Activity_Delete"

sqlcmd -S %server% -d %database% -E -b -i "Activity_Get.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Activity_Get"

sqlcmd -S %server% -d %database% -E -b -i "WorkflowType_Get.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc WorkflowType_Get"

sqlcmd -S %server% -d %database% -E -b -i "ActivityCategory_CreateOrUpdate.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc ActivityCategory_CreateOrUpdate"

sqlcmd -S %server% -d %database% -E -b -i "ActivityCategory_Get.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc ActivityCategory_Get"

sqlcmd -S %server% -d %database% -E -b -i "Application_Get.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Application_Get"

sqlcmd -S %server% -d %database% -E -b -i "AuthorizationGroup_CreateOrUpdate.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc AuthorizationGroup_CreateOrUpdate"

sqlcmd -S %server% -d %database% -E -b -i "AuthorizationGroup_Get.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc AuthorizationGroup_Get"

sqlcmd -S %server% -d %database% -E -b -i "StatusCode_Get.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc StatusCode_Get"

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibrary_RecursionCheck.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc ActivityLibrary_RecursionCheck"

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibraryDependency_CreateOrUpdate.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc ActivityLibraryDependency_CreateOrUpdate"

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibraryDependency_CreateOrUpdateListHead.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc ActivityLibraryDependency_CreateOrUpdateListHead"

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibraryDependency_GetTree.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc ActivityLibraryDependency_GetTree"

sqlcmd -S %server% -d %database% -E -b -i "Activity_Search.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Activity_Search"

sqlcmd -S %server% -d %database% -E -b -i "Activity_Exists.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Activity_Exists"

sqlcmd -S %server% -d %database% -E -b -i "Marketplace_SearchAllItems.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Marketplace_SearchAllItems"

sqlcmd -S %server% -d %database% -E -b -i "Marketplace_SearchExecutableItems.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Marketplace_SearchExecutableItems"

sqlcmd -S %server% -d %database% -E -b -i "Marketplace_SearchXamlItems.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Marketplace_SearchXamlItems"

sqlcmd -S %server% -d %database% -E -b -i "Marketplace_Search.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Marketplace_Search"

sqlcmd -S %server% -d %database% -E -b -i "Marketplace_GetAssetDetails.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Marketplace_GetAssetDetails"

sqlcmd -S %server% -d %database% -E -b -i "Activity_SetLock.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Activity_SetLock"

sqlcmd -S %server% -d %database% -E -b -i "Activity_OverrideLock.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Activity_OverrideLock"

sqlcmd -S %server% -d %database% -E -b -i "Activity_UpdateLock.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Activity_UpdateLock"

sqlcmd -S %server% -d %database% -E -b -i "WorkflowType_Search.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc WorkflowType_Search"

sqlcmd -S %server% -d %database% -E -b -i "WorkflowType_CreateOrUpdate.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc WorkflowType_CreateOrUpdate"

sqlcmd -S %server% -d %database% -E -b -i "TaskActivity_CreateOrUpdate.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc TaskActivity_CreateOrUpdate"

sqlcmd -S %server% -d %database% -E -b -i "TaskActivity_Get.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc TaskActivity_Get"

sqlcmd -S %server% -d %database% -E -b -i "TaskActivity_Search.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc TaskActivity_Search"

sqlcmd -S %server% -d %database% -E -b -i "TaskActivity_UpdateStatus.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc TaskActivity_UpdateStatus"

sqlcmd -S %server% -d %database% -E -b -i "Permission_Get.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Permission_Get"

sqlcmd -S %server% -d %database% -E -b -i "AuthorizationGroup_EnableOrDisable.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc AuthorizationGroup_EnableOrDisable"

sqlcmd -S %server% -d %database% -E -b -i "Activity_ChangeAuthor.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Activity_ChangeAuthor"

sqlcmd -S %server% -d %database% -E -b -i "Activity_Copy.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Activity_Copy"

sqlcmd -S %server% -d %database% -E -b -i "Activity_Move.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Activity_Move"

sqlcmd -S %server% -d %database% -E -b -i "PermissionGrant.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Grant sproc Permission"

goto done

REM: error handler
:errors
exit /B  -1

:done
echo done