# TaskRtUpdater

TaskRtUpdater is a real-time task management backend built with **.NET 8**, **PostgreSQL**, and **WebSockets**, designed following **Clean Architecture principles** with a strong focus on code quality, separation of concerns, and domain-driven design.

---

## 🌍 Public API URL (Railway)

```
https://taskrtupdater-production.up.railway.app
```

Swagger UI:

```
https://taskrtupdater-production.up.railway.app/swagger
```

Websocket subscription:

```
(download ws-test-html and run it normally; press F12 to see real time actions from all other users)
```

Database URL:

```
psql -h shortline.proxy.rlwy.net -U postgres -p 12002 -d railway -f [Script Name].postgres.sql (password is needed)
```

---

## 🚀 Summary

- RESTful API built with ASP.NET Core (.NET 8). Create, update, list or delete tasks, and view critical path
- PostgreSQL database
- Real-time notifications via WebSockets
- Clean Architecture (Domain, Application, Infrastructure, Presentation)
- Repository pattern
- Observer pattern for real-time updates
- Directed acyclic graph for dependencies, with depth topological search
- Use cases (application services)
- Custom domain exceptions
- Fully async / await

---

## 📦 REST API Endpoints

### Create a task

**POST** `/api/tasks`

```json
{
  "title": "Initial task",
  "description": "First task of the project",
  "priority": 10,
  "duration": 3,
  "dependencies": [2, 3]
}
```

---

### Get all tasks

**GET** `/api/tasks`

Returns all tasks including their dependencies.

---

### Get task by id

**GET** `/api/tasks/{id}`

---

### Update task status

**PUT** `/api/tasks/{id}`

```json
{
  "status": "InProgress"
}
```

Notes:
- A task cannot move to `InProgress` or `Done` if any dependency is not `Done`
- A task marked as `Done` cannot be reverted

---

### WebSocket Events

- Task created
- Task status updated
- Real-time broadcast to all connected clients

---

## 🧠 Architecture Overview

The project strictly follows **Clean Architecture**:

```
src/
├── Domain
│   ├── Entities
│   ├── Exceptions
│   └── Interfaces
│
├── Enums
│
├── Application
│   ├── DTOs
│   └── UseCases
│
├── Infrastructure
│   ├── Data (EF Core / PostgreSQL)
│   ├── Repositories
│   └── Observer (WebSocket notifications)
│
├── Presentation
│   ├── Controllers
│   └── Middleware
│
├── Scripts
│
└── UnitTests
```

---

## 🧩 Design Patterns Used

- **Repository Pattern** – database abstraction
- **Observer Pattern** – WebSocket real-time notifications
- **Abstract Factory** – infrastructure service creation
- **Dependency Injection** – loose coupling

---

## 🛡️ Code Quality Features

- No business logic in controllers
- All rules enforced at domain/use-case level
- Strong typing and enum safety
- Explicit error handling with custom exceptions
- Fully async database access
- Environment-based configuration
- Production-ready PostgreSQL defaults
- Consistent and testable architecture

---

## 🛠️ Tech Stack

- .NET 8 / C#
- ASP.NET Core Web API
- PostgreSQL
- WebSockets
- Swagger / OpenAPI
- Railway (for deployment)

---

## 👤 Author

**Sergio Juan Rollán Moralejo**

