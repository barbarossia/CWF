set server=%1
set database=%2

sqlcmd -S %server% -d %database% -E -b -i "Activity.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created table Activity

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibrary.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created table ActivityLibrary

sqlcmd -S %server% -d %database% -E -b -i "WorkflowType.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created table WorkflowType

goto done

REM: error handler
:errors
exit /B  -1

:done
echo done