using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Application.Users.RefreshToken;
using CapituloZero.Web.Api.Extensions;
using CapituloZero.Web.Api.Infrastructure;

namespace CapituloZero.Web.Api.Endpoints.Users;

internal sealed class RefreshToken : IEndpoint
{
    internal sealed record Request(string RefreshToken);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/refresh-token", async (
            Request request,
            ICommandHandler<RefreshTokenCommand, CapituloZero.Application.Users.Login.LoginResponse> handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new RefreshTokenCommand(request.RefreshToken), ct).ConfigureAwait(false);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Users)
        .WithName("RefreshToken")
        .Produces<CapituloZero.Application.Users.Login.LoginResponse>()
        .ProducesValidationProblem();
    }
}
