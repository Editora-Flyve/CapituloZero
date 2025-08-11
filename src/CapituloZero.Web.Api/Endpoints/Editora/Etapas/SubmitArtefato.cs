using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Application.Editora.Etapas;
using CapituloZero.SharedKernel;
using CapituloZero.Web.Api.Endpoints;
using CapituloZero.Web.Api.Extensions;
using CapituloZero.Web.Api.Infrastructure;

namespace CapituloZero.Web.Api.Endpoints.Editora.Etapas;

internal sealed class SubmitArtefato : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("editora/etapas/artefatos", async (
            SubmitArtefatoRequest request,
            ICommandHandler<SubmitArtefatoCommand> handler,
            CancellationToken cancellationToken) =>
        {
                var command = new SubmitArtefatoCommand(
                    request.EtapaId,
                    new Uri(request.FileUri),
                    request.FileName,
                    request.ContentType,
                    request.SizeBytes,
                    request.UploadedByUserId);

            Result result = await handler.Handle(command, cancellationToken).ConfigureAwait(false);

            return result.Match(() => Results.Ok(), CustomResults.Problem);
        })
    .WithTags(Tags.Editora)
    .HasPermission(CapituloZero.Web.Api.Endpoints.Users.Permissions.TerceirosAccess);
    }
}
