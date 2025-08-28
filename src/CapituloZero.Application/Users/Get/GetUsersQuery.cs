using CapituloZero.Application.Abstractions.Messaging;

namespace CapituloZero.Application.Users.Get;

public sealed record GetUsersQuery : IQuery<IReadOnlyList<UserListItemResponse>>;

