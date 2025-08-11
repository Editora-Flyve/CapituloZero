using CapituloZero.Application.Abstractions.Messaging;
using CapituloZero.Domain.Users;

namespace CapituloZero.Application.Users.Login;

public sealed record LoginUserCommand(string Email, string Password, UserType? DesiredType) : ICommand<LoginResponse>;
