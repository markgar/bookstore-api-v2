# Bookstore API v2 — Project Specification

## High-Level Summary

Build a production-ready RESTful Web API for a bookstore that supports full CRUD operations on a `Book` resource. The API is built with C# and ASP.NET Core, tested with xUnit, containerized with Docker, and continuously integrated via GitHub Actions.

This document is the **source of truth** for the desired end state of the project. All planning, implementation, and validation should reference this spec.

---

## Tech Stack & Language

| Category          | Choice                                      |
|-------------------|----------------------------------------------|
| Language          | C# 12                                        |
| Runtime           | .NET 8 (LTS)                                 |
| Framework         | ASP.NET Core Web API (minimal hosting model) |
| Testing           | xUnit, FluentAssertions, Microsoft.AspNetCore.Mvc.Testing |
| Containerization  | Docker (multi-stage build)                   |
| CI/CD             | GitHub Actions                               |
| Data Store        | In-memory (Entity Framework Core InMemory provider) |
| API Documentation | Swagger / OpenAPI (Swashbuckle)              |

---

## Features & Requirements

### 1. Book Model

The `Book` entity must have the following properties:

| Property | Type     | Constraints                                      |
|----------|----------|--------------------------------------------------|
| Id       | int      | Auto-generated primary key                       |
| Title    | string   | Required, max length 200                         |
| Author   | string   | Required, max length 150                         |
| ISBN     | string   | Required, must be a valid 13-digit ISBN (ISBN-13) |
| Price    | decimal  | Required, must be > 0                            |
| Genre    | string   | Required, max length 50                          |

### 2. API Endpoints

All endpoints are under the `/api/books` route prefix and return JSON.

| Method | Route             | Description                  | Success Status | Error Status          |
|--------|-------------------|------------------------------|----------------|-----------------------|
| GET    | `/api/books`      | Retrieve all books           | 200 OK         | —                     |
| GET    | `/api/books/{id}` | Retrieve a single book by ID | 200 OK         | 404 Not Found         |
| POST   | `/api/books`      | Create a new book            | 201 Created    | 400 Bad Request       |
| PUT    | `/api/books/{id}` | Update an existing book      | 204 No Content | 400 / 404             |
| DELETE | `/api/books/{id}` | Delete a book                | 204 No Content | 404 Not Found         |

#### Behavior Details

- **GET /api/books** — Returns an array of all books. Returns an empty array `[]` if no books exist.
- **GET /api/books/{id}** — Returns the book object. Returns 404 if the ID does not exist.
- **POST /api/books** — Accepts a JSON body matching the Book model (without `Id`). Returns 201 with a `Location` header pointing to the new resource. Returns 400 if validation fails.
- **PUT /api/books/{id}** — Accepts a full replacement JSON body. The `Id` in the URL must match the `Id` in the body (if provided). Returns 404 if the book does not exist. Returns 400 if validation fails.
- **DELETE /api/books/{id}** — Deletes the book. Returns 204 on success. Returns 404 if the book does not exist.

### 3. Validation

- Use Data Annotations on the model for validation (`[Required]`, `[MaxLength]`, `[Range]`, `[RegularExpression]`).
- ISBN validation: must be exactly 13 digits (regex: `^\d{13}$`).
- Price must be greater than 0.
- The API must return `400 Bad Request` with a `ValidationProblemDetails` response body when validation fails.

### 4. Data Store

- Use **Entity Framework Core** with the **InMemory provider** for simplicity and testability.
- Register `BookstoreDbContext` in DI with `AddDbContext<BookstoreDbContext>(options => options.UseInMemoryDatabase("BookstoreDb"))`.
- No migrations or persistent storage is required.

### 5. Project Structure

```
builder/
├── .github/
│   └── workflows/
│       └── ci.yml
├── src/
│   └── BookstoreApi/
│       ├── Controllers/
│       │   └── BooksController.cs
│       ├── Models/
│       │   └── Book.cs
│       ├── Data/
│       │   └── BookstoreDbContext.cs
│       ├── Program.cs
│       ├── BookstoreApi.csproj
│       └── Properties/
│           └── launchSettings.json
├── tests/
│   └── BookstoreApi.Tests/
│       ├── BooksControllerTests.cs
│       ├── BookModelValidationTests.cs
│       ├── BooksIntegrationTests.cs
│       └── BookstoreApi.Tests.csproj
├── BookstoreApi.sln
├── Dockerfile
├── .gitignore
├── README.md
├── SPEC.md
└── TASKS.md (generated later)
```

### 6. Testing

#### Unit Tests (`BooksControllerTests.cs`)

- Test each controller action method in isolation.
- Mock the `BookstoreDbContext` using the EF Core InMemory provider.
- Cover:
  - `GetAll` returns empty list when no books exist.
  - `GetAll` returns all books.
  - `GetById` returns the correct book.
  - `GetById` returns 404 for non-existent ID.
  - `Create` returns 201 and the created book.
  - `Create` returns 400 for invalid model.
  - `Update` returns 204 for valid update.
  - `Update` returns 404 for non-existent ID.
  - `Delete` returns 204 for existing book.
  - `Delete` returns 404 for non-existent ID.

