using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Application.Users.Register;
using CapituloZero.Domain.Users;
using CapituloZero.SharedKernel;
using CapituloZero.Web.Api.Extensions;
using CapituloZero.Web.Api.Infrastructure;

namespace CapituloZero.Web.Api.Endpoints.Users;

internal sealed class Register : IEndpoint
{
    public sealed record Request(string Email, string FirstName, string LastName, string Password, UserType? Types);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/register", async (
            Request request,
            ICommandHandler<RegisterUserCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RegisterUserCommand(
                request.Email,
                request.FirstName,
                request.LastName,
                request.Password,
                request.Types);

            Result<Guid> result = await handler.Handle(command, cancellationToken).ConfigureAwait(false);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Users);
    }
}
