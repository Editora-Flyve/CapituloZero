using CapituloZero.Domain.Editora.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CapituloZero.Infrastructure.Editora;

internal sealed class FuncaoConfiguration : IEntityTypeConfiguration<Funcao>
{
    public void Configure(EntityTypeBuilder<Funcao> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Descricao).IsRequired();
        builder.HasMany(f => f.Terceiros).WithOne(t => t.Funcao).OnDelete(DeleteBehavior.Restrict);
    }
}
