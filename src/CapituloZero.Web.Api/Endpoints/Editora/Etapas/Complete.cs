using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Application.Editora.Etapas;
using CapituloZero.SharedKernel;
using CapituloZero.Web.Api.Endpoints;
using CapituloZero.Web.Api.Extensions;
using CapituloZero.Web.Api.Infrastructure;

namespace CapituloZero.Web.Api.Endpoints.Editora.Etapas;

internal sealed class Complete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("editora/etapas/complete", async (
            CompleteEtapaRequest request,
            ICommandHandler<CompleteEtapaCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CompleteEtapaCommand(request.EtapaId);

            Result result = await handler.Handle(command, cancellationToken).ConfigureAwait(false);

            return result.Match(() => Results.Ok(), CustomResults.Problem);
        })
    .WithTags(Tags.Editora)
    .HasPermission(CapituloZero.Web.Api.Endpoints.Users.Permissions.TerceirosAccess);
    }
}
