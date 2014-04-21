SET BranchRoot=%cd%

::Set this to override _Branch_props.proj
::set LocalPropertiesPrefix=_Branch

::set msbuildver to override the default.
if "%msbuildver%"=="" set msbuildver=v4.0.30319
if "%msbuild%"=="" set msbuild=%windir%\Microsoft.NET\Framework\%msbuildver%\msbuild.exe
if /i "%DevBuildItems%"=="" set DevBuildItems=SolutionToBuild;MSIToBuild
set RunUnitTests=true
if exist justcleaned.dat set skiptools=true
set DefaultPlat=Any CPU
Set DefaultFlavor=Debug
set buildpath=..\_build
if exist \\pqotfspvh02\Tools\UpdateBranchTools.cmd call \\pqotfspvh02\Tools\UpdateBranchTools.cmd
if not exist %BuildPath%\_Build_Props.proj echo An Error has occurred while updating the build tools. Please sync %BuildPath% manually
if /i "%1"=="rel" set DevConfig=Release
if /i "%1"=="dbg" set DevConfig=Debug
if /i "%1"=="all" set DevConfig=All
if "%1"=="" set DevConfig=%DefaultFlavor%&Set DevPlat=%DefaultPlat%
if not "%2"=="" set Devplat=%2
if "%2"=="" set DevPlat=%DefaultPlat%
if /i "%2"=="all" set DevPlat=All
set BuildType=Desktop
set StopOnTestFailure=false
