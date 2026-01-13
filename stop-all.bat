@echo off
chcp 65001 > nul
echo ========================================
echo   DỪNG TẤT CẢ DỊCH VỤ
echo ========================================
echo.

echo [1/2] Dừng Web Application (Port 5241)...
for /f "tokens=5" %%a in ('netstat -ano ^| findstr :5241') do (
    taskkill /PID %%a /F > nul 2>&1
)
echo ✅ Web Application đã dừng

echo.
echo [2/2] Dừng AI Service (Port 8000)...
for /f "tokens=5" %%a in ('netstat -ano ^| findstr :8000') do (
    taskkill /PID %%a /F > nul 2>&1
)
echo ✅ AI Service đã dừng

echo.
echo ========================================
echo   TẤT CẢ DỊCH VỤ ĐÃ DỪNG
echo ========================================
echo.
pause
