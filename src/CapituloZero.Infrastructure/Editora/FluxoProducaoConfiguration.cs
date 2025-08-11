using CapituloZero.Domain.Editora.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CapituloZero.Infrastructure.Editora;

internal sealed class FluxoProducaoConfiguration : IEntityTypeConfiguration<FluxoProducao>
{
    public void Configure(EntityTypeBuilder<FluxoProducao> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Nome).IsRequired();
        builder.Property(f => f.Descricao).HasDefaultValue(string.Empty);
        builder.HasMany(f => f.Etapas)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(f => f.Etapas).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
