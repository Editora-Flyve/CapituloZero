using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.Application.Abstractions.Data;
using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Domain.Users;
using Microsoft.EntityFrameworkCore;
using CapituloZero.SharedKernel;

namespace CapituloZero.Application.Users.Login;

internal sealed class LoginUserCommandHandler(
    IApplicationDbContext context,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider) : ICommandHandler<LoginUserCommand, string>
{
    public async Task<Result<string>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        User? user = await context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Email == command.Email, cancellationToken).ConfigureAwait(false);

        if (user is null)
        {
            return Result.Failure<string>(UserErrors.NotFoundByEmail);
        }

        bool verified = passwordHasher.Verify(command.Password, user.PasswordHash);

        if (!verified)
        {
            return Result.Failure<string>(UserErrors.NotFoundByEmail);
        }

        string token = tokenProvider.Create(user);

        return token;
    }
}
