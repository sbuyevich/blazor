@echo off
setlocal

set "CLASS_CODE=%~1"
if "%CLASS_CODE%"=="" (
    echo Error: CLASS_CODE is required.
    echo Usage: %~nx0 demo
    exit /b 1
)

powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0run.ps1" -ClassCode "%CLASS_CODE%"
exit /b %ERRORLEVEL%
