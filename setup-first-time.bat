@echo off
chcp 65001 > nul
echo ========================================
echo   THIẾT LẬP LẦN ĐẦU - MỘC VỊ STORE
echo ========================================
echo.

echo Hướng dẫn này sẽ giúp bạn thiết lập dự án lần đầu tiên.
echo.
echo Yêu cầu:
echo - .NET 8.0 SDK
echo - SQL Server
echo - Python 3.8+
echo.
pause

echo.
echo ========================================
echo   BƯỚC 1: KIỂM TRA MÔI TRƯỜNG
echo ========================================
echo.

echo Kiểm tra .NET SDK...
dotnet --version
if %errorlevel% neq 0 (
    echo ❌ .NET SDK chưa được cài đặt
    echo Tải tại: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)
echo ✅ .NET SDK đã cài đặt

echo.
echo Kiểm tra Python...
python --version
if %errorlevel% neq 0 (
    echo ❌ Python chưa được cài đặt
    echo Tải tại: https://www.python.org/downloads/
    pause
    exit /b 1
)
echo ✅ Python đã cài đặt

echo.
echo ========================================
echo   BƯỚC 2: RESTORE PACKAGES (.NET)
echo ========================================
echo.

dotnet restore
if %errorlevel% neq 0 (
    echo ❌ Restore packages thất bại
    pause
    exit /b 1
)
echo ✅ Restore packages thành công

echo.
echo ========================================
echo   BƯỚC 3: BUILD PROJECT
echo ========================================
echo.

dotnet build
if %errorlevel% neq 0 (
    echo ❌ Build thất bại
    pause
    exit /b 1
)
echo ✅ Build thành công

echo.
echo ========================================
echo   BƯỚC 4: SETUP AI SERVICE
echo ========================================
echo.

cd Trainning_AI

echo Tạo virtual environment...
python -m venv venv
if %errorlevel% neq 0 (
    echo ❌ Tạo virtual environment thất bại
    pause
    exit /b 1
)
echo ✅ Virtual environment đã tạo

echo.
echo Kích hoạt virtual environment...
call venv\Scripts\activate.bat

echo.
echo Cài đặt Python packages...
pip install -r requirements.txt
if %errorlevel% neq 0 (
    echo ❌ Cài đặt packages thất bại
    pause
    exit /b 1
)
echo ✅ Python packages đã cài đặt

cd ..

echo.
echo ========================================
echo   BƯỚC 5: CẤU HÌNH
echo ========================================
echo.

echo Kiểm tra file cấu hình...
if not exist "appsettings.json" (
    echo ❌ Không tìm thấy appsettings.json
    pause
    exit /b 1
)
echo ✅ appsettings.json tồn tại

echo.
echo Kiểm tra .env cho AI Service...
if not exist "Trainning_AI\.env" (
    echo ⚠️  Chưa có file .env
    echo.
    echo Tạo file Trainning_AI\.env với nội dung:
    echo GEMINI_API_KEY=your_api_key_here
    echo.
    echo Lấy API key tại: https://makersuite.google.com/app/apikey
    echo.
    pause
) else (
    echo ✅ File .env đã tồn tại
)

echo.
echo ========================================
echo   BƯỚC 6: DATABASE
echo ========================================
echo.

echo Chạy migrations...
dotnet ef database update
if %errorlevel% neq 0 (
    echo ⚠️  Migration thất bại
    echo.
    echo Kiểm tra:
    echo 1. SQL Server đang chạy
    echo 2. Connection string trong appsettings.json
    echo.
    pause
) else (
    echo ✅ Database đã được tạo
)

echo.
echo ========================================
echo   THIẾT LẬP HOÀN TẤT!
echo ========================================
echo.

echo Các bước tiếp theo:
echo.
echo 1. Cấu hình Email trong appsettings.json
echo 2. Cấu hình Google OAuth (optional)
echo 3. Thêm GEMINI_API_KEY vào Trainning_AI\.env
echo 4. Insert dữ liệu mẫu:
echo    sqlcmd -S localhost -d MocViStoreDB -i SQL_Scripts\InsertProductsData.sql -f 65001
echo.
echo 5. Chạy hệ thống:
echo    start-all.bat
echo.

pause
