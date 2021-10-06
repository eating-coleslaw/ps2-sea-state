@echo off

echo Building the .NET API...
call :BuildDotnetApp
if %ERRORLEVEL% NEQ 0 (
    echo Failed to build the API. Exiting...
    pause
    exit
)
call :OnSuccessfulBuild
pause
exit


:OnSuccessfulBuild
choice /M "Build completed successfully. Start the app now?" /T 30 /D N
if %ERRORLEVEL% EQU 1 (
    echo Starting the API...
    cd /d "%~dp0"
    call "RunApp.bat"
    exit
)
if %ERRORLEVEL% EQU 2 (
    echo Did not start the API. Run RunApp.bat as administrator to start the API.
    pause
    exit
)

:BuildDotnetApp
cd /d "%~dp0"
dotnet build "..\PlanetsideSeaState.sln" --verbosity m --configuration Release
exit /B