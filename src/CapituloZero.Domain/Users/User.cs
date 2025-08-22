namespace CapituloZero.Domain.Users;

// Lightweight User DTO retained for JWT token creation compatibility.
public sealed class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string PasswordHash { get; set; } = string.Empty;
}
