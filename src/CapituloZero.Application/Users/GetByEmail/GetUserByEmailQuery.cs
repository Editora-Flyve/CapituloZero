using CapituloZero.Application.Abstractions.Messaging;

namespace CapituloZero.Application.Users.GetByEmail;

public sealed record GetUserByEmailQuery(string Email) : IQuery<UserResponse>;
