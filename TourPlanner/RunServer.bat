@echo off
echo ===================================
echo TOUR PLANNER API SERVER STARTUP
echo ===================================
echo.
echo This batch file will run the server and keep the console window open.
echo Press Ctrl+C to stop the server when finished.
echo.
echo Starting server...
echo.
dotnet run --project "%~dp0"
echo.
echo Server stopped.
pause 