# CapituloZero

Modular monolith (ASP.NET Core / .NET 9 + Aspire) para uma plataforma editorial abrangente composta por:
- Loja pública (catálogo, pré‑vendas).
- Portal do Autor (submissões, relatórios, royalties).
- Portal de Parceiros (kanban de tarefas, pagamentos, contratos).
- Painel Administrativo (gestão de pedidos, pré‑vendas, estoque, finanças, etiquetas).

Arquitetura baseada em camadas horizontais (SharedKernel, Domain, Application, Infrastructure, Web.Api, Web, AppHost) + contextos verticais (Loja, Autor, Parceiros, Admin) isolados logicamente dentro do mesmo processo.

## Principais Padrões
- CQRS (Commands / Queries + Handlers internos selados)
- Result Pattern (falhas sem exceção de regra de negócio)
- Domain Events (publicados após commit => eventual consistency)
- Decorators (Validation + Logging) via Scrutor
- Minimal API modular (`IEndpoint`) + Blazor Server UI
- Centralização de versions de pacotes (`Directory.Packages.props`)

## Estrutura (resumida)
```
src/
	CapituloZero.SharedKernel   # Primitivos
	CapituloZero.Domain         # Domínio por contexto
	CapituloZero.Application    # Casos de uso (CQRS)
	CapituloZero.Infrastructure # EF Core, Auth, Permissões, Eventos
	CapituloZero.Web.Api        # Endpoints Minimal API
	CapituloZero.Web            # Blazor UI
	CapituloZero.AppHost        # Aspire host (observabilidade, discovery)
doc/architecture.md           # Detalhes de arquitetura
.github/copilot-instructions.md # Guia para agentes de IA
```

## Executar (Dev)
Requisitos: .NET 9 SDK, PostgreSQL (local ou container) com connection string configurada em `appsettings.Development.json`.

```
dotnet build
dotnet run --project src/CapituloZero.Web.Api      # API (aplica migrations em Development)
dotnet run --project src/CapituloZero.Web          # UI Blazor
dotnet run --project src/CapituloZero.AppHost      # (Opcional) Orquestra Aspire
```

## Migrations EF Core
```
dotnet ef migrations add <Name> --project src/CapituloZero.Infrastructure --startup-project src/CapituloZero.Web.Api -c ApplicationDbContext
dotnet ef database update --project src/CapituloZero.Infrastructure --startup-project src/CapituloZero.Web.Api -c ApplicationDbContext
```

## Testes e Cobertura
- Projetos de teste:
  - `tests/CapituloZero.UnitTests` (SharedKernel/Domain)
  - `tests/CapituloZero.ApplicationTests` (Handlers/Validators)
  - `tests/CapituloZero.IdentityTests` (fluxos de identidade)
- Executar todos os testes com coleta de cobertura (usa `coverlet.collector` já referenciado):

```bash
# Restaurar, compilar e testar com cobertura
dotnet restore CapituloZero.sln
dotnet build CapituloZero.sln -c Debug
dotnet test CapituloZero.sln -c Debug -v minimal --collect:"XPlat Code Coverage" --logger:"trx;LogFileName=test_results.trx"
```

- Onde ver a cobertura:
  - Um arquivo `coverage.cobertura.xml` é gerado em `tests/*/TestResults/<guid>/` por projeto; visualize com extensões do VS Code ou converta para HTML via ferramentas como `reportgenerator`.
- Metas:
  - Mínimo: 95% linhas cobertas
  - Meta: 100% das unidades core (SharedKernel, Application handlers/validators)

## Permissões e Proteção de Endpoints
- Tipos de usuário (roles): `Default`, `Autor`, `Parceiro`, `Admin` (semeados em banco; novos usuários recebem `Default`).
- Mapeamento tipos → permissões (infra pronta):
  - Default → `users:access` (base para qualquer usuário autenticado)
  - Autor → `autor:access`
  - Parceiro → `parceiro:access`
  - Admin → `users:admin` (tem acesso a tudo)
- Como usar nos endpoints (Minimal API):
  - Todos autenticados: `.HasPermission("users:access")`
  - Autor (e Admin): `.HasPermission("autor:access")`
  - Parceiro (e Admin): `.HasPermission("parceiro:access")`
  - Apenas Admin: `.HasPermission("users:admin")`

Rotas de demonstração já mapeadas (para validar):
- GET `/demo/default` → todos autenticados
- GET `/demo/autor` → Autor e Admin
- GET `/demo/parceiro` → Parceiro e Admin
- GET `/demo/admin` → apenas Admin

## Adicionando Uma Feature (Exemplo)
1. Domain: `Domain/Autores/Autor.cs` + eventos + erros.
2. Application: `Application/Autores/RegisterAutor/RegisterAutorCommand.cs` (+ Handler + Validator + Response).
3. Infrastructure: DbSet se persistido + migration.
4. API: Endpoint em `Web.Api/Endpoints/Autores/Register.cs`.
5. UI: Componente Razor consumindo endpoint.

## Convenções de Nomenclatura
- Verbos/padrões técnicos em inglês; substantivos de domínio em português (`GetAutorQuery`).
- Erros prefixados por contexto (`Autores.NotFound`).
- DTOs de saída terminam em `Response`.

## Contribuindo
- Respeite limites de contexto (não referencie diretamente entidades de outro contexto no domínio).
- Use `Result` para retornar falhas; escolha `ErrorType` correto.
- Não alterar ordem dos decorators (validação antes de logging).
- Mantenha handlers `internal sealed` (necessário para scanning).

## Próximos Passos Planejados
- Introdução de permissões granulares (`.HasPermission("Contexto.Ação")`).
- Eventos de integração entre contextos para reduzir acoplamento.
- Testes de arquitetura para reforçar dependências permitidas.

Consulte `doc/architecture.md` para detalhes aprofundados e `.github/copilot-instructions.md` para diretrizes de automação/IA.