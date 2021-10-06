@echo off

echo Starting PlanetsideSeaState API...
call :StartDotnetApp
if %ERRORLEVEL% NEQ 0 (
    echo Failed to start the API. Exiting...
    pause
    exit
)
pause

:StartDotnetApp
cd /d "%~dp0"
dotnet run --verbosity m --configuration Release --project "..\src\PlanetsideSeaState.Api\PlanetsideSeaState.Api.csproj"
exit /B