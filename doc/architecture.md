# CapituloZero - Arquitetura e Guia de Extens�o

## Vis�o Geral

O CapituloZero � uma solu��o modular baseada em .NET 9, composta por m�ltiplos projetos organizados em camadas, com foco em separa��o de responsabilidades, extensibilidade e boas pr�ticas de desenvolvimento. O front-end utiliza Blazor Server, enquanto a API segue o padr�o minimal API com endpoints modulares.

---

## Estrutura de Projetos

- **CapituloZero.Web**: Aplica��o Blazor Server (UI).
- **CapituloZero.Web.Api**: API RESTful, endpoints modulares, autentica��o/autoriza��o, health checks, Swagger.
- **CapituloZero.Application**: Camada de aplica��o, orquestra l�gica de neg�cio, comandos, queries, handlers, valida��o e logging.
- **CapituloZero.Domain**: Entidades de dom�nio, eventos de dom�nio, regras de neg�cio.
- **CapituloZero.Infrastructure**: Persist�ncia (EF Core), autentica��o, autoriza��o, servi�os de infraestrutura.
- **CapituloZero.ServiceDefaults**: Configura��es compartilhadas de telemetria, resili�ncia, service discovery.
- **CapituloZero.SharedKernel**: Utilit�rios e contratos compartilhados.

---

## Padr�es Arquiteturais

- **CQRS (Command Query Responsibility Segregation)**: Separa��o clara entre comandos (escrita) e queries (leitura), com handlers espec�ficos para cada opera��o.
- **Domain Events**: Eventos de dom�nio s�o disparados pelas entidades e processados ap�s persist�ncia.
- **Decorators**: Handlers de comandos e queries s�o decorados para adicionar valida��o (FluentValidation) e logging (Serilog).
- **Inje��o de Depend�ncias**: Servi�os, handlers, validadores e contextos s�o registrados via m�todos de extens�o (`AddApplication`, `AddInfrastructure`, `AddPresentation`).

---

## Como Criar Novos M�dulos e Funcionalidades

### 1. Dom�nio

- Crie entidades no projeto `CapituloZero.Domain`.
- Defina eventos de dom�nio, enums e regras de neg�cio.

### 2. Application (Comandos, Queries, Handlers)

- Crie comandos (`ICommand`/`ICommand<TResponse>`) e queries (`IQuery<TResponse>`) em subpastas tem�ticas.
- Implemente handlers para comandos (`ICommandHandler<>`) e queries (`IQueryHandler<>`).
- Adicione validadores usando FluentValidation.
- Handlers s�o registrados automaticamente via assembly scanning.

### 3. Infraestrutura

- Configure persist�ncia no `ApplicationDbContext` (DbSets, configura��es).
- Implemente servi�os de infraestrutura (ex: autentica��o, autoriza��o, providers).
- Registre depend�ncias em `DependencyInjection.cs`.

### 4. API (Endpoints)

- Crie endpoints implementando a interface `IEndpoint` no projeto `CapituloZero.Web.Api`.
- Utilize inje��o de depend�ncias para acessar handlers de comandos/queries.
- Mapeie endpoints em `MapEndpoint(IEndpointRouteBuilder app)`.

### 5. Blazor (UI)

- Crie componentes Razor em `CapituloZero.Web\Components`.
- Consuma APIs via HttpClient (ex: `WeatherApiClient`).
- Utilize diretivas como `@inject` para acessar servi�os.

---

## Exemplo de Fluxo para Nova Funcionalidade

1. **Dom�nio**: Adicione entidade e evento.
2. **Application**: Crie comando/query, handler e validador.
3. **Infraestrutura**: Atualize o DbContext e configura��es.
4. **API**: Implemente endpoint e mapeamento.
5. **Blazor**: Crie componente para consumir e exibir dados.

---

## Conven��es

- **Handlers**: Nomeados como `XxxCommandHandler` ou `XxxQueryHandler`.
- **Endpoints**: Nomeados pelo recurso e a��o (ex: `Todos/Create.cs`).
- **Valida��o**: Sempre via FluentValidation.
- **Logging**: Autom�tico via decorators.
- **Domain Events**: Disparados nas entidades, processados ap�s `SaveChangesAsync`.

---

## Extensibilidade

- Novos m�dulos seguem a estrutura de pastas e padr�es existentes.
- Basta criar as classes e interfaces necess�rias; o assembly scanning e DI cuidam do registro.
- Para novos servi�os de infraestrutura, adicione m�todos de extens�o em `DependencyInjection.cs`.

---

## Observa��es

- O sistema � altamente modular e desacoplado.
- O uso de decorators permite adicionar cross-cutting concerns sem poluir handlers.
- A arquitetura facilita testes, manuten��o e evolu��o.

---

Este guia serve como refer�ncia para cria��o e extens�o de m�dulos e funcionalidades no CapituloZero.
