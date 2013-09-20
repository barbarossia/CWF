set server=%1
set database=%2
set olddb=%3

sqlcmd -S %server% -d %database% -E -b -i "AuthorizationGroup.sql" -v DBName = '%olddb%'
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Merged table AuthorizationGroup

sqlcmd -S %server% -d %database% -E -b -i "ActivityCategory.sql" -v DBName = '%olddb%'
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Merged table ActivityCategory

sqlcmd -S %server% -d %database% -E -b -i "MergeDatabse\ActivityLibrary.sql" -v DBName = '%olddb%'
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Merged table ActivityLibrary

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibraryDependency.sql" -v DBName = '%olddb%'
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Merged table ActivityLibraryDependency

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibraryDependencyHead.sql" -v DBName = '%olddb%'
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Merged table ActivityLibraryDependencyHead

sqlcmd -S %server% -d %database% -E -b -i "MergeDatabse\Activity.sql" -v DBName = '%olddb%'
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Merged table Activity

sqlcmd -S %server% -d %database% -E -b -i "TaskActivity.sql" -v DBName = '%olddb%'
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Merged table TaskActivity

sqlcmd -S %server% -d %database% -E -b -i "MergeDatabse\WorkflowType.sql" -v DBName = '%olddb%'
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Merged table WorkflowType

sqlcmd -S %server% -d %database% -E -b -i "MergeDatabse\UpdateWorkflowType.sql" -v DBName = '%olddb%'
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Merged table UpdateWorkflowType

goto done

REM: error handler
:errors
exit /B  -1

:done
echo done