using CapituloZero.Domain.Editora.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CapituloZero.Infrastructure.Editora;

internal sealed class EtapaTemplateConfiguration : IEntityTypeConfiguration<EtapaTemplate>
{
    public void Configure(EntityTypeBuilder<EtapaTemplate> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Nome).IsRequired();
        builder.Property(e => e.Ordem).IsRequired();
        builder.Property(e => e.PrazoDias).HasDefaultValue(0);
        builder.Property(e => e.ObservacaoPadrao).HasDefaultValue(string.Empty);
        builder.HasOne(e => e.Funcao).WithMany().OnDelete(DeleteBehavior.Restrict);
    }
}
