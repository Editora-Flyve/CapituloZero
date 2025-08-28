using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Application.Users.Get;
using CapituloZero.SharedKernel;
using CapituloZero.Web.Api.Extensions;
using CapituloZero.Web.Api.Infrastructure;

namespace CapituloZero.Web.Api.Endpoints.Users;

internal sealed class GetUsers: IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users", async (
                IQueryHandler<GetUsersQuery, IReadOnlyList<UserListItemResponse>> handler,
                CancellationToken cancellationToken) =>
            {
                var query = new GetUsersQuery();

                Result<IReadOnlyList<UserListItemResponse>> result = await handler.Handle(query, cancellationToken).ConfigureAwait(false);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .HasPermission(Permissions.UsersAdmin)
            .WithTags(Tags.Users);
    }
}