using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.SharedKernel;

namespace CapituloZero.Application.Users.RefreshToken;

internal sealed class RefreshTokenCommandHandler(
    IIdentityService identityService)
    : ICommandHandler<RefreshTokenCommand, Login.LoginResponse>
{
    public async Task<Result<Login.LoginResponse>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var result = await identityService.RefreshAsync(command.RefreshToken, cancellationToken).ConfigureAwait(false);
        return result;
    }
}
