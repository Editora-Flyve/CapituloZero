---
description: 'Guidelines for building C# applications'
applyTo: '**/*.cs'
---
# CapituloZero – AI Coding Agent Guidelines

## Big Picture

- Modular .NET 9 solution with clear separation by layers: Web (Blazor Server UI), Web.Api (Minimal API), Application, Domain, Infrastructure, ServiceDefaults, SharedKernel.
- Follows CQRS: commands (write) and queries (read) are always separated, with dedicated handlers and validation/logging decorators.
- Domain Events are used for business events, triggered by entities and processed after persistence.
- Dependency injection is managed via extension methods (`AddApplication`, `AddInfrastructure`, `AddPresentation`).

## Developer Workflows

- Build and run via standard .NET CLI (`dotnet build`, `dotnet run`), solution file is [`CapituloZero.sln`](CapituloZero.sln ).
- API endpoints are modular: implement `IEndpoint`; the app discovers them via `services.AddEndpoints(Assembly)` and maps them with `app.MapEndpoints()`.
- Blazor components live in `Web/Components` and consume APIs via `HttpClient` (see `WeatherApiClient` for example).
- Data access uses Entity Framework Core, with context and configuration in `Infrastructure`.
- Handlers, validators, and services are auto-registered via assembly scanning.
 - Authentication uses JWT; authorization uses dynamic permission policies. Use `.RequireAuthorization()` and `.HasPermission("perm")` on endpoints as needed.
 - Prefer returning `result.Match(Results.Ok, CustomResults.Problem)` from endpoints to standardize error shapes.

## Project-Specific Conventions

- Use PascalCase for public members, camelCase for private fields/local variables, and prefix interfaces with "I".
- Prefer file-scoped namespaces, single-line using directives, and code style from `.editorconfig`.
- Always use async/await for I/O.
- Error handling: use `Result`/`Error` types for flow control; exceptions only for unexpected failures.
- Naming: `XxxCommandHandler`, `XxxQueryHandler`, endpoints by resource/action (e.g., `Todos/Create.cs`).
- XML doc comments required for public APIs, with `<example>` and `<code>` when relevant.
 - EF Core: PostgreSQL via Npgsql, snake_case naming; default schema is `public`.

## Integration & Extensibility

- New modules follow existing folder and class patterns; DI and assembly scanning handle registration.
- Infrastructure services are extended via methods in `DependencyInjection.cs`.
- Cross-cutting concerns (validation, logging) are added via decorators, not inside handlers.

## Feature Implementation Flow

1. Add entities/events in `Domain`.
2. Create commands/queries, handlers, and validators in `Application`.
3. Update DbContext/configuration in `Infrastructure`.
4. Implement endpoint in `Web.Api`.
5. Create Blazor component to consume/display data.

## Examples

- Endpoints: see `Web.Api/Endpoints`.
- Blazor components: see `Web/Components`.
- API client: see `Web/WeatherApiClient.cs`.

## Restrictions

- Do not generate code for Razor Pages or MVC; use Blazor Server only.
- When adding dependencies, prefer Microsoft-supported packages first (for example: Microsoft.*, System.*, Azure.*, AspNetCore.*).
- Only introduce third-party open-source libraries when there is no solid Microsoft option and they are compatible with the project's architecture and licensing.
- Always respect modularity and separation of concerns.