# Code Review Findings

- [ ] `.gitignore` (line 36): `launchSettings.json` is listed in `.gitignore`, but SPEC task 17 requires it to be tracked and committed. Remove `launchSettings.json` from `.gitignore` so the file can be committed in a later phase.
- [ ] `BookstoreApi.slnx`: SPEC and acceptance criteria require the solution file to be named `BookstoreApi.sln`, but the repo uses `BookstoreApi.slnx`. Rename to `BookstoreApi.sln` or regenerate with the standard `.sln` format to match the spec.
- [ ] `src/BookstoreApi/Program.cs` (line 21): `app.UseAuthorization()` is included but SPEC explicitly states auth is out of scope. Remove this middleware call to avoid confusion and unnecessary pipeline overhead.
- [ ] `src/BookstoreApi/Program.cs` (line 19): `app.UseHttpsRedirection()` will cause issues when running in the Docker container, which is configured for HTTP on port 8080 only. Remove or conditionalize this middleware to match the deployment target.
