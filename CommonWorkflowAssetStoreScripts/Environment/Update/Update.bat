set server =%1
set database =%2
set env =%3

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibrary.sql" -v Evr = '%env%'
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Updated table ActivityLibrary

sqlcmd -S %server% -d %database% -E -b -i "Activity.sql" -v Evr = '%env%'
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Updated table Activity

sqlcmd -S %server% -d %database% -E -b -i "WorkflowType.sql" -v Evr = '%env%'
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Updated table WorkflowType

goto done

REM: error handler
:errors
exit /B  -1

:done
echo done