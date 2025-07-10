using CapituloZero.Application.Abstractions.Messaging;

namespace CapituloZero.Application.Users.GetById;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserResponse>;
