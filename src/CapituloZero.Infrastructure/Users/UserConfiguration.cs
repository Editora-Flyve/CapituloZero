using CapituloZero.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CapituloZero.Domain.Users;

namespace CapituloZero.Infrastructure.Users;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.Types)
            .HasConversion<int>()
            .HasDefaultValue(UserType.Cliente);

        builder.Property(u => u.ActiveType)
            .HasConversion<int?>();
    }
}
