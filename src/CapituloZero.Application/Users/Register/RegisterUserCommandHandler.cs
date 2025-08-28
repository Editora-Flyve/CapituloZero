using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.SharedKernel;

namespace CapituloZero.Application.Users.Register;

internal sealed class RegisterUserCommandHandler(IIdentityService identityService)
    : ICommandHandler<RegisterUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
    return await identityService.RegisterAsync(command.Email, command.FirstName, command.LastName, command.Password, cancellationToken).ConfigureAwait(false);
    }
}
