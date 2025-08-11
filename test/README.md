# Tests

This folder contains unit tests, application tests, and architecture tests.

Projects:
- CapituloZero.SharedKernel.Tests: Core primitives like Result, Entity, ValueObject.
- CapituloZero.Domain.Tests: Domain entities behavior.
- CapituloZero.Application.Tests: Handlers and use-cases with EFCore InMemory.
- CapituloZero.Web.Api.Tests: Endpoint discovery and minimal web checks.
- CapituloZero.ArchitectureTests: Dependency rules across layers.

Run the whole suite with coverage:

```powershell
# Build
dotnet build .\CapituloZero.sln -c Debug
# Test with coverage
dotnet test .\CapituloZero.sln -c Debug --collect:"XPlat Code Coverage"
```
