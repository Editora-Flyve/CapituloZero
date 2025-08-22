using CapituloZero.Domain.Todos;
using Microsoft.EntityFrameworkCore;

namespace CapituloZero.Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<TodoItem> TodoItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
