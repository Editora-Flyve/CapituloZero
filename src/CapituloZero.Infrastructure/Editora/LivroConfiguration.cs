using CapituloZero.Domain.Editora.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CapituloZero.Infrastructure.Editora;

internal sealed class LivroConfiguration : IEntityTypeConfiguration<Livro>
{
    public void Configure(EntityTypeBuilder<Livro> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Titulo).IsRequired();
        builder.Property(l => l.Subtitulo).HasDefaultValue(string.Empty);
        builder.Property(l => l.CurrentEtapaIndex);
        builder.Property(l => l.DataInicio);
        builder.Property(l => l.DataConclusao);

        builder.HasOne(l => l.Autor)
            .WithMany(a => a.Livros)
            .HasForeignKey("AutorId");

        builder.HasMany(l => l.Etapas)
            .WithOne()
            .HasForeignKey(e => e.LivroId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(l => l.Etapas).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
