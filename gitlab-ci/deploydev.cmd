@echo off

set SRC_PATH=..\%1
if "%SRC_PATH%"=="" set SRC_PATH=%~dp0\..\%PRODUCT_NAME%\bin\Release

set SERVICE_NAME=%2
if "%SERVICE_NAME%"=="" set SERVICE_NAME=RMon.Service

set SERVER_NAME=%3
if "%SERVER_NAME%"=="" set SERVER_NAME=osr-dev-test

set SERVICE_PATH=%~4
if "%SERVICE_PATH%"=="" set SERVICE_PATH=\\OSR-DEV-TEST\c$\Program Files (x86)\RMon4\Site204Dev\RMon.Service

sc \\%SERVER_NAME% stop %SERVICE_NAME%

set /A tries=5
:retry
if %tries% LEQ 0 GOTO error
set /A tries-=1

echo Pause for 10 seconds
powershell.exe -nologo -noprofile -command "Start-Sleep -s 10"
echo Publishing from %SRC_PATH% to %SERVICE_PATH%

for /f "tokens=*" %%f in (deploydev.files) do (
	xcopy /s /y /e "%SRC_PATH%\%%f" "%SERVICE_PATH%\%%f"
)
if errorlevel 4 goto retry

sc \\%SERVER_NAME% start %SERVICE_NAME%
goto exit

:error
echo Error while copying
exit 4

:exit