set server=%1
set database=%2

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

sqlcmd -S %server% -d %database% -E -b -i "TaskActivity.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created table TaskActivity

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibraryDependency.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created table ActivityLibraryDependency

sqlcmd -S %server% -d %database% -E -b -i "Permission.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created table Permission

sqlcmd -S %server% -d %database% -E -b -i "Role.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created table Role

sqlcmd -S %server% -d %database% -E -b -i "RoleEnvPermission.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created table RoleEnvPermission

sqlcmd -S %server% -d %database% -E -b -i "Environment.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created table Environment

goto done

REM: error handler
:errors
exit /B  -1

:done
echo done