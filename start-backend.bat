@echo off
chcp 65001 > nul
echo ========================================
echo   KHỞI ĐỘNG BACKEND (ASP.NET Core)
echo ========================================
echo.

echo Đang khởi động Web Application...
echo - Framework: ASP.NET Core 8.0
echo - Port: 5241
echo - URL: http://localhost:5241
echo.

dotnet run

pause
