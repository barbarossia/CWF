@echo off
REM: Command File Created 
REM: Date Generated: 6/12/2011
REM: Authentication type: Windows NT
REM: Usage: CommandFilename [Server] [Database]

if '%1' == '' goto usage
if '%2' == '' goto usage
if '%3' == '' goto usage
if '%4' == '' goto usage

if '%1' == '/?' goto usage
if '%1' == '-?' goto usage
if '%1' == '?' goto usage
if '%1' == '/help' goto usage

set server=%1
set database=%2
set PublishPath=%3
set WorkflowControllerURL=%4

echo server=%server%
echo database=%database%

REM: if %ERRORLEVEL% NEQ 0 goto errors
echo "*********************************************************************"
echo "***** Executing CommonWorkflowAssetStore DB Scripts" on %server%
echo "*********************************************************************"



:tablecreate
echo "***********************************************************"
echo "***** Creating Tables on %database%
echo "***********************************************************"

CD "..\tables"

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibrary.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created table ActivityLibrary

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibraryDependencyHead.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created table ActivityLibraryDependencyHead

sqlcmd -S %server% -d %database% -E -b -i "Activity.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created table Activity

sqlcmd -S %server% -d %database% -E -b -i "WorkflowType.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created table WorkflowType

sqlcmd -S %server% -d %database% -E -b -i "ActivityCategory.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created table ActivityCategory

sqlcmd -S %server% -d %database% -E -b -i "Application.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created table Application

sqlcmd -S %server% -d %database% -E -b -i "AuthorizationGroup.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created table AuthorizationGroup

sqlcmd -S %server% -d %database% -E -b -i "Icon.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created table Icon

sqlcmd -S %server% -d %database% -E -b -i "StatusCode.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created table StatusCode

sqlcmd -S %server% -d %database% -E -b -i "ToolBoxTabName.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created table ToolBoxTabName

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibraryDependency.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created table ActivityLibraryDependency

echo "***********************************************************"
echo "***** creating Contraints on  %database%
echo "***********************************************************"

CD "..\Constraints"

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibrary.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created constraint ActivityLibrary.sql"

sqlcmd -S %server% -d %database% -E -b -i "Activity.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created constraint Activity.sql"

sqlcmd -S %server% -d %database% -E -b -i "WorkflowType.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created constraint WorkflowType.sql"

sqlcmd -S %server% -d %database% -E -b -i "ActivityCategory.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created constraint ActivityCategory.sql"

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibraryDependency.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created constraint ActivityLibraryDependency.sql"

echo "***********************************************************"
echo "***** creating types on  %database%
echo "***********************************************************"

CD "..\Types"

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibraryTableType.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created type ActivityLibraryTableType"

echo "***********************************************************"
echo "***** creating functions on  %database%
echo "***********************************************************"

CD "..\Indexes"

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibrary.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created indexes ActivityLibrary.sql"

sqlcmd -S %server% -d %database% -E -b -i "Activity.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created indexes Activity.sql"

CD "..\Functions"

sqlcmd -S %server% -d %database% -E -b -i "CleanUpInputParameter.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created function CleanUpInputParameter.sql"

echo "***********************************************************"
echo "creating sprocs on  %database%
echo "***********************************************************"

CD "..\Sprocs"

if /i '%3' == 'u' goto remainingsprocs
if /i '%3' == 'uat' goto remainingsprocs
if /i '%3' == 's' goto remainingsprocs
if /i '%3' == 'staging' goto remainingsprocs
if /i '%3' == 'p' goto remainingsprocs
if /i '%3' == 'prod' goto remainingsprocs

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

:remainingsprocsivi

sqlcmd -S %server% -d %database% -E -b -i "Error_Raise.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Error_Raise"

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibrary_Delete.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc ActivityLibrary_Delete"

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibrary_Get.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc ActivityLibrary_Get"

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibrary_CreateOrUpdate.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc ActivityLibrary_CreateOrUpdate"

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibrary_Exists.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc ActivityLibrary_Exists"

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibrary_GetWithoutExecutable.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc ActivityLibrary_GetWithoutExecutable"

