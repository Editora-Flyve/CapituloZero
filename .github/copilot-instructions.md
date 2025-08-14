## CapituloZero – AI Agent Working Instructions

Goal: Make an AI agent instantly productive in this modular monolith (.NET 9 + Aspire) while preserving existing patterns.

### 1. Layered + Modular Monolith
- Horizontal layers (current code): `SharedKernel` (primitives), `Domain`, `Application`, `Infrastructure`, `Web.Api` (Minimal APIs), `Web` (Blazor UI), `AppHost/ServiceDefaults` (Aspire wiring).
- Vertical business contexts (planned / emerging modules):
	- Loja (Store): catálogo, carrinho, pedidos, pré‑vendas.
	- Autor (Author Portal): submissão de livros, relatórios, royalties.
	- Terceiros (Partner / Freelancer Portal): tarefas (kanban), pagamentos, contratos.
	- Admin: gestão de pré‑venda, loja, finanças, estoque, etiquetas.
- Each context will group Domain + Application folders logically (e.g. `Domain/Autores`, `Application/Autores/RegisterAutor`). Keep cross‑context coupling only through Application abstractions or future integration events (do NOT directly reference another context’s entities inside a different context’s domain logic).

### 2. Naming & Language Convention
- Code constructs (verbs, framework words) in English; core domain nouns remain Portuguese: e.g. `GetAutorQuery`, `CreatePreVendaCommand`, `AutorRelatorioResponse`.
- Files/folders: PascalCase; keep Portuguese noun inside (avoid translations that lose business meaning).
- Prefer singular for entity folders (`Domain/Autores/Autor.cs`).

### 3. CQRS + Result Pattern
- Commands: `class XCommand : ICommand<T>` + `internal sealed class XCommandHandler : ICommandHandler<XCommand,T>`.
- Queries: `record XQuery(...) : IQuery<T>` optionally, or class; handler mirrors commands.
- Return `Result` / `Result<T>` (no throwing for business rules). Domain / not found / conflict -> use `Error.NotFound|Problem|Conflict`.
- Validation via FluentValidation decorators BEFORE logging decorator (ordering already enforced in `AddApplication`).

### 4. Domain & Events
- Entities inherit `Entity` and raise events via `Raise(new SomethingHappenedDomainEvent(id))`.
- Events persisted then dispatched AFTER save (`ApplicationDbContext.SaveChangesAsync`), meaning handlers must be idempotent and tolerate eventual consistency.
- Put events beside the aggregate (`Domain/<Context>/<Aggregate>...Event.cs`).

### 5. Endpoints (Minimal API)
- One endpoint class per feature: `internal sealed class Create : IEndpoint` inside `Endpoints/<Context>/<Resource>/` when context‑specific, else existing root folders (current Todos/Users illustrate pattern).
- Mapping style: build DTO/request -> command/query -> `handler.Handle(...)` -> `result.Match(Results.Ok|NoContent, CustomResults.Problem)`.
- Use `.RequireAuthorization()` or `.HasPermission("PermissionName")` (permission infra already scaffolded).

### 6. EF Core / Persistence
- Single `ApplicationDbContext` (modular monolith). Keep DbSets grouped by context region comment (add when introducing new sets). Use migrations from Infrastructure project.
- Snake_case naming via `UseSnakeCaseNamingConvention`; default schema `Schemas.Default` (extend later only if necessary—document change if done).
- Migrations: add/update commands:
	- Add: `dotnet ef migrations add <Name> --project src/CapituloZero.Infrastructure --startup-project src/CapituloZero.Web.Api -c ApplicationDbContext`
	- Update DB: same command with `database update`.

### 7. Authentication / Authorization
- JWT bearer (`Jwt:Secret|Issuer|Audience`). Retrieve current user via `IUserContext` (compare IDs like in `CreateTodoCommandHandler`).
- Future context permissions: wrap routes with `.HasPermission("Loja.GerenciarPedidos")` etc once defined.

### 8. Logging & Observability
- Serilog + decorators already emit per-command/query logs with success/failure and pushed error object.
- Aspire + OpenTelemetry packages available; when adding instrumentation prefer using `ServiceDefaults` extensions.

### 9. Adding a New Feature (Template)
1. Domain: entity + errors + events in `Domain/<Context>/<Aggregate>/`.
2. Application: `Application/<Context>/<Feature>/` with Command/Query + Handler + Validator.
3. Infrastructure (if persistence): add DbSet + optional configuration; create migration.
4. Endpoint: `Web.Api/Endpoints/<Context>/<FeatureName>.cs` implementing `IEndpoint`.
5. (Optional UI) Blazor component under `Web/Components/<Context>/` consuming the API.

### 10. Cross-Cutting Conventions
- Keep handlers `internal sealed` (assembly scanning). Avoid public unless part of cross-project API.
- Avoid referencing UI or Infrastructure from Application/Domain (enforced by structure, keep it clean).
- Use Portuguese domain codes in `Error` (e.g. `Error.NotFound("Autores.NotFound", ...)`).
- Consistent async: append `Async` only for public infrastructure services; handlers already imply async context.

### 11. Safety & Refactors
- Do not alter decorator registration order (validation must precede logging).
- Do not move domain event dispatch to BEFORE save unless business requirement justifies (changes transactional semantics).
- Preserve `partial class Program` for tests.

### 12. Quick Run / Dev Loop
- Build: `dotnet build`.
- Run API: `dotnet run --project src/CapituloZero.Web.Api` (applies migrations in Development).
- Run Blazor UI: `dotnet run --project src/CapituloZero.Web`.
- Run Aspire host (optional orchestration): `dotnet run --project src/CapituloZero.AppHost`.

### 13. Test Guidance (currently sparse)
- Prefer testing Application handlers directly (inject in-memory DbContext or use Testcontainers later). Architecture tests project reserved for dependency rules.

Need clarifications? Ask for: permission model, eventing between contexts, UI composition, or test strategy expansion.

