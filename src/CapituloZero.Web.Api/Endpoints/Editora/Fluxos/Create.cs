using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Application.Editora.Fluxos;
using CapituloZero.SharedKernel;
using CapituloZero.Web.Api.Endpoints;
using CapituloZero.Web.Api.Extensions;
using CapituloZero.Web.Api.Infrastructure;

namespace CapituloZero.Web.Api.Endpoints.Editora.Fluxos;

internal sealed class Create : IEndpoint
{
    public sealed class Request
    {
        public required string Nome { get; set; }
        public string? Descricao { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("editora/fluxos", async (
            Request request,
            ICommandHandler<CreateFluxoProducaoCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateFluxoProducaoCommand(request.Nome, request.Descricao);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
    .WithTags(Tags.Editora)
    .HasPermission(CapituloZero.Web.Api.Endpoints.Users.Permissions.AdminAccess);
    }
}
