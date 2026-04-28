@echo off
setlocal

set "CLASS_CODE=%~1"
if "%CLASS_CODE%"=="" (
    set "CLASS_CODE=demo"
)

set "PORT=%~2"
if "%PORT%"=="" (
    set "PORT=5555"
)

powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0run.ps1" -ClassCode "%CLASS_CODE%" -Port "%PORT%"
exit /b %ERRORLEVEL%