#### Model Validation Tests (`BookModelValidationTests.cs`)

- Validate that a fully valid `Book` passes validation.
- Validate that missing required fields fail.
- Validate that ISBN with wrong format fails.
- Validate that negative or zero price fails.
- Validate that exceeding max length fails.

#### Integration Tests (`BooksIntegrationTests.cs`)

- Use `WebApplicationFactory<Program>` to spin up an in-memory test server.
- Test full HTTP request/response cycle for each endpoint.
- Test end-to-end flows: create a book, retrieve it, update it, delete it, verify deletion.

### 7. Dockerfile

Multi-stage Docker build:

- **Stage 1 — Build:** Use `mcr.microsoft.com/dotnet/sdk:8.0` as the build image. Copy solution and project files, restore dependencies, then copy all source and publish in Release configuration.
- **Stage 2 — Runtime:** Use `mcr.microsoft.com/dotnet/aspnet:8.0` as the runtime image. Copy published output from Stage 1. Expose port 8080. Set the entry point to the API assembly.

The Dockerfile must:
- Produce a working container that starts the API on port 8080.
- Use `.dockerignore` best practices (or rely on `.gitignore` patterns).
- Leverage Docker layer caching by copying `.csproj` files and restoring before copying full source.

### 8. GitHub Actions CI Workflow (`.github/workflows/ci.yml`)

Trigger on:
- `push` to `main` branch
- `pull_request` to `main` branch

Jobs:

#### Job: `build-and-test`
- Runs on: `ubuntu-latest`
- Steps:
  1. Checkout the repository.
  2. Set up .NET 8 SDK using `actions/setup-dotnet@v4`.
  3. Restore dependencies: `dotnet restore`.
  4. Build the solution: `dotnet build --no-restore --configuration Release`.
  5. Run tests: `dotnet test --no-build --configuration Release --verbosity normal`.

#### Job: `docker-build`
- Runs on: `ubuntu-latest`
- Needs: `build-and-test` (only runs if tests pass)
- Steps:
  1. Checkout the repository.
  2. Build the Docker image: `docker build -t bookstore-api:${{ github.sha }} .`
  3. Verify the image was created: `docker image inspect bookstore-api:${{ github.sha }}`.

---

## Constraints & Guidelines

1. **No external database** — Use EF Core InMemory provider only. No SQL Server, PostgreSQL, SQLite, etc.
2. **No authentication/authorization** — The API is open. Auth is out of scope.
3. **No pagination, filtering, or sorting** — Keep endpoints simple. These are out of scope.
4. **Follow standard ASP.NET Core conventions** — Use controllers (not minimal APIs), dependency injection, and the standard project template structure.
5. **Use controller-based routing** with `[ApiController]` and `[Route("api/[controller]")]` attributes.
6. **Return appropriate HTTP status codes** as defined in the endpoints table above.
7. **Use `System.Text.Json`** for JSON serialization (the ASP.NET Core default).
8. **Target .NET 8** — Use the current LTS release.
9. **Keep the solution self-contained** — No external service dependencies, no config files beyond what ASP.NET Core provides by default.
10. **Code should compile and pass all tests before being considered complete.**

---

## Acceptance Criteria

The project is considered **done** when ALL of the following are true:

- [ ] The solution file `BookstoreApi.sln` exists at the repository root and references both the API project and the test project.
- [ ] `dotnet build` succeeds with zero errors and zero warnings in Release configuration.
- [ ] `dotnet test` runs all unit and integration tests, and all tests pass.
- [ ] The API starts successfully with `dotnet run` and responds to requests on the configured port.
- [ ] All five CRUD endpoints (`GET /api/books`, `GET /api/books/{id}`, `POST /api/books`, `PUT /api/books/{id}`, `DELETE /api/books/{id}`) function correctly and return the specified HTTP status codes.
- [ ] Model validation is enforced: invalid requests return 400 with appropriate error details.
- [ ] At least 10 unit tests exist covering all controller actions and edge cases.
- [ ] At least 5 model validation tests exist.
- [ ] At least 5 integration tests exist covering the full HTTP request/response lifecycle.
- [ ] `docker build .` succeeds and produces a runnable container image.
- [ ] The container starts and the API is accessible on port 8080.
- [ ] The GitHub Actions CI workflow (`.github/workflows/ci.yml`) is present, syntactically valid, and defines both `build-and-test` and `docker-build` jobs.
- [ ] The CI workflow triggers on push and pull request to `main`.
- [ ] The `docker-build` job depends on `build-and-test` completing successfully.
- [ ] All source code follows standard C# naming conventions (PascalCase for public members, camelCase for locals/parameters).
- [ ] The README.md accurately describes the project, how to build, test, and run it.
