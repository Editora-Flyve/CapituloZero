using Microsoft.AspNetCore.Identity;

namespace CapituloZero.Infrastructure.Users;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}
