# TASKS — Bookstore API v2

This task list tracks all work needed to implement the project described in SPEC.md.

---

## Phase 1: Project Scaffolding

- [x] 1. Create the solution file `BookstoreApi.sln` at the repository root.
- [x] 2. Create the API project `src/BookstoreApi/BookstoreApi.csproj` targeting .NET 8, using the ASP.NET Core Web API template with controllers (not minimal APIs). Remove any default scaffolding (e.g., `WeatherForecast` controller/model) that does not belong to this project.
- [x] 3. Create the test project `tests/BookstoreApi.Tests/BookstoreApi.Tests.csproj` targeting .NET 8, using xUnit. Add package references for `FluentAssertions`, `Microsoft.AspNetCore.Mvc.Testing`, and a project reference to `src/BookstoreApi`.
- [x] 4. Add both projects to the solution file so `dotnet build` and `dotnet test` work from the repo root.
- [x] 5. Verify the solution builds with zero errors: `dotnet build --configuration Release`.

## Phase 2: Book Model & Data Layer

- [x] 6. Create `src/BookstoreApi/Models/Book.cs` with the `Book` entity containing properties `Id` (int, auto-generated PK), `Title` (string, Required, MaxLength 200), `Author` (string, Required, MaxLength 150), `ISBN` (string, Required, regex `^\d{13}$`), `Price` (decimal, Required, Range > 0), and `Genre` (string, Required, MaxLength 50). Use Data Annotations for all validation constraints.
- [x] 7. Create `src/BookstoreApi/Data/BookstoreDbContext.cs` with a `DbSet<Book> Books` property, inheriting from `DbContext`.
- [x] 8. Register `BookstoreDbContext` in `Program.cs` using `AddDbContext<BookstoreDbContext>(options => options.UseInMemoryDatabase("BookstoreDb"))`. Add the required NuGet package `Microsoft.EntityFrameworkCore.InMemory`.
- [x] 9. Verify the solution still builds cleanly.

## Phase 3: API Controller & Endpoints

- [x] 10. Create `src/BookstoreApi/Controllers/BooksController.cs` with `[ApiController]` and `[Route("api/[controller]")]` attributes. Inject `BookstoreDbContext` via constructor.
- [x] 11. Implement `GET /api/books` — returns `200 OK` with all books (empty array if none).
- [x] 12. Implement `GET /api/books/{id}` — returns `200 OK` with the book, or `404 Not Found` if not found.
- [x] 13. Implement `POST /api/books` — accepts a Book JSON body (without Id), returns `201 Created` with a `Location` header, or `400 Bad Request` if validation fails.
- [x] 14. Implement `PUT /api/books/{id}` — accepts a full replacement body, returns `204 No Content` on success, `404 Not Found` if the book doesn't exist, or `400 Bad Request` if validation fails. If an `Id` is provided in the body it must match the URL `id`.
- [x] 15. Implement `DELETE /api/books/{id}` — returns `204 No Content` on success, or `404 Not Found` if the book doesn't exist.

## Phase 4: Program.cs Configuration

- [x] 16. Ensure `Program.cs` uses the minimal hosting model, registers controllers (`AddControllers`), maps controllers (`MapControllers`), and configures Swagger/OpenAPI with Swashbuckle (`AddEndpointsApiExplorer`, `AddSwaggerGen`, `UseSwagger`, `UseSwaggerUI`). Add the `Swashbuckle.AspNetCore` NuGet package.
- [x] 17. Create `src/BookstoreApi/Properties/launchSettings.json` configuring the API to listen on an appropriate port for local development. Remove `launchSettings.json` from `.gitignore` or ensure the file is tracked (SPEC expects it committed).
- [x] 18. Verify the API starts with `dotnet run` and responds to a request (e.g., `GET /api/books` returns `[]`).

## Phase 5: Unit Tests — Controller

- [ ] 19. Create `tests/BookstoreApi.Tests/BooksControllerTests.cs` with unit tests using an EF Core InMemory database. Implement at minimum:
    - `GetAll` returns empty list when no books exist.
    - `GetAll` returns all seeded books.
    - `GetById` returns the correct book.
    - `GetById` returns `404` for a non-existent ID.
    - `Create` returns `201` and the created book.
    - `Create` returns `400` for an invalid model.
    - `Update` returns `204` for a valid update.
    - `Update` returns `404` for a non-existent ID.
    - `Delete` returns `204` for an existing book.
    - `Delete` returns `404` for a non-existent ID.

