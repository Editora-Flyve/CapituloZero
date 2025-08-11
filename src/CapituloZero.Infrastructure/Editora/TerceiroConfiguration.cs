using CapituloZero.Domain.Editora.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CapituloZero.Infrastructure.Editora;

internal sealed class TerceiroConfiguration : IEntityTypeConfiguration<Terceiro>
{
    public void Configure(EntityTypeBuilder<Terceiro> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Nome).IsRequired();
        builder.Property(t => t.Documento).IsRequired();
        builder.Property(t => t.Email).IsRequired();
        builder.Property(t => t.UserId);
        builder.HasOne(t => t.Funcao).WithMany(f => f.Terceiros).OnDelete(DeleteBehavior.Restrict);
    }
}
