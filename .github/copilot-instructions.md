# AI Coding Agent Instructions for CapituloZero

Purpose: Give an AI assistant just enough project-specific context to be productive quickly while staying consistent with existing patterns.

## 1. Solution & High-Level Architecture
Projects (see `CapituloZero.sln`):
- CapituloZero.AppHost: .NET Aspire distributed app orchestrator (`AppHost.cs`) wiring Postgres + migration + web app.
- CapituloZero.MigrationService: Headless worker that runs EF Core migrations and seeds Identity roles, then exits.
- CapituloZero.WebApp: Server-side ASP.NET Core + Blazor Hybrid host (server + WASM) plus Identity UI endpoints.
- CapituloZero.WebApp.Client: Client-side interactive components referenced by the server for WASM render mode.
- CapituloZero.Infra.IdentityApp: EF Core + ASP.NET Identity infrastructure (DbContext, migrations, DI helpers, ApplicationUser).
- CapituloZero.Domain: Domain base abstractions (`Entity`, `ValueObject`). (Currently minimal — extend here, not in Infra.)
- CapituloZero.ServiceDefaults: Cross-cutting defaults (OpenTelemetry, health checks, service discovery, resilience) used by every service.

Runtime flow:
1. `CapituloZero.AppHost` starts; provisions Postgres container + volume; launches MigrationService (waits), then WebApp (waits for DB + migrations completion).
2. MigrationService migrates + ensures roles (Cliente, Autor, Parceiro, Admin) then stops.
3. WebApp starts with Identity + MudBlazor + interactive components (server + WASM) and maps identity endpoints.

## 2. Key Conventions & Patterns
- Database access only via `ApplicationDbContext` in Infra project. Add new DbSets + migrations there.
- Register data layer through `builder.AddIdentityUser()` (see `DIExtensions.cs`) which internally configures Npgsql with connection name `capitulozero-db` (Aspire resource name).
- IDs: `Entity` constructor uses `Guid.CreateVersion7()` for ordered UUIDs; keep this for new aggregates.
- Domain model rules: Place domain logic (invariants, value objects) inside `CapituloZero.Domain`; avoid putting business logic in controllers/pages.
- Value objects must implement `GetEqualityComponents()` via `yield return` pattern; follow `ValueObject` example.
- Telemetry/health: Always call `builder.AddServiceDefaults()` early in new services to ensure OpenTelemetry + resilience + health endpoints.
- Migrations: Do NOT run `dotnet ef database update` manually in normal flow; rely on MigrationService during distributed startup. For iterative local dev you can still scaffold migrations normally.

## 3. Adding / Modifying Data Model
1. Edit / add entities or value objects in `CapituloZero.Domain`.
2. Add corresponding `DbSet<T>` to `ApplicationDbContext` (Infra project).
3. Create migration (inside Infra project): `dotnet ef migrations add <Name> -p src/CapituloZero.Infra.IdentityApp -s src/CapituloZero.AppHost` (AppHost supplies Aspire hosting env vars) OR use MigrationService as startup.
4. Let AppHost run again; MigrationService will apply it automatically.

## 4. Running & Debugging
- Preferred: Run the distributed app (`CapituloZero.AppHost` as startup project). It orchestrates Postgres + services.
- Direct WebApp run: Possible, but migrations/roles must already exist or you’ll see Identity errors.
- Health endpoints (dev only): `/health`, `/alive` (exclude from tracing by design).
- WebAssembly debugging enabled only in Development via `app.UseWebAssemblyDebugging()`.

## 5. Authentication & Identity
- Identity configured with cookies (default scheme `IdentityConstants.ApplicationScheme`).
- Account endpoints auto-mapped: `app.MapAdditionalIdentityEndpoints()`.
- Email sender is a no-op (`IdentityNoOpEmailSender`); features requiring confirmation will need replacement later.
- Default roles seeded in MigrationService — extend seeding there (keep idempotent checks using `RoleManager`).

## 6. Telemetry & Resilience
- OpenTelemetry tracing + metrics added centrally (ASP.NET Core, HTTP Client, runtime). Excludes health endpoints.
- Optional OTLP exporter requires `OTEL_EXPORTER_OTLP_ENDPOINT` in configuration.
- Add custom Activity sources: follow pattern in `Worker.cs` (static ActivitySource + StartActivity).

## 7. Adding a New Service
1. Create new project (e.g., Background worker). Reference `CapituloZero.ServiceDefaults` and any needed domain/infra projects.
2. Call `builder.AddServiceDefaults();` then any custom registrations.
3. If DB access needed: call `builder.AddIdentityUser();` or create a similar extension if using a different DbContext.
4. Expose health endpoints only if appropriate; follow pattern in existing services.
5. Wire into `AppHost` via `builder.AddProject<Projects.YourProject>("your-service")` and define dependencies with `.WithReference(postgresdb)` / `.WaitFor(...)`.

## 8. Postgres & Connection Handling
- Postgres resource named `postgres-capitulozero`; database resource `capitulozero-db` consumed by services via Aspire connection name.
- Design-time factory (`ApplicationDbContextFactory`) currently hardcodes a localhost connection string (including credentials). Consider env var substitution if rotating secrets.

## 9. Blazor / UI Structure
- Server project (`CapituloZero.WebApp`) hosts both Server and WASM interactive modes: `.AddInteractiveServerComponents()` + `.AddInteractiveWebAssemblyComponents()`.
- Client project (`CapituloZero.WebApp.Client`) holds shared UI components & routes; imported into server via `.AddAdditionalAssemblies(...)` using `_Imports` assembly reference.
- Add new shared components under `CapituloZero.WebApp.Client/Components` and reference namespaces in `_Imports.razor` if needed.

## 10. Safe Extension Guidelines for AI
- Keep cross-cutting concerns centralized: Do not duplicate telemetry or health config per service.
- Don’t bypass `AddIdentityUser()`; update that extension if connection or migrations assembly changes.
- When adding roles or seeding, maintain idempotency (check existence first) to keep MigrationService safe for repeated runs.
- Favor Value Objects for multi-field concepts that require equality logic; avoid primitive obsession.
- Avoid leaking Infra (EF) types (DbContext) into Blazor components directly; prefer scoped services wrapping data access (pattern not yet implemented—introduce cleanly if needed).

## 11. Missing or Thin Areas (Opportunities)
- No tests yet: when adding, colocate under a new `tests/` folder (xUnit recommended) and reference domain first.
- README is empty—augment with run instructions mirroring sections 3–5.
- Secrets currently embedded in design-time factory; replace with env-based configuration in future PR.

## 12. Quick Commands (local dev)
- Restore & build all: `dotnet build` (root).
- Run full distributed environment: set AppHost as startup: `dotnet run --project src/CapituloZero.AppHost`.
- Add migration: `dotnet ef migrations add InitialCreate -p src/CapituloZero.Infra.IdentityApp -s src/CapituloZero.AppHost`.

---
If any section is unclear or you need deeper guidance (e.g., introducing repositories, CQRS, testing strategy), ask for refinement before large changes.