## Phase 6: Unit Tests — Model Validation

- [ ] 20. Create `tests/BookstoreApi.Tests/BookModelValidationTests.cs` with at least 5 tests:
    - A fully valid `Book` passes validation.
    - Missing required fields (Title, Author, ISBN, Genre) fail validation.
    - ISBN with wrong format (e.g., letters, wrong length) fails validation.
    - Negative or zero `Price` fails validation.
    - Exceeding `MaxLength` on `Title`, `Author`, or `Genre` fails validation.

## Phase 7: Integration Tests

- [ ] 21. Create `tests/BookstoreApi.Tests/BooksIntegrationTests.cs` using `WebApplicationFactory<Program>` with at least 5 tests:
    - `GET /api/books` returns `200` and an empty array initially.
    - `POST /api/books` creates a book and returns `201` with a `Location` header.
    - `GET /api/books/{id}` retrieves the created book.
    - `PUT /api/books/{id}` updates the book and returns `204`.
    - `DELETE /api/books/{id}` deletes the book and returns `204`; subsequent GET returns `404`.
- [ ] 22. Ensure `Program` class is accessible to the test project (e.g., add `InternalsVisibleTo` or make `Program` partial/public).

## Phase 8: Run & Fix All Tests

- [ ] 23. Run `dotnet test --configuration Release --verbosity normal` from the repo root. Fix any failing tests or compilation errors until all tests pass with zero failures.

## Phase 9: Dockerfile

- [ ] 24. Create `Dockerfile` at the repository root with a multi-stage build:
    - **Stage 1 (build):** Use `mcr.microsoft.com/dotnet/sdk:8.0`. Copy `.csproj` files and restore first (layer caching). Then copy all source, publish in Release configuration.
    - **Stage 2 (runtime):** Use `mcr.microsoft.com/dotnet/aspnet:8.0`. Copy published output. Expose port 8080. Set `ASPNETCORE_URLS=http://+:8080`. Set entry point to `BookstoreApi.dll`.
- [ ] 25. Create a `.dockerignore` file (or verify existing `.gitignore` patterns are sufficient) to exclude `bin/`, `obj/`, `.git/`, etc.

## Phase 10: GitHub Actions CI Workflow

- [ ] 26. Create `.github/workflows/ci.yml` with:
    - Triggers on `push` to `main` and `pull_request` to `main`.
    - **Job `build-and-test`:** runs on `ubuntu-latest`. Steps: checkout, setup .NET 8 SDK (`actions/setup-dotnet@v4`), `dotnet restore`, `dotnet build --no-restore --configuration Release`, `dotnet test --no-build --configuration Release --verbosity normal`.
    - **Job `docker-build`:** runs on `ubuntu-latest`, `needs: build-and-test`. Steps: checkout, `docker build -t bookstore-api:${{ github.sha }} .`, `docker image inspect bookstore-api:${{ github.sha }}`.

## Phase 11: Final Verification

- [ ] 27. Run `dotnet build --configuration Release` — must succeed with zero errors and zero warnings.
- [ ] 28. Run `dotnet test --configuration Release --verbosity normal` — all tests must pass (≥10 controller tests, ≥5 model tests, ≥5 integration tests).
- [ ] 29. Run `dotnet run --project src/BookstoreApi` and manually verify `GET /api/books` returns `200 OK` with `[]`.
- [ ] 30. Validate the Dockerfile builds successfully: `docker build -t bookstore-api:test .` (if Docker is available).
- [ ] 31. Validate the CI workflow YAML is syntactically correct (e.g., using `yamllint` or a YAML parser).
- [ ] 32. Review README.md — ensure it accurately describes how to build, test, run locally, and run via Docker. Update if needed.
- [ ] 33. Final end-to-end walkthrough: create a book via POST, retrieve it via GET, update it via PUT, delete it via DELETE, and confirm deletion — all returning the correct status codes.
