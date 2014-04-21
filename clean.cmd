@echo off
setlocal
call setproject.cmd %1 %2 %3 %4
@echo justcleaned>justcleaned.dat
%msbuild% %buildpath%\TFSBuild.proj /t:Clean @%buildpath%\tfsbuild.rsp  /clp:verbosity=Normal /p:ProjectItems=%ProjectItems% /p:BuildConfigs="%DevConfig%|%DevPlat%"  /flp:LogFile=msbuild.%1.log;Verbosity=normal;Encoding=ASCII;append  /fl1 /flp1:warningsonly;logfile=msbuild.%1.wrn;Encoding=ASCII;append /fl2  /flp2:errorsonly;logfile=msbuild.%1.err;Encoding=ASCII;append
endlocal
