@echo off
REM: Command File Created 
REM: Date Generated: 5/12/2013
REM: Authentication type: Windows NT

echo Please input the server you want to connect.
set /p server=
if '%server%'=='' goto errors
echo.
echo Please input the database you want to create.
set /p database=
if '%database%'=='' goto errors
sqlcmd -S %server% -d master -E -b -i "CheckIfExists.sql" -v DBName = '%database%'
if %ERRORLEVEL% NEQ 0 goto errors
echo.

set count=0
:configLoop 
	echo Please input the database which you want to migrated?
	set /p existsDB= 
	if '%existsDB%'=='' goto errors	
	sqlcmd -S %server% -d master -E -b -i "CheckIfNotExists.sql" -v DBName = '%existsDB%'
	if %ERRORLEVEL% NEQ 0 goto errors 
	set Obj[%count%].database=%existsDB%
	echo.
	choice /c 1234 /m "Please select one environment: dev, test, stage, prod"
	if %errorlevel% ==1 set Obj[%count%].envir=dev
	if %errorlevel% ==2 set Obj[%count%].envir=test
	if %errorlevel% ==3 set Obj[%count%].envir=stage
	if %errorlevel% ==4 set Obj[%count%].envir=prod
	echo.	
	choice /m "Do you want to continue"
	if %errorlevel%==1 (
				set /a count+=1
				goto configLoop) 

echo server=%server%
echo database=%database%

REM: if %ERRORLEVEL% NEQ 0 goto errors
echo "*********************************************************************"
echo "***** Executing MergeCommonWorkflowAssetStore DB Scripts" on %server%
echo "*********************************************************************"

CD ".."
CD "..\Setup"

sqlcmd -S %server% -d master -E -b -i "CommonWorkflowAssetStoreDBCreate.sql" -v DBName = '%database%'
if %ERRORLEVEL% NEQ 0 goto errors

sqlcmd -S %server% -d %database% -E -b -i "CWFAssetStoreUserRole.sql"
if %ERRORLEVEL% NEQ 0 goto errors

:table create
echo "***********************************************************"
echo "***** Creating Tables on %database%
echo "***********************************************************"

CD "..\Merge\Tables"

CALL Update.BAT %server% %database%
if %ERRORLEVEL% NEQ 0 goto errors


echo "***********************************************************"
echo "***** Populating objects in  %database%
echo "***********************************************************"

CD "..\Data"

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

:migration data
echo "***********************************************************"
echo "***** Migerating Tables on %database%
echo "***********************************************************"

CD "..\Merge\Data"

SET Obj_Index=0

:LoopStart
IF %Obj_Index% GTR %count% GOTO :other

SET Obj_Current.database=0
SET Obj_Current.envir=0

FOR /F "usebackq delims==. tokens=1-3" %%I IN (`SET Obj[%Obj_Index%]`) DO (
     SET Obj_Current.%%J=%%K
)

CD "..\Data"

CALL Copy.BAT %server% %database% %Obj_Current.database% %Obj_Current.envir%
if %ERRORLEVEL% NEQ 0 goto errors

CD "..\Update"

CALL Update.BAT %server% %database% %Obj_Current.envir%
if %ERRORLEVEL% NEQ 0 goto errors

SET /A Obj_Index=%Obj_Index% + 1

GOTO LoopStart


:other
echo "***********************************************************"
echo "***** creating Contraints on  %database%
echo "***********************************************************"

CD ".."
CD "..\Constraints"

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibrary.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created constraint ActivityLibrary.sql"

sqlcmd -S %server% -d %database% -E -b -i "Activity.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created constraint Activity.sql"

sqlcmd -S %server% -d %database% -E -b -i "WorkflowType.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created constraint WorkflowType.sql"

sqlcmd -S %server% -d %database% -E -b -i "ActivityCategory.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created constraint ActivityCategory.sql"

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibraryDependency.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created constraint ActivityLibraryDependency.sql"

sqlcmd -S %server% -d %database% -E -b -i "TaskActivity.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created constraint TaskActivity.sql"

sqlcmd -S %server% -d %database% -E -b -i "Permission.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created constraint Permission.sql"

sqlcmd -S %server% -d %database% -E -b -i "RoleEnvPermission.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created constraint RoleEnvPermission.sql"

sqlcmd -S %server% -d %database% -E -b -i "AuthorizationGroup.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created constraint AuthorizationGroup.sql"

echo "***********************************************************"
echo "***** creating types on  %database%
echo "***********************************************************"

CD "..\Types"

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibraryTableType.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created type ActivityLibraryTableType"

sqlcmd -S %server% -d %database% -E -b -i "AuthGroupNameTableType.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created type AuthGroupNameTableType"

sqlcmd -S %server% -d %database% -E -b -i "EnvironmentTableType.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Created type EnvironmentTableType"

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

:copy clear
echo "***********************************************************"
echo "***** Clearing Tables on %database%
echo "***********************************************************"

CD "..\Merge\Clear"

sqlcmd -S %server% -d %database% -E -b -i "Activity.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Cleared table Activity

sqlcmd -S %server% -d %database% -E -b -i "ActivityCategory.sql" 
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Cleared table ActivityCategory

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibrary.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Cleared table ActivityLibrary

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibraryDependency.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Cleared table ActivityLibraryDependency

sqlcmd -S %server% -d %database% -E -b -i "ActivityLibraryDependencyHead.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Cleared table ActivityLibraryDependencyHead

sqlcmd -S %server% -d %database% -E -b -i "AuthorizationGroup.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Cleared table AuthorizationGroup

sqlcmd -S %server% -d %database% -E -b -i "TaskActivity.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Cleared table TaskActivity

sqlcmd -S %server% -d %database% -E -b -i "WorkflowType.sql"
if %ERRORLEVEL% NEQ 0 goto errors
echo "   ***** Cleared table WorkflowType

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