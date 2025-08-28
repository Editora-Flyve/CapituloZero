namespace CapituloZero.Application.Users.Get;

public sealed record UserListItemResponse
{
    public Guid Id { get; init; }
    public required string Email { get; init; }
    // Tipos (roles)
    public IReadOnlyList<string> Tipos { get; init; } = new List<string>();
}

