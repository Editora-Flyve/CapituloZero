using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Application.Users.Login;
using CapituloZero.SharedKernel;
using CapituloZero.Web.Api.Extensions;
using CapituloZero.Web.Api.Infrastructure;
using CapituloZero.Domain.Users;

namespace CapituloZero.Web.Api.Endpoints.Users;

internal sealed class Login : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/login", async (
            LoginUserRequest request,
            ICommandHandler<LoginUserCommand, LoginResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new LoginUserCommand(request.Email, request.Password, request.DesiredType);

            Result<LoginResponse> result = await handler.Handle(command, cancellationToken).ConfigureAwait(false);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Users);
    }
}
