# TicTacToeAPI

## Описание проекта

REST API для игры в крестики-нолики NxN (где N >= 3) между двумя игроками.
Особенность игры — каждый **третий ход** существует вероятность **10%**, что будет поставлен **знак противника**.

API реализовано на **.NET 9** с поддержкой хранения состояния игры в **PostgreSQL** или в **файловой системе**.

---

## Архитектурные решения

### Структура проекта:

```
TicTacToeAPI
├── Controllers
│   └── GameController.cs
├── Services
│   ├── IGameService.cs
│   └── GameService.cs
├── Repositories
│   ├── Interfaces
│   │   └── IGameRepository.cs
│   ├── FileGameRepository
│   │   └── FileGameRepository.cs
│   └── EfCoreGameRepository
│       └── EfCoreGameRepository.cs
├── Db
│   └── TicTacToeDbContext.cs
├── Models
├── Program.cs
```

### Хранилище:

Выбор хранилища через `STORAGE_TYPE`:

* `FILE` - файловая система
* `DB` - PostgreSQL

### Бизнес-логика:

Сервис `GameService` инкапсулирует:

* игровую механику
* проверку победы
* 10% шанс нечестного хода

---

## 🛠️ Переменные окружения

| Переменная                             | Значение                  | Пример        |
| -------------------------------------- | ------------------------- | ------------- |
| `BOARD_SIZE`                           | Размер поля               | `3`           |
| `WIN_CONDITION`                        | Условие победы (N подряд) | `3`           |
| `STORAGE_TYPE`                         | `FILE` или `DB`           | `DB`          |
| `ConnectionStrings__DefaultConnection` | PostgreSQL                | `Host=db;...` |

---

## 🖋️ Описание API

### `GET /health`

Проверка статуса.
**200 OK**

### `POST api/games`

Создает новую игру.

**Response:**

```json
{
  "gameId": "47958ac6-7a11-45d6-a614-b602869a9250",
  "board": [
    null,
    null,
    null,
    null,
    null,
    null,
    null,
    null,
    null
  ],
  "currentPlayer": "X",
  "status": "Active"
}
```

### `POST api/games/{id}/moves`

Ход игрока.

```json
{
  "row": 0,
  "column": 0,
  "player": "X"
}
```

Идемпотентность: если ход уже отправлялся, вернется тот же ETag.

### `GET api/games/{id}`

Текущее состояние игры.

---

## 🧰 Тесты

* Unit и интеграционные тесты
* Запуск:

```bash
dotnet test
```

* Покрытие > 30%

---

## 🐳 Docker

### docker-compose

```yaml
services:
  db:
    image: postgres
    environment:
      POSTGRES_DB: tictactoe
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
    ports:
      - "5432:5432"

  api:
    build: .
    ports:
      - "8080:8080"
    environment:
      BOARD_SIZE: 3
      WIN_CONDITION: 3
      STORAGE_TYPE: DB
      ConnectionStrings__DefaultConnection: Host=db;Port=5432;Database=tictactoe;Username=postgres;Password=postgres
    depends_on:
      - db
```

Запуск:

```bash
docker-compose up --build
curl http://localhost:8080/health
```

---

## 💾 Сохранение (Crash-safe)

Игра сохраняется в файле или базе данных PostgreSQL.
После рестарта контейнера `GET api/games/{id}` восстанавливает сессию.

---

## 🔗 Пример запуска

```bash
docker-compose up --build
curl http://localhost:8080/health
```
