using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Application.Editora.Livros;
using CapituloZero.SharedKernel;
using CapituloZero.Web.Api.Endpoints;
using CapituloZero.Web.Api.Extensions;
using CapituloZero.Web.Api.Infrastructure;

namespace CapituloZero.Web.Api.Endpoints.Editora.Livros;

internal sealed class Create : IEndpoint
{
    public sealed class Request
    {
        public required string Titulo { get; set; }
        public string? Subtitulo { get; set; }
        public required string AutorNome { get; set; }
        public required string AutorEmail { get; set; }
        public required Guid FluxoProducaoId { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("editora/livros", async (
            Request request,
            ICommandHandler<CreateLivroCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateLivroCommand(
                request.Titulo,
                request.Subtitulo,
                request.AutorNome,
                request.AutorEmail,
                request.FluxoProducaoId);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
    .WithTags(Tags.Editora)
    .HasPermission(CapituloZero.Web.Api.Endpoints.Users.Permissions.AdminAccess);
    }
}
