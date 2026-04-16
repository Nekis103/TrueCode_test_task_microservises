@echo off
chcp 65001 >nul
title TrueCode Currency Service - Docker Start
color 0A

echo ========================================
echo   TrueCode Currency Service
echo   Starting via Docker
echo ========================================
echo.

:: 1. Check Docker
echo [1/6] Checking Docker...
docker --version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] Docker not installed!
    echo Download Docker Desktop: https://www.docker.com/products/docker-desktop/
    pause
    exit /b 1
)
echo [OK] Docker found
echo.

:: 2. Run tests (optional)
echo [2/6] Running tests...
echo.
echo WARNING: Tests will take 1-2 minutes
choice /C YN /M "Run tests before starting services"
if errorlevel 2 (
    echo [SKIP] Tests skipped
    goto :skip_tests
)

echo.
echo Running tests in Docker...
docker-compose -f docker-compose.test.yml up --build --abort-on-container-exit
set TEST_RESULT=%errorlevel%

if %TEST_RESULT% neq 0 (
    echo.
    echo [ERROR] Tests failed!
    echo Do you want to continue starting services?
    choice /C YN /M "Continue starting"
    if errorlevel 2 (
        echo Startup cancelled
        pause
        exit /b 1
    )
) else (
    echo [OK] All tests passed!
)

:: Cleanup after tests
docker-compose -f docker-compose.test.yml down 2>nul

:skip_tests
echo.

:: 3. Stop old containers
echo [3/6] Stopping old containers...
docker-compose down 2>nul
echo [OK] Old containers stopped
echo.

:: 4. Start containers
echo [4/6] Starting all services...
docker-compose up -d
if errorlevel 1 (
    echo [ERROR] Failed to start containers!
    pause
    exit /b 1
)
echo [OK] Services started
echo.

:: 5. Wait for readiness
echo [5/6] Waiting for services to start (15 seconds)...
timeout /t 15 /nobreak >nul
echo [OK] Services ready
echo.

:: 6. Open Swagger
echo [6/6] Opening documentation...
start http://localhost:7232/swagger
start http://localhost:7244/swagger

echo.
echo ========================================
echo   ALL SERVICES STARTED!
echo ========================================
echo.
echo   Gateway:    http://localhost:5000
echo   User:       http://localhost:7232/swagger
echo   Finance:    http://localhost:7244/swagger
echo.
echo   Check:      curl http://localhost:5000/currency
echo.
echo   Stop:       docker-stop.bat
echo.
echo ========================================
pause