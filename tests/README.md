# QAMS Test Structure

Planned test folders:

- `tests/backend`: unit and integration tests for .NET services. Current baseline includes `Qams.BuildingBlocks.Tests`.
- `tests/contracts`: API and event contract tests.
- `tests/e2e`: end-to-end workflow tests.
- `tests/security`: tenant isolation, RBAC/ABAC, e-signature, and search-trimming tests.
- `tests/compliance`: Part 11, audit trail, retention, export, and validation evidence tests.
- `tests/performance`: load and latency tests.
- `tests/ai-evals`: grounding, citation, safety, privacy, and regulated-boundary AI evaluations.

Run backend tests with:

```powershell
dotnet test src/backend/Qams.sln -m:1
```
