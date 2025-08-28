using CapituloZero.Application.Abstractions.Messaging;

namespace CapituloZero.Application.Users.Login;

public sealed record LoginUserCommand(string Email, string Password) : ICommand<LoginResponse>;
