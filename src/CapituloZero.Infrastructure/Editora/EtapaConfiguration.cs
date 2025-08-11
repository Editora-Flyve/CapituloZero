using CapituloZero.Domain.Editora.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CapituloZero.Infrastructure.Editora;

internal sealed class EtapaConfiguration : IEntityTypeConfiguration<Etapa>
{
    public void Configure(EntityTypeBuilder<Etapa> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Nome).IsRequired();
        builder.Property(e => e.Observacao).HasDefaultValue(string.Empty);
        builder.Property(e => e.DataLimite).IsRequired();
        builder.Property(e => e.Status).IsRequired();
        builder.Property(e => e.StartedAt);
        builder.Property(e => e.CompletedAt);

        builder.HasOne<Funcao>("Funcao").WithMany().OnDelete(DeleteBehavior.Restrict);

        // Map the collection navigation via the property, but use the backing field for access
        builder.HasMany(e => e.Artefatos)
            .WithOne()
            .HasForeignKey(a => a.EtapaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(e => e.Artefatos).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
