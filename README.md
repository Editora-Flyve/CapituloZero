# CapituloZero

Modular monolith (ASP.NET Core / .NET 9 + Aspire) para uma plataforma editorial abrangente composta por:
- Loja pública (catálogo, pré‑vendas).
- Portal do Autor (submissões, relatórios, royalties).
- Portal de Terceiros (kanban de tarefas, pagamentos, contratos).
- Painel Administrativo (gestão de pedidos, pré‑vendas, estoque, finanças, etiquetas).

Arquitetura baseada em camadas horizontais (SharedKernel, Domain, Application, Infrastructure, Web.Api, Web, AppHost) + contextos verticais (Loja, Autor, Terceiros, Admin) isolados logicamente dentro do mesmo processo.

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