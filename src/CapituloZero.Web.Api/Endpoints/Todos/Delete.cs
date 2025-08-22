using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Application.Todos.Delete;
using CapituloZero.SharedKernel;
using CapituloZero.Web.Api.Extensions;
using CapituloZero.Web.Api.Infrastructure;

namespace CapituloZero.Web.Api.Endpoints.Todos;

internal sealed class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("todos/{id:guid}", async (
            Guid id,
            ICommandHandler<DeleteTodoCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteTodoCommand(id);

            Result result = await handler.Handle(command, cancellationToken).ConfigureAwait(false);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Todos)
        .RequireAuthorization();
    }
}
