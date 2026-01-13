@echo off
chcp 65001 > nul
echo ========================================
echo   MỘC VỊ STORE với CHATGPT AI - FULL STACK
echo ========================================
echo.

echo 🤖 Sử dụng ChatGPT cho trải nghiệm chat tốt nhất!
echo 💡 AI sẽ hiểu và giao tiếp tự nhiên như con người
echo.

echo [0/4] Chuẩn bị môi trường...
echo Dừng các tiến trình cũ...
for /f "tokens=5" %%a in ('netstat -ano ^| findstr :5241') do taskkill /PID %%a /F > nul 2>&1
for /f "tokens=5" %%a in ('netstat -ano ^| findstr :8000') do taskkill /PID %%a /F > nul 2>&1
echo ✅ Đã dọn dẹp các tiến trình cũ

echo.
echo [1/4] Cấu hình ChatGPT API...
cd Trainning_AI
if not exist .env (
    powershell -Command "Set-Content -Path '.env' -Value 'OPENAI_API_KEY=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' -Encoding UTF8"
    echo ✅ Đã tạo file .env với OpenAI API Key
) else (
    echo ✅ File .env đã tồn tại
)

echo.
echo [2/4] Cài đặt thư viện OpenAI (nếu chưa có)...
pip show openai > nul 2>&1
if %errorlevel% neq 0 (
    echo Đang cài đặt OpenAI...
    pip install openai > nul 2>&1
    echo ✅ Đã cài đặt OpenAI
) else (
    echo ✅ OpenAI đã được cài đặt
)

echo.
echo [3/4] Khởi động ChatGPT AI Service...
echo - Port: 8000
echo - Model: GPT-4o-mini (Nhanh & Thông minh)
echo - API Docs: http://localhost:8000/docs
echo.
start "ChatGPT AI Service" cmd /k "python -m app.main"

echo Đợi 8 giây để ChatGPT AI khởi động...
timeout /t 8 /nobreak > nul

cd ..
echo.
echo [4/4] Khởi động Web Application...
echo - Port: 5241
echo - URL: http://localhost:5241
echo - Framework: ASP.NET Core 8.0
echo.
start "Web App - Mộc Vị Store" cmd /k "dotnet run"

echo Đợi 10 giây để Web App khởi động...
timeout /t 10 /nobreak > nul

echo.
echo ========================================
echo   HỆ THỐNG ĐÃ KHỞI ĐỘNG VỚI CHATGPT!
echo ========================================
echo.
echo 🌐 Website chính:  http://localhost:5241
echo 🤖 ChatGPT API:    http://localhost:8000/docs
echo 💬 AI Chat Demo:   http://localhost:5241/ai-chat-demo.html
echo 👨‍💼 Admin Panel:    http://localhost:5241/Staff/Dashboard
echo.
echo 🎯 Tính năng ChatGPT:
echo   ✅ Giao tiếp tự nhiên như con người
echo   ✅ Hiểu cảm xúc và ngữ cảnh
echo   ✅ Tư vấn sản phẩm thông minh
echo   ✅ Phản hồi nhanh và chính xác
echo   ✅ Nhớ cuộc trò chuyện
echo.

echo Tự động mở trình duyệt...
start http://localhost:5241

echo.
echo 🎊 Chúc mừng! Website với ChatGPT AI đã sẵn sàng!
echo.
echo 💡 Hướng dẫn sử dụng:
echo   1. Truy cập website: http://localhost:5241
echo   2. Click icon chat ở góc phải màn hình
echo   3. Bắt đầu chat với ChatGPT AI
echo   4. Thử hỏi: "Tôi muốn mua hoa quả sấy"
echo.
echo Nhấn phím bất kỳ để đóng cửa sổ này...
pause > nul
