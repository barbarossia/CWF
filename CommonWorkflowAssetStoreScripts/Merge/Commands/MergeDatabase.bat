@echo off
REM: Command File Created 
REM: Date Generated: 5/12/2013
REM: Authentication type: Windows NT

echo Please input the server you want to connect.
set /p server=
if '%server%'=='' goto errors
echo.
echo Please input the database name you want to?
set /p database=
if '%database%'=='' goto errors
sqlcmd -S %server% -d master -E -b -i "..\..\Setup\CheckIfNotExists.sql" -v DBName = '%database%'
if %ERRORLEVEL% NEQ 0 goto errors
echo.

echo Please input the database name you want from?
set /p mergeDB= 
if '%mergeDB%'=='' goto errors	
sqlcmd -S %server% -d master -E -b -i "..\..\Setup\CheckIfNotExists.sql" -v DBName = '%mergeDB%'
if %ERRORLEVEL% NEQ 0 goto errors 
echo.

echo server=%server%
echo database=%database%
echo mergeDB=%mergeDB%


REM: if %ERRORLEVEL% NEQ 0 goto errors
:table alter
echo "***********************************************************"
echo "***** Alter Tables on %database%
echo "***********************************************************"

CD "..\Tables"

CALL Update.BAT %server% %database%
if %ERRORLEVEL% NEQ 0 goto errors

:copy data
echo "***********************************************************"
echo "***** Copying Tables on %database%
echo "***********************************************************"

CD "..\Data"

CALL CopyDatabase.BAT %server% %database% %mergeDB%
if %ERRORLEVEL% NEQ 0 goto errors

CD "..\Clear"

CALL Update.BAT %server% %database%
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
cd ..\..\Merge\Commands
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