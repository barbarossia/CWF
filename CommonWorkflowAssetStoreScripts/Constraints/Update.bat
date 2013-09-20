set server =%1
set database =%2

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

sqlcmd -S %server% -d %database% -E -b -i "TaskActivity.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created constraint TaskActivity.sql"

sqlcmd -S %server% -d %database% -E -b -i "Permission.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created constraint Permission.sql"

sqlcmd -S %server% -d %database% -E -b -i "RoleEnvPermission.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created constraint RoleEnvPermission.sql"

sqlcmd -S %server% -d %database% -E -b -i "AuthorizationGroup.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created constraint AuthorizationGroup.sql"

goto done

REM: error handler
:errors
exit /B  -1

:done
echo done