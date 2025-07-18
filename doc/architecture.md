# CapituloZero - Arquitetura e Guia de Extensão

## Visão Geral

O CapituloZero é uma solução modular baseada em .NET 9, composta por múltiplos projetos organizados em camadas, com foco em separação de responsabilidades, extensibilidade e boas práticas de desenvolvimento. O front-end utiliza Blazor Server, enquanto a API segue o padrão minimal API com endpoints modulares.

---

## Estrutura de Projetos

- **CapituloZero.Web**: Aplicação Blazor Server (UI).
- **CapituloZero.Web.Api**: API RESTful, endpoints modulares, autenticação/autorização, health checks, Swagger.
- **CapituloZero.Application**: Camada de aplicação, orquestra lógica de negócio, comandos, queries, handlers, validação e logging.
- **CapituloZero.Domain**: Entidades de domínio, eventos de domínio, regras de negócio.
- **CapituloZero.Infrastructure**: Persistência (EF Core), autenticação, autorização, serviços de infraestrutura.
- **CapituloZero.ServiceDefaults**: Configurações compartilhadas de telemetria, resiliência, service discovery.
- **CapituloZero.SharedKernel**: Utilitários e contratos compartilhados.

---

## Padrões Arquiteturais

- **CQRS (Command Query Responsibility Segregation)**: Separação clara entre comandos (escrita) e queries (leitura), com handlers específicos para cada operação.
- **Domain Events**: Eventos de domínio são disparados pelas entidades e processados após persistência.
- **Decorators**: Handlers de comandos e queries são decorados para adicionar validação (FluentValidation) e logging (Serilog).
- **Injeção de Dependências**: Serviços, handlers, validadores e contextos são registrados via métodos de extensão (`AddApplication`, `AddInfrastructure`, `AddPresentation`).

---

## Como Criar Novos Módulos e Funcionalidades

### 1. Domínio

- Crie entidades no projeto `CapituloZero.Domain`.
- Defina eventos de domínio, enums e regras de negócio.

### 2. Application (Comandos, Queries, Handlers)

- Crie comandos (`ICommand`/`ICommand<TResponse>`) e queries (`IQuery<TResponse>`) em subpastas temáticas.
- Implemente handlers para comandos (`ICommandHandler<>`) e queries (`IQueryHandler<>`).
- Adicione validadores usando FluentValidation.
- Handlers são registrados automaticamente via assembly scanning.

### 3. Infraestrutura

- Configure persistência no `ApplicationDbContext` (DbSets, configurações).
- Implemente serviços de infraestrutura (ex: autenticação, autorização, providers).
- Registre dependências em `DependencyInjection.cs`.

### 4. API (Endpoints)

- Crie endpoints implementando a interface `IEndpoint` no projeto `CapituloZero.Web.Api`.
- Utilize injeção de dependências para acessar handlers de comandos/queries.
- Mapeie endpoints em `MapEndpoint(IEndpointRouteBuilder app)`.

### 5. Blazor (UI)

- Crie componentes Razor em `CapituloZero.Web\Components`.
- Consuma APIs via HttpClient (ex: `WeatherApiClient`).
- Utilize diretivas como `@inject` para acessar serviços.

---

## Exemplo de Fluxo para Nova Funcionalidade

1. **Domínio**: Adicione entidade e evento.
2. **Application**: Crie comando/query, handler e validador.
3. **Infraestrutura**: Atualize o DbContext e configurações.
4. **API**: Implemente endpoint e mapeamento.
5. **Blazor**: Crie componente para consumir e exibir dados.

---

## Convenções

- **Handlers**: Nomeados como `XxxCommandHandler` ou `XxxQueryHandler`.
- **Endpoints**: Nomeados pelo recurso e ação (ex: `Todos/Create.cs`).
- **Validação**: Sempre via FluentValidation.
- **Logging**: Automático via decorators.
- **Domain Events**: Disparados nas entidades, processados após `SaveChangesAsync`.

---

## Extensibilidade

- Novos módulos seguem a estrutura de pastas e padrões existentes.
- Basta criar as classes e interfaces necessárias; o assembly scanning e DI cuidam do registro.
- Para novos serviços de infraestrutura, adicione métodos de extensão em `DependencyInjection.cs`.

---

## Observações

- O sistema é altamente modular e desacoplado.
- O uso de decorators permite adicionar cross-cutting concerns sem poluir handlers.
- A arquitetura facilita testes, manutenção e evolução.

---

Este guia serve como referência para criação e extensão de módulos e funcionalidades no CapituloZero.
