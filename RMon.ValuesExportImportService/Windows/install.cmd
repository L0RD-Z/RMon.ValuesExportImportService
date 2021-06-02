@setlocal enableextensions enabledelayedexpansion
@echo off
set CUR_PATH=%~dp0
set file=service.info
set currarea=
for /f "usebackq delims=" %%a in ("!CUR_PATH!\!file!") do (
    set ln=%%a
    if "x!ln:~0,1!"=="x[" (
        set currarea=!ln!
    ) else (
        for /f "tokens=1,2 delims==" %%b in ("!ln!") do (
            set currkey=%%b
            set currval=%%c
            set "!currkey!=!currval!"
        )
    )
)

set prefix=%~1
set SERVICE_NAME=%prefix%%SERVICE_NAME%
set SERVICE_DISPLAY_NAME=%prefix%%SERVICE_DISPLAY_NAME%

sc create "%SERVICE_NAME%" binPath= "%CUR_PATH%%SERVICE_EXE%.exe" DisplayName= "%SERVICE_DISPLAY_NAME%" start= auto
sc description "%SERVICE_NAME%" "%SERVICE_DESCRIPTION%"
sc failure "%SERVICE_NAME%" reset= 0 actions= restart/60000

endlocal
