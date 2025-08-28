using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.SharedKernel;

namespace CapituloZero.Application.Users.GetById;

internal sealed class GetUserByIdQueryHandler(IIdentityService identityService, IUserContext userContext)
    : IQueryHandler<GetUserByIdQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
    return await identityService.GetByIdAsync(query.UserId, userContext.UserId, cancellationToken).ConfigureAwait(false);
    }
}
