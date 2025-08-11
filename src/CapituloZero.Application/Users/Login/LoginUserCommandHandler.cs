using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.Application.Abstractions.Data;
using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Domain.Users;
using Microsoft.EntityFrameworkCore;
using CapituloZero.SharedKernel;
using CapituloZero.Domain.Users.Entities;

namespace CapituloZero.Application.Users.Login;

internal sealed class LoginUserCommandHandler(
    IApplicationDbContext context,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider) : ICommandHandler<LoginUserCommand, LoginResponse>
{
    public async Task<Result<LoginResponse>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        User? user = await context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Email == command.Email, cancellationToken).ConfigureAwait(false);

        if (user is null)
        {
            return Result.Failure<LoginResponse>(UserErrors.NotFoundByEmail);
        }

        bool verified = passwordHasher.Verify(command.Password, user.PasswordHash);

        if (!verified)
        {
            return Result.Failure<LoginResponse>(UserErrors.NotFoundByEmail);
        }

        // gather available types
        var available = GetTypes(user.Types);
        if (available.Count == 0)
        {
            // All users are at least Cliente by default if not set
            available = new List<UserType> { UserType.Cliente };
        }

        UserType activeType;

        if (command.DesiredType.HasValue)
        {
            if (!available.Contains(command.DesiredType.Value))
            {
                return Result.Failure<LoginResponse>(Error.Failure(
                    "Users.InvalidType",
                    "Selected type is not available for this user"));
            }
            activeType = command.DesiredType.Value;
        }
        else if (available.Count == 1)
        {
            activeType = available[0];
        }
        else
        {
            // require selection
            return new LoginResponse(true, null, available, null);
        }

        // set active type just for token creation (no persistence here)
        var tmpUser = new User
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PasswordHash = user.PasswordHash,
            Types = user.Types,
            ActiveType = activeType
        };
        string token = tokenProvider.Create(tmpUser);

        return new LoginResponse(false, token, available, activeType);
    }

    private static List<UserType> GetTypes(UserType flags)
    {
        var list = new List<UserType>();
    foreach (UserType t in Enum.GetValues<UserType>())
        {
            if (t == UserType.None) continue;
            if (flags.HasFlag(t)) list.Add(t);
        }
        return list;
    }
}
