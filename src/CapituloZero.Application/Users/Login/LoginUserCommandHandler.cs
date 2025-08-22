using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.SharedKernel;

namespace CapituloZero.Application.Users.Login;

internal sealed class LoginUserCommandHandler(
    IIdentityService identityService) : ICommandHandler<LoginUserCommand, string>
{
    public async Task<Result<string>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        return await identityService.LoginAsync(command.Email, command.Password, cancellationToken).ConfigureAwait(false);
    }
}
