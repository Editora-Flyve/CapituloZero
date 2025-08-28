using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.SharedKernel;

namespace CapituloZero.Application.Users.Get;

internal sealed class GetUsersQueryHandler(IIdentityService identityService)
    : IQueryHandler<GetUsersQuery, IReadOnlyList<UserListItemResponse>>
{
    public async Task<Result<IReadOnlyList<UserListItemResponse>>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
    {
        return await identityService.GetAllAsync(cancellationToken).ConfigureAwait(false);
    }
}

