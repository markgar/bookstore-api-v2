# Bookstore API v2

A C# ASP.NET Core Web API for managing a bookstore inventory. Provides full CRUD endpoints for books with attributes including title, author, ISBN, price, and genre.

## Features

- **CRUD Endpoints** — Create, Read, Update, and Delete books via a RESTful API.
- **Book Model** — Each book has a title, author, ISBN, price, and genre.
- **xUnit Tests** — Comprehensive unit and integration tests using xUnit.
- **Dockerized** — Multi-stage Dockerfile for optimized container builds.
- **CI/CD** — GitHub Actions workflow for continuous integration (build, test, Docker build).

## Tech Stack

- **Language:** C# (.NET 8)
- **Framework:** ASP.NET Core Web API
- **Testing:** xUnit, Microsoft.AspNetCore.Mvc.Testing
- **Containerization:** Docker (multi-stage build)
- **CI:** GitHub Actions

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/) (optional, for containerized runs)

### Run Locally

```bash
cd src/BookstoreApi
dotnet run
```

### Run Tests

```bash
dotnet test
```

### Build Docker Image

```bash
docker build -t bookstore-api .
docker run -p 8080:8080 bookstore-api
```

## API Endpoints

| Method | Endpoint         | Description          |
|--------|------------------|----------------------|
| GET    | /api/books       | List all books       |
| GET    | /api/books/{id}  | Get a book by ID     |
| POST   | /api/books       | Create a new book    |
| PUT    | /api/books/{id}  | Update an existing book |
| DELETE | /api/books/{id}  | Delete a book        |

## License

MIT