sqlcmd -S %server% -d %database% -E -b -i "Activity_GetByActivityLibrary.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Activity_GetByActivityLibrary"

sqlcmd -S %server% -d %database% -E -b -i "Activity_DeleteByNameAndVersion.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Activity_DeleteByNameAndVersion"

sqlcmd -S %server% -d %database% -E -b -i "Activity_CreateOrUpdate.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Activity_CreateOrUpdate"

sqlcmd -S %server% -d %database% -E -b -i "Activity_Delete.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Activity_Delete"

sqlcmd -S %server% -d %database% -E -b -i "Activity_Get.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Activity_Get"

sqlcmd -S %server% -d %database% -E -b -i "WorkflowType_CreateOrUpdate.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc WorkflowType_CreateOrUpdate"

sqlcmd -S %server% -d %database% -E -b -i "WorkflowType_Get.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc WorkflowType_Get"

sqlcmd -S %server% -d %database% -E -b -i "ActivityCategory_CreateOrUpdate.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc ActivityCategory_CreateOrUpdate"

sqlcmd -S %server% -d %database% -E -b -i "ActivityCategory_Delete.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc ActivityCategory_Delete"

sqlcmd -S %server% -d %database% -E -b -i "ActivityCategory_Get.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc ActivityCategory_Get"

sqlcmd -S %server% -d %database% -E -b -i "Application_CreateOrUpdate.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Application_CreateOrUpdate"

sqlcmd -S %server% -d %database% -E -b -i "Application_Delete.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Application_Delete"

sqlcmd -S %server% -d %database% -E -b -i "Application_Get.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Application_Get"

sqlcmd -S %server% -d %database% -E -b -i "AuthorizationGroup_CreateOrUpdate.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc AuthorizationGroup_CreateOrUpdate"

sqlcmd -S %server% -d %database% -E -b -i "AuthorizationGroup_Delete.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc AuthorizationGroup_Delete"

sqlcmd -S %server% -d %database% -E -b -i "AuthorizationGroup_Get.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc AuthorizationGroup_Get"

sqlcmd -S %server% -d %database% -E -b -i "StatusCode_CreateOrUpdate.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc StatusCode_CreateOrUpdate"

sqlcmd -S %server% -d %database% -E -b -i "StatusCode_Delete.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc StatusCode_Delete"

sqlcmd -S %server% -d %database% -E -b -i "StatusCode_Get.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc StatusCode_Get"

sqlcmd -S %server% -d %database% -E -b -i "ToolBoxTabName_CreateOrUpdate.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc ToolBoxTabName_CreateOrUpdate"

sqlcmd -S %server% -d %database% -E -b -i "ToolBoxTabName_Delete.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc ToolBoxTabName_Delete"

sqlcmd -S %server% -d %database% -E -b -i "ToolboxTabName_Get.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc ToolboxTabName_Get"

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibrary_RecursionCheck.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc ActivityLibrary_RecursionCheck"

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibraryDependency_CreateOrUpdate.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc ActivityLibraryDependency_CreateOrUpdate"

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibraryDependency_Delete.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc ActivityLibraryDependency_Delete"

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibraryDependency_Get.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc ActivityLibraryDependency_Get"

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibraryDependency_CreateOrUpdateListHead.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc ActivityLibraryDependency_CreateOrUpdateListHead"

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibraryDependency_GetTree.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc ActivityLibraryDependency_GetTree"

sqlcmd -S %server% -d %database% -E -b -i "PublishWorkflow_GetByNameVersion.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc PublishWorkflow_GetByNameVersion"

sqlcmd -S %server% -d %database% -E -b -i "PublishWorkflow_GetByWorkflowType.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc PublishWorkflow_GetByWorkflowType"

