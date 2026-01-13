@echo off
chcp 65001 > nul
echo ========================================
echo   CHẠY MỘC VỊ STORE - ĐƠN GIẢN
echo ========================================
echo.

echo Dừng các tiến trình cũ...
for /f "tokens=5" %%a in ('netstat -ano ^| findstr :5241') do taskkill /PID %%a /F > nul 2>&1
for /f "tokens=5" %%a in ('netstat -ano ^| findstr :8000') do taskkill /PID %%a /F > nul 2>&1

echo.
echo [1/2] Khởi động AI Service...
start "AI Service" cmd /k "cd Trainning_AI && python -m app.main"

timeout /t 3 /nobreak > nul

echo.
echo [2/2] Khởi động Backend...
dotnet run --urls "http://localhost:5241"

pause
