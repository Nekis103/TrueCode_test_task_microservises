@echo off
chcp 65001 >nul
title TrueCode Currency Service - Docker Stop
color 0C

echo ========================================
echo   TrueCode Currency Service
echo   Остановка через Docker
echo ========================================
echo.

echo [1/3] Остановка основных сервисов...
docker-compose down
echo [OK] Сервисы остановлены
echo.

echo [2/3] Остановка тестовых контейнеров...
docker-compose -f docker-compose.test.yml down 2>nul
echo [OK] Тестовые контейнеры остановлены
echo.

echo [3/3] Очистка неиспользуемых ресурсов...
docker system prune -f
echo [OK] Очистка завершена
echo.

echo ========================================
echo   ВСЕ КОНТЕЙНЕРЫ ОСТАНОВЛЕНЫ!
echo ========================================
echo.
pause