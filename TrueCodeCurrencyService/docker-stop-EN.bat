@echo off
chcp 65001 >nul
title TrueCode Currency Service - Docker Stop
color 0C

echo ========================================
echo   TrueCode Currency Service
echo   Stopping via Docker
echo ========================================
echo.

echo [1/3] Stopping main services...
docker-compose down
echo [OK] Services stopped
echo.

echo [2/3] Stopping test containers...
docker-compose -f docker-compose.test.yml down 2>nul
echo [OK] Test containers stopped
echo.

echo [3/3] Cleaning unused resources...
docker system prune -f
echo [OK] Cleanup completed
echo.

echo ========================================
echo   ALL CONTAINERS STOPPED!
echo ========================================
echo.
pause