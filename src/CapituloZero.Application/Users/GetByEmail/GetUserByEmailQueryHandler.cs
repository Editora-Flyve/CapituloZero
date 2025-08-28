using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.SharedKernel;

namespace CapituloZero.Application.Users.GetByEmail;

internal sealed class GetUserByEmailQueryHandler(IIdentityService identityService, IUserContext userContext)
    : IQueryHandler<GetUserByEmailQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken)
    {
    return await identityService.GetByEmailAsync(query.Email, userContext.UserId, cancellationToken).ConfigureAwait(false);
    }
}
