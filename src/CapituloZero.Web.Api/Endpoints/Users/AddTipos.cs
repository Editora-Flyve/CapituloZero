using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Application.Users.AddTipos;
using CapituloZero.Web.Api.Extensions;
using CapituloZero.Web.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace CapituloZero.Web.Api.Endpoints.Users;

internal sealed class AddTipos : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/{userId:guid}/tipos", async (
            [FromRoute] Guid userId,
            [FromBody] Request request,
            IUserContext current,
            ICommandHandler<AddTiposCommand> handler) =>
        {
            var command = new AddTiposCommand(userId, request.Tipos ?? Array.Empty<string>(), current.UserId);
            var result = await handler.Handle(command, default).ConfigureAwait(false);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.UsersAdmin)
        .WithTags(Tags.Users);
    }

    internal sealed class Request
    {
        public IEnumerable<string>? Tipos { get; set; }
    }
}
