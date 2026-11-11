# .NET 8 Todo API

A small CRUD API organized into five projects:

```text
DotNet8TodoApi/
├── src/
│   ├── API/              # ASP.NET Core host and HTTP endpoints
│   ├── Metadata/         # API name, version, descriptions, and tags
│   ├── Data/             # Repository implementations and DI registration
│   └── Domain/           # Domain entities and repository contracts
├── tests/
│   └── IntegrationTest/  # End-to-end HTTP integration tests
├── Dockerfile
└── DotNet8TodoApi.sln
```

## Prerequisites

- .NET 8 SDK
- Docker (optional)

## Run locally

```bash
dotnet restore
dotnet run --project src/API/API.csproj
```

The HTTP profile listens on `http://localhost:5080`.

## Test

```bash
dotnet test
```

## Docker

Build and run from the repository root:

```bash
docker build -t dotnet8-todo-api .
docker run --rm -p 8080:8080 dotnet8-todo-api
```

Then call `http://localhost:8080/health` or the Todo endpoints under `http://localhost:8080/api/todos`.

## Endpoints

| Method | Route | Purpose |
|---|---|---|
| GET | `/` | API information |
| GET | `/health` | Health probe |
| GET | `/api/todos` | List all todo items |
| GET | `/api/todos/{id}` | Get one todo item |
| POST | `/api/todos` | Create a todo item |
| PUT | `/api/todos/{id}` | Update a todo item |
| DELETE | `/api/todos/{id}` | Delete a todo item |

Example request:

```bash
curl -X POST http://localhost:5080/api/todos \
  -H "Content-Type: application/json" \
  -d '{"title":"Create a .NET 8 API"}'
```

> The included repository is in-memory, so data resets whenever the process restarts. Replace `InMemoryTodoRepository` in the Data project with a database-backed implementation when persistence is required.
