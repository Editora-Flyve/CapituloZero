using CapituloZero.Application.Abstractions.Messaging;

namespace CapituloZero.Application.Users.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<Login.LoginResponse>;

