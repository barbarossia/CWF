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

CD "..\Setup"

sqlcmd -S %server% -d master -E -b -i "CommonWorkflowAssetStoreDBCreate.sql" -v DBName = '%database%'
if %ERRORLEVEL% NEQ 0 goto errors

sqlcmd -S %server% -d %database% -E -b -i "CWFAssetStoreUserRole.sql"
if %ERRORLEVEL% NEQ 0 goto errors

:devuserrole
goto tablecreate

--sqlcmd -S %1 -d %2 -E -b -i "CWFAssetStoreUserRoleDev.sql"
--if %ERRORLEVEL% NEQ 0 goto errors

:tablecreate
echo "***********************************************************"
echo "***** Creating Tables on %database%
echo "***********************************************************"

CD "..\tables"

CALL Tables.BAT %server% %database%
if %ERRORLEVEL% NEQ 0 goto errors

echo "***********************************************************"
echo "***** creating types on  %database%
echo "***********************************************************"

CD "..\Types"

CALL Update.BAT %server% %database%
if %ERRORLEVEL% NEQ 0 goto errors

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

CALL Sprocs.BAT %server% %database%
if %ERRORLEVEL% NEQ 0 goto errors

echo "***********************************************************"
echo "***** Populating objects in  %database%
echo "***********************************************************"

CD "..\Data"

CALL UpdateWithData.BAT %server% %database%
if %ERRORLEVEL% NEQ 0 goto errors

echo "***********************************************************"
echo "***** creating environment on  %database%
echo "***********************************************************"

CD "..\Environment\Tables"
CALL Update.BAT %server% %database%
if %ERRORLEVEL% NEQ 0 goto errors

CD ".."

echo "***********************************************************"
echo "***** creating Contraints on  %database%
echo "***********************************************************"

CD "..\Constraints"
CALL Update.BAT %server% %database%
if %ERRORLEVEL% NEQ 0 goto errors

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


if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Loaded data sucessfully
echo "   ***** Completed Processing"


:contactgroupend

REM: CD "..\commands"

goto finish


REM: How to use screen
:usage
echo.
echo. Usage: GenerateCWFAssetStoreDB.bat ^<Server^> ^<DBName^> ^<PublishPath^> ^<WorkflowControllerURL^> ^<Environment^>
echo.    Server ::=  ^<target SQL Server^>
echo.    DBName ::=  ^<Name of database to create^>
echo.    PublishPath ::= Remote file path where OASWorkflowInstallerService 
echo.                    will listen for published URLs.
echo.    WorkflowControllerURL ::= URL of a WorkflowController which the 
echo.                    OASWorkflowInstallerService will install to.
echo.    Environment ::=  ^<Name of EnvironmentL: dev, test, stage, prod^>
echo.
echo. Example: GenerateCWFAssetStoreDB.bat pqocwfddb02 CommonWorkflowAssetStoreDEV \\PQOOASIWB02\WorkflowDrop http://pqooasiwb02/OASP dev

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



