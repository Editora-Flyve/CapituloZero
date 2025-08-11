using CapituloZero.Domain.Editora.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CapituloZero.Infrastructure.Editora;

internal sealed class AutorConfiguration : IEntityTypeConfiguration<Autor>
{
    public void Configure(EntityTypeBuilder<Autor> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Nome).IsRequired();
        builder.Property(a => a.Email).IsRequired();
        builder.HasMany(a => a.Livros).WithOne(l => l.Autor).OnDelete(DeleteBehavior.Restrict);
    }
}
