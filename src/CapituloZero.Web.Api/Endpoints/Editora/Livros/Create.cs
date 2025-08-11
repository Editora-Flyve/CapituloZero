using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Application.Editora.Livros;
using CapituloZero.SharedKernel;
using CapituloZero.Web.Api.Endpoints;
using CapituloZero.Web.Api.Extensions;
using CapituloZero.Web.Api.Infrastructure;

namespace CapituloZero.Web.Api.Endpoints.Editora.Livros;

internal sealed class Create : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("editora/livros", async (
            CreateLivroRequest request,
            ICommandHandler<CreateLivroCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateLivroCommand(
                request.Titulo,
                request.Subtitulo,
                request.AutorNome,
                request.AutorEmail,
                request.FluxoProducaoId);

            Result<Guid> result = await handler.Handle(command, cancellationToken).ConfigureAwait(false);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
    .WithTags(Tags.Editora)
    .HasPermission(CapituloZero.Web.Api.Endpoints.Users.Permissions.AdminAccess);
    }
}
