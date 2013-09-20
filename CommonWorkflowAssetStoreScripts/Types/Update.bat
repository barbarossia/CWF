set server =%1
set database =%2

:copy types

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibraryTableType.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created type ActivityLibraryTableType"

sqlcmd -S %server% -d %database% -E -b -i "AuthGroupNameTableType.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created type AuthGroupNameTableType"

sqlcmd -S %server% -d %database% -E -b -i "EnvironmentTableType.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created type EnvironmentTableType"

goto done

REM: error handler
:errors
exit /B  -1

:done
echo done