sqlcmd -S %server% -d %database% -E -b -i "Activity_GetByCategoryNameWithoutExecutable.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Activity_GetByCategoryNameWithoutExecutable"

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

sqlcmd -S %server% -d %database% -E -b -i "Activity_UpdateLock.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   **** Created sproc Activity_UpdateLock"

echo "***********************************************************"
echo "***** Populating objects in  %database%
echo "***********************************************************"

CD "..\Data"

sqlcmd -S %server% -d %database% -E -b -i "AuthorizationGroup.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Loaded AuthorizationGroup"

sqlcmd -S %server% -d %database% -E -b -i "Application.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Loaded Application

sqlcmd -S %server% -d %database% -E -b -i "ActivityCategory.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Loaded ActivityCategory

sqlcmd -S %server% -d %database% -E -b -i "StatusCode.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Loaded StatusCode

sqlcmd -S %server% -d %database% -E -b -i "Icon.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Loaded Icon

sqlcmd -S %server% -d %database% -E -b -i "ToolBoxTabName.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Loaded ToolBoxTabName

echo "***********************************************************"
echo "***** creating TRIGGERs on  %database%
echo "***********************************************************"

CD "..\Triggers"
echo "   ***** No triggers to create"

echo "***********************************************************"
echo "***** creating VIEWs on  %database%
echo "***********************************************************"

CD "..\Views"

echo "   ***** No views to create"

echo "***********************************************************"
echo "***** Create FullText search capabilites on %database%
echo "***********************************************************"

CD "..\SETUP"

echo "   ***** No fulltext search to create"


echo "***********************************************************"
echo "***** adding Lookup Values on %database%
echo "***********************************************************"

CD "..\Data"

echo "   ***** No data to create"


echo "***********************************************************"
echo "Post Processing on %database%
echo "***********************************************************"

:devcontactgroup

REM Only need to use InsertInitialData if you need a blank DB you can insert OASP activities into to create InsertOASPActivitiesAndTemplates.sql
REM sqlcmd -S %server% -d %database% -E -b -i "InsertInitialData.sql" -v PublishPath = "'%PublishPath%'" -v WorkflowControllerURL = "'%WorkflowControllerURL%'"

echo "   ***** Loaded initial data
sqlcmd -S %server% -d %database% -E -b -i "InsertOASPActivitiesAndTemplates.sql" -v PublishPath = "'%PublishPath%'" -v WorkflowControllerURL = "'%WorkflowControllerURL%'"

echo "   ***** Loaded OASP data

if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Loaded data sucessfully
echo "   ***** Completed Processing"


:contactgroupend

REM: CD "..\commands"

goto finish


REM: How to use screen
:usage
echo.
echo. Usage: GenerateCWFAssetStoreDB.bat ^<Server^> ^<DBName^> ^<PublishPath^> ^<WorkflowControllerURL^>
echo.    Server ::=  ^<target SQL Server^>
echo.    DBName ::=  ^<Name of database to create^>
echo.    PublishPath ::= Remote file path where OASWorkflowInstallerService 
echo.                    will listen for published URLs.
echo.    WorkflowControllerURL ::= URL of a WorkflowController which the 
echo.                    OASWorkflowInstallerService will install to.
echo.
echo. Example: GenerateCWFAssetStoreDB.bat pqocwfddb02 CommonWorkflowAssetStoreDEV \\PQOOASIWB02\WorkflowDrop http://pqooasiwb02/OASP

echo.
goto done

REM: error handler
:errors
echo.
echo WARNING! Error(s) were detected!
echo --------------------------------
echo Please evaluate the situation and, if needed, restart this command file. You may need to
echo supply command parameters when executing this command file.
echo.
pause
goto done

REM: finished execution
:finish
echo.
echo Script execution was sucessful.
echo.
cd ..\Commands

REM: "Perform the sql scripts under the Test directory"
REM: CD "..\"
REM: CD "Test"
REM: DIR

:done
@echo on



