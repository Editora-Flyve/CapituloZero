using CapituloZero.Domain.Editora.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CapituloZero.Infrastructure.Editora;

internal sealed class ArtefatoConfiguration : IEntityTypeConfiguration<Artefato>
{
    public void Configure(EntityTypeBuilder<Artefato> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.FileUri).IsRequired();
        builder.Property(a => a.FileName).IsRequired();
        builder.Property(a => a.ContentType).IsRequired();
        builder.Property(a => a.SizeBytes);
        builder.Property(a => a.UploadedByUserId);
        builder.Property(a => a.UploadedAt).HasConversion(d => DateTime.SpecifyKind(d, DateTimeKind.Utc), v => v);
    }
}
