using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Users.Entities;

public sealed class User : Entity
{
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string PasswordHash { get; set; }
    // Bitwise flags representing the types this user belongs to
    public Users.UserType Types { get; set; }
    // The currently selected/active type for this session (optional)
    public Users.UserType? ActiveType { get; set; }
}
