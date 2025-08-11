using CapituloZero.Application.Abstractions.Data;
using CapituloZero.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace CapituloZero.Web.Api.Endpoints.Users;

internal sealed class ListTypes : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/types", async (
            ListUserTypesRequest request,
            IApplicationDbContext context,
            CancellationToken cancellationToken) =>
        {
            var user = await context.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken).ConfigureAwait(false);
            if (user is null) return Results.NotFound();

            var types = new List<UserType>();
            foreach (var t in Enum.GetValues<UserType>())
            {
                if (t == UserType.None) continue;
                if (user.Types.HasFlag(t)) types.Add(t);
            }
            if (types.Count == 0) types.Add(UserType.Cliente);

            return Results.Ok(types);
        })
        .WithTags(Tags.Users);
    }
}
