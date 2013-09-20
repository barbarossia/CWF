@echo off
REM: Command File Created 
REM: Date Generated: 5/12/2013
REM: Authentication type: Windows NT

echo Please input the server you want to connect.
set /p server=
if '%server%'=='' goto errors
echo.
echo Please input the database you want to upgrade.
set /p database=
if '%database%'=='' goto errors
CD "..\..\Setup"
sqlcmd -S %server% -d master -E -b -i "CheckIfNotExists.sql" -v DBName = '%database%'
if %ERRORLEVEL% NEQ 0 goto errors
echo.

CD "..\Upgrade"
choice /c 1234 /m "Please select one environment: dev, test, stage, prod"
if %errorlevel% ==1 set env=dev
if %errorlevel% ==2 set env=test
if %errorlevel% ==3 set env=stage
if %errorlevel% ==4 set env=prod
echo.	


echo server=%server%
echo database=%database%
echo env=%env%

:table create
echo "***********************************************************"
echo "***** Creating Tables on %database%
echo "***********************************************************"

CD "..\Tables"

CALL Tables.BAT %server% %database%
if %ERRORLEVEL% NEQ 0 goto errors

echo "***********************************************************"
echo "***** Populating objects in  %database%
echo "***********************************************************"

CD "..\Upgrade\Data"

CALL Update.BAT %server% %database%
if %ERRORLEVEL% NEQ 0 goto errors

:Add column
echo "***********************************************************"
echo "***** Alter Tables on %database%
echo "***********************************************************"

CD "..\Upgrade\Tables"

CALL Update.BAT %server% %database%
if %ERRORLEVEL% NEQ 0 goto errors

:Add column
echo "***********************************************************"
echo "***** Insert BuildIn Admin on %database%
echo "***********************************************************"

CD "..\..\Data"
sqlcmd -S %server% -d %database% -E -b -i "AuthorizationGroup.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Loaded AuthorizationGroup"

:Add column
echo "***********************************************************"
echo "***** Update Environment on %database%
echo "***********************************************************"

CD "..\Environment\Update"

CALL Update.BAT %server% %database% %env%
if %ERRORLEVEL% NEQ 0 goto errors

:aler column
echo "***********************************************************"
echo "***** Alter tables on %database%
echo "***********************************************************"

CD "..\Tables"

CALL Update.BAT %server% %database% %env%
if %ERRORLEVEL% NEQ 0 goto errors

CD ".."

:other
echo "***********************************************************"
echo "***** creating Contraints on  %database%
echo "***********************************************************"

CD "..\Constraints"

CALL Update.BAT %server% %database%
if %ERRORLEVEL% NEQ 0 goto errors

echo "***********************************************************"
echo "***** creating types on  %database%
echo "***********************************************************"

CD "..\Types"

CALL Update.BAT %server% %database%
if %ERRORLEVEL% NEQ 0 goto errors


echo "***********************************************************"
echo "creating sprocs on  %database%
echo "***********************************************************"

CD "..\Sprocs"

CALL Sprocs.BAT %server% %database%
if %ERRORLEVEL% NEQ 0 goto errors


REM: CD "..\commands"

goto finish

REM: How to use screen
:usage
echo.
echo. Usage: MergeCWFAssetStoreDB.bat ^<Server^> ^<DBName^> ^<OldDBName1^> ^<OldDBName2^>
echo.    Server ::=  ^<target SQL Server^>
echo.    DBName ::=  ^<Name of database to create^>
echo.    OldDBName1 ::= ^<Name of old database to merge^> 
echo.    OldDBName2 ::= ^<Name of another old database to merge^> 
echo.
echo. Example: MergeCWFAssetStoreDB.bat pqocwfddb02 CommonWorkflowAssetStoreDEV CommonWorkflowAssetStorePPE CommonWorkflowAssetStoreFT3

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
cd ..\..\Upgrade\Commands
pause
goto done

REM: finished execution
:finish
echo.
echo Script execution was sucessful.
echo.
cd ..\Commands

:done
@echo on