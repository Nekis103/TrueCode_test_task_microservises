@echo off
chcp 65001 >nul
title TrueCode Currency Service - Docker Start
color 0A

echo ========================================
echo   TrueCode Currency Service
echo   Запуск через Docker
echo ========================================
echo.

:: 1. Проверка Docker
echo [1/6] Проверка Docker...
docker --version >nul 2>&1
if errorlevel 1 (
    echo [ОШИБКА] Docker не установлен!
    echo Скачайте Docker Desktop: https://www.docker.com/products/docker-desktop/
    pause
    exit /b 1
)
echo [OK] Docker найден
echo.

:: 2. Запуск тестов (опционально, можно пропустить)
echo [2/6] Запуск тестов...
echo.
echo ВНИМАНИЕ: Запуск тестов займет 1-2 минуты
choice /C YN /M "Запустить тесты перед стартом сервисов"
if errorlevel 2 (
    echo [SKIP] Тесты пропущены
    goto :skip_tests
)

echo.
echo Запуск тестов в Docker...
docker-compose -f docker-compose.test.yml up --build --abort-on-container-exit
set TEST_RESULT=%errorlevel%

if %TEST_RESULT% neq 0 (
    echo.
    echo [ОШИБКА] Тесты не пройдены!
    echo Хотите продолжить запуск сервисов?
    choice /C YN /M "Продолжить запуск"
    if errorlevel 2 (
        echo Запуск отменен
        pause
        exit /b 1
    )
) else (
    echo [OK] Все тесты пройдены успешно!
)

:: Очистка после тестов
docker-compose -f docker-compose.test.yml down 2>nul

:skip_tests
echo.

:: 3. Остановка старых контейнеров этого проекта
echo [3/6] Остановка старых контейнеров...
docker-compose down 2>nul
echo [OK] Старые контейнеры остановлены
echo.

:: 4. Запуск контейнеров
echo [4/6] Запуск всех сервисов...
docker-compose up -d
if errorlevel 1 (
    echo [ОШИБКА] Не удалось запустить контейнеры!
    pause
    exit /b 1
)
echo [OK] Сервисы запущены
echo.

:: 5. Ожидание готовности
echo [5/6] Ожидание запуска сервисов (15 секунд)...
timeout /t 15 /nobreak >nul
echo [OK] Сервисы готовы
echo.

:: 6. Открытие Swagger
echo [6/6] Открытие документации...
start http://localhost:7232/swagger
start http://localhost:7244/swagger

echo.
echo ========================================
echo   ВСЕ СЕРВИСЫ ЗАПУЩЕНЫ!
echo ========================================
echo.
echo   Gateway:    http://localhost:5000
echo   User:       http://localhost:7232/swagger
echo   Finance:    http://localhost:7244/swagger
echo.
echo   Проверка:   curl http://localhost:5000/currency
echo.
echo   Остановка:  docker-stop.bat
echo.
echo ========================================
pause