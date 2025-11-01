@echo off
echo ========================================
echo DKSKMaui Test Runner
echo ========================================
echo.

REM Run the PowerShell script
powershell.exe -ExecutionPolicy Bypass -File "%~dp0run-tests.ps1" -OpenReport

REM Check if PowerShell execution was successful
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ERROR: Test execution failed!
    echo Please check the output above for details.
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo Tests completed successfully!
pause
