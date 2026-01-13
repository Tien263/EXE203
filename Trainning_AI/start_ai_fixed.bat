@echo off
chcp 65001 > nul
echo Khoi dong ChatGPT AI Service...

REM Set environment variable directly
set OPENAI_API_KEY=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

echo API Key: %OPENAI_API_KEY:~0,20%...
echo.

python -m app.main

pause
