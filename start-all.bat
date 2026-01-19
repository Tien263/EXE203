@echo off
chcp 65001 > nul
echo ========================================
echo   MOC VI STORE voi CHATGPT AI - FULL STACK
echo ========================================
echo.

echo Dung ChatGPT cho trai nghiem chat tot nhat!
echo.

echo [0/4] Chuan bi moi truong...
echo Dung cac tien trinh cu...
for /f "tokens=5" %%a in ('netstat -ano ^| findstr :5241') do taskkill /PID %%a /F > nul 2>&1
for /f "tokens=5" %%a in ('netstat -ano ^| findstr :8000') do taskkill /PID %%a /F > nul 2>&1
echo OK - Da don dep cac tien trinh cu

echo.
echo [1/4] Cau hinh ChatGPT API...
cd Trainning_AI
if not exist .env (
    (
        echo OPENAI_API_KEY=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
    ) > .env
    echo OK - Da tao file .env voi OpenAI API Key
) else (
    echo OK - File .env da ton tai
)

echo.
echo [2/4] Kich hoat virtual environment va cai dat dependencies...
if exist venv\Scripts\activate.bat (
    call venv\Scripts\activate.bat
    echo OK - Virtual environment da kich hoat
) else (
    echo Dang tao virtual environment moi...
    python -m venv venv
    call venv\Scripts\activate.bat
)

echo Dang cai dat/cap nhat dependencies...
pip install -r requirements.txt > nul 2>&1
echo OK - Dependencies da san sang

echo.
echo [3/4] Khoi dong ChatGPT AI Service...
echo - Port: 8000
echo - Model: GPT-4o-mini
echo - API Docs: http://localhost:8000/docs
echo.
start "ChatGPT AI Service" cmd /k "call venv\Scripts\activate.bat && uvicorn app.main:app --host 0.0.0.0 --port 8000 --reload"

echo Doi 8 giay de ChatGPT AI khoi dong...
timeout /t 8 /nobreak > nul

cd ..
echo.
echo [4/4] Khoi dong Web Application...
echo - Port: 5241
echo - URL: http://localhost:5241
echo - Framework: ASP.NET Core 8.0
echo.
start "Web App - MOC VI STORE" cmd /k "dotnet run"

echo Doi 10 giay de Web App khoi dong...
timeout /t 10 /nobreak > nul

echo.
echo ========================================
echo   HE THONG DA KHOI DONG!
echo ========================================
echo.
echo Website chinh: http://localhost:5241
echo ChatGPT API: http://localhost:8000/docs
echo.

echo Dang mo trinh duyet voi cache disabled...
echo [INFO] Browser se tu dong load file JavaScript moi nhat!
start chrome --disable-cache --disk-cache-size=1 http://localhost:5241

echo.
echo Chuc mung! Website da san sang!
echo.
pause > nul
