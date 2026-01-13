@echo off
chcp 65001 > nul
echo ========================================
echo   MỘC VỊ STORE với CHATGPT AI
echo ========================================
echo.

echo 🤖 Sử dụng ChatGPT cho trải nghiệm chat tốt nhất!
echo 💡 AI sẽ hiểu và giao tiếp tự nhiên như con người
echo.

echo Dừng các tiến trình cũ...
for /f "tokens=5" %%a in ('netstat -ano ^| findstr :5241') do taskkill /PID %%a /F > nul 2>&1
for /f "tokens=5" %%a in ('netstat -ano ^| findstr :8000') do taskkill /PID %%a /F > nul 2>&1

echo.
echo [1/3] Tạo file .env với OpenAI API Key...
cd Trainning_AI
if not exist .env (
    copy .env.example .env > nul 2>&1
    echo ✅ Đã tạo file .env
) else (
    echo ✅ File .env đã tồn tại
)

echo.
echo [2/3] Khởi động ChatGPT AI Service...
start "ChatGPT AI Service" cmd /k "python -m app.main"

timeout /t 5 /nobreak > nul

cd ..
echo.
echo [3/3] Khởi động Web Application...
start "Web App" cmd /k "dotnet run"

echo.
echo ========================================
echo   HỆ THỐNG ĐÃ KHỞI ĐỘNG VỚI CHATGPT!
echo ========================================
echo.
echo 🌐 Website:     http://localhost:5241
echo 🤖 ChatGPT AI:  http://localhost:8000/docs
echo 💬 AI Chat:     http://localhost:5241/ai-chat-demo.html
echo.
echo 🎯 Tính năng ChatGPT:
echo   ✅ Giao tiếp tự nhiên như con người
echo   ✅ Hiểu cảm xúc và ngữ cảnh
echo   ✅ Tư vấn sản phẩm thông minh
echo   ✅ Phản hồi nhanh và chính xác
echo.

timeout /t 10 /nobreak > nul
start http://localhost:5241

echo Nhấn phím bất kỳ để đóng...
pause > nul
