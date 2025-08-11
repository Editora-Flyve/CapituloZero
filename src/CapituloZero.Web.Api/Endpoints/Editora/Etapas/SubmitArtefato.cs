using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Application.Editora.Etapas;
using CapituloZero.SharedKernel;
using CapituloZero.Web.Api.Endpoints;
using CapituloZero.Web.Api.Extensions;
using CapituloZero.Web.Api.Infrastructure;

namespace CapituloZero.Web.Api.Endpoints.Editora.Etapas;

internal sealed class SubmitArtefato : IEndpoint
{
    public sealed class Request
    {
        public required Guid EtapaId { get; set; }
        public required string FileUri { get; set; }
        public required string FileName { get; set; }
        public required string ContentType { get; set; }
        public long SizeBytes { get; set; }
        public required Guid UploadedByUserId { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("editora/etapas/artefatos", async (
            Request request,
            ICommandHandler<SubmitArtefatoCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new SubmitArtefatoCommand(
                request.EtapaId,
                request.FileUri,
                request.FileName,
                request.ContentType,
                request.SizeBytes,
                request.UploadedByUserId);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(() => Results.Ok(), CustomResults.Problem);
        })
    .WithTags(Tags.Editora)
    .HasPermission(CapituloZero.Web.Api.Endpoints.Users.Permissions.TerceirosAccess);
    }
}
