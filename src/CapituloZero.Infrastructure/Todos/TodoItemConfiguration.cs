using CapituloZero.Domain.Todos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CapituloZero.SharedKernel;

namespace CapituloZero.Infrastructure.Todos;

internal sealed class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.DueDate).HasConversion(d => d != null ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : d, v => v);

        // Map UserId (Value Object) <-> Guid (uuid) with conversion
        builder.Property(t => t.UserId)
            .HasConversion(id => (Guid)id, value => (UserId)value)
            .HasColumnType("uuid")
            .IsRequired();

    // Avoid direct FK to Identity user (cross-context). Keep an index for queries instead.
    builder.HasIndex(t => t.UserId);
    }
}
