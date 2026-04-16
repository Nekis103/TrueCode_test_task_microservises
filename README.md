# TrueCode Currency Service

Тестовое задание: микросервисная система для управления курсами валют и избранными валютами пользователей.

## Технологии

| Технология | Назначение |
|------------|------------|
| .NET 8 | Основная платформа |
| PostgreSQL 15 | База данных |
| Docker | Контейнеризация |
| JWT | Аутентификация |
| Ocelot | API Gateway |
| MediatR | CQRS |
| Entity Framework Core | ORM |

## Требования

- **Docker Desktop** (запущен) - [Скачать](https://www.docker.com/products/docker-desktop/)
- **.NET 8 SDK** - [Скачать](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Windows 10/11** (рекомендуется)

## Быстрый старт

### 1. Распакуйте архив с проектом

### 2. Запустите `docker_start.bat` от имени администратора

Скрипт автоматически:
- Предоставит выбор пройти unit тесты или нет (около 2х минут)
- Проверяет наличие Docker и .NET 8
- Запускает PostgreSQL в контейнере
- Применяет миграции базы данных
- Запускает UserService, FinanceService и Gateway
- Открывает Swagger документацию

**Ожидание:** ~30 секунд

### 3. Остановка сервисов

- `docker-stop.bat` - остановка всех контейнеров
- `docker-compose down -v` - полная очистка (удаление всех данных)

## API Эндпоинты

### Swagger документация

| Сервис | URL |
|--------|-----|
| **UserService** | http://localhost:7232/swagger |
| **FinanceService** | http://localhost:7244/swagger |

### Gateway (порт 5000)

| Метод | URL | Описание | Auth |
|-------|-----|----------|------|
| GET | `/currency` | Получить все валюты | ❌ |
| GET | `/currency/{code}` | Получить валюту по коду (USD, EUR) | ❌ |
| POST | `/auth/register` | Регистрация пользователя | ❌ |
| POST | `/auth/login` | Логин (получение JWT токена) | ❌ |
| GET | `/auth/me` | Информация о текущем пользователе | ✅ |
| GET | `/favorites` | Получить избранные валюты | ✅ |
| POST | `/favorites/{code}` | Добавить валюту в избранное | ✅ |
| DELETE | `/favorites/{code}` | Удалить валюту из избранного | ✅ |

## Тестирование

### Через Swagger (рекомендуется)

1. Откройте **http://localhost:7232/swagger**;
2. Выполните `POST /api/auth/register` - создайте пользователя;
3. Выполните `POST /api/auth/login` - скопируйте полученный токен;
4. Нажмите кнопку **Authorize** (замок в правом верхнем углу);
5. Вставьте токен **без слова Bearer** (Swagger добавит сам);
6. Выполняйте запросы требующие авторизации.

### Через Postman (командная строка)

```bash
В корне проект расположен файл для импорт в Postman:
v2 - TrueCode Currency ServiceV2.postman_collection.json
v2.1 - TrueCode Currency ServiceV2.1.postman_collection.json
```

### Архитектура
```
Client (Swagger/Postman)
    │
    ▼
Gateway (порт 5000) - Ocelot
    │
    ├──► UserService (порт 7232)
    │       ├── Регистрация
    │       ├── Логин (JWT)
    │       └── Избранное
    │
    └──► FinanceService (порт 7244)
            ├── Курсы валют
            │
            └── Фоновый сервис
                    │
                    ▼
                cbr.ru (обновление каждые 60 минут)
```

## Архитектурное решение

### Текущая реализация (для упрощения проверки)
- Используется одна база данных PostgreSQL (удобство проверки)
- **UserService**:
  - Отвечает за пользователей и избранное
  - Имеет доступ к таблице `Currencies` (только для связи с избранным)
  - Хранит `UserFavoriteCurrencies` с `CurrencyId`
- **FinanceService**:
  - Отвечает за курсы валют
  - Не имеет доступа к таблицам `Users` и `UserFavoriteCurrencies`
  - Фоновый сервис загружает курсы с cbr.ru каждые 60 минут
- **SharedKernel**:
  - Содержит общие сущности: `User`, `Currency`, `UserFavoriteCurrency`
  - Используется обоими сервисами для единой модели данных
- **Коммуникация между сервисами (HTTP)**:
  - UserService → FinanceService: проверка существования валюты
  - FinanceService → UserService: получение списка избранного пользователя

### В production среде рекомендуется
- Разделить базы данных:
  - **UserService DB**: `users`, `user_favorites` (хранить `currency_code`)
  - **FinanceService DB**: `currencies`
- Убрать таблицу `Currencies` из UserService полностью
- Хранить в избранном только `currency_code` (string) без прямой связи на `Currency`
- Вся коммуникация остаётся на HTTP/gRPC (уже реализовано)