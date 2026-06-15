@echo off
REM Quick setup script for CarWatch Belgium Extension on Windows

echo.
echo 🚀 CarWatch Belgium Extension Setup
echo ====================================
echo.

REM Check Node.js
where node >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ❌ Node.js is not installed. Please install Node.js 18+
    exit /b 1
)

for /f "tokens=*" %%i in ('node --version') do set NODE_VERSION=%%i
echo ✓ Node.js %NODE_VERSION% found
echo.

REM Install dependencies
echo 📦 Installing dependencies...
call npm install
if %ERRORLEVEL% NEQ 0 (
    echo ❌ npm install failed
    exit /b 1
)
echo.

REM Build extension
echo 🔨 Building extension...
call npm run build
if %ERRORLEVEL% NEQ 0 (
    echo ❌ Build failed
    exit /b 1
)
echo.

REM Check if build succeeded
if exist "dist\manifest.json" (
    echo ✅ Build successful!
    echo.
    echo 📂 Extension is ready in 'dist\' folder
    echo.
    echo Next steps:
    echo 1. Open Chrome: chrome://extensions/
    echo 2. Enable 'Developer mode'
    echo 3. Click 'Load unpacked'
    echo 4. Select the 'dist' folder
    echo.
    echo For Edge:
    echo 1. Open Edge: edge://extensions/
    echo 2. Enable 'Developer mode'
    echo 3. Click 'Load unpacked'
    echo 4. Select the 'dist' folder
) else (
    echo ❌ Build failed! Check errors above.
    exit /b 1
)
