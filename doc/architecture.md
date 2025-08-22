# CapituloZero - Arquitetura (Modular Monolith)

## Visão Geral
CapituloZero é um monolito modular em .NET 9 (Aspire) que concentra múltiplos contextos de negócio isolados logicamente (Loja, Autor, Terceiros, Admin) dentro do mesmo processo e banco (por enquanto). A UI (Blazor Server) expõe uma loja pública; após login, o usuário pode navegar para outros portais conforme permissões.

Principais objetivos:
- Evoluir rápido sem complexidade inicial de microsserviços.
- Facilitar futura extração de contextos (boundary claro hoje = serviço independente amanhã).
- Reutilizar infraestrutura (observabilidade, auth, persistence) com baixo acoplamento entre domínios.

## Camadas (Horizontais)
- SharedKernel: Primitivos (`Entity`, `Result`, `Error`, eventos, value objects futuros).
- Domain: Modelo puro + eventos + erros, organizado por contexto (`Domain/Autores`, `Domain/Loja`).
- Application: Casos de uso (Commands / Queries / Handlers / Validators / Decorators) segregados por contexto.
- Infrastructure: EF Core (`ApplicationDbContext`), auth JWT, permissões, dispatcher de domain events, serviços técnicos.
- Web.Api: Minimal API modular (endereços HTTP -> Commands/Queries) via interface `IEndpoint` e reflexão.
- Web: Blazor Server (componentes por contexto em `Components/<Contexto>`).
- AppHost / ServiceDefaults: Aspire (telemetria, service discovery, resiliency policies) para orquestração local.

## Contextos (Verticais Planejados)
- Loja: catálogo, carrinho, pedidos, pré‑vendas.
- Autor: submissão de livros, relatórios, royalties.
- Terceiros: kanban de tarefas, pagamentos, contratos.
- Admin: gestão de pré‑vendas, loja, finanças, estoque, etiquetas.

Cada contexto agrega seu domínio + aplicação; evitar uso direto de entidades de outro contexto. Se precisar interagir, usar IDs ou (futuramente) eventos de integração.

## Padrões-Chave
- CQRS: `XCommand : ICommand<T>` / `XQuery : IQuery<T>` + handlers internos selados retornando `Result`.
- Result Pattern: Falhas de negócio nunca lançam exceção—retornar `Result.Failure(Error)` com `ErrorType` adequado.
- Domain Events: Acumulados em `Entity.DomainEvents`; publicados APÓS `SaveChangesAsync` (eventual consistency, exigir idempotência de handlers).
- Decorators: Validação (FluentValidation) antes de Logging (Serilog). Ordem preservada em `AddApplication`.
- Endpoints: Uma classe por feature implementando `IEndpoint`. Roteamento definido dentro de `MapEndpoint`. Sempre mapear `Result` -> HTTP via `result.Match(Results.Ok|NoContent, CustomResults.Problem)`.

## Convenções de Módulo e Nomenclatura (consistência)
- Organização por feature em todas as camadas (vertical):
	- Domain: `Domain/<Contexto>/<Aggregate>/...`
	- Application: `Application/<Contexto>/<Feature>/...`
	- Infrastructure: `Infrastructure/<Contexto|AreaTecnica>/<Feature|Config>/...`
	- Web.Api: `Web.Api/Endpoints/<Contexto>/<Feature>.cs`
- Verbos/palavras técnicas em inglês; substantivos de domínio em português: `GetAutorQuery`, `CreatePreVendaCommand`, `AutorRelatorioResponse`.
- Pastas/arquivos: PascalCase. Agregados em singular (`Autor`, `Pedido`).
- Códigos de erro prefixam contexto/área em inglês quando cross-cutting: ex. `Users.NotFound`, `Users.Unauthorized`; para domínio, prefira contexto: `Autores.NotFound`.
- DTOs de resposta terminam em `Response`.
- Identity/Users na infraestrutura padronizado em inglês: pasta/namespace `Infrastructure.Users`.
- Schemas de banco:
	- Padrão: `public` (Schemas.Default)
	- Identity: `users` (Schemas.Users)
- SharedKernel Value Objects: preferir ValueObjects para IDs cross-context, ex.: `UserId` com conversões implícitas para `Guid`.

## Fluxo Para Nova Feature
1. Domain: `Domain/<Contexto>/<Aggregate>/<Aggregate>.cs` + erros + eventos.
2. Application: `Application/<Contexto>/<Feature>/` (Command/Query + Handler + Validator + Response DTO se necessário).
3. Infrastructure: adicionar DbSet + configuration + migration (somente se persistido).
4. API: `Web.Api/Endpoints/<Contexto>/<Feature>.cs` implementando `IEndpoint`.
5. UI: componente Razor em `Web/Components/<Contexto>/<Feature>.razor` consumindo endpoint.

## Persistência
- Único `ApplicationDbContext` (monolito modular). Futuro split implica extrair parte do modelo + migrations.
- Naming: `UseSnakeCaseNamingConvention`; schema padrão `Schemas.Default`.
- Migrations (gerar no projeto Infrastructure com startup Web.Api):
	- Add: `dotnet ef migrations add <Name> --project src/CapituloZero.Infrastructure --startup-project src/CapituloZero.Web.Api -c ApplicationDbContext`
	- Update: `dotnet ef database update --project src/CapituloZero.Infrastructure --startup-project src/CapituloZero.Web.Api -c ApplicationDbContext`
- Migrações auto aplicadas em Development via `app.ApplyMigrations()`.

## Autenticação & Autorização
- JWT (config: `Jwt:Secret|Issuer|Audience`).
- `IUserContext` injeta identidade para validar ownership (exemplo em criação de Todo).
- `.HasPermission("Contexto.Ação")` preparado para granularidade futura (permissões personalizadas).

## Logging & Observability
- Serilog estruturado + decorators de handlers para tracing de sucesso/falha.
- Aspire + OpenTelemetry já referenciados; preferir adicionar instrumentação via extensões existentes.

## Testes
- Projetos de teste scaffold: priorizar testes de handlers (Application) com contexto em memória ou Testcontainers posteriormente.
- Futuro: Architecture tests para reforçar ausência de dependências proibidas (Domain -> Infrastructure/Web).

## Salvaguardas Arquiteturais
- Não antecipar microserviços: manter limites claros e dependências mínimas.
- Não mover publicação de domain events para antes do commit sem justificativa de consistência imediata.
- Evitar serviços god/mega-handlers: dividir por caso de uso.
- Para bibliotecas externas: preferir pacotes ativos / recomendados Microsoft (ex: Polly, System.Text.Json, etc.).

## Exemplo: Registrar Autor
Domain: `Autor` + `AutorRegistradoDomainEvent`.
Application: `RegisterAutorCommand`, handler valida e levanta evento.
Infrastructure: adiciona DbSet `Autores`; criar migration.
API: `Endpoints/Autor/Register.cs` -> POST `/autores` -> retorna `Result<Guid>`.
UI: Formulário Razor publica comando via HttpClient.

## Roteiro de Evolução Futura
- Introduzir eventos de integração para comunicação cross-context sem acoplamento.
- Adicionar permissionamento granular e feature flags por contexto.
- Externalizar contextos de alta escala (ex: Loja) primeiro se necessário.

---
Este documento guia a evolução incremental segura do monolito modular.
