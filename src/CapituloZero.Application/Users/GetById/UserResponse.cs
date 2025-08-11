namespace CapituloZero.Application.Users.GetById;

public sealed record UserResponse
{
    public Guid Id { get; init; }
    public required string Email { get; init; }

    public required string FirstName { get; init; }

    public required string LastName { get; init; }
}
