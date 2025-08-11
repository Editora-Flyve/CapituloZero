using CapituloZero.Domain.Users;

namespace CapituloZero.Application.Users.Login;

public sealed record LoginResponse(
    bool RequiresSelection,
    string? Token,
    IReadOnlyList<UserType> AvailableTypes,
    UserType? ActiveType);
