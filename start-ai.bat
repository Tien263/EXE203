@echo off
chcp 65001 > nul
echo ========================================
echo   KHỞI ĐỘNG AI SERVICE (Python FastAPI)
echo ========================================
echo.

echo Đang chuyển đến thư mục AI...
cd Trainning_AI

echo.
echo Kích hoạt virtual environment...
if exist venv\Scripts\activate.bat (
    call venv\Scripts\activate.bat
    echo ✅ Virtual environment đã kích hoạt
) else (
    echo ⚠️  Virtual environment không tồn tại
    echo Đang tạo virtual environment mới...
    python -m venv venv
    call venv\Scripts\activate.bat
    echo.
    echo Đang cài đặt dependencies...
    pip install -r requirements.txt
)

echo.
echo Đang khởi động AI Service...
echo - Framework: FastAPI
echo - Port: 8000
echo - API Docs: http://localhost:8000/docs
echo.

python -m app.main

pause
