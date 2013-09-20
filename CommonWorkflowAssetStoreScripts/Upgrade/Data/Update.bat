set server =%1
set database =%2

:copy update

CD "..\..\Data"

sqlcmd -S %server% -d %database% -E -b -i "Environment.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Loaded Environment

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