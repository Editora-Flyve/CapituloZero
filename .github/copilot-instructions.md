## CapituloZero – AI Coding Agent Quick Guide
Purpose: Minimum, high‑value context so an agent can contribute safely and fast. Keep answers/action aligned with these patterns; extend, don’t reinvent.

### 1. Architecture Snapshot
Solution projects:
- AppHost (Aspire orchestrator) – boots Postgres, waits for MigrationService then WebApp.
- MigrationService – applies EF Core migrations + seeds Identity roles (Cliente, Autor, Parceiro, Admin) then exits (idempotent; extend seeding here only).
- WebApp – ASP.NET Core + Blazor Hybrid host (Server + WASM render modes) + Identity endpoints.
- WebApp.Client – UI components, pages, services (Auth, antiforgery) consumed by WebApp.
- Infra.IdentityApp – EF Core + ASP.NET Identity (`ApplicationDbContext`, migrations, DI extension `AddIdentityUser`).
- Domain – Core abstractions (`Entity` with Guid V7, `ValueObject`). Put business rules here.
- ServiceDefaults – Cross‑cutting setup (OpenTelemetry, health, resilience) via `AddServiceDefaults()`.

### 2. Runtime Flow
AppHost → starts Postgres resource → runs MigrationService (blocks) → starts WebApp (interactive server + WASM) → users hit Identity + custom Blazor pages.

### 3. Data & Migrations
Only modify schema in Infra.IdentityApp + Domain:
1. Add/adjust entity/value object in Domain.
2. Add `DbSet` to `ApplicationDbContext`.
3. Create migration: `dotnet ef migrations add <Name> -p src/CapituloZero.Infra.IdentityApp -s src/CapituloZero.AppHost`.
4. Run AppHost – MigrationService applies it automatically (don’t run `database update` manually in normal flow).

### 4. Identity & Auth Pattern
Cookie auth (scheme: Application). WebApp maps `app.MapAdditionalIdentityEndpoints()`.
Email sender is no‑op (`IdentityNoOpEmailSender`).
Roles seeded once per startup by MigrationService – keep seeding idempotent.

### 5. Blazor Client/Server Conventions
Hybrid: Server hosts both Server + WASM interactivity (`AddInteractiveServerComponents`, `AddInteractiveWebAssemblyComponents`).
Client assembly added to server via `.AddAdditionalAssemblies(typeof(CapituloZero.WebApp.Client._Imports).Assembly)`.
Shared UI resides under `WebApp.Client` (Components/Pages/Services). Avoid direct EF DbContext usage in components; create scoped services if needed.

### 6. Auth UI / HTTP Access Refactor (Important)
Use centralized services (in `WebApp.Client/Services`):
- `IAntiforgeryTokenProvider` caches `/antiforgery` token thread‑safely.
- `IAuthApi` wraps login & register endpoints returning `ApiResult<T>` (uniform success/error contract).
- `AuthComponentBase` supplies `IsSubmitting`, `ErrorMessage`, and `ExecuteSubmitAsync(Func<Task>)` – derive auth pages/components from it instead of duplicating state logic.
Register all via `AddClientAppServices()` in BOTH WebApp.Client `Program.cs` and WebApp `Program.cs` (for server-side interactive components). Do not re‑instantiate HttpClient manually; base address is set once using `NavigationManager.BaseUri`.

### 7. Domain Modeling Rules
Use `Guid.CreateVersion7()` (already in `Entity`) – don’t swap to random/new Guid.
Value objects: implement equality via `GetEqualityComponents()` yield sequence (see `ValueObject.cs`).
Keep business invariants within Domain; UI/Infra should orchestrate only.

### 8. Telemetry & Resilience
Always call `AddServiceDefaults()` early in new service startup. Custom Activity sources: follow MigrationService `Worker.cs` pattern.
Health endpoints: `/health`, `/alive` (excluded from tracing by default).

### 9. Adding a New Service / Worker
1. New project → reference Domain + Infra (if DB) + ServiceDefaults.
2. Call `builder.AddServiceDefaults();` then `builder.AddIdentityUser();` if it needs the shared DB.
3. Register in AppHost with dependencies (`.WithReference(postgresdb)` and `.WaitFor(...)`).

### 10. Common Do / Avoid
Do: Extend seeding in MigrationService with existence checks.
Do: Centralize outbound HTTP wrappers (pattern used by `IAuthApi`).
Do: Use `AddClientAppServices()` for any new client-side service families.
Avoid: Direct `HttpClient` BaseAddress assignments inside components (already injected).
Avoid: Calling `dotnet ef database update` in normal multi-project runtime.
Avoid: Embedding secrets; prefer env vars (design-time factory currently hardcoded – future improvement).

### 11. Quick Commands
Build all: `dotnet build` (root).
Run full stack: `dotnet run --project src/CapituloZero.AppHost`.
Add migration: `dotnet ef migrations add <Name> -p src/CapituloZero.Infra.IdentityApp -s src/CapituloZero.AppHost`.

### 12. When Unsure
Prefer following existing extension patterns (e.g., create `AddXyzServices()`), keep seeding idempotent, and ask before introducing new architectural layers (CQRS, MediatR, etc.).

---
If something here seems stale (e.g., additional services added) request clarification and propose an update to this guide.
