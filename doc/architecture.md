
# CapituloZero – Architecture & AI Coding Guide

## Overview

CapituloZero is a modular .NET 9 solution, designed for extensibility, separation of concerns, and maintainability. It consists of multiple projects, each representing a distinct layer in a clean architecture. The front-end uses Blazor Server, while the backend exposes a Minimal API with modular endpoints.

---

## Project Structure & Layer Responsibilities

- **CapituloZero.Web**: Blazor Server UI. Contains Razor components in `Components/`, consumes APIs via `HttpClient` (see `WeatherApiClient.cs`).
- **CapituloZero.Web.Api**: RESTful Minimal API. Endpoints are modular (`Endpoints/`), implement `IEndpoint`, and are registered via `MapEndpoint`. Handles authentication, authorization, health checks, and Swagger.
- **CapituloZero.Application**: Application layer. Orchestrates business logic, implements CQRS (commands/queries), handlers, validation (FluentValidation), and logging (Serilog via decorators). Handlers and validators are auto-registered via assembly scanning.
- **CapituloZero.Domain**: Domain layer. Contains domain entities, domain events, business rules, and aggregates. Events are triggered by entities and processed after persistence.
- **CapituloZero.Infrastructure**: Infrastructure layer. Implements persistence (Entity Framework Core, see `Database/ApplicationDbContext.cs`), authentication, authorization, and other infrastructure services. Domain events are dispatched via `DomainEventsDispatcher` after `SaveChangesAsync`.
- **CapituloZero.ServiceDefaults**: Shared service configuration (telemetry, resilience, service discovery, health checks, OpenTelemetry). See `Extensions.cs` for extension methods.
- **CapituloZero.SharedKernel**: Shared contracts, base types, and utilities (e.g., `Entity`, `ValueObject`, `Result`, `Error`, domain event interfaces).

---

## Architectural Patterns

- **CQRS (Command Query Responsibility Segregation)**: Commands (write) and queries (read) are strictly separated, each with dedicated handlers and validation/logging decorators.
- **Domain Events**: Entities raise domain events, which are dispatched after persistence (see `ApplicationDbContext` and `DomainEventsDispatcher`).
- **Decorators**: Command/query handlers are decorated for validation (FluentValidation) and logging (Serilog), implemented via assembly scanning and DI.
- **Dependency Injection**: All services, handlers, validators, and contexts are registered via extension methods (`AddApplication`, `AddInfrastructure`, `AddPresentation`).
- **Error Handling**: Uses `Result`/`Error` types for flow control; exceptions are reserved for unexpected failures. API error responses are standardized via `CustomResults`.

---

## File & Layer Interactions

- **Blazor UI (`Web`)**: Razor components interact with the API via `HttpClient`. Services are injected using `@inject`. UI logic is separated from data access.
- **API Endpoints (`Web.Api`)**: Endpoints implement `IEndpoint`, use DI to access command/query handlers, and return standardized results. Endpoints are mapped in `MapEndpoint`.
- **Application Layer**: Defines commands/queries (in feature folders), handlers (`XxxCommandHandler`, `XxxQueryHandler`), and validators. Handlers orchestrate domain logic and interact with the domain/entities.
- **Domain Layer**: Entities encapsulate business rules and raise domain events. Events are processed after persistence by handlers implementing `IDomainEventHandler<>`.
- **Infrastructure Layer**: `ApplicationDbContext` manages persistence, applies configurations, and publishes domain events after saving changes. Infrastructure services are registered in `DependencyInjection.cs`.
- **ServiceDefaults**: Provides extension methods for telemetry, resilience, and service discovery, referenced by all service projects.
- **SharedKernel**: Supplies base types, error/result handling, and domain event contracts used across all layers.

---

## Conventions & Best Practices

- **Naming**: Handlers as `XxxCommandHandler`/`XxxQueryHandler`; endpoints by resource/action (e.g., `Todos/Create.cs`).
- **Code Style**: PascalCase for public members, camelCase for private fields/locals, interfaces prefixed with "I". Prefer file-scoped namespaces and single-line using directives.
- **Async/Await**: All I/O is asynchronous.
- **Validation**: Always via FluentValidation, applied as decorators.
- **Logging**: Automatic via Serilog decorators.
- **Error Handling**: Use `Result`/`Error` types for flow control; exceptions only for unexpected failures.
- **XML Doc Comments**: Required for public APIs, with `<example>` and `<code>` when relevant.

---

## Extensibility & Integration

- New modules follow existing folder/class patterns; DI and assembly scanning handle registration automatically.
- Infrastructure services are extended via methods in `DependencyInjection.cs`.
- Cross-cutting concerns (validation, logging) are added via decorators, not inside handlers.
- For new features:
	1. Add entities/events in `Domain`.
	2. Create commands/queries, handlers, and validators in `Application`.
	3. Update DbContext/configuration in `Infrastructure`.
	4. Implement endpoint in `Web.Api`.
	5. Create Blazor component to consume/display data.

---

## AI-Ready Guidelines

- All architectural conventions, file structures, and code patterns are designed for automated code generation and extension by AI agents.
- Modular boundaries and clear responsibilities enable safe, incremental code synthesis.
- Extension points (DI, decorators, endpoints, validators) are explicit and discoverable.
- All new code must respect separation of concerns, layer boundaries, and registration conventions.

---

## Restrictions

- Razor Pages and MVC are not used; only Blazor Server for UI.
- External libraries may be introduced if open-source and compatible with the architecture.
- Modularity and separation of concerns must always be respected.

---

This guide is the canonical reference for architecture, extensibility, and AI-driven code generation in CapituloZero.
