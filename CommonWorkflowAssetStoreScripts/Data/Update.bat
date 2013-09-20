set server =%1
set database =%2

:copy update

sqlcmd -S %server% -d %database% -E -b -i "Environment.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Loaded Environment

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

sqlcmd -S %server% -d %database% -E -b -i "Role.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Loaded Role

sqlcmd -S %server% -d %database% -E -b -i "Permission.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Loaded Permission

sqlcmd -S %server% -d %database% -E -b -i "RoleEnvPermission.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Loaded RoleEnvPermission

goto done

REM: error handler
:errors
exit /B  -1

:done
echo